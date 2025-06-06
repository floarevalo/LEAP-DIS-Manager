using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using System.Net;
namespace runFuncts
{
    public static class runFuncts
    {
        public static bool validateRunParams(string receiveIpAddress, int receivePort, string databaseIpAddress, int databasePort)
        {
            if (IPAddress.TryParse(receiveIpAddress, out _))
            {
                if(IPAddress.TryParse(databaseIpAddress, out _))
                {
                    if (receivePort > 0 && receivePort < 65536)
                    {
                        if(databasePort > 0 && receivePort < 665536)
                        {
                            return true;
                        }
                        else
                        {
                            MessageBox.Show("Invalid database port number. Please update in settings");
                        }
                    }
                    else
                    {
                        MessageBox.Show("Invalid receiver port number. Please update in settings");
                    }
                }
                else
                {
                    MessageBox.Show("Invalid database IP address. Please update in settings");
                }
            }
            else
            {
                MessageBox.Show("Invalid receiver IP address. Please update in settings");
            }
            return false;
        }


    }
}