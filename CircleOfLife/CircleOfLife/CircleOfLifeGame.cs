using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Nuclex.UserInterface;
using Nuclex.Input;
using Nuclex.Game;


namespace CircleOfLife
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class CircleOfLifeGame : Microsoft.Xna.Framework.Game
    {
        public GraphicsDeviceManager graphics;
        public SpriteBatch spriteBatch;
        
        //graphics fields
        public Texture2D spriteSheet;

        public Nuclex.UserInterface.Visuals.Flat.FlatGuiVisualizer UIVisualizer;

        public GameFonts gameFonts;

        public Ecosystem ecosystem;
        public User user;

        //Sounds
        Song backgroundMusic;
        bool backgroundStart = false;

        //Map coordinates: these variables should be moved to a more appropriate class..eventually..perhaps
        public int mapSizeX;
        public int mapSizeY;
        public int screenSizeX;
        public int screenSizeY;

        //temporary variable for implementing map scrolling
        public Vector3 userView;
        public bool scrollLeft = false;
        public bool scrollRight = false;
        public bool scrollDown = false;
        public bool scrollUp = false;

        public bool menuOpen = false;
        public bool dialogOpen = false;
        public bool createOpen = false;
        public bool startOpen = true;
        public bool addingSpecies = false;

        public Species newSpecies;

        //Points!!
        public int oPoints = 200;
        public int ePoints;

        //Real radius
        public int realRadius;

        public CircleOfLifeGame()
        {
            graphics = new GraphicsDeviceManager(this);
          
            //set resolution -- this stuff gets changed??
            graphics.PreferredBackBufferWidth = 1440;
            graphics.PreferredBackBufferHeight = 960;
            screenSizeX = 1440;
            screenSizeY = 960;
            //``

            graphics.IsFullScreen = false; // just for debuggin

            realRadius = 10000; //20k x 20k..lots to work with :)
            //map size is initially twice screen size
            mapSizeX = 2000;
            mapSizeY = 2000;
            //initialize
            userView = new Vector3(-mapSizeX / 4, -mapSizeY /4,1.0f);

            

            Content.RootDirectory = "Content";
            IsMouseVisible = false;
        }

        /// <summary>
        /// </summary>
        protected override void Initialize()
        {
            //Initialize ecosystem
            ecosystem = new Ecosystem(this, mapSizeX, mapSizeY);
            //Initialize user interface system
            user = new User(this, ecosystem);
            base.Initialize();
        }


        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            spriteSheet = Content.Load<Texture2D>("spriteSheet");

            gameFonts.Default = Content.Load<SpriteFont>("BaseFont");
            gameFonts.Header = Content.Load<SpriteFont>("HeaderFont");
            gameFonts.Title = Content.Load<SpriteFont>("TitleFont");

            backgroundMusic = Content.Load<Song>("turtlewoods");
            
            MediaPlayer.IsRepeating = true;

           // UIVisualizer = Nuclex.UserInterface.Visuals.Flat.FlatGuiVisualizer.FromFile(Services, "..\\..\\..\\..\\CircleOfLifeContent\\Skin\\EntropyUISheet.xml");
            //these intialization functions must occur after sprite sheet has been loaded
            user.initializeGameScreen();
            user.initializeMenuScreen();
            user.initializeCreateScreen();

        }

        void initializeGameComponents()
        {
        }
        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            if (menuOpen || createOpen || startOpen)
            {

            }
            else
            {
                ecosystem.Update(gameTime);

                //Navigation scrolling section
                //the values need to be tuned to make scrolling smooth
                if (scrollLeft && userView.X < 100)
                    userView.X += 7.0f * (gameTime.ElapsedGameTime.Ticks / 100000.0f);
                if (scrollRight && userView.X > -mapSizeX + graphics.PreferredBackBufferWidth)
                    userView.X -= 7.0f * (gameTime.ElapsedGameTime.Ticks / 100000.0f);
                if (scrollUp && userView.Y < 100)
                    userView.Y += 7.0f * (gameTime.ElapsedGameTime.Ticks / 100000.0f);
                if (scrollDown && userView.Y > -mapSizeY + graphics.PreferredBackBufferHeight)
                    userView.Y -= 7.0f * (gameTime.ElapsedGameTime.Ticks / 100000.0f);
            }

            //Music stuff
            if (!backgroundStart)
            {
               // MediaPlayer.Play(backgroundMusic);
                backgroundStart = true;
            }  

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend);
            if (startOpen)
            {
                //draw menu background
                user.drawStart(gameTime, spriteBatch, spriteSheet, gameFonts);
            }
            else if (createOpen)
            {
                //draw menu background
                user.drawCreate(gameTime, spriteBatch, spriteSheet, gameFonts);
            }
            else if (menuOpen)
            {
                //draw menu background
                user.drawMenu(gameTime, spriteBatch, spriteSheet, gameFonts);
            }
            else
            {
                ecosystem.draw(gameTime, spriteBatch, spriteSheet, userView);
                spriteBatch.Draw(spriteSheet, new Rectangle((int)(userView.X * userView.Z), (int)(userView.Y * userView.Z ), (int)((float)mapSizeX * userView.Z), (int)((float)mapSizeY * userView.Z)), new Rectangle(0, 0, 999, 999), Color.White, 0.0f, Vector2.Zero, SpriteEffects.None, 0.9f);
                spriteBatch.Draw(spriteSheet, new Rectangle((int)(userView.X * userView.Z), (int)(userView.Y * userView.Z), (int)((float)mapSizeX * userView.Z), (int)((float)mapSizeY * userView.Z)), new Rectangle(0, 1050, 1, 1), Color.White, 0.0f, Vector2.Zero, SpriteEffects.None, 1.0f);
                //spriteBatch.Draw(spriteSheet, new Rectangle((int)userView.X, (int)userView.Y, mapSizeX , mapSizeY ), new Rectangle(0, 0, 1000, 1000), Color.White, 0.0f, Vector2.Zero, SpriteEffects.None, 0.1f);
                //spriteBatch.Draw(spriteSheet, new Rectangle(400, 400, 200, 200), new Rectangle(1000, 0, 300, 300),Color.White);
                user.drawHUD(gameTime, spriteBatch, spriteSheet, gameFonts);
            }


            spriteBatch.End();

            base.Draw(gameTime);
            spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend);
            user.drawMouse(spriteBatch, spriteSheet);
            spriteBatch.End();
        }


        public struct GameFonts
        {
            public SpriteFont Default;
            public SpriteFont Header;
            public SpriteFont Title;
        }

        public Vector2 realToRelative(Vector2 realPosition)
        {
            Vector2 result = new Vector2((realPosition.X + userView.X) * userView.Z, (realPosition.Y + userView.Y) * userView.Z);
            return result;
        }
    }
}

