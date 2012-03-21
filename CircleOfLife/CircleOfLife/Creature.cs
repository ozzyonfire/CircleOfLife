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

        //sprite location base
        Rectangle spriteRectangle;
        Rectangle body;

        //creature state
        public byte state;
        public int food;
        private int water;
        public int energy;
        private float currSpeed;

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
                    spriteRectangle = new Rectangle(0, 0, 100, 100);
                    break;
                case 1:
                    spriteRectangle = new Rectangle(100, 0, 100, 100);
                    break;
                default:
                    spriteRectangle = new Rectangle(0, 0, 100, 100);
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
        }

        public void update(GameTime gameTime)
        {
            if (state == 1) // chase
            {
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

        }

        public void draw(GameTime gameTime, SpriteBatch spriteBatch, Texture2D spriteSheet)
        {
            if (state == 1)
                spriteBatch.Draw(spriteSheet, position, spriteRectangle, color, orientation, new Vector2(0), 0.01f * size, SpriteEffects.None, 0.0f);
            else if (state == 2)
                spriteBatch.Draw(spriteSheet, position, spriteRectangle, color, orientation, new Vector2(0), 0.01f * size, SpriteEffects.None, 0.0f);
            else if (state == 3)
                spriteBatch.Draw(spriteSheet, position, spriteRectangle, color, orientation, new Vector2(0), 0.01f * size, SpriteEffects.None, 0.0f);
            else if (state == 4)
                spriteBatch.Draw(spriteSheet, position, spriteRectangle, color, orientation, new Vector2(0), 0.01f * size, SpriteEffects.None, 0.0f);
            else
                spriteBatch.Draw(spriteSheet, position, spriteRectangle, color, orientation, new Vector2(0), 0.01f * size, SpriteEffects.None, 0.0f);
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
                randomGoal();
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
                randomGoal();
            }

            orientation = TurnToFace(position, wanderDirection, orientation,
                .25f * turnSpeed);
        }

        public void randomGoal()
        {
            // new random goal position
            goalPosition.X = (float)random.NextDouble() * 1024;
            goalPosition.Y = (float)random.NextDouble() * 768;
        }

        public void avoid(Vector2 otherPosition)
        {
            // this is used to control crowding of creatures of the same type
            // works pretty well but a little choppy
            Vector2 seekPosition = position - otherPosition;

            seekPosition.Normalize();

            position += seekPosition * currSpeed;
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
    }
}
