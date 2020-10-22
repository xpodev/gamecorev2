using System.IO;
using System.Text;
using System.Security.Cryptography;

namespace GameCore.Mdp
{
    public class MdpDirectoryInfo : MdpFileInfo
    {
        public MdpDirectoryInfo(string name, byte[] hash) : base(name, hash)
        {

        }

        public override bool CompareFile(string path, HashAlgorithm algorithm)
        {

            byte[] hash = GetDirectoryHash(Path.Combine(path, Name), algorithm);

            return CompareHashes(hash, Hash);
        }

        public static byte[] GetDirectoryHash(string dirPath, HashAlgorithm algorithm)
        {
            string[] files = Directory.GetFiles(dirPath, "*.*", SearchOption.AllDirectories);

            if (files.Length == 0)
            {
                return new byte[0];
            }

            for (int i = 0; i < files.Length - 1; i++)
            {
                string file = files[i];
                if (Path.GetFileName(file).StartsWith(MdpApi.Resources.IgnoreCharacter))
                {
                    continue;
                }

                string relativePath = file.Substring(dirPath.Length);
                byte[] pathBytes = Encoding.UTF8.GetBytes(relativePath.ToLower());
                algorithm.TransformBlock(pathBytes, 0, pathBytes.Length, pathBytes, 0);

                byte[] contentBytes = File.ReadAllBytes(file);
                algorithm.TransformBlock(contentBytes, 0, contentBytes.Length, contentBytes, 0);
            }

            byte[] fileContent = File.ReadAllBytes(files[files.Length - 1]);
            byte[] hash = algorithm.TransformFinalBlock(fileContent, 0, fileContent.Length);
            return hash;
        }
    }
}
