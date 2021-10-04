using System;

namespace GameCore.Test
{
    class Program
    {
        static void Main(string[] args)
        {
            foreach ((int i, string s) in new Enumerate<string>(new string[] { "Hello", "My", "Name", "Is" }))
            {
                Console.WriteLine(i + " " + s);
            }
            return;
        }
    }
}
