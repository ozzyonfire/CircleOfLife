using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using Nuclex.UserInterface;
using Nuclex.Input;
using Nuclex.Game;


namespace CircleOfLife
{
    public class User
    {
        Ecosystem ecosystem;    //reference to ecosystem class
        Game game;
        CircleOfLifeGame baseGame;
        Viewport viewport;

        //Nuclex Stuff
        GuiManager gui;
        InputManager input;
        Nuclex.Game.States.GameStateManager state;  //?

        //devices
        Nuclex.Input.Devices.IKeyboard keyboard;
        Nuclex.Input.Devices.IMouse mouse;

        //Game screens for different modes
        Screen gameScreen;
        Screen menuScreen;
        Screen startScreen;
        Screen createScreen;

        //temporary dialog
        SpeciesDialog speciesDialog;
        GameDialog gameDialog;


        Rectangle hudBackground = Sprites.hudBackground;
        Rectangle hudDestination;

        PerkTree perkTree;
        PerkTree createTree;
        public Effects effects;

        public float lastRotation = 0.0f; //for preview

        #region nuclex controls
        //Nuclex.UserInterface.Controls.Desktop.HorizontalSliderControl menuSlider;
        Nuclex.UserInterface.Controls.Desktop.ListControl speciesList;
        Nuclex.UserInterface.Controls.Desktop.ButtonControl upgradeButton;
        Nuclex.UserInterface.Controls.Desktop.ButtonControl cancelButton;
        //Nuclex.UserInterface.Controls.LabelControl description;

        // Nuclex.UserInterface.Controls.LabelControl header = new Nuclex.UserInterface.Controls.LabelControl();
        Nuclex.UserInterface.Controls.LabelControl pointsLabel = new Nuclex.UserInterface.Controls.LabelControl();
        Nuclex.UserInterface.Controls.LabelControl nameLabel = new Nuclex.UserInterface.Controls.LabelControl();
        Nuclex.UserInterface.Controls.LabelControl colorLabel = new Nuclex.UserInterface.Controls.LabelControl();
        Nuclex.UserInterface.Controls.LabelControl dietLabel = new Nuclex.UserInterface.Controls.LabelControl();
        //Nuclex.UserInterface.Controls.LabelControl sizeLabel = new Nuclex.UserInterface.Controls.LabelControl();
        //Nuclex.UserInterface.Controls.LabelControl speedLabel = new Nuclex.UserInterface.Controls.LabelControl();
        //Nuclex.UserInterface.Controls.LabelControl agilityLabel = new Nuclex.UserInterface.Controls.LabelControl();
        //Nuclex.UserInterface.Controls.LabelControl detectionLabel = new Nuclex.UserInterface.Controls.LabelControl();

        Nuclex.UserInterface.Controls.Desktop.InputControl nameInput = new Nuclex.UserInterface.Controls.Desktop.InputControl();
        Nuclex.UserInterface.Controls.Desktop.ListControl colorInput = new Nuclex.UserInterface.Controls.Desktop.ListControl();
        Nuclex.UserInterface.Controls.Desktop.ListControl dietInput = new Nuclex.UserInterface.Controls.Desktop.ListControl();

        Nuclex.UserInterface.Controls.Desktop.ButtonControl createButton = new Nuclex.UserInterface.Controls.Desktop.ButtonControl();
        #endregion



        //public Nuclex.UserInterface.Controls.Desktop.ButtonControl clearButton = new Nuclex.UserInterface.Controls.Desktop.ButtonControl();
        public GameButton clearButton = new GameButton();

        public User(Game game, Ecosystem ecosystem)
        {
            this.ecosystem = ecosystem;
            this.game = game;
            this.baseGame = (CircleOfLifeGame)game;

            //Initialize Nuclex Managers
            state = new Nuclex.Game.States.GameStateManager(game.Services);
            input = new InputManager(game.Services);
            gui = new GuiManager(game.Services);
            
            //Add nuclex managers to game components
            game.Components.Add(state);
            game.Components.Add(input);
            game.Components.Add(gui);

            //initialize input device objects
            keyboard = input.GetKeyboard();
            mouse = input.GetMouse();

            hudDestination = new Rectangle(0, (int)(baseGame.graphics.PreferredBackBufferHeight * 0.9f), baseGame.graphics.PreferredBackBufferWidth, (int)(baseGame.graphics.PreferredBackBufferHeight * 0.1f));
            //meh
            Initialize();
        }

        public void Initialize()
        {
            //load in skin
            //Nuclex.UserInterface.Visuals.Flat.FlatGuiVisualizer.FromResource(
            


            //retrieve viewport
            viewport = game.GraphicsDevice.Viewport;

            gameScreen = new Screen(viewport.Width, viewport.Height);

            menuScreen = new Screen(viewport.Width, viewport.Height);

            createScreen = new Screen(viewport.Width, viewport.Height);
            startScreen = new Screen(viewport.Width, viewport.Height);

            speciesDialog = new SpeciesDialog();
            //set clear button listener
            speciesDialog.clearButton.Pressed += new EventHandler(clearButton_Pressed);
            
            
            //DEBUG DIALOG!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
            //gameScreen.Desktop.Children.Add(speciesDialog);
            //DEBUG DIALOG!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
            

            //GameUI gameUI = new GameUI(gameScreen, baseGame.spriteSheet);
            gui.Screen = startScreen;
            
            //listeners
            mouse.MouseMoved += new Nuclex.Input.Devices.MouseMoveDelegate(mouse_MouseMoved);
            //mouse.MouseButtonPressed += new Nuclex.Input.Devices.MouseButtonDelegate(mouse_MouseButtonReleased);
            mouse.MouseButtonReleased += new Nuclex.Input.Devices.MouseButtonDelegate(mouse_MouseButtonReleased);
            mouse.MouseWheelRotated += new Nuclex.Input.Devices.MouseWheelDelegate(mouse_MouseWheelRotated);

            keyboard.KeyReleased += new Nuclex.Input.Devices.KeyDelegate(keyboard_KeyReleased);

            keyboard.KeyPressed += new Nuclex.Input.Devices.KeyDelegate(keyboard_KeyPressed);
            
        }

        public void enterMenu()
        {
            baseGame.menuOpen = true;
            speciesList.Items.Clear();
            for (int i = 0; i < ecosystem.species.Count; i++)
            {
                speciesList.Items.Add(ecosystem.species[i].name);
            }
            perkTree.showSpecies(ecosystem.species[0]);
            gui.Screen = menuScreen;

        }

        public void enterCreate()
        {
            baseGame.createOpen = true;
            gui.Screen = createScreen;

            nameInput.Text = "Kleemo" + ecosystem.species.Count.ToString();
            //Reset Defaults!!
        }


        void perkSliderMoved(object sender, EventArgs e)
        {
            Nuclex.UserInterface.Controls.Desktop.HorizontalSliderControl slider = (Nuclex.UserInterface.Controls.Desktop.HorizontalSliderControl)sender;
            perkTree.offset = (int)(slider.ThumbPosition*100);
        }

        public void drawHUD(GameTime gameTime, SpriteBatch spriteBatch, Texture2D spriteSheet, CircleOfLifeGame.GameFonts gameFonts)
        {
            //only draws the background - nuclex handles the buttons
            //spriteBatch.Draw(spriteSheet, hudDestination, hudBackground, Color.White, 0.0f, new Vector2(0), SpriteEffects.None, 0.0f);
            effects.draw(gameTime, spriteBatch, spriteSheet, gameFonts);
            spriteBatch.DrawString(gameFonts.Header, "Order: " + baseGame.oPoints.ToString() + "\nEvolution: " + baseGame.ePoints.ToString(), new Vector2(viewport.Width * 0.9f, 25), Color.Magenta);

            //drawMouse(spriteBatch, spriteSheet);
        }

        public void drawMenu(GameTime gameTime, SpriteBatch spriteBatch, Texture2D spriteSheet, CircleOfLifeGame.GameFonts gameFonts)
        {
            //Draw menu background
            spriteBatch.Draw(spriteSheet, new Rectangle(0, 0, viewport.Width, viewport.Height), new Rectangle(0, 1050, 1, 1), Color.White, 0.0f, Vector2.Zero, SpriteEffects.None, 1.0f);
            spriteBatch.Draw(spriteSheet, new Rectangle(0, 0, viewport.Width, viewport.Height), new Rectangle(100, 100, 800, 800), Color.White, 0.0f, Vector2.Zero, SpriteEffects.None, 0.5f);
            //Draw menu Title
            spriteBatch.DrawString(gameFonts.Title, "Upgrade Menu", new Vector2(viewport.Width * 0.45f, viewport.Height * 0.05f), Color.Black);
            perkTree.draw(gameTime, spriteBatch, spriteSheet, gameFonts);

            spriteBatch.DrawString(gameFonts.Header, "Order Points: " + baseGame.oPoints.ToString() + "      Evolution Points: " + baseGame.ePoints.ToString(), new Vector2(50, 650), Color.Black);
            drawPreview(new Rectangle(200, 575, 150, 150), gameTime, spriteBatch, spriteSheet);
            //drawMouse(spriteBatch, spriteSheet);
        }

        public void drawCreate(GameTime gameTime, SpriteBatch spriteBatch, Texture2D spriteSheet, CircleOfLifeGame.GameFonts gameFonts)
        {
            //Draw menu background
            spriteBatch.Draw(spriteSheet, new Rectangle(0, 0, viewport.Width, viewport.Height), new Rectangle(0, 1050, 1, 1), Color.White, 0.0f, Vector2.Zero, SpriteEffects.None, 1.0f);
            spriteBatch.Draw(spriteSheet, new Rectangle(0, 0, viewport.Width, viewport.Height), new Rectangle(100, 100, 800, 800), Color.White, 0.0f, Vector2.Zero, SpriteEffects.None, 0.1f);
            spriteBatch.DrawString(gameFonts.Title, "Species Creator", new Vector2(viewport.Width * 0.4f, viewport.Height * 0.1f), Color.Black);
            spriteBatch.DrawString(gameFonts.Header, "Order Points: " + baseGame.oPoints.ToString() + "      Evolution Points: " + baseGame.ePoints.ToString(), new Vector2(viewport.Width * 0.4f, viewport.Height * 0.8f), Color.Black);
            createTree.draw(gameTime, spriteBatch, spriteSheet, gameFonts);
            drawPreview(new Rectangle(1250,400,200,200),gameTime, spriteBatch, spriteSheet);
            //drawMouse(spriteBatch, spriteSheet);
        }

        public void drawStart(GameTime gameTime, SpriteBatch spriteBatch, Texture2D spriteSheet, CircleOfLifeGame.GameFonts gameFonts)
        {
            spriteBatch.Draw(spriteSheet, new Rectangle((int)(viewport.Width * 0.25f), (int)(viewport.Height * 0.25f), (int)(viewport.Width * 0.5f), (int)(viewport.Height * 0.4f)), Sprites.title, Color.White, 0.0f, Vector2.Zero, SpriteEffects.None, 0.1f);
            //Draw menu Title
            spriteBatch.DrawString(gameFonts.Title, "Press Anything to Start", new Vector2(viewport.Width * 0.35f, viewport.Height * 0.75f), new Color(255, 0, 0, 50));

        }

        public void drawMouse(SpriteBatch spriteBatch, Texture2D spriteSheet)
        {
            MouseState ms = mouse.GetState();
            Rectangle mouseRect;
            if (baseGame.scrollRight)
                mouseRect = new Rectangle(1100,300,100,100);
            else if (baseGame.scrollLeft)
                mouseRect = new Rectangle(1200,300,100,100);
            else if (baseGame.scrollUp)
                mouseRect = new Rectangle(1300,300,100,100);
            else if (baseGame.scrollDown)
                mouseRect = new Rectangle(1400, 300, 100, 100);
            else if (baseGame.addingSpecies)
                mouseRect = new Rectangle(1500, 300, 100, 100);
            else
                mouseRect = new Rectangle(1000,300,100,100);

            spriteBatch.Draw(spriteSheet, new Rectangle(ms.X - 25, ms.Y - 25, 50, 50), mouseRect, Color.Green, 0.0f, Vector2.Zero, SpriteEffects.None, 0.0f);
        }


        //-------------------------
        //EVENT HANDLERS:
        void clearButton_Pressed(object sender, EventArgs e)
        {
            ecosystem.clearEcosystem();
            this.speciesDialog.clearButton.sourceRect = new RectangleF(0, 0, 100, 100);
        }

        void mouse_MouseMoved(float x, float y)
        {
            KeyboardState ks = keyboard.GetState();
            //if outside game screen bounds return
            if(x == -1 || y == -1)
                return;


            //if in menu mode
            if (baseGame.menuOpen)
                perkTree.mouseOver((int)x, (int)y);
            else if (baseGame.createOpen)
                createTree.mouseOver((int)x, (int)y);
            else
            {
                //check to see if the mouse has been moved to a position that indicates the user wants to scroll


                if (x < viewport.Width * 0.05)
                    baseGame.scrollLeft = true;
                else if (x > viewport.Width * 0.95)
                    baseGame.scrollRight = true;
                else if (baseGame.scrollLeft || baseGame.scrollRight)
                {
                    if (!ks.IsKeyDown(Keys.Left))
                        baseGame.scrollLeft = false;
                    if (!ks.IsKeyDown(Keys.Right))
                        baseGame.scrollRight = false;
                }

                if (y < viewport.Height * 0.05)
                    baseGame.scrollUp = true;
                else if (y > viewport.Height * 0.95)
                    baseGame.scrollDown = true;
                else if (baseGame.scrollUp || baseGame.scrollDown)
                {
                    if (!ks.IsKeyDown(Keys.Up))
                        baseGame.scrollUp = false;
                    if (!ks.IsKeyDown(Keys.Down))
                        baseGame.scrollDown = false;
                }
            }
            


            
        }

        void mouse_MouseButtonReleased(MouseButtons buttons)
        {
            MouseState ms = mouse.GetState();


            //return if in menu state
            if (baseGame.menuOpen)
            {
                perkTree.mouseClick(ms.X, ms.Y);
            }
            else if (baseGame.createOpen)
            {
                createTree.mouseClick(ms.X, ms.Y);
            }
            else if (baseGame.startOpen)
            {

            }
            else if (baseGame.addingSpecies)
            {
                Species newSpecies = ecosystem.addSpecies(nameInput.Text, baseGame.newStats);
                if (baseGame.newStats.diet == 1)
                {
                    newSpecies.perks[1] = true;
                }
                else
                {
                    newSpecies.perks[0] = true;
                }

                newSpecies.addCreature((int)((ms.X) / baseGame.userView.Z - baseGame.userView.X), (int)((ms.Y) / baseGame.userView.Z - baseGame.userView.Y));
                baseGame.addingSpecies = false;
            }
            else
            {
                /*
                //Temporary debug stuff
                //check not in dialog
                if (ms.X > speciesDialog.Bounds.Left.Fraction * viewport.Width + speciesDialog.Bounds.Left.Offset &&
                    ms.X < speciesDialog.Bounds.Right.Fraction * viewport.Width + speciesDialog.Bounds.Right.Offset &&
                    ms.Y < speciesDialog.Bounds.Bottom.Fraction * viewport.Width + speciesDialog.Bounds.Bottom.Offset &&
                    ms.Y > speciesDialog.Bounds.Top.Fraction * viewport.Width + speciesDialog.Bounds.Top.Offset)
                    return;

                // set up species in here for now
                Ecosystem.speciesStats preyStats = new Ecosystem.speciesStats();

                preyStats.size = Convert.ToInt16(speciesDialog.sizeInput.Text);
                preyStats.detection = Convert.ToInt16(speciesDialog.detectionInput.Text);
                preyStats.speed = Convert.ToInt16(speciesDialog.speedInput.Text);
                preyStats.energyCap = 100;
                preyStats.foodCap = 100;
                preyStats.waterCap = 100;
                preyStats.energyValue = 20;
                preyStats.agility = Convert.ToSingle(speciesDialog.agilityInput.Text);
                if (speciesDialog.colorInput.SelectedItems.Count == 1)
                    switch (speciesDialog.colorInput.Items[speciesDialog.colorInput.SelectedItems[0]])
                    {
                        case "Red":
                            preyStats.color = Color.Red;
                            break;
                        case "Green":
                            preyStats.color = Color.Green;
                            break;
                        case "Blue":
                            preyStats.color = Color.Blue;
                            break;
                        case "Brown":
                            preyStats.color = Color.Brown;
                            break;
                        case "Orange":
                            preyStats.color = Color.Orange;
                            break;
                        default:
                            preyStats.color = Color.Brown;
                            break;
                    }
                else
                    preyStats.color = Color.Brown;

                if (speciesDialog.dietInput.SelectedItems.Count == 1)
                    switch (speciesDialog.dietInput.SelectedItems[0])
                    {
                        case 0:
                            preyStats.diet = 0;
                            break;
                        case 1:
                            preyStats.diet = 1;
                            break;
                        case 2:
                            preyStats.diet = 2;
                            break;
                    }
                else
                    preyStats.diet = 0;

                Species newSpecies = ecosystem.addSpecies(speciesDialog.nameInput.Text, preyStats);
                newSpecies.addCreature((int)((ms.X) / baseGame.userView.Z - baseGame.userView.X), (int)((ms.Y) / baseGame.userView.Z - baseGame.userView.Y));
                // newSpecies.addCreature(ms.X - (int)baseGame.userView.X + 10, ms.Y - (int)baseGame.userView.Y);
                // newSpecies.addCreature(ms.X - (int)baseGame.userView.X + 10, ms.Y - (int)baseGame.userView.Y + 10);
                //  newSpecies.addCreature(ms.X - (int)baseGame.userView.X, ms.Y - (int)baseGame.userView.Y + 10);

                speciesDialog.nameInput.Text = "Kleemo" + ecosystem.species.Count.ToString();
                */

            }
        }

        void mouse_MouseWheelRotated(float ticks)
        {

            MouseState ms = mouse.GetState();
            baseGame.userView = new Vector3(baseGame.userView.X + (viewport.Width / 2 - ms.X)* (baseGame.userView.Z), baseGame.userView.Y + (viewport.Height / 2 - ms.Y) * (baseGame.userView.Z), baseGame.userView.Z + 0.1f * ticks);
            //Console.WriteLine("old: " + baseGame.userView.X.ToString() + ", " + baseGame.userView.Y.ToString());   
            //mouse centered zoom!
            //baseGame.userView = new Vector3(baseGame.userView.X - viewport.Width * (0.1f * ticks)/*+ (viewport.Width / 2 )* (baseGame.userView.Z)*/, baseGame.userView.Y - viewport.Height * (0.1f * ticks)/* + (viewport.Height / 2 - ms.Y) * (baseGame.userView.Z)*/, baseGame.userView.Z + 0.1f * ticks);
            //baseGame.userView = new Vector3(baseGame.userView.X, baseGame.userView.Y, baseGame.userView.Z + 0.1f * ticks);
            //Console.WriteLine("New: " + baseGame.userView.X.ToString() + ", " + baseGame.userView.Y.ToString());
        }

        void keyboard_KeyReleased(Keys key)
        {
            //Quit game when escape is pressed
            if (key.Equals(Keys.Escape))
                game.Exit();

            //Exit start screen
            if (baseGame.startOpen)
            {
                gui.Screen = createScreen;
                baseGame.startOpen = false;
                baseGame.createOpen = true;
                return;
            }

            //stop scrolling if mouse is in the middle
            MouseState ms = mouse.GetState();
            if (key.Equals(Keys.Right) && !(ms.X > viewport.Width * 0.95))
                baseGame.scrollRight = false;
            if (key.Equals(Keys.Left) && !(ms.X < viewport.Width * 0.05))
                baseGame.scrollLeft = false;
            if (key.Equals(Keys.Up) && !(ms.Y < viewport.Height * 0.05))
                baseGame.scrollUp = false;
            if (key.Equals(Keys.Down) && !(ms.Y > viewport.Height * 0.95))
                baseGame.scrollDown = false;
            
            //
            if (key.Equals(Keys.M))
            {
                if (baseGame.menuOpen)
                {
                    baseGame.menuOpen = false;
                    gui.Screen = gameScreen;
                }
                else
                {
                    enterMenu();
                }
            }
            if (key.Equals(Keys.Space))
            {
                effects.addFloatingString("CAT!!",new Vector2(ms.X,ms.Y),Color.Red);
            }



            //debug map expansion
            if (key.Equals(Keys.E))
            {
                if (baseGame.createOpen)
                {
                    baseGame.createOpen = false;
                    gui.Screen = gameScreen;
                }
                else
                {
                    baseGame.createOpen = true;
                    gui.Screen = createScreen;
                }
            }
        }


        void keyboard_KeyPressed(Keys key)
        {
            //this is adjusted to control speed of adjustment
            //if (DateTime.Now.Ticks - lastInput.Ticks < 300)
            //    return;
            if (key.Equals(Keys.Left) || key.Equals(Keys.Right) || key.Equals(Keys.Down) || key.Equals(Keys.Up))
            {
                if (key.Equals(Keys.Left))
                    baseGame.scrollLeft = true;
                if (key.Equals(Keys.Right))
                    baseGame.scrollRight = true;
                if (key.Equals(Keys.Up))
                    baseGame.scrollUp = true;
                if (key.Equals(Keys.Down))
                    baseGame.scrollDown = true;
            }
        }

        void drawPreview(Rectangle destination, GameTime gameTime, SpriteBatch spriteBatch, Texture2D spriteSheet)
        {
            float rotation = lastRotation + ((float)gameTime.ElapsedGameTime.Milliseconds/5)*0.01f;
            lastRotation = rotation;
            //float rotation = 0.0f;
            Color color;
            

            if (baseGame.menuOpen)
            {
                Species selected;
                if (speciesList.SelectedItems.Count == 0)
                    selected = ecosystem.species[0];
                else
                    selected = ecosystem.species[speciesList.SelectedItems[0]];

                if(selected.Stats.diet == 1)
                    if(perkTree.selectedPerkNode == null)
                        spriteBatch.Draw(spriteSheet, destination, Sprites.carnivore, selected.Stats.color, rotation, new Vector2(destination.Width / 2, destination.Height / 2), SpriteEffects.None, 0.0f);

                    else if(perkTree.selectedPerkNode.ID == 2)
                    {
                        spriteBatch.Draw(spriteSheet, destination, Sprites.carnivoreTail, selected.Stats.color, rotation, new Vector2(destination.Width / 2, destination.Height / 2), SpriteEffects.None, 0.0f);
               
                    }
                    else if (perkTree.selectedPerkNode.ID == 3)
                    {
                        spriteBatch.Draw(spriteSheet, destination, Sprites.carnivoreEyes, selected.Stats.color, rotation, new Vector2(destination.Width / 2, destination.Height / 2), SpriteEffects.None, 0.0f);
               
                    }
                    else
                       
                            spriteBatch.Draw(spriteSheet, destination, Sprites.carnivore, selected.Stats.color, rotation, new Vector2(destination.Width / 2, destination.Height / 2), SpriteEffects.None, 0.0f);

                        
                else
                {
                    if(perkTree.selectedPerkNode == null)
                        spriteBatch.Draw(spriteSheet, destination, Sprites.herbivore, selected.Stats.color, rotation, new Vector2(destination.Width / 2, destination.Height / 2), SpriteEffects.None, 0.0f);
                
                    else if (perkTree.selectedPerkNode.ID == 2)
                    {
                        spriteBatch.Draw(spriteSheet, destination, Sprites.herbivoreTail, selected.Stats.color, rotation, new Vector2(destination.Width / 2, destination.Height / 2), SpriteEffects.None, 0.0f);

                    }
                    else if (perkTree.selectedPerkNode.ID == 3)
                    {
                        spriteBatch.Draw(spriteSheet, destination, Sprites.herbivoreEyes, selected.Stats.color, rotation, new Vector2(destination.Width / 2, destination.Height / 2), SpriteEffects.None, 0.0f);
                
                    }
                    else
                    spriteBatch.Draw(spriteSheet, destination, Sprites.herbivore, selected.Stats.color, rotation, new Vector2(destination.Width / 2, destination.Height / 2), SpriteEffects.None, 0.0f);
                }

            }
            else if (baseGame.createOpen)
            {
                if (colorInput.SelectedItems.Count == 1)
                    switch (colorInput.Items[colorInput.SelectedItems[0]])
                    {
                        case "Red":
                            color = Color.Red;
                            break;
                        case "Green":
                            color = Color.Green;
                            break;
                        case "Blue":
                            color = Color.Blue;
                            break;
                        case "Brown":
                            color = Color.Brown;
                            break;
                        case "Orange":
                            color = Color.Orange;
                            break;
                        default:
                            color = Color.Brown;
                            break;
                    }
                else
                    color = Color.Brown;

                if (createTree.perks[1].Selected)
                {
                    spriteBatch.Draw(spriteSheet, destination, Sprites.carnivore, color, rotation, new Vector2(destination.Width / 2, destination.Height / 2), SpriteEffects.None, 0.0f);
                }
                else
                {
                    spriteBatch.Draw(spriteSheet, destination, Sprites.herbivore, color, rotation, new Vector2(destination.Width / 2, destination.Height / 2), SpriteEffects.None, 0.0f);
            
                }
            }
            
        }



        //The following builds and initializes the games gui's seperated from the user class mainly for organizational purposes
        public void initializeGameScreen()
        {
            GameButton torchButton = new GameButton(new UniRectangle(
                    new UniScalar(0.11f, 0.0f),
                    new UniScalar(0.91f, 0.0f),
                    new UniScalar(0.09f, 0.0f),
                    new UniScalar(0.09f, 0.0f)),
                    baseGame.spriteSheet, new RectangleF(
                        Sprites.torchButton.X,
                        Sprites.torchButton.Y,
                        Sprites.torchButton.Width,
                        Sprites.torchButton.Height));
                torchButton.hoverSourceRect = new RectangleF(
                        Sprites.torchButton.X + 150,
                        Sprites.torchButton.Y,
                        Sprites.torchButton.Width,
                        Sprites.torchButton.Height);

                torchButton.Text = "Menu";
                torchButton.Pressed += new EventHandler(torchButton_Pressed);

                GameButton addButton = new GameButton(new UniRectangle(
                        new UniScalar(0.0f, 0.0f),
                        new UniScalar(0.91f, 0.0f),
                        new UniScalar(0.09f, 0.0f),
                        new UniScalar(0.09f, 0.0f)),
                        baseGame.spriteSheet, new RectangleF(
                            Sprites.addButton.X,
                            Sprites.addButton.Y,
                            Sprites.addButton.Width,
                            Sprites.addButton.Height));
                addButton.hoverSourceRect = new RectangleF(
                        Sprites.addButton.X + 150,
                        Sprites.addButton.Y,
                        Sprites.addButton.Width,
                        Sprites.addButton.Height);

                addButton.Text = "Add";
                addButton.Pressed += new EventHandler(addButton_Pressed);


                gameScreen.Desktop.Children.Add(addButton);
                gameScreen.Desktop.Children.Add(torchButton);
        }

        void addButton_Pressed(object sender, EventArgs e)
        {
            enterCreate();
        }

        void torchButton_Pressed(object sender, EventArgs e)
        {
            enterMenu();
        }


        public void initializeMenuScreen()
        {
            perkTree = new PerkTree();
            effects = new Effects();

            Perk newPerk;

            newPerk = perkTree.add("Herbivore", new Vector2(650, 150));
            newPerk.effects = "Allows creature to\n consume plants";
            newPerk.cost = "O: 100     E: 0";
            newPerk.exclusive = perkTree.add("Carnivore", new Vector2(900, 150));
            newPerk.exclusive.exclusive = newPerk;
            newPerk = newPerk.exclusive;
            newPerk.effects = "Allows creature to\nconsume creatures";
            newPerk.cost = "O: 500     E: 1";
            newPerk = perkTree.add("Pincer", new Vector2(650, 450));
            newPerk.effects = "Increased attack\nFaster consumption";
            newPerk.cost = "O: 500     E: 5";
            newPerk.blocked = true;
            newPerk = perkTree.add("Tail", new Vector2(650, 300));
            newPerk.effects = "Increased speed";
            newPerk.cost = "O: 500     E: 5";
            newPerk = perkTree.add("Eyes", new Vector2(900, 300));
            newPerk.effects = "Increased detection";
            newPerk.cost = "O: 200    E: 5";
            newPerk = perkTree.add("Swarm", new Vector2(1150, 300));
            newPerk.effects = "Increased birth rate";
            newPerk.cost = "O: 1000     E: 10";
            newPerk.blocked = true;
            newPerk = perkTree.add("Scent", new Vector2(900, 450));
            newPerk.effects = "Detection of corpses";
            newPerk.cost = "O: 500     E: 5";
            newPerk = perkTree.add("Bulk", new Vector2(1150, 450));
            newPerk.effects = "Increased defence\nSlower speed";
            newPerk.cost = "O: 1500     E: 5";
            newPerk = perkTree.add("Hibernate", new Vector2(650, 600));
            newPerk.effects = "Conserve energy by\nremaining still";
            newPerk.cost = "O: 1000     E: 10";
            newPerk.blocked = true;
            newPerk = perkTree.add("Canibal", new Vector2(900, 600));
            newPerk.effects = "Consume corpses of\nsame species";
            newPerk.cost = "O: 200     E: 5";
            newPerk.blocked = true;


            //initialize elements
            speciesList = new Nuclex.UserInterface.Controls.Desktop.ListControl();
            upgradeButton = new Nuclex.UserInterface.Controls.Desktop.ButtonControl();
            cancelButton = new Nuclex.UserInterface.Controls.Desktop.ButtonControl();
            //menuSlider = new Nuclex.UserInterface.Controls.Desktop.HorizontalSliderControl();
            //description = new Nuclex.UserInterface.Controls.LabelControl();

            speciesList.Bounds = new UniRectangle(new UniScalar(0.0f, 50.0f),new UniScalar(0.0f, 200.0f),new UniScalar(0.0f, 300.0f),new UniScalar(0.0f, 250.0f));
            speciesList.SelectionMode = Nuclex.UserInterface.Controls.Desktop.ListSelectionMode.Single;
            speciesList.SelectionChanged += new EventHandler(speciesList_SelectionChanged);

            upgradeButton.Bounds = new UniRectangle(new UniScalar(0.02f, 0.0f), new UniScalar(0.0f, 700.0f), new UniScalar(0.0f, 250.0f), new UniScalar(0.0f, 50.0f));
            upgradeButton.Text = "Upgrade";
            upgradeButton.Pressed += new EventHandler(upgradeButton_Pressed);


            cancelButton.Bounds = new UniRectangle(new UniScalar(0.02f, 260.0f), new UniScalar(0.0f, 700.0f), new UniScalar(0.0f, 250.0f), new UniScalar(0.0f, 50.0f));
            cancelButton.Text = "Cancel";
            cancelButton.Pressed += new EventHandler(cancelButton_Pressed);

            //menuSlider.Bounds = new UniRectangle(new UniScalar(0.3f, 0), new UniScalar(0.75f, 0),new UniScalar(0.5f,0), new UniScalar(0.0f, 30));
            //menuSlider.ThumbSize = 0.1f;
            //menuSlider.Moved += new EventHandler(perkSliderMoved);

            //description.Bounds = new UniRectangle(new UniScalar(0.3f, 0), new UniScalar(0.8f, 0), new UniScalar(0.5f, 0), new UniScalar(0.2f, 0));
            //description.Text = Sprites.description;

            menuScreen.Desktop.Children.Add(speciesList);
            menuScreen.Desktop.Children.Add(upgradeButton);
            menuScreen.Desktop.Children.Add(cancelButton);
        }

        void upgradeButton_Pressed(object sender, EventArgs e)
        {
            if (perkTree.selectedPerkNode != null)
            {
                int n = perkTree.selectedPerkNode.ID;
                perkTree.selectedPerkNode.bought = true;
                int s;
                if(speciesList.SelectedItems.Count == 0)
                    s = 0;
                else
                    s = speciesList.SelectedItems[0];
                Species selected = ecosystem.species[s];
                selected.perks[n] = true;

                baseGame.menuOpen = false;
                gui.Screen = gameScreen;
            }
        }

        void cancelButton_Pressed(object sender, EventArgs e)
        {
            baseGame.menuOpen = false;
            gui.Screen = gameScreen;
        }

        void speciesList_SelectionChanged(object sender, EventArgs e)
        {
            if(speciesList.SelectedItems.Count != 0)
                perkTree.showSpecies(ecosystem.species[speciesList.SelectedItems[0]]);
            
        }

        public void initializeCreateScreen()
        {
            createTree = new PerkTree();

            Perk newPerk = createTree.add("Herbivore", new Vector2(650, 475));
            newPerk.effects = "Allows creature to\n consume plants";
            newPerk.cost = "O: 100     E: 0";
            newPerk.Selected = true;
            createTree.selectedPerkNode = newPerk;
            newPerk = createTree.add("Carnivore", new Vector2(900, 475));
            newPerk.effects = "Allows creature to\nconsume creatures";
            newPerk.cost = "O: 500     E: 1";


            nameLabel.Bounds = new UniRectangle(new UniScalar(0.4f, 10f), new UniScalar(0.1f, 125.0f), 300, 0);
            colorLabel.Bounds = new UniRectangle(new UniScalar(0.4f, 10f), new UniScalar(0.1f, 155.0f), 300, 0);
           // dietLabel.Bounds = new UniRectangle(new UniScalar(0.4f, 10f), new UniScalar(0.1f, 75.0f), 80, 24);



            nameInput.Bounds = new UniRectangle(new UniScalar(0.5f, 10f), new UniScalar(0.1f, 125.0f), 300, 24);
            colorInput.Bounds = new UniRectangle(new UniScalar(0.5f, 10f), new UniScalar(0.1f, 155.0f), 300, 150);
            //dietInput.Bounds = new UniRectangle(new UniScalar(0.5f, 10f), new UniScalar(0.1f, 75.0f), 120, 24);

            createButton.Bounds = new UniRectangle(new UniScalar(0.5f, -5f), new UniScalar(0.7f, 0.0f), 200, 50);

            nameLabel.Text = "Species Name:";
            colorLabel.Text = "Color:";
            dietLabel.Text = "Diet:";

            createButton.Text = "Create";
            createButton.Pressed += new EventHandler(createButton_Pressed);

            colorInput.Items.Add("Red");
            colorInput.Items.Add("Green");
            colorInput.Items.Add("Blue");
            colorInput.Items.Add("Brown");
            colorInput.Items.Add("Orange");
            colorInput.SelectionMode = Nuclex.UserInterface.Controls.Desktop.ListSelectionMode.Single;

            dietInput.Items.Add("Herbivore");
            dietInput.Items.Add("Carnivore");
            dietInput.Items.Add("Omnivore");
            dietInput.SelectionMode = Nuclex.UserInterface.Controls.Desktop.ListSelectionMode.Single;

            nameInput.Text = "Kleemo";


            createScreen.Desktop.Children.Add(nameLabel);
            createScreen.Desktop.Children.Add(colorLabel);
            createScreen.Desktop.Children.Add(dietLabel);

            createScreen.Desktop.Children.Add(nameInput);
            createScreen.Desktop.Children.Add(colorInput);
            createScreen.Desktop.Children.Add(dietInput);

            createScreen.Desktop.Children.Add(createButton);

        }

        void createButton_Pressed(object sender, EventArgs e)
        {
            Ecosystem.speciesStats speciesStats;
            speciesStats.size = 40;
            speciesStats.detection = 150;
            speciesStats.speed = 5;
            speciesStats.energyCap = 100;
            speciesStats.foodCap = 20;
            speciesStats.waterCap = 100;
            speciesStats.energyValue = 20;
            speciesStats.agility = 0.15f;

            if (createTree.perks[1].Selected)
            {
                speciesStats.diet = 1;
            }
            else
            {
                speciesStats.diet = 0;
            }
           

            // newSpecies.addCreature(ms.X - (int)baseGame.userView.X + 10, ms.Y - (int)baseGame.userView.Y);
            // newSpecies.addCreature(ms.X - (int)baseGame.userView.X + 10, ms.Y - (int)baseGame.userView.Y + 10);
            //  newSpecies.addCreature(ms.X - (int)baseGame.userView.X, ms.Y - (int)baseGame.userView.Y + 10);

           
            
            if (colorInput.SelectedItems.Count == 1)
                switch (colorInput.Items[colorInput.SelectedItems[0]])
                {
                    case "Red":
                        speciesStats.color = Color.Red;
                        break;
                    case "Green":
                        speciesStats.color = Color.Green;
                        break;
                    case "Blue":
                        speciesStats.color = Color.Blue;
                        break;
                    case "Brown":
                        speciesStats.color = Color.Brown;
                        break;
                    case "Orange":
                        speciesStats.color = Color.Orange;
                        break;
                    default:
                        speciesStats.color = Color.Brown;
                        break;
                }
            else
                speciesStats.color = Color.Brown;



            baseGame.newStats = speciesStats;
            baseGame.addingSpecies = true;
            baseGame.createOpen = false;
            gui.Screen = gameScreen;
        }


        public void dialog(string text, string title)
        {
            float w = menuScreen.Width;
            float h = menuScreen.Height;
            UniRectangle bounds = new UniRectangle(w * 0.35f, h * 0.4f, w * 0.3f, h * 0.2f);
            gameDialog = new GameDialog(text, title, bounds);
            baseGame.dialogOpen = true;
            gameDialog.proceedButton.Pressed += new EventHandler(proceedButton_Pressed);
            if (baseGame.menuOpen)
            {
                menuScreen.Desktop.Children.Add(gameDialog);
            }
            else
            {
                gameScreen.Desktop.Children.Add(gameDialog);
            }
        }

        void proceedButton_Pressed(object sender, EventArgs e)
        {
            gameDialog.Close();
            baseGame.dialogOpen = false;
        }


         public partial class GameDialog : Nuclex.UserInterface.Controls.Desktop.WindowControl
        {

            
            // Nuclex.UserInterface.Controls.LabelControl header = new Nuclex.UserInterface.Controls.LabelControl();
            public Nuclex.UserInterface.Controls.LabelControl nameLabel = new Nuclex.UserInterface.Controls.LabelControl();

            public Nuclex.UserInterface.Controls.Desktop.ButtonControl proceedButton = new Nuclex.UserInterface.Controls.Desktop.ButtonControl();
            //public GameButton clearButton = new GameButton();
            

            public GameDialog(string text, string title, UniRectangle bounds)
            {

                nameLabel.Text = text;
                this.EnableDragging = false;
                this.Title = title;
                this.Bounds = bounds;

                nameLabel.Bounds = new UniRectangle(new UniScalar(0.0f, 10f), new UniScalar(0.0f, 50.0f), 80, 24);
                //nameLabel.Text = "Incredibly useful information\n remove this by commenting out:\n \"gameScreen.Desktop.Children.Add(gameDialog);\" in the user class";
                
                proceedButton.Bounds = new UniRectangle(new UniScalar(0.75f,0), new UniScalar(0.85f,0), 100, 24);
                proceedButton.Text = "Proceed";


                Children.Add(nameLabel);
                Children.Add(proceedButton);
            }

        }
        
        
        public partial class SpeciesDialog : Nuclex.UserInterface.Controls.Desktop.WindowControl
        {

            
            // Nuclex.UserInterface.Controls.LabelControl header = new Nuclex.UserInterface.Controls.LabelControl();
            public Nuclex.UserInterface.Controls.LabelControl nameLabel = new Nuclex.UserInterface.Controls.LabelControl();
            public Nuclex.UserInterface.Controls.LabelControl colorLabel = new Nuclex.UserInterface.Controls.LabelControl();
            public Nuclex.UserInterface.Controls.LabelControl dietLabel = new Nuclex.UserInterface.Controls.LabelControl();
            public Nuclex.UserInterface.Controls.LabelControl sizeLabel = new Nuclex.UserInterface.Controls.LabelControl();
            public Nuclex.UserInterface.Controls.LabelControl speedLabel = new Nuclex.UserInterface.Controls.LabelControl();
            public Nuclex.UserInterface.Controls.LabelControl agilityLabel = new Nuclex.UserInterface.Controls.LabelControl();
            public Nuclex.UserInterface.Controls.LabelControl detectionLabel = new Nuclex.UserInterface.Controls.LabelControl();

            public Nuclex.UserInterface.Controls.Desktop.InputControl nameInput = new Nuclex.UserInterface.Controls.Desktop.InputControl();
            public Nuclex.UserInterface.Controls.Desktop.ListControl colorInput = new Nuclex.UserInterface.Controls.Desktop.ListControl();
            public Nuclex.UserInterface.Controls.Desktop.ListControl dietInput = new Nuclex.UserInterface.Controls.Desktop.ListControl();
            public Nuclex.UserInterface.Controls.Desktop.InputControl sizeInput = new Nuclex.UserInterface.Controls.Desktop.InputControl();
            public Nuclex.UserInterface.Controls.Desktop.InputControl speedInput = new Nuclex.UserInterface.Controls.Desktop.InputControl();
            public Nuclex.UserInterface.Controls.Desktop.InputControl agilityInput = new Nuclex.UserInterface.Controls.Desktop.InputControl();
            public Nuclex.UserInterface.Controls.Desktop.InputControl detectionInput = new Nuclex.UserInterface.Controls.Desktop.InputControl();

            //public Nuclex.UserInterface.Controls.Desktop.ButtonControl clearButton = new Nuclex.UserInterface.Controls.Desktop.ButtonControl();
            public GameButton clearButton = new GameButton();
            

            public SpeciesDialog()
            {
                nameLabel.Bounds = new UniRectangle(new UniScalar(0.0f, 10f), new UniScalar(0.0f, 50.0f), 80, 24);
                colorLabel.Bounds = new UniRectangle(new UniScalar(0.0f, 10f), new UniScalar(0.0f, 75.0f), 80, 24);
                dietLabel.Bounds = new UniRectangle(new UniScalar(0.0f, 10f), new UniScalar(0.0f, 100.0f), 80, 24);
                sizeLabel.Bounds = new UniRectangle(new UniScalar(0.0f, 10f), new UniScalar(0.0f, 125.0f), 80, 24);
                speedLabel.Bounds = new UniRectangle(new UniScalar(0.0f, 10f), new UniScalar(0.0f, 150.0f), 80, 24);
                agilityLabel.Bounds = new UniRectangle(new UniScalar(0.0f, 10f), new UniScalar(0.0f, 175.0f), 80, 24);
                detectionLabel.Bounds = new UniRectangle(new UniScalar(0.0f, 10f), new UniScalar(0.0f, 200.0f), 80, 24);
                
                
                
                nameInput.Bounds = new UniRectangle(new UniScalar(0.5f, 10f), new UniScalar(0.0f, 50.0f), 120, 24);
                colorInput.Bounds = new UniRectangle(new UniScalar(0.5f, 10f), new UniScalar(0.0f, 75.0f), 120, 24);
                dietInput.Bounds = new UniRectangle(new UniScalar(0.5f, 10f), new UniScalar(0.0f, 100.0f), 120, 24);
                sizeInput.Bounds = new UniRectangle(new UniScalar(0.5f, 10f), new UniScalar(0.0f, 125.0f), 120, 24);
                speedInput.Bounds = new UniRectangle(new UniScalar(0.5f, 10f), new UniScalar(0.0f, 150.0f), 120, 24);
                agilityInput.Bounds = new UniRectangle(new UniScalar(0.5f, 10f), new UniScalar(0.0f, 175.0f), 120, 24);
                detectionInput.Bounds = new UniRectangle(new UniScalar(0.5f, 10f), new UniScalar(0.0f, 200.0f), 120, 24);

                clearButton.Bounds = new UniRectangle(new UniScalar(0.5f, -5f), new UniScalar(0.0f, 235.0f), 100, 24);
                
                nameLabel.Text = "Species Name:";
                colorLabel.Text = "Color:";
                dietLabel.Text = "Diet:";
                sizeLabel.Text = "Size:";
                speedLabel.Text = "Speed:";
                agilityLabel.Text = "Agility:";
                detectionLabel.Text = "Detection:";



                colorInput.Items.Add("Red");
                colorInput.Items.Add("Green");
                colorInput.Items.Add("Blue");
                colorInput.Items.Add("Brown");
                colorInput.Items.Add("Orange");
                colorInput.SelectionMode = Nuclex.UserInterface.Controls.Desktop.ListSelectionMode.Single;

                dietInput.Items.Add("Herbivore");
                dietInput.Items.Add("Carnivore");
                dietInput.Items.Add("Omnivore");
                dietInput.SelectionMode = Nuclex.UserInterface.Controls.Desktop.ListSelectionMode.Single;

                nameInput.Text = "Kleemo";
                sizeInput.Text = "40";
                speedInput.Text = "5";
                agilityInput.Text = "0.15";
                detectionInput.Text = "150";

                clearButton.Text = "";

                this.Bounds = new UniRectangle(100.0f, 100.0f, 300.0f, 275.0f);

                Children.Add(nameLabel);
                Children.Add(colorLabel);
                Children.Add(dietLabel);
                Children.Add(sizeLabel);
                Children.Add(speedLabel);
                Children.Add(agilityLabel);
                Children.Add(detectionLabel);

                Children.Add(nameInput);
                Children.Add(colorInput);
                Children.Add(dietInput);
                Children.Add(sizeInput);
                Children.Add(speedInput);
                Children.Add(agilityInput);
                Children.Add(detectionInput);

                Children.Add(clearButton);
            }

        }

        //This is an extension of the button control with functionality necessary for this game
        public class GameButton : Nuclex.UserInterface.Controls.Desktop.ButtonControl
        {
            public Texture2D spriteSheet;
            public RectangleF baseSourceRect;
            public RectangleF hoverSourceRect;
            public RectangleF pressedSourceRect;

            public GameButton()
            {

            }

            public GameButton(UniRectangle bounds, Texture2D spriteSheet, RectangleF sourceRect)
            {
                this.Bounds = bounds;
                this.imageTexture = spriteSheet;
                this.baseSourceRect = sourceRect;
                this.sourceRect = sourceRect;
            }

            protected override void OnMouseEntered()
            {
                sourceRect = hoverSourceRect;
                base.OnMouseEntered();
            }

            protected override void OnMouseLeft()
            {
                sourceRect = baseSourceRect;
                base.OnMouseLeft();
            }

            /* protected override void OnMousePressed(MouseButtons button)
             {
                 base.OnMousePressed(button);
             }
                

             protected override void OnMouseReleased(MouseButtons button)
             {
                    
                 base.OnMousePressed(button);
             }*/
        }
    

     

    }
}
