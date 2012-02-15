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

namespace CircleOfLife
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class CircleOfLifeGame : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        //User Input history
        KeyboardState oldKS;
        MouseState oldMS;

        //graphics fields
        public Texture2D preyTexture;
        Texture2D predatorTexture;

        //Initialize ecosystem
        Ecosystem ecosystem = new Ecosystem();

        public CircleOfLifeGame()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            //Sets window size
            graphics.PreferredBackBufferWidth = 1024;
            graphics.PreferredBackBufferHeight = 768;
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            //Set mouse visibility
            this.IsMouseVisible = true;
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


            preyTexture = Content.Load<Texture2D>("panda");
            predatorTexture = Content.Load<Texture2D>("dragon");
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
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

            updateInput();
            updateEcosystem();

            base.Update(gameTime);
        }

        private void updateInput()
        {
            KeyboardState newKS = Keyboard.GetState();
            MouseState newMS = Mouse.GetState();
            


            // Check for exit.
            if (newKS.IsKeyDown(Keys.Escape))
            {
                Exit();
            }


            if (newMS.LeftButton.Equals(ButtonState.Pressed))
            {
                // If not down last update, key has just been pressed.
                if (!oldMS.LeftButton.Equals(ButtonState.Pressed))
                {
                    //Creature creation done here for now
                    Ecosystem.speciesStats stats = new Ecosystem.speciesStats();
                    stats.diet = 0;
                    stats.size = 5;
                    stats.detection = 100;
                    stats.speed = 5;
                    stats.energyCap = 100;
                    stats.foodCap = 100;
                    stats.waterCap = 100;
                    stats.energyValue = 50;
                    stats.agility = 0.15f;
                    ecosystem.addSpecies("mouse", stats, (short)newMS.X, (short)newMS.Y);
                }
            }
            else if (newMS.RightButton.Equals(ButtonState.Pressed))
            {
                // If not down last update, key has just been pressed.
                if (!oldMS.RightButton.Equals(ButtonState.Pressed))
                {
                    //Creature creation done here for now
                    Ecosystem.speciesStats stats = new Ecosystem.speciesStats();
                    stats.diet = 1;
                    stats.size = 10;
                    stats.detection = 110;
                    stats.speed = 4;
                    stats.energyCap = 100;
                    stats.foodCap = 100;
                    stats.waterCap = 100;
                    stats.energyValue = 50;
                    stats.agility = 0.10f;
                    ecosystem.addSpecies("cat", stats, (short)newMS.X, (short)newMS.Y);
                }
            }
            // Update saved state.
            oldKS = newKS;
            oldMS = newMS;
        }


        private void updateEcosystem()
        {
            ecosystem.update();
        }


        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.White);

            spriteBatch.Begin(/*SpriteSortMode.BackToFront, BlendState.AlphaBlend*/);

            //Ecosystem class calls the draw function of every creature
            ecosystem.draw(ref graphics, ref spriteBatch, ref preyTexture);


            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
