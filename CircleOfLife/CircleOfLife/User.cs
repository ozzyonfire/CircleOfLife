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
    class User
    {
        Ecosystem ecosystem;    //reference to ecosystem class
        Game game;
        Viewport viewport;

        //Nuclex Stuff
        GuiManager gui;
        InputManager input;
        Nuclex.Game.States.GameStateManager state;  //?

        //devices
        Nuclex.Input.Devices.IKeyboard keyboard;
        Nuclex.Input.Devices.IMouse mouse;

        //temporary dialog
        SpeciesDialog speciesDialog;


        public User(Game game, Ecosystem ecosystem)
        {
            this.ecosystem = ecosystem;
            this.game = game;
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

            //meh
            Initialize();
        }

        public void Initialize()
        {
            //retrieve viewport
            viewport = game.GraphicsDevice.Viewport;
           
            Screen mainScreen = new Screen(viewport.Width, viewport.Height);
            speciesDialog = new SpeciesDialog();
            //set clear button listener
            speciesDialog.clearButton.Pressed += new EventHandler(clearButton_Pressed);
            mainScreen.Desktop.Children.Add(speciesDialog);
            gui.Screen = mainScreen;

            //listeners
            mouse.MouseMoved += new Nuclex.Input.Devices.MouseMoveDelegate(mouse_MouseMoved);
            mouse.MouseButtonReleased += new Nuclex.Input.Devices.MouseButtonDelegate(mouse_MouseButtonReleased);

            keyboard.KeyReleased += new Nuclex.Input.Devices.KeyDelegate(keyboard_KeyReleased);


        }




        public void Draw(GameTime gameTime)
        {
        }


        //EVENT HANDLERS:


        void clearButton_Pressed(object sender, EventArgs e)
        {
            ecosystem.clearEcosystem();
        }

        void mouse_MouseMoved(float x, float y)
        {
            //Navigation

        }

        void mouse_MouseButtonReleased(MouseButtons buttons)
        {
            MouseState ms = mouse.GetState();
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
                        preyStats.color = Color.White;
                        break;
                }
            else
                preyStats.color = Color.White;
            
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
            newSpecies.addCreature(ms.X, ms.Y);
        }


        void keyboard_KeyReleased(Keys key)
        {
            //Quit game when escape is pressed
            if (key.Equals(Keys.Escape))
                game.Exit();

            //
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

        public Nuclex.UserInterface.Controls.Desktop.ButtonControl clearButton = new Nuclex.UserInterface.Controls.Desktop.ButtonControl();


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

            sizeInput.Text = "20";
            speedInput.Text = "5";
            agilityInput.Text = "0.15";
            detectionInput.Text = "150";

            clearButton.Text = "Clear Species";

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
}
