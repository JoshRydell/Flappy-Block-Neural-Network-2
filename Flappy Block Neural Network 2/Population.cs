using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
namespace Flappy_Block_Neural_Network_2
{
    class Population
    {
        Block[] Pop;
        public int Score { get; private set; }
        public int BestScore { get; private set; }
        public int Gen{ get; private set; }
        public int BestGen { get; private set; }
        bool SaveGen;
        private const int PopSize = 300;
        bool pass;
        bool freshPop;
        public Population(Point StartP)
        {
            SaveGen = false;
            pass = true;
            Score = 0;
            BestScore = 0;
            BestGen = 1;
            Gen = 1;
            Pop = new Block[PopSize];
            freshPop = false;
            for(int i = 0; i < PopSize; i++)
            {
                Pop[i] = new Block(StartP);
            }
        }
        public Population(Point StartP, string fileName)
        {
            Pop = new Block[1];
            Pop[0] = new Block(StartP, new NeuralNetwork(fileName,false));
            freshPop = true;
        }

        public void Update(Graphics g, Size Size, Wall[] Walls, int floor)
        {
            bool Respwan = false;
            Wall NextWall;
            bool newPop = true;
            if (30 > Walls[0].Location + Wall.Width)
            {
                NextWall = Walls[1];
                pass = false;
                Respwan = true;
            }
            else
            {
                if(!pass)
                {
                    Score++;
                    pass = true;
                    if (Score > BestScore)
                    {
                        BestScore = Score;
                        BestGen = Gen;
                        SaveGen = true;
                    }
                }
                NextWall = Walls[0];
            }
            
            foreach (Block Block in Pop)
            {
                if(!Block.Dead)
                {
                    newPop = false;
                    Block.Think(NextWall, Size);
                    Block.Update(g);

                    if (NextWall.Collision(Block) || Block.Location.Y + Block.dim > Size.Height - floor)
                    {
                        if (Block.Location.Y + Block.dim > Size.Height - floor)
                        {
                            Block.Floor(Size.Height - floor, g);
                        }

                        Block.Die();
                    }
                }
            }

            if(newPop && Respwan && !freshPop)
            {

                Score = 0;
                Gen++;
                CalculatePopFitness();
                List<Block> MatingPool = new List<Block>();
                //Maybe Sort Birds By Fitness?

                if (SaveGen)
                {
                    SaveGen = false;
                    Block BestBlock = Pop[0];
                    foreach(Block block in Pop)
                    {
                        if(block.Fitness > BestBlock.Fitness)
                        {
                            BestBlock = block;
                        }
                    }
                    BestBlock.Brain.SaveToFile("Highest Scorer");
                }

                int index = 0;
                for(int i = 0; i < PopSize; i++)
                {
                    float numBirds = Pop[i].Fitness *100;
                    for(int j = 0; j < numBirds; j++)
                    {
                        MatingPool.Add(Pop[i]);
                        index++;
                    }
                }

                for(int i = 0; i < PopSize; i++)
                {
                    Pop[i] = MakeChild(MatingPool.ToArray(), new Point(Block.startX, NextWall.TopHeight + Wall.Gap / 2));
                }
            }
    
            else if(newPop && Respwan && freshPop)
            {
                SaveGen = false;
                pass = true;
                Score = 0;
                BestScore = 0;
                BestGen = 1;
                Gen = 1;
                Pop = new Block[PopSize];
                freshPop = false;
                for (int i = 0; i < PopSize; i++)
                {
                    Pop[i] = new Block(new Point(Block.startX, NextWall.TopHeight + Wall.Gap / 2));
                }
            }
                
        }



        private Block PickBlock(Block[] MatingPool)
        {
            Random rand = new Random(Guid.NewGuid().GetHashCode());
            float a = (float)rand.NextDouble();
            int index = 0;
            while (a > 0.000001)
            {
                a -= MatingPool[index].Fitness;
                index++;
            }
            index--;

            return MatingPool[index];
        }
        private Block MakeChild(Block[] MatingPool, Point StartPoint)
        {
            Block ParentA = PickBlock(MatingPool);
            Block ParentB = PickBlock(MatingPool);
            NeuralNetwork ChildBrain = ParentA.Brain.CrossOver(ParentB.Brain);
            
            Block child = new Block(StartPoint, ChildBrain);
            child.Brain.Mutate(0.1f);
            return child;
        }
        public void CalculatePopFitness()
        {
            int sumScore = 0;
            foreach (Block Block in Pop)
            {
                sumScore += Block.Score;
            }
            foreach (Block Block in Pop)
            {
                Block.CalcFitness(sumScore);
            }
        }
    }
}
