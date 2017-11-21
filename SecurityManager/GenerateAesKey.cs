using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace SecurityManager
{
    class GenerateAesKey
    {
        // Holds the current session's key.
        private byte[] mySessionKey;

        public GenerateAesKey()
        {
        }

        public byte[] MySessionkey
        {
            get { return mySessionKey; }
            set { mySessionKey = value; }
        }

        // Get the Public Key from the Server
        RSAParameters publicKey = GetFromServer();

        private static RSAParameters GetFromServer()
        {
            //TO DO
            throw new NotImplementedException();
        }

        // Send encrypted session key to Server.
        public void SendToServer()
        {
            byte[] b = GenerateAndEncryptSessionKey(publicKey);
            //send to server TO DO
        }
 

        private byte[] GenerateAndEncryptSessionKey(RSAParameters publicKey)
        {
            using (Aes aes = Aes.Create())
            {
                aes.KeySize = aes.LegalKeySizes[0].MaxSize;
                // Setting the KeySize generates a new key, but if you're paranoid, you can call aes.GenerateKey() again.
                mySessionKey = aes.Key;
            }

            using (RSACryptoServiceProvider rsa = new RSACryptoServiceProvider())
            {
                rsa.ImportParameters(publicKey);
                return rsa.Encrypt(mySessionKey, true /* use OAEP padding */);
            }
        }


    }
}
