using System;


namespace GameCore
{
    /// <summary>
    /// Use this class to create a database for your object.
    /// </summary>
    public class Database
    {
        /// <summary>
        /// The main table of this database.
        /// </summary>
        private readonly DatabaseTable db;

        /// <summary>
        /// Constructs a new database.
        /// </summary>
        /// <param name="name">The name of the root table. This does not affect any lookups</param>
        public Database(string name)
        {
            db = new DatabaseTable(name);
        }

        /// <summary>
        /// Creates a new table.
        /// </summary>
        /// <param name="name">The name of the new table to create</param>
        public void CreateTable(string name)
        {
            if (db.ObjectExists(name))
            {
                throw new InvalidOperationException($"Table \'{name}\' already exists");
            }
            db.RegisterObject(new DatabaseTable(name));
        }

        /// <summary>
        /// Removes a table.
        /// </summary>
        /// <param name="name">The name of the table to remove.</param>
        public void RemoveTable(string name)
        {
            db.RemoveObject(name);
        }

        /// <summary>
        /// Removes a table.
        /// </summary>
        /// <param name="id">The id of the table to remove</param>
        public void RemoveTable(int id)
        {
            db.RemoveObject(id);
        }

        /// <summary>
        /// Gets an items using its path.
        /// </summary>
        /// <param name="path">The path of the item in the form "table_name.item_name"</param>
        /// <returns>The object that presents at the end of the path. <c>null</c> if that path doesn't exist</returns>
        public DatabaseItem GetItem(string path)
        {
            string[] parts = path.Split('.');
            DatabaseTable table = db;
            for (int i = 0; i < parts.Length - 1; i++)
            {
                table = table.GetObject(parts[i]) as DatabaseTable;
                if (table == null)
                {
                    return null;
                }
            }
            return table.GetObject(parts[parts.Length - 1]);
        }
    }
}
