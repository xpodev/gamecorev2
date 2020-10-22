using System;
using System.Xml;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Newtonsoft.Json;


namespace GameCore.Mdp
{
    public class MdpFileList
    {
        public static byte[] ConvertHexStringToByteArray(string hexString)
        {
            if (hexString.Length % 2 != 0)
            {
                throw new ArgumentException(string.Format("The binary key cannot have an odd number of digits: {0}", hexString));
            }

            byte[] data = new byte[hexString.Length / 2];
            for (int index = 0; index < data.Length; index++)
            {
                string byteValue = hexString.Substring(index * 2, 2);
                data[index] = byte.Parse(byteValue, System.Globalization.NumberStyles.HexNumber);
            }

            return data;
        }

        public static string ConvertByteArrayToHexString(byte[] hash)
        {
            StringBuilder Result = new StringBuilder(hash.Length * 2);
            string HexAlphabet = "0123456789ABCDEF";

            foreach (byte B in hash)
            {
                Result.Append(HexAlphabet[(B >> 4)]);
                Result.Append(HexAlphabet[(B & 0xF)]);
            }

            return Result.ToString();
        }

        public enum ListWriteMode
        {
            Append,
            Overwrite
        }

        public enum ListFormatType
        {
            Xml,
            Json
        }

        public enum ListSource
        {
            Local,
            RemoteServer,
            Text
        }

        [JsonIgnore]
        private readonly List<MdpFileInfo> files = new List<MdpFileInfo>();

        public MdpFileList()
        {
            UpdateFiles();
        }

        [JsonProperty(MdpApi.FileList.Json.FilesListElementName)]
        public MdpFileInfo[] Files
        {
            get;
            private set;
        }

        public void DumpToJson(string path)
        {
            string json = JsonConvert.SerializeObject(this);
            File.WriteAllText(path, json);
        }

        public void AddFiles(string s, ListSource source, ListFormatType format, ListWriteMode mode = ListWriteMode.Append)
        {
            switch (source)
            {
                case ListSource.RemoteServer:
                    AddFilesFromRemote(s, format, mode);
                    break;
                case ListSource.Local:
                    AddFilesFromLocal(s, format, mode);
                    break;
                case ListSource.Text:
                    AddFilesFromText(s, format, mode);
                    break;
                default:
                    break;
            }
        }

        public void AddFilesFromRemote(string host, ListFormatType format = ListFormatType.Json, ListWriteMode mode = ListWriteMode.Append)
        {
            MdpRemoteServer mdpRemote = new MdpRemoteServer();
            string result = mdpRemote.GetFilesList(host, "");
            AddFilesFromText(result, format, mode);
        }

        public void AddFilesFromLocal(string path, ListFormatType format = ListFormatType.Json, ListWriteMode mode = ListWriteMode.Append)
        {
            string fileContent = File.ReadAllText(path);
            AddFilesFromText(fileContent, format, mode);
        }

        public void AddFilesFromText(string text, ListFormatType format = ListFormatType.Json, ListWriteMode mode = ListWriteMode.Append)
        {
            switch (format)
            {
                case ListFormatType.Xml:
                    AddFilesFromXml(text, mode);
                    break;
                case ListFormatType.Json:
                    AddFilesFromJson(text, mode);
                    break;
            }
        }

        public void AddFilesFromXml(string s, ListWriteMode mode = ListWriteMode.Append)
        {
            XmlDocument xml = new XmlDocument();
            xml.LoadXml(s);
            XmlElement root = xml.DocumentElement;
            List<MdpFileInfo> filesInfo = new List<MdpFileInfo>();
            foreach (XmlNode fileNode in root.ChildNodes)
            {
                string fileRelativePath = fileNode.Attributes[MdpApi.FileList.Xml.FileNameAttributeName].Value;
                string fileHash = fileNode.Attributes[MdpApi.FileList.Xml.FileHashAttributeName].Value;
                byte[] hash = ConvertHexStringToByteArray(fileHash);
                MdpFileInfo fileInfo;
                if (fileNode.Name == MdpApi.FileList.Xml.DirectoryTagName)
                {
                    fileInfo = new MdpDirectoryInfo(fileRelativePath, hash);
                }
                else
                {
                    fileInfo = new MdpFileInfo(fileRelativePath, hash);
                }
                filesInfo.Add(fileInfo);
            }
            AddFiles(filesInfo, mode);
        }

        public void AddFilesFromJson(string s, ListWriteMode mode = ListWriteMode.Append)
        {
            MdpFileList mdpFileList = JsonConvert.DeserializeObject<MdpFileList>(s);
            List<MdpFileInfo> filesInfos = new List<MdpFileInfo>();
            foreach (MdpFileInfo fileInfo in mdpFileList.Files)
            {
                string fileRelativePath = fileInfo.Name;
                MdpFileInfo file;
                if (fileRelativePath.EndsWith("/"))
                {
                    file = new MdpDirectoryInfo(fileRelativePath, fileInfo.Hash);
                }
                else
                {
                    file = new MdpFileInfo(fileRelativePath, fileInfo.Hash);
                }
                filesInfos.Add(file);
            }
            AddFiles(filesInfos, mode);
        }

        public void AddFiles(IEnumerable<MdpFileInfo> filesInfo, ListWriteMode mode = ListWriteMode.Append)
        {
            if (mode == ListWriteMode.Overwrite)
            {
                files.Clear();
            }
            files.AddRange(filesInfo);
            UpdateFiles();
        }

        public void AddFiles(MdpDirectoryInfo directoryInfo, ListWriteMode mode = ListWriteMode.Append)
        {
            AddFiles(new MdpFileInfo[] { directoryInfo }, mode);
        }

        public MdpFileInfo[] ValidateFiles(string path, MdpConfiguration config)
        {
            List<MdpFileInfo> corruptedFiles = new List<MdpFileInfo>();
            foreach (MdpFileInfo fileInfo in Files)
            {
                try
                {
                    if (!fileInfo.CompareFile(path, config.Hash))
                    {
                        corruptedFiles.Add(fileInfo);
                    }
                }
                catch (FileNotFoundException)
                {
                    corruptedFiles.Add(fileInfo);
                }
                catch (DirectoryNotFoundException)
                {
                    corruptedFiles.Add(fileInfo);
                }
            }
            return corruptedFiles.ToArray();
        }

        private void UpdateFiles()
        {
            Files = files.ToArray();
        }
    }
}
