using System.Text;

namespace GameCore.Net
{
    public class UTF16String :  StringBase
    {
        public UTF16String() : base(Encoding.Unicode) { }

        public UTF16String(string str) : base(str, Encoding.Unicode) { }
    }
}
