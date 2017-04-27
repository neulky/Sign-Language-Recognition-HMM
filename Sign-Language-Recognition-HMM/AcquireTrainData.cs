using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Sign_Language_Recognition_HMM
{
    class AcquireTrainData
    {
        public double[][][] train_sequences;

        public void AcquireModelSequences()
        {
            
            string sourceDirectory = "G:\\GitHubKinect\\HMM_Model_3\\Right_Train_Data\\2";      //打开即将要训练的存放训练数据的文件夹
            var txtFiles = Directory.EnumerateFiles(sourceDirectory, "*.txt");

            List<List<double[]>> sequences = new List<List<double[]>>();   //训练序列
            List<double[]> sequence = new List<double[]>();

            foreach(string currentFile in txtFiles)
            {
                string textData = System.IO.File.ReadAllText(currentFile);

                string[] result = textData.Split('@');

                double[] double_point;
                for(int i = 0;i < result.Length - 1;i++)
                {
                    double_point = Array.ConvertAll(result[i].Split(','), Double.Parse);
                    sequence.Add(double_point);

                }
                sequences.Add(sequence);
            }

            double[][][] temp_sequenses = new double[sequences.Count][][];
            for (int i = 0; i < sequences.Count; i++)                     //初始化交错数组
            {
                temp_sequenses[i] = new double[sequences[i].Count][];
                for(int j = 0;j < sequences[i].Count;j++)
                {
                    temp_sequenses[i][j] = new double[sequences[i][j].Length];
                }
            }

            temp_sequenses[0][0][0] = 0;

            for (int i = 0; i < sequences.Count; i++)
            {
                for (int j = 0; j < sequences[i].Count; j++)
                {
                    for (int k = 0; k < sequences[i][j].Length; k++)
                    {
                        temp_sequenses[i][j][k] = sequences[i][j][k] / 100;
                    }
                }
            }

            train_sequences = temp_sequenses;  //将获得的变量赋给公有属性 train_sequences
        }
    } 
}
