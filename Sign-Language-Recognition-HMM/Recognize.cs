using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Accord.Statistics.Distributions.Multivariate;
using Sign_Language_Recognition_HMM.connect;

namespace Sign_Language_Recognition_HMM
{
    class Recognize
    {
        public static void Main(string[] args)
        {
            AcquireTrainData test = new AcquireTrainData();              //获取待训练数据
            test.AcquireModelSequences();
            //初始化转移矩阵
            double[,] transitions = new double[,]
            {
                { 0.1, 0.1, 0.2, 0.1, 0.1, 0.4 },
                { 0.1, 0.1, 0.2, 0.1, 0.1, 0.4 },
                { 0.2, 0.1, 0.1, 0.1, 0.2, 0.3 },
                { 0.1, 0.1, 0.2, 0.1, 0.2, 0.3 },
                { 0.1, 0.1, 0.2, 0.1, 0.1, 0.4 },
                { 0.2, 0.1, 0.1, 0.1, 0.1, 0.4 }
            };
            //初始化初始概率矩阵
            double[] probabilites = new double[]
            {
                0.1, 0.1, 0.1, 0.1, 0.2, 0.4
            };
            //初始化连续正太分布
            MultivariateNormalDistribution[] mul = new MultivariateNormalDistribution[]
            {
                new MultivariateNormalDistribution(dimension: 3),
                new MultivariateNormalDistribution(dimension: 3),
                new MultivariateNormalDistribution(dimension: 3),
                new MultivariateNormalDistribution(dimension: 3),
                new MultivariateNormalDistribution(dimension: 3),
                new MultivariateNormalDistribution(dimension: 3)
            };

            HMM Wave_HMM = new HMM(transitions, probabilites, mul);   //创建HMM模型

            //Wave_HMM.train(test.train_sequences);                     //训练HMM模型

            //KinectData kinectData = new KinectData();               //有一个版本是通过C#编写的代码直接获取手坐标
            //kinectData.GetKinectData();
            //double[][][] recognition_sequences = kinectData.kinectdata_seq;      
            //Wave_HMM.recognize(recognition_sequences[1]);            


            //AcquireTestData testData = new AcquireTestData();       //有一个版本为读取文件中的测试数据 
            //testData.AcquireTestSequences();
            //string result = Wave_HMM.recognize(testData.test_sequences);          
            //Console.WriteLine("识别结果为：{0}", result);

            Server server = new Server();             //打开该服务器，获取客户端传来的数据
            server.connect();
            string result = Wave_HMM.recognize(server.receiveMessage());
            server.sendMessage(result);          //将识别结果返回客户端

            Console.WriteLine("识别结果为：{0}", result);

            server.Close();    //关闭连接



        }
        
    }
}
