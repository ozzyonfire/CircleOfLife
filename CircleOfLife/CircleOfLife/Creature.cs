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
        private short size;
        public short detection;
        private short speed;
        private short energyCap;
        public short foodCap;
        private short waterCap;
        private short energyValue;
        private float agility;

        //creature state
        public byte state;    //temporary way of doing this
        public short food;
        private short water;
        private short energy;
        private float currSpeed;

        //creatures
        private Creature predator;
        private Creature prey;

        //position
        private Vector2 position;
        private float orientation;
        private Vector2 goalPosition;


        //Random :}
        Random random = new Random();

        //accesors

        public Vector2 Position { get { return position; } }
        public short EnergyValue { get { return energyValue; } }
        public Creature Predator { get { return predator; } set { predator = value; } }
        public Creature Prey { get { return prey; } set { prey = value; } }

        //constructor
        public Creature(short xPos, short yPos, Ecosystem.speciesStats stats)
        {
            diet = stats.diet;
            size = stats.size;
            detection = stats.detection;
            speed = stats.speed;
            energyCap = stats.energyCap;
            foodCap = stats.foodCap;
            waterCap = stats.waterCap;
            energyValue = stats.energyValue;
            agility = stats.agility;

            position = new Vector2(xPos, yPos);
            goalPosition = new Vector2((float)random.NextDouble() * 1024f, (float)random.NextDouble() * 768f);
            orientation = new float();

            state = 0; //wander
            food = 0;
            water = waterCap;
            energy = energyCap;
            currSpeed = 1f;
        }

        public void update()
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

        }

        public void draw(ref GraphicsDeviceManager graphics, ref SpriteBatch spriteBatch, ref Texture2D spriteSheet)
        {
            if(diet==1)
                spriteBatch.Draw(spriteSheet, position, null, Color.Red, orientation, new Vector2(0),0.3f,SpriteEffects.None,0.0f);
            else
                spriteBatch.Draw(spriteSheet, position, null, Color.White, orientation, new Vector2(0), 0.3f, SpriteEffects.None, 0.0f);

        }

        private void Evade(Vector2 position, ref Creature pred, ref float orientation, float turnSpeed)
        {
            Vector2 predPosition = pred.position;
            Vector2 seekPosition = 2 * position - predPosition; // optimal direction to run away (not very exciting)
            float distanceToGoal = Vector2.Distance(position, goalPosition);
            float distanceToPred = Vector2.Distance(position, pred.position);
            
            if (distanceToGoal < 300)
            {
                // assign a new random goal position
                goalPosition.X = (float)random.NextDouble() * 1024f;
                goalPosition.Y = (float)random.NextDouble() * 768f;
            }

            if (distanceToPred < 50)
            {
                // high priority choose optimal, can't have full turn speed
                orientation = TurnToFace(position, seekPosition, orientation, 0.20f * turnSpeed);
            }
            else
            {
                // choose random point to run to
                orientation = TurnToFace(position, goalPosition, orientation, turnSpeed);
            }
        }

        private void Chase(Vector2 position, ref Creature prey, ref float orientation, float turnSpeed)
        {
            Vector2 preyPosition = prey.position;
            orientation = TurnToFace(position, preyPosition, orientation, .6f * turnSpeed);
        }

        private void Wander(Vector2 position, ref Vector2 wanderDirection,
            ref float orientation, float turnSpeed)
        {
            float distanceFromGoal = Vector2.Distance(wanderDirection, position);
            if (distanceFromGoal < 10)
            {
                // new random goal position
                wanderDirection.X = (float)random.NextDouble() * 1024;
                wanderDirection.Y = (float)random.NextDouble() * 768;
                //Console.WriteLine("new goal: x {0} y {1}", goalPosition.X, goalPosition.Y);
            }

            // makes them a little inaccurate
            wanderDirection.X +=
                MathHelper.Lerp(-20f, 20f, (float)random.NextDouble());
            wanderDirection.Y +=
                MathHelper.Lerp(-20f, 20f, (float)random.NextDouble());

            /*
            // we'll renormalize the wander direction, ...
            if (wanderDirection != Vector2.Zero)
            {
                wanderDirection.Normalize();
            }
            */

            orientation = TurnToFace(position, wanderDirection, orientation,
                .05f * turnSpeed);

            /*
            // next, we'll turn the characters back towards the center of the screen, to
            // prevent them from getting stuck on the edges of the screen.
            Vector2 screenCenter = Vector2.Zero;
            screenCenter.X = 512;   //hard coded for now
            screenCenter.Y = 384;
            

            float distanceFromScreenCenter = Vector2.Distance(screenCenter, position);
            if (distanceFromScreenCenter > 500)
            {
                float MaxDistanceFromScreenCenter =
                    Math.Min(screenCenter.Y, screenCenter.X);

                float normalizedDistance =
                    distanceFromScreenCenter / MaxDistanceFromScreenCenter;

                float turnToCenterSpeed = .25f * normalizedDistance * normalizedDistance *
                    turnSpeed;

                // once we've calculated how much we want to turn towards the center, we can
                // use the TurnToFace function to actually do the work.
                orientation = TurnToFace(position, screenCenter, orientation,
                    turnToCenterSpeed);

            }
             */
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
        public void Feed(short value)
        {
            this.food += value;
        }
    }
}
