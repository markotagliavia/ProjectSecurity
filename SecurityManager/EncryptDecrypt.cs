﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace SecurityManager
{
    public class EncryptDecrypt
    {
        public EncryptDecrypt() { }

        public byte[] EncryptData(byte[] key, string data)
        {
            using (Aes aes = Aes.Create())
            {
                byte[] result;

                aes.Key = key;
                aes.GenerateIV();

                using (ICryptoTransform encryptor = aes.CreateEncryptor())
                {
                    using (MemoryStream ms = new MemoryStream())
                    {
                        using (CryptoStream cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
                        {
                            using (StreamWriter writer = new StreamWriter(cs))
                            {
                                writer.Write(data);
                            }
                        }

                        byte[] encrypted = ms.ToArray();
                        result = new byte[aes.BlockSize / 8 + encrypted.Length];

                        // Result is built as: IV (plain text) + Encrypted(data)
                        Array.Copy(aes.IV, result, aes.BlockSize / 8);
                        Array.Copy(encrypted, 0, result, aes.BlockSize / 8, encrypted.Length);

                        return result;
                    }
                }
            }
        }

        public byte[] Decrypt(byte[] key, byte[] data)
        {
            using (Aes aes = Aes.Create())
            {
                aes.Key = key;

                // Extract the IV from the data first.
                byte[] iv = new byte[aes.BlockSize / 8];
                Array.Copy(data, iv, iv.Length);
                aes.IV = iv;

                // The remainder of the data is the encrypted data we care about.
                byte[] encryptedData = new byte[data.Length - iv.Length];
                Array.Copy(data, iv.Length, encryptedData, 0, encryptedData.Length);

                using (ICryptoTransform decryptor = aes.CreateDecryptor())
                {
                    using (MemoryStream ms = new MemoryStream(encryptedData))
                    {
                        using (CryptoStream cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read))
                        {
                            using (StreamReader reader = new StreamReader(cs))
                            {
                                return Encoding.ASCII.GetBytes(reader.ReadToEnd());
                            }
                        }
                    }
                }
            }
        }
    }
}
