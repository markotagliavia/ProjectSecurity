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
        // Get the Public Key from the Server
      /*  RSAParameters publicKey = GetFromServer();

        private static RSAParameters GetFromServer()
        {
            throw new NotImplementedException();
        }

        // Holds the current session's key.
        byte[] MySessionKey;

        // Send encrypted session key to Server.
        public void SendToServer(GenerateAndEncryptSessionKey(publicKey))
        {
        
        }
 

        private byte[] GenerateAndEncryptSessionKey(RSAParameters publicKey)
        {
            using (Aes aes = Aes.Create())
            {
                aes.KeySize = aes.LegalKeySizes[0].MaxSize;
                // Setting the KeySize generates a new key, but if you're paranoid, you can call aes.GenerateKey() again.

                MySessionKey = aes.Key;
            }

            using (RSACryptoServiceProvider rsa = new RSACryptoServiceProvider())
            {
                rsa.ImportParameters(publicKey);

                return rsa.Encrypt(MySessionKey, true /* use OAEP padding );
            }
        }*/
    }
}
