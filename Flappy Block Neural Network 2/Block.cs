using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
namespace Flappy_Block_Neural_Network_2
{
    class Block
    {
        public Point Location { get; private set; }
        public static int startX { get; private set; } = 30;
        private int vel;
        const int acc = 2;
        public NeuralNetwork Brain { get; private set; }
        public int Score { get; private set; }
        public float Fitness { get; private set; }
        public bool Dead { get; private set; }
        public static int dim { get; private set; } = 20;
        public Block(Point Location)
        {
            Dead = false;
            Score = 0;
            Brain = new NeuralNetwork(new int[] { 5, 5, 1 });
            this.Location = Location;
            
        }

        public Block(Point Location, NeuralNetwork Brain)
        {
            Dead = false;
            Score = 0;
            this.Location = Location;
            this.Brain = Brain;
            //this.Brain = Brain.Copy();
        }
        public void Die()
        {
            Dead = true;
        }

        public void CalcFitness(int ScoreSum)
        {
            Fitness = Score / (float)ScoreSum;
        }

        public void Think(Wall Wall, Size Size)
        {
            if (Brain.FeedForward(new float[] { Wall.Location, Wall.TopHeight, Wall.BottomHeight, Location.Y, vel })[0] > 0.5)
            {
                Jump();
            }
        }
        public void Floor(int floor, Graphics g)
        {
            Location = new Point(Location.X, floor - dim / 2);
            g.FillRectangle(new SolidBrush(Color.FromArgb(128, Color.Yellow)), Location.X, Location.Y, dim, dim);
        }
        public void Jump()
        {
            vel = -15;
        }
        public void Update(Graphics g)
        {
            Score++;
            vel += acc;
            Location = new Point(Location.X, Location.Y + vel);

            g.FillRectangle(new SolidBrush(Color.FromArgb(128, Color.Yellow)), Location.X, Location.Y, dim, dim);
        }

    }
}
