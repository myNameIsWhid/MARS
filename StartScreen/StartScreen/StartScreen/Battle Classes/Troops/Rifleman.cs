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
    class Rifleman : Soldier
    {
        public Rifleman(Rectangle[] source, Rectangle bulletSource, Texture2D texture, Color team, Vector2 pos, double degree) : base("Rifleman", source, bulletSource, texture, team, pos, degree, 1, 30, 150, 200, 1, 5, 10, 5, 10, 20, new Rectangle((int)pos.X, (int)pos.Y, (int)(14), (int)(40)))
        {

        }
    }
}
