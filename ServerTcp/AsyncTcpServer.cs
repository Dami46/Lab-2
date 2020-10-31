using ServerTcpLibrary;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ServerTcp
{
    /// <summary>
    /// Klasa zawierająca funkcje implementacyjne serwera
    /// </summary>
    public class AsyncTcpServer : AbstractServerTcp
    {
        byte[] guess = new byte[256];
        byte[] msg = new byte[256];
        byte[] msg2 = new byte[256];
        byte[] msg3 = new byte[256];
        StreamReader streamReader;

        /// <summary>
        /// Konstruktor domyślny klasy
        /// </summary>
        public AsyncTcpServer(IPAddress IP, int port) : base(IP, port)
        {
            this.guess = new ASCIIEncoding().GetBytes(" Zgadnij liczbe od 0 do 9 \r \n");
            this.msg = new ASCIIEncoding().GetBytes(" Brawo udalo ci sie zgadnac liczbe! \r \n");
            this.msg2 = new ASCIIEncoding().GetBytes(" Wybrano zla wartosc \r \n");
            this.msg3 = new ASCIIEncoding().GetBytes("Chcesz kontynuowac rozgrywke ? (1-tak 0-nie) \r \n");
        }
        public delegate void TransmissionDataDelegate(NetworkStream stream);

    
        protected override void AcceptClient()
        {
            while (true)
            {
                tcpClient = TcpListener.AcceptTcpClient();
                networkStream = tcpClient.GetStream();
                TransmissionDataDelegate transmissionDelegate = new TransmissionDataDelegate(BeginDataTransmission);
                transmissionDelegate.BeginInvoke(networkStream, TransmissionCallback, tcpClient);
            }
        }
       
        private void TransmissionCallback(IAsyncResult ar)
        {
            tcpClient.Close();
        }

        /// <summary>
        /// Funkcja implementująca działanie serwera
        /// </summary>
        public override void Start()
        {
            StartListening();
            //transmission starts within the accept function
            AcceptClient();
        }

        protected override void BeginDataTransmission(NetworkStream stream)
        {
            Random random = new Random();
            int number = random.Next(10);
            int nextGame = 1;

            Console.WriteLine("Number to guess: " + number);
            while (nextGame == 1)
            {
                try
                {
                    Console.WriteLine("Client connected");
                    while (tcpClient.Connected)
                    {
                        streamReader = new StreamReader(networkStream);
                        networkStream.Write(guess, 0, guess.Length);

                        var guessedVal = streamReader.ReadLine();
                        String time = DateTime.Now.ToString("h:mm:ss");
                        Console.WriteLine(time + " -> " + guessedVal);
                        int guessedVal2;
                        try
                        {
                            guessedVal2 = Int32.Parse(guessedVal);
                        }
                        catch (FormatException e)
                        {
                            guessedVal2 = 12;
                        }

                        if (guessedVal2 > 11 || guessedVal2 < 0)
                        {
                            networkStream.Write(msg2, 0, msg2.Length);
                        }

                        if (number.Equals(guessedVal2))
                        {
                            networkStream.Write(msg, 0, msg.Length);

                            networkStream.Write(msg3, 0, msg3.Length);

                            number = random.Next(10);
                            Console.WriteLine("Number to guess: " + number);
                        }
                    }
                }  catch (Exception ex) {
                  Console.WriteLine(ex);
                }
            }
        }
    }
}

