using System;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Xml.Linq;
using AutoUpdater.Configurations;

namespace AutoUpdater
{
    public class AutoUpdater
    {
        private const string ConfigFile = "config.xml";

        public static void CheckForUpdates()
        {
            try
            {
                // Load configuration
                Configuration configuration = LoadConfiguration(ConfigFile);

                // Fetch information about the latest version
                try
                {
                    var webRequest = WebRequest.Create(configuration.UpdateCheckUrl);
                    using (var response = webRequest.GetResponse())
                    using (var content = response.GetResponseStream())
                    using (var reader = new StreamReader(content))
                    {
                        string latestVersion = reader.ReadToEnd();

                        // Compare versions
                        Version currentVersion = new Version(GetCurrentVersion());
                        Version latest = new Version(latestVersion);

                        if (latest > currentVersion)
                        {
                            Console.WriteLine("A new version is available. Do you want to update? (Y/N)");
                            string input = Console.ReadLine().Trim().ToUpper();
                            if (input == "Y")
                            {
                                DownloadUpdate(configuration);
                            }
                        }
                        else
                        {
                            Console.WriteLine("You are already using the latest version.");
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("An error occurred while checking for updates: " + ex.Message);
                }
                Console.ReadLine();
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred while checking for updates: " + ex.Message);
            }
        }

        private static Configuration LoadConfiguration(string configFile)
        {
            try
            {
                XDocument doc = XDocument.Load(configFile);
                Configuration configuration = new Configuration(configFile);
                return configuration;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error loading configuration: " + ex.Message);
                return null;
            }
        }

        private static void DownloadUpdate(Configuration configuration)
        {
            try
            {
                // Download the update package
                using (var client = new WebClient())
                {
                    client.DownloadFile(configuration.UpdatePackageUrl, "update.zip");
                }

                ApplyUpdate();
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred while downloading the update: " + ex.Message);
            }
        }

        private static void ApplyUpdate()
        {
            try
            {
                // Unzip the update package
                string updatePackagePath = "update.zip";
                string extractPath = "update";

                ZipFile.ExtractToDirectory(updatePackagePath, extractPath);
                
                // Add logic to apply the update (e.g., extract files from the update package)
                // Here, we're just displaying a message
                Console.WriteLine("Update downloaded and applied successfully. Please restart the application.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred while applying the update: " + ex.Message);
            }
        }

        private static string GetCurrentVersion()
        {
            // Add logic to get the current version of the application
            // For example, you might read the version from a configuration file or assembly metadata
            // Here, we're just returning a dummy version
            return "1.0.0";
        }

        public static void Main(string[] args)
        {
            CheckForUpdates();
        }
    }
}
