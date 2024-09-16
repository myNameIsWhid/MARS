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
    class Tank : Soldier
    {
        public Tank(Rectangle[] tankSource, Rectangle[] bulletSource, Texture2D texture, Color team, Vector2 pos, double degree) : base("Tank", tankSource, bulletSource, texture, team, pos, degree, 0.3, 420, 300, 350, 3, 10, 8, 100, 15, 100, new Rectangle((int)pos.X, (int)pos.Y, (int)(102), (int)(160)))
        {

        }

        public override void hit(Soldier other)
        {
            for (int i = 0; i < bullets.Count; i++)
            {
                if (bullets[i].getRect().Intersects(other.rec) && !other.team.Equals(team))
                {
                    double bullet = (bullets[i].heading * (180 / Math.PI) % 360);
                    double otherAngle = other.degree % 360;
                    if (Math.Abs(bullet - otherAngle) < 90)
                        other.panic = true;

                    int temp = other.health;
                    int bulldmg = bullets[i].damage;
                    if (other.soldierClass.Equals("Tank"))
                        bulldmg /= 5;
                    other.health -= bulldmg;
                    if (other.health <= 0)
                    {
                        other.isDead = true;
                        bullets[i].damage = Math.Abs(other.health);
                    }
                    else
                    {
                        bullets[i].damage -= temp;
                    }
                }
            }
        }

    }
}
