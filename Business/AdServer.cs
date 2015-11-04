using log4net;
using System;
using System.Collections.Generic;
using System.IO;
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
                string baseDirPath = AppDomain.CurrentDomain.BaseDirectory;
                adServersFilePath = baseDirPath + "App_Data\\adServerList.txt";
                LogManager.GetLogger(new AdServer().GetType().Name).Debug("Using file for adServers " + adServersFilePath);

                if (!System.IO.File.Exists(adServersFilePath))
                {
                    LogManager.GetLogger(new AdServer().GetType().Name).Debug("Could not find file" + adServersFilePath);
                    return;

                }
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
