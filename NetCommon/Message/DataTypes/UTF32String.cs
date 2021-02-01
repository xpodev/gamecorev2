using System.Text;

namespace GameCore.Net
{
    public class UTF32String : StringBase
    {
        public UTF32String() : base(Encoding.UTF32) { }

        public UTF32String(string str) : base(str, Encoding.UTF32) { }
    }
}
