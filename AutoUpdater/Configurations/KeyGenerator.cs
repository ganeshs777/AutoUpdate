using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Xml.Linq;

namespace AutoUpdater.Configurations
{
    public class KeyGenerator
    {
        public static void GenerateKeyAndIV(string configFile)
        {
            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.GenerateKey();
                aesAlg.GenerateIV();

                byte[] key = aesAlg.Key;
                byte[] iv = aesAlg.IV;

                // Store key and IV in the XML configuration file
                StoreKeyAndIV(configFile, key, iv);
            }
        }

        private static void StoreKeyAndIV(string configFile, byte[] key, byte[] iv)
        {
            XDocument doc = XDocument.Load(configFile);

            // Create or update the Security element
            var securityElement = doc.Root.Element("Security");
            if (securityElement == null)
            {
                securityElement = new XElement("Security");
                doc.Root.Add(securityElement);
            }

            // Store key and IV as base64 strings
            securityElement.SetElementValue("Key", Convert.ToBase64String(key));
            securityElement.SetElementValue("IV", Convert.ToBase64String(iv));

            // Save the changes
            doc.Save(configFile);
        }

        //public static void Main(string[] args)
        //{
        //    string configFile = "config.xml";
        //    GenerateKeyAndIV(configFile);
        //    Console.WriteLine("Key and IV generated and stored in the configuration file.");
        //}
    }
}
