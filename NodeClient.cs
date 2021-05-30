using Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace Crypto_Network_Simulation {
    public class NodeClient : ISendMessageBehavior {
        protected string publicKey;
        protected string privateKey;
        private readonly string localFolder;

        public string Name { get; set; }
        public NodeClient() {
            GenerateKeys();
        }

        private void GenerateKeys() {
            //TODO rewrite it (use your own algoritm)
            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
            MyRSACryptoServiceProvider myRSA = new MyRSACryptoServiceProvider();
            // Get the public keyy   
            string publicKey = myRSA.GenerateKey(false); // false to get the public key   
            string privateKey = myRSA.GenerateKey(true);
            //// Get the public keyy   
            string publicKey2 = rsa.ToXmlString(false); // false to get the public key   
            string privateKey2 = rsa.ToXmlString(true);

            this.publicKey = publicKey;
            this.privateKey = privateKey;
        }

        public List<NodeClient> ConnectedClients { get; set; } = new List<NodeClient>();

        private Dictionary<NodeClient, string> ClientPublicKey { get; set; } = new Dictionary<NodeClient, string>();
        private Dictionary<NodeClient, string> ClientSymetricKey { get; set; } = new Dictionary<NodeClient, string>();

        public void SendMessage(string message, NodeClient recepient) {
            if (!ClientPublicKey.ContainsKey(recepient)) {
                DisplayMessage(null, "Get public key from " + recepient.Name);

                Package packageToGetKey = new Package() {
                    Sender = this,
                    Recipient = recepient,
                    Type = PackageType.GetPublicKey
                };
                SendPackage(packageToGetKey, this);
            }

            string publicKey = ClientPublicKey[recepient];

            DisplayMessage(message, "Sending message");
            Package package = new Package() {
                Message = AsyncEncrypt(message, publicKey),
                Sender = this,
                Recipient = recepient,
                Type = PackageType.RecieveMessage
            };

            SendPackage(package, this);
        }

        private string AsyncEncrypt(string message, string publicKey) {
            byte[] dataToEncrypt = Convert.FromBase64String(message);

            // Create a byte array to store the encrypted data in it   
            byte[] encryptedData;
            using (RSACryptoServiceProvider rsa = new RSACryptoServiceProvider(4096)) {
                // Set the rsa pulic key   
                rsa.FromXmlString(publicKey);

                // Encrypt the data and store it in the encyptedData Array   
                encryptedData = rsa.Encrypt(dataToEncrypt, false);
            }

            return Convert.ToBase64String(encryptedData);

        }

        private string AsyncDecrypt(string message) {
            byte[] dataToDecrypt = Convert.FromBase64String(message);

            // Create an array to store the decrypted data in it   
            byte[] decryptedData;
            using (RSACryptoServiceProvider rsa = new RSACryptoServiceProvider(4096)) {

                // Set the private key of the algorithm   
                rsa.FromXmlString(privateKey);
                decryptedData = rsa.Decrypt(dataToDecrypt, false);
            }
            return Convert.ToBase64String(decryptedData);
        }

        public void SendPackage(Package package, NodeClient exceptNodeClient) {
            DisplayPachage(package);
            if (package.Recipient == this) {
                HandlePackage(package);
                return;
            }

            foreach (var nextClient in ConnectedClients.Where(x => x != exceptNodeClient)) {
                nextClient.SendPackage(package, this);
            }
        }

        private void HandlePackage(Package package) {
            switch (package.Type) {
                case PackageType.GetPublicKey:
                    HandleGetPublicKey(package);
                    break;
                case PackageType.ReceivePublicKey:
                    HandleReceivePublicKey(package);
                    break;
                case PackageType.RecieveMessage:
                    HandleRecieveMessage(package);
                    break;
                case PackageType.GetSymetricKey:
                    HandleGetSymetricKey(package);
                    break;
                case PackageType.ReceiveSymetricKey:
                    HandleRecieveSymetricKey(package);
                    break;
                case PackageType.RecieveFile:
                    HandleRecieveFile(package);
                    break;
            }
        }

        private void HandleRecieveFile(Package package) {
            string symetricKey = ClientSymetricKey.GetValueOrDefault(package.Sender);
            if(symetricKey == null) {
                Package symetricMessage = new Package() {
                    Type = PackageType.GetSymetricKey,
                    Sender = this,
                    Recipient = package.Sender
                };
                SendPackage(symetricMessage, this);
                symetricKey = ClientSymetricKey.GetValueOrDefault(package.Sender);
            }
            SymetricFileCypher.DecryptFile(package.File, this.localFolder, symetricKey);
            DisplayMessage("File recieved", "Recieved message");
        }

        private void HandleRecieveSymetricKey(Package package) {
            string symetricKey = AsyncDecrypt(package.Message);
            ClientSymetricKey.Add(package.Sender, symetricKey);
        }

        private void HandleGetSymetricKey(Package package) {
            string symetricKey;
            if (!ClientSymetricKey.TryGetValue(package.Sender, out symetricKey)) {
                symetricKey = SymetricFileCypher.CreateSymetricKey();
                ClientSymetricKey.Add(package.Sender, symetricKey);
            }
            string message = AsyncDecrypt(symetricKey);
            Package packageToGetKey = new Package() {
                Message = message,
                Sender = this,
                Recipient = package.Sender,
                Type = PackageType.ReceiveSymetricKey
            };
            SendPackage(packageToGetKey, this);
        }

        private void HandleReceivePublicKey(Package package) {
            string message = AsyncDecrypt(package.Message);
            ClientPublicKey.Add(package.Sender, message);
        }

        private void HandleGetPublicKey(Package package) {
            Package packageToGetKey = new Package() {
                Message = publicKey,
                Sender = this,
                Recipient = package.Sender,
                Type = PackageType.ReceivePublicKey
            };
            SendPackage(packageToGetKey, this);
        }

        private void HandleRecieveMessage(Package package) {
            string message = AsyncDecrypt(package.Message);
            DisplayMessage(message, "Recieved message");
        }

        private void DisplayMessage(string message, string action) {
            Console.WriteLine(Name + " " + action);
            if (!string.IsNullOrEmpty(message))
                Console.WriteLine("Message: " + message);
            Console.WriteLine();
        }

        private void DisplayPachage(Package package) {
            Console.WriteLine(Name + " handling package");
            Console.WriteLine("Type: " + package.Type);
            if (string.IsNullOrEmpty(package.Message))
                Console.WriteLine("Message: " + package.Message);
            Console.WriteLine();
        }
    }
}