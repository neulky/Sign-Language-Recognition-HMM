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
                { 0.1, 0.1, 0.2, 0.1, 0.1, 0.1, 0.1, 0.1, 0.1 },
                { 0.1, 0.1, 0.2, 0.1, 0.1, 0.1, 0.1, 0.1, 0.1 },
                { 0.2, 0.1, 0.1, 0.1, 0.1, 0.1, 0.1, 0.1, 0.1 },
                { 0.1, 0.1, 0.2, 0.1, 0.1, 0.1, 0.1, 0.1, 0.1 },
                { 0.1, 0.1, 0.2, 0.1, 0.1, 0.1, 0.1, 0.1, 0.1 },
                { 0.2, 0.1, 0.1, 0.1, 0.1, 0.1, 0.1, 0.1, 0.1 },
                { 0.1, 0.1, 0.2, 0.1, 0.1, 0.1, 0.1, 0.1, 0.1 },
                { 0.1, 0.1, 0.1, 0.1, 0.2, 0.1, 0.1, 0.1, 0.1 },
                { 0.1, 0.1, 0.1, 0.1, 0.2, 0.1, 0.1, 0.1, 0.1 }
            };
            //初始化初始概率矩阵
            double[] probabilites = new double[]
            {
                0.1, 0.1, 0.1, 0.1, 0.2, 0.1, 0.1, 0.1, 0.1
            };
            //初始化连续正太分布
            MultivariateNormalDistribution[] mul = new MultivariateNormalDistribution[]
            {
                new MultivariateNormalDistribution(dimension: 2),
                new MultivariateNormalDistribution(dimension: 2),
                new MultivariateNormalDistribution(dimension: 2),
                new MultivariateNormalDistribution(dimension: 2),
                new MultivariateNormalDistribution(dimension: 2),
                new MultivariateNormalDistribution(dimension: 2),
                new MultivariateNormalDistribution(dimension: 2),
                new MultivariateNormalDistribution(dimension: 2),
                new MultivariateNormalDistribution(dimension: 2)
            };

            HMM Wave_HMM = new HMM(transitions, probabilites, mul);   //创建HMM模型

            //Wave_HMM.train(test.train_sequences);                     //训练HMM模型

            AcquireTestData testData = new AcquireTestData();       //获取待测试序列
            testData.AcquireTestSequences();

            //KinectData kinectData = new KinectData();
            //kinectData.GetKinectData();

            //for(int i = 0;i < kinectData.kinectdata_seq[1].Length;i++)
            //{
            //    Console.WriteLine("({0},{1})",kinectData.kinectdata_seq[1][i][0],kinectData.kinectdata_seq[1][i][1]);
            //}
            
            //double[][][] recognition_sequences = kinectData.kinectdata_seq;
            //Wave_HMM.recognize(recognition_sequences[1]);            //测试概率大小
            
            //Wave_HMM.recognize(testData.test_sequences);            //测试概率大小

            Server server = new Server();
            server.connect();
            
            string result = Wave_HMM.recognize(server.receiveMessage());
            Console.WriteLine("识别结果为：{0}", result);
        }
        
    }
}
