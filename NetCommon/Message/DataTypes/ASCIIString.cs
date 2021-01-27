using System.Text;

namespace GameCore.Net
{
    public class ASCIIString : StringBase
    {
        public ASCIIString() : base(Encoding.ASCII) { }

        public ASCIIString(string str) : base(str, Encoding.ASCII) { }
    }
}
