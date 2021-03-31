using System.Text;

namespace GameCore.NetAlpha
{
    public abstract class StringBase : StructArray<byte>
    {
        public Encoding Encoding
        {
            get;
            protected set;
        }

        public string Text
        {
            get
            {
                return Encoding.GetString(data);
            }
        }

        public StringBase(Encoding encoding) : base(null)
        {
            Encoding = encoding;
        }

        public StringBase(string str, Encoding encoding) : base(encoding.GetBytes(str))
        {
            Encoding = encoding;
        }

        public override string ToString()
        {
            return Text;
        }
    }
}
