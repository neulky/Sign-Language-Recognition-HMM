using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Accord.Statistics.Distributions.Multivariate;

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

            Wave_HMM.recognize(testData.test_sequences);            //测试概率大小

        }
        
    }
}
