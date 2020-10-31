using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using ServerTcp;

namespace Lab_2
{   
    /// <summary>
    /// Klasa zawierająca metode uruchamiającą Main
    /// </summary>
    class Program
    {   /// <summary>
        /// Funkcja uruchamiająca tworząca instancje klasy ServerLibrary
        /// i uruchamiająca serwer
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            AsyncTcpServer tcpServer = new AsyncTcpServer(IPAddress.Parse("127.0.0.1"), 8000);
            tcpServer.Start();
        }
    }
}
