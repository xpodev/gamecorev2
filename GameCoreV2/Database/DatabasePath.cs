using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace GameCore
{
    public sealed class DatabasePath : IDatabasePath
    {
        private const string DBPathRegexContainerIDGroupName = "containerID";
        private const string DBPathRegexPathGroupName = "path";
        private const string DBPathRegexItemGroupName = "item";
        private const string DBPathRegexString = 
            @"^(?<" + 
            DBPathRegexContainerIDGroupName + 
            ">[A-Za-z0-9]*?)(:(?<" + 
            DBPathRegexPathGroupName + 
            ">([A-Za-z0-9]*/)*)(?<" + 
            DBPathRegexItemGroupName + 
            ">[A-Za-z0-9]*)?)?$";

        private const char DBPathSeparator = '/';

        private static readonly Regex DBPathRegex = new Regex(DBPathRegexString);

        private readonly string m_container = "";
        private readonly string[] m_path = Array.Empty<string>();
        private readonly string m_item = "";

        public DatabasePath(string path)
        {
            if (!IsValidPath(path))
            {
                throw new ArgumentException($"The path \'{path}\' is not a valid DB path");
            }
            Match match = DBPathRegex.Match(path);

            m_container = match.Groups[DBPathRegexContainerIDGroupName].Value;
            m_path = match.Groups[DBPathRegexPathGroupName].Value.Split(DBPathSeparator);
            m_item = match.Groups[DBPathRegexItemGroupName].Value;
        }

        public string ContainerID => m_container;

        public string[] PathParts => m_path;

        public string Item => m_item;

        public static bool IsValidPath(string path)
        {
            return DBPathRegex.IsMatch(path);
        }
    }
}
