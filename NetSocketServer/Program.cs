using System;

namespace NetSocketServer
{
    class Program
    {
        static int Main(string[] args)
        {
            Server.StartListening();
            return 0;
        }
    }
}
