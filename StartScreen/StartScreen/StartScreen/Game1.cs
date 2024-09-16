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
using System.IO;


namespace StartScreen
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        enum gameState
        { 
            Start, PlayerCount, ScrollingText, Game
        }
        enum PlayerTurn
        {
            One, Two, Three, Four
        }
        enum SideMenu
        {
            None, Settings, Home, Shop
        }
        
        SideMenu sideMenu = SideMenu.Settings;
        PlayerTurn turn = PlayerTurn.One;
        gameState game = gameState.Start;

        //Startup(Start Screen, PlayerCount Screen, Scrolling text)
        Texture2D StartScreen, PlayerCountScreen;
        Texture2D white;
        SpriteFont scrollingTextFont;
        Button start, quit;
        Texture2D MarsLogo;
        Button two, three, four;
        int PlayerCount;
        String[] openingText;
        Vector2 TextLoc;
        double y;
        double v;
        
        //misc
        int width, height;
        public Rectangle playScreen; //the inner screen where the game acc happens
        Rectangle fullScreen;
        public Texture2D blank;
        KeyboardState oldK;
        MouseState oldM;
        public Texture2D buttonUpTex, buttonDownTex;
        int timer;

        //popup stuff
        Popup popup;

        Button buyTroopsButton;

        Nation nation;

        //UI
        Rectangle TurnIndicator1, TurnIndicator2, TurnIndicator3, TurnIndicator4;
        Texture2D GameUI;

        //SideMenu
        Texture2D SettingsIcon, HomeIcon, ShopIcon, MoneyIcon;
        public SpriteFont menuFONT;
        int[] credits;
        Button Settings, Home, Shop;
        Button SoundsButton;
        SoundEffect music;
        SoundEffectInstance menuMusic;

        //shop
        Button[,] ShopButtons; // ROWS GO IN ORDER: RIFLEMAN, SCOUT, TANK, SNIPER, HEAVY

        Slider[] troopShopSliders;
        int[] prices;
        Button AddorSubract, Barracks, Gamble, ActionPoint;
        int gamblePrice;
        int BarrackPrice;
        int actionPointPrice;
        //FOR TESTING PURPOSES
        int[,] soldiers;
        List<string> names;

        Rectangle[] playerRecs;

        Rectangle[] turnRecs;

        Map map;

        Slider soundVolumeSlider = new Slider(new Rectangle(), 100, Color.White, "SFX");
        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            
            graphics.PreferredBackBufferWidth = 1920;
            graphics.PreferredBackBufferHeight = 1080;
            graphics.IsFullScreen = true;
            IsMouseVisible = true;
        }
        protected override void Initialize()
        {
            //Scrolling intro text and starting position
            prices = new int[5] { 20, 25, 50, 80, 200 };

            TextLoc = new Vector2(300, 1080);
            openingText = new String[] { "It is the Year 2069. Earth is running out of resources and", "humans are looking for a new planet to inhabit. Scientists", " find a quantum planet that has abundant resources and", "livable conditions. The biggest countries in the world have", " decided to make a move and have determined to make", " the planet theirs. These countries don't know that", " 'sharing is caring' and are wanting all of this planet to", " themselves. The countries go claim respective states", " in the planet they call their own, but tensions quickly", "rise and an all out interstellar war commences." };
            
            //Holds Y value of scrolling text as a double, in order to change the Y by less than 60 pixels/sec
            y = 1080;
            //intro text starting speed;
            v = .5;
            nation = null;
            gamblePrice = 500;
            BarrackPrice = 400;
            actionPointPrice = 50;

            //show whose turn it is, to make UI look nice
            TurnIndicator1 = new Rectangle(420, 20, 20, 75); 
            TurnIndicator2 = new Rectangle(420, 140, 20, 75);
            TurnIndicator3 = new Rectangle(420, 260, 20, 75);
            TurnIndicator4 = new Rectangle(420, 380, 20, 75);

            turnRecs = new Rectangle[4];
            turnRecs[0] = TurnIndicator1;
            turnRecs[1] = TurnIndicator2;
            turnRecs[2] = TurnIndicator3;
            turnRecs[3] = TurnIndicator4;

            
  



            playerRecs = new Rectangle[4];
            playerRecs[0] = new Rectangle(14, 16, 400, 100);
            playerRecs[1] = new Rectangle(14, 16 + 120, 400, 100);
            playerRecs[2] = new Rectangle(14, 16 + 120 * 2, 400, 100);
            playerRecs[3] = new Rectangle(14, 16 + 120 * 3, 400, 100);

            //holds each person's # of credits(currency)
            credits = new int[4] { 1000, 1000, 4000, 100 };

            //changes later when player selects playerCount, but this is default
            PlayerCount = 4;


            width = GraphicsDevice.Viewport.Width;
            height = GraphicsDevice.Viewport.Height;

            fullScreen = new Rectangle(0, 0, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height);
            oldK = Keyboard.GetState();
            oldM = Mouse.GetState();
            base.Initialize();

            timer = 0;
            //FOR TESTING PURPOSES: Row is Players and COL is # of each type of soldier
            soldiers = new int[4, 5];

        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
            blank = Content.Load<Texture2D>("white");
            StartScreen = Content.Load<Texture2D>("Background");
            PlayerCountScreen = Content.Load<Texture2D>("Background");
            scrollingTextFont = Content.Load<SpriteFont>("SpriteFont1");
            menuFONT = Content.Load<SpriteFont>("SpriteFont2");
            SettingsIcon = Content.Load<Texture2D>("System");
            // TODO: use this.Content to load your game content here
            GameUI = Content.Load<Texture2D>("Game UI");
            MarsLogo = Content.Load<Texture2D>("Mars Logo no bg");
            MoneyIcon = Content.Load<Texture2D>("Money");
            buttonUpTex = Content.Load<Texture2D>("ButtonUp");
            buttonDownTex = Content.Load<Texture2D>("ButtonDown");
            music = Content.Load<SoundEffect>("Gustav Holst Mars");
            ShopIcon = Content.Load<Texture2D>("homeIcon");
            HomeIcon = Content.Load<Texture2D>("shopIcon");
            white = Content.Load<Texture2D>("white");
            menuMusic = music.CreateInstance();
            menuMusic.Volume = 1;
            menuMusic.IsLooped = true;
            menuMusic.Play();
            //Startup

         

            start = new Button(buttonUpTex, buttonDownTex, new Rectangle(width/3, height/2+100, 250, 155), "Start", menuFONT, false);
            quit = new Button(buttonUpTex, buttonDownTex, new Rectangle(width /3 + 375, height/2+100, 250, 155), "Quit", menuFONT, false);
            two = new Button(buttonUpTex, buttonDownTex, new Rectangle(100, 100, 250, 155), "2P", menuFONT, false);
            three = new Button(buttonUpTex, buttonDownTex, new Rectangle(100, 300, 250, 155), "3P", menuFONT, false);
            four = new Button(buttonUpTex, buttonDownTex, new Rectangle(100, 500, 250, 155), "4P", menuFONT, false);

            // Sidemenu
            ShopButtons = new Button[5, 3];
            Shop = new Button(buttonUpTex, buttonDownTex, new Rectangle(width - 250, height - 120, 50, 50), "", menuFONT, false);
            Settings = new Button(buttonUpTex, buttonDownTex, new Rectangle(width - 175, height - 120, 50, 50), "", menuFONT, false);
            Home = new Button(buttonUpTex, buttonDownTex, new Rectangle(width - 100, height - 120, 50, 50), "", menuFONT, false);
            SoundsButton = new Button(buttonUpTex, buttonDownTex, new Rectangle(width - 250, height - 250, 200, 100), "Music On/Off", menuFONT, true);

            soundVolumeSlider = new Slider(new Rectangle(SoundsButton.rect.X - 30, SoundsButton.rect.Y - 50,200 ,30), 100, Color.White, "SFX");

            AddorSubract = new Button(buttonUpTex, buttonDownTex, new Rectangle(width - 300, height - 620, 50, 50), "+/-", menuFONT, false);
           
            //shop buttons setup

            troopShopSliders = new Slider[5];

            for(int i = 0; i < 5; i++)
            {
                switch(i)
                {
                    case 0:
                     troopShopSliders[i] = new Slider(new Rectangle(width - 300, height - 500 + (60 * i), 255, 30), 10, Color.Yellow,"Scout " + prices[i]);
                        break;
                    case 1:
                        troopShopSliders[i] = new Slider(new Rectangle(width - 300, height - 500 + (60 * i), 255, 30), 10, Color.Yellow, "Rifleman " + prices[i]);
                        break;
                    case 2:
                        troopShopSliders[i] = new Slider(new Rectangle(width - 300, height - 500 + (60 * i), 255, 30), 10, Color.Yellow, "Heavy " + prices[i]);
                        break;
                    case 3:
                        troopShopSliders[i] = new Slider(new Rectangle(width - 300, height - 500 + (60 * i), 255, 30), 10, Color.Yellow, "Sniper " + prices[i]);
                        break;
                    case 4:
                        troopShopSliders[i] = new Slider(new Rectangle(width - 300, height - 500 + (60 * i), 255, 30), 10, Color.Yellow, "Tank " + prices[i]);
                        break;
                }
            }

            Barracks = new Button(buttonUpTex, buttonDownTex, new Rectangle(width - 300, troopShopSliders[0].rec.Y - 50 - 10, 250, 50), "Buy Barrack (" + BarrackPrice + ")", menuFONT, false);
            Gamble = new Button(buttonUpTex, buttonDownTex, new Rectangle(width - 300, troopShopSliders[0].rec.Y - 120 - 10, 250, 50), "Gamble (" + gamblePrice + ")", menuFONT, false);

            ActionPoint = new Button(buttonUpTex, buttonDownTex, new Rectangle(width - 300, troopShopSliders[0].rec.Y - 190 - 10, 250, 50), "Buy Action Point (" + actionPointPrice + ")", menuFONT, false);
           


            buyTroopsButton = new Button(buttonUpTex, buttonDownTex, new Rectangle(troopShopSliders[4].rec.X, troopShopSliders[4].rec.Y + 50, 250, 50), "Purchase Troops", menuFONT, false);

            Slider.font = menuFONT;


            Sounds.addSound("click", Content.Load<SoundEffect>("Sounds/item-open").CreateInstance());
            Sounds.addSound("dismiss", Content.Load<SoundEffect>("Sounds/item-close").CreateInstance());
            Sounds.addSound("error", Content.Load<SoundEffect>("Sounds/saw-05").CreateInstance(),0.2f);
            Sounds.addSound("battleLost", Content.Load<SoundEffect>("Sounds/Crowd Aww Sound Effect").CreateInstance());
            Sounds.addSound("battleWon", Content.Load<SoundEffect>("Sounds/BattleWin").CreateInstance(), 0.2f);
            Sounds.addSound("GameWon", Content.Load<SoundEffect>("Sounds/GameWon").CreateInstance(),0.4f);
            Sounds.addSound("good", Content.Load<SoundEffect>("Sounds/vibraphone-26").CreateInstance(), 0.2f);

            //loads every type of map

            int level = new Random().Next(0,3);
            List<List<Texture2D>> levelAssets = new List<List<Texture2D>>();
            for (int i = 1; i <= 3; i++)
            {
                List<Texture2D> temp = new List<Texture2D>();
                temp.Add(Content.Load<Texture2D>($"Level{i}/Level_{i}"));
                temp.Add(Content.Load<Texture2D>($"Level{i}/Level_{i}_Hills"));
                temp.Add(Content.Load<Texture2D>($"Level{i}/Level_{i}_Island"));
                temp.Add(Content.Load<Texture2D>($"Level{i}/Level_{i}_Lakes"));
                temp.Add(Content.Load<Texture2D>($"Level{i}/Level_{i}_Mars"));
                temp.Add(Content.Load<Texture2D>($"Level{i}/Level_{i}_Mountains"));
                levelAssets.Add(temp);
            }

            width = GraphicsDevice.Viewport.Width;
            height = GraphicsDevice.Viewport.Height;

            playScreen = new Rectangle((int)(width * .235), height / 18, (int)(width * .532), height * 17 / 18);


            names = readNames();

            map = new Map(levelAssets[level].ElementAt(0), playScreen, troopShopSliders);
            Map.prices = prices;
            map.setUpMasks(levelAssets[level].ElementAt(2), levelAssets[level].ElementAt(5), levelAssets[level].ElementAt(3), levelAssets[level].ElementAt(4), levelAssets[level].ElementAt(1));

            Arrow.setUp(Content.Load<Texture2D>("triangle"));
            //[which map]


            //Popups
            Popup.screen = playScreen;
            Popup.blank = blank;
            Popup.font = menuFONT;
            Popup.up = buttonUpTex;
            Popup.down = buttonDownTex;
            //popup = new Popup(new int[5] { 10, 10, 10, 10, 5 });

            for (int i = 0; i < 3; i++)
            {
                string biomeName = "";
                switch (i)
                {
                    case 0:
                        biomeName = "Hill";
                        break;
                    case 1:
                        biomeName = "Mars";
                        break;
                    case 2:
                        biomeName = "Mountain";
                        break;
                }
                for (int j = 0; j < 3; j++)
                {
                    List<Vector2> meshPoints = readFiles(biomeName + (j + 1));
                    Battle.battlefields[i, j] = new Battlefield(meshPoints, Content.Load<Texture2D>("Battlefields/" + biomeName + (j + 1)), biomeName);
                }
            }

            //spawn zones

            //Hills1
            Rectangle attack = new Rectangle(0, 300, 100, 400);
            Rectangle defend = new Rectangle(GraphicsDevice.Viewport.Width - 100, 250, 100, 500);
            Battle.battlefields[0, 0].setSpawns(attack, defend);

            //Hills2
            attack = new Rectangle(0, 100, 100, 700);
            defend = new Rectangle(GraphicsDevice.Viewport.Width - 100, 100, 100, 400);
            Battle.battlefields[0, 1].setSpawns(attack, defend);

            //Hills3
            attack = new Rectangle(0, 400, 100, 300);
            defend = new Rectangle(GraphicsDevice.Viewport.Width - 100, 150, 100, 750);
            Battle.battlefields[0, 2].setSpawns(attack, defend);

            //Mars1
            attack = new Rectangle(0, 200, 100, 800);
            defend = new Rectangle(GraphicsDevice.Viewport.Width - 100, 500, 100, 400);
            Battle.battlefields[1, 0].setSpawns(attack, defend);

            //Mars2
            attack = new Rectangle(0, 550, 100, 400);
            defend = new Rectangle(GraphicsDevice.Viewport.Width - 100, 300, 100, 700);
            Battle.battlefields[1, 1].setSpawns(attack, defend);

            //Mars3
            attack = new Rectangle(0, 300, 100, 700);
            defend = new Rectangle(GraphicsDevice.Viewport.Width - 100, 50, 100, 850);
            Battle.battlefields[1, 2].setSpawns(attack, defend);

            //Mountains1
            attack = new Rectangle(0, GraphicsDevice.Viewport.Height / 2 - 150, 100, 300);
            defend = new Rectangle(GraphicsDevice.Viewport.Width - 100, GraphicsDevice.Viewport.Height / 2 - 150, 100, 300);
            Battle.battlefields[2, 0].setSpawns(attack, defend);

            //Mountains2
            attack = new Rectangle(0, GraphicsDevice.Viewport.Height / 2 - 150, 100, 300);
            defend = new Rectangle(GraphicsDevice.Viewport.Width - 100, GraphicsDevice.Viewport.Height / 2 - 150, 100, 300);
            Battle.battlefields[2, 1].setSpawns(attack, defend);

            //Mountains3
            attack = new Rectangle(0, GraphicsDevice.Viewport.Height / 2 - 150, 100, 300);
            defend = new Rectangle(GraphicsDevice.Viewport.Width - 100, GraphicsDevice.Viewport.Height / 2 - 150, 100, 300);
            Battle.battlefields[2, 2].setSpawns(attack, defend);

            Battle.soldierTexture = Content.Load<Texture2D>("Soldier");
            Battle.tankTexture = Content.Load<Texture2D>("Tank");
            Battle.sb = spriteBatch;
            Battle.gd = GraphicsDevice;
            Battle.font1 = menuFONT;


        }

        public List<Vector2> readFiles(string name)
        {
            string filePath = "Content/Battlefields/" + name + "-Impassable.txt";
            List<Vector2> points = new List<Vector2>();
            try
            {
                using (StreamReader reader = new StreamReader(filePath))
                {
                    string temp;
                    while ((temp = reader.ReadLine()) != null)
                    {
                        string[] parts = temp.Split('\t');
                        float x = float.Parse(parts[0]);
                        float y = float.Parse(parts[1]);

                        points.Add(new Vector2(x, y));
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            return points;
        }

        public List<String> readNames()
        {
            string filePath = "Content/TerritoryNames.txt";
            List<string> names1 = new List<string>();
            try
            {
                using (StreamReader reader = new StreamReader(filePath))
                {
                    string temp;
                    while ((temp = reader.ReadLine()) != null)
                    {
                        names1.Add(temp);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            return names1;
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }
        public void NextTurn()
        {
            if (turn == PlayerTurn.One)
                turn = PlayerTurn.Two;
            else if (turn == PlayerTurn.Two)
            {
                if (PlayerCount == 2)
                    turn = PlayerTurn.One;
                else
                    turn = PlayerTurn.Three;
            }
            else if (turn == PlayerTurn.Three)
            {
                if (PlayerCount == 3)
                    turn = PlayerTurn.One;
                else
                    turn = PlayerTurn.Four;
            }
            else if (turn == PlayerTurn.Four)
                turn = PlayerTurn.One;
        }
        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        
        protected override void Update(GameTime gameTime)
        {
            timer++;

            

            if (game == gameState.Start)
            {
                
                
                
                MouseState mouse = Mouse.GetState();
                KeyboardState kb = Keyboard.GetState();
                if (mouse.LeftButton == ButtonState.Pressed && oldM.LeftButton != ButtonState.Pressed)
                {
                    Rectangle temp = new Rectangle(mouse.X, mouse.Y, 1, 1);
                    if (temp.Intersects(start.rect))
                    {
                        if (start.selected)
                            game = gameState.PlayerCount;
                        else
                        {
                            start.select(); quit.deselect();
                        }

                    }
                    if (temp.Intersects(quit.rect))
                    {
                        if (quit.selected)
                            this.Exit();
                        else
                        {
                            quit.select(); start.deselect();
                        }

                    }
                }
                oldK = kb;
                oldM = mouse;
            }
            else if (game == gameState.PlayerCount)
            {
                MouseState mouse = Mouse.GetState();
                KeyboardState kb = Keyboard.GetState();
                if (mouse.LeftButton == ButtonState.Pressed && oldM.LeftButton != ButtonState.Pressed)
                {
                    Rectangle temp = new Rectangle(mouse.X, mouse.Y, 1, 1);
                    if (temp.Intersects(two.rect))
                    {
                        if (two.selected)
                            game = gameState.ScrollingText;
                        else
                        {
                            PlayerCount = 2;
                            two.select();
                            three.deselect();
                            four.deselect();
                        }
                    }
                    if (temp.Intersects(three.rect))
                    {
                        if (three.selected)
                            game = gameState.ScrollingText;
                        else
                        {
                            PlayerCount = 3;
                            two.deselect();
                            three.select();
                            four.deselect();
                        }
                    }
                    if (temp.Intersects(four.rect))
                    {
                        if (four.selected)
                            game = gameState.ScrollingText;
                        else
                        {
                            PlayerCount = 4;
                            two.deselect();
                            three.deselect();
                            four.select();
                        }
                    }

                }
                oldK = kb;
                oldM = mouse;
            }
            else if (game == gameState.ScrollingText)
            {
                KeyboardState kb = Keyboard.GetState();
                if (kb.IsKeyDown(Keys.Space))
                    v = 15;
                else
                    v = .5;
                y -= v;
                TextLoc.Y = (int)y;
                if (TextLoc.Y < -1200)
                {
                    //Lol
                    game = gameState.Game;
                    map.setUpNations(PlayerCount);
                    map.setUpTerritories(30, names);
                }
                oldK = kb;
            }
            else if (game == gameState.Game)
            {

                MouseState mouse = Mouse.GetState();
                KeyboardState kb = Keyboard.GetState();

                //VVV VERY IMPORTANT
                map.update(oldM, oldK,gameTime, timer);

                if(nation != map.currentNation)
                {
                    //for(int i = 0; i < 5; i++)
                    //{
                    //    troopShopSliders[i].max = (int)Math.Floor((double)map.currentNation.money / (double)prices[i]);
                    //    troopShopSliders[i].at = 0;
                    //    troopShopSliders[i].color = map.currentNation.color;
                    //}
                }

                for (int i = 0; i < troopShopSliders.Length; i++)
                {
                    int amount = 0;
                    for (int j = 0; j < 5; j++)
                    {
                        amount += (int)troopShopSliders[j].at * prices[j];
                    }
                 troopShopSliders[i].update(map.currentNation.money, amount, prices[i]);
                   
                }

                


                //popup.update(kb, mouse);


                //a few keys to test functionality of stuff
                if (kb.IsKeyDown(Keys.Space) && !oldK.IsKeyDown(Keys.Space)) //change between player turns
                    NextTurn();

                //if (kb.IsKeyDown(Keys.Escape)) // escape the program
                //    this.Exit();
                           
                if (mouse.LeftButton == ButtonState.Pressed && oldM.LeftButton != ButtonState.Pressed)
                {
                    Rectangle temp = new Rectangle(mouse.X, mouse.Y, 1, 1);
                    //double click to select button

                    //selecting sidemenu bar
                    if (temp.Intersects(Settings.rect))
                    {
                        if (Settings.selected)
                            sideMenu = SideMenu.Settings;
                        else
                        {
                            Settings.select();
                            Shop.deselect();
                            Home.deselect();
                        }
                    }
                    if (temp.Intersects(Home.rect))
                    {
                        if (sideMenu == SideMenu.Home)
                            game = gameState.Start;
                        if (Home.selected)
                            sideMenu = SideMenu.Home;
                        else
                        {
                            Settings.deselect();
                            Shop.deselect();
                            Home.select();
                        }
                    }
                    if (temp.Intersects(Shop.rect))
                    {
                        if (Shop.selected)
                            sideMenu = SideMenu.Shop;
                        else
                        {
                            Settings.deselect();
                            Shop.select();
                            Home.deselect();
                        }
                    }
                    
                    //settings here
                    if (sideMenu == SideMenu.Settings && mouse.LeftButton == ButtonState.Pressed && oldM.LeftButton != ButtonState.Pressed)
                    {
                        soundVolumeSlider.update();
                        Sounds.volume = (float)soundVolumeSlider.at;
                        if (temp.Intersects(SoundsButton.rect))
                        {
                            if (SoundsButton.selected)
                            {
                                SoundsButton.deselect();
                                menuMusic.Volume = 0;
                            }
                            else
                            {
                                SoundsButton.select();
                                menuMusic.Volume = 1;

                            }

                        }
                    }

                   

                    //shop logic here
                    if (sideMenu == SideMenu.Shop)
                    {
                        if (temp.Intersects(buyTroopsButton.rect))
                        {
                            if (buyTroopsButton.selected)
                            {
                                buyTroopsButton.deselect();
                                int[] troops = new int[5];

                                for(int i = 0; i < 5; i++)
                                {
                                    troops[i] += (int)troopShopSliders[i].at;
                                    troopShopSliders[i].at = 0;
                                }
                                map.purchaseTroops(troops, prices);
                                //LUCAS ADD CODE HERE FOR WHAT THE BUTTON SHOULD DO
                            }
                            else
                            {
                                buyTroopsButton.select();
                            }
                        }
                
                        if (temp.Intersects(Barracks.rect))
                        {
                            if (Barracks.selected)
                            {
                                Barracks.deselect();
                                //LUCAS ADD CODE HERE FOR WHAT THE BUTTON SHOULD DO
                                map.purchaseBarracks(BarrackPrice);
                            }
                            else
                            {
                                Barracks.select();
                            }
                        }
                        if (temp.Intersects(Gamble.rect))
                        {
                            if (Gamble.selected)
                            {
                                Gamble.deselect();
                                map.gamble(prices,gamblePrice,0.1);
                                //LUCAS ADD CODE HERE FOR WHAT THE BUTTON SHOULD DO
                            }
                            else
                            {
                                Gamble.select();
                            }
                        }
                        if (temp.Intersects(ActionPoint.rect))
                        {
                            if (ActionPoint.selected)
                            {
                                ActionPoint.deselect();
                                //map.gamble(prices, gamblePrice, 0.1);
                                map.purchaseActionPoint(actionPointPrice);
                                //LUCAS ADD CODE HERE FOR WHAT THE BUTTON SHOULD DO
                            }
                            else
                            {
                                ActionPoint.select();
                            }
                        }
                        
                            
                        

                    }
                    if (map.currentNation != null)
                        nation = map.currentNation;
                }
                if (sideMenu == SideMenu.Settings)
                {
                    soundVolumeSlider.update(true);
                    Sounds.volume = (float)soundVolumeSlider.at/100f;
                }

                oldK = kb;
                oldM = mouse;
            }
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();


            base.Update(gameTime);
        }
        public void DrawNames(SpriteBatch sb)
        {
            if (PlayerCount == 2)
            {
                //sb.Draw(blank, NameTag1, Color.PeachPuff);
                //sb.Draw(blank, NameTag2, Color.PeachPuff);
                if (turn == PlayerTurn.One)
                {
                    sb.Draw(blank, TurnIndicator1, Color.Green);
                    sb.Draw(blank, TurnIndicator2, Color.Gray);
                }
                if (turn == PlayerTurn.Two)
                {
                    sb.Draw(blank, TurnIndicator1, Color.Gray);
                    sb.Draw(blank, TurnIndicator2, Color.Green);
                }

            }
            if (PlayerCount == 3)
            {
                //sb.Draw(blank, NameTag1, Color.PeachPuff);
                //sb.Draw(blank, NameTag2, Color.PeachPuff);
                //sb.Draw(blank, NameTag3, Color.PeachPuff);
                if (turn == PlayerTurn.One)
                {
                    sb.Draw(blank, TurnIndicator1, Color.Green);
                    sb.Draw(blank, TurnIndicator2, Color.Gray);
                    sb.Draw(blank, TurnIndicator3, Color.Gray);
                }
                if (turn == PlayerTurn.Two)
                {
                    sb.Draw(blank, TurnIndicator1, Color.Gray);
                    sb.Draw(blank, TurnIndicator2, Color.Green);
                    sb.Draw(blank, TurnIndicator3, Color.Gray);
                }
                if (turn == PlayerTurn.Three)
                {
                    sb.Draw(blank, TurnIndicator1, Color.Gray);
                    sb.Draw(blank, TurnIndicator2, Color.Gray);
                    sb.Draw(blank, TurnIndicator3, Color.Green);
                }
            }
            if (PlayerCount == 4)
            {
                //sb.Draw(blank, NameTag1, Color.PeachPuff);
                //sb.Draw(blank, NameTag2, Color.PeachPuff);
                //sb.Draw(blank, NameTag3, Color.PeachPuff);
                //sb.Draw(blank, NameTag4, Color.PeachPuff);
                if (turn == PlayerTurn.One)
                {
                    sb.Draw(blank, TurnIndicator1, Color.Green);
                    sb.Draw(blank, TurnIndicator2, Color.Gray);
                    sb.Draw(blank, TurnIndicator3, Color.Gray);
                    sb.Draw(blank, TurnIndicator4, Color.Gray);
                }
                if (turn == PlayerTurn.Two)
                {
                    sb.Draw(blank, TurnIndicator1, Color.Gray);
                    sb.Draw(blank, TurnIndicator2, Color.Green);
                    sb.Draw(blank, TurnIndicator3, Color.Gray);
                    sb.Draw(blank, TurnIndicator4, Color.Gray);
                }
                if (turn == PlayerTurn.Three)
                {
                    sb.Draw(blank, TurnIndicator1, Color.Gray);
                    sb.Draw(blank, TurnIndicator2, Color.Gray);
                    sb.Draw(blank, TurnIndicator3, Color.Green);
                    sb.Draw(blank, TurnIndicator4, Color.Gray);
                }
                if (turn == PlayerTurn.Four)
                {
                    sb.Draw(blank, TurnIndicator1, Color.Gray);
                    sb.Draw(blank, TurnIndicator2, Color.Gray);
                    sb.Draw(blank, TurnIndicator3, Color.Gray);
                    sb.Draw(blank, TurnIndicator4, Color.Green);
                }
            }
            
        }
        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);
            spriteBatch.Begin();

            if (game == gameState.Start)
            {
                spriteBatch.Draw(StartScreen, fullScreen, Color.White);
                start.Draw(spriteBatch);
                quit.Draw(spriteBatch);
                spriteBatch.Draw(MarsLogo, new Rectangle(width/2 - 225, 150, 450, 450), Color.White);

            }
            else if (game == gameState.PlayerCount)
            {
                spriteBatch.Draw(PlayerCountScreen, fullScreen, Color.White);
                two.Draw(spriteBatch); three.Draw(spriteBatch); four.Draw(spriteBatch);
            }
            else if (game == gameState.ScrollingText)
            {

                for (int i = 0; i < openingText.Length; i++)
                    spriteBatch.DrawString(scrollingTextFont, openingText[i], new Vector2(TextLoc.X, TextLoc.Y + (100 * i)), Color.White);
            }
            else if (game == gameState.Game)
            {
                
                spriteBatch.DrawString(scrollingTextFont, "MARS", new Vector2(120, 750), Color.DarkBlue);
                map.draw(spriteBatch, white, timer, gameTime,scrollingTextFont,menuFONT);


                map.drawCursor(spriteBatch, white);
                map.drawPopup(spriteBatch);
                map.drawInstructions(spriteBatch, white, menuFONT);

                if (map.currentBattle == null)
                {

                    
                    spriteBatch.Draw(GameUI, fullScreen, Color.White);

                    if (map.gracePeriod)
                    {
                        spriteBatch.DrawString(menuFONT, "Grace Period", new Vector2(300, height - 50), Color.White);
                    }

                    Home.Draw(spriteBatch); Settings.Draw(spriteBatch); Shop.Draw(spriteBatch);
                    //drawing icons;
                    spriteBatch.Draw(SettingsIcon, new Rectangle(Settings.rect.X + 5, Settings.rect.Y + 5, Settings.rect.Width - 10, Settings.rect.Height - 15), Color.White);
                    spriteBatch.Draw(ShopIcon, new Rectangle(Shop.rect.X - 2, Shop.rect.Y - 3, Shop.rect.Width, Shop.rect.Height), Color.White);
                    spriteBatch.Draw(HomeIcon, new Rectangle(Home.rect.X, Home.rect.Y, Home.rect.Width, Home.rect.Height), Color.White);
                    //PlayerMoney


                    //playerRecs[0];

                    if (map.currentNation != null)
                    {
                        for (int i = 0; i < map.nations.Count; i++)
                        {
                            float opacity = 1;
                            if (map.currentNation != map.nations[i])
                                opacity = 0.5f;

                            //spriteBatch.Draw(white, playerRecs[map.turn], map.currentNation.color);
                            spriteBatch.DrawString(menuFONT, "PLAYER ", new Vector2(playerRecs[i].X + 2, playerRecs[i].Y + 2), Color.White * opacity);
                            spriteBatch.DrawString(menuFONT, "" + (i + 1), new Vector2(playerRecs[i].X + 100, playerRecs[i].Y + 2), map.nations[i].color * opacity);

                            spriteBatch.DrawString(menuFONT, "Money " + map.nations[i].money, new Vector2(playerRecs[i].X + 2, playerRecs[i].Y + 26), Color.White * opacity);
                            spriteBatch.DrawString(menuFONT, "Income " + map.nations[i].getIncome(), new Vector2(playerRecs[i].X + 2, playerRecs[i].Y + 50), Color.White * opacity);
                            spriteBatch.DrawString(menuFONT, "Action Points " + map.nations[i].actionPoints, new Vector2(playerRecs[i].X + 2, playerRecs[i].Y + 74), Color.White * opacity);


                            spriteBatch.DrawString(menuFONT, ((int)((double)(map.nations[i].territories.Count * 100) / map.territories.Count)) + "%", new Vector2(playerRecs[i].X + 170, playerRecs[i].Y + 30), Color.White * opacity);
                            spriteBatch.DrawString(menuFONT, "Owned", new Vector2(playerRecs[i].X + 150, playerRecs[i].Y + 50), Color.White * opacity);

                            spriteBatch.DrawString(menuFONT, "" + map.nations[i].getTroopPower()/5, new Vector2(playerRecs[i].X + 300, playerRecs[i].Y + 30), Color.White * opacity);
                            spriteBatch.DrawString(menuFONT, "Troop Power", new Vector2(playerRecs[i].X + 250, playerRecs[i].Y + 50), Color.White * opacity);

                            if (map.currentNation == map.nations[i])
                            {
                                spriteBatch.Draw(white, turnRecs[i], map.currentNation.color);
                            }
                        }
                    }


                    if (map.hoveringTerritory != null)
                    {
                        spriteBatch.DrawString(menuFONT, map.hoveringTerritory.name + " has", new Vector2(width - 400, 30), Color.White);

                        //spriteBatch.Draw(MoneyIcon, new Rectangle(width - 260, 100, 30, 30), Color.White);
                        spriteBatch.DrawString(menuFONT, map.hoveringTerritory.getIncome() + " Income", new Vector2(width - 400, 60), Color.White);

                        int j = 0;
                        for (int i = 0; i < 5; i++)
                        {
                            if (map.hoveringTerritory.troops[i] > 0)
                            {
                                j++;
                                switch (i)
                                {
                                    case 0:
                                        spriteBatch.DrawString(menuFONT, map.hoveringTerritory.troops[i] + " Scouts", new Vector2(width - 400, 90 + (j * 30)), Color.White);
                                        break;
                                    case 1:
                                        spriteBatch.DrawString(menuFONT, map.hoveringTerritory.troops[i] + " Riflemen", new Vector2(width - 400, 90 + (j * 30)), Color.White);
                                        break;
                                    case 2:
                                        spriteBatch.DrawString(menuFONT, map.hoveringTerritory.troops[i] + " Heavys", new Vector2(width - 400, 90 + (j * 30)), Color.White);
                                        break;
                                    case 3:
                                        spriteBatch.DrawString(menuFONT, map.hoveringTerritory.troops[i] + " Snipers", new Vector2(width - 400, 90 + (j * 30)), Color.White);
                                        break;
                                    case 4:
                                        spriteBatch.DrawString(menuFONT, map.hoveringTerritory.troops[i] + " Tanks", new Vector2(width - 400, 90 + (j * 30)), Color.White);
                                        break;
                                }
                            }
                        }
                        j += 2;
                        spriteBatch.DrawString(menuFONT, (map.hoveringTerritory.getTroopPower()/5) + " Total troop power", new Vector2(width - 400, 90 + (j * 30)), Color.White);

                    }


                    //SideMenu draws
                    if (sideMenu == SideMenu.Settings)
                    {
                        spriteBatch.DrawString(menuFONT, "SETTINGS", new Vector2(1520, 400), Color.White);
                        SoundsButton.Draw(spriteBatch);
                        soundVolumeSlider.draw(spriteBatch, white);

                    }
                    if (sideMenu == SideMenu.Shop)
                    {

                        for (int i = 0; i < troopShopSliders.Length; i++)
                            troopShopSliders[i].draw(spriteBatch, white);

                        int amount = 0;
                        for (int i = 0; i < troopShopSliders.Length; i++)
                        {
                            amount += (int)troopShopSliders[i].at * prices[i];
                        }
                        buyTroopsButton.words = "Purchase Troops (" + amount + ")";
                        buyTroopsButton.Draw(spriteBatch);


                        //spriteBatch.DrawString(menuFONT, "" + (map.currentNation.money - amount), new Vector2(troopShopSliders[4].rec.X, troopShopSliders[4].rec.Y + 30), Color.White);

                        if (map.currentNation != null)
                            spriteBatch.DrawString(menuFONT, "SHOP " + map.currentNation.money, new Vector2(1520, 400), Color.White);


                        Barracks.Draw(spriteBatch);
                        Gamble.Draw(spriteBatch);
                        ActionPoint.Draw(spriteBatch);


                    }
                    if (sideMenu == SideMenu.Home)
                    {
                        spriteBatch.DrawString(menuFONT, "ARE YOU SURE? \n GOING TO HOME WILL \n RESET ALL PROGRESS", new Vector2(1520, 400), Color.White);
                    }

                    map.drawCursor(spriteBatch, white, true);
                }
                

                //if (turn == PlayerTurn.One)
                //{
                //    spriteBatch.DrawString(menuFONT, "" + credits[0], new Vector2(width - 150, 75), Color.White);
                //}
                //if (turn == PlayerTurn.Two)
                //{
                //    spriteBatch.DrawString(menuFONT, "" + credits[1], new Vector2(width - 150, 75), Color.White);
                //}
                //if (turn == PlayerTurn.Three)
                //{
                //    spriteBatch.DrawString(menuFONT, "" + credits[2], new Vector2(width - 150, 75), Color.White);
                //}
                //if (turn == PlayerTurn.Four)
                //{
                //    spriteBatch.DrawString(menuFONT, "" + credits[3], new Vector2(width - 150, 75), Color.White);
                //}





                //popups
                //popup.draw(spriteBatch);
            }




            spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}
