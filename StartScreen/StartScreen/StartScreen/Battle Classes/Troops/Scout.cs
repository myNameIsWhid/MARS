using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;


namespace StartScreen
{ 
    class Scout : Soldier
    {
        public Scout(Rectangle[] source, Rectangle bulletSource, Texture2D texture, Color team, Vector2 pos, double degree) : base("Scout", source, bulletSource, texture, team, pos, degree, 3, 8, 210, 250, 0.7, 3, 25, 1, 25, 10, new Rectangle((int)pos.X, (int)pos.Y, (int)(14 * 0.7), (int)(40 * 0.7)))
        {

        }
    }
}
