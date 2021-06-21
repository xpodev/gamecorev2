namespace GameCore
{
    /// <summary>
    /// This class represents an item on a <c>GameCore.DatabaseTable</c>.
    /// </summary>
    /// See <see cref="GameCore.DatabaseTable"/>
    /// See <see cref="GameCore.DatabaseTable.RegisterObject(DatabaseItem)"/>
    public abstract class DatabaseItem
    {
        /// <summary>
        /// The unique-id of the object. This is also the index of the item in the database.
        /// </summary>
        public int Id
        {
            get;
            protected set;
        }

        /// <summary>
        /// The name of the object.
        /// </summary>
        public string Name
        {
            get;
            protected set;
        }

        /// <summary>
        /// Constructs a new <c>DatabaseItem</c>.
        /// </summary>
        /// <param name="name">The name of the new item. This will be used in database lookups.</param>
        public DatabaseItem(string name)
        {
            Id = -1;
            Name = name;
        }

        /// <summary>
        /// This method is called just before this item is registered on a database.
        /// </summary>
        /// <param name="id">The new id of this item</param>
        internal void OnRegister(int id)
        {
            if (Id != -1)
            {
                throw new System.InvalidOperationException("Tried to register an object that is already registered within a database");
            }
            Id = id;
        }

        /// <summary>
        /// This method is called after the item was removed from the database.
        /// </summary>
        internal void OnUnregister()
        {
            Id = -1;
        }
    }
}
