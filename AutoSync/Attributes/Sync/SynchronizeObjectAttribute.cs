using System;

namespace GameCore.Net.Sync
{
    using UIDType = Int32;

    [AttributeUsage(AttributeTargets.All, Inherited = true)]
    public abstract class SynchronizeObjectAttribute : SynchronizeAttribute
    {
        class DefaultUIDGenerator : IUIDGenerator<UIDType>
        {
            private UIDType m_uid = 0;

            public UIDType GenerateUID()
            {
                return m_uid++;
            }
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
