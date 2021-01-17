using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameCore.Net
{
    public struct NetString<T> : ICustomInsert<T>, ICustomExtract<T> where T : struct, Enum
    {

        private byte[] data;
        private uint size;

        public string Text
        {
            get
            {
                return Encoding.ASCII.GetString(data);
            }
        }

        public NetString(string str)
        {
            data = Encoding.ASCII.GetBytes(str);
            size = (uint)data.Length;
        }

        public override string ToString()
        {
            return Text;
        }

        public void Insert(Message<T> message)
        {
            message.Insert(data);
            message.Insert(size);
        }

        public void Extract(Message<T> message)
        {
            size = message.Extract<uint>();
            data = message.ExtractBytes((int)size);
        }
    }
}
