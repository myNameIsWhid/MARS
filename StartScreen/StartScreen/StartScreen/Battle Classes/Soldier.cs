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
    class Soldier : Boid
    {
        private Texture2D texture;
        Color teamColor;
        Color shadow = new Color(0, 10, 30, 120);
        public Color team;
        public List<Bullet> bullets;
        Rectangle[] sourceRecs;
        public int fireRate;
        public Bullet template;
        public bool isDead;
        public int bulletSize;
        public int bulletSpeed;
        public int bulletDmg;
        public int fireCone;
        public int health;
        public int startHealth;
        public int timer;
        public bool reloading;
        public int animIndex;
        public string soldierClass;
        public int panicTimer;
        public bool panic;


        public Soldier(string soldierClass, Rectangle[] sourceRecs, Rectangle bulletSource, Texture2D texture, Color team, Vector2 pos, double degree, double speed, int fireRate, int visCon, int conRad, double size, int bulletSize, int bulletSpeed, int bulletDmg, int fireCone, int health, Rectangle rec) : base(pos, degree, visCon, conRad, size, speed, rec)
        {
            panic = false;
            panicTimer = 0;
            this.soldierClass = soldierClass;
            this.sourceRecs = sourceRecs;
            animIndex = 1;
            reloading = false;
            timer = 0;
            this.health = health;
            this.startHealth = health;
            this.health = startHealth;
            this.bulletDmg = bulletDmg;
            this.fireCone = fireCone;
            this.bulletSize = bulletSize;
            this.bulletSpeed = bulletSpeed;
            isDead = false;
            this.texture = texture;
            bullets = new List<Bullet>();
            this.team = team;
            teamColor = team;
            this.fireRate = fireRate;
           
            template = new Bullet(bulletSource, texture, bulletSize, bulletSpeed, bulletDmg, this);
            center = new Vector2(rec.X + rec.Width / 2, rec.Y + rec.Height / 2);
        }

        public Soldier(string soldierClass, Rectangle[] sourceRecs, Rectangle[] bulletSource, Texture2D texture, Color team, Vector2 pos, double degree, double speed, int fireRate, int visCon, int conRad, double size, int bulletSize, int bulletSpeed, int bulletDmg, int fireCone, int health, Rectangle rec) : base(pos, degree, visCon, conRad, size, speed, rec)
        {
            panic = false;
            panicTimer = 0;
            this.soldierClass = soldierClass;
            this.sourceRecs = sourceRecs;
            animIndex = 1;
            reloading = false;
            timer = 0;
            this.health = health;
            this.startHealth = health;
            this.health = startHealth;
            this.bulletDmg = bulletDmg;
            this.fireCone = fireCone;
            this.bulletSize = bulletSize;
            this.bulletSpeed = bulletSpeed;
            isDead = false;
            this.texture = texture;
            bullets = new List<Bullet>();
            this.team = team;
            this.fireRate = fireRate;
            teamColor = team;
            
            template = new Bullet(bulletSource, texture, bulletSize, bulletSpeed, bulletDmg, this);
            center = new Vector2(rec.X + rec.Width / 2, rec.Y + rec.Height / 2);
        }

        public string getClass()
        {
            return soldierClass;
        }

        public virtual void hit(Soldier other)
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
                    other.health -= bullets[i].damage;
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
        public bool isOffScreen(GraphicsDevice gd)
        {
            if (pos.X < 0  || pos.X > gd.Viewport.Width || pos.Y < 0 || pos.Y > gd.Viewport.Height)
                return true;
            return false;
        }
        public virtual void Update(GameTime gt, GraphicsDevice gd, List<Vector2> points)
        {
            if (panic)
            {
                panicTimer++;

                if (panicTimer > 30)
                {
                    panic = false;
                    panicTimer = 0;
                }
            }
            if (isOffScreen(gd))
            {
                pos.X = gd.Viewport.Width / 2;
                pos.Y = gd.Viewport.Height / 2;
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
            if (health <= 0)
                isDead = true;

            List<Soldier> vision = new List<Soldier>();
            List<Soldier> fire = new List<Soldier>();

            for (int i = 0; i < boidList.Count; i++)
            {
                Soldier currentSoldier = (Soldier)boidList[i];

                Vector2 centerCurrentBoid = currentSoldier.pos;
                if (!this.Equals(currentSoldier) && IsPointInSector(center, degree, visonCone, centerCurrentBoid, coneRadius))
                {
                    vision.Add(currentSoldier);
                    //color = Color.Yellow;
                }
                if (!this.Equals(currentSoldier) && IsPointInSector(center, degree, fireCone, centerCurrentBoid, coneRadius))
                {
                    fire.Add(currentSoldier);
                    //color = Color.Red;
                }
            }
            if (vision.Count > 0)
            {
                int index = 0;
                for (int i = 0; i < vision.Count; i++)
                {
                    if (!(vision[i].team.Equals(this.team)))
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
                            if (this.speed > fire[i].speed)
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




            for (int i = 0; i < bullets.Count; i++)
            {
                bullets[i].Update(gt);
                if (bullets[i].isOffScreen(gd) || bullets[i].damage <= 0)
                    bullets.RemoveAt(i);
            }
        }

        public virtual void Fire()
        {
            animIndex = 1;
            reloading = true;
            bullets.Add(new Bullet(template, center, degree));
        }

        public void Draw(GameTime gt, SpriteBatch sb, Texture2D blank, bool debug, SpriteFont font1, string biomeType)
        {
            for (int i = 0; i < bullets.Count; i++)
            {
                bullets[i].Draw(sb);
            }

            //shadows
            double scalarX = 1;
            double scalarY = 1;
            SpriteEffects flipped = SpriteEffects.None;
            switch (biomeType)
            {
                case "Hill":
                    scalarX = 5.7;
                    scalarY = -3.5;
                    break;
                case "Mars":
                    scalarY = 4.9;
                    scalarX = 3.2;
                    break;
                case "Mountain":
                    scalarY = -6.4;
                    scalarX = -2.6;
                    break;
                default:
                    scalarX = 1;
                    scalarY = 1;
                    break;
            }


            sb.Draw(texture, new Rectangle((int)(rec.X - scalarX), (int)(rec.Y - scalarY), rec.Width, rec.Height), sourceRecs[animIndex], shadow, (float)((degree + 90) * (Math.PI / 180)), new Vector2(sourceRecs[animIndex].Width / 2, sourceRecs[animIndex].Height / 2), flipped, 1);
            sb.Draw(texture, rec, sourceRecs[animIndex], color, (float)((degree + 90) * (Math.PI / 180)), new Vector2(sourceRecs[animIndex].Width / 2, sourceRecs[animIndex].Height / 2), SpriteEffects.None, 1);

            if (debug)
            {
                for (int j = 0; j < visonCone / 2; j++)
                {
                    //round cone
                    sb.Draw(texture, new Rectangle((int)(((pos.X) + (Math.Cos((((degree) - (j)) * (Math.PI / 180))) * coneRadius * scale / 20))), (int)(((pos.Y) + (Math.Sin((((degree) - (j)) * (Math.PI / 180))) * coneRadius * scale / 20))), (int)Math.Ceiling(scale / 10), (int)Math.Ceiling(scale / 10)), null, teamColor, (float)((0) * (180 / Math.PI)), new Vector2(sourceRecs[animIndex].Width / 2, sourceRecs[animIndex].Height / 2), SpriteEffects.None, 1);
                    sb.Draw(texture, new Rectangle((int)(((pos.X) + (Math.Cos((((degree) + (j)) * (Math.PI / 180))) * coneRadius * scale / 20))), (int)(((pos.Y) + (Math.Sin((((degree) + (j)) * (Math.PI / 180))) * coneRadius * scale / 20))), (int)Math.Ceiling(scale / 10), (int)Math.Ceiling(scale / 10)), null, teamColor, (float)((0) * (180 / Math.PI)), new Vector2(sourceRecs[animIndex].Width / 2, sourceRecs[animIndex].Height / 2), SpriteEffects.None, 1);
                }
                for (double k = 1; k < coneRadius / 2; k++)
                {
                    //lines extruding
                    sb.Draw(texture, new Rectangle((int)(((pos.X) + (Math.Cos((((degree) - (visonCone / 2)) * (Math.PI / 180))) * (coneRadius * scale / 20 * (k / (coneRadius / 2)))))), (int)(((pos.Y) + (Math.Sin((((degree) - (visonCone / 2)) * (Math.PI / 180))) * (coneRadius * scale / 20 * (k / (coneRadius / 2)))))), (int)Math.Ceiling(scale / 10), (int)Math.Ceiling(scale / 10)), null, teamColor, (float)((0) * (180 / Math.PI)), new Vector2(sourceRecs[animIndex].Width / 2, sourceRecs[animIndex].Height / 2), SpriteEffects.None, 1);
                    sb.Draw(texture, new Rectangle((int)(((pos.X) + (Math.Cos((((degree) + (visonCone / 2)) * (Math.PI / 180))) * (coneRadius * scale / 20 * (k / (coneRadius / 2)))))), (int)(((pos.Y) + (Math.Sin((((degree) + (visonCone / 2)) * (Math.PI / 180))) * (coneRadius * scale / 20 * (k / (coneRadius / 2)))))), (int)Math.Ceiling(scale / 10), (int)Math.Ceiling(scale / 10)), null, teamColor, (float)((0) * (180 / Math.PI)), new Vector2(sourceRecs[animIndex].Width / 2, sourceRecs[animIndex].Height / 2), SpriteEffects.None, 1);
                }
                sb.Draw(texture, new Rectangle((int)((pos.X) + (Math.Cos((((degree)) * (Math.PI / 180))) * coneRadius * scale / 20)), (int)((pos.Y) + (Math.Sin((((degree)) * (Math.PI / 180))) * coneRadius * scale / 20)), (int)Math.Ceiling(scale / 4), (int)Math.Ceiling(scale / 4)), null, Color.Blue, (float)((0) * (180 / Math.PI)), new Vector2(sourceRecs[animIndex].Width / 2, sourceRecs[animIndex].Height / 2), SpriteEffects.None, 1);
            }

            sb.Draw(blank, new Rectangle((int)(pos.X - 15), (int)pos.Y - rec.Height / 2, 30, 5), Color.Black);
            sb.Draw(blank, new Rectangle((int)(pos.X - 15), (int)pos.Y - rec.Height / 2, (int)(30 * ((double)health / (double)startHealth)), 5), teamColor);

            sb.DrawString(font1, soldierClass, new Vector2((pos.X - 15), pos.Y + rec.Height / 2), teamColor);
        }

        public void changeTeam(Color newTeam)
        {
            team = newTeam;
           
        }
        public void boidusDeletus(Soldier delete)
        {
            for (int i = 0; i < boidList.Count; i++)
                if (delete == boidList[i])
                    boidList[i] = null;
        }
    }
}
