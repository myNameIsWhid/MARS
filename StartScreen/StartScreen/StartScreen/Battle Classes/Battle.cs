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
    class Battle
    {
            public static Battlefield[,] battlefields = new Battlefield[3, 3];
            public static SpriteBatch sb;
            public static GraphicsDevice gd;
            public static SpriteFont font1;
            Battlefield battle;
            Rectangle screen;
            KeyboardState oldKB = Keyboard.GetState();
            Random random = new Random();

            List<Soldier> armies;

            public static Texture2D soldierTexture;

            Rectangle[] soldierSources;
            Rectangle soldierbulletSource;

            public static Texture2D tankTexture;

            Rectangle[] tankSources;
            Rectangle[] tankbulletSources;
            List<Soldier> dead;

            //count of scout, rifleman, heavy, sniper, tank in that order
            int[] survivingSoldiers;
            bool victory = false;
            bool battleDone = false;
            bool paused;

            bool debug;

            Color team1;
            Color team2;
            tileType biome;
            int[] attackers;
            int[] defenders;
            public Territory attacker;
            public Territory defender;

            //90+ speed is max before extreme lag
            int speed = 1;
        
        public Battle(Territory attacker, Territory defender, int[] troops, tileType biome)
            {
                this.attacker = attacker;
                this.defender = defender;
                screen = new Rectangle(0, 0, gd.Viewport.Width, gd.Viewport.Height);
                soldierSources = new Rectangle[2];
                soldierSources[0] = new Rectangle(0, 0, 18, 32);
                soldierSources[1] = new Rectangle(18, 0, 18, 32);
                soldierbulletSource = new Rectangle(36, 0, 4, 20);

                tankSources = new Rectangle[2];
                tankSources[0] = new Rectangle(0, 0, 102, 160);
                tankSources[1] = new Rectangle(102, 0, 102, 160);
                tankbulletSources = new Rectangle[2];
                tankbulletSources[0] = new Rectangle(205, 0, 10, 60);
                tankbulletSources[1] = new Rectangle(205, 61, 10, 60);

                dead = new List<Soldier>();

                

                this.biome = biome;

                switch(biome)
                {
                    case tileType.Hill:
                        battle = battlefields[0, random.Next(0, 3)];
                        break;
                    case tileType.Desert:
                        battle = battlefields[1, random.Next(0, 3)];
                        break;
                    case tileType.Mountain:
                        battle = battlefields[2, random.Next(0, 3)];
                        break;
                 }
                
                this.team1 = attacker.getColor();
                this.team2 = defender.getColor();
                this.attackers = troops;
                this.defenders = defender.troops;
                generateBoids();
            }


            void generateBoids()
            {
                victory = false;
                armies = new List<Soldier>();
                Boid.reset();
                Vector2 pos = Vector2.Zero;
                double degree = 0;

                for (int i = 0; i < attackers.Length; i++)
                {
                    for (int j = 0; j < attackers[i]; j++)
                    {
                        pos = new Vector2(random.Next(battle.spawnAttack.X, battle.spawnAttack.X + battle.spawnAttack.Width), random.Next(battle.spawnAttack.Y, battle.spawnAttack.Y + battle.spawnAttack.Height));
                        degree = random.Next(330, 390);

                        switch (i)
                        {
                            case 0:
                                armies.Add(new Scout(soldierSources, soldierbulletSource, soldierTexture, team1, pos, degree));
                                break;
                            case 1:
                                armies.Add(new Rifleman(soldierSources, soldierbulletSource, soldierTexture, team1, pos, degree));
                                break;
                            case 2:
                                armies.Add(new Heavy(soldierSources, soldierbulletSource, soldierTexture, team1, pos, degree));
                                break;
                            case 3:
                                armies.Add(new Sniper(soldierSources, soldierbulletSource, soldierTexture, team1, pos, degree));
                                break;
                            case 4:
                                armies.Add(new Tank(tankSources, tankbulletSources, tankTexture, team1, pos, degree));
                                break;
                        }
                    }
                }
                for (int i = 0; i < defenders.Length; i++)
                {
                    for (int j = 0; j < defenders[i]; j++)
                    {
                        pos = new Vector2(random.Next(battle.spawnDefend.X, battle.spawnDefend.X + battle.spawnDefend.Width), random.Next(battle.spawnDefend.Y, battle.spawnDefend.Y + battle.spawnDefend.Height));
                        degree = random.Next(150, 210);

                        switch (i)
                        {
                            case 0:
                                armies.Add(new Scout(soldierSources, soldierbulletSource, soldierTexture, team2, pos, degree));
                                break;
                            case 1:
                                armies.Add(new Rifleman(soldierSources, soldierbulletSource, soldierTexture, team2, pos, degree));
                                break;
                            case 2:
                                armies.Add(new Heavy(soldierSources, soldierbulletSource, soldierTexture, team2, pos, degree));
                                break;
                            case 3:
                                armies.Add(new Sniper(soldierSources, soldierbulletSource, soldierTexture, team2, pos, degree));
                                break;
                            case 4:
                                armies.Add(new Tank(tankSources, tankbulletSources, tankTexture, team2, pos, degree));
                                break;
                        }
                    }
                }
            }

            public tileType GetBattleBiome()
            {
                return biome;
            }

            public void Update(GameTime gameTime)
            {
                KeyboardState KB = Keyboard.GetState();

                    
                    if (KB.IsKeyDown(Keys.X) && oldKB.IsKeyUp(Keys.X))
                        debug = !debug;
                    if (KB.IsKeyDown(Keys.Escape) && oldKB.IsKeyUp(Keys.Escape))
                        paused = !paused;
                    if (KB.IsKeyDown(Keys.F) && oldKB.IsKeyUp(Keys.F))
                    {
                        victory = true;
                    }

                if (KB.IsKeyDown(Keys.Up) && oldKB.IsKeyUp(Keys.Up))
                {
                    speed++;
                }

                if (KB.IsKeyDown(Keys.Down) && oldKB.IsKeyUp(Keys.Down))
                {
                    if(speed > 1)
                      speed--;
                }

                if (KB.IsKeyDown(Keys.R) && oldKB.IsKeyUp(Keys.R))
                {
                    speed = 1;
                }


                for (int i = 0; i < armies.Count; i++)
                    {
                        if (!paused)
                        {
                        for(int j = 0; j < speed; j++)
                        {
                            armies[i].Update(gameTime, gd, battle.mesh);
                        }
                            
                        if (KB.IsKeyDown(Keys.S))
                        {
                            armies[i].Update(gameTime, gd, battle.mesh);
                            armies[i].Update(gameTime, gd, battle.mesh);
                        }
                    }


                        for (int j = 0; j < armies.Count; j++)
                        {
                            if (!armies[i].team.Equals(armies[j].team))
                                armies[i].hit(armies[j]);
                        }
                        if (armies[i].isDead)
                        {
                            dead.Add(armies[i]);

                            armies.RemoveAt(i);
                            Boid.boidList.RemoveAt(i);
                            i--;
                        }
                    }
                for (int i = 0; i < dead.Count; i++)
                {
                    dead[i].changeTeam(Color.Black);
                }

                if(!victory)
                {
                    victory = checkWin();
                    if(victory)
                    {
                        if(getWinner() == attacker)
                            Sounds.playSound("battleWon");
                        if (getWinner() == defender)
                            Sounds.playSound("battleLost");
                     }
                }
                  
            if (victory)
                {

               
                speed = 1;
                    MouseState mouse = Mouse.GetState();
                    if (mouse.LeftButton == ButtonState.Pressed)
                    {
                        Sounds.stopSound("battleLost");
                        Sounds.stopSound("battleWon");
                        battleDone = true;
                     }
                }
                
                oldKB = KB;
            }
            private bool checkWin()
            {
                for (int i = 0; i < armies.Count; i++)
                  {
                    if (!armies[i].team.Equals(armies[0].team))
                        return false;
                 }
                return true;
            }

            public Territory getWinner()
            {
                int attackSum = 0;
                int defendSum = 0;

                for(int i = 0; i < armies.Count; i++)
                {
                
                    switch (armies[i].soldierClass)
                    {
                        case "Scout":
                            if (armies[i].team == attacker.getColor())
                                attackSum += 20;
                            else
                                defendSum += 20;
                            break;
                        case "Rifleman":
                            if (armies[i].team == attacker.getColor())
                                attackSum += 25;
                            else
                                defendSum += 25;
                            break;
                        case "Heavy":
                            if (armies[i].team == attacker.getColor())
                                attackSum += 50;
                            else
                                defendSum += 50;
                            break;
                        case "Sniper":
                            if (armies[i].team == attacker.getColor())
                                attackSum += 80;
                            else
                                defendSum += 80;
                            break;
                        case "Tank":
                            if (armies[i].team == attacker.getColor())
                                attackSum += 200;
                            else
                                defendSum += 200;
                            break;
                    }
                }

            if (attackSum > defendSum)
            {
                return attacker;
            } else
            {
                return defender;
            }
             
            }
            public int[] leftovers()
            {
                survivingSoldiers = new int[5];
                for (int i = 0; i < armies.Count; i++)
                {
                    switch (armies[i].soldierClass)
                    {
                        case "Scout":
                            survivingSoldiers[0]++;
                            break;
                        case "Rifleman":
                            survivingSoldiers[1]++;
                            break;
                        case "Heavy":
                            survivingSoldiers[2]++;
                            break;
                        case "Sniper":
                            survivingSoldiers[3]++;
                            break;
                        case "Tank":
                            survivingSoldiers[4]++;
                            break;
                    }
                }
                return survivingSoldiers;
            }

            public bool BattleOver()
            {
                return battleDone;
            }

            public void Draw(Texture2D blank, GameTime gameTime)
            {
                battle.Draw(sb, gd);
                for (int i = 0; i < armies.Count; i++)
                    armies[i].Draw(gameTime, sb, blank, debug, font1, battle.type);

                    sb.DrawString(font1, (speed) + "x", new Vector2(0, gd.Viewport.Height -20), Color.White);

            if (victory)
                {
                    sb.Draw(blank, screen, new Color(0, 0, 0, 50));
                    sb.DrawString(font1, "Click Anywhere to Continue...", new Vector2(gd.Viewport.Width / 2 - 100, gd.Viewport.Height / 2), Color.White);
                }

                if(debug)
                {
                    for (int i = 0; i < battle.mesh.Count; i++)
                        sb.Draw(blank, new Rectangle((int)battle.mesh[i].X - 2, (int)battle.mesh[i].Y - 2, 4, 4), Color.White);
                }
            }
        }
    }

