using System;
using System.Xml;
using System.Net;
using System.Collections.Generic;
using System.IO;

namespace GameCore.Mdp
{
    public class MdpClient
    {
        private readonly Dictionary<string, Action<XmlNode>> actions = new Dictionary<string, Action<XmlNode>>();

        private MdpFileList FileList
        {
            get;
            set;
        }

        public MdpClient()
        {
            Configuration = new MdpConfiguration();
            FileList = new MdpFileList();

            actions.Add("AddFiles", AddFiles);
            actions.Add("Config", Configure);
        }

        public MdpConfiguration Configuration
        {
            get;
            private set;
        }

        public void LoadXml(XmlNode mdpRootNode)
        {
            foreach (XmlNode child in mdpRootNode.ChildNodes)
            {
                actions[child.Name].Invoke(child);
            }
        }

        public void LoadFile(string path)
        {
            XmlDocument xml = new XmlDocument();
            xml.Load(path);
            LoadXml(xml.DocumentElement);
        }

        public static MdpClient FromXml(XmlNode mdpRootNode)
        {
            MdpClient mdp = new MdpClient();
            mdp.LoadXml(mdpRootNode);
            return mdp;
        }

        public static MdpClient FromFile(string path)
        {
            XmlDocument xml = default;
            xml.Load(path);
            return FromXml(xml.DocumentElement);
        }

        public void Run(string path)
        {
            MdpFileInfo[] corruptedFiles = FileList.ValidateFiles(path, Configuration);
            DownloadCorruptedFiles(path, corruptedFiles);
        }

        private void DownloadCorruptedFiles(string path, MdpFileInfo[] infos, string remoteDirUrl = "")
        {
            WebClient client = new WebClient();
            string baseDlUrl = MdpApi.Resources.GetFilesDownloadUrl(Configuration.RemoteFileServer, remoteDirUrl);
            foreach (MdpFileInfo fileInfo in infos)
            {
                string fullPath = Path.GetFullPath(Path.Combine(path, fileInfo.Name));
                Directory.GetParent(fullPath).Create();

                if (fileInfo is MdpDirectoryInfo dirInfo)
                {
                    MdpRemoteServer remote = new MdpRemoteServer();
                    string json = remote.GetFilesList(Configuration.RemoteFileServer, remoteDirUrl + "/" + dirInfo.Name);
                    MdpFileList list = new MdpFileList();
                    list.AddFilesFromJson(json);
                    DownloadCorruptedFiles(fullPath, list.Files, remoteDirUrl + "/" + dirInfo.Name);
                }
                else
                {
                    client.DownloadFile(baseDlUrl + fileInfo.Name, fullPath);
                }
            }
        }

        public void Configure(XmlNode configNode)
        {
            Configuration = new MdpConfiguration(System.Security.Cryptography.HashAlgorithm.Create(configNode.Attributes[MdpApi.Xml.HashNameAttributeName].Value))
            {
                RemoteFileServer = configNode.Attributes[MdpApi.Xml.RemoteServerAttributeName].Value
            };
        }

        private T GetEnumFromString<T>(string value, T fallback = default) where T : Enum
        {
            try
            {
                T result = (T)Enum.Parse(typeof(T), value, true);
                return result;
            }
            catch (ArgumentException)
            {
                return fallback;
            }
        }

        public void AddFiles(XmlNode filesNode)
        {
            string sourceString = filesNode.Attributes[MdpApi.Xml.SourceAttributeName].Value;
            string formatString = filesNode.Attributes[MdpApi.Xml.SourceFormatAttributeName].Value;
            string modeString = filesNode.Attributes[MdpApi.Xml.WriteModeAttributeName].Value;
            string sourceValue = filesNode.InnerText;

            MdpFileList.ListSource source = GetEnumFromString(sourceString, (MdpFileList.ListSource)(-1));
            MdpFileList.ListFormatType format = GetEnumFromString<MdpFileList.ListFormatType>(formatString);
            MdpFileList.ListWriteMode mode = GetEnumFromString<MdpFileList.ListWriteMode>(modeString);

            if (source == MdpFileList.ListSource.Text)
            {
                sourceValue = (format == MdpFileList.ListFormatType.Xml) ? filesNode.InnerXml : filesNode.InnerText;
            }

            FileList.AddFiles(sourceValue, source, format, mode);
        }
    }
}
