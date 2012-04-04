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

        //temporary dialog
        SpeciesDialog speciesDialog;
        GameDialog gameDialog;


        Rectangle hudBackground = Sprites.hudBackground;
        Rectangle hudDestination;

        PerkTree perkTree;
        public Effects effects;

        Nuclex.UserInterface.Controls.Desktop.HorizontalSliderControl menuSlider;
        Nuclex.UserInterface.Controls.Desktop.ListControl speciesList;
        Nuclex.UserInterface.Controls.LabelControl description;

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

            speciesDialog = new SpeciesDialog();
            //set clear button listener
            speciesDialog.clearButton.Pressed += new EventHandler(clearButton_Pressed);
            gameScreen.Desktop.Children.Add(speciesDialog);


            //GameUI gameUI = new GameUI(gameScreen, baseGame.spriteSheet);
            gui.Screen = gameScreen;
            
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
                speciesList.Items.Add(ecosystem.species[i].name);
            gui.Screen = menuScreen;
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
            spriteBatch.DrawString(gameFonts.Header, "Score: 1337", new Vector2(viewport.Width * 0.9f, 25), Color.Magenta);

            drawMouse(spriteBatch, spriteSheet);
        }

        public void drawMenu(GameTime gameTime, SpriteBatch spriteBatch, Texture2D spriteSheet, CircleOfLifeGame.GameFonts gameFonts)
        {
            //Draw menu background
            spriteBatch.Draw(spriteSheet, new Rectangle(0, 0, viewport.Width, viewport.Height), new Rectangle(0, 1050, 1, 1), Color.White, 0.0f, Vector2.Zero, SpriteEffects.None, 1.0f);
            spriteBatch.Draw(spriteSheet, new Rectangle(0, 0, viewport.Width, viewport.Height), new Rectangle(100, 100, 800, 800), Color.White, 0.0f, Vector2.Zero, SpriteEffects.None, 0.1f);
            //Draw menu Title
            spriteBatch.DrawString(gameFonts.Title, "Game Menu", new Vector2(viewport.Width * 0.45f, viewport.Height * 0.05f), new Color(255,0,0,128));
            perkTree.draw(gameTime, spriteBatch, spriteSheet, gameFonts);

            drawMouse(spriteBatch, spriteSheet);
        }
        

        public void drawMouse(SpriteBatch spriteBatch, Texture2D spriteSheet)
        {
            MouseState ms = mouse.GetState();
            Rectangle mouseRect;
            if(baseGame.scrollRight)
                mouseRect = new Rectangle(1100,300,100,100);
            else if (baseGame.scrollLeft)
                mouseRect = new Rectangle(1200,300,100,100);
            else if (baseGame.scrollUp)
                mouseRect = new Rectangle(1300,300,100,100);
            else if (baseGame.scrollDown)
                mouseRect = new Rectangle(1400, 300, 100, 100);
            else if (baseGame.menuOpen)
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


            //if in menu mode
            perkTree.mouseOver((int)x, (int)y);
        }

        void mouse_MouseButtonReleased(MouseButtons buttons)
        {
            MouseState ms = mouse.GetState();


            //return if in menu state
            if (baseGame.menuOpen)
            {
                perkTree.mouseClick(ms.X, ms.Y);
            }
            else
            {
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
            }
        }

        void mouse_MouseWheelRotated(float ticks)
        {
           /* if (baseGame.menuOpen)
            {
                //scroll perk tree if mouse is in that area
                //TODO check mouse location
                //TODO adjust scroll bar
                int newOffset = perkTree.offset - (int)ticks*9;
                if (newOffset < 0)
                    perkTree.offset = 0;
                else
                    perkTree.offset = newOffset;

            }
            else
            {

            }*/
            MouseState ms = mouse.GetState();
                
            //mouse centered zoom!
            baseGame.userView = new Vector3((baseGame.userView.X - ms.X + viewport.Width / 2) * (baseGame.userView.Z + 0.1f * ticks), (baseGame.userView.Y + ms.Y - viewport.Height / 2) * (baseGame.userView.Z + 0.1f * ticks), baseGame.userView.Z + 0.1f * ticks);
            //baseGame.userView = new Vector3(baseGame.userView.X, baseGame.userView.Y, baseGame.userView.Z + 0.1f * ticks);

        }

        void keyboard_KeyReleased(Keys key)
        {
            //Quit game when escape is pressed
            if (key.Equals(Keys.Escape))
                game.Exit();

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
                baseGame.mapSizeX = baseGame.mapSizeX + 50;
                baseGame.mapSizeY = baseGame.mapSizeY + 50;
            }
            //debug map contraction
            if (key.Equals(Keys.Q))
            {
                baseGame.mapSizeX = baseGame.mapSizeX - 50;
                baseGame.mapSizeY = baseGame.mapSizeY - 50;
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

        void drawPerview(Rectangle destination)
        {
            //the art for this class not yet available
            
        }



        //The following builds and initializes the games gui's seperated from the user class mainly for organizational purposes
        public void initializeGameScreen()
        {
            GameButton torchButton = new GameButton(new UniRectangle(
                    new UniScalar(0.0f, 0.0f),
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
                gameScreen.Desktop.Children.Add(torchButton);
        }

        void torchButton_Pressed(object sender, EventArgs e)
        {
            enterMenu();
        }


        public void initializeMenuScreen()
        {
            perkTree = new PerkTree();
            effects = new Effects();

            //initialize elements
            speciesList = new Nuclex.UserInterface.Controls.Desktop.ListControl();
            Nuclex.UserInterface.Controls.Desktop.ButtonControl createButton = new Nuclex.UserInterface.Controls.Desktop.ButtonControl();
            //menuSlider = new Nuclex.UserInterface.Controls.Desktop.HorizontalSliderControl();
            description = new Nuclex.UserInterface.Controls.LabelControl();

            speciesList.Bounds = new UniRectangle(new UniScalar(0.0f, 50.0f),new UniScalar(0.0f, 200.0f),new UniScalar(0.0f, 300.0f),new UniScalar(0.0f, 300.0f));
            speciesList.SelectionMode = Nuclex.UserInterface.Controls.Desktop.ListSelectionMode.Single;

            createButton.Bounds = new UniRectangle(new UniScalar(0.0f, 50.0f),new UniScalar(0.0f, 505.0f),new UniScalar(0.0f, 300.0f),new UniScalar(0.0f, 50.0f));
            createButton.Text = "New Species";

            //menuSlider.Bounds = new UniRectangle(new UniScalar(0.3f, 0), new UniScalar(0.75f, 0),new UniScalar(0.5f,0), new UniScalar(0.0f, 30));
            //menuSlider.ThumbSize = 0.1f;
            //menuSlider.Moved += new EventHandler(perkSliderMoved);

            description.Bounds = new UniRectangle(new UniScalar(0.3f, 0), new UniScalar(0.8f, 0), new UniScalar(0.5f, 0), new UniScalar(0.2f, 0));
            description.Text = Sprites.description;

            //menuScreen.Desktop.Children.Add(menuSlider);
            //menuScreen.Desktop.Children.Add(speciesList);
            //menuScreen.Desktop.Children.Add(createButton);
            menuScreen.Desktop.Children.Add(description);
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
