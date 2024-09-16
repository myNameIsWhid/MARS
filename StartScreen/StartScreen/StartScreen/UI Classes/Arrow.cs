using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System;
using System.Collections.Generic;
using System.Linq;


namespace StartScreen
{
    class Arrow
    {
        public Vector2 origin;
        public Vector2 destination;
        public Color color;
        public int width;
        public double ratio = 1;

        public static Texture2D triangle;

        public Arrow(Vector2 origin, Vector2 destination, Color color, int width, double ratio)
        {
            this.origin = origin;
            this.destination = destination;
            this.color = color;
            this.width = width;
            this.ratio = ratio;
        }

        public static void setUp(Texture2D triangle_)
        {
            triangle = triangle_;
        }

        public void updateDestination(Vector2 newDestination)
        {
            destination = newDestination;
        }

        public void updateOrigin(Vector2 newOrigin)
        {
            origin = newOrigin;
        }

        public void draw(SpriteBatch batch, Texture2D white, Rectangle zoom, Rectangle display)
        {
            double scale = 1024.0 / (double)zoom.Width;
            double distance = GetDistance(destination, origin);
            double capSize = width * ratio;
            double angle = Math.Atan2(destination.Y - origin.Y, destination.X - origin.X);

            if (distance > capSize * 2)
            {
                //I love trig!
                batch.Draw(white, new Rectangle((int)(((origin.X + (Math.Cos(angle) * (distance / 2 - capSize / 2))) - zoom.X) * scale) + display.X, (int)(((origin.Y + (Math.Sin(angle) * (distance / 2 - capSize / 2))) - zoom.Y) * scale) + display.Y, (int)((distance - capSize * 2) * scale), (int)(width * scale)), null, color * 1f, (float)angle, new Vector2(white.Width / 2, white.Height / 2), SpriteEffects.None, 1);

                double num = capSize + 1;

                batch.Draw(triangle, new Rectangle((int)(((destination.X - (Math.Cos(angle) * (num))) - zoom.X) * scale) + display.X, (int)(((destination.Y - (Math.Sin(angle) * (num))) - zoom.Y) * scale) + display.Y, (int)(capSize * scale) , (int)(capSize * 2 * scale)), null, color * 1f, (float)(angle), new Vector2(triangle.Width / 2, triangle.Height / 2), SpriteEffects.None, 1);
            }
        }

        public bool tooBig()
        {
            double distance = GetDistance(destination, origin);
            double capSize = width * ratio;
            if (distance < capSize * 2)
                return true;
            return false;
        }

        public static double GetDistance(Vector2 one, Vector2 two)
        {
            return Math.Sqrt(Math.Pow((two.X - one.X), 2) + Math.Pow((two.Y - one.Y), 2));
        }

    }
}
