using System;
using System.IO;

namespace GameCore.Database
{
    public class Database<T> : DatabaseTable<string, DatabaseTable<T>>, IDatabase
    {
        private const string GameCoreDatabaseURIScheme = "gcdb";

        public ReturnT GetItem<ReturnT>(string path) where ReturnT : class
        {
            Uri uri = new Uri(path, UriKind.Absolute);
            if (uri.Scheme != GameCoreDatabaseURIScheme)
            {
                throw new ArgumentException($"Invalid URI scheme '{uri.Scheme}'. must be '{GameCoreDatabaseURIScheme}'");
            }
            string databaseName = uri.Host;
            string databasePath = uri.LocalPath;
            databaseName += '.' + Directory.GetParent(path).FullName.Replace('/', '.');
            return this[databaseName][Path.GetFileName(databasePath)] as ReturnT;
        }
    }
}
