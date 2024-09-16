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
    class Button
    {
        Texture2D Up, Down;
        public Rectangle rect;
        SpriteFont sf;
        public string words;
        public Boolean selected = false;
        public Button(Texture2D tU, Texture2D tD, Rectangle r, string w, SpriteFont s, Boolean b)
        {
            
            Up = tU;
            Down = tD;
            rect = r;
            sf = s;
            words = w;
            if (b)
                selected = true;
        }
        public void select() { selected = true; }
        public void deselect() { selected = false; }
        public void Draw(SpriteBatch sb)
        {
            if (selected)
                sb.Draw(Down, rect, Color.LightBlue);
            else
                sb.Draw(Up, rect, Color.White);
            sb.DrawString(sf, words, new Vector2(rect.X+10, rect.Y+20), Color.Black);
        }
    }
}
