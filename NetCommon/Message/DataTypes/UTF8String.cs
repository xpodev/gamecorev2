using System.Text;

namespace GameCore.Net
{
    public class UTF8String : StructArray<byte>
    {
        public UTF8String() : base(null) { }

        public string Text
        {
            get
            {
                return Encoding.ASCII.GetString(data);
            }
        }

        public UTF8String(string str) : base(Encoding.UTF8.GetBytes(str))
        {
        }

        public override string ToString()
        {
            return Text;
        }
    }
}
