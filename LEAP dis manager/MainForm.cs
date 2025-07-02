// Required namespaces for DIS processing, networking, multithreading, PostgreSQL access, and UI handling
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
        // Fields for DIS filtering, settings, and data processing
        private HashSet<UInt64> leapDISEntityTypes;
        private Settings settingsForm;
        private ScenarioInputForm newScenario;
        private string receivingIpAddress;
        private int receivingPort;
        private string databaseIpAddress;
        private int databasePort;
        private bool isMulticast;
        private int exerciseID;
        private string sectionID = ""; // Active scenario ID
        private int siteID;

        // Thread-safe queue to hold received UDP packets
        private ConcurrentQueue<byte[]> receivedByteQueue;
        private ConcurrentQueue<RemoveEntityPdu> removeEntityPduQueue;

        // UDP Client and related networking objects
        private UdpClient client;
        private IPEndPoint epReceive;
        private UdpClient sendClient;
        private IPEndPoint epSend;

        // Thread to manage UDP receiving
        private Thread receiveThread;
        private string LocalIPAddress;

        // Control flags
        private bool isCancelled = false;
        private Exception exception;

        // BackgroundWorker to process packets
        private BackgroundWorker listenWorker; // Background processor for parsing packets
        private BackgroundWorker sendWorker; //Bkacground processer for sending dis packets

        // Names of unit types supported by LEAP (from database)
        private HashSet<string> supportedEntityTypeNames;

        // Dictionary to track last received timestamps per unit
        private Dictionary<string, string> entityTimestamps;

        // Timeout detection
        private System.Windows.Forms.Timer entityTimeoutTimer;
        private DateTime lastEntityReceivedTime;
        public MainForm()
        {
            InitializeComponent();

            // Configure background worker and load settings
            SetupWorkers();

            LoadSettings();

            // Load valid unit names from database
            FetchSupportedEntityNames();

            // Initialize UI and internal data structures
            EntityUpdateLabel.Text = "No entities received yet";
            entityTimestamps = new Dictionary<string, string>();

            // Timer setup to check entity timeout
            entityTimeoutTimer = new System.Windows.Forms.Timer();
            entityTimeoutTimer.Interval = 10 * 1000; // Checks every 10 seconds
            lastEntityReceivedTime = DateTime.Now;
            entityTimeoutTimer.Tick += CheckEntityTimeout;// // Subscribe event

        }

        // Loads all valid unit names from the database into supportedEntityTypeNames
        private void FetchSupportedEntityNames()
        {
            supportedEntityTypeNames = new HashSet<string>();

            string userId = "postgres";
            string password = "postgres";
            string databaseName = "LEAP";
            string connectionString = $"Host={databaseIpAddress};Port={databasePort};Database={databaseName};User Id={userId};Password={password};";

            try
            {
                using var conn = new NpgsqlConnection(connectionString);
                conn.Open();

                string query = "SELECT unit_name FROM preset_units";
                using var cmd = new NpgsqlCommand(query, conn);
                using var reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    string name = reader.GetString(0).Trim();
                    if (!string.IsNullOrEmpty(name))
                        supportedEntityTypeNames.Add(name);
                }

                Console.WriteLine($"Loaded {supportedEntityTypeNames.Count} supported entity names.");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading entity names: {ex.Message}");
            }
        }

        // Load persisted settings into variables and UI
        private void LoadSettings()
        {
            receivingIpAddress = Properties.Settings.Default.receivingIpAddress;
            receivingPort = Properties.Settings.Default.receivingPort;
            databaseIpAddress = Properties.Settings.Default.databaseIpAddress;
            databasePort = Properties.Settings.Default.databasePort;
            isMulticast = Properties.Settings.Default.isMulticast;
            exerciseID = Properties.Settings.Default.exerciseID;
            siteID = Properties.Settings.Default.siteID;
            siteIdUpDown.Value = siteID;
            //sectionID = Properties.Settings.Default.sectionID;
        }

        // Save updated runtime settings back to internal state
        public void SaveSettings(string newReceivingIpAddress, int newReceivingPort, string newDatabaseIpAddress, int newDatabasePort, bool newIsMulticast, int newExerciseID)
        {
            receivingIpAddress = newReceivingIpAddress;
            receivingPort = newReceivingPort;
            databaseIpAddress = newDatabaseIpAddress;
            databasePort = newDatabasePort;
            isMulticast = newIsMulticast;
            exerciseID = newExerciseID;
        }

        // Open the Settings form
        private void openSettings(object sender, EventArgs e)
        {
            if (settingsForm == null || settingsForm.IsDisposed)
            {
                settingsForm = new Settings(this, receivingIpAddress, receivingPort, databaseIpAddress, databasePort, isMulticast, exerciseID);
            }

            settingsForm.Show();
        }


        // Start UDP receiving
        public void initializeUDPReceiverAndSender()
        {
            isCancelled = false;
            exception = null;
            setupReceiverUDPClient(); // Setup the UDP client and begin receiving asyncoronously
            setUpSenderUDPClient();
            if (exception != null) { MessageBox.Show("Failed to connect receive socket!"); }
        }

        // Stop UDP receiving and background worker
        public void stopReceiving()
        {
            isCancelled = true;
            client?.Close();

            UpdateRunButtonText(false);  // Update UI to reflect stopped state
        }

        private void stopSending()
        {
            client?.Close();
        }
        // Sets up the UDP client socket and begins asynchronous receiving
        private void setupReceiverUDPClient()
        {
            try
            {
                receivedByteQueue = new ConcurrentQueue<byte[]>();
                //Default to receive from any IP address
                IPAddress receivingIp = IPAddress.Any;

                // Use specific IP address if not set to "0.0.0.0"
                if (receivingIpAddress != "0.0.0.0")
                {
                    //Otherwise only receive from a specific IP address
                    receivingIp = IPAddress.Parse(receivingIpAddress);
                }

                epReceive = new IPEndPoint(receivingIp, receivingPort); // Create endpoint to bind

                // Create UDP client and set socket options for address reuse
                client = new UdpClient();
                client.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);

                // Bind the client to the specified endpoint
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
                // Begin asynchronous receive operation
                client.BeginReceive(new AsyncCallback(ReceiveUdpPacketAsync), null);
            }
            catch (SocketException ex)
            {
                exception = ex;
                stopReceiving();
            }
        }

        private void setUpSenderUDPClient()
        {
            try
            {
                removeEntityPduQueue = new ConcurrentQueue<RemoveEntityPdu>();
                IPAddress sendIp = IPAddress.Broadcast;
                epSend = new IPEndPoint(sendIp, 3000);
                sendClient = new UdpClient();
                sendClient.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
            }
            catch (SocketException ex)
            {
                exception = ex;
                //TODO: Create a method for stopSending
                sendClient?.Close();
            }
        }
        // Checks if no entities have been received in the last 5 minutes and updates the UI
        private void CheckEntityTimeout(object sender, EventArgs e)
        {
            if ((DateTime.Now - lastEntityReceivedTime).TotalMinutes >= 5)
            {
                // Update label to indicate no recent entity activity
                EntityUpdateLabel.Text = "No entities received in 5 minutes";
                EntityUpdateLabel.ForeColor = Color.Red;
            }
        }
        // Asynchronous callback for receiving UDP packets
        private void ReceiveUdpPacketAsync(IAsyncResult result)
        {
            if (!isCancelled)
            {
                // Complete the receive operation and get the packet
                byte[] receivedBytes = client.EndReceive(result, ref epReceive);


                // Only process packets with more than 1 byte
                if (receivedBytes.Length > 1)
                {
                    // Add the received packet to the queue for background processing
                    receivedByteQueue.Enqueue(receivedBytes);
                }
                // Continue listening for more packets
                client.BeginReceive(new AsyncCallback(ReceiveUdpPacketAsync), client);
            }
        }

        // Configures the background worker that processes received PDUs
        private void SetupWorkers()
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

            sendWorker = new BackgroundWorker
            {
                WorkerReportsProgress = true,
                WorkerSupportsCancellation = true
            };
            sendWorker.DoWork += new DoWorkEventHandler(sendWorker_DoWork);
            listenWorker.DoWork += new DoWorkEventHandler(listenWorker_DoWork);
            listenWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(listenWorker_RunWorkerCompleted);
        }

        // Configures the background worker that sends PDUs


        // The core background loop that processes received PDUs from the queue
        private void listenWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker worker = sender as BackgroundWorker;

            while (!worker.CancellationPending)
            {
                // Try to get a packet from the queue
                if (receivedByteQueue.TryDequeue(out byte[] receivedBytes))
                {
                    // Convert the raw byte array into a PDU using the OpenDIS library
                    Pdu pdu = PduProcessor.ConvertByteArrayToPdu1998(receivedBytes[2], receivedBytes, Endian.Big);

                    // Only process PDUs matching the specified exercise ID (or 0 for all)
                    if (pdu.ExerciseID == exerciseID || exerciseID == 0)
                    {
                        switch (pdu)
                        {
                            // Only process Entity State PDUs
                            case EntityStatePdu espdu:
                                int disSiteID = espdu.EntityID.Site;

                                // Extract null-terminated marking (unit name) from PDU
                                byte[] markingCharacters = espdu.Marking.Characters;
                                int nullIndex = Array.IndexOf(markingCharacters, (byte)0);
                                int byteLength = (nullIndex >= 0) ? nullIndex : markingCharacters.Length; // If no null, read all 11
                                string unit_name = System.Text.Encoding.Default.GetString(markingCharacters, 0, byteLength);
                                int length = (unit_name.Length < 12) ? unit_name.Length : 11;

                                unit_name = unit_name.Substring(0, length);

                                // Filter only supported entity types and valid site ID
                                if (supportedEntityTypeNames.Contains(unit_name) && disSiteID == siteID)


                                {

                                    // Send valid PDU to the database
                                    sendToDatabase(databaseIpAddress, databasePort, espdu);

                                }
                                break;
                        }
                    }
                }
            }
        }

        private void sendWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker sendWorker = sender as BackgroundWorker;

            string userId = "postgres";
            string password = "postgres";
            string databaseName = "LEAP";
            string connectionString = $"Host={databaseIpAddress};Port={databasePort};Database={databaseName};User Id={userId};Password={password};";

            try
            {
                using var conn = new NpgsqlConnection(connectionString);
                conn.Open();

                // Subscribe to PostgreSQL notifications on the "unit_dead" channel
                using var cmd = new NpgsqlCommand("LISTEN unit_dead;", conn);
                cmd.ExecuteNonQuery();

                conn.Notification += (o, e) =>
                {
                    var payload = e.Payload;

                    // Parse JSON string to object
                    var json = System.Text.Json.JsonDocument.Parse(payload).RootElement;

                    string unitName = json.GetProperty("unit_name").GetString();
                    string sectionId = json.GetProperty("section_id").GetString();
                    int ern = json.GetProperty("unit_ern").GetInt32();
                    int siteId = json.GetProperty("site_id").GetInt32();

                    try
                    {
                        //TODO: Convert the received database information into the RemoveEntityPDU
                        RemoveEntityPdu removeEntityPdu = new RemoveEntityPdu();
                        DataOutputStream dos = new DataOutputStream();
                        removeEntityPdu.Marshal(dos);
                        byte[] bytes = dos.ConvertToBytes();
                        sendClient.Send(bytes, bytes.Length, epSend);

                        Console.WriteLine("Sent RemoveEntityPdu");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Error sending PDU: " + ex.Message);
                    }
                };

                while (!sendWorker.CancellationPending)
                {
                    conn.Wait(); // Waits for NOTIFY event from DB
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error in ListenForUnitDeathsAsync: " + ex.Message);
            }
        }


        // Updates the DataGridView with the latest timestamps from entityTimestamps
        private void UpdateEntityTableFromTimestamps()
        {
            // === BEFORE refresh: save UI state ===
            int firstDisplayedRowIndex = dataGridView.FirstDisplayedScrollingRowIndex;
            int selectedRowIndex = dataGridView.CurrentRow?.Index ?? -1;

            // Clear and repopulate DataGridView with updated entity timestamps
            dataGridView.Rows.Clear();
            foreach (var entity in entityTimestamps)
            {
                dataGridView.Rows.Add(entity.Key, entity.Value);
            }
            // === AFTER refresh: restore UI state ===
            if (dataGridView.RowCount > 0 && firstDisplayedRowIndex >= 0 && firstDisplayedRowIndex < dataGridView.RowCount)
            {
                dataGridView.FirstDisplayedScrollingRowIndex = firstDisplayedRowIndex;
            }

            if (selectedRowIndex >= 0 && selectedRowIndex < dataGridView.RowCount)
            {
                dataGridView.Rows[selectedRowIndex].Selected = true;
            }
        }

        // Sends EntityStatePdu data to the PostgreSQL database, either inserting or updating a unit entry
        private void sendToDatabase(string databaseIp, int databasePort, EntityStatePdu entity)
        {
            // Database connection parameters
            string userId = "postgres";
            string password = "postgres";
            string databaseName = "LEAP";
            string connectionString = $"Host={databaseIp};Port={databasePort};Database={databaseName};User Id={userId};Password={password};";

            try
            {
                // Extract unit name from PDU marking (null-terminated string)
                byte[] markingCharacters = entity.Marking.Characters;
                int nullIndex = Array.IndexOf(markingCharacters, (byte)0);
                int byteLength = (nullIndex >= 0) ? nullIndex : markingCharacters.Length; // If no null, read all 11
                string unit_name = System.Text.Encoding.Default.GetString(markingCharacters, 0, byteLength);
                int length = (unit_name.Length < 12) ? unit_name.Length : 11;

                unit_name = unit_name.Substring(0, length);
                int unit_ern = entity.EntityID.Entity;
                int application_id = entity.EntityID.Application;
                int site_id = entity.EntityID.Site;
                using NpgsqlConnection conn = new NpgsqlConnection(connectionString);
                conn.Open();

                // Check if the unit already exists in the `dis` table
                string query = "SELECT COUNT(*) FROM dis WHERE section_id = @section_id AND unit_name = @unit_name";
                using NpgsqlCommand checkCommand = new NpgsqlCommand(query, conn);


                //sectionID is a global variable
                checkCommand.Parameters.AddWithValue("@section_id", sectionID);
                checkCommand.Parameters.AddWithValue("@unit_name", unit_name);


                int count = Convert.ToInt32(checkCommand.ExecuteScalar());

                if (count == 0)
                {
                    // INSERT new unit if not found
                    string insertQuery = "INSERT INTO dis (unit_name, section_id, unit_ern, application_id, site_id, xcord, ycord, zcord) VALUES (@unit_name, @section_id, @unit_ern , @application_id, @site_id, @xcord, @ycord, @zcord)";
                    using NpgsqlCommand insertCommand = new NpgsqlCommand(insertQuery, conn);

                    insertCommand.Parameters.AddWithValue("@unit_name", unit_name);
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
                        // Update UI and local timestamps on success
                        this.Invoke(new Action(() =>
                        {
                            EntityUpdateLabel.Text = $"Last entity received at {DateTime.Now:HH:mm:ss}";
                            lastEntityReceivedTime = DateTime.Now;
                            string timestamp = DateTime.Now.ToString("HH:mm:ss");
                            EntityUpdateLabel.ForeColor = SystemColors.ControlText;

                            if (entityTimestamps.ContainsKey(unit_name))
                            {
                                entityTimestamps[unit_name] = timestamp;
                            }
                            else
                            {
                                entityTimestamps.Add(unit_name, timestamp);
                            }
                            // Optional: update DataGridView from the dictionary
                            UpdateEntityTableFromTimestamps();
                        }));
                        Console.WriteLine("EntityID inserted successfully.");
                    }
                    else
                    {
                        Console.WriteLine("Failed to insert EntityID.");
                    }
                }
                else
                {
                    // UPDATE existing unit if found
                    string updateQuery = "UPDATE dis SET unit_name = @unit_name, section_id = @section_id, unit_ern = @unit_ern, application_id = @application_id, site_id = @site_id, xcord = @xcord, ycord = @ycord, zcord =  @zcord" +
                        " WHERE section_id = @section_id AND unit_name = @unit_name ";
                    using NpgsqlCommand updateCommand = new NpgsqlCommand(updateQuery, conn);

                    updateCommand.Parameters.AddWithValue("@unit_name", unit_name);
                    updateCommand.Parameters.AddWithValue("@section_id", sectionID);
                    updateCommand.Parameters.AddWithValue("@unit_ern", unit_ern);
                    updateCommand.Parameters.AddWithValue("@application_id", application_id);
                    updateCommand.Parameters.AddWithValue("@site_id", site_id);
                    updateCommand.Parameters.AddWithValue("@xcord", entity.EntityLocation.X);
                    updateCommand.Parameters.AddWithValue("@ycord", entity.EntityLocation.Y);
                    updateCommand.Parameters.AddWithValue("@zcord", entity.EntityLocation.Z);

                    int rowsAffected = updateCommand.ExecuteNonQuery();
                    if (rowsAffected > 0)
                    {
                        this.Invoke(new Action(() =>
                        {
                            // Update UI and local timestamps on success
                            lastEntityReceivedTime = DateTime.Now;
                            EntityUpdateLabel.Text = $"Last entity received at {DateTime.Now:HH:mm:ss}";
                            string timestamp = DateTime.Now.ToString("HH:mm:ss");
                            EntityUpdateLabel.ForeColor = SystemColors.ControlText;
                            if (entityTimestamps.ContainsKey(unit_name))
                            {
                                entityTimestamps[unit_name] = timestamp;
                            }
                            else
                            {
                                entityTimestamps.Add(unit_name, timestamp);
                            }
                            // Optional: update DataGridView from the dictionary
                            UpdateEntityTableFromTimestamps();
                        }));
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


        // Handles the completion of the BackgroundWorker (e.g. after cancel or error)
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

        private void sendWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {

        }

        // Handles UI logic for closing the program
        private void close_program(object sender, EventArgs e)
        {
            this.Close();
        }

        // Main run/start logic for the system (starts or stops UDP and worker threads)
        private void run(object sender, EventArgs e)
        {

            if (Start_button.Text == "Start")
            {
                // Validate section input
                if (sectionID == "")
                {
                    MessageBox.Show("Please input a section ID");
                    return;
                }

                // Validate network/database parameters
                if (!runFuncts.runFuncts.validateRunParams(receivingIpAddress, receivingPort, databaseIpAddress, databasePort))
                {
                    return;
                }
                else
                {
                    // Initialize UDP receiver and background worker
                    initializeUDPReceiverAndSender();
                    if (!listenWorker.IsBusy)
                    {
                        listenWorker.RunWorkerAsync();
                        UpdateRunButtonText(true);
                    }
                    if (!sendWorker.IsBusy)
                    {
                        sendWorker.RunWorkerAsync();
                    }
                }

                // Start timeout timer to monitor activity
                lastEntityReceivedTime = DateTime.Now;
                entityTimeoutTimer.Start();
            }
            else if (Start_button.Text == "Stop")
            {
                // Stop receiving and worker thread
                stopReceiving();
                stopSending();
                if (listenWorker.WorkerSupportsCancellation == true)
                {
                    listenWorker.CancelAsync();
                }

                if (sendWorker.WorkerSupportsCancellation == true)
                {
                    sendWorker.CancelAsync();
                }
                entityTimeoutTimer.Stop();
            }
        }

        // Updates the Start/Stop button text based on current state
        private void UpdateRunButtonText(bool isRunning)
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

        // Updates the siteID value and persists it to user settings
        private void siteIdUpDown_ValueChanged(object sender, EventArgs e)
        {
            siteID = (int)siteIdUpDown.Value;
            Properties.Settings.Default.siteID = siteID;
            Properties.Settings.Default.Save();
        }


        // Fetches all section IDs from the database and populates the dropdown list
        private void FetchSectionIDsForDropdown()
        {
            List<string> sectionIDs = new List<string>();

            string userId = "postgres";
            string password = "postgres";
            string databaseName = "LEAP";
            string connectionString = $"Host={databaseIpAddress};Port={databasePort};Database={databaseName};User Id={userId};Password={password};";

            try
            {
                using var conn = new NpgsqlConnection(connectionString);
                conn.Open();

                // Query to get all section IDs from the 'sections' table
                string query = "SELECT sectionid FROM sections";
                using var cmd = new NpgsqlCommand(query, conn);
                using var reader = cmd.ExecuteReader();

                // Read section IDs from the result set
                while (reader.Read())
                {
                    string sectionID = reader.GetString(0).Trim();
                    if (!string.IsNullOrEmpty(sectionID))
                        sectionIDs.Add(sectionID);
                }

                // Clear the dropdown and repopulate with updated list
                sectionIdDropdown.Items.Clear();
                foreach (string id in sectionIDs)
                {
                    sectionIdDropdown.Items.Add(id);
                }
                Console.WriteLine($"Loaded {sectionIDs.Count} sectionIDs.");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading entity names: {ex.Message}");
            }
        }


        // Event handler for when the dropdown is clicked (refreshed dynamically before showing)
        private void comboBox1_DropDown(object sender, EventArgs e)
        {
            sectionIdDropdown.Items.Clear();
            FetchSectionIDsForDropdown();

        }


        // Event handler when user selects a different section ID from the dropdown
        private void OnSectionIdSelectionChanged(object sender, EventArgs e)
        {
            sectionID = sectionIdDropdown.SelectedItem.ToString();
        }



        // Opens a dialog window to create a new scenario (section)
        private void OpenNewScenarioDialog(object sender, EventArgs e)
        {
            newScenario = new ScenarioInputForm();  // Open form

            // If user confirms (clicked OK)
            if (newScenario.ShowDialog() == DialogResult.OK)
            {
                string newId = newScenario.CreatedSectionId;

                // Set the global variable
                sectionID = newId;

                // Refresh dropdown contents from DB
                FetchSectionIDsForDropdown();

                // Add the new section ID to the dropdown if it’s not already in there
                if (!sectionIdDropdown.Items.Contains(newId))
                {
                    sectionIdDropdown.Items.Add(newId);
                }


                sectionIdDropdown.SelectedItem = newId; // Select the new one
            }
        }
    }
}
