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

        Ecosystem ecosystem;
        User user;


        //Map coordinates: these variables should be moved to a more appropriate class..eventually..perhaps
        public int mapSizeX;    
        public int mapSizeY;

        //temporary variable for implementing map scrolling
        public Vector2 userView;
        public bool scrollLeft = false;
        public bool scrollRight = false;
        public bool scrollDown = false;
        public bool scrollUp = false;

        public bool menuOpen = false;
        public bool dialogOpen = false;

        public CircleOfLifeGame()
        {
            graphics = new GraphicsDeviceManager(this);
          
            //set resolution
            graphics.PreferredBackBufferWidth = 1440;
            graphics.PreferredBackBufferHeight = 960;
            //map size is initially twice screen size
            mapSizeX = 1920;
            mapSizeY = 1920;
            graphics.IsFullScreen = false;
            //initialize
            userView = new Vector2(-mapSizeX / 4, -mapSizeY /4);

            Content.RootDirectory = "Content";
            IsMouseVisible = true;
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

           // UIVisualizer = Nuclex.UserInterface.Visuals.Flat.FlatGuiVisualizer.FromFile(Services, "..\\..\\..\\..\\CircleOfLifeContent\\Skin\\EntropyUISheet.xml");
            //these intialization functions must occur after sprite sheet has been loaded
            user.initializeGameScreen();
            user.initializeMenuScreen();

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
            if (menuOpen)
            {

            }
            else
            {
                ecosystem.Update(gameTime);

                //Navigation scrolling section
                //the values need to be tuned to make scrolling smooth
                if (scrollLeft && userView.X < 0)
                    userView.X += 7.0f * (gameTime.ElapsedGameTime.Ticks / 100000.0f);
                if (scrollRight && userView.X > -mapSizeX + graphics.PreferredBackBufferWidth)
                    userView.X -= 7.0f * (gameTime.ElapsedGameTime.Ticks / 100000.0f);
                if (scrollUp && userView.Y < 0)
                    userView.Y += 7.0f * (gameTime.ElapsedGameTime.Ticks / 100000.0f);
                if (scrollDown && userView.Y > -mapSizeY + graphics.PreferredBackBufferHeight)
                    userView.Y -= 7.0f * (gameTime.ElapsedGameTime.Ticks / 100000.0f);
            }

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.White);

            spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend);

            if (menuOpen)
            {
                //draw menu background
                user.drawMenu(gameTime, spriteBatch, spriteSheet, gameFonts);
            }
            else
            {
                ecosystem.draw(gameTime, spriteBatch, spriteSheet, userView);
                spriteBatch.Draw(spriteSheet, new Rectangle((int)userView.X, (int)userView.Y, mapSizeX, mapSizeY), new Rectangle(0, 0, 1000, 1000), Color.White, 0.0f, Vector2.Zero, SpriteEffects.None, 0.1f);
                user.drawHUD(gameTime, spriteBatch, spriteSheet, gameFonts);
            }


            spriteBatch.End();

            base.Draw(gameTime);
        }


        public struct GameFonts
        {
            public SpriteFont Default;
            public SpriteFont Header;
            public SpriteFont Title;
        }
    }
}

