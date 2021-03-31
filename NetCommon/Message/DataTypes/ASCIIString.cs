using System.Text;

namespace GameCore.NetAlpha
{
    public class ASCIIString : StringBase
    {
        public ASCIIString() : base(Encoding.ASCII) { }

        public ASCIIString(string str) : base(str, Encoding.ASCII) { }
    }
}
