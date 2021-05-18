using System;
using System.Collections.Generic;
using System.Text;

namespace Crypto_Network_Simulation {
    public class Package {
        public string Message { get; set; }
        public byte[] File { get; set; }
        public NodeClient Sender { get; set; }
        public NodeClient Recipient { get; set; }
        public PackageType Type { get; set; }
    }
}
