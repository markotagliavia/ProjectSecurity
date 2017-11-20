using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using static SecurityManager.GennerateRsaKey;

namespace SecurityManager
{
    class DecryptSymmetricKey
    {

       /* public void SetSymmetricKey(Guid id, byte[] encryptedKey)
        {
            SessionKey session = sessionKeys[id];

            using (RSACryptoServiceProvider rsa = new RSACryptoServiceProvider())
            {
                rsa.ImportParameters(session.PrivateKey);

                session.SymmetricKey = rsa.Decrypt(encryptedKey, true);
            }
        }*/
    }
}
