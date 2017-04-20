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
            double[][][] sequences =
            {
                new double[][] // sequence 1
                {
                    new double[] { 1.0, 2.1 },
                    new double[] { 3.1, 4.1 }, // observation 2 of sequence 1
                    new double[] { 5.2, 6.2 }, // observation 3 of sequence 1
                    new double[] { 1.3, 2.3 }, // observation 1 of sequence 1
                    new double[] { 3.1, 4.2 }, // observation 2 of sequence 1
                    new double[] { 5.3, 6.1 }, // observation 3 of sequence 1
                    new double[] { 1.3, 2.3 }, // observation 1 of sequence 1
                    new double[] { 3.1, 4.2 }, // observation 2 of sequence 1
                    new double[] { 5.3, 6.1 }, // observation 3 of sequence 1
                },
                new double[][] // sequence 1
                {
                    new double[] { 3.1, 4.0 }, // observation 2 of sequence 1
                    new double[] { 5.2, 6.2 }, // observation 3 of sequence 1
                    new double[] { 1.1, 2.2 }, // observation 1 of sequence 1
                    new double[] { 3.2, 4.2 }, // observation 2 of sequence 1
                    new double[] { 5.1, 6.1 }, // observation 3 of sequence 1
                    new double[] { 1.1, 2.2 }, // observation 1 of sequence 1
                    new double[] { 3.1, 4.2 }, // observation 2 of sequence 1
                    new double[] { 5.3, 6.2 }, // observation 3 of sequence 1
                },
                new double[][] // sequence 1
                {
                    new double[] { 3.0, 4.0 },
                    new double[] { 5.1, 6.1 }, // observation 3 of sequence 1
                    new double[] { 1.2, 2.2 }, // observation 1 of sequence 1
                    new double[] { 3.1, 4.3 }, // observation 2 of sequence 1
                    new double[] { 5.1, 6.2 }, // observation 3 of sequence 1
                    new double[] { 1.4, 2.5 }, // observation 1 of sequence 1
                    new double[] { 3.4, 4.5 }, // observation 2 of sequence 1
                    new double[] { 5.4, 6.2 }, // observation 3 of sequence 1
                },
            };

            MultivariateNormalDistribution[] mul = new MultivariateNormalDistribution[]
            {
                new MultivariateNormalDistribution(dimension: 2),
                new MultivariateNormalDistribution(dimension: 2),
                new MultivariateNormalDistribution(dimension: 2)
            };

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

            double[] weights = teacher.LogWeights;
            Console.WriteLine("weights:");
            for(int i = 0;i < weights.Length;i++)
            {
                Console.Write("{0} ", weights[i]);
            }

            double mean1 = hmm_mul[0].Mean[0];
            double mean2 = hmm_mul[1].Mean[0];

            Console.WriteLine("{0} ", mean1);
            Console.WriteLine("{0} ", mean2);

            double[,] a = hmm_mul[0].Covariance;

            Console.WriteLine("{0} {1} ", a[0, 0], a[0, 1]);
            Console.WriteLine("{0} {1} ", a[1, 0], a[1, 1]);

            string file = "G:\\GitHubKinect\\HMM_Model\\HMM_Model\\wave.txt";
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

        public void recognize(double[][] recognize_seq)
        {
            string file = "G:\\GitHubKinect\\HMM_Model\\HMM_Model\\wave.txt";
            HiddenMarkovModel<MultivariateNormalDistribution, double[]> model = CreateFromFile(file);  //取出训练好的模型

            int[] States = model.Decide(recognize_seq);          //输出待测序列的隐含状态

            for (int i = 0; i < States.Length; i++)
            {
                System.Console.Write("{0} ", States[i]);
            }

            System.Console.WriteLine();
            System.Console.WriteLine();
            System.Console.WriteLine();

            double p = Math.Exp(model.LogLikelihood(recognize_seq));   //输出待测序列的可能性

            System.Console.WriteLine("{0}", p);

        }
    }
}
