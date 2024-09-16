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

    public enum MapStage
    {
        Start,
        Generate,
        StartingSpots,
        Turn,
        EndTurn,
        Win,
        End
    }

    class Map
    {

        Tile[,] grid;

        MapStage stage = MapStage.Generate;
        public List<Territory> territories;
        public List<Nation> nations;
        public int turn = 0;
        public Nation currentNation;
        Random rand;

        public static int[] prices = new int[5];

        bool instructions = true;

        //GERNATION
        List<Tile> tilesToDo = new List<Tile>();

        Popup currentPopup;

        public bool gracePeriod = true;


        Texture2D mapTex;


        //PLAYER
        // Actions
        // Action Points

        public List<MapLog> log = new List<MapLog>();
        Mask landMask;
        Mask moutainMask;
        Mask lakeMask;
        Mask marsMask;
        Mask hillsMask;

        Rectangle zoom;
        Rectangle display;
        Boolean isZooming = false;
        Boolean isPanning = false;
        int zoomDrawTime = 0;
        int zoomDrawCoolDown = 1;

        public Territory hoveringTerritory;
        List<Territory> selectedTerritories;

        Arrow selelctionArrow = new Arrow(new Vector2(0), new Vector2(10, 10), Color.White, 10, 1.0);
        List<Action> actions;

        Boolean biomeBorders = false;
        Boolean nationBorders = false;
        int wealthBorders = 0;
        int troopBorders = 0;

        public List<DrawTile> drawingTiles = new List<DrawTile>();
        List<Rectangle> recsToDo = new List<Rectangle>();


        List<Tile> water = new List<Tile>();

      

        public Battle currentBattle;


 
        public Map(Texture2D mapTex, Rectangle displayRec,Slider[] sliders)
        {
            this.display = displayRec;
            this.mapTex = mapTex;
            territories = new List<Territory>();
            nations = new List<Nation>();
            grid = new Tile[1024, 1024];
            zoom = new Rectangle(0, 0, 1024, 1024);
            selectedTerritories = new List<Territory>();
            actions = new List<Action>();
            Action.map = this;
            this.sliders = sliders;
           
        }

        public Slider[] sliders = new Slider[5];
        public void setUpMasks(Texture2D land, Texture2D moutains, Texture2D lakes, Texture2D mars, Texture2D hills)
        {
            hillsMask = new Mask(hills);
            landMask = new Mask(land);
            lakeMask = new Mask(lakes);
            marsMask = new Mask(mars);
            moutainMask = new Mask(moutains);
            rand = new Random();
        }
        public void setUpTerritories(int numOfTerritories, List<string> names)
        {

            for (int i = 0; i < numOfTerritories; i++)
            {
                int cordX = rand.Next(0, grid.GetLength(0) - 1);
                int cordY = rand.Next(0, grid.GetLength(0) - 1);

                //checks if that color is black
                while (landMask.getColorAt(cordY, cordX) != Color.White || lakeMask.getColorAt(cordY, cordX) == Color.White)
                {
                    cordX = rand.Next(1, grid.GetLength(0) - 1);
                    cordY = rand.Next(1, grid.GetLength(0) - 1);
                }
                //makes a new territory
                addNewTerritory(cordX, cordY, i, names[i]);
                territories[i].index = i;
                // if (mask[j + i * map.GetLength(0)] == Color.Black)
            }

           

        }
        public void setUpNations(int numOfNations)
        {
            //more money for more players
            Territory.inflation = (int)(Territory.inflation * (numOfNations / 2.0));
            for (int i = 0; i < numOfNations; i++)
            {
                switch (i)
                {
                    case 0:
                        nations.Add(new Nation(Color.Red));
                        break;
                    case 1:
                        nations.Add(new Nation(Color.Cyan));
                        break;
                    case 2:
                        nations.Add(new Nation(Color.Yellow));
                        break;
                    case 3:
                        nations.Add(new Nation(Color.Magenta));
                        break;
                }
            }
            currentNation = nations[0];
            selelctionArrow.color = currentNation.color;
        }
        public void update(MouseState oldMouse,KeyboardState oKb, GameTime gameTime, int timer)
        {

            MouseState mouse = Mouse.GetState();
            KeyboardState kb = Keyboard.GetState();
            if (recsToDo.Count > 0)
            {
                quadTreeIterative(drawingTiles, 8);
            }

            if (stage == MapStage.Generate)
            {
                iterateMap(130);

                for(int i = 0; i < territories.Count; i++)
                {
                    //SOMETIMES THE MASKS DONT WORK AND I NEED TO REDO THE GENERATION
                    if(territories[i].tiles.Count < 3)
                    {
                        tilesToDo.Clear();
                        grid = new Tile[1024, 1024];
                        for (int j = 0; j < territories.Count; j++)
                        {
                            int cordX = rand.Next(0, grid.GetLength(0) - 1);
                            int cordY = rand.Next(0, grid.GetLength(0) - 1);

                            //checks if that color is black
                            while (landMask.getColorAt(cordY, cordX) != Color.White || lakeMask.getColorAt(cordY, cordX) == Color.White)
                            {
                                cordX = rand.Next(1, grid.GetLength(0) - 1);
                                cordY = rand.Next(1, grid.GetLength(0) - 1);
                            }
                            territories[j].tiles.Clear();
                            territories[j].seedTile = new Tile(cordX, cordY, territories[j], tileType.Unknown);
                            territories[j].tiles.Add(territories[j].seedTile);
                            
                            grid[cordY, cordX] = new Tile(cordX, cordY, territories[j], tileType.Unknown);
                            tilesToDo.Add(grid[cordY, cordX]);
                        }
                        i = territories.Count;
                    }
                }
               
            }
            handleZoom(oldMouse);

            if (!kb.IsKeyDown(Keys.Escape) && oKb.IsKeyDown(Keys.Escape))
                instructions = !instructions;

            if (kb.IsKeyDown(Keys.B))
                biomeBorders = true;
            else
                biomeBorders = false;

            if (kb.IsKeyDown(Keys.N))
                nationBorders = true;
            else
                nationBorders = false;

            if (kb.IsKeyDown(Keys.M))
            {
                int maxWealth = 0;
                for (int i = 0; i < territories.Count; i++)
                {
                    if (territories[i].getIncome() > maxWealth)
                        maxWealth = territories[i].getIncome();
                }
                wealthBorders = maxWealth;
            }
            else
                wealthBorders = 0;

            if (kb.IsKeyDown(Keys.V))
            {
                int maxTroop = 0;
                for (int i = 0; i < territories.Count; i++)
                {
                    if (territories[i].getTroopPower() > maxTroop)
                        maxTroop = territories[i].getTroopPower();
                }
                troopBorders = maxTroop;
            }
            else
                troopBorders = 0;








            if (stage == MapStage.StartingSpots)
            {
                handleHovering(mouse);
                pickStartingSpots(mouse, oldMouse);
                selelctionArrow.updateDestination(new Vector2(mouse.X - display.X, mouse.Y - display.Y));
                selelctionArrow.updateOrigin(new Vector2(mouse.X - display.X + 20, mouse.Y -display.Y + 20));
                mouse = oldMouse;
            }


            //Turn actions
            if (stage == MapStage.Turn)
            {

                if((int)Math.Floor((double)turn / nations.Count) == 4 && gracePeriod)
                    gracePeriod = false;
                
                if (kb.IsKeyDown(Keys.Space) && currentNation.actionPoints == 0)
                {
                    stage = MapStage.EndTurn;
                    if (actions.Count > 0)
                        actions[0].startAnimation(120);
                }

                if (!kb.IsKeyDown(Keys.P) && oKb.IsKeyDown(Keys.P))
                {
                    stage = MapStage.EndTurn;
                    if (actions.Count > 0)
                        actions[0].startAnimation(120);
                }
                handleHovering(mouse);

                if (currentPopup != null)
                {
                    currentPopup.update(kb, mouse);
                    if (currentPopup.done)
                    {
                        actions.Add(new Action(currentPopup.type, selectedTerritories[0], selectedTerritories[1], currentNation));
                        actions[actions.Count - 1].updateTroopAmount(currentPopup.troopCount);
                        currentNation.actionPoints--;
                        currentPopup = null;
                        oldMouse = mouse;
                        for (int i = 0; i < selectedTerritories.Count; i++)
                            selectedTerritories[i].selected = false;
                        selectedTerritories.Clear();
                    }
                }
                if (currentPopup != null)
                {
                    if (currentPopup.popup == PopupStage.None)
                    {
                        currentPopup = null;
                        for (int i = 0; i < selectedTerritories.Count; i++)
                            selectedTerritories[i].selected = false;
                        selectedTerritories.Clear();
                    }
                }

                if (selelctionArrow != null && currentPopup == null)
                {
                    selelctionArrow.updateDestination(new Vector2(mouse.X - display.X, mouse.Y - display.Y));
                    if (selectedTerritories.Count == 0)
                    {
                        selelctionArrow.updateOrigin(new Vector2(mouse.X - display.X + 20, mouse.Y - display.Y + 20));
                    }
                }

                if (mouse.LeftButton == ButtonState.Released && oldMouse.LeftButton == ButtonState.Pressed && currentPopup == null) //LMB
                {
                    if (hoveringTerritory != null)
                    {
                        Boolean inLine = false;
                        for (int i = 0; i < actions.Count; i++)
                        {
                            if (actions[i].destination == hoveringTerritory)
                                inLine = true;
                        }
                        //Select one of your territoires to start
                        if ((hoveringTerritory.nation == currentNation || inLine) && selectedTerritories.Count == 0)
                        {
                            selectedTerritories.Add(hoveringTerritory);
                            hoveringTerritory.selected = true;
                            updateTerritoryDrawTiles(hoveringTerritory);
                            selelctionArrow = new Arrow(hoveringTerritory.center, new Vector2(mouse.X - display.X, mouse.Y - display.Y), currentNation.color, 10, 1.0);
                            Sounds.playSound("click");
                        }
                        //Select another territory
                        if (selectedTerritories.Count >= 1)
                        {
                            if (selectedTerritories[0].borderingTerritories.Contains(hoveringTerritory))
                            {
                                selectedTerritories.Add(hoveringTerritory);
                                hoveringTerritory.selected = true;
                                updateTerritoryDrawTiles(hoveringTerritory);
                                Sounds.playSound("click");
                            }

                        }
                        //obsoltete
                        if (!selectedTerritories.Contains(hoveringTerritory) && selectedTerritories.Count >= 2)
                        {
                            selectedTerritories[1].selected = false;
                            selectedTerritories.RemoveAt(1);
                            updateTerritoryDrawTiles(hoveringTerritory);
                        }

                        


                        if (selectedTerritories.Count == 2 && currentNation.actionPoints > 0 && currentPopup == null)
                        {
                            selelctionArrow.updateDestination(selectedTerritories[1].center);

                            //Include troops from chain
                            int[] troops1 = new int[5];

                            for (int i = 0; i < 5; i++)
                                troops1[i] += selectedTerritories[0].troops[i];

                            for (int i = 0; i < actions.Count; i++)
                            {
                                if (actions[i].destination == selectedTerritories[0] && actions[i].type != ActionType.Attack)
                                {
                                    for (int j = 0; j < 5; j++)
                                        troops1[j] += actions[i].troops[j];
                                }

                                if (actions[i].origin == selectedTerritories[0])
                                {
                                    for (int j = 0; j < 5; j++)
                                        troops1[j] -= actions[i].troops[j];
                                }
                            }

                            if (currentNation != selectedTerritories[1].nation && selectedTerritories[1].occupied && !gracePeriod)
                            {

                                //promt attack or foregin aid
                                currentPopup = new Popup(PopupStage.AttackorMove, ActionType.Undecided, troops1, currentNation.color);

                            }
                            else if (currentNation == selectedTerritories[1].nation)
                            {
                                //promt transfer
                                currentPopup = new Popup(PopupStage.Move, ActionType.Transfer, troops1, currentNation.color);
                            }
                            else
                            { //promt aquire
                                currentPopup = new Popup(PopupStage.Move, ActionType.Transfer, troops1, currentNation.color);
                            }

                            //Percent Based If There was a battle in chain
                            for (int i = 0; i < actions.Count; i++)
                            {
                                if (actions[i].destination == selectedTerritories[0] && actions[i].type == ActionType.Attack)
                                {
                                    currentPopup.percent = true;
                                    break;
                                }
                            }

                        } 
                        
                    }
                }
                if (mouse.RightButton == ButtonState.Released && oldMouse.RightButton == ButtonState.Pressed) //RMB
                {

                    if (hoveringTerritory != null)
                    {
                        if (selectedTerritories.Contains(hoveringTerritory))
                        {
                            //remove selcetion

                            if (hoveringTerritory == selectedTerritories[0])
                            {
                                for (int i = 0; i < selectedTerritories.Count; i++)
                                {
                                    selectedTerritories[i].selected = false;
                                    updateTerritoryDrawTiles(selectedTerritories[i]);
                                }

                                selectedTerritories.Clear();

                            }
                            else
                            {
                                selectedTerritories.Remove(hoveringTerritory);
                                hoveringTerritory.selected = false;
                                updateTerritoryDrawTiles(hoveringTerritory);
                            }
                            Sounds.playSound("dismiss");
                        }
                        else
                        {
                            //remove actions
                            for (int i = 0; i < actions.Count; i++)
                            {
                                bool removed = false;
                                if (actions[i].origin == hoveringTerritory)
                                {
                                    actions.Remove(actions[i]);
                                    currentNation.actionPoints++;
                                    i--;
                                    Sounds.playSound("dismiss");
                                    removed = true;
                                }
                                //remove all in chain if needed
                                while (removed)
                                {
                                    removed = false;
                                    for (int j = 0; j < actions.Count; j++)
                                    {
                                        Territory actionOr = actions[j].origin;
                                        bool arrowPointing = actions[j].initiator == actions[j].origin.nation;
                                        for (int k = 0; k < actions.Count; k++)
                                        {
                                            if (actions[k].destination == actionOr)
                                                arrowPointing = true;

                                        }
                                        if (!arrowPointing)
                                        {
                                            removed = true;
                                            actions.Remove(actions[j]);
                                            currentNation.actionPoints++;
                                            j--;
                                        }
                                    }

                                }
                                   
                            }
                        }
                    }
                }
            }

            //End of Turn resoltions
            if (stage == MapStage.EndTurn)
            {
                if (actions.Count > 0)
                {
                    Action currentAction = actions[0];
                    if (currentAction.type == ActionType.Attack)
                    {
                        zoomIn(currentAction.destination.center, 40.0);
                    }

                    if (currentAction.animationTime == 0)
                    {
                        currentAction.complete();
                        zoom = new Rectangle(0, 0, 1024, 1024);
                    }
                    if (currentAction.finshied && currentBattle == null)
                    {
                        //remove loser

                        actions.Remove(currentAction);
                        if (actions.Count == 0)
                        {
                            startTurn();
                        }
                        else
                        {
                            actions[0].startAnimation(120);
                        }
                    }
                }
                else
                {
                    startTurn();
                }

                if (currentBattle != null)
                {
                    selelctionArrow.updateOrigin(new Vector2(-100,-100));
                    selelctionArrow.updateDestination(new Vector2(-100, -100));
                    currentBattle.Update(gameTime);
                    if (currentBattle.BattleOver())
                    {
                        Territory winner = currentBattle.getWinner();

                        if (winner == currentBattle.attacker)
                        {
                           
                            currentBattle.defender.nation.territories.Remove(currentBattle.defender);
                            currentBattle.defender.nation.findBorderTiles();
                            if (currentBattle.defender.nation.territories.Count == 0)
                            {
                                //remove loser
                                for (int i = 0; i < nations.Count; i++)
                                {
                                    if (currentBattle.defender.nation == nations[i])
                                    {
                                        nations.Remove(nations[i]);
                                        Console.Out.WriteLine("LOSE");
                                        Sounds.playSound("battleLost");
                                        break;
                                    }
                                }
                                
                               
                            }
                            currentBattle.defender.nation = currentBattle.attacker.nation;
                            log.Add(new MapLog(currentBattle.attacker.nation.color, currentBattle.defender.index));
                            currentBattle.attacker.nation.territories.Add(currentBattle.defender);
                            currentBattle.attacker.nation.findBorderTiles();
                            //crown winner
                            if (nations.Count == 1)
                            {
                                stage = MapStage.Win;
                                Sounds.playSound("GameWon");
                                Console.Out.WriteLine("WIN");
                                for (int i = 0; i < territories.Count; i++)
                                {
                                    territories[i].nation = null;
                                    territories[i].occupied = false;
                                    territories[i].transparent = true;
                                }
                            }
                            currentBattle.defender.troops = currentBattle.leftovers();
                        }
                        if (winner == currentBattle.defender)
                        {
                            currentBattle.defender.troops = currentBattle.leftovers();
                        }


                        currentBattle = null;
                        zoom = new Rectangle(0, 0, 1024, 1024);


                    }
                }

            }

            if (stage == MapStage.Win)
            {
                selelctionArrow.updateDestination(new Vector2(mouse.X - display.X, mouse.Y - display.Y));
                selelctionArrow.updateOrigin(new Vector2(mouse.X - display.X + 20, mouse.Y - display.Y + 20));

                Console.Out.WriteLine("WINNING");
                if (actions.Count > 0)
                    actions.Clear();

                if(timer % (int)(((double)(30.0)/ log.Count) * 60) == 0)
                {
                    if (MapLog.logTime < log.Count)
                    {
                        territories[log[MapLog.logTime].index].color = log[MapLog.logTime].color;
                        territories[log[MapLog.logTime].index].occupied = true;
                        territories[log[MapLog.logTime].index].transparent = false;
                        updateTerritoryDrawTiles(territories[log[MapLog.logTime].index]);
                    }

                    if (MapLog.logTime > log.Count + 3)
                    {
                        stage = MapStage.End;
                    }
                        MapLog.logTime++;
                }

            }

            if(stage == MapStage.End)
            {
                selelctionArrow.updateDestination(new Vector2(mouse.X - display.X, mouse.Y - display.Y));
                selelctionArrow.updateOrigin(new Vector2(mouse.X + 20 - display.X, mouse.Y + 20 -display.Y));
            }
        }
        public void handleHovering(MouseState mouse)
        {
            double scale = 1024.0 / (double)zoom.Width;

            if(new Rectangle(mouse.X,mouse.Y,1,1).Intersects(display))
            {
                for (int i = 0; i < territories.Count; i++)
                {
                    if (territories[i] != hoveringTerritory)
                    {
                        Territory cTer = territories[i];
                        Rectangle cBounds = new Rectangle((int)((cTer.bounds.X - zoom.X) * scale), (int)((cTer.bounds.Y - zoom.Y) * scale), (int)((cTer.bounds.Width * scale)), (int)((cTer.bounds.Height * scale)));
                        Rectangle mouseRec = new Rectangle((int)((mouse.X - zoom.X - display.X) * scale), (int)((mouse.Y - zoom.Y - display.Y) * scale), 1, 1);
                        if (mouseRec.Intersects(cBounds))
                        {
                            for (int j = 0; j < cTer.tiles.Count; j++)
                            {
                                Rectangle tileRec = new Rectangle((int)((cTer.tiles[j].y - zoom.X) * scale), (int)((cTer.tiles[j].x - zoom.Y) * scale), (int)(cTer.tiles[j].width * scale), (int)(cTer.tiles[j].height * scale));

                                if (mouseRec.Intersects(tileRec))
                                {
                                    if (hoveringTerritory != null)
                                        hoveringTerritory.hover = false;


                                    updateTerritoryDrawTiles(hoveringTerritory);
                                    hoveringTerritory = cTer;
                                    hoveringTerritory.hover = true;

                                    updateTerritoryDrawTiles(hoveringTerritory);
                                }
                            }
                        }
                    }
                }

            } else  {
                hoveringTerritory = null;
            }
        }
        public void handleZoom(MouseState oldMouse)
        {
            MouseState mouse = Mouse.GetState();

            double amount = zoom.Width / 8;

            //panning
            if (mouse.MiddleButton == ButtonState.Pressed)
            {
                zoom.X -= mouse.X - oldMouse.X;
                zoom.Y -= mouse.Y - oldMouse.Y;
                isPanning = true;
                zoomDrawTime = 0;
                drawingTiles.Clear();
                recsToDo.Clear();
            }



            if ((isZooming || isPanning) && mouse.ScrollWheelValue == oldMouse.ScrollWheelValue)
                zoomDrawTime++;


            if ((isZooming || isPanning) && zoomDrawTime == zoomDrawCoolDown)
            {
                isZooming = false;
                setDrawTiles();
            }

            if (mouse.ScrollWheelValue > oldMouse.ScrollWheelValue)
            {
                isZooming = true;
                zoomDrawTime = 0;
                drawingTiles.Clear();
                recsToDo.Clear();
                //ZOOM IN
                double xScale = ((mouse.X - 512.0 - display.X) / 512.0) + 1;
                double yScale = ((mouse.Y - 512.0 - display.Y) / 512.0) + 1;

                zoom.X += (int)((amount * xScale) / 2);
                zoom.Width -= (int)amount;
                zoom.Y += (int)((amount * yScale) / 2);
                zoom.Height -= (int)amount;
            }

            if (mouse.ScrollWheelValue < oldMouse.ScrollWheelValue && zoom.Width != 1024)
            {
                //ZOOM OUT
                // zoom.X -> 0 when zoom.Width -> 1024 && amount -> 128
                isZooming = true;
                zoomDrawTime = 0;
                drawingTiles.Clear();
                recsToDo.Clear();

                //Doesnt work oh well
                double xScale = (((zoom.X + (zoom.Width / 2) - display.X/2) - 512.0) / 512.0) + 1;
                double yScale = (((zoom.Y + (zoom.Height / 2) - display.Y/2) - 512.0) / 512.0) + 1;

                zoom.X -= (int)((amount * xScale) / 2);
                zoom.Width += (int)amount;
                zoom.Y -= (int)((amount * yScale) / 2);
                zoom.Height += (int)amount;
            }

            while (zoom.X + zoom.Width > 1024)
                zoom.X--;
            while (zoom.X < 0)
                zoom.X++;
            while (zoom.Y + zoom.Height > 1024)
                zoom.Y--;
            while (zoom.Y < 0)
                zoom.Y++;

            if (zoom.Width > 1024)
                zoom.Width -= (int)Math.Abs(zoom.Width - 1024);

            if (zoom.X < 0)
                zoom.X += (int)Math.Abs(zoom.X);

            if (zoom.Height > 1024)
                zoom.Height -= (int)Math.Abs(zoom.Height - 1024.0);

            if (zoom.Y < 0)
                zoom.Y += (int)Math.Abs(zoom.Y);
        }

        public void zoomIn(Vector2 point, double by)
        {



            // double amount = zoom.Width / 8;
            double amount = (double)zoom.Width / by;

            //ZOOM IN

            zoomDrawTime = 0;
            drawingTiles.Clear();
            recsToDo.Clear();
 

            double xScale = ((point.X - 512) / 512.0) + 1;
            double yScale = ((point.Y - 512) / 512.0) + 1;


            zoom.X += (int)((amount * xScale) / 2);
            zoom.Width -= (int)amount;
            zoom.Y += (int)((amount * yScale) / 2);
            zoom.Height -= (int)amount;

            

            isZooming = true;
        }

        public void startTurn()
        {
            turn++;
            
            
            
            stage = MapStage.Turn;
            currentNation = nations[turn % nations.Count];


            currentNation.gainIncome();
            selelctionArrow.color = currentNation.color;
            for (int i = 0; i < 5; i++)
            {
                sliders[i].max = (int)Math.Floor((double)currentNation.money / (double)prices[i]);
                sliders[i].at = 0;
                sliders[i].color = currentNation.color;
            }
            currentNation.actionPoints = 2;
            for (int i = 0; i < selectedTerritories.Count; i++)
            {
                selectedTerritories[i].selected = false;
                updateTerritoryDrawTiles(selectedTerritories[i]);
            }

            selectedTerritories.Clear();

        }

        public Boolean iterateMap(int amountTodo)
        {
            //GENERATION ALOGO
            List<Tile> tilesToAdd = new List<Tile>();
            if (tilesToDo.Count > 0)
            {
                for (int k = 0; k < Math.Min(tilesToDo.Count, amountTodo); k++)
                {
                    int thickness = rand.Next(1, 3);
                    Tile cTile = tilesToDo[k];

                    for (int i = cTile.x - thickness; i < cTile.x + thickness + 1; i++)
                    {
                        for (int j = cTile.y - thickness; j < cTile.y + thickness + 1; j++)
                        {
                            if (!generateAt(i, j, thickness, tilesToAdd))
                                break;
                        }
                    }
                }

                tilesToDo.RemoveRange(0, Math.Min(amountTodo, tilesToDo.Count));
                //VVV COMESMETIC
                tilesToAdd = shuffleTiles(tilesToAdd);
                tilesToDo.AddRange(tilesToAdd);
            }

            if (tilesToDo.Count == 0 && stage == MapStage.Generate)
            {
   
                stage = MapStage.StartingSpots;

                oceanQuadTree(0, 0, 1024, water, new Territory());
                water.Sort((tile1, tile2) => tile2.width.CompareTo(tile1.width));
                //LOSSY VV
                double amount = 0.4;
                water.RemoveRange((int)(water.Count * amount), (int)(water.Count - (water.Count * amount)));



                setTerritoriesBorders();
                optimizeTerritoriesTiles();

                setDrawTiles();
                return true;
            }


            return false;
        }
        public Boolean generateAt(int i, int j, int thickness, List<Tile> tilesToAdd)
        {
            if (landMask.getColorAt(j, i) != Color.White || lakeMask.getColorAt(j, i) != Color.Black)
                return false;

            //checks if the selected tile is null
            if (grid[j, i] == null)
            {
                //makes a new tile
                grid[j, i] = new Tile(i, j, changeTerritory(j, i, thickness), tileType.Unknown);
                grid[j, i].territory.tiles.Add(grid[j, i]);

                //adds it to the tilesToAdd
                tilesToAdd.Add(grid[j, i]);
            }
            else
            {
                Territory oldTerr = grid[j, i].territory;
                grid[j, i].territory = changeTerritory(j, i, thickness);

                if (oldTerr != grid[j, i].territory)
                {
                    oldTerr.tiles.Remove(grid[j, i]);
                    grid[j, i].territory.tiles.Add(grid[j, i]);
                }
            }
            return true;
        }
        public Territory changeTerritory(int x, int y, int thickness)
        {
            Dictionary<Territory, int> numOfTerritory = new Dictionary<Territory, int>();

            Boolean allNull = true;

            for (int i = x - thickness; i < x + thickness + 1; i++)
            {
                for (int j = y - thickness; j < y + thickness + 1; j++)
                {
                    if (!(x == i && y == j) && grid[i, j] != null)
                    {
                        allNull = false;
                        for (int k = 0; k < territories.Count; k++)
                        {
                            // if (landMask.getColorAt(i, j) != Color.Black)
                            // {
                            if (grid[i, j].territory == territories[k])
                            {

                                if (!numOfTerritory.ContainsKey(territories[k]))
                                    numOfTerritory.Add(territories[k], 1);
                                else
                                    numOfTerritory[territories[k]]++;
                            }
                            // }
                        }

                    }
                }
            }

            if (allNull)
                Console.Out.WriteLine("allNull (Bad)");

            Territory maxTerritory = new Territory();
            int maxColorNum = 0;

            for (int k = 0; k < territories.Count; k++)
            {
                if (numOfTerritory.ContainsKey(territories[k]))
                {
                    if (maxColorNum < numOfTerritory[territories[k]])
                    {
                        maxTerritory = territories[k];
                        maxColorNum = numOfTerritory[territories[k]];
                    }
                }

            }
            return maxTerritory;
        }
        public void pickStartingSpots(MouseState mouse, MouseState oldMouse)
        {


            if (mouse.LeftButton == ButtonState.Released && oldMouse.LeftButton == ButtonState.Pressed && hoveringTerritory != null)
            {

                if (hoveringTerritory.nation == null)
                {
                    hoveringTerritory.selected = true;
                    if (selectedTerritories.Contains(hoveringTerritory))
                    {
                        currentNation.territories.Add(hoveringTerritory);
                        hoveringTerritory.transparent = false;
                        hoveringTerritory.occupied = true;
                        hoveringTerritory.nation = currentNation;
                        log.Add(new MapLog(hoveringTerritory.getColor(), hoveringTerritory.index));
                        hoveringTerritory.barracks = true;
                        setNationsBorders();
                        hoveringTerritory.troops[0] += 5;
                        turn++;
                        Sounds.playSound("click");
                        selectedTerritories.Remove(hoveringTerritory);
                        hoveringTerritory.selected = false;
                        if (turn == nations.Count)
                        {
                            for (int i = 0; i < selectedTerritories.Count; i++)
                            {
                                selectedTerritories[i].selected = false;
                                updateTerritoryDrawTiles(selectedTerritories[i]);
                            }

                            selectedTerritories.Clear();
                            turn--;
                            startTurn();
                        }
                        else
                        {
                            currentNation = nations[turn];
                            selelctionArrow.color = currentNation.color;
                        }
                    }
                    else
                    {

                        for (int i = 0; i < selectedTerritories.Count; i++)
                        {
                            selectedTerritories[i].selected = false;
                            selectedTerritories[i].hover = false;
                            selectedTerritories[i].transparent = true;
                            updateTerritoryDrawTiles(selectedTerritories[i]);
                        }
                        selectedTerritories.Clear();
                        selectedTerritories.Add(hoveringTerritory);
                        hoveringTerritory.selected = true;
                        hoveringTerritory.transparent = false;
                    }
                    updateTerritoryDrawTiles(hoveringTerritory);
                }
            }

            if (mouse.RightButton == ButtonState.Released && oldMouse.RightButton == ButtonState.Pressed)
            {
                if (hoveringTerritory != null)
                {
                    if (selectedTerritories.Contains(hoveringTerritory))
                    {
                        selectedTerritories.Remove(hoveringTerritory);
                        hoveringTerritory.selected = false;
                        Sounds.playSound("dismiss");
                    }
                }
                updateTerritoryDrawTiles(hoveringTerritory);
            }


        }

        public void setTerritoriesBorders()
        {
            Mask[] masks = new Mask[3];
            masks[0] = moutainMask;
            masks[1] = marsMask;
            masks[2] = hillsMask;

            for (int i = 0; i < territories.Count; i++)
            {
                territories[i].borderingTerritories = new List<Territory>();
                territories[i].findBorders(grid, 1, false, masks);

            }
        }
        public void optimizeTerritoriesTiles()
        {
            for (int i = 0; i < territories.Count; i++)
            {
                territories[i].optimizeTiles(grid);
            }

            landMask = null;
            lakeMask = null;
            marsMask = null;
            moutainMask = null;
            hillsMask = null;
            grid = null;
        }
        public void setNationsBorders()
        {
            for (int i = 0; i < nations.Count; i++)
            {
                nations[i].borders = new List<List<Tile>>();
                nations[i].findBorderTiles();
            }
        }
        public void addNewTerritory(int x, int y, int colorNum, string name)
        {
            territories.Add(new Territory(x, y, nations[colorNum % nations.Count].color, name));
            grid[y, x] = new Tile(x, y, territories.Last<Territory>(), tileType.Unknown);
            tilesToDo.Add(grid[y, x]);
        }

        public static double GetDistance(Vector2 one, Vector2 two)
        {
            return Math.Sqrt(Math.Pow((two.X - one.X), 2) + Math.Pow((two.Y - one.Y), 2));
        }
        public void drawAll(SpriteBatch batch, Texture2D white, int timer)
        {
            double scale = 1024.0 / (double)zoom.Width;



            batch.Draw(mapTex, new Rectangle(display.X, display.Y, 1024, 1024), zoom, Color.White);


            if (tilesToDo.Count > 0)
            {
                for (int i = 0; i < tilesToDo.Count; i++)
                {
                    double distanceToCenter = (GetDistance(new Vector2(512, 512), new Vector2(tilesToDo[i].y, tilesToDo[i].x)) / 512.0);

                    Color color = new Color((int)((1 - distanceToCenter) * 255), (int)(0), (int)(distanceToCenter * 255));
                    batch.Draw(white, new Rectangle((int)((tilesToDo[i].y - zoom.X) * scale) + display.X, (int)((tilesToDo[i].x - zoom.Y - 3) * scale) + display.Y, (int)(tilesToDo[i].width * scale + 1), (int)(tilesToDo[i].height * scale + 1)), color);
                    batch.Draw(white, new Rectangle((int)((tilesToDo[i].y - zoom.X) * scale) + display.X, (int)((tilesToDo[i].x - zoom.Y - 2) * scale) + display.Y, (int)(tilesToDo[i].width * scale + 1), (int)(tilesToDo[i].height * scale + 1)), darken(color, 0.8));
                    batch.Draw(white, new Rectangle((int)((tilesToDo[i].y - zoom.X) * scale) + display.X, (int)((tilesToDo[i].x - zoom.Y - 1) * scale) + display.Y, (int)(tilesToDo[i].width * scale + 1), (int)(tilesToDo[i].height * scale + 1)), darken(color, 0.75));
                    batch.Draw(white, new Rectangle((int)((tilesToDo[i].y - zoom.X) * scale) + display.X, (int)((tilesToDo[i].x - zoom.Y) * scale) + display.Y, (int)(tilesToDo[i].width * scale + 1), (int)(tilesToDo[i].height * scale + 1)), darken(color, 0.7));
                    batch.Draw(white, new Rectangle((int)((tilesToDo[i].y - zoom.X) * scale) + display.X, (int)((tilesToDo[i].x - zoom.Y + 1) * scale) + display.Y, (int)(tilesToDo[i].width * scale + 1), (int)(tilesToDo[i].height * scale + 1)), darken(color, 0.65));
                    batch.Draw(white, new Rectangle((int)((tilesToDo[i].y - zoom.X) * scale) + display.X, (int)((tilesToDo[i].x - zoom.Y + 2) * scale) + display.Y, (int)(tilesToDo[i].width * scale + 1), (int)(tilesToDo[i].height * scale + 1)), darken(color, 0.6));
                }

            }
            else
            {

                if (!isZooming && recsToDo.Count == 0 && drawingTiles.Count > 0)
                {
                    for (int j = 0; j < drawingTiles.Count; j++)
                    {
                        batch.Draw(white, new Rectangle(drawingTiles[j].x + display.X, drawingTiles[j].y + display.Y, drawingTiles[j].width, drawingTiles[j].height), null, drawingTiles[j].color * drawingTiles[j].opacity, 0, new Vector2(0), SpriteEffects.None, 0);
                    }
                }


                for (int j = 0; j < territories.Count; j++)
                {

                    if (!territories[j].hover)
                    {
                        if (recsToDo.Count > 0 || isZooming)
                            territories[j].drawTiles(batch, white, zoom, display);
                        if (territories[j].barracks)
                            territories[j].drawBarracks(batch, white, zoom, display);
                        territories[j].drawBorder(batch, white, zoom, display, biomeBorders, nationBorders, wealthBorders, troopBorders);
                    }

                }

                if (hoveringTerritory != null)
                {
                    if (recsToDo.Count > 0 || isZooming)
                        hoveringTerritory.drawTiles(batch, white, zoom, display);
                    if (hoveringTerritory.barracks)
                        hoveringTerritory.drawBarracks(batch, white, zoom, display);

                    hoveringTerritory.drawBorder(batch, white, zoom, display, biomeBorders, nationBorders, wealthBorders, troopBorders);
                }




                if (nationBorders && hoveringTerritory != null)
                {
                    for (int i = 0; i < nations.Count; i++)
                    {
                        nations[i].drawBorders(batch, white, zoom, display);
                    }
                    if (hoveringTerritory.nation != null)
                        hoveringTerritory.nation.drawBorders(batch, white, zoom, display);
                }
            }

            for (int i = 0; i < actions.Count; i++)
                actions[i].draw(batch, white, zoom, display);



            

          

            int num = 200;
            int height = (int)((double)1024.0 / ((double)num));

            for (int i = 0; i < num; i++)
            {
                int y = (int)(((i / (double)(num / 2)) * 1024.0));

                batch.Draw(white, new Rectangle(0, (int)(y + ((((timer / 5) % (height * 2)) / (double)(height * 2)) * (height * 2))), 3000, height), Color.White * 0.01f);
            }
        }

        public void drawCursor(SpriteBatch batch, Texture2D white)
        {

            if ((currentBattle == null || currentPopup == null))
            {
                Console.Out.WriteLine("curoser");
                if (selelctionArrow != null)
                    selelctionArrow.draw(batch, white, zoom, display);
            }
            
        }

        public void drawCursor(SpriteBatch batch, Texture2D white, bool ha)
        {
            MouseState mouse = Mouse.GetState();
            if (!(new Rectangle(mouse.X, mouse.Y, 1, 1).Intersects(display)) && ha && currentBattle == null)
            {
                if (selelctionArrow != null)
                    selelctionArrow.draw(batch, white, zoom, display);
            }

        }

        public void drawPopup(SpriteBatch batch)
        {
            if (currentPopup != null)
                currentPopup.draw(batch);

        }

        public void drawInstructions(SpriteBatch batch, Texture2D white, SpriteFont smallFont)
        {
            if (instructions)
            {
                batch.Draw(white, display, Color.Black * 0.8f);
                batch.DrawString(smallFont, "\n Welcome to Mars! \n Conquer your enemies and achieve world domination! \n \n Start by picking your starting location, then take turns planning actions and distributing troops.  \n  \n Controls : \n \n ESC to bring up this menu! \n LMB to select \n RMB to deselect \n \n Map : \n \n SCROLL to zoom \n MMB to pan \n Click a territory to make a new action, click another territory to confirm \n RMB an unselected territory to remove its actions \n SPACE to end your turn (only with 0 action points) \n P to pass your turn \n Hold V to see the troop border overlay \n Hold B to see the biome border overlay \n Hold N to see the nation border overlay \n Hold M to see the wealth border overlay \n To remember, CalVary, Biome, Nation, Wealth \n \n Shop : \n \n Action points are used in the shop and for making actions \n Gambling randomly gives a number of troops to a random territory of yours with a barracks \n Territories with Barracks can have troops bought into them, territories without  Barracks cannot receive troops \n from the shop \n Those sliders can be used to select what troops to buy  \n \n Battle : \n \n Press Up Arrow to speed up the game \n Press Down Arrow to slow down the game \n Press F to forfeit (Only in extreme cases)", new Vector2(display.X + 50, display.Y + 50), Color.White);
            }

        }

        public void draw(SpriteBatch batch, Texture2D white, int timer, GameTime gameTime,SpriteFont font, SpriteFont smallFont)
        {
            if(stage != MapStage.End)
            {
                if (currentBattle == null)
                {
                    drawAll(batch, white, timer);
                }
                else
                {
                    currentBattle.Draw(white, gameTime);
                }
            } 
            else
            { 
                //credits
                batch.Draw(white, display, Color.Black * 0.8f);
                batch.DrawString(font, "A game By \n Lucas Ivy \n Nikhil Gurow \n Ravi Pothukanuri \n and Rushy Bikki",new Vector2(display.X + 200, display.Y + 100), Color.White);
                batch.DrawString(font, "Click the home button to exit", new Vector2(display.X + 200, display.Y + 700), Color.White);
            }
        }

        public static Color darken(Color c, double amount)
        {
            return new Color((int)(c.R * amount), (int)(c.G * amount), (int)(c.B * amount));

        }

        public void setDrawTiles()
        {
            drawingTiles = new List<DrawTile>();
            recsToDo.Clear();
            recsToDo.Add(new Rectangle(0, 0, 1024, 1024));
        }

        public void updateTerritoryDrawTiles(Territory territory)
        {

            for (int i = 0; i < drawingTiles.Count; i++)
            {
                if (drawingTiles[i].tile.territory == territory)
                    drawingTiles[i].updateColor(territory);
            }
        }

       

        public void oceanQuadTree(int x, int y, int size, List<Tile> quadedTiles, Territory ocean)
        {
            //Makes the water
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
                        if (grid[j, i] == null)

                            inside = true;
                        else
                            outside = true;
                        if (outside && inside)
                        {
                            oceanQuadTree(sqaure.X, sqaure.Y, size / 2, quadedTiles, ocean);
                            j = sqaure.Y + sqaure.Height;
                            i = sqaure.X + sqaure.Width;
                            break;
                        }
                    }
                }
                if (inside && !outside)
                    quadedTiles.Add(new Tile(sqaure.X, sqaure.Y, sqaure.Width, sqaure.Height, ocean, tileType.Unknown));
            }
        }
        public void quadTreeIterative(List<DrawTile> quadedTiles, int amount)
        {
            
            double scale = 1024.0 / (double)zoom.Width;

            List<Rectangle> recsToConsider = recsToDo;
            List<Rectangle> newToDo = new List<Rectangle>();


            if (recsToConsider[0].Width > 2)
            {
                for (int n = 0; n < Math.Min(amount, recsToConsider.Count); n++)
                {
                    Rectangle currentRec = recsToConsider[n];

                    int half = currentRec.Width / 2;
                    Rectangle[] sqaures = new Rectangle[4];
                    sqaures[0] = new Rectangle(currentRec.X, currentRec.Y, half, half); //topLeft
                    sqaures[1] = new Rectangle(currentRec.X + half, currentRec.Y, half, half); // topRight
                    sqaures[3] = new Rectangle(currentRec.X + half, currentRec.Y + half, half, half); // bottomRight
                    sqaures[2] = new Rectangle(currentRec.X, currentRec.Y + half, half, half); // bottomLeft

                    List<Territory> territoriesToConsider = new List<Territory>();
                    for (int i = 0; i < territories.Count; i++)
                    {
                        Territory cTer = territories[i];
                        Rectangle cBounds = new Rectangle((int)((cTer.bounds.X - zoom.X) * scale), (int)((cTer.bounds.Y - zoom.Y) * scale), (int)(cTer.bounds.Width * scale), (int)(cTer.bounds.Height * scale));
                        if (cBounds.Intersects(currentRec))
                            territoriesToConsider.Add(cTer);
                    }



                    for (int r = 0; r < 4; r++)
                    {
                        Rectangle sqaure = sqaures[r];
                        List<Territory> territoriesInsideSquare = new List<Territory>();
                        Boolean intersectsPerimeter = false;
                        for (int i = 0; i < territoriesToConsider.Count; i++)
                        {
                            if (i == -1)
                                break;

                            for (int w = 0; w < water.Count; w++)
                            {
                                if (new Rectangle((int)((water[w].y - zoom.X) * scale), (int)((water[w].x - zoom.Y) * scale), (int)(water[w].width * scale), (int)(water[w].height * scale)).Intersects(sqaure))
                                    intersectsPerimeter = true;
                            }


                            for (int j = 0; j < territoriesToConsider[i].tiles.Count; j++)
                            {
                                Tile cTile = territoriesToConsider[i].tiles[j];
                                if (new Rectangle((int)((cTile.y - zoom.X) * scale), (int)((cTile.x - zoom.Y) * scale), (int)(cTile.width * scale + 1), (int)(cTile.height * scale + 1)).Intersects(sqaure))
                                {
                                    if (cTile.territory != null)
                                        territoriesInsideSquare.Add(cTile.territory);
                                    j = territoriesToConsider[i].tiles.Count;

                                    if (territoriesInsideSquare.Count >= 2 || (territoriesInsideSquare.Count == 1 && intersectsPerimeter))
                                    {
                                        newToDo.Add(sqaure);
                                        i = -2;
                                        break;
                                    }
                                }
                            }
                        }
                        if (territoriesInsideSquare.Count == 1 && !intersectsPerimeter)
                            quadedTiles.Add(new DrawTile(new Tile(sqaure.X, sqaure.Y, sqaure.Width, sqaure.Height, territoriesInsideSquare[0], tileType.Unknown)));
                    }
                }
                recsToDo.RemoveRange(0, Math.Min(amount, recsToConsider.Count));
                recsToDo.AddRange(newToDo);
            }
            else
            {
                recsToDo.Clear();
            }

        }

        public List<Tile> shuffleTiles(List<Tile> tiles)
        {
            List<Tile> newTiles = new List<Tile>();
            List<Tile> oldTiles = tiles;
            int capacity = tiles.Count;
            for (int i = 0; i < capacity; i++)
            {
                int num = rand.Next(0, oldTiles.Count);
                newTiles.Add(oldTiles[num]);
                tiles.RemoveAt(num);
            }
            return newTiles;
        }

        public void gamble(int[] prices, int price, double discount)
        {
            // I LOVE GAMBLING  I LOVE GAMBLING
            if (currentNation.money >= price && currentNation.actionPoints > 0)
            {
                List<Territory> barrackTerritories = new List<Territory>();
                for (int i = 0; i < currentNation.territories.Count; i++)
                {
                    if (currentNation.territories[i].barracks)
                        barrackTerritories.Add(currentNation.territories[i]);
                }

                if (barrackTerritories.Count != 0)
                {

                    Territory cTer = barrackTerritories[rand.Next(0, barrackTerritories.Count)];
                    currentNation.money -= price;
                    for (int i = 0; i < 5; i++)
                    {
                        sliders[i].max = (int)Math.Floor((double)currentNation.money / (double)prices[i]);
                        sliders[i].at = 0;
                    }

                    price += (int)(price * discount);

                    int[] order = new int[5];

                    for (int i = 0; i < 4; i++)
                    {
                        int j = rand.Next(i, 5);
                        order[i] = j;
                    }

                    while (price > 20)
                    {
                        for (int j = 0; j < 5; j++)
                        {
                            int i = order[j];
                            int amount = rand.Next(0, (int)(price) + 1) + 1;
                            price -= (int)((double)amount / prices[i]) * prices[i];
                            cTer.troops[i] += (int)((double)amount / prices[i]);
                        }
                    }

                    
                    currentNation.actionPoints--;
                    Sounds.playSound("good");
                } else
                {
                    Sounds.playSound("error");
                }
            }
            else
            {
                Sounds.playSound("error");
            }
        }

        public void purchaseTroops(int[] troops, int[] prices)
        {
            int amount = 0;
            if (selectedTerritories.Count > 0)
            {
                if (selectedTerritories[0].barracks && troops.Sum() > 0)
                {
                    for (int i = 0; i < 5; i++)
                        amount += troops[i] * prices[i];


                    if (currentNation.money >= amount && currentNation.actionPoints > 0)
                    {
                        for (int i = 0; i < 5; i++)
                            selectedTerritories[0].troops[i] += troops[i];
                        currentNation.money -= amount;
                        currentNation.actionPoints--;
                        for (int i = 0; i < 5; i++)
                        {
                            sliders[i].max = (int)Math.Floor((double)currentNation.money / (double)prices[i]);
                            sliders[i].at = 0;
                        }
                        Sounds.playSound("good");
                    }
                    else
                    {
                        Sounds.playSound("error");
                    }
                }
                else
                {
                    Sounds.playSound("error");
                }
            }
            else
            {
                Sounds.playSound("error");
            }
        }

        public void purchaseActionPoint(int price)
        {
            if (currentNation.money >= price)
            {
                currentNation.actionPoints++;
                currentNation.money -= price;
                for (int i = 0; i < 5; i++)
                {
                    sliders[i].max = (int)Math.Floor((double)currentNation.money / (double)prices[i]);
                    sliders[i].at = 0;
                }
                Sounds.playSound("good");
            }
            else
            {
                Sounds.playSound("error");
            }
        }

        public void purchaseBarracks(int price)
        {

            if (selectedTerritories.Count > 0)
            {
                if (currentNation.money >= price && !selectedTerritories[0].barracks && currentNation.actionPoints > 0)
                {
                    selectedTerritories[0].barracks = true;
                    currentNation.money -= price;
                    currentNation.actionPoints--;
                    for (int i = 0; i < 5; i++)
                    {
                        sliders[i].max = (int)Math.Floor((double)currentNation.money / (double)prices[i]);
                        sliders[i].at = 0;
                    }
                    Sounds.playSound("good");
                }
                else
                {
                    Sounds.playSound("error");
                }
            } else
            {
                Sounds.playSound("error");
            }
        }


    }

    class DrawTile
    {
        // FOR SMOOTH COLORS AND NOT OVERLAPING UGLY ONES BECAUSE OF THE ZOOM
        public int x;
        public int y;
        public int width;
        public int height;

        public Color color;

        public float opacity;
        public Tile tile;

        public DrawTile(Tile tile)
        {
            this.tile = tile;
            x = tile.x;
            y = tile.y;
            width = tile.width;
            height = tile.height;
            updateColor(tile.territory);
        }

        public void updateColor(Territory territory)
        {

            tile.territory = territory;
            if (territory.hover && !territory.selected && !territory.transparent)
            {
                opacity = 0.2f;
            }
            if (!territory.hover && !territory.selected && !territory.transparent)
            {
                opacity = 0.1f;
            }
            if (territory.hover && territory.transparent)
            {
                opacity = 0.05f;
            }
            if (territory.selected)
            {
                opacity = 0.3f;
            }


            color = territory.getColor();
        }


    }

}





