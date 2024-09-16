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
    public enum ActionType
    {
        Undecided,
        Attack,
        ForeignAid,
        Transfer
    }
    public enum BiomeType
    {
        Hills,
        Moutain,
        Desert,
    }

    class Action
    {
        public Arrow arrow;
        public ActionType type;
        public Territory origin;
        public Territory destination;
        public Nation initiator;

        public int animationTime = 0;
        public int duration = 0;

        public Boolean finshied = false;
        public int[] troops = new int[5];

        public Boolean percentBased = false;
        public static Map map;
        public Action(Arrow arrow, Territory origin, Territory destination)
        {
            this.arrow = arrow;
            this.origin = origin;
            this.destination = destination;
        }

        public Action(Arrow arrow)
        {
            this.arrow = arrow;
        }
        public Action(Arrow arrow, ActionType type, Territory origin, Territory destination)
        {
            this.type = type;
            this.arrow = arrow;

            switch (type)
            {
                case ActionType.Attack:
                    arrow.color = Color.Red;
                    break;
                case ActionType.ForeignAid:
                    arrow.color = Color.Yellow;
                    break;
                case ActionType.Transfer:
                    arrow.color = Color.Blue;
                    break;
            }
            this.origin = origin;
            this.destination = destination;
        }

        public Action(ActionType type, Territory origin, Territory destination, Nation initiator)
        {
            this.initiator = initiator;
            this.type = type;

            arrow = new Arrow(origin.center, destination.center, Color.White, 6, 1.0);
            switch (type)
            {
                case ActionType.Attack:
                    arrow.color = Color.Red;
                    break;
                case ActionType.ForeignAid:
                    arrow.color = Color.Yellow;
                    break;
                case ActionType.Transfer:
                    arrow.color = Color.Blue;
                    break;
            }
            this.origin = origin;
            this.destination = destination;
        }

        public void updateTroopAmount(int[] nTroops)
        {
            troops = nTroops;
            if (troops[4] == 3319)
                percentBased = true;


            arrow.width = getTroopPower() / 30;
            if (percentBased)
                arrow.width = troops[3]/2;

           
            
            while(arrow.tooBig())
            {
                arrow.width--;
            }
            arrow.width -= 5;

            if (arrow.width < 5)
                arrow.width = 5;
        }

        public void startAnimation(int time)
        {
            animationTime = time;
            duration = time;
        }

        public void complete()
        {
            if (type == ActionType.Attack && origin.nation == initiator)
            {
                tileType biome = origin.borderTypes[destination];
                if (!percentBased)
                {
                    for (int i = 0; i < 5; i++)
                    {
                        origin.troops[i] -= troops[i];
                    }
                }
                else
                {
                    int percent = troops[3];
                    int[] oldTroops = new int[5];
                    for (int i = 0; i < 5; i++)
                        oldTroops[i] += origin.troops[i];
                    

                    for (int i = 0; i < 5; i++)
                    {
                        troops[i] = 0;
                        troops[i] += (int)Math.Floor(origin.troops[i] * ((double)percent/100.0));
                        origin.troops[i] -= (int)Math.Floor(origin.troops[i] * ((double)percent / 100.0));
                    }

                    //cant leave a terrotoity without a troop
                    if(origin.troops.Sum() == 0)
                    {
                        if (troops.Sum() > 1)
                        {
                            for (int i = 0; i < 5; i++)
                            {
                                if(troops[i] > 0)
                                {
                                    troops[i]--;
                                    origin.troops[i]++;
                                    break;
                                }
                            }
                        } 
                        else
                        {
                            //like it never happned
                            origin.troops = oldTroops;
                            troops = new int[5];
                            //fight with 0 troops == Lose instantly
                        }
                    }
                }
               
                map.currentBattle = new Battle(origin, destination, troops, biome);
              
                
            } else
            {
                animationTime = 0;
            }
            finshied = true;
            animationTime--;

            if (type == ActionType.ForeignAid)
            {
                if (!percentBased)
                {
                    for (int i = 0; i < 5; i++)
                    {
                        origin.troops[i] -= troops[i];
                        destination.troops[i] += troops[i];
                    }
                } else  {
                    int percent = troops[3];

                    for (int i = 0; i < 5; i++)
                    {
                        troops[i] = 0;
                        destination.troops[i] += (int)Math.Floor(origin.troops[i] * ((double)percent / 100.0));
                        origin.troops[i] -= (int)Math.Floor(origin.troops[i] * ((double)percent / 100.0));
                    }
                }
                finshied = true;
            }

            if (type == ActionType.Transfer && origin.nation == initiator)
            {
                if (!destination.occupied)
                {
                    destination.nation = initiator;
                    Sounds.playSound("good");
                    map.log.Add(new MapLog(destination.getColor(), destination.index));
                    destination.nation.territories.Add(destination);
                    destination.transparent = false;
                    destination.occupied = true;

                    if (!percentBased)
                    {
                        for (int i = 0; i < 5; i++)
                        {
                            origin.troops[i] -= troops[i];
                            destination.troops[i] += troops[i];
                        }
                    } else  {
                        int percent = troops[3];

                        for (int i = 0; i < 5; i++)
                        {
                            troops[i] = 0;
                            destination.troops[i] += (int)Math.Floor(origin.troops[i] * ((double)percent / 100.0));
                            origin.troops[i] -= (int)Math.Floor(origin.troops[i] * ((double)percent / 100.0));
                        }
                    }
                    map.updateTerritoryDrawTiles(destination);
                    initiator.findBorderTiles();
                } else
                {
                    destination.transparent = false;
                    destination.occupied = true;
                    if (!percentBased)
                    {
                        for (int i = 0; i < 5; i++)
                        {
                            origin.troops[i] -= troops[i];
                            destination.troops[i] += troops[i];
                        }
                    }
                    else
                    {
                        int percent = troops[3];

                        for (int i = 0; i < 5; i++)
                        {
                            troops[i] = 0;
                            destination.troops[i] += (int)Math.Floor(origin.troops[i] * ((double)percent / 100.0));
                            origin.troops[i] -= (int)Math.Floor(origin.troops[i] * ((double)percent / 100.0));
                        }
                    }
                }
                finshied = true;
            }
        }

        public void draw(SpriteBatch batch, Texture2D white, Rectangle zoom, Rectangle display)
        {
            //RETURNS TRUE WHEN FINSIHED


            arrow.draw(batch, white, zoom, display);
            if (animationTime > 0)
            {

                double scale = 1024.0 / (double)zoom.Width;
                double distance = Arrow.GetDistance(arrow.destination, arrow.origin) * (troops.Sum() / 19.5);
                double distance2 = Arrow.GetDistance(arrow.destination, arrow.origin);
                double capSize = arrow.width * arrow.ratio;
                double angle = Math.Atan2(arrow.destination.Y - arrow.origin.Y, arrow.destination.X - arrow.origin.X);
                double animationRatio = (animationTime) / (double)duration;


                //  batch.Draw(tex, new Rectangle((int)((tiles[i].y - zoom.X) * scale), (int)((tiles[i].x - zoom.Y) * scale), (int)(tiles[i].width * scale + 1), (int)(tiles[i].height * scale + 1)), tiles[i].getColor() * 0.2f);
                for (int i = 0; i < troops.Sum(); i++)
                {


                    double x = (arrow.origin.X + (Math.Cos(angle) * (distance - capSize / 2)) * ((i) / (double)troops.Sum())) - (Math.Cos(angle) * distance * 2 * animationRatio) + (Math.Cos(angle) * distance);
                    double y = (arrow.origin.Y + (Math.Sin(angle) * (distance - capSize / 2)) * ((i) / (double)troops.Sum())) - (Math.Sin(angle) * distance * 2 * animationRatio) + (Math.Sin(angle) * distance);
                    double width = arrow.width / 2;

                    //long
                    // If troop is on the arrow, show it
                    if (new Rectangle((int)(((arrow.origin.X + (Math.Cos(0) * (distance - capSize / 2)) * ((i) / (double)troops.Sum())) - (Math.Cos(0) * distance * 2 * animationRatio) + (Math.Cos(0) * distance) - zoom.X) * scale), (int)(((arrow.origin.Y + (Math.Sin(0) * (distance - capSize / 2)) * ((i) / (double)troops.Sum())) - (Math.Sin(0) * distance * 2 * animationRatio) + (Math.Sin(0) * distance) - zoom.Y) * scale), (int)(width * scale), (int)(width * scale)).Intersects(new Rectangle((int)(((arrow.origin.X + (Math.Cos(0) * (capSize / 2))) - zoom.X) * scale), (int)(((arrow.origin.Y + (Math.Sin(0) * (capSize / 2))) - zoom.Y) * scale), (int)((distance2 - capSize * 2) * scale), (int)(width * scale))))
                        batch.Draw(white, new Rectangle((int)((x - zoom.X) * scale) + display.X, (int)((y - zoom.Y) * scale) + display.Y, (int)(width * scale), (int)(width * scale)), null, Color.Black, (float)angle, new Vector2(white.Width / 2, white.Height / 2), SpriteEffects.None, 1);

                }


            }

            animationTime--;


        }

        public int getTroopPower()
        {
            return (25 * troops[(int)TroopType.Rifleman]) + (20 * troops[(int)TroopType.Scout]) + (200 * troops[(int)TroopType.Tank]) + (80 * troops[(int)TroopType.Sniper]) + (50 * troops[(int)TroopType.Heavy]);
        }

    }
}
