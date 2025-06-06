using OpenDis.Dis1998;
using OpenDis.Core;
using System.ComponentModel;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Net;
using Npgsql;
using System.Diagnostics;
using System;

namespace LEAP_dis_manager
{
    //Generic UI TODOs
    //TODO: Add in text fields to display the current settings the user has applied. This will let the user see their settings without needing to open the Settings menu.
        //Could also display if the UDP socket is connected yet or not.

    public partial class MainForm : Form
    {
        private HashSet<UInt64> leapDISEntityTypes;

        private Settings settingsForm;

        private string receivingIpAddress;
        private int receivingPort;
        private string databaseIpAddress;
        private int databasePort;
        private bool isMulticast;
        private int exerciseID;

        private ConcurrentQueue<byte[]> receivedByteQueue;
        private UdpClient client;
        private IPEndPoint epReceive;

        private Thread receiveThread;
        private string LocalIPAddress;
        private bool isCancelled = false;
        private Exception exception;

        private BackgroundWorker listenWorker;

        public MainForm()
        {
            InitializeComponent();
            InitializeBackgroundWorker();
            LoadSettings();

            leapDISEntityTypes = new HashSet<UInt64>();
            //leapDISEntityTypes.Add(217018310884122881); //UInt64 value for F16 Vipers, received from debugging the EntityTypeToUInt64 function to get its return value -- Entity Type of 1.2.225.1.3.3.1

            //TODO: Read from database to initialize the leapDISEntityTypes hash set with acceptable DIS Entity Types
                //This should be the list of all unit types that are supported within LEAP
        }

        private void LoadSettings()
        {
            receivingIpAddress = Properties.Settings.Default.receivingIpAddress;
            receivingPort = Properties.Settings.Default.receivingPort;
            databaseIpAddress = Properties.Settings.Default.databaseIpAddress;
            databasePort = Properties.Settings.Default.databasePort;
            isMulticast = Properties.Settings.Default.isMulticast;
            exerciseID = Properties.Settings.Default.exerciseID;
        }

        public void SaveSettings(string newReceivingIpAddress, int newReceivingPort, string newDatabaseIpAddress, int newDatabasePort, bool newIsMulticast, int newExerciseID)
        {
            receivingIpAddress = newReceivingIpAddress;
            receivingPort = newReceivingPort;
            databaseIpAddress = newDatabaseIpAddress;
            databasePort = newDatabasePort;
            isMulticast = newIsMulticast;
            exerciseID = newExerciseID;
        }

        private void openSettings(object sender, EventArgs e)
        {
            if (settingsForm == null || settingsForm.IsDisposed)        //Checks to see if settings are not found/disposed of and creates a new instance of the Settings form if so
            {
                settingsForm = new Settings(this, receivingIpAddress, receivingPort, databaseIpAddress, databasePort, isMulticast, exerciseID);
            }

            settingsForm.Show();
        }

        public void beginReceiving()
        {
            isCancelled = true;
            exception = null;
            receiveThread = new Thread(new ThreadStart(BeginReceive));
            receiveThread.Start();
            receiveThread.Join();
            if (exception != null) { MessageBox.Show("Failed to connect receive socket!"); }
        }

        public void stopReceiving()
        {
            isCancelled = true;
            client?.Close();
            //If the receive thread is still awaiting joining back with the main thread, interrupt it.
            receiveThread.Interrupt();
            SetButtonState(false);
        }

        private void BeginReceive()
        {
            try
            {
                receivedByteQueue = new ConcurrentQueue<byte[]>();
                //Default to receive from any IP address
                IPAddress receivingIp = IPAddress.Any;

                //If user input 0.0.0.0 as the IP, let that represent any IP address
                if (receivingIpAddress != "0.0.0.0")
                {
                    //Otherwise only receive from a specific IP address
                    receivingIp = IPAddress.Parse(receivingIpAddress);
                }

                epReceive = new IPEndPoint(receivingIp, receivingPort);

                //Make the receive socket non-binding to make the IP Endpoint reusable
                client = new UdpClient();
                client.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
                client.ExclusiveAddressUse = false;
                client.Client.Bind(epReceive);

                //TODO: Actually test multicast
                if (isMulticast)
                {
                    //TODO: Uncomment below code if/when receiving loopback network traffic becomes a togglable option
                    //Setup multicast loopback if enabled.
                    //client.MulticastLoopback = allowLoopback;

                    epReceive = new IPEndPoint(IPAddress.Any, receivingPort);
                    client.JoinMulticastGroup(IPAddress.Parse(receivingIpAddress));
                }
                client.BeginReceive(new AsyncCallback(asyncReceive), client);
            }
            catch (SocketException ex)
            {
                exception = ex;
                stopReceiving();
            }
        }

        private void asyncReceive(IAsyncResult result)
        {
            if (!isCancelled)
            {
                byte[] receivedBytes = client.EndReceive(result, ref epReceive);

                //TODO: Uncomment below code if/when receiving loopback network traffic becomes a togglable option
                //Ignore packets from self if loopback is disabled. This will cover ignoring broadcast packets. Ignoring multicast packets is covered in setting up of the receive socket above through MulticastLoopback.
                /*if (!allowLoopback && epReceive.Address.ToString().Equals(LocalIPAddress))
                {
                    return;
                }*/

                if (receivedBytes.Length > 1)
                {
                    //Write the new received message to the queue
                    receivedByteQueue.Enqueue(receivedBytes);
                }

                client.BeginReceive(new AsyncCallback(asyncReceive), client);
            }
        }

        private void InitializeBackgroundWorker()
        {
            if (listenWorker != null)
            {
                listenWorker.Dispose();
            }

            listenWorker = new BackgroundWorker
            {
                WorkerReportsProgress = true,
                WorkerSupportsCancellation = true
            };

            listenWorker.DoWork += new DoWorkEventHandler(listenWorker_DoWork);
            listenWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(listenWorker_RunWorkerCompleted);
        }


        private void listenWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker worker = sender as BackgroundWorker;

            while (!worker.CancellationPending)
            {
                if (receivedByteQueue.TryDequeue(out byte[] receivedBytes))
                {

                    Pdu pdu = PduProcessor.ConvertByteArrayToPdu1998(receivedBytes[2], receivedBytes, Endian.Big);

                    //Using values less than 1 to represent any exercise ID -- No reason for this, just the way we decided..
                    if (pdu.ExerciseID == exerciseID || exerciseID < 1)
                    {
                        switch (pdu)
                        {
                            //Verify the received PDU is an ESPDU
                            case EntityStatePdu espdu:
                                //Verify that the entity described in the PDU is supported by LEAP
                                //NOTE: Here we only care about the Entity Type. The entity type is what determines the type of unit (ex: F-16, F-22, INF, etc.). This will filter unit types.
                                if (leapDISEntityTypes.Contains(EntityTypeToUInt64(espdu.EntityType)))
                                {
                                    /*Debug.WriteLine(Syste0m.Text.Encoding.UTF8.GetString(espdu.Marking.Characters, 0, espdu.Marking.Characters.Length));
                                    Debug.WriteLine("");*/
                                    sendToDatabase(databaseIpAddress, databasePort, espdu);
                                }
                                break;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Converts an Entity Type to a UInt64. Used for easy comparisons between Entity Types.
        /// </summary>
        /// <param name="entityType">The Entity Type to convert.</param>
        /// <returns>The UInt64 representation of the Entity Type.</returns>
        public static UInt64 EntityTypeToUInt64(EntityType entityType)
        {
            byte category = entityType.Category;
            ushort country = entityType.Country;
            byte domain = entityType.Domain;
            byte entityKind = entityType.EntityKind;
            byte extra = entityType.Extra;
            byte specific = entityType.Specific;
            byte subcategory = entityType.Subcategory;

            UInt64 entityType64 = ((UInt64)subcategory << 56) | ((UInt64)specific << 48) | ((UInt64)extra << 40) | ((UInt64)entityKind << 32) |
                                  ((UInt64)domain << 24) | ((UInt64)country << 8) | (UInt64)category;
            
            //For debug
            //string binary = UInt64ToString(entityType64);

            return entityType64;
        }

        /// <summary>
        /// Send a message to the database notifying it of the received DIS entity. This will add or update an entry based on if it is a new entity or not.
        /// </summary>
        /// <param name="databaseIp">IP of the database.</param>
        /// <param name="databasePort">Port of the database.</param>
        /// <param name="entity">Entity State PDU containing info of the DIS entity.</param>
        private void sendToDatabase(string databaseIp, int databasePort, EntityStatePdu entity)
        {
            //NOTE: Here we care about the Entity ID. The Entity ID is a unique ID for each entity in the sim (i.e. it points to a specific unit). This will represent each LEAP unit.
            string userId = "postgres";
            string password = "postgres";
            string databaseName = "LEAP";
            string connectionString = $"Host={databaseIp};Port={databasePort};Database={databaseName};User Id={userId};Password={password};";

            try
            {
                using NpgsqlConnection conn = new NpgsqlConnection(connectionString);
                conn.Open();

                //Check the database to see if a unit with the given Entity ID already exists
                string query = "SELECT COUNT(*) FROM units WHERE id = @entityId";
                using NpgsqlCommand checkCommand = new NpgsqlCommand(query, conn);
                checkCommand.Parameters.AddWithValue("@entityId", entity.EntityID);

                int count = Convert.ToInt32(checkCommand.ExecuteScalar());
                
                if (count == 0)
                {
                    //If a unit with the given Entity ID does not exist in the database, add it
                    bool allegiance = (entity.ForceId == 1);
                   
                    string insertQuery = "INSERT INTO UNITS (id, isFriendly, unit_id, xcord, ycord, zcord) VALUES (@entityID, @allegiance, @entityMarking, @xcord, @ycord, @zcord)";
                    using NpgsqlCommand insertCommand = new NpgsqlCommand(insertQuery, conn);
                    insertCommand.Parameters.AddWithValue("@entityId", entity.EntityID);
                    insertCommand.Parameters.AddWithValue("@allegiance", allegiance);
                    insertCommand.Parameters.AddWithValue("@entityMarking", entity.Marking);
                    insertCommand.Parameters.AddWithValue("@xcord", entity.EntityLocation.X);
                    insertCommand.Parameters.AddWithValue("@ycord", entity.EntityLocation.Y);
                    insertCommand.Parameters.AddWithValue("@zcord", entity.EntityLocation.Z);

                    //Checks to see if the Entity ID was inserted successfully, should return 1 if successful
                    int rowsAffected = insertCommand.ExecuteNonQuery();
                    if (rowsAffected > 0)
                    {
                        Console.WriteLine("EntityID inserted successfully.");
                    }
                    else
                    {
                        Console.WriteLine("Failed to insert EntityID.");
                    }
                }
                else
                {
                    //If a unit with the given Entity ID exists in the database, update it
                    string updateQuery = "UPDATE UNITS SET xcord = @xcord, ycord = @ycord, zcord = @zcord WHERE id = @entityID";
                    using NpgsqlCommand updateCommand = new NpgsqlCommand(updateQuery, conn);

                    updateCommand.Parameters.AddWithValue("@entityId", entity.EntityID);
                    updateCommand.Parameters.AddWithValue("@xcord", entity.EntityLocation.X);
                    updateCommand.Parameters.AddWithValue("@ycord", entity.EntityLocation.Y);
                    updateCommand.Parameters.AddWithValue("@zcord", entity.EntityLocation.Z);

                    int rowsAffected = updateCommand.ExecuteNonQuery();
                    if (rowsAffected > 0)
                    {
                        Console.WriteLine("EntityID inserted successfully.");
                    }
                    else
                    {
                        Console.WriteLine("Failed to insert EntityID.");
                    }
                }

            }
            catch (NpgsqlException npgsqlEx)
            {
                MessageBox.Show($"PostgreSQL Error: {npgsqlEx.Message}");
                Console.WriteLine(npgsqlEx.ToString());
            }
            catch (SocketException socketEx)
            {
                MessageBox.Show($"Socket Error: {socketEx.Message}");
                Console.WriteLine(socketEx.ToString());
            }
            catch (Exception ex)
            {
                MessageBox.Show($"General Error: {ex.Message}");
                Console.WriteLine(ex.ToString());
            }
        }

        private void listenWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                // Handle any errors that occurred during the background operation
                MessageBox.Show($"Error: {e.Error.Message}");
            }
            else if (e.Cancelled)
            {
                // Handle cancellation
                MessageBox.Show("Operation was cancelled.");
            }
            else
            {
                // Access the result from e.Result
            }
        }

        private void close_program(object sender, EventArgs e)
        {
            this.Close();
        }

        private void run(object sender, EventArgs e)
        {
            if (Start_button.Text == "Start")
            {
                if (!runFuncts.runFuncts.validateRunParams(receivingIpAddress, receivingPort, databaseIpAddress, databasePort))
                {
                    return;
                }
                else
                {
                    //Separate receiving PDUs and parsing PDUs onto separate threads
                    beginReceiving();
                    if (!listenWorker.IsBusy)
                    {
                        listenWorker.RunWorkerAsync();
                        SetButtonState(true);
                    }
                }
            }
            else if (Start_button.Text == "Stop")
            {
                stopReceiving();
                if (listenWorker.WorkerSupportsCancellation == true)
                {
                    listenWorker.CancelAsync();
                }
            }
        }

        private void SetButtonState(bool isRunning)
        {
            if (isRunning)
            {
                Start_button.Text = "Stop";
            }
            else
            {
                Start_button.Text = "Start";
            }
        }
    }
}
