using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime;
using System.Windows.Forms;
using System.IO;

namespace Flappy_Block_Neural_Network_2
{
    public partial class Form1 : Form
    {

        public Form1()
        {
            InitializeComponent();
            
            pictureBox1.Location = new Point(0, 0);
            FormBorderStyle = FormBorderStyle.None;
            Size = new Size(1000, 500);
            pictureBox1.Size = Size;
            Text = "Flappy Block Neural Network";

            pop = new Population(new Point(Block.startX, Size.Height / 4));
            Walls = new List<Wall>();
            for (int i = 0; i < Size.Width / 100; i++) 
            {
                Walls.Add(new Wall(Size, Size.Width + 30 + i * (Wall.DistanceFromNextWall + Wall.Width)));
            }
            label1.Location = new Point(0, Size.Height - 25);
            label2.Location = new Point(Size.Width / 2 - label2.Width/2, Size.Height - 25);
            label3.Location = new Point(Size.Width / 4 - label3.Width / 2, Size.Height - 25);
            label4.Location = new Point(3*Size.Width / 4 - label4.Width / 2, Size.Height - 25);
            label1.BackColor = Color.FromArgb(255, Color.Brown);
            label2.BackColor = Color.FromArgb(255, Color.Brown);
            label3.BackColor = Color.FromArgb(255, Color.Brown);
            label4.BackColor = Color.FromArgb(255, Color.Brown);

            speed = false;
            slow = false;

            timer1.Interval = 33;
            timer1.Start();

            
        }
        Population pop;
        List<Wall> Walls;
        bool speed;
        bool slow;
        private void timer1_Tick(object sender, EventArgs e)
        {
            const int floor = 30;

            if(speed && timer1.Interval > 1)
            {
                timer1.Interval--;
            }
            else if (slow && timer1.Interval < 33)
            {
                timer1.Interval++;
            }
            Bitmap b = new Bitmap(Size.Width, Size.Height);
            Graphics g = Graphics.FromImage(b);
            g.Clear(Color.SkyBlue);
            for(int i = 0; i < Walls.Count; i++)
            {
                Walls[i].Update(g);
            }


            pop.Update(g, Size, Walls.ToArray(), floor);
            



            g.FillRectangle(Brushes.Brown, 0, Size.Height - floor, Size.Width, floor);
            pictureBox1.Image = b;

            label1.Text = "Score : " + pop.Score.ToString();
            label2.Text = "Generation : " + pop.Gen.ToString();
            label3.Text = "Top Score : " + pop.BestScore.ToString();
            label4.Text = "Best Generation : " + pop.BestGen.ToString();
            for (int i = 0; i < Walls.Count; i++)
            {
                if(Walls[i].Location + Wall.Width < 0)
                {
                    Walls.RemoveAt(i);
                    i--;
                    Walls.Add(new Wall(Size, Walls[Walls.Count - 1].Location + Wall.DistanceFromNextWall + Wall.Width));
                }
            }


        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.KeyCode == Keys.Space)
            {
                timer1.Enabled = !timer1.Enabled;
            }
            else if (e.KeyCode == Keys.Enter)
            {
                Application.Exit();
            }
            else if (e.KeyCode == Keys.Back)
            {
                pop = new Population(new Point(Block.startX, Walls[0].Location + Wall.Gap / 2));
            }
            else if(e.KeyCode == Keys.Right)
            {
                speed = true;
            }
            else if (e.KeyCode == Keys.Left)
            {
                slow = true;
            }
            else if(e.KeyCode == Keys.O)
            {
                OpenFileDialog dlg = new OpenFileDialog();
                dlg.Filter = "Neural Networks (*.nn) | *.nn";
                dlg.Title = "Load Network";
                if(dlg.ShowDialog() == DialogResult.OK)
                {
                    pop = new Population(new Point(Block.startX, Walls[0].Location + Wall.Gap / 2), dlg.FileName);
                }
                
            }
        }

        private void Form1_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Right)
            {
                speed = false;
            }
            else if (e.KeyCode == Keys.Left)
            {
                slow = false;
            }
        }
    }
}
