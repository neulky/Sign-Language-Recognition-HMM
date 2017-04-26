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
        
        public double[][] receiveMessage()         //接收数据
        {
            double[][] recognize_seq;
            string recStr = "";
            byte[] recByte = new byte[4096];
            int bytes = serverSocket.Receive(recByte, recByte.Length, 0);
            recStr += Encoding.ASCII.GetString(recByte, 0, bytes);

            Console.WriteLine("{0}", recStr);

            recognize_seq = transitionFromStringToDouble(recStr);       //将接收到的的字符串转换为double[][]数组
            return recognize_seq;
        }
        public void sendMessage(string sendStr)
        {
            byte[] sendByte = Encoding.ASCII.GetBytes(sendStr);
            serverSocket.Send(sendByte, sendByte.Length, 0);
        }
        public void Close()
        {
            serverSocket.Close();
            sSocket.Close();
        }
        public double[][] transitionFromStringToDouble(string recStr)
        {
            double[][] recSequence;
            List<double[]> sequence = new List<double[]>();

            string[] point = recStr.Split('@');

            double[] double_point;
            for (int i = 0; i < point.Length - 1;i++)
            {
                double_point = Array.ConvertAll(point[i].Split(','), Double.Parse);
                sequence.Add(double_point);
            }

            recSequence = new double[sequence.Count][];
            for (int i = 0; i < sequence.Count;i++)
            {
                recSequence[i] = new double[sequence[i].Length];
                for(int j = 0;j < sequence[i].Length;j++)
                {
                    recSequence[i][j] = sequence[i][j] / 100;
                }
            }
            return recSequence;
        }
    }
}
