using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace GameCore.Mdp
{
    public class MdpConfiguration
    {
        public static HashAlgorithm DefaultHash = SHA1.Create();

        public MdpConfiguration()
        {
            Hash = DefaultHash;
        }

        public MdpConfiguration(HashAlgorithm algorithm)
        {
            Hash = algorithm;
        }

        public string RemoteFileServer
        {
            get;
            set;
        }

        public HashAlgorithm Hash
        {
            get;
            private set;
        }
    }
}
