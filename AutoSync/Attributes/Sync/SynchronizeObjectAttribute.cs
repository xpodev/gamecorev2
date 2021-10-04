using System;

namespace GameCore.Net.Sync
{
    using UIDType = Int32;

    [AttributeUsage(AttributeTargets.All, Inherited = true)]
    public abstract class SynchronizeObjectAttribute : SynchronizeAttribute
    {
        class DefaultUIDGenerator : IUIDGenerator<UIDType>
        {
            private readonly System.Collections.Generic.HashSet<UIDType> m_ids = new System.Collections.Generic.HashSet<UIDType>();

            public UIDType GenerateUID()
            {
                //Console.WriteLine("Generating Id...");
                UIDType potetentialUid;
                do
                {
                    potetentialUid = Guid.NewGuid().ToString().GetHashCode();
                } while (!m_ids.Add(potetentialUid));
                return potetentialUid;
            }

            public void Remove(UIDType uid) => m_ids.Remove(uid);
        }

        public static IUIDGenerator<UIDType> UIDGenerator { get; set; }

        public UIDType Id { get; set; }

        public SynchronizeObjectAttribute()
        {
            Id = UIDGenerator.GenerateUID();
            //Console.WriteLine("Sync UID: " + Id);
        }

        static SynchronizeObjectAttribute()
        {
            UIDGenerator = new DefaultUIDGenerator();
        }
    }
}
