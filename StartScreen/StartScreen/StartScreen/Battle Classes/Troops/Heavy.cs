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
    class Heavy : Soldier
    {

        Random random = new Random();
        public Heavy(Rectangle[] source, Rectangle bulletSource, Texture2D texture, Color team, Vector2 pos, double degree) : base("Heavy", source, bulletSource, texture, team, pos, degree, 0.8, 3, 180, 200, 2.5, 4, 15, 3, 30, 50, new Rectangle((int)pos.X, (int)pos.Y, (int)(14), (int)(40)))
        {

        }

        public override void Fire()
        {
            animIndex = 1;
            reloading = true;
            bullets.Add(new Bullet(template, center, degree + random.Next(-20, 20)));
        }
    }
}
