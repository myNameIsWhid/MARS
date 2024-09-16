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
    class Slider
    {
        public int max;
        public double at = 0;

        public Rectangle rec;

        public string label;

        static public SpriteFont font;
        public Color color;
        public Color fontColor = Color.White;

        public bool hover = false;

        public Slider(Rectangle rec, int max, Color color, string label, Color fontColor)
        {
            this.fontColor = fontColor;
            this.rec = rec;
            this.max = max;
            at = max;
            this.color = color;
            this.label = label;
        }
        public Slider(Rectangle rec, int max, Color color, string label)
        {
            this.rec = rec;
            this.max = max;
            at = max;
            this.color = color;
            this.label = label;
        }
        public void update(int money, int amount, int price)
        {
            MouseState mouse = Mouse.GetState();

            if (new Rectangle(mouse.X, mouse.Y, 1, 1).Intersects(rec))
                hover = true;
            else
                hover = false;

            if(mouse.LeftButton == ButtonState.Pressed && new Rectangle(mouse.X,mouse.Y,1,1).Intersects(rec))
            {
                int rX = mouse.X - rec.X;
                if(rX > 0 && rX < rec.Width)
                {
                    if(!(money < amount + (price * (Math.Round((rX / (double)rec.Width) * max) - at)) && Math.Round((rX / (double)rec.Width) * max)  >= at))
                    {
                        at = (rX / (double)rec.Width) * max;
                        at = Math.Round(at);
                    } else
                    {
                        Sounds.playSound("error");
                    }
                }
            }
        }

        public void update()
        {
            MouseState mouse = Mouse.GetState();

            if (new Rectangle(mouse.X, mouse.Y, 1, 1).Intersects(rec))
                hover = true;
            else
                hover = false;

            if (mouse.LeftButton == ButtonState.Pressed && new Rectangle(mouse.X, mouse.Y, 1, 1).Intersects(rec))
            {
                int rX = mouse.X - rec.X;
                if (rX > 0 && rX < rec.Width)
                {
                        at = (rX / (double)rec.Width) * max;
                        at = Math.Round(at);
                }
            }
        }

        public void update(bool sound)
        {
            MouseState mouse = Mouse.GetState();

            if (new Rectangle(mouse.X, mouse.Y, 1, 1).Intersects(rec))
                hover = true;
            else
                hover = false;

            if (mouse.LeftButton == ButtonState.Pressed && new Rectangle(mouse.X, mouse.Y, 1, 1).Intersects(rec))
            {
                int rX = mouse.X - rec.X;
                if (rX > 0 && rX < rec.Width)
                {
                    at = (rX / (double)rec.Width) * max;
                    at = Math.Round(at);
                    Sounds.editSound();
                }
            }
        }

        

        public void draw(SpriteBatch batch, Texture2D white)
        {
            if(!hover)
                batch.Draw(white, new Rectangle(rec.X, rec.Y, (int)(rec.Width), rec.Height), Color.Black);
            else
                batch.Draw(white, new Rectangle(rec.X, rec.Y, (int)(rec.Width), rec.Height), Color.Black * 0.6f);
            batch.Draw(white, new Rectangle(rec.X,rec.Y,(int)((at/max) * rec.Width), rec.Height), color);
            batch.DrawString(font,label, new Vector2(rec.X - 105,rec.Y + 3), fontColor);
            batch.DrawString(font, "" + at, new Vector2(rec.X + rec.Width + 9, rec.Y + 3), fontColor);
        }

       

    }
}
