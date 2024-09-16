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
    public class Mask
    {

        // FOR GENERATION
        public Texture2D tex;
        Color[] colors;

        public Mask(Texture2D tex)
        {
            this.tex = tex;
            colors = new Color[tex.Width * tex.Height];
            tex.GetData<Color>(colors);
        }

        public Color getColorAt(int x, int y)
        {
            return colors[x + (y * tex.Width)];
        }

        public Boolean isColorAtRec(Color color, Rectangle rec)
        {

            for (int i = rec.X; i < rec.Width + rec.X; i++)
            {
                for (int j = rec.Y; j < rec.Height + rec.Y; j++)
                {
                    if (getColorAt(i, j) == color)
                        return true;
                }
            }
            return false;

        }

        public void drawMask(SpriteBatch batch, Texture2D white)
        {
            for (int i = 0; i < tex.Width; i++)
            {
                for (int j = 0; j < tex.Width; j++)
                {
                    batch.Draw(tex, new Rectangle(i, j, 1, 1), getColorAt(i, j));
                }
            }
        }
    }
}
