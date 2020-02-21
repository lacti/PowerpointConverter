using System;

namespace ConvertWin32Core
{
    class Program
    {
        static void Main(string[] args)
        {
            using (var server = new Server())
            {
                server.Start();
#if DEBUG
                server.OpenBrowser();
#endif
                Console.ReadKey(true);
            }
        }
    }
}
