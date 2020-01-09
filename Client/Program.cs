using System;

namespace Client
{
    class Program
    {
        private static AsyncSocketClient client;
        static void Main(string[] args)
        {
            client = new AsyncSocketClient("127.0.0.1", 8976, 8977);
            client.Start();
        }
    }
}
