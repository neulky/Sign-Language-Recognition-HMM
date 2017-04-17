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
    class Program
    {
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
        static void Main(string[] args)
        {
            double[][][] sequences =
            {
                new double[][] // sequence 1
                {
                    new double[] { 1.0, 2.1 }, // observation 1 of sequence 1
                    new double[] { 3.1, 4.1 }, // observation 2 of sequence 1
                    new double[] { 5.1, 6.2 }, // observation 3 of sequence 1
                    new double[] { 1.2, 2.2 }, // observation 1 of sequence 1
                    new double[] { 3.1, 4.1 }, // observation 2 of sequence 1
                    new double[] { 5.2, 6.2 }, // observation 3 of sequence 1
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
                transitions: new double[,]
                {
                    { 0.1, 0.2, 0.7},
                    { 0.2, 0.3, 0.5},
                    { 0.1, 0.5, 0.4}
                },
                emissions: mul,
                probabilities: new double[]
                {
                    0.1, 0.5, 0.4
                },
                logarithm: false

                );
            var teacher = new BaumWelchLearning<MultivariateNormalDistribution, double[]>(model)
            {
                Tolerance = 0.0001,
                Iterations = 0,
                FittingOptions = new NormalOptions()
                {
                    Regularization = 1e-5
                }
            };

            teacher.Learn(sequences);

            double mean1 = mul[0].Mean[0];
            double mean2 = mul[1].Mean[0];

            Console.WriteLine("{0} ", mean1);
            Console.WriteLine("{0} ", mean2);

            double[,] a = mul[0].Covariance;

            Console.WriteLine("{0} {1} ", a[0, 0], a[0, 1]);
            Console.WriteLine("{0} {1} ", a[1, 0], a[1, 1]);

            string file = "G:\\GitHubKinect\\HMM_Model\\test.txt";
            SaveToFile(file,model);

            //double logLikelihood = teacher.LogLikelihood;

            HiddenMarkovModel<MultivariateNormalDistribution, double[]> model1 = CreateFromFile(file);

            // See the likelihood of the sequences learned
            double a1 = Math.Exp(model1.LogLikelihood(new[] { 
                 
                
                    new double[] { 5.1, 6.1 }, // observation 3 of sequence 1
                    new double[] { 1.2, 2.2 }, // observation 1 of sequence 1
                    new double[] { 3.1, 4.3 }, // observation 2 of sequence 1
                    new double[] { 5.1, 6.2 }, // observation 3 of sequence 1
                    new double[] { 1.4, 2.5 }, // observation 1 of sequence 1
                    new double[] { 3.4, 4.5 }, // observation 2 of sequence 1
                    new double[] { 5.4, 6.2 }, // observation 3 of sequence 1

            })); // 0.000208

            //double a2 = Math.Exp(model.LogLikelihood(new[] { 
            //    new double[] { 2, 2 }, 
            //    new double[] { 9, 8  },
            //    new double[] { 1, 0 }})); // 0.0000376

            //// See the likelihood of an unrelated sequence
            //double a3 = Math.Exp(model.LogLikelihood(new[] { 
            //    new double[] { 8, 7 }, 
            //    new double[] { 9, 8  },
            //    new double[] { 1, 0 }})); // 2.10 x 10^(-89)
            
            Console.WriteLine("a1 = {0}", a1);
            //Console.WriteLine("a2 = {0}", a2);
            //Console.WriteLine("a3 = {0}", a3);

            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine();

            int[] S = model1.Decide(new double[][] { 
                
                
                new double[] { 5.1, 6.1 }, 
                new double[] { 1.5, 2 },
                new double[] { 3, 4 },
                new double[] { 5, 6 }, 
                new double[] { 1, 2 },
                new double[] { 3, 4 },
                new double[] { 5, 6 }, 
                new double[] { 1, 2 },
                new double[] { 5, 6 }, 
            });

            for(int i=0;i<S.Length;i++)
            {
                Console.Write("{0} ", S[i]);
            }

            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine();

            double[][] A = model1.LogTransitions;
            for(int i=0;i<A.Length;i++)
            {
                for(int j=0;j<A[i].Length;j++)
                {
                    Console.Write("{0}  ", Math.Exp(A[i][j]));
                }
                Console.WriteLine();
            }

            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine();

            double[] I = model1.LogInitial;
            for(int i=0;i<I.Length;i++)
            {
                Console.Write("{0}  ", Math.Exp(I[i]));
            }

            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine();
            
        }
    }
}
