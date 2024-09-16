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


    public enum TroopType
    {
        Scout,
        Rifleman,
        Heavy,
        Sniper,
        Tank

        
    }
    public class Territory
    {
        public static int inflation = 5;
        public string name = "";
        public int index;

        public Tile seedTile;

        public List<Tile> tiles;

        public Dictionary<Territory, List<Tile>> borders;
        public Dictionary<Territory, tileType> borderTypes;
        public List<Territory> borderingTerritories;

        public Color color;
        public Boolean transparent = false;

        public Nation nation;
        public Boolean occupied = false;

        public Rectangle bounds = new Rectangle(0, 0, 0, 0);
        public Vector2 center = new Vector2(0);

        public int[] troops = new int[5];

        public Boolean barracks = false;

        public Boolean hover = false;
        public Boolean selected = false;
        public Territory(int _x, int _y, Nation _nation)
        {
            nation = _nation;
            seedTile = new Tile(_x, _y, this, tileType.Unknown);
            color = _nation.color;
            tiles = new List<Tile>();
            tiles.Add(seedTile);
            borders = new Dictionary<Territory, List<Tile>>();
            borderTypes = new Dictionary<Territory, tileType>();
        }

        public Territory(int _x, int _y, Color _color, string name)
        {
            seedTile = new Tile(_x, _y, this, tileType.Unknown);
            color = _color;
            tiles = new List<Tile>();
            tiles.Add(seedTile);
            borderingTerritories = new List<Territory>();
            borders = new Dictionary<Territory, List<Tile>>();
            borderTypes = new Dictionary<Territory, tileType>();
            this.name = name;
        }

        public Territory()
        {
            tiles = new List<Tile>();
        }

        public Color getColor()
        {
            if (nation != null)
            {
                return nation.color;
            }
            return color;
        }

        public void changeNation(Nation _nation)
        {
            nation = _nation;
            color = _nation.color;
        }

        public void findBorders(Tile[,] map, int thickness, Boolean oceanBorder, Mask[] masks)
        {
            //find border tiles
            for (int i = 0; i < tiles.Count; i++)
                findBorderTile(tiles[i], tiles[i].y, tiles[i].x, map, thickness, oceanBorder, masks);

            //find border type
            for (int i = 0; i < borderingTerritories.Count; i++)
            {
                Territory bTer = borderingTerritories[i];
                int[] typeCounts = new int[4];
                List<Tile> cBorder = borders[bTer];
                typeCounts[3] = 1;
                for (int j = 0; j < cBorder.Count; j++)
                {
                    if (cBorder[j].type == tileType.Mountain)
                        typeCounts[0]++;
                    if (cBorder[j].type == tileType.Desert)
                        typeCounts[1]++;
                    if (cBorder[j].type == tileType.Hill)
                        typeCounts[2]++;
                }
                typeCounts[1] = (int)(typeCounts[1] * 4.95);
                typeCounts[0] = (int)(typeCounts[0] * 2.75);


                if (typeCounts[0] == typeCounts.Max<int>() && !borderTypes.ContainsKey(bTer))
                    borderTypes.Add(bTer, tileType.Mountain);
                if (typeCounts[1] == typeCounts.Max<int>() && !borderTypes.ContainsKey(bTer))
                    borderTypes.Add(bTer, tileType.Desert);
                if (typeCounts[2] == typeCounts.Max<int>() && !borderTypes.ContainsKey(bTer))
                    borderTypes.Add(bTer, tileType.Hill);

                if (typeCounts[3] == typeCounts.Max<int>() && !borderTypes.ContainsKey(bTer))
                    borderTypes.Add(bTer, tileType.Unknown);

                for (int j = 0; j < cBorder.Count; j++)
                {
                    if (borderTypes[bTer] == tileType.Mountain)
                        cBorder[j].defaultColor = Color.Blue;
                    if (borderTypes[bTer] == tileType.Desert)
                        cBorder[j].defaultColor = Color.Red;
                    if (borderTypes[bTer] == tileType.Hill)
                        cBorder[j].defaultColor = Color.Yellow;
                    if (borderTypes[bTer] == tileType.Unknown)
                        cBorder[j].defaultColor = Color.Pink;
                }
            }

        }

        public void findBorderTile(Tile tile, int x, int y, Tile[,] map, int thickness, Boolean oceanBorder, Mask[] masks)
        {
            for (int i = x - thickness; i < x + thickness + 1; i++)
            {
                for (int j = y - thickness; j < y + thickness + 1; j++)
                {
                    if (map[i, j] == null && oceanBorder)
                        return;

                    if (!(x == i && y == j) && map[i, j] != null)
                    {
                        if (map[i, j].territory != this)
                        {
                            if (!borderingTerritories.Contains(map[i, j].territory))
                            {
                                borderingTerritories.Add(map[i, j].territory);
                                borders.Add(map[i, j].territory, new List<Tile>());
                            }

                            if (masks[0].getColorAt(i, j) != Color.Black)
                                tile.type = tileType.Mountain;
                            if (masks[1].getColorAt(i, j) != Color.Black)
                                tile.type = tileType.Desert;
                            if (masks[2].getColorAt(i, j) != Color.Black)
                                tile.type = tileType.Hill;



                            borders[map[i, j].territory].Add(tile);
                            return;
                        }
                    }
                }
            }
            return;
        }

        public void optimizeTiles(Tile[,] map)
        {
            int beforeCount = tiles.Count;
            color = Color.White;
            transparent = true;
            List<Tile> quadedTiles = new List<Tile>();

            quadTree(map, 0, 0, 1024, quadedTiles);
            tiles = quadedTiles;
            findBounds();
            findCenter();
            sortTilesBySize(tiles);

            //LOSSY VV
            double amount = 0.5;
            tiles.RemoveRange((int)(tiles.Count * amount), (int)(tiles.Count - (tiles.Count * amount)));
            beautifyTiles();

            Console.Out.WriteLine(beforeCount + " -> " + tiles.Count + " : " + (((double)(beforeCount - tiles.Count) / (double)beforeCount) * 100) + "%" + " " + (Math.Round((double)beforeCount / (double)tiles.Count)));
        }
        public void beautifyTiles()
        //DOES NOT WORK
        {

            List<Tile> uglyTiles = new List<Tile>();
            for (int t = 0; t < tiles.Count; t++)
            {
                Tile cTile = tiles[t];
                int[] neighbors = new int[4];
                // N E S W
                // 0 1 2 3

                for (int i = 0; i < cTile.width; i++)
                {
                    if (isThereATileAt(cTile.x + i, cTile.y - 1))
                        neighbors[0] = 1;
                    if (isThereATileAt(cTile.x + cTile.width, cTile.y + i))
                        neighbors[1] = 1;
                    if (isThereATileAt(cTile.x + i, cTile.y + cTile.width))
                        neighbors[2] = 1;
                    if (isThereATileAt(cTile.x - 1, cTile.y + i))
                        neighbors[3] = 1;
                }

                if (neighbors.Sum() == 1)
                    uglyTiles.Add(cTile);

                //UGLY TILE                MUCH BETTER
                //--> # <--       (#)           
                // #  #  #  ->  #  #  #  ->  #  #  #
                // #  #  #      #  #  #      #  #  #

                if (neighbors.Sum() == 2)
                {
                    if (neighbors[0] == 1 && neighbors[1] == 1)
                        cTile.shape = Shape.NorthEastRight;
                    if (neighbors[2] == 1 && neighbors[1] == 1)
                        cTile.shape = Shape.SouthEastRight;
                    if (neighbors[2] == 1 && neighbors[3] == 1)
                        cTile.shape = Shape.SouthWestRight;
                    if (neighbors[0] == 1 && neighbors[3] == 1)
                        cTile.shape = Shape.NorthWestRight;
                }
            }




            for (int i = 0; i < uglyTiles.Count; i++)
                tiles.Remove(uglyTiles[i]);

        }

        public bool isThereATileAt(int x, int y)
        {
            //Ah nice and simple
            for (int t = 0; t < tiles.Count; t++)
            {
                Tile cTile = tiles[t];
                if (new Rectangle(x, y, 1, 1).Intersects(new Rectangle(cTile.x, cTile.y, cTile.width, cTile.width)))
                    return true;
            }
            return false;
        }

        public static int nearestPowerOfTwo(int num)
        {
            int i = 1;
            while (Math.Pow(2, i) < num)
                i++;
            return (int)(Math.Pow(2, i));
        }
        public void findCenter()
        {
            //MORE EFFICIENT WITH WEIGHTED AVG THAN WITHOUT
            Vector2 values = new Vector2();
            double weights = 0.0;
            for (int i = 0; i < tiles.Count; i++)
            {
                weights += ((double)tiles[i].width / (double)tiles[0].width);
                values.X += (float)(tiles[i].x * ((double)tiles[i].width / (double)tiles[0].width));
                values.Y += (float)(tiles[i].y * ((double)tiles[i].width / (double)tiles[0].width));

            }
            center = new Vector2((float)(values.Y / weights), (float)(values.X / weights));

            //VVV DID NOT WORK?
            //bool inTerritory = false;
            //for (int i = 0; i < tiles.Count; i++)
            //{
            //    if (new Rectangle((int)center.X, (int)center.Y, 1, 1).Intersects(new Rectangle(tiles[i].y, tiles[i].x, tiles[i].width, tiles[i].height)))
            //    {
            //        inTerritory = true;
            //        break;
            //    }
            //}
            //if (!inTerritory)
            //    center = new Vector2(seedTile.y, seedTile.x);
        }
        public void findBounds()
        {
            bounds = new Rectangle(0, 0, 0, 0);
            int[] indexes = new int[4];
            // N E S W
            // 0 1 2 3

            //  N        /-\
            //W   E     |   v
            //  S        \_/

            // Xs and Ys are switched because life cant be too easy

            for (int i = 0; i < tiles.Count; i++)
            {
                if (tiles[i].y < tiles[indexes[3]].y)
                    indexes[3] = i;


                if (tiles[i].x > tiles[indexes[2]].x)
                    indexes[2] = i;
                if (tiles[i].y > tiles[indexes[1]].y)
                    indexes[1] = i;


                if (tiles[i].x < tiles[indexes[0]].x)
                    indexes[0] = i;
            }
            bounds = new Rectangle(tiles[indexes[3]].y, tiles[indexes[0]].x, tiles[indexes[1]].y + tiles[indexes[1]].height - tiles[indexes[3]].y, tiles[indexes[2]].x + tiles[indexes[2]].width - tiles[indexes[0]].x);
        }
        public void sortTilesBySize(List<Tile> tilesToSort)
        {
            tilesToSort.Sort((tile1, tile2) => tile2.width.CompareTo(tile1.width));
        }
        public void quadTree(Tile[,] map, int x, int y, int size, List<Tile> quadedTiles)
        {
            int half = size / 2;
            Rectangle[] sqaures = new Rectangle[4];
            sqaures[0] = new Rectangle(x, y, half, half); //topLeft
            sqaures[1] = new Rectangle(x + half, y, half, half); // topRight
            sqaures[3] = new Rectangle(x + half, y + half, half, half); // bottomRight 
            sqaures[2] = new Rectangle(x, y + half, half, half); // bottomLeft

            Boolean outside = false;
            Boolean inside = false;

            for (int r = 0; r < 4; r++)
            {
                Rectangle sqaure = sqaures[r];
                for (int i = sqaure.X; i < sqaure.X + sqaure.Width; i++)
                {

                    for (int j = sqaure.Y; j < sqaure.Y + sqaure.Height; j++)
                    {
                        if (map[j, i] != null)
                        {
                            if (map[j, i].territory == this)
                                inside = true;
                            else
                                outside = true;
                        }
                        else
                        {
                            outside = true;
                        }

                        if (outside && inside)
                        {
                            quadTree(map, sqaure.X, sqaure.Y, size / 2, quadedTiles);
                            j = sqaure.Y + sqaure.Height;
                            i = sqaure.X + sqaure.Width;
                            break;
                        }

                    }
                }
                if (inside && !outside)
                    quadedTiles.Add(new Tile(sqaure.X, sqaure.Y, sqaure.Width, sqaure.Height, this, tileType.Unknown));
            }

        }

        public void drawTiles(SpriteBatch batch, Texture2D tex, Rectangle zoom, Rectangle display)
        {
            double scale = 1024.0 / (double)zoom.Width;

            for (int i = 0; i < tiles.Count; i++)
            {
                if (hover && !selected && !transparent)
                    batch.Draw(tex, new Rectangle((int)((tiles[i].y - zoom.X) * scale) + display.X, (int)((tiles[i].x - zoom.Y - 5) * scale) + display.Y, (int)(tiles[i].width * scale + 1), (int)(tiles[i].height * scale + 1)), null, tiles[i].getColor() * 0.2f, 0, new Vector2(0), SpriteEffects.None, 0);


                if (!hover && !selected && !transparent)
                    batch.Draw(tex, new Rectangle((int)((tiles[i].y - zoom.X) * scale) + display.X, (int)((tiles[i].x - zoom.Y) * scale) + display.Y, (int)(tiles[i].width * scale + 1), (int)(tiles[i].height * scale + 1)), null, tiles[i].getColor() * 0.1f, 0, new Vector2(0), SpriteEffects.None, 1);


                if (hover && transparent)
                    batch.Draw(tex, new Rectangle((int)((tiles[i].y - zoom.X) * scale) + display.X, (int)((tiles[i].x - zoom.Y) * scale) + display.Y, (int)(tiles[i].width * scale + 1), (int)(tiles[i].height * scale + 1)), null, tiles[i].getColor() * 0.05f, 0, new Vector2(0), SpriteEffects.None, 1);


                if (selected)
                    batch.Draw(tex, new Rectangle((int)((tiles[i].y - zoom.X) * scale) + display.X, (int)((tiles[i].x - zoom.Y) * scale) + display.Y, (int)(tiles[i].width * scale + 1), (int)(tiles[i].height * scale + 1)), tiles[i].getColor() * 0.3f);

            }
        }

        public void drawBarracks(SpriteBatch batch, Texture2D tex, Rectangle zoom, Rectangle display)
        {

            double scale = 1024.0 / (double)zoom.Width;
            int barracksWidth = 10;

            if (hover && !selected && !transparent)
                batch.Draw(tex, new Rectangle((int)((center.X - zoom.X) * scale) + display.X, (int)((center.Y - zoom.Y) * scale) + display.Y, (int)(barracksWidth * scale), (int)(barracksWidth * scale)), null, getColor() * 0.5f, 0, new Vector2(tex.Width / 2, tex.Height), SpriteEffects.None, 0);


            if (!hover && !selected && !transparent)
                batch.Draw(tex, new Rectangle((int)((center.X - zoom.X) * scale) + display.X, (int)((center.Y - zoom.Y) * scale + display.Y), (int)(barracksWidth * scale), (int)(barracksWidth * scale)), null, getColor() * 0.7f, 0, new Vector2(tex.Width / 2, tex.Height), SpriteEffects.None, 0);


            if (selected)
                batch.Draw(tex, new Rectangle((int)((center.X - zoom.X) * scale) + display.X, (int)((center.Y - zoom.Y) * scale) + display.Y, (int)(barracksWidth * scale), (int)(barracksWidth * scale)), null, getColor() * 0.9f, 0, new Vector2(tex.Width / 2, tex.Height), SpriteEffects.None, 0);

        }

        public void drawBorder(SpriteBatch batch, Texture2D tex, Rectangle zoom, Rectangle display, Boolean biomeBorders, Boolean nationBorders, int wealthBorders, int troopBorders)
        {

            double scale = 1024.0 / (double)zoom.Width;


            for (int i = 0; i < borderingTerritories.Count; i++)
            {
                List<Tile> cBorder = borders[borderingTerritories[i]];
                for (int j = 0; j < cBorder.Count; j++)
                {
                    Color borderColor = new Color(12, 12, 12);

                    if (biomeBorders)
                        borderColor = cBorder[j].defaultColor;

                    if (nationBorders)
                        break;

                    if (wealthBorders != 0)
                    {
                        int wealthColor = (int)(255 * (getIncome() / (double)wealthBorders));
                        borderColor = new Color((int)(255 - (wealthColor / 1.75)), wealthColor, 0);
                    }

                    if (troopBorders != 0)
                    {
                        int troopColor = (int)(255 * (getTroopPower() / (double)(troopBorders)));
                        borderColor = new Color(troopColor, 0, 255 - troopColor);
                    }

                    if (hover)
                    {
                        batch.Draw(tex, new Rectangle((int)((cBorder[j].y - zoom.X) * scale) + display.X, (int)((cBorder[j].x + 3 - zoom.Y) * scale) + display.Y, (int)(cBorder[j].width * scale + 1), (int)(cBorder[j].height * scale + 1)), null, darken(borderColor, 0.5), 0, new Vector2(0, 0), SpriteEffects.None, 1);
                        batch.Draw(tex, new Rectangle((int)((cBorder[j].y - zoom.X) * scale) + display.X, (int)((cBorder[j].x + 2 - zoom.Y) * scale) + display.Y, (int)(cBorder[j].width * scale + 1), (int)(cBorder[j].height * scale + 1)), null, darken(borderColor, 0.55), 0, new Vector2(0, 0), SpriteEffects.None, 1);
                        batch.Draw(tex, new Rectangle((int)((cBorder[j].y - zoom.X) * scale) + display.X, (int)((cBorder[j].x + 1 - zoom.Y) * scale) + display.Y, (int)(cBorder[j].width * scale + 1), (int)(cBorder[j].height * scale + 1)), null, darken(borderColor, 0.6), 0, new Vector2(0, 0), SpriteEffects.None, 1);
                        batch.Draw(tex, new Rectangle((int)((cBorder[j].y - zoom.X) * scale) + display.X, (int)((cBorder[j].x + 1 - zoom.Y) * scale) + display.Y, (int)(cBorder[j].width * scale + 1), (int)(cBorder[j].height * scale + 1)), null, darken(borderColor, 0.65), 0, new Vector2(0, 0), SpriteEffects.None, 1);
                        batch.Draw(tex, new Rectangle((int)((cBorder[j].y - zoom.X) * scale) + display.X, (int)((cBorder[j].x - zoom.Y) * scale) + display.Y, (int)(cBorder[j].width * scale + 1), (int)(cBorder[j].height * scale + 1)), null, darken(borderColor, 0.7), 0, new Vector2(0, 0), SpriteEffects.None, 1);
                        batch.Draw(tex, new Rectangle((int)((cBorder[j].y - zoom.X) * scale) + display.X, (int)((cBorder[j].x - 1 - zoom.Y) * scale) + display.Y, (int)(cBorder[j].width * scale + 1), (int)(cBorder[j].height * scale + 1)), null, darken(borderColor, 0.75), 0, new Vector2(0, 0), SpriteEffects.None, 1);
                        batch.Draw(tex, new Rectangle((int)((cBorder[j].y - zoom.X) * scale) + display.X, (int)((cBorder[j].x - 2 - zoom.Y) * scale) + display.Y, (int)(cBorder[j].width * scale + 1), (int)(cBorder[j].height * scale + 1)), null, darken(borderColor, 0.8), 0, new Vector2(0, 0), SpriteEffects.None, 1);
                        batch.Draw(tex, new Rectangle((int)((cBorder[j].y - zoom.X) * scale) + display.X, (int)((cBorder[j].x - 3 - zoom.Y) * scale) + display.Y, (int)(cBorder[j].width * scale + 1), (int)(cBorder[j].height * scale + 1)), null, darken(borderColor, 0.9), 0, new Vector2(0, 0), SpriteEffects.None, 1);
                        batch.Draw(tex, new Rectangle((int)((cBorder[j].y - zoom.X) * scale) + display.X, (int)((cBorder[j].x - 4 - zoom.Y) * scale) + display.Y, (int)(cBorder[j].width * scale + 1), (int)(cBorder[j].height * scale + 1)), null, borderColor, 0, new Vector2(0, 0), SpriteEffects.None, 1);

                    }
                    else
                    {
                        batch.Draw(tex, new Rectangle((int)((cBorder[j].y - zoom.X) * scale) + display.X, (int)((cBorder[j].x - zoom.Y) * scale) + display.Y, (int)(cBorder[j].width * scale + 1), (int)(cBorder[j].height * scale + 1)), null, borderColor, 0, new Vector2(0, 0), SpriteEffects.None, 0);

                    }

                }

            }
        }

        public int getIncome()
        {
            return borderingTerritories.Count * inflation;
        }

        public int getTroopPower()
        {
            return (25 * troops[(int)TroopType.Rifleman]) + (20 * troops[(int)TroopType.Scout]) + (200 * troops[(int)TroopType.Tank]) + (80 * troops[(int)TroopType.Sniper]) + (50 * troops[(int)TroopType.Heavy]);
        }

        public static Color darken(Color c, double amount)
        {
            return new Color((int)(c.R * amount), (int)(c.G * amount), (int)(c.B * amount));
        }

    }
}
