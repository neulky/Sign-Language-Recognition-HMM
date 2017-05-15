using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Accord.Statistics.Distributions.Multivariate;
using Accord.Statistics.Models.Markov.Learning;
using Accord.Statistics.Models.Markov;
using Accord.Statistics.Distributions.Fitting;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace Sign_Language_Recognition_HMM
{
    class HMM
    {
        private static double[,] hmm_transitions;
        private static double[] hmm_probabilities;
        private static MultivariateNormalDistribution[] hmm_mul;

        public HMM(double[,] Hmm_transitions, double[] Hmm_probabilities, MultivariateNormalDistribution[] Hmm_mul)
        {
            hmm_transitions = Hmm_transitions;
            hmm_probabilities = Hmm_probabilities;
            hmm_mul = Hmm_mul;
        }
        public static void SaveToFile(string path, HiddenMarkovModel<MultivariateNormalDistribution, double[]> model)
        {
            using (var file = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.Write))
            {
                (new BinaryFormatter()).Serialize(file, model);
            }
        }

        public static HiddenMarkovModel<MultivariateNormalDistribution, double[]> CreateFromFile(string path)
        {
            HiddenMarkovModel<MultivariateNormalDistribution, double[]> model;
            using (var file = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                model = (HiddenMarkovModel<MultivariateNormalDistribution, double[]>)(new BinaryFormatter()).Deserialize(file);
            }
            return model;

        }
        public void train(double[][][] seq)
        {
            var model = new HiddenMarkovModel<MultivariateNormalDistribution, double[]>(
                transitions: hmm_transitions,
                emissions: hmm_mul,
                probabilities: hmm_probabilities,
                logarithm: false
                );
            var teacher = new BaumWelchLearning<MultivariateNormalDistribution, double[]>(model)
            {
                Tolerance = 0.00000001,
                Iterations = 0,
                FittingOptions = new NormalOptions()
                {
                    Regularization = 1e-5
                }
            };

            teacher.Learn(seq);

            double mean1 = hmm_mul[0].Mean[0];
            double mean2 = hmm_mul[1].Mean[0];

            Console.WriteLine("{0} ", mean1);
            Console.WriteLine("{0} ", mean2);

            double[,] a = hmm_mul[0].Covariance;

            Console.WriteLine("{0} {1} ", a[0, 0], a[0, 1]);
            Console.WriteLine("{0} {1} ", a[1, 0], a[1, 1]);

            string file = "G:\\GitHubKinect\\HMM_Model_3\\HMM_Model_Hog\\爸爸\\righthand.txt";       //将训练得到的模型存入指定的文档中
            SaveToFile(file, model);

            double likelihood = Math.Exp(teacher.LogLikelihood);
            Console.WriteLine("log_p = {0}", likelihood);
            Console.WriteLine();
            Console.WriteLine();

            double[][] A = model.LogTransitions;
            for (int i = 0; i < A.Length; i++)
            {
                for (int j = 0; j < A[i].Length; j++)
                {
                    Console.Write("{0}  ", Math.Exp(A[i][j]));
                }
                Console.WriteLine();
            }

            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine();

            double[] I = model.LogInitial;
            for (int i = 0; i < I.Length; i++)
            {
                Console.Write("{0}  ", Math.Exp(I[i]));
            }

            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine();

        }

        public string recognize(double[][][] recognize_seq)     //从这里编写
        {
            int handFlag = 2;     //默认为2；handFlag = 1表示只有一只手参加该手语，handFlag = 2表示两只手同时参加了该手语
            if(recognize_seq[0].Length == 0 || recognize_seq[1].Length == 0)
                handFlag = 1;

            string sourceDirectory = "G:\\GitHubKinect\\HMM_Model_3\\HMM_Model";       //打开存放模型文件的文件夹
            
            List<string> modelNames = new List<string>();
            List<double> modelRecognitionResults = new List<double>();
            
            var directorys = Directory.EnumerateDirectories(sourceDirectory);
            int modelCount = 0;
            foreach (string currentDirectory in directorys)
            {
                var txtFiles = Directory.EnumerateFiles(currentDirectory, "*.txt");
                if (txtFiles.Count() == handFlag)     //如果该模板与待识别手语都使用一只手或两只手，则进行进一步匹配
                {
                    HiddenMarkovModel<MultivariateNormalDistribution, double[]>[] models = new HiddenMarkovModel<MultivariateNormalDistribution, double[]>[txtFiles.Count()];
                    int count = 0;
                    foreach(string currentFile in txtFiles)
                    {
                        models[count] = CreateFromFile(currentFile);
                        count++;
                    }
                    double result = 0;

                    if(handFlag == 1)
                    {
                        result = Math.Exp(models[0].LogLikelihood(recognize_seq[1]));     //只有一只手参与该手语 默认为右手

                        int[] states= models[0].Decide(recognize_seq[1]);
                        for(int i = 0;i < states.Length;i++)
                        {
                            Console.Write("{0} ", states[i]);
                        }
                        Console.WriteLine();
                    }
                    if(handFlag == 2)
                    {
                        result = Math.Exp(models[0].LogLikelihood(recognize_seq[0]))
                            + Math.Exp(models[1].LogLikelihood(recognize_seq[1]));
                        int[] states0 = models[0].Decide(recognize_seq[0]);
                        int[] states1 = models[1].Decide(recognize_seq[1]);
                        for(int i = 0;i < states0.Length;i++)
                        {
                            Console.Write("{0} ", states0[i]);
                        }
                        Console.WriteLine();
                        Console.WriteLine(Math.Exp(models[0].LogLikelihood(recognize_seq[0])));
                        for(int i = 0;i < states1.Length;i++)
                        {
                            Console.Write("{0} ", states1[i]);
                        }
                        Console.WriteLine();
                        Console.WriteLine(Math.Exp(models[1].LogLikelihood(recognize_seq[1])));
                        
                    }
                    Console.WriteLine("model {0}:{1}", currentDirectory, result);
                    modelNames.Add(currentDirectory.Substring(currentDirectory.LastIndexOf("\\") + 1));    //将该手语名称加入数组
                    modelRecognitionResults.Add(result);   //将对应的匹配率加入数组
                }
                modelCount++;
            }

            double max = 0;
            int model_flag = 0;
            for(int i = 0;i < modelRecognitionResults.Count;i++)
            {
                if (max < modelRecognitionResults[i])
                {
                    max = modelRecognitionResults[i];
                    model_flag = i;
                }
            }
            return modelNames[model_flag];
        }
    }
}
