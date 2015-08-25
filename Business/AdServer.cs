using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business
{
    public class AdServer
    {
        
            public static string adServersFilePath { get; set; }
            public static List<string> adServers { get; set; }

            static AdServer()
            {
                adServersFilePath = "C:\\Users\\apurva.jalit\\Workspace\\Notocol\\Business\\adServerList.txt";
                string[] servers = System.IO.File.ReadAllLines(@adServersFilePath);
                adServers = new List<string>();
                foreach (string line in servers)
                {
                    // Use a tab to indent each line of the file.
                    Console.WriteLine("\t" + line);
                    adServers.Add(line);
                }
            }

            public static bool isAdServer(string serverName)
            {
                return adServers.Contains("g"+serverName);
            }
        }
    
}
