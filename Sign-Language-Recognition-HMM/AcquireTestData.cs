using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Sign_Language_Recognition_HMM
{
    class AcquireTestData
    {
        public double[][] test_sequences;

        public void AcquireTestSequences()
        {

            string txtFiles = "G:\\GitHubKinect\\HMM_Model\\Right_Test_Data\\wave.txt";
           
            List<double[]> sequence = new List<double[]>();

            string textData = System.IO.File.ReadAllText(txtFiles);

            string[] result = textData.Split('@');

            double[] double_point;
            for (int i = 0; i < result.Length - 1; i++)
            {
                //System.Console.WriteLine("{0} {1}", result[i],i);

                double_point = Array.ConvertAll(result[i].Split(','), Double.Parse);
                sequence.Add(double_point);

            }

            double[][] temp_sequenses = new double[sequence.Count][];

            for (int i = 0; i < sequence.Count;i++)
            {
                temp_sequenses[i] = new double[sequence[i].Length];
            }

            for (int i = 0; i < sequence.Count;i++)
            {
                for(int j = 0;j <sequence[i].Length;j++)
                {
                    temp_sequenses[i][j] = sequence[i][j];
                }
            }

            test_sequences = temp_sequenses;  //将获得的变量赋给公有属性 train_sequences
        }
    }
}
