using System;
using System.Collections.Generic;

namespace Crypto_Network_Simulation {
    class Program {
        static void Main(string[] args) {
            NodeClient c1 = new NodeClient() {
                Name = "Client 1"
            };
            NodeClient c2 = new NodeClient() {
                Name = "Client 2"
            };
            NodeClient c3 = new NodeClient() {
                Name = "Client 3"
            };

            c1.ConnectedClients.Add(c2);
            c2.ConnectedClients.AddRange(new List<NodeClient> { c1, c3 });
            c3.ConnectedClients.Add(c2);


            c1.SendMessage("FirstMessage", c3);
            c1.SendFile(@"D:\KPI\Security\Crypto-Network-Simulation\lab1.docx", c3);

            Console.ReadLine();
        }
    }
}
