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
        Texture2D bushTexture;

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

            // set up species in here for now
            Ecosystem.speciesStats preyStats = new Ecosystem.speciesStats();
            preyStats.diet = 0;
            preyStats.size = 5;
            preyStats.detection = 100;
            preyStats.speed = 7;
            preyStats.energyCap = 100;
            preyStats.foodCap = 100;
            preyStats.waterCap = 100;
            preyStats.energyValue = 20;
            preyStats.agility = 0.15f;
            ecosystem.addSpecies("mouse", preyStats, preyTexture);

            Ecosystem.speciesStats predStats = new Ecosystem.speciesStats();
            predStats.diet = 1;
            predStats.size = 10;
            predStats.detection = 150;
            predStats.speed = 5;
            predStats.energyCap = 100;
            predStats.foodCap = 100;
            predStats.waterCap = 100;
            predStats.energyValue = 50;
            predStats.agility = 0.15f;
            ecosystem.addSpecies("cat", predStats, predatorTexture);
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
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

            updateInput();
            updateEcosystem(gameTime);

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
                    if (newMS.X > 0 && newMS.X < graphics.PreferredBackBufferWidth
                        && newMS.Y > 0 && newMS.Y < graphics.PreferredBackBufferHeight)
                    {
                        ecosystem.addCreature(0, (short)newMS.X, (short)newMS.Y);
                    }
                }
            }
            else if (newMS.RightButton.Equals(ButtonState.Pressed))
            {
                // If not down last update, key has just been pressed.
                if (oldMS.RightButton.Equals(ButtonState.Pressed))
                {
                    if (newMS.X > 0 && newMS.X < graphics.PreferredBackBufferWidth
                        && newMS.Y > 0 && newMS.Y < graphics.PreferredBackBufferHeight)
                    {
                        //Creature creation done here for now
                        ecosystem.addCreature(1, (short)newMS.X, (short)newMS.Y);
                    }
                }
            }
            else if (newMS.MiddleButton.Equals(ButtonState.Pressed))
            {
                // If not down last update, key has just been pressed.
                if (!oldMS.MiddleButton.Equals(ButtonState.Pressed))
                {
                    // add a shrub
                    Ecosystem.floraStats stats = new Ecosystem.floraStats();
                    stats.foodValue = 10;
                    stats.size = 50;
                    stats.energyValue = 20;
                    ecosystem.addFlora("shrub", bushTexture, stats, (short)newMS.X, (short)newMS.Y);
                }
            }
            // Update saved state.
            oldKS = newKS;
            oldMS = newMS;
        }


        private void updateEcosystem(GameTime gameTime)
        {
            ecosystem.update(gameTime);
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
