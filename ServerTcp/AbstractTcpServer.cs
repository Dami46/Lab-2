using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ServerTcpLibrary
{

    public abstract class AbstractServerTcp
    {
        #region Fields
        protected IPAddress iPAddress;
        protected int port;
        protected int buffer_size = 1024;
        protected bool running;
        protected TcpListener tcpListener;
        protected TcpClient tcpClient;
        protected NetworkStream networkStream;
        #endregion
        #region Properties
        /// <summary>
        /// This property gives access to the IP address of a server instance. Property can't be changed when the Server is running. 
        /// </summary>
        public IPAddress IPAddress { get => iPAddress; set { if (!running) iPAddress = value; else throw new Exception("nie można zmienić adresu IP kiedy serwer jest uruchomiony"); } }
        /// <summary>
        /// This property gives access to the port of a server instance. Property can't be changed when the Server is running. Setting invalid port numbers will cause an exception. 
        /// </summary>
        public int Port
        {
            get => port; set
            {
                int tmp = port;
                if (!running) port = value; else throw new Exception("nie można zmienić portu kiedy serwer jest uruchomiony");
                if (!checkPort())
                {
                    port = tmp;
                    throw new Exception("błędna wartość portu");
                }
            }
        }
        /// <summary>
        /// This property gives access to the buffer size of a server instance. Property can't be changed when the Server is running. Setting invalid size numbers will cause an exception. 
        /// </summary>
        public int Buffer_size
        {
            get => buffer_size; set
            {
                if (value < 0 || value > 1024 * 1024 * 64) throw new Exception("błędny rozmiar pakietu");
                if (!running) buffer_size = value; else throw new Exception("nie można zmienić rozmiaru pakietu kiedy serwer jest uruchomiony");
            }
        }

        protected TcpListener TcpListener { get => tcpListener; set => tcpListener = value; }
        protected TcpClient TcpClient { get => tcpClient; set => tcpClient = value; }
        protected NetworkStream Stream { get => networkStream; set => networkStream = value; }
        #endregion
        #region Constructors
        /// <summary>
        /// A default constructor. It doesn't start the server. Invalid port numbers will thrown an exception.
        /// </summary>
        /// <param name="IP">IP address of the server instance.</param>
        /// <param name="port">Port number of the server instance.</param>
        public AbstractServerTcp(IPAddress IP, int port)
        {
            running = false;
            IPAddress = IP;
            Port = port;
            if (!checkPort())
            {
                Port = 8000;
                throw new Exception("błędna wartość portu, ustawiam port na 8000");
            }
        }
        #endregion
        #region Functions
        /// <summary>
        /// This function will return false if Port is set to a value lower than 1024 or higher than 49151.
        /// </summary>
        /// <returns>An information wether the set Port value is valid.</returns>
        protected bool checkPort()
        {
            if (port < 1024 || port > 49151) return false;
            return true;
        }
        /// <summary>
        /// This function starts the listener.
        /// </summary>
        protected void StartListening()
        {
            TcpListener = new TcpListener(IPAddress, Port);
            TcpListener.Start();
            Console.WriteLine("The Server has started on port " + port);
        }
        /// <summary>
        /// This function waits for the Client connection.
        /// </summary>
        protected abstract void AcceptClient();
        /// <summary>
        /// This function implements Echo and transmits the data between server and client.
        /// </summary>
        protected abstract void BeginDataTransmission(NetworkStream stream);
        /// <summary>
        /// This function fires off the default server behaviour. It interrupts the program.
        /// </summary>
        public abstract void Start();
        #endregion
    }
}

