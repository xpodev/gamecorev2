using GameCore.Mdp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Windows.Forms;

namespace TMdpGenerator
{
    class Program
    {
        static string HashName = "sha1";

        [STAThread]
        static void Main(string[] args)
        {

            OpenFileDialog fileDialog = new OpenFileDialog();
            fileDialog.Filter = "Include.txt|include.txt";
            fileDialog.InitialDirectory = Directory.GetCurrentDirectory();
            fileDialog.DefaultExt = ".txt";
            fileDialog.Title = "Open File: include.txt";
            fileDialog.Multiselect = false;

            DialogResult result = fileDialog.ShowDialog();
            if (result == DialogResult.Cancel)
            {
                return;
            }

            string includeFile = fileDialog.FileName;
            Console.WriteLine("Opening file:" + includeFile);

            string parentDirectory = Path.GetDirectoryName(includeFile);
            string fileContent = File.ReadAllText(includeFile);

            string[] includeObjects = fileContent.Split(new char[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);

            List<string> filesList = new List<string>(), dirsList = new List<string>();
            for (int i = 0; i < includeObjects.Length; i++)
            {
                string includedObject = Path.Combine(parentDirectory, includeObjects[i].Trim('\n', '\r'));
                if (Directory.Exists(includedObject))
                {
                    dirsList.Add(includedObject);
                }
                if (File.Exists(includedObject))
                {
                    filesList.Add(includedObject);
                }
            }

            GenerateDirectoryList(parentDirectory, dirsList.ToArray(), filesList.ToArray());

            Console.WriteLine("");
            Console.Write("Press any key to quit the program...");
            Console.ReadKey();
        }

        static void GenerateDirectoryList(string dirPath, string[] dirsList, string[] filesList)
        {
            MdpFileList files = new MdpFileList();
            List<MdpFileInfo> filesInfos = new List<MdpFileInfo>();
            foreach (string filePath in filesList)
            {
                if (Path.GetFileName(filePath).StartsWith(MdpApi.Resources.IgnoreCharacter))
                {
                    continue;
                }
                MdpFileInfo fileInfo = new MdpFileInfo(Path.GetFileName(filePath), MdpFileInfo.GetFileHash(filePath, HashAlgorithm.Create(HashName)));
                filesInfos.Add(fileInfo);
                Console.WriteLine($"Adding File: {fileInfo.Name} | Hash: {fileInfo.HashString}");
            }
            List<MdpDirectoryInfo> dirInfos = new List<MdpDirectoryInfo>();
            foreach (string filePath in dirsList)
            {
                if (Path.GetDirectoryName(filePath).StartsWith(MdpApi.Resources.IgnoreCharacter))
                {
                    continue;
                }
                MdpFileInfo fileInfo = new MdpFileInfo(Path.GetFileName(filePath) + "/", MdpDirectoryInfo.GetDirectoryHash(filePath, HashAlgorithm.Create(HashName)));
                filesInfos.Add(fileInfo);
                Console.WriteLine($"Adding Directory: {fileInfo.Name} | Hash: {fileInfo.HashString}");
                GenerateDirectoryList(filePath, Directory.GetDirectories(filePath), Directory.GetFiles(filePath));
            }
            files.AddFiles(filesInfos);
            files.AddFiles(dirInfos);
            files.DumpToJson(Path.Combine(dirPath, MdpApi.Resources.ListFileName));
        }
    }
}
