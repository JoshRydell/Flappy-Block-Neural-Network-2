using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
namespace Flappy_Block_Neural_Network_2
{
    class NeuralNetwork
    {
        public Matrix[] Weights;
        public Matrix[] Bias;
        Matrix[] Network;
        public NeuralNetwork(int[] Layout)
        {
            if (Layout.Length < 3)
            {
                throw new Exception("Network must have at least one hidden layer.");
            }
            for (int i = 0; i < Layout.Length; i++)
            {
                if (Layout[i] < 1)
                {
                    throw new Exception("All layers must have one or more nodes.");
                }
            }
            Weights = new Matrix[Layout.Length - 1];
            Bias = new Matrix[Layout.Length - 1];
            Network = new Matrix[Layout.Length];



            for (int i = 0; i < Weights.Length; i++)
            {
                Weights[i] = Matrix.Randomise(Layout[i + 1], Layout[i]);
                Bias[i] = Matrix.Randomise(Layout[i + 1], 1);
            }
        }
        private NeuralNetwork(Matrix[] Weights, Matrix[] Bias)
        {
            if (Weights.Length != Bias.Length)
            {
                throw new Exception("Dimension Error");
            }
            Network = new Matrix[Weights.Length + 1];

            this.Weights = new Matrix[Weights.Length];
            this.Bias = new Matrix[Bias.Length];
            for (int i = 0; i < Weights.Length; i++)
            {
                this.Weights[i] = Weights[i].Copy();
                this.Bias[i] = Bias[i].Copy();
            }

        }
        public NeuralNetwork CrossOver(NeuralNetwork Parent)
        {
            if(Parent.Weights.Length != Weights.Length && Parent.Bias.Length != Bias.Length)
            {
                throw new Exception("Networks not the same size.");
            }

            Random rand = new Random(Guid.NewGuid().GetHashCode());


            Matrix[] NewWeights = new Matrix[Weights.Length];
            Matrix[] NewBias = new Matrix[Bias.Length];

            for (int i = 0; i < NewWeights.Length; i++)
            {
                if(Weights[i].Rows != Parent.Weights[i].Rows || Weights[i].Columns != Parent.Weights[i].Columns || Bias[i].Rows != Parent.Bias[i].Rows )
                {
                    throw new Exception("Matrices wrong size.");
                }

                NewWeights[i] = new Matrix(Weights[i].Rows, Weights[i].Columns);
                NewBias[i] = new Matrix(Bias[i].Rows, 1);
                int mid = rand.Next(Weights[i].Rows*Weights[i].Columns);
                int index = 0;

                for(int j = 0; j < Weights[i].Rows; j++)
                {
                    for(int k = 0; k < Weights[i].Columns; k++)
                    {
                        if(index < mid)
                        {
                            NewWeights[i][j, k] = Weights[i][j,k];
                        }
                        else
                        {
                            NewWeights[i][j, k] = Parent.Weights[i][j,k];
                        }
                        index++;
                    }
                }
                index = 0;
                mid = rand.Next(Bias[i].Rows);
                for (int j = 0; j < Bias[i].Rows; j++)
                {
                    if (index < mid)
                    {
                        NewBias[i][j, 0] = Bias[i][j,0];
                    }
                    else
                    {
                        NewBias[i][j, 0] = Parent.Bias[i][j,0];
                    }
                    index++;
                }

            }

            return new NeuralNetwork(NewWeights, NewBias);
        }
        public void Mutate(float Rate)
        {
            Random rand = new Random(Guid.NewGuid().GetHashCode());
            for(int i = 0; i < Weights.Length; i++)
            {
                for(int j = 0; j < Weights[i].Rows;  j++)
                {
                    for(int k = 0; k < Weights[i].Columns; k++)
                    {
                        if (rand.NextDouble() < Rate)
                        {
                            Weights[i][j, k] += (float)(rand.NextDouble() / 5 - 0.1);
                        }
                        
                    }
                }
                for (int j = 0; j < Bias[i].Rows; j++)
                {
                    if (rand.NextDouble() < Rate)
                    { 
                        Bias[i][j, 0] += (float)(rand.NextDouble() / 5 - 0.1);
                    }
                }
            }
        }
        public NeuralNetwork(string FileName)
        {
            BinaryReader reader = new BinaryReader(File.Open(Directory.GetCurrentDirectory() + "//" + FileName, FileMode.Open));
            int length = reader.ReadInt32();
            Network = new Matrix[length + 1];
            Weights = new Matrix[length];
            Bias = new Matrix[length];
            for (int i = 0; i < length; i++)
            {
                int Rows = reader.ReadInt32();
                int Columns = reader.ReadInt32();
                Weights[i] = new Matrix(Rows, Columns);
                for (int j = 0; j < Weights[i].Rows; j++)
                {
                    for (int k = 0; k < Weights[i].Columns; k++)
                    {
                        Weights[i][j, k] = (float)reader.ReadDouble();
                    }
                }
                Rows = reader.ReadInt32();
                Columns = reader.ReadInt32();
                Bias[i] = new Matrix(Rows, Columns);
                for (int j = 0; j < Rows; j++)
                {
                    Bias[i][j, 0] = (float)reader.ReadDouble();
                }
            }
        }
        public NeuralNetwork(string FileName, bool InDirectory)
        {
            if (InDirectory)
            {
                FileName = Directory.GetCurrentDirectory() + "//" + FileName;
            }
            BinaryReader reader = new BinaryReader(File.Open(FileName, FileMode.Open));
            int length = reader.ReadInt32();
            Network = new Matrix[length + 1];
            Weights = new Matrix[length];
            Bias = new Matrix[length];
            for (int i = 0; i < length; i++)
            {
                int Rows = reader.ReadInt32();
                int Columns = reader.ReadInt32();
                Weights[i] = new Matrix(Rows, Columns);
                for (int j = 0; j < Weights[i].Rows; j++)
                {
                    for (int k = 0; k < Weights[i].Columns; k++)
                    {
                        Weights[i][j, k] = (float)reader.ReadDouble();
                    }
                }
                Rows = reader.ReadInt32();
                Columns = reader.ReadInt32();
                Bias[i] = new Matrix(Rows, Columns);
                for (int j = 0; j < Rows; j++)
                {
                    Bias[i][j, 0] = (float)reader.ReadDouble();
                }
            }
        }
        public void SaveToFile(string Name)
        {
            BinaryWriter writer = new BinaryWriter(File.Open(Directory.GetCurrentDirectory() + "//" + Name + ".nn", FileMode.Create));
            writer.Write(Network.Length - 1);
            for (int i = 0; i < Weights.Length; i++)
            {
                writer.Write(Weights[i].Rows);
                writer.Write(Weights[i].Columns);
                for (int j = 0; j < Weights[i].Rows; j++)
                {
                    for (int k = 0; k < Weights[i].Columns; k++)
                    {
                        writer.Write((double)Weights[i][j, k]);
                    }
                }

                writer.Write(Bias[i].Rows);
                writer.Write(Bias[i].Columns);
                for (int j = 0; j < Weights[i].Rows; j++)
                {
                    writer.Write((double)Bias[i][j, 0]);
                }
            }
            writer.Close();
        }
        public NeuralNetwork Copy()
        {
            return new NeuralNetwork(Weights, Bias);
        }
        public float[] FeedForward(float[] inputs)
        {
            Network[0] = new Matrix(inputs);
            for (int i = 0; i < Network.Length - 1; i++)
            {
                Network[i + 1] = Weights[i] * Network[i] + Bias[i];
                Network[i + 1] = Activate(Network[i + 1]);
            }
            return Network[Network.Length - 1].ToArray();

        }
        public void Train(float[] inputs, float[] targets)
        {
            const float LearningRate = 0.1f;

            Network[0] = new Matrix(inputs);
            for (int i = 0; i < Network.Length - 1; i++)
            {
                Network[i + 1] = Weights[i] * Network[i] + Bias[i];
                Network[i + 1] = Activate(Network[i + 1]);
            }

            Matrix Error = new Matrix(targets) - Network[Network.Length - 1];
            for (int i = Network.Length - 1; i > 0; i--)
            {
                Matrix gradient = Deactivate(Network[i]);
                gradient = Matrix.ElementWiseMult(gradient, Error);
                gradient *= LearningRate;
                Bias[i - 1] += gradient;
                Matrix DeltaWeights = gradient * Network[i - 1].Transpose();
                Weights[i - 1] += DeltaWeights;
                Error = Weights[i - 1].Transpose() * Error;
            }

        }
        private static Matrix Activate(Matrix A)
        {
            Matrix B = new Matrix(A.Rows, A.Columns);
            for (int i = 0; i < A.Rows; i++)
            {
                for (int j = 0; j < A.Columns; j++)
                {
                    B[i, j] = (1 / (float)(1 + Math.Exp(-A[i, j])));
                }
            }

            return B;
        }
        private static Matrix Deactivate(Matrix A)
        {
            Matrix B = new Matrix(A.Rows, A.Columns);
            for (int i = 0; i < A.Rows; i++)
            {
                for (int j = 0; j < A.Columns; j++)
                {
                    B[i, j] = A[i, j] * (1 - A[i, j]);
                }
            }

            return B;
        }
    }
    class Matrix
    {
        public float[,] Mat { get; private set; }
        public int Columns { get; private set; }
        public int Rows { get; private set; }
        public Matrix(int Rows, int Columns)
        {
            this.Columns = Columns;
            this.Rows = Rows;
            Mat = new float[Rows, Columns];
            for (int i = 0; i < Rows; i++)
            {
                for (int j = 0; j < Columns; j++)
                {
                    Mat[i, j] = 0;
                }
            }
        }
        public Matrix Copy()
        {
            Matrix A = new Matrix(Rows, Columns);

            for(int i = 0; i < Rows; i++)
            { 
                for(int j = 0; j < Columns; j++)
                {
                    A[i, j] = Mat[i, j];
                }
            }
            return A;
        }
        public Matrix(float[] Array)
        {
            Columns = 1;
            Rows = Array.Length;
            Mat = new float[Rows, Columns];
            for (int i = 0; i < Rows; i++)
            {
                Mat[i, 0] = Array[i];
            }
        }
        public float[] ToArray()
        {
            float[] arr = new float[Columns * Rows];
            int index = 0;
            for (int i = 0; i < Rows; i++)
            {
                for (int j = 0; j < Columns; j++)
                {
                    arr[index] = Mat[i, j];
                    index++;
                }
            }

            return arr;
        }
        public Matrix Transpose()
        {
            Matrix C = new Matrix(Columns, Rows);
            for (int i = 0; i < Rows; i++)
            {
                for (int j = 0; j < Columns; j++)
                {
                    C[j, i] = Mat[i, j];
                }
            }

            return C;
        }
        public static Matrix operator *(Matrix A, Matrix B)
        {
            if (A.Columns != B.Rows)
            {
                throw new Exception("Dimension error");
            }
            Matrix C = new Matrix(A.Rows, B.Columns);

            for (int j = 0; j < C.Columns; j++)
            {
                for (int i = 0; i < C.Rows; i++)
                {
                    for (int a = 0; a < B.Rows; a++)
                    {
                        C[i, j] += A[i, a] * B[a, j];
                    }
                }
            }

            return C;
        }
        public static Matrix ElementWiseMult(Matrix A, Matrix B)
        {
            if (A.Rows != B.Rows && A.Columns != B.Columns)
            {
                throw new Exception("Dimension error");
            }
            Matrix C = new Matrix(A.Rows, A.Columns);
            for (int i = 0; i < C.Rows; i++)
            {
                for (int j = 0; j < C.Columns; j++)
                {
                    C[i, j] = A[i, j] * B[i, j];
                }
            }
            return C;
        }
        public static Matrix operator *(float A, Matrix B)
        {

            Matrix C = new Matrix(B.Rows, B.Columns);
            for (int i = 0; i < C.Rows; i++)
            {
                for (int j = 0; j < C.Columns; j++)
                {
                    C[i, j] += A * B[i, j];
                }
            }

            return C;
        }
        public static Matrix operator *(Matrix B, float A)
        {
            return A * B;
        }
        public static Matrix Randomise(int Rows, int Columns)
        {
            Random rand = new Random(Guid.NewGuid().GetHashCode());
            Matrix C = new Matrix(Rows, Columns);
            for (int i = 0; i < Rows; i++)
            {
                for (int j = 0; j < Columns; j++)
                {
                    C[i, j] = (float)rand.NextDouble() * 2 - 1;
                }
            }

            return C;
        }
        public static Matrix Randomise(int Rows, int Columns, float Magnitude)
        {
            return Magnitude * Randomise(Rows, Columns);
        }
        public static Matrix operator +(Matrix A, Matrix B)
        {
            if (A.Rows != B.Rows && A.Columns != B.Columns)
            {
                throw new Exception("Dimension error");
            }
            Matrix C = new Matrix(A.Rows, A.Columns);
            for (int i = 0; i < C.Rows; i++)
            {
                for (int j = 0; j < C.Columns; j++)
                {
                    C[i, j] = A[i, j] + B[i, j];
                }
            }
            return C;
        }
        public static Matrix operator -(Matrix A, Matrix B)
        {

            return (A + -1 * B);
        }
        public float this[int i, int j]
        {

            get
            {
                if (i < 0 || i >= Rows || j < 0 || j >= Columns)
                {
                    throw new Exception("Dimension error");
                }
                return Mat[i, j];
            }
            set
            {
                if (i < 0 || i >= Rows || j < 0 || j >= Columns)
                {
                    throw new Exception("Dimension error");
                }
                Mat[i, j] = value;
            }
        }

    }

}
