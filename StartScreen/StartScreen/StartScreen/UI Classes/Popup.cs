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

        //If stage goes from attackorMove to attack, type = attcak
        //If stage goes from attackorMove to Move, type = foreginAid

        public enum PopupStage
        {
            None, AttackorMove, Attack, Move, UnoccupiedNation
        }
        class Popup
        {


            public PopupStage popup = PopupStage.None;
            Button Attack, MoveTroops;
            Button Troop1Up, Troop2Up, Troop3Up, Troop4Up, Troop5Up, Troop1Down, Troop2Down, Troop3Down, Troop4Down, Troop5Down;
            Button Done;
            public ActionType type;
            public int[] troopCount;
            Rectangle AttackorMoveBOX;
            Rectangle NumberOfTroopsBOX;
            MouseState oldM;
            int x;
            public static SpriteFont font;
            public static Texture2D blank;
            public static Rectangle screen;
            int[] maxTroops;
            public static Texture2D down;
            public static Texture2D up;
            public Boolean done = false;
            public Boolean percent = false;
        public Slider percentSlider;

        public Slider[] troopSliders;
            public Popup(PopupStage popup, ActionType type, int[] max, Color color)
            {
                this.popup = popup;
                this.type = type;
                maxTroops = max;
                troopCount = new int[5] { 0, 0, 0, 0, 0 };
                oldM = Mouse.GetState();
                x = 40;
                AttackorMoveBOX = new Rectangle(screen.Width / 2 - 350 + screen.X, screen.Height / 2 - 150 + screen.Y, 700, 300);
                NumberOfTroopsBOX = new Rectangle(screen.Width / 2 - 350 + screen.X, screen.Height / 2 - 150 + screen.Y, 600, 325);
                Attack = new Button(up, down, new Rectangle(AttackorMoveBOX.X + 50, AttackorMoveBOX.Y + 100, 250, 125), "Attack", font, false);
                MoveTroops = new Button(up, down, new Rectangle(Attack.rect.X + 320, Attack.rect.Y, Attack.rect.Width, Attack.rect.Height), "Move Troops", font, false);
                Troop1Up = new Button(up, down, new Rectangle(NumberOfTroopsBOX.X + 30, NumberOfTroopsBOX.Y + 80, x, x), "", font, false);
                Troop1Down = new Button(up, down, new Rectangle(Troop1Up.rect.X, Troop1Up.rect.Y + 100, x, x), "", font, false);
                Troop2Up = new Button(up, down, new Rectangle(Troop1Up.rect.X + 70, Troop1Up.rect.Y, x, x), "", font, false);
                Troop2Down = new Button(up, down, new Rectangle(Troop2Up.rect.X, Troop2Up.rect.Y + 100, x, x), "", font, false);
                Troop3Up = new Button(up, down, new Rectangle(Troop2Up.rect.X + 70, Troop1Up.rect.Y, x, x), "", font, false);
                Troop3Down = new Button(up, down, new Rectangle(Troop3Up.rect.X, Troop3Up.rect.Y + 100, x, x), "", font, false);
                Troop4Up = new Button(up, down, new Rectangle(Troop3Up.rect.X + 70, Troop1Up.rect.Y, x, x), "", font, false);
                Troop4Down = new Button(up, down, new Rectangle(Troop4Up.rect.X, Troop4Up.rect.Y + 100, x, x), "", font, false);
                Troop5Up = new Button(up, down, new Rectangle(Troop4Up.rect.X + 70, Troop1Up.rect.Y, x, x), "", font, false);
                Troop5Down = new Button(up, down, new Rectangle(Troop5Up.rect.X, Troop5Up.rect.Y + 100, x, x), "", font, false);
                Done = new Button(up, down, new Rectangle(NumberOfTroopsBOX.X + (NumberOfTroopsBOX.Width / 2) - 50, Troop3Down.rect.Y + x + 30, 100, 50), "Done", font, false);
                // Slider
                troopSliders = new Slider[5];


                int j = 0;
                for (int i = 0; i < 5; i++)
                {
                    if (maxTroops[i] > 0)
                    {
                        j++;

                        switch (i)
                        {
                            case 0:
                                troopSliders[i] = new Slider(new Rectangle(NumberOfTroopsBOX.X + 140, NumberOfTroopsBOX.Y + 50 + (30 * j), 400, 20), maxTroops[i], color, "Scout", Color.Black);
                                break;
                            case 1:
                                troopSliders[i] = new Slider(new Rectangle(NumberOfTroopsBOX.X + 140, NumberOfTroopsBOX.Y + 50 + (30 * j), 400, 20), maxTroops[i], color, "Rifleman", Color.Black);
                                break;
                            case 2:
                                troopSliders[i] = new Slider(new Rectangle(NumberOfTroopsBOX.X + 140, NumberOfTroopsBOX.Y + 50 + (30 * j), 400, 20), maxTroops[i], color, "Heavy", Color.Black);
                                break;
                            case 3:
                                troopSliders[i] = new Slider(new Rectangle(NumberOfTroopsBOX.X + 140, NumberOfTroopsBOX.Y + 50 + (30 * j), 400, 20), maxTroops[i], color, "Sniper", Color.Black);
                                break;
                            case 4:
                                troopSliders[i] = new Slider(new Rectangle(NumberOfTroopsBOX.X + 140, NumberOfTroopsBOX.Y + 50 + (30 * j), 400, 20), maxTroops[i], color, "Tank", Color.Black);
                                break;
                        }
                    }
                    else
                    {
                    troopSliders[i] = new Slider(new Rectangle(0, 0, 0,0), 0, color, "Heavy", Color.Black);
                     } 
                }

                percentSlider = new Slider(new Rectangle(NumberOfTroopsBOX.X + 140, NumberOfTroopsBOX.Y + 30 + (30 * 2), 400, 50), 100, color, "Percent", Color.Black);


        }
            public void update(KeyboardState kb, MouseState mouse)
            {

            
            Rectangle temp = new Rectangle(mouse.X, mouse.Y, 1, 1);
            if (mouse.RightButton == ButtonState.Pressed && oldM.RightButton != ButtonState.Pressed)
                {
                    popup = PopupStage.None;
                    Sounds.playSound("dismiss");
            }

                if (mouse.LeftButton == ButtonState.Pressed && oldM.LeftButton != ButtonState.Pressed)
                {
                   
                    if (popup == PopupStage.AttackorMove)
                    {
                        if (temp.Intersects(Attack.rect))
                        {
                            
                        if (Attack.selected)
                        {
                            type = ActionType.Attack;
                            popup = PopupStage.Attack;
                            Sounds.playSound("press");
                        }
                                
                            else
                            {
                                Attack.select();
                                MoveTroops.deselect();
                            }
                        }
                        if (temp.Intersects(MoveTroops.rect))
                        {


                            if (MoveTroops.selected)
                            {
                                if (popup == PopupStage.AttackorMove)
                                    type = ActionType.ForeignAid;
                                popup = PopupStage.Move;
                            Sounds.playSound("press");
                        }

                            else
                            {
                                MoveTroops.select();
                                Attack.deselect();
                            }
                        }
                    }

                   
                }

            if ((popup == PopupStage.Attack || popup == PopupStage.Move) && !percent)
            {
                //NO DOUBLE CLICK NEEDED TO PRESS BUTTON
                //add/delete troops
                for (int i = 0; i < 5; i++)
                {
                    troopSliders[i].update();
                }

                if (temp.Intersects(Done.rect) && ((mouse.LeftButton == ButtonState.Pressed && oldM.LeftButton != ButtonState.Pressed) || kb.IsKeyDown(Keys.Enter)))
                {
                    int[] troops = new int[5];
                    for (int i = 0; i < 5; i++)
                    {
                        troops[i] += (int)troopSliders[i].at;
                    }
                    troopCount = troops;
                    if (maxTroops.Sum() - troopCount.Sum() > 0)
                    {
                        done = true;
                        Sounds.playSound("click");
                    } else
                    {
                        Sounds.playSound("error");
                    }
                }
            }
            if ((popup == PopupStage.Attack || popup == PopupStage.Move) && percent)
            {
                percentSlider.update();

                if (temp.Intersects(Done.rect) && ((mouse.LeftButton == ButtonState.Pressed && oldM.LeftButton != ButtonState.Pressed) || kb.IsKeyDown(Keys.Enter)))
                {
                    troopCount[4] = 3319;
                    troopCount[3] = (int)percentSlider.at;
                    done = true;
                    Sounds.playSound("click");
                }
            }
            oldM = mouse;
            }
            public void draw(SpriteBatch spriteBatch)
            {
            if (popup == PopupStage.AttackorMove)
                {
                spriteBatch.Draw(blank, AttackorMoveBOX, Color.White * 0.8f);
                    Attack.Draw(spriteBatch);
                    MoveTroops.Draw(spriteBatch);
                    spriteBatch.DrawString(font, "Would You Like to Attack or Move troops", new Vector2(AttackorMoveBOX.X + 10, AttackorMoveBOX.Y + 10), Color.Black);
                }
                if (popup == PopupStage.Move)
                {

                    spriteBatch.Draw(blank, NumberOfTroopsBOX, Color.White * 0.8f);

                    Done.Draw(spriteBatch);
                    if(!percent)
                     spriteBatch.DrawString(font, "How many troops would you Like to move", new Vector2(NumberOfTroopsBOX.X + 10, NumberOfTroopsBOX.Y + 10), Color.Black);
                    else
                    spriteBatch.DrawString(font, "What percentage of remaining troops would you Like to move", new Vector2(NumberOfTroopsBOX.X + 10, NumberOfTroopsBOX.Y + 10), Color.Black);

                if (!percent)
                     {
                        for (int i = 0; i < 5; i++)
                        {
                            troopSliders[i].draw(spriteBatch, blank);
                        }
                    } else {
                        percentSlider.draw(spriteBatch, blank);
                    }
                    
                 }
                if (popup == PopupStage.Attack)
                {
                    spriteBatch.Draw(blank, NumberOfTroopsBOX, Color.White * 0.8f);
                    if (!percent)
                    {
                        for (int i = 0; i < 5; i++)
                        {
                            troopSliders[i].draw(spriteBatch, blank);
                        }
                    }
                    else
                    {
                        percentSlider.draw(spriteBatch, blank);
                    }
                Done.Draw(spriteBatch);
                if (!percent)
                    spriteBatch.DrawString(font, "How many troops would you like to attack with", new Vector2(NumberOfTroopsBOX.X + 10, NumberOfTroopsBOX.Y + 10), Color.Black);
                else
                    spriteBatch.DrawString(font, "What percentage of remaining troops would you like to attack with", new Vector2(NumberOfTroopsBOX.X + 10, NumberOfTroopsBOX.Y + 10), Color.Black);

            }
        }
        }

    }

