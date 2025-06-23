using Npgsql;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LEAP_dis_manager
{
    public partial class ScenarioInputForm : Form
    {
        private string databaseIpAddress;
        private int databasePort;
        private string sectionId;
        public ScenarioInputForm()
        {
            InitializeComponent();
            loadDataBase();
        }
        private void loadDataBase()
        {
            databaseIpAddress = Properties.Settings.Default.databaseIpAddress;
            databasePort = Properties.Settings.Default.databasePort;

        }

        public static void addSectionId(string sectionId, string dbIp, int dbPort)
        {

            string userId = "postgres";
            string password = "postgres";
            string databaseName = "LEAP";
            string connectionString = $"Host={dbIp};Port={dbPort};Database={databaseName};User Id={userId};Password={password};";
            try
            {
                using NpgsqlConnection conn = new NpgsqlConnection(connectionString);
                conn.Open();

                //Check the database to see if a unit with the given Entity ID already exists
                //this assumes that we use appplication_id and site_id to distinguish between different entities
                string query = "SELECT COUNT(*) FROM sections WHERE sectionid = @section_id";
                using NpgsqlCommand checkCommand = new NpgsqlCommand(query, conn);


                //sectionID is a global variable
                checkCommand.Parameters.AddWithValue("@section_id", sectionId);


                int count = Convert.ToInt32(checkCommand.ExecuteScalar());

                //That means that sectionid already exists
                if (count > 0)
                {
                    //If a unit with the given Entity ID does not exist in the database, add it
                    MessageBox.Show("Section ID already exists");
 
                }
                //sectionid doesn't exist so add it
                else
                {
                    //TODO: add the online thing
                    string insertQuery = "INSERT INTO sections (sectionid, isonline) VALUES (@section_id, @is_online)";
                    using NpgsqlCommand insertCommand = new NpgsqlCommand(insertQuery, conn);
                    insertCommand.Parameters.AddWithValue("@section_id", sectionId);
                    insertCommand.Parameters.AddWithValue("@is_online", false);
                    int rowsAffected = insertCommand.ExecuteNonQuery();
                    if (rowsAffected > 0)
                    {
                        MessageBox.Show("Section ID has been inserted");

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







        private void OK_Click(object sender, EventArgs e)
        {
            addSectionId(sectionId, databaseIpAddress, databasePort);
        }


        private void scenarioNameTextBox_Click(object sender, EventArgs e)
        {
            TextBox textBox = sender as TextBox;
            if (textBox != null)
            {
                textBox.SelectAll();
            }
        }

        private void scenarioNameTextBox_TextChanged(object sender, EventArgs e)
        {
            sectionId = newScenarioNameTextBox.Text;
        }

        private void Cancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
