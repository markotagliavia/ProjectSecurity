using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace SecurityManager
{
    public class GenerateAesKey
    {
        // Holds the current session's key.
        private byte[] mySessionKey;
        private RSAParameters publicKey;
        private int keySize;

        public GenerateAesKey(RSAParameters publicKey, int keySize)
        {
            this.publicKey = publicKey;
            this.keySize = keySize;
        }

        public byte[] MySessionkey
        {
            get { return mySessionKey; }
            set { mySessionKey = value; }
        }

        // Get the Public Key from the Server
        // = GetFromServer();

        public RSAParameters PublicKey
        {
            get { return publicKey; }
            set { publicKey = value; }
        }

        /* private static RSAParameters GetFromServer()
         {
             //TO DO
             throw new NotImplementedException();
         }

         // Send encrypted session key to Server.
         public void SendToServer()
         {
             byte[] b = GenerateAndEncryptSessionKey(publicKey);
             //send to server TO DO
         }*/

        public byte[] AesEncryptedSessionKey
        {
            get { return GenerateAndEncryptSessionKey(publicKey); }
        }

        private byte[] GenerateAndEncryptSessionKey(RSAParameters publicKey)
        {
            using (Aes aes = Aes.Create())
            {
                aes.KeySize = aes.LegalKeySizes[0].MaxSize;
                // Setting the KeySize generates a new key, but if you're paranoid, you can call aes.GenerateKey() again.
                aes.GenerateKey();
                mySessionKey = aes.Key;
            }

            using (RSACryptoServiceProvider rsa = new RSACryptoServiceProvider(keySize))
            {
                rsa.ImportParameters(publicKey);
                return rsa.Encrypt(mySessionKey, true /* use OAEP padding */);
            }
        }


    }
}
