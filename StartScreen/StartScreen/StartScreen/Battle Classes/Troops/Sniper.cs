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
    class Sniper : Soldier
    {
        public Sniper(Rectangle[] source, Rectangle bulletSource, Texture2D texture, Color team, Vector2 pos, double degree) : base("Sniper", source, bulletSource, texture, team, pos, degree, 0.7, 120, 150, 450, 1, 3, 10, 40, 10, 15, new Rectangle((int)pos.X, (int)pos.Y, (int)(14), (int)(40)))
        {

        }
        //Overriden method so that Snipers prioritize Tanks
        public override void Update(GameTime gt, GraphicsDevice gd, List<Vector2> points)
        {
            if (panic)
            {
                panicTimer++;
                steerAwayFromPoint(new Vector2((float)(5 * Math.Cos(degree)), (float)(5 * Math.Sin(degree))), panic);
                if (panicTimer > 120)
                {
                    panic = false;
                    panicTimer = 0;
                }
            }
            if (isOffScreen(gd))
            {
                rec.X = gd.Viewport.Width / 2;
                rec.Y = gd.Viewport.Height / 2;
            }

            if (reloading)
                timer++;
            if (timer >= 5 || !reloading)
                animIndex = 0;
            if (timer > fireRate)
            {
                reloading = false;
                timer = 0;
            }

            if (health > startHealth)
                health = startHealth;

            List<Soldier> vision = new List<Soldier>();
            List<Soldier> fire = new List<Soldier>();

            Vector2 centerPos = new Vector2((float)(pos.X + scale / 2), (float)(pos.Y + scale / 2));
            for (int i = 0; i < boidList.Count; i++)
            {
                Soldier currentSoldier = (Soldier)boidList[i];

                Vector2 centerCurrentBoid = new Vector2((float)(currentSoldier.pos.X + currentSoldier.scale / 2), (float)(currentSoldier.pos.Y + currentSoldier.scale / 2));
                if (!this.Equals(currentSoldier) && IsPointInSector(centerPos, degree, visonCone, centerCurrentBoid, coneRadius))
                    vision.Add(currentSoldier);
                if (!this.Equals(currentSoldier) && IsPointInSector(centerPos, degree, fireCone, centerCurrentBoid, coneRadius))
                    fire.Add(currentSoldier);
            }
            if (vision.Count > 0)
            {
                int index = 0;
                for (int i = 0; i < vision.Count; i++)
                {
                    if (vision[i].startHealth > vision[index].startHealth && !vision[i].team.Equals(this.team))
                    {
                        index = i;
                    }
                }

                move(gd, vision[index].team.Equals(this.team), soldierClass, vision, index, panic, points);
                if (fire.Count > 0)
                {
                    for (int i = 0; i < fire.Count; i++)
                    {
                        if (!fire[i].team.Equals(this.team) && fire[i].Equals(vision[index]))
                        {
                            moveSpeedAffector = 0.5;
                            if (!reloading)
                                Fire();
                        }
                    }
                }
                else
                {
                    moveSpeedAffector = 1.0;
                }

            }
            else
            {
                moveSpeedAffector = 1.0;
                move(gd, true, soldierClass, vision, 0, panic, points);
            }



            center = centerPos;
            for (int i = 0; i < bullets.Count; i++)
            {
                bullets[i].Update(gt);
                if (bullets[i].isOffScreen(gd) || bullets[i].damage <= 0)
                    bullets.RemoveAt(i);
            }
        }
    }
}
