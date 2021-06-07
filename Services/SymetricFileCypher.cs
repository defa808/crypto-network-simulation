using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Services {
    public static class SymetricFileCypher {
        public static byte[] EncryptFile(string key, byte[] file) {
            byte[] iv = new byte[16];
            byte[] array;

            using (Aes aes = Aes.Create()) {
                aes.Key = Convert.FromBase64String(key);
                aes.IV = iv;

                ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);

                using (MemoryStream memoryStream = new MemoryStream()) {
                    using (CryptoStream cryptoStream = new CryptoStream((Stream)memoryStream, encryptor, CryptoStreamMode.Write)) {
                        using (BinaryWriter binaryWriter = new BinaryWriter((Stream)cryptoStream)) {
                            binaryWriter.Write(file);
                        }

                        array = memoryStream.ToArray();
                    }
                }
            }

            return array;
        }

        public static byte[] DecryptFile(string key, byte[] file) {
            byte[] iv = new byte[16];

            using (Aes aes = Aes.Create()) {
                aes.Key = Convert.FromBase64String(key);
                aes.IV = iv;
                ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

                using (MemoryStream memoryStream = new MemoryStream(file)) {
                    using (CryptoStream cryptoStream = new CryptoStream((Stream)memoryStream, decryptor, CryptoStreamMode.Read)) {
                        using (BinaryReader binaryReader = new BinaryReader((Stream)cryptoStream)) {
                            using (MemoryStream memoryStreamRead = new MemoryStream()) {
                                binaryReader.BaseStream.CopyTo(memoryStreamRead);
                                return memoryStreamRead.ToArray();
                            }
                        }
                    }
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
