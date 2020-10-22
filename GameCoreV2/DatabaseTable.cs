using System.Collections.Generic;


namespace GameCore
{
    /// <summary>
    /// This class is represents a table that holds items.
    /// </summary>
    public class DatabaseTable : DatabaseItem
    {
        /// <summary>
        /// This list contains all the items in the table.
        /// </summary>
        protected List<DatabaseItem> Items = new List<DatabaseItem>();

        /// <summary>
        /// This queue contains the available ids to make sure the whole list is being utilized.
        /// </summary>
        private readonly Queue<int> AvailableIds = new Queue<int>();

        /// <summary>
        /// Constructs a new <c>DatabaseTable</c>
        /// </summary>
        /// <param name="name">The name of the new table. This will be used in database lookups</param>
        public DatabaseTable(string name) : base(name)
        {

        }

        /// <summary>
        /// Check whether an item exists in the table.
        /// </summary>
        /// <param name="name">The name of the item</param>
        /// <returns><c>true</c> if the item exists, <c>false</c> otherwise</returns>
        public bool ObjectExists(string name)
        {
            return GetObject(name) != null;
        }

        /// <summary>
        /// Check whether an item exists in the table.
        /// </summary>
        /// <param name="id">The id of the item</param>
        /// <returns><c>true</c> if the item exists, <c>false</c> otherwise</returns>
        public bool ObjectExists(int id)
        {
            return GetObject(id) != null;
        }

        /// <summary>
        /// Registers an object in this table.
        /// </summary>
        /// <param name="obj">The object to register</param>
        public void RegisterObject(DatabaseItem obj)
        {
            int objectId = Items.Count;
            if (AvailableIds.Count > 0)
            {
                objectId = AvailableIds.Dequeue();
            }
            obj.OnRegister(objectId);
            Items.Add(obj);
        }

        /// <summary>
        /// Removes an object from the table.
        /// </summary>
        /// <param name="name">The name of the object to remove</param>
        /// <returns>The item that was removed.</returns>
        public DatabaseItem RemoveObject(string name)
        {
            DatabaseItem item = GetObject(name);
            if (item == null)
            {
                throw new KeyNotFoundException($"Could not remove \'{name}\' from \'{Name}\' because it doesn\'t exist");
            }
            Items.RemoveAt(item.Id);
            AvailableIds.Enqueue(item.Id);
            item.OnUnregister();
            return item;
        }

        /// <summary>
        /// Removes an object from the table.
        /// </summary>
        /// <param name="id">The id of the object to remove</param>
        /// <returns>The item that was removed.</returns>
        public DatabaseItem RemoveObject(int id)
        {
            DatabaseItem item = GetObject(id);
            if (item == null)
            {
                throw new KeyNotFoundException($"Could not remove Item({id}) from \'{Name}\' because it doesn\'t exist");
            }
            Items.RemoveAt(item.Id);
            AvailableIds.Enqueue(item.Id);
            item.OnUnregister();
            return item;
        }

        /// <summary>
        /// Get an item from the table.
        /// </summary>
        /// <param name="name">The name of the item</param>
        /// <returns>The requested item. <c>null</c> if no item was found</returns>
        public DatabaseItem GetObject(string name)
        {
            foreach (DatabaseItem objectBase in Items)
            {
                if (objectBase.Name == name)
                {
                    return objectBase;
                }
            }
            return null;
        }

        /// <summary>
        /// Get an item from the table.
        /// </summary>
        /// <param name="id">The id of the item</param>
        /// <returns>The requested item. <c>null</c> if no item was found</returns>
        public DatabaseItem GetObject(int id)
        {
            if (id < 0 || id >= Items.Count)
            {
                return null;
            }
            return Items[id];
        }
    }
}
