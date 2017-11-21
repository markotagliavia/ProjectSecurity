using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;


namespace SecurityManager
{

    public class GennerateRsaKey
    {
        private int keySize;

        public GennerateRsaKey(int size)
        {
            keySize = size;
            SessionKeys = new Dictionary<Guid, SessionKey>();
        }

        public sealed class SessionKey
        {
            public Guid Id;
            public byte[] SymmetricKey;
            public RSAParameters PublicKey;
            public RSAParameters PrivateKey;

            public SessionKey(Guid id, byte[] SymmetricKey, RSAParameters PublicKey, RSAParameters PrivateKey)
            {
                this.Id = id;
                this.SymmetricKey = SymmetricKey;
                this.PublicKey = PublicKey;
                this.PrivateKey = PrivateKey;
            }
        }


        private Dictionary<Guid, SessionKey> sessionKeys;

        public Dictionary<Guid, SessionKey> SessionKeys
        {
            get { return sessionKeys; }
            set { sessionKeys = value; }
        }


        public RSAParameters Generate(Guid sessionId)
        {
            // NOTE: Make the key size configurable.
            using (RSACryptoServiceProvider rsa = new RSACryptoServiceProvider(keySize))
            {
                RSAParameters a = rsa.ExportParameters(false);
                RSAParameters b = rsa.ExportParameters(true);
                SessionKey s = new SessionKey(sessionId, null, a, b);
                /* {
                     Id = sessionId,
                     PublicKey = rsa.ExportParameters(false),
                     PrivateKey = rsa.ExportParameters(true),
                     SymmetricKey = null,
                 */

                sessionKeys.Add(Guid.NewGuid(), s);

                return s.PublicKey;
            }
        }

        public bool SetSymmetricKey(Guid id, byte[] encryptedKey)
        {
            SessionKey session = null;
            foreach (SessionKey si in this.SessionKeys.Values)
            {
                if (si.Id.Equals(id))
                {
                    session = si;
                }

            }

            bool retVal = true;
            try
            {
                using (RSACryptoServiceProvider rsa = new RSACryptoServiceProvider(keySize))
                {
                    rsa.ImportParameters(session.PrivateKey);
                    session.SymmetricKey = rsa.Decrypt(encryptedKey, true);
                }
            }
            catch (Exception ex)
            {
                retVal = false;
            }
            return retVal;
        }
    }
}
