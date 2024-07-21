using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
namespace Flappy_Block_Neural_Network_2
{
    class Wall
    {

        public static int Gap { get; private set; } = 100;
        public static int Width { get; private set; } = 40;
        public static int DistanceFromNextWall { get; private set; } = 180;
        public int TopHeight { get; private set; }
        public int BottomHeight { get; private set; }
        private static int vel = -8;
        public int Location { get; private set; }
        public Wall(Size Size, int startX)
        {
            Random rand = new Random(Guid.NewGuid().GetHashCode());
            int buffer = 40;
            Location = startX;
            TopHeight = rand.Next(buffer, Size.Height - (Gap + 2*buffer));
            BottomHeight = Size.Height - (TopHeight + Gap);
        }

        public void Update(Graphics g)
        {
            Location += vel;
            g.FillRectangle(Brushes.DarkGreen, Location, 0, Width, TopHeight);
            g.FillRectangle(Brushes.DarkGreen, Location, TopHeight + Gap, Width,  BottomHeight);
        }

        public bool Collision(Block Block)
        {
            return OneCollision(Block.Location) || OneCollision(new Point(Block.Location.X, Block.Location.Y + Block.dim)) || OneCollision(new Point(Block.Location.X + Block.dim, Block.Location.Y)) || OneCollision(new Point(Block.Location.X + Block.dim, Block.Location.Y + Block.dim));
        }
        private bool OneCollision(Point point)
        {
            return  (point.X > Location && point.X < Location + Width && !new Rectangle(Location, TopHeight, Width, Gap).Contains(point));
        }
    }
}
