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
    public class Nation
    {
        //NATION also acts as a player class

        public Color color;
        public List<Territory> territories;

        public int percentOwned = 0;

        public List<List<Tile>> borders;

        public int actionPoints = 0;
        public int money = 0;

        public Nation(Color color)
        {
            this.color = color;
            territories = new List<Territory>();
            borders = new List<List<Tile>>();
        }

        public void findBorderTiles()
        {
            borders.Clear();
            for (int j = 0; j < territories.Count; j++)
            {
                for (int i = 0; i < territories[j].borderingTerritories.Count; i++)
                {
                    Territory bTer = territories[j].borderingTerritories[i];

                    if (bTer.nation != this)
                        borders.Add(territories[j].borders[bTer]);
                    
                }
            }
        }

        public void gainIncome()
        {
            for (int i = 0; i < territories.Count; i++)
                money += territories[i].getIncome();
        }

        public int  getIncome()
        {
            int amount = 0;
            for (int i = 0; i < territories.Count; i++)
                amount += territories[i].getIncome();
            return amount;
        }
        public int getTroopPower()
        {
            int amount = 0;
            for (int i = 0; i < territories.Count; i++)
                amount += territories[i].getTroopPower();
            return amount;
        }


        public void drawBorders(SpriteBatch batch, Texture2D tex, Rectangle zoom, Rectangle display)
        {
            double scale = 1024.0 / (double)zoom.Width;
            for (int i = 0; i < borders.Count; i++)
            {
                List<Tile> cBorder = borders[i];

                for (int j = 0; j < cBorder.Count; j++)
                {

                    Color borderColor = color;

                    batch.Draw(tex, new Rectangle((int)((cBorder[j].y - zoom.X) * scale) + display.X, (int)((cBorder[j].x + 3 - zoom.Y) * scale) + display.Y, (int)(cBorder[j].width * scale + 1), (int)(cBorder[j].height * scale + 1)), null, darken(borderColor, 0.5), 0, new Vector2(0, 0), SpriteEffects.None, 1);
                    batch.Draw(tex, new Rectangle((int)((cBorder[j].y - zoom.X) * scale) + display.X, (int)((cBorder[j].x + 2 - zoom.Y) * scale) + display.Y, (int)(cBorder[j].width * scale + 1), (int)(cBorder[j].height * scale + 1)), null, darken(borderColor, 0.55), 0, new Vector2(0, 0), SpriteEffects.None, 1);
                    batch.Draw(tex, new Rectangle((int)((cBorder[j].y - zoom.X) * scale) + display.X, (int)((cBorder[j].x + 1 - zoom.Y) * scale) + display.Y, (int)(cBorder[j].width * scale + 1), (int)(cBorder[j].height * scale + 1)), null, darken(borderColor, 0.6), 0, new Vector2(0, 0), SpriteEffects.None, 1);
                    batch.Draw(tex, new Rectangle((int)((cBorder[j].y - zoom.X) * scale) + display.X, (int)((cBorder[j].x + 1 - zoom.Y) * scale) + display.Y, (int)(cBorder[j].width * scale + 1), (int)(cBorder[j].height * scale + 1)), null, darken(borderColor, 0.65), 0, new Vector2(0, 0), SpriteEffects.None, 1);
                    batch.Draw(tex, new Rectangle((int)((cBorder[j].y - zoom.X) * scale) + display.X, (int)((cBorder[j].x - zoom.Y) * scale) + display.Y, (int)(cBorder[j].width * scale + 1), (int)(cBorder[j].height * scale + 1)), null, darken(borderColor, 0.7), 0, new Vector2(0, 0), SpriteEffects.None, 1);
                    batch.Draw(tex, new Rectangle((int)((cBorder[j].y - zoom.X) * scale) + display.X, (int)((cBorder[j].x - 1 - zoom.Y) * scale) + display.Y, (int)(cBorder[j].width * scale + 1), (int)(cBorder[j].height * scale + 1)), null, darken(borderColor, 0.75), 0, new Vector2(0, 0), SpriteEffects.None, 1);
                    batch.Draw(tex, new Rectangle((int)((cBorder[j].y - zoom.X) * scale) + display.X, (int)((cBorder[j].x - 2 - zoom.Y) * scale) + display.Y, (int)(cBorder[j].width * scale + 1), (int)(cBorder[j].height * scale + 1)), null, darken(borderColor, 0.8), 0, new Vector2(0, 0), SpriteEffects.None, 1);
                    batch.Draw(tex, new Rectangle((int)((cBorder[j].y - zoom.X) * scale) + display.X, (int)((cBorder[j].x - 3 - zoom.Y) * scale) + display.Y, (int)(cBorder[j].width * scale + 1), (int)(cBorder[j].height * scale + 1)), null, darken(borderColor, 0.9), 0, new Vector2(0, 0), SpriteEffects.None, 1);
                    batch.Draw(tex, new Rectangle((int)((cBorder[j].y - zoom.X) * scale) + display.X, (int)((cBorder[j].x - 4 - zoom.Y) * scale) + display.Y, (int)(cBorder[j].width * scale + 1), (int)(cBorder[j].height * scale + 1)), null, borderColor, 0, new Vector2(0, 0), SpriteEffects.None, 1);
                }

            }
        }

        public static Color darken(Color c, double amount)
        {
            return new Color((int)(c.R * amount), (int)(c.G * amount), (int)(c.B * amount));

        }



    }
}
