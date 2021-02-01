using System.Text;

namespace GameCore.Net
{
    public class UTF8String : StringBase
    {
        public UTF8String() : base(Encoding.UTF8) { }

        public UTF8String(string str) : base(str, Encoding.UTF8) { }
    }
}
