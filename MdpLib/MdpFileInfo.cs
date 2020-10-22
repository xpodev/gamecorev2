using System.IO;
using System.Security.Cryptography;
using Newtonsoft.Json;


namespace GameCore.Mdp
{
    public class MdpFileInfo
    {
        protected bool CompareHashes(byte[] hashA, byte[] hashB)
        {
            if (hashA.Length == hashB.Length)
            {
                for (int i = 0; i < hashA.Length; i++)
                {
                    if (hashA[i] != hashB[i])
                    {
                        return false;
                    }
                }
                return true;
            }
            return false;
        }

        public MdpFileInfo(string name, byte[] hash)
        {
            Name = name;
            Hash = hash;
        }

        [JsonProperty(MdpApi.FileList.Json.FileNamePropertyName)]
        public string Name
        {
            get;
            set;
        }

        [JsonProperty(MdpApi.FileList.Json.FileHashPropertyName)]
        public string HashString
        {
            get
            {
                return MdpFileList.ConvertByteArrayToHexString(Hash);
            }
            set
            {
                Hash = MdpFileList.ConvertHexStringToByteArray(value);
            }
        }

        [JsonIgnore]
        public byte[] Hash
        {
            get;
            set;
        }

        public virtual bool CompareFile(string path, HashAlgorithm algorithm)
        {
            byte[] hash = GetFileHash(Path.Combine(path, Name), algorithm);
            return CompareHashes(hash, Hash);
        }

        public static byte[] GetFileHash(string path, HashAlgorithm algorithm)
        {
            using (FileStream stream = File.OpenRead(path))
            {
                byte[] hash = algorithm.ComputeHash(stream);
                return hash;
            }
        }
    }
}
