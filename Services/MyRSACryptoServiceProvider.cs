using System;
using System.Numerics;
using System.Security.Cryptography;
using System.Security.Permissions;
using System.Text;

namespace Services {
    public class MyRSACryptoServiceProvider {
        //[System.Security.SecurityCritical]
        //private SafeProvHandle _safeProvHandle;
        //[System.Security.SecurityCritical]
        //private SafeKeyHandle _safeKeyHandle;
        private CspParameters _parameters;


        //private void GetKeyPair() {
        //    if (_safeKeyHandle == null) {
        //        lock (this) {
        //            if (_safeKeyHandle == null) {
        //                // We only attempt to generate a random key on desktop runtimes because the CoreCLR
        //                // RSA surface area is limited to simply verifying signatures.  Since generating a
        //                // random key to verify signatures will always lead to failure (unless we happend to
        //                // win the lottery and randomly generate the signing key ...), there is no need
        //                // to add this functionality to CoreCLR at this point.
        //                BigInteger e = new BigInteger(65537);
        //                BigInteger n = new BigInteger(561);
        //                BigInteger p = BigInteger.Zero;
        //                BigInteger q = BigInteger.Zero;
        //                Unitls.GenerateRSAKoef(ref p, ref q, n);
        //                Unitls.Create( p.ToByteArray(), q.ToByteArray(), e.ToByteArray(), n.ToByteArray());
        //                //Utils.GetKeyPairHelper(CspAlgorithmType.Rsa, _parameters, _randomKeyContainer, _dwKeySize, ref _safeProvHandle, ref _safeKeyHandle);
        //            }
        //        }
        //    }
        //}

        BigInteger p = BigInteger.Zero;
        BigInteger q = BigInteger.Zero;

        [System.Security.SecuritySafeCritical]
        public RSAParameters ExportParameters(bool isPrivate) {
            //GetKeyPair();
            //if (isPrivate) {
            //    KeyContainerPermission kp = new KeyContainerPermission(KeyContainerPermissionFlags.NoFlags);
            //    KeyContainerPermissionAccessEntry entry = new KeyContainerPermissionAccessEntry(_parameters, KeyContainerPermissionFlags.Export);
            //    kp.AccessEntries.Add(entry);
            //    kp.Demand();
            //}
            BigInteger e = new BigInteger(65537);
            BigInteger n = new BigInteger(4096);
            while (true) {
                try {
                    //if (p == BigInteger.Zero || q == BigInteger.Zero)
                    //    Unitls.GenerateRSAKoef(ref p, ref q, e, n);
                    p = new BigInteger(11);
                    q = new BigInteger(3);
                    return Unitls.Create(p.ToByteArray(), q.ToByteArray(), e.ToByteArray(), n.ToByteArray());
                } catch (ArgumentException exception) {
                    p = BigInteger.Zero;
                    q = BigInteger.Zero;
                }
            }

            //RSACspObject rsaCspObject = new RSACspObject();
            //int blobType = isPrivate ? Constants.PRIVATEKEYBLOB : Constants.PUBLICKEYBLOB;
            // _ExportKey will check for failures and throw an exception
            //Utils._ExportKey(_safeKeyHandle, blobType, rsaCspObject);
            //return RSAObjectToStruct(rsaCspbObject);
        }

        //private static RSAParameters RSAObjectToStruct(RSACspObject rsaCspObject) {
        //    RSAParameters rsaParams = new RSAParameters();
        //    rsaParams.Exponent = rsaCspObject.Exponent;
        //    rsaParams.Modulus = rsaCspObject.Modulus;
        //    rsaParams.P = rsaCspObject.P;
        //    rsaParams.Q = rsaCspObject.Q;
        //    rsaParams.DP = rsaCspObject.DP;
        //    rsaParams.DQ = rsaCspObject.DQ;
        //    rsaParams.InverseQ = rsaCspObject.InverseQ;
        //    rsaParams.D = rsaCspObject.D;
        //    return rsaParams;
        //}

        public String GenerateKey(bool isPrivate) {
            RSAParameters rsaParams = this.ExportParameters(isPrivate);
            StringBuilder sb = new StringBuilder();
            sb.Append("<RSAKeyValue>");
            // Add the modulus
            sb.Append("<Modulus>" + Convert.ToBase64String(rsaParams.Modulus) + "</Modulus>");
            // Add the exponent
            sb.Append("<Exponent>" + Convert.ToBase64String(rsaParams.Exponent) + "</Exponent>");
            if (isPrivate) {
                // Add the private components
                sb.Append("<P>" + Convert.ToBase64String(rsaParams.P) + "</P>");
                sb.Append("<Q>" + Convert.ToBase64String(rsaParams.Q) + "</Q>");
                sb.Append("<DP>" + Convert.ToBase64String(rsaParams.DP) + "</DP>");
                sb.Append("<DQ>" + Convert.ToBase64String(rsaParams.DQ) + "</DQ>");
                sb.Append("<InverseQ>" + Convert.ToBase64String(rsaParams.InverseQ) + "</InverseQ>");
                sb.Append("<D>" + Convert.ToBase64String(rsaParams.D) + "</D>");
            }
            sb.Append("</RSAKeyValue>");
            return (sb.ToString());
        }
    }
}
