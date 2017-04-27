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

            string file = "G:\\GitHubKinect\\HMM_Model_3\\HMM_Model\\2.txt";       //将训练得到的模型存入指定的文档中
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

        public string recognize(double[][] recognize_seq)
        {
            string sourceDirectory = "G:\\GitHubKinect\\HMM_Model_3\\HMM_Model";       //打开存放模型文件的文件夹
            var txtFiles = Directory.EnumerateFiles(sourceDirectory, "*.txt");

            int count = 0;
            HiddenMarkovModel<MultivariateNormalDistribution, double[]>[] models = new HiddenMarkovModel<MultivariateNormalDistribution, double[]>[10];
            foreach (string currentFile in txtFiles)
            {
                models[count] = CreateFromFile(currentFile);
                count++;
            }

            for (int i = 0; i < count; i++)
            {
                int[] States = models[i].Decide(recognize_seq);          //输出待测序列的隐含状态

                for (int j = 0; j < States.Length; j++)
                {
                    System.Console.Write("{0} ", States[j]);
                }

                System.Console.WriteLine();
            }

            System.Console.WriteLine();
            System.Console.WriteLine();

            double max = 0;
            int model_flag = 0;
            for (int i = 0; i < count; i++)
            {
                double p = Math.Exp(models[i].LogLikelihood(recognize_seq));   //输出待测序列的可能性
                if (i == 0)
                {
                    max = p;
                }
                else
                {
                    if (max < p)
                    {
                        max = p;
                        model_flag = i;
                    }
                }
                System.Console.WriteLine("model {0}: {1}", i, p);
            }
            return model_flag.ToString();
        }
    }
}
