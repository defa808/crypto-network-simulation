using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Crypto_Network_Simulation {
    public class NodeClient {
        protected string publicKey;
        protected string privateKey;
        public NodeClient() {
            GenerateKeys();
        }

        private void GenerateKeys() {
            throw new NotImplementedException();
        }

        public List<NodeClient> ConnectedClients { get; set; }
        
        private Dictionary<NodeClient,string> ClientPublicKey { get; set; }

        public void SendMessage(string message, NodeClient recepient) {
            
          


            if (!ClientPublicKey.ContainsKey(recepient)) {
                //send key
                Package messageKey = new Package() {
                    Message = "ste",
                    Sender = this,
                    Recipient = recepient,

                };


            }

            string publicKey = ClientPublicKey.GetValueOrDefault(recepient);

            Package package = new Package() {
                Message = AsyncCrypto(message, publicKey),
                Sender = this,
                Recipient = recepient
            };
            //crypto

        }

        private string AsyncCrypto(string message, string publicKey) {
            throw new NotImplementedException();
        }

        public void SendPackage(Package package, NodeClient exceptNodeClient) {


            if (package.Recipient == this) {
                HandlePackage(package);
                return;
            }

            foreach (var item in ConnectedClients.Where(x=> x != exceptNodeClient)) {
                item.SendPackage(package, this);
            }
        }

        private void HandlePackage(Package package) {
            switch (package.Type) {

            }


            package.Type switch {
                PackageType.GetPublicKey => throw new NotImplementedException(),
                PackageType.ReceivePublicKey => throw new NotImplementedException()
            };


            //1 SendMesage from client 1 to client 3 with asymmetric algorithm. Except when they share asymetric keys. (So client 1 говорит елиенту 3 я хочу тебе отправить сообщение, клиент 3 передает ключ клиенту 1, клиент 1 передает шифрованое сообщение)
            //2 
            //3


        }
    }
}