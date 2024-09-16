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

    public enum Shape
    {
        //Neighbor direction : shape
        Square,
        NorthEquilateral,
        EastEquilateral,
        SouthEquilateral,
        WestEquilateral,
        NorthWestRight,
        NorthEastRight,
        SouthEastRight,
        SouthWestRight,
    }
    public enum tileType
    {
        Mountain,
        Hill,
        Desert,
        Unknown
    }
    public class Tile
    {

        public int x;
        public int y;

        public int width = 1;
        public int height = 1;

        public Rectangle rec;
        public tileType type;
        public Shape shape = Shape.Square;

        public Color defaultColor = Color.Black;


        public Territory territory;



        public Tile(int _x, int _y, Territory _territory, tileType Ttype)
        {

            territory = _territory;
            x = _x;
            y = _y;
            type = Ttype;


        }

        public Tile(int _x, int _y, int _w, int _h, Territory _territory, tileType Ttype)
        {

            territory = _territory;
            x = _x;
            y = _y;
            width = _w;
            height = _h;
            type = Ttype;
        }

        public Color getColor()
        {

            if (territory == null)
            {
                return defaultColor;

            }
            return territory.getColor();



        }
    }

}
