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
    class Boid
    {

        // static Boid[][] boids = new Boid[]
        public static ArrayList boidList = new ArrayList();
        public List<Soldier> viewCone;
        public static int identNum = 0;
        public ArrayList boidInConeList = new ArrayList();

        public static int wallRad = 100;

        public double speed;
        public int ident;
        public double visonCone;
        public double coneRadius;
        public double scale;
        public double scaleFactor;
        public double degree;
        public Vector2 pos;
        public Rectangle rec;
        public Color color = Color.White;
        public Vector2 center;
        Random random = new Random();
        public double moveSpeedAffector = 1.0;
        public bool avoid = false;


        public Boid()
        {
            visonCone = 80;
            coneRadius = 100;
            degree = 0;
            pos = new Vector2(500, 500);
            scale = 20;
            rec = new Rectangle((int)pos.X, (int)pos.Y, (int)scale, (int)scale);
            ident = identNum;
            identNum++;
            boidList.Add(this);
            viewCone = new List<Soldier>();

        }
        public Boid(Vector2 pos, double degree, double visionCone, double coneRad, double size, double speed, Rectangle rec)
        {
            this.speed = speed;
            this.visonCone = visionCone;
            this.coneRadius = coneRad;
            this.degree = degree;
            this.pos = pos;
            this.scale = 20;
            this.scaleFactor = size;
            this.rec = rec;
            ident = identNum;
            identNum++;
            boidList.Add(this);
            viewCone = new List<Soldier>();
        }


        public static void reset()
        {
            boidList.Clear();
        }

        public List<Soldier> BoidsInSight()
        {
            return viewCone;
        }

        public virtual void move(GraphicsDevice gD, bool isTeam, string type, List<Soldier> boidsInCone, int index, bool panic, List<Vector2> points)
        {
            //which boids do I consider
            viewCone = boidsInCone;

            center = pos;

            Vector2 vision = Vector2.Zero;
            avoid = false;
            for (int i = 0; i < points.Count; i++)
            {
                if (IsPointInSector(center, degree, visonCone, points[i], 100))
                {
                    avoid = true;
                    vision = points[i];
                    break;
                }
            }


            //avoid other boid SEPERATION
            if (center.Y - wallRad <= 0)
            {
                steerAwayFromPoint(new Vector2(center.X, 0), panic);
            }
            else if (center.X - wallRad <= 0)
            {
                steerAwayFromPoint(new Vector2(0, center.Y), panic);
            }
            else if (center.Y + wallRad >= gD.Viewport.Height)
            {
                steerAwayFromPoint(new Vector2(center.X, gD.Viewport.Height), panic);
            }
            else if (center.X + wallRad >= gD.Viewport.Width)
            {
                steerAwayFromPoint(new Vector2(gD.Viewport.Width, center.Y), panic);
            }
            else if (avoid)
            {
                steerAwayFromPoint(vision);
            }
            else if (panic)
            {
                steerAwayFromPoint(new Vector2((float)(5 * Math.Cos(degree)), (float)(5 * Math.Sin(degree))), panic);
            }
            else if (!isTeam)
                steerTowardsCurrentTarget(viewCone[index]);
            else
                if (!type.Equals("Sniper") || !type.Equals("Tank"))
                alignWithBoids(viewCone, type);

            pos.X += (float)(Math.Cos(degree * Math.PI / 180) * (speed * moveSpeedAffector));
            pos.Y += (float)(Math.Sin(degree * Math.PI / 180) * (speed * moveSpeedAffector));

            rec.X = (int)pos.X;
            rec.Y = (int)pos.Y;




            if (pos.X > gD.Viewport.Width)
                flip(4);
            if (pos.X < 0)
                flip(2);
            if (pos.Y > gD.Viewport.Height)
                flip(3);
            if (pos.Y < 0)
                flip(1);

        }



        public void flip(int side)
        {
            switch (side)
            {
                case 1:
                    degree = random.Next(225, 315);
                    break;
                case 2:
                    degree = random.Next(315, 405);
                    break;
                case 3:
                    degree = random.Next(45, 135);
                    break;
                case 4:
                    degree = random.Next(135, 225);
                    break;
            }
        }

        public void steerAwayFromPoint(Vector2 target, bool panic)
        {
            int multiplier = 1;
            if (panic)
                multiplier = 5;
            if (IsPointInSector(pos, degree - 90, 180, target, wallRad))
                degree += speed * multiplier;
            else if (IsPointInSector(pos, degree + 90, 180, target, wallRad))
                degree -= speed * multiplier;
        }

        public void steerAwayFromPoint(Vector2 target)
        {
            int coneDegree = 90;
            for (int i = 0; i < 10; i++)
            {
                if (IsPointInSector(pos, degree - coneDegree / 2, coneDegree, target, (i + 1) * 10))
                    degree += speed;
                else if (IsPointInSector(pos, degree + coneDegree / 2, coneDegree, target, (i + 1) * 10))
                    degree -= speed;
            }
        }
        public void alignWithBoids(List<Soldier> currentBoids, string type)
        {
            double radians = degree * Math.PI / 180;
            for (int i = 0; i < currentBoids.Count; i++)
            {
                if (currentBoids[i].soldierClass.Equals(type))
                {
                    //bool temp = ((currentBoids[i].degree * Math.PI / 180) - radians) > Math.PI;
                    //if (temp)
                    //    radians -= ((currentBoids[i].degree * Math.PI / 180) - radians) / 100 / speed;
                    //else
                    //    radians += ((currentBoids[i].degree * Math.PI / 180) - radians) / 100 / speed;

                    radians += ((currentBoids[i].degree * Math.PI / 180) - radians) / 100 / speed;
                }
            }

            degree = radians * 180 / Math.PI;
        }


        public void steerTowardsCenter(List<Soldier> currentBoids)
        {
            if (currentBoids.Count > 0)
            {
                Vector2 center = new Vector2(0, 0);
                for (int i = 0; i < currentBoids.Count; i++)
                {
                    center.X += ((Boid)currentBoids[i]).pos.X;
                    center.Y += ((Boid)currentBoids[i]).pos.Y;
                }
                center.X = center.X / currentBoids.Count;
                center.Y = center.Y / currentBoids.Count;
                degree -= (Math.Atan2(pos.Y - center.Y, pos.X - center.X) * 180.0 / Math.PI) / 60;
            }
        }
        public static int sideOfLineThisPointIs(Vector2 l1, Vector2 l2, Vector2 p1)
        {
            int leftSign = (-1) * (l2.Y - l1.Y) < 0 ? -1 : 1;
            return (((p1.X - l1.X) * (l2.Y - l1.Y) - (p1.Y - l1.Y) * (l2.X - l1.X) < 0 ? -1 : 1) == leftSign ? -1 : 1);
        }
        public double distance(Boid one, Boid two)
        {
            return Math.Sqrt(Math.Pow((two.pos.X - one.pos.X), 2) + Math.Pow((two.pos.Y - one.pos.Y), 2));
        }
        public static double distance(Vector2 one, Vector2 two)
        {
            return Math.Sqrt(Math.Pow((two.X - one.X), 2) + Math.Pow((two.Y - one.Y), 2));
        }

        public bool IsPointInSector(Vector2 center, double degree1, double coneDegrees, Vector2 point, double radius)
        {

            //360
            //checks two addition sectors not covered by the initial 180 sector. 
            // No obtuse angles sectors checked, just muitple acute ones
            if (coneDegrees > 180)
            {
                //coneDegress = 360 -> 135
                //coneDegress = 180 -> 0

                if (IsPointInSector(center, degree1 + (coneDegrees * 135 / 360), (coneDegrees - 180) / 2, point, radius) || IsPointInSector(center, degree1 - (coneDegrees * 135 / 360), (coneDegrees - 180) / 2, point, radius))
                {
                    return true;
                }
                coneDegrees = 180;
            }

            //must be in close enought
            if (distance(point, center) >= radius)
                return false;

            double startAngle = Math.PI + Math.PI * (degree1 - coneDegrees / 2) / 180;
            coneDegrees = Math.PI * coneDegrees / 180;
            double endAngle = startAngle + coneDegrees;

            // Calculate sector start and end vectors
            Vector2 sectorStart = new Vector2((float)(radius * Math.Cos(startAngle)), (float)(radius * Math.Sin(startAngle)));
            Vector2 sectorEnd = new Vector2((float)(radius * Math.Cos(endAngle)), (float)(radius * Math.Sin(endAngle)));

            // Calculate relative point vector
            Vector2 relativePoint = new Vector2(point.X - center.X, point.Y - center.Y);

            // Check if point is clockwise from start vector and counter-clockwise from end vector
            return IsClockwise(sectorStart, relativePoint) && !IsClockwise(sectorEnd, relativePoint);
        }

        public bool IsPointInSector(Vector2 center, double degree1, double coneDegrees, Vector2 point)
        {

            //360
            //checks two addition sectors not covered by the initial 180 sector. 
            // No obtuse angles sectors checked, just muitple acute ones
            if (coneDegrees > 180)
            {
                //coneDegress = 360 -> 135
                //coneDegress = 180 -> 0

                if (IsPointInSector(center, degree1 + (coneDegrees * 135 / 360), (coneDegrees - 180) / 2, point) || IsPointInSector(center, degree1 - (coneDegrees * 135 / 360), (coneDegrees - 180) / 2, point))
                {
                    return true;
                }
                coneDegrees = 180;
            }

            //must be in close enought

            double startAngle = Math.PI + Math.PI * (degree1 - coneDegrees / 2) / 180;
            coneDegrees = Math.PI * coneDegrees / 180;
            double endAngle = startAngle + coneDegrees;

            // Calculate sector start and end vectors
            Vector2 sectorStart = new Vector2((float)(999999 * Math.Cos(startAngle)), (float)(999999 * Math.Sin(startAngle)));
            Vector2 sectorEnd = new Vector2((float)(999999 * Math.Cos(endAngle)), (float)(999999 * Math.Sin(endAngle)));

            // Calculate relative point vector
            Vector2 relativePoint = new Vector2(point.X - center.X, point.Y - center.Y);

            // Check if point is clockwise from start vector and counter-clockwise from end vector
            return IsClockwise(sectorStart, relativePoint) && !IsClockwise(sectorEnd, relativePoint);
        }


        public void steerTowardsCenterOfMass(ArrayList currentBoids)
        {
            if (currentBoids.Count > 0)
            {
                Vector2 center = new Vector2(0, 0);
                for (int i = 0; i < currentBoids.Count; i++)
                {
                    center.X += ((Boid)currentBoids[i]).pos.X;
                    center.Y += ((Boid)currentBoids[i]).pos.Y;
                }
                center.X = center.X / currentBoids.Count;
                center.Y = center.Y / currentBoids.Count;
                //Console.WriteLine($"{Math.Atan2(pos.Y - center.Y, pos.X - center.X) * 180.0 / Math.PI}");
                if (IsPointInSector(pos, degree - 90, 180, center))
                    degree -= 1;
                else
                    degree += 1;
            }
        }


        public void steerTowardsCurrentTarget(Boid target)
        {
            // Calculate the direction vector from the boid to the target
            if (IsPointInSector(pos, degree - 90, 180, target.pos))
                degree -= speed;
            else
                degree += speed;
        }


        public static bool IsClockwise(Vector2 v1, Vector2 v2)
        {
            return -v1.X * v2.Y + v1.Y * v2.X > 0;
        }



        public static void changeAllVisonRadius(double howMuch)
        {
            for (int i = 0; i < boidList.Count; i++)
            {
                Boid currentBoid = (Boid)boidList[i];
                currentBoid.coneRadius += howMuch;
            }
        }
        public static void changeAllVisonCone(double howMuch)
        {
            for (int i = 0; i < boidList.Count; i++)
            {
                Boid currentBoid = (Boid)boidList[i];
                currentBoid.visonCone += howMuch;
            }
        }

        public static void changeAllScale(double howMuch)
        {
            for (int i = 0; i < boidList.Count; i++)
            {
                Boid currentBoid = (Boid)boidList[i];
                currentBoid.scale += howMuch;
                currentBoid.rec.Width = (int)currentBoid.scale;
                currentBoid.rec.Height = (int)currentBoid.scale;
            }
        }
    }
}
