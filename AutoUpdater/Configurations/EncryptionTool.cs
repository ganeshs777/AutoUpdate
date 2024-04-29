using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Xml.Linq;

namespace AutoUpdater.Configurations
{
    public class EncryptionTool
    {
        // AES encryption
        public static string EncryptStringToBytes_Aes(string plainText, byte[] key, byte[] iv)
        {
            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = key;
                aesAlg.IV = iv;

                // Create a encryptor to perform the stream transform.
                ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

                // Create the streams used for encryption.
                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                        {
                            // Write all data to the stream.
                            swEncrypt.Write(plainText);
                        }
                    }
                    return Convert.ToBase64String(msEncrypt.ToArray());
                }
            }
        }

        // Store encrypted username, password, and application details in XML configuration file
        public static void StoreEncryptedConfig(string configFile, string appName, string installedPath, string updateCheckUrl, string encryptedUsername, string encryptedPassword)
        {
            try
            {
                XDocument doc = new XDocument();
                XElement root = new XElement("AutoUpdaterConfig");
                doc.Add(root);

                // Add application details
                XElement appElement = new XElement("Application");
                appElement.Add(new XElement("Name", appName));
                appElement.Add(new XElement("InstalledPath", installedPath));
                appElement.Add(new XElement("UpdateCheckUrl", updateCheckUrl));
                root.Add(appElement);

                // Add security details
                XElement securityElement = new XElement("Security");
                securityElement.Add(new XElement("Username", encryptedUsername));
                securityElement.Add(new XElement("Password", encryptedPassword));
                root.Add(securityElement);

                doc.Save(configFile);
                Console.WriteLine("Encrypted configuration stored in the file: " + configFile);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error storing encrypted configuration: " + ex.Message);
            }
        }

        // Main method to manipulate configurations
        //public static void Main(string[] args)
        //{
        //    //if (args.Length != 8)
        //    //{
        //    //    Console.WriteLine("Usage: EncryptionTool.exe <config_file> <key_base64> <iv_base64> <app_name> <installed_path> <update_check_url> <username> <password>");
        //    //    return;
        //    //}

        //    //string configFile = args[0];
        //    //string keyBase64 = args[1];
        //    //string ivBase64 = args[2];
        //    //string appName = args[3];
        //    //string installedPath = args[4];
        //    //string updateCheckUrl = args[5];
        //    //string username = args[6];
        //    //string password = args[7];

        //    string configFile = @"C:\Users\lENOVO\source\repos\AutoUpdater\AutoUpdaterConfig.xml";
        //    string keyBase64 = "eGCWR0iQoeh+zzd437dz7Rhfwbf737waF+zkwtJQPss=";
        //    string ivBase64 = "jSdGd98R2gwIMSTvTHYG4A==";
        //    string appName = "System1";
        //    string installedPath = "C:\\Program Files\\System1";
        //    string updateCheckUrl = "http://example.com/update.xml";
        //    string username = "admin";
        //    string password = "admin";

        //    // Encrypt username and password using AES encryption
        //    string encryptedUsername = EncryptStringToBytes_Aes(username, Convert.FromBase64String(keyBase64), Convert.FromBase64String(ivBase64));
        //    string encryptedPassword = EncryptStringToBytes_Aes(password, Convert.FromBase64String(keyBase64), Convert.FromBase64String(ivBase64));

        //    // Store encrypted configuration in the output configuration file
        //    StoreEncryptedConfig(configFile, appName, installedPath, updateCheckUrl, encryptedUsername, encryptedPassword);
        //}
    }
}
