using Npgsql;
using OpenDis.Core;
using OpenDis.Dis1998;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;

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
        private string sectionID = "";
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

            //Manually adding all the entity IDs. 
            //TODO: change it so that the erns(entityIDs) are pulled from the db
            leapDISEntityTypes = new HashSet<UInt64>();
            leapDISEntityTypes.Add(10992);
            leapDISEntityTypes.Add(10993);
            leapDISEntityTypes.Add(10994);
            leapDISEntityTypes.Add(11002);
            leapDISEntityTypes.Add(11003);
            leapDISEntityTypes.Add(11011);
            leapDISEntityTypes.Add(1012);
            leapDISEntityTypes.Add(11013);
            leapDISEntityTypes.Add(11014);
            leapDISEntityTypes.Add(11015);
            leapDISEntityTypes.Add(11016);
            leapDISEntityTypes.Add(11018);
            leapDISEntityTypes.Add(11019);
            leapDISEntityTypes.Add(11020);
            leapDISEntityTypes.Add(11021);
            leapDISEntityTypes.Add(11027);
            leapDISEntityTypes.Add(11028);
            leapDISEntityTypes.Add(11029);
            leapDISEntityTypes.Add(11030);
            leapDISEntityTypes.Add(11037);
            leapDISEntityTypes.Add(11039);
            leapDISEntityTypes.Add(11040);
            leapDISEntityTypes.Add(11046);
            leapDISEntityTypes.Add(11047);
            leapDISEntityTypes.Add(11048);
            leapDISEntityTypes.Add(11253);
            leapDISEntityTypes.Add(11200);
            leapDISEntityTypes.Add(11201);
            leapDISEntityTypes.Add(11202);
            leapDISEntityTypes.Add(11203);
            leapDISEntityTypes.Add(11204);
            leapDISEntityTypes.Add(11205);
            leapDISEntityTypes.Add(11206);
            leapDISEntityTypes.Add(11207);
            leapDISEntityTypes.Add(11208);
            leapDISEntityTypes.Add(11209);
            leapDISEntityTypes.Add(11210);
            leapDISEntityTypes.Add(11211);
            leapDISEntityTypes.Add(11212);
            leapDISEntityTypes.Add(11213);
            leapDISEntityTypes.Add(11214);
            leapDISEntityTypes.Add(11215);
            leapDISEntityTypes.Add(11216);
            leapDISEntityTypes.Add(11217);
            leapDISEntityTypes.Add(11218);
            leapDISEntityTypes.Add(11219);
            leapDISEntityTypes.Add(11220);
            leapDISEntityTypes.Add(11221);
            leapDISEntityTypes.Add(11222);
            leapDISEntityTypes.Add(11223);
            leapDISEntityTypes.Add(11224);
            //leapDISEntityTypes.Add(723391694654364940);
            //leapDISEntityTypes.Add(217018310884122881);
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
            //sectionID = Properties.Settings.Default.sectionID;
        }

        public void SaveSettings(string newReceivingIpAddress, int newReceivingPort, string newDatabaseIpAddress, int newDatabasePort, bool newIsMulticast, int newExerciseID, string newSectionID)
        {
            receivingIpAddress = newReceivingIpAddress;
            receivingPort = newReceivingPort;
            databaseIpAddress = newDatabaseIpAddress;
            databasePort = newDatabasePort;
            isMulticast = newIsMulticast;
            exerciseID = newExerciseID;
            sectionID = newSectionID;
        }

        private void openSettings(object sender, EventArgs e)
        {
            if (settingsForm == null || settingsForm.IsDisposed)
            {
                settingsForm = new Settings(this, receivingIpAddress, receivingPort, databaseIpAddress, databasePort, isMulticast, exerciseID, sectionID);
            }

            settingsForm.Show();
        }
        /// <summary>
        /// 
        /// </summary>
        public void beginReceiving()
        {
            isCancelled = false;
            exception = null;
            BeginReceive();
            //receiveThread = new Thread(new ThreadStart(BeginReceive));
            //receiveThread.Start();
            //receiveThread.Join();
            if (exception != null) { MessageBox.Show("Failed to connect receive socket!"); }
        }

        public void stopReceiving()
        {
            isCancelled = true;
            client?.Close();
            //If the receive thread is still awaiting joining back with the main thread, interrupt it.
            //receiveThread.Interrupt();
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
                //client.ExclusiveAddressUse = false;
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
;
                client.BeginReceive(new AsyncCallback(asyncReceive), null);
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
                    if (pdu.ExerciseID == exerciseID || exerciseID == 0)
                    {
                        switch (pdu)
                        {
                            //Verify the received PDU is an ESPDU
                            case EntityStatePdu espdu:
                                
                                //Verify that the entity described in the PDU is supported by LEAP
                                //NOTE: Here we only care about the Entity Type. The entity type is what determines the type of unit (ex: F-16, F-22, INF, etc.). This will filter unit types.
                                if (leapDISEntityTypes.Contains(UInt64.Parse(espdu.EntityID.Entity.ToString())))
                                    

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
                byte[] markingCharacters = entity.Marking.Characters;
                int nullIndex = Array.IndexOf(markingCharacters, (byte)0);
                string unit_name = System.Text.Encoding.Default.GetString(markingCharacters, 0, nullIndex);
                int length = (unit_name.Length < 12) ? unit_name.Length : 11;

                unit_name = unit_name.Substring(0, length);
                int unit_ern = entity.EntityID.Entity;
                int application_id = entity.EntityID.Application;
                int site_id = entity.EntityID.Site;
                using NpgsqlConnection conn = new NpgsqlConnection(connectionString);
                conn.Open();
                
                //Check the database to see if a unit with the given Entity ID already exists
                //this assumes that we use appplication_id and site_id to distinguish between different entities
                string query = "SELECT COUNT(*) FROM dis WHERE section_id = @section_id AND unit_name = @unit_name";
                using NpgsqlCommand checkCommand = new NpgsqlCommand(query, conn);


                //sectionID is a global variable
                checkCommand.Parameters.AddWithValue("@section_id", sectionID);
                checkCommand.Parameters.AddWithValue("@unit_name", unit_name);


                int count = Convert.ToInt32(checkCommand.ExecuteScalar());
                
                if (count == 0)
                {
                    //If a unit with the given Entity ID does not exist in the database, add it
                    
                    string insertQuery = "INSERT INTO dis (unit_name, section_id, unit_ern, application_id, site_id, xcord, ycord, zcord) VALUES (@unit_name, @section_id, @unit_ern , @application_id, @site_id, @xcord, @ycord, @zcord)";
                    using NpgsqlCommand insertCommand = new NpgsqlCommand(insertQuery, conn);
                    insertCommand.Parameters.AddWithValue("@unit_name", unit_name);
                    //sectionID is a global variable
                    insertCommand.Parameters.AddWithValue("@section_id", sectionID);

                    insertCommand.Parameters.AddWithValue("@unit_ern", unit_ern);
                    insertCommand.Parameters.AddWithValue("@application_id", application_id);
                    insertCommand.Parameters.AddWithValue("@site_id", site_id);




                    insertCommand.Parameters.AddWithValue("@xcord", entity.EntityLocation.X);
                    insertCommand.Parameters.AddWithValue("@ycord", entity.EntityLocation.Y);
                    insertCommand.Parameters.AddWithValue("@zcord", entity.EntityLocation.Z);


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
                    string updateQuery = "UPDATE dis SET unit_name = @unit_name, section_id = @section_id, unit_ern = @unit_ern, application_id = @application_id, site_id = @site_id, xcord = @xcord, ycord = @ycord, zcord =  @zcord" +
                        " WHERE section_id = @section_id AND unit_name = @unit_name ";
                    using NpgsqlCommand updateCommand = new NpgsqlCommand(updateQuery, conn);



                    updateCommand.Parameters.AddWithValue("@unit_name", unit_name);
                    //sectionID is a global variable
                    updateCommand.Parameters.AddWithValue("@section_id", sectionID);
                    updateCommand.Parameters.AddWithValue("@unit_ern",unit_ern);
                    updateCommand.Parameters.AddWithValue("@application_id", application_id);
                    updateCommand.Parameters.AddWithValue("@site_id", site_id);




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

                if (sectionID == "")
                {
                    MessageBox.Show("Please input a section ID");
                    return;
                }

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
        private void sectionIDTextBox_TextChanged(object sender, EventArgs e)
        {
            sectionID = sectionIDTextBox.Text;
        }
    }
}
