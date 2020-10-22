using System;

namespace GameCore.Mdp
{
    // TODO: Rewrite this API (although it's pretty nice)
    public static class MdpApi
    {
        public static class Xml
        {
            public const string HashNameAttributeName = "hash";
            public const string RemoteServerAttributeName = "host";

            public const string SourceAttributeName = "source";
            public const string SourceFormatAttributeName = "format";
            public const string WriteModeAttributeName = "mode";
        }

        public static class Header
        {
            public const string MdpVersion = "Mdp-Version";
        }

        public static class Resources
        {
            public const string IgnoreCharacter = ".";
            public const string ListFileName = IgnoreCharacter + "list";

            public const string FilesHost = "files";
            public const string FilesList = FilesHost + "/" + "{0}" + "/" + ListFileName;

            public static string GetFilesDownloadUrl(string host, string file = "")
            {
                return host.TrimEnd('/') + "/" + FilesHost + "/" + file + "/";
            }

            public static string GetFilesListUrl(string host, string dir)
            {
                return new Uri(host.Trim().Trim('/') + "/" + string.Format(FilesList, dir)).AbsoluteUri;
            }
        }

        public static class FileList
        {
            public static class Json
            {
                public const string FilesListElementName = "files";

                public const string FileNamePropertyName = "name";
                public const string FileHashPropertyName = "hash";
            }

            public static class Xml
            {
                public const string FilesListNodeName = "Files";

                public const string FileTagName = "File";
                public const string DirectoryTagName = "Directory";

                public const string FileNameAttributeName = "name";
                public const string FileHashAttributeName = "hash";
            }
        }
    }
}
