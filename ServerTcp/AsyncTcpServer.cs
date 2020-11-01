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
        byte[] msg4 = new byte[256];
        StreamReader streamReader;
        string one = " Zgadnij liczbe od 0 do 9 \r \n";
        string two = " Brawo udalo ci sie zgadnac liczbe! \r \n";
        string three = " Wybrano zla wartosc \r \n";
        string four = "Chcesz kontynuowac rozgrywke ? (1-tak 0-nie) \r \n";
        string five = "Rozgrywka zakończona \r \n";


        /// <summary>
        /// Konstruktor domyślny klasy
        /// </summary>
        public AsyncTcpServer(IPAddress IP, int port) : base(IP, port)
        {
            this.guess = new ASCIIEncoding().GetBytes(one);
            this.msg = new ASCIIEncoding().GetBytes(two);
            this.msg2 = new ASCIIEncoding().GetBytes(three);
            this.msg3 = new ASCIIEncoding().GetBytes(four);
            this.msg4 = new ASCIIEncoding().GetBytes(five);
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
            StringBuilder gameLog = new StringBuilder();
            Random random = new Random();
            int number = random.Next(10);
            int nextGame = 1;
            Console.WriteLine("Number to guess: " + number);
            gameLog.Append("Number to guess: " + number + Environment.NewLine);
            while (nextGame == 1)
            {
                try
                {
                    Console.WriteLine("Client connected");
                    gameLog.Append("Client connected  \n" + Environment.NewLine);
                    while (tcpClient.Connected)
                    {
                        streamReader = new StreamReader(networkStream);
                        networkStream.Write(guess, 0, guess.Length);
                        gameLog.Append(one + Environment.NewLine);
                        var guessedVal = streamReader.ReadLine();
                        String time = DateTime.Now.ToString("h:mm:ss");
                        Console.WriteLine(time + " -> " + guessedVal);
                        gameLog.Append(time + " -> " + guessedVal + Environment.NewLine);
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
                            gameLog.Append(three + Environment.NewLine);
                        }

                        if (number.Equals(guessedVal2))
                        {
                            networkStream.Write(msg, 0, msg.Length);
                            gameLog.Append(two + Environment.NewLine);
                            Console.WriteLine("Client guessed the number");
                            gameLog.Append("Client guessed the number " + Environment.NewLine);

                            networkStream.Write(msg3, 0, msg3.Length);
                            gameLog.Append(four + Environment.NewLine);
                            var continueGame = streamReader.ReadLine();
                            nextGame = Int32.Parse(continueGame);
                            if (nextGame == 0)
                            {
                                networkStream.Write(msg4, 0, msg4.Length);
                                gameLog.Append(five + Environment.NewLine);
                                saveGameToFile(gameLog);
                                tcpClient.Close();
                                break;
                            }
                            else
                            {
                                number = random.Next(10);
                                Console.WriteLine("Number to guess: " + number);
                                gameLog.Append("Number to guess: " + number + Environment.NewLine);

                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Zaden klient nie jest polonczony z serwerem");
                    gameLog.Append("Zaden klient nie jest polonczony z serwerem" + Environment.NewLine);
                }
            }
        }

        public void saveGameToFile(StringBuilder gameLog)
        {
            String path = Directory.GetCurrentDirectory();
            String filename = path + @"\result.txt";
            int i = 1;
            while (File.Exists(filename))
            {
                filename = path + @"\result" + i + @".txt";
                i++;
            }
            using (FileStream fs = File.Create(filename))
            {
            }
            using (StreamWriter streamWriter = new StreamWriter(filename, true))
            {
                streamWriter.WriteLine(gameLog);
            }
        }
    }
}

