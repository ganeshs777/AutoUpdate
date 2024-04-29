using System.Security.Cryptography;
using System.Xml.Linq;

namespace AutoUpdater.Configurations
{
    public class Configuration
    {
        public string ApplicationName { get; set; }
        public string InstalledPath { get; set; }
        public string UpdateCheckUrl { get; set; }
        public string EncryptedUsername { get; set; }
        public string EncryptedPassword { get; set; }
        public string UpdatePackageUrl { get; set; }
        public byte[] Key { get; set; }
        public byte[] IV { get; set; }

        public Configuration(string configFile)
        {
            LoadConfiguration(configFile);
        }

        private void LoadConfiguration(string configFile)
        {
            try
            {
                XDocument doc = XDocument.Load(configFile);
                var appElement = doc.Root.Element("Application");
                var securityElement = doc.Root.Element("Security");

                if (appElement != null)
                {
                    ApplicationName = appElement.Element("Name")?.Value;
                    InstalledPath = appElement.Element("InstalledPath")?.Value;
                    UpdateCheckUrl = appElement.Element("UpdateCheckUrl")?.Value;
                    UpdatePackageUrl = appElement.Element("UpdatePackageUrl")?.Value;
                }

                if (securityElement != null)
                {
                    EncryptedUsername = securityElement.Element("Username")?.Value;
                    EncryptedPassword = securityElement.Element("Password")?.Value;
                    Key = Convert.FromBase64String(securityElement.Element("Key")?.Value);
                    IV = Convert.FromBase64String(securityElement.Element("IV")?.Value);

                    // Decrypt the username and password using the loaded key and IV
                    if (!string.IsNullOrEmpty(EncryptedUsername))
                        EncryptedUsername = DecryptStringFromBytes_Aes(Convert.FromBase64String(EncryptedUsername), Key, IV);
                    if (!string.IsNullOrEmpty(EncryptedPassword))
                        EncryptedPassword = DecryptStringFromBytes_Aes(Convert.FromBase64String(EncryptedPassword), Key, IV);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error loading configuration: " + ex.Message);
            }
        }

        // AES decryption using provided key and IV
        private static string DecryptStringFromBytes_Aes(byte[] cipherText, byte[] key, byte[] iv)
        {
            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = key;
                aesAlg.IV = iv;

                // Create a decryptor to perform the stream transform.
                ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

                // Create the streams used for decryption.
                using (MemoryStream msDecrypt = new MemoryStream(cipherText))
                {
                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                        {
                            // Read the decrypted bytes from the decrypting stream and return them as a string.
                            return srDecrypt.ReadToEnd();
                        }
                    }
                }
            }
        }
    }
}