using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Services {
    public static class SymetricFileCypher {
        public static byte[] EncryptData(byte[] data, string publicKey) {
            using (var asymmetricProvider = new RSACryptoServiceProvider()) {
                asymmetricProvider.FromXmlString(publicKey);
                return asymmetricProvider.Encrypt(data, true);
            }
        }

        public static byte[] DecryptData(byte[] data, string privateKey) {
            using (var asymmetricProvider = new RSACryptoServiceProvider()) {
                asymmetricProvider.FromXmlString(privateKey);
                if (asymmetricProvider.PublicOnly)
                    throw new Exception("The key provided is a public key and does not contain the private key elements required for decryption");
                return asymmetricProvider.Decrypt(data, true);
            }
        }
        public static void EncryptFile(string inputFilePath, string outputFilePath, string publicKey) {
            using (var symmetricCypher = new AesManaged()) {
                // Generate random key and IV for symmetric encryption
                var key = new byte[symmetricCypher.KeySize / 8];
                var iv = new byte[symmetricCypher.BlockSize / 8];
                using (var rng = new RNGCryptoServiceProvider()) {
                    rng.GetBytes(key);
                    rng.GetBytes(iv);
                }

                // Encrypt the symmetric key and IV
                var buf = new byte[key.Length + iv.Length];
                Array.Copy(key, buf, key.Length);
                Array.Copy(iv, 0, buf, key.Length, iv.Length);
                buf = EncryptData(buf, publicKey);

                var bufLen = BitConverter.GetBytes(buf.Length);

                // Symmetrically encrypt the data and write it to the file, along with the encrypted key and iv
                using (var cypherKey = symmetricCypher.CreateEncryptor(key, iv))
                using (var fsIn = new FileStream(inputFilePath, FileMode.Open))
                using (var fsOut = new FileStream(outputFilePath, FileMode.Create))
                using (var cs = new CryptoStream(fsOut, cypherKey, CryptoStreamMode.Write)) {
                    fsOut.Write(bufLen, 0, bufLen.Length);
                    fsOut.Write(buf, 0, buf.Length);
                    fsIn.CopyTo(cs);
                }
            }
        }

        public static void DecryptFile(byte[] filePath, string outputFilePath, string symetricKey) {
            using (var symmetricCypher = new AesManaged())
            using (MemoryStream theMemStream = new MemoryStream()) {
                var iv = new byte[symmetricCypher.BlockSize / 8];

                theMemStream.Write(filePath, 0, filePath.Length);

                // Decript the file data using the symmetric algorithm
                using (var cypherKey = symmetricCypher.CreateDecryptor(Convert.FromBase64String(symetricKey), iv))
                using (var fsOut = new FileStream(outputFilePath, FileMode.Create))
                using (var cs = new CryptoStream(fsOut, cypherKey, CryptoStreamMode.Write)) {
                    theMemStream.CopyTo(cs);
                }
            }
        }

        public static string CreateSymetricKey() {
            using (var symmetricCypher = new AesManaged()) {
                symmetricCypher.GenerateKey();
                return Convert.ToBase64String(symmetricCypher.Key);
            }

        }
    }
}
