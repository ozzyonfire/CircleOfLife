using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;



namespace CircleOfLife
{
    class Creature
    {
        //species characteristics
        public byte diet;
        private int size;
        public int detection;
        private int speed;
        private int energyCap;
        public int foodCap;
        private int waterCap;
        private int energyValue;
        public float agility;
        public Color color;
        public int stamina;

        //sprite location base
        Rectangle spriteRectangle;
        public Rectangle body;

        int mapHeight;
        int mapWidth;

        //creature state
        public byte state;
        public int food;
        private int water;
        public int energy;
        private float currSpeed;

        //Dead?
      //  public bool dead;      //when killed by natural causes(predator or starvation) and there is a corpse
       // public bool erased;    //when killed by a mormo or power and disappear nearly instantly
        public int foodValue;


        //creatures
        private Creature predator;
        private Creature prey;

        // flora
        public Environment flora;

        //position
        private Vector2 position;
        private float orientation;
        public Vector2 goalPosition;

        // timer
        TimeSpan deathtimer;
        public TimeSpan feedTimer;
        public TimeSpan sprintTime;
        public TimeSpan restTime;

        //Animations
        int frameOffset;

        //Random :}
        Random random = new Random();

        //accesors

        public Vector2 Position { get { return position; } }
        public int EnergyValue { get { return energyValue; } }
        public Creature Predator { get { return predator; } set { predator = value; } }
        public Creature Prey { get { return prey; } set { prey = value; } }

        //constructor
        public Creature(int xPos, int yPos, Ecosystem.speciesStats stats)
        {
            diet = stats.diet;
            switch (diet)
            {
                case 0:
                    spriteRectangle = Sprites.herbivore;
                    break;
                case 1:
                    spriteRectangle = Sprites.carnivore;
                    break;
                default:
                    spriteRectangle = Sprites.mormo;
                    break;
            }
            size = stats.size;
            detection = stats.detection;
            speed = stats.speed;
            energyCap = stats.energyCap;
            foodCap = stats.foodCap;
            waterCap = stats.waterCap;
            energyValue = stats.energyValue;
            agility = stats.agility;

            color = stats.color;

            position = new Vector2(xPos, yPos);
            goalPosition = new Vector2((float)random.NextDouble() * 1024f, (float)random.NextDouble() * 768f);
            orientation = new float();

            Rectangle body = new Rectangle(xPos, yPos, size, size);

            state = 0; //wander
            food = 0;
            water = waterCap;
            energy = energyCap;
            currSpeed = 1f;
            deathtimer = new TimeSpan(0, 0, 0);
            feedTimer = new TimeSpan(0, 0, 0);
            sprintTime = new TimeSpan(0, 0, 0);
            restTime = new TimeSpan(0, 0, 0);
            this.stamina = 6;

            this.mapWidth = 1920;   //hmm
            this.mapHeight = 1920;

            foodValue = size * 2;//Should be tweeked

            //animation offset
            frameOffset = random.Next(4);
        }

        public void update(GameTime gameTime)
        {
            if (state == 4)
                return;
            else if (state == 1) // chase
            {
                this.sprintTime += gameTime.ElapsedGameTime;
                Chase(position, ref prey, ref orientation, agility);
                Vector2 heading = new Vector2((float)Math.Cos(orientation), (float)Math.Sin(orientation));

                if (currSpeed < speed)
                {
                    currSpeed += 0.05f * currSpeed;
                }

                position += heading * currSpeed;
            }
            else if (state == 0) // wander
            {
                this.restTime += gameTime.ElapsedGameTime;
                if (flora != null && this.diet == 0)
                {
                    goalPosition = flora.position;
                }

                Wander(position, ref goalPosition, ref orientation, agility);
                Vector2 heading = new Vector2(
                   (float)Math.Cos(orientation), (float)Math.Sin(orientation));

                if (currSpeed > 0.25f * speed)
                {
                    currSpeed -= 0.05f * currSpeed;
                }

                position += heading * currSpeed;
            }
            else if (state == 2) // evade
            {
                this.sprintTime += gameTime.ElapsedGameTime;
                Evade(position, ref predator, ref orientation, agility);
                Vector2 heading = new Vector2(
                   (float)Math.Cos(orientation), (float)Math.Sin(orientation));

                if (currSpeed < speed)
                {
                    currSpeed += 0.05f * currSpeed;
                }
                position += heading * currSpeed;
            }

            deathtimer += gameTime.ElapsedGameTime;

            // remove energy
            if (deathtimer > TimeSpan.FromSeconds(1))
            {
                this.energy -= 5;
                if (energy < 0)
                {
                    // kill the creature
                    this.state = 4;
                }
                deathtimer = TimeSpan.Zero;
            }

            body.X = (int)position.X;
            body.Y = (int)position.Y;
            float w = this.size * 0.01f * spriteRectangle.Width;
            float h = this.size * 0.01f * spriteRectangle.Height;
            body.Width = (int)w;
            body.Height = (int)h;
        }

  

        private void Evade(Vector2 position, ref Creature pred, ref float orientation, float turnSpeed)
        {
            if (pred == null || pred.state == 4)
            {
                // it died
                this.state = 0;
                return;
            }
            Vector2 predPosition = pred.position; // bug here, because they can die now
            Vector2 seekPosition = 2 * position - predPosition; // optimal direction to run away (not very exciting)
            float distanceToGoal = Vector2.Distance(position, goalPosition);
            float distanceToPred = Vector2.Distance(position, pred.position);
            
            if (distanceToGoal < 300)
            {
                // assign a new random goal position
                randomGoal(mapWidth, mapHeight);
            }

            if (distanceToPred < 50)
            {
                // high priority choose optimal, can't have full turn speed
                orientation = TurnToFace(position, seekPosition, orientation, 0.20f * turnSpeed);
            }
            else
            {
                // choose random point to run to
                orientation = TurnToFace(position, goalPosition, orientation, 0.9f * turnSpeed);
            }
        }

        private void Chase(Vector2 position, ref Creature prey, ref float orientation, float turnSpeed)
        {
            if (prey == null || prey.state == 4)
            {
                this.state = 0;
                return;
            }
            Vector2 preyPosition = prey.position;
            
            // we may want to include a flocking algorithm so multiple predators dont get stuck behind the same prey
            /*
            if (prey.Predator != null)
            {
                Vector2 otherPredPosition = position - prey.Predator.position;
                orientation = TurnToFace(position, otherPredPosition, orientation, .5f * turnSpeed);
            }
            */

            orientation = TurnToFace(position, preyPosition, orientation, 0.75f * turnSpeed);
        }

        private void Wander(Vector2 position, ref Vector2 wanderDirection,
            ref float orientation, float turnSpeed)
        {
            float distanceFromGoal = Vector2.Distance(wanderDirection, position);
            if (distanceFromGoal < 200 && this.flora == null)
            {
                // new random goal position
                randomGoal(mapWidth, mapHeight);
            }

            orientation = TurnToFace(position, wanderDirection, orientation,
                .25f * turnSpeed);
        }

        public void randomGoal(int width, int height)
        {
            this.mapWidth = width;
            this.mapHeight = height;
            int i = 0;
            while (i == 0)
            {

                // new random goal position
                goalPosition.X = (float)random.Next(width);
                goalPosition.Y = (float)random.Next(height);

                Vector2 center = new Vector2(width / 2, height / 2);

                float minD = Math.Min(height / 2, width / 2);
                if (Vector2.Distance(goalPosition, center) < minD)
                {
                    i++;
                }
            }
            
        }

        public void avoid(Vector2 otherPosition)
        {
            // this is used to control crowding of creatures of the same type
            // works pretty well but a little choppy
            Vector2 seekPosition = position - otherPosition;

            seekPosition.Normalize();

            position += seekPosition * currSpeed;
        }

        public void turnToCenter(float distanceFromScreenCenter, Vector2 center, float maxDistance)
        {
            float normalizedDistance =
                distanceFromScreenCenter / maxDistance;

            float turnToCenterSpeed = .3f * normalizedDistance * normalizedDistance *
                this.agility;

            // once we've calculated how much we want to turn towards the center, we can
            // use the TurnToFace function to actually do the work.
            orientation = TurnToFace(position, center, orientation,
                turnToCenterSpeed);
        }

        /// <summary>
        /// Calculates the angle that an object should face, given its position, its
        /// target's position, its current angle, and its maximum turning speed.
        /// </summary>
        private static float TurnToFace(Vector2 position, Vector2 faceThis,
            float currentAngle, float turnSpeed)
        {

            float x = faceThis.X - position.X;
            float y = faceThis.Y - position.Y;

            float desiredAngle = (float)Math.Atan2(y, x);

            float difference = WrapAngle(desiredAngle - currentAngle);

            // clamp that between -turnSpeed and turnSpeed.
            difference = MathHelper.Clamp(difference, -turnSpeed, turnSpeed);

            return WrapAngle(currentAngle + difference);
        }

        /// <summary>
        /// Returns the angle expressed in radians between -Pi and Pi.
        /// <param name="radians">the angle to wrap, in radians.</param>
        /// <returns>the input value expressed in radians from -Pi to Pi.</returns>
        /// </summary>
        private static float WrapAngle(float radians)
        {
            while (radians < -MathHelper.Pi)
            {
                radians += MathHelper.TwoPi;
            }
            while (radians > MathHelper.Pi)
            {
                radians -= MathHelper.TwoPi;
            }
            return radians;
        }

        // mutator
        public void Feed(int value)
        {
            this.food += value;
        }



        public void draw(GameTime gameTime, SpriteBatch spriteBatch, Texture2D spriteSheet, Vector3 offset, int frame)
        {
            if (state == 4)
            {
                spriteRectangle.X = 400;
                spriteBatch.Draw(spriteSheet, new Vector2((int)(offset.Z * (position.X + offset.X)), (int)(offset.Z * (position.Y + offset.Y))), spriteRectangle, color, orientation, new Vector2(0), 0.01f * size * offset.Z, SpriteEffects.None, 0.9f);
            }
            else
            {
                spriteRectangle.X = 100 * ((frame + frameOffset) % 4);
                spriteBatch.Draw(spriteSheet, new Vector2((int)(offset.Z * (position.X + offset.X)), (int)(offset.Z * (position.Y + offset.Y))), spriteRectangle, color, orientation, new Vector2(0), 0.01f * size * offset.Z, SpriteEffects.None, 0.9f);
            }
        }     


    }
}
