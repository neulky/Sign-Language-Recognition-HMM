using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;

namespace Sign_Language_Recognition_HMM.connect
{
    class Server
    {
        private Socket sSocket;
        private Socket serverSocket;     

        public void connect()
        {
            int port = 6000;
            string host = "127.0.0.2";

            IPAddress ip = IPAddress.Parse(host);
            IPEndPoint ipe = new IPEndPoint(ip, port);

            sSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            sSocket.Bind(ipe);
            sSocket.Listen(0);
            Console.WriteLine("监听已经打开，请等待");

            serverSocket = sSocket.Accept();        //建立连接
            Console.WriteLine("连接已经建立");
            
        }
        
        public double[][][] receiveMessage()         //接收数据
        {
            double[][][] recognize_seq;
            string recStr = "";
            byte[] recByte = new byte[4096];
            int bytes = serverSocket.Receive(recByte, recByte.Length, 0);
            recStr += Encoding.ASCII.GetString(recByte, 0, bytes);

            //Console.WriteLine("{0}", recStr);

            recognize_seq = transitionFromStringToDouble(recStr);       //将接收到的的字符串转换为double[][]数组
            return recognize_seq;
        }
        public void sendMessage(string sendStr)
        {
            byte[] sendByte = Encoding.UTF8.GetBytes(sendStr);
            serverSocket.Send(sendByte, sendByte.Length, 0);
        }
        public void Close()
        {
            serverSocket.Close();
            sSocket.Close();
        }
        public double[][][] transitionFromStringToDouble(string recStr)
        {
            double[][][] recSequence;
            List<List<double[]>> sequences = new List<List<double[]>>();

            double[] double_point;
            string[] hand = recStr.Split('#');
            string[][] point = new string[hand.Length][];
            Console.WriteLine(hand.Length);
            //for (int i = 0; i < hand.Length;i++)
            //{
            //    Console.WriteLine(hand[i]);
            //}

            for (int i = 0; i < hand.Length; i++)
            {
                List<double[]> sequence = new List<double[]>();
                point[i] = hand[i].Split('@');
                if(hand[i] != null)
                {
                    for (int j = 0; j < point[i].Length - 1;j++)
                    {
                        double_point = Array.ConvertAll(point[i][j].Split(','), Double.Parse);
                        sequence.Add(double_point);
                    }
                }
                sequences.Add(sequence);
            }

            recSequence = new double[sequences.Count][][];
            for (int i = 0; i < sequences.Count;i++)
            {
                recSequence[i] = new double[sequences[i].Count][];
                for(int j = 0;j < sequences[i].Count;j++)
                {
                    recSequence[i][j] = new double[sequences[i][j].Length];
                    for(int k = 0;k < sequences[i][j].Length;k++)
                    {
                        recSequence[i][j][k] = sequences[i][j][k];
                    }
                }
            }
            return recSequence;
        }
    }
}
