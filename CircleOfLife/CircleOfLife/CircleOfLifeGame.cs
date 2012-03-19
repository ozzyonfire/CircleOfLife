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
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        
        //graphics fields
        Texture2D spriteSheet;
        Texture2D bushTexture;

        Ecosystem ecosystem;
        User user;

        KeyboardState oldKS;
        MouseState oldMS;


        public CircleOfLifeGame()
        {
            graphics = new GraphicsDeviceManager(this);
          
            //set resolution
            graphics.PreferredBackBufferWidth = 1920;
            graphics.PreferredBackBufferHeight = 1080;
            graphics.IsFullScreen = true;
           
            KeyboardState oldKS = Keyboard.GetState();
            MouseState oldMS = Mouse.GetState();

            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            //Initialize ecosystem
            ecosystem = new Ecosystem(this);
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

            spriteSheet = Content.Load<Texture2D>("test_sheet2");
            bushTexture = Content.Load<Texture2D>("bush");
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

            ecosystem.Update(gameTime);
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
            ecosystem.draw(gameTime, spriteBatch, spriteSheet);

            spriteBatch.End();

            base.Draw(gameTime);
        }

    }


    //nothing..game state template
    public class TitleState : Nuclex.Game.States.DrawableGameState
    {

        GraphicsDevice graphicsDevice;

        SpriteBatch spriteBatch;

        public TitleState(GraphicsDevice graphicsDevice, SpriteBatch spriteBatch)
        {
            this.graphicsDevice = graphicsDevice;
            this.spriteBatch = spriteBatch;
        }

        public override void Update(GameTime gameTime)
        {

        }

        public override void Draw(GameTime gameTime)
        {
            graphicsDevice.Clear(Color.Brown);

            //base.Draw(gameTime);
        }
    }

}

