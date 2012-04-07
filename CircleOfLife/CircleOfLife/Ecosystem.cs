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
    public class Ecosystem
    {
        //references
        Game game;
        CircleOfLifeGame baseGame;
        public Map map;

        public List<Species> species = new List<Species>(50);
        public List<Species> speciesTemp = new List<Species>(50);
        private List<Environment> flora = new List<Environment>(100);

        //Random :}
        Random random = new Random();


        public bool leakActive = false;
        public Vector2 leakPosition;

        public Ecosystem(Game game, int width, int height)
        {
            //assign references
            this.game = game;
            this.baseGame = (CircleOfLifeGame)game;
            map = new Map();
            map.intialize(width, height);

        }

        public Species addSpecies(String name, speciesStats stats)
        {
            //Check if valid input
            Species newSpecies = new Species(name, stats);
            speciesTemp.Add(newSpecies);
            rescanSpecies();

            return newSpecies;
        }

        public void addCreature(short n, short x, short y)
        {
            species[n].addCreature(x, y);
        }


        // we should iterate through all the creatures and perform the necessary tasks
        // so feed, check to reproduce, etc.
        public void Update(GameTime gameTime)
        {
            species = speciesTemp;
            // detection is based on the hysteresis methods to make transitions smoother
            bool detected = false;
            bool pred = false;

            // iterate through all species and all creatures for each species
            for (int i = 0; i < species.Count; i++) // go through all the species
            {
                for (int j = 0; j < species[i].Creatures.Count; j++) // go through the population of creatures for each species
                {
                    if (leakActive)
                    {
                        if (Vector2.Distance(species[i].Creatures[j].Position,baseGame.relativeToReal(leakPosition)) < 150)
                        {
                            species[i].Creatures[j].state = 6;
                        }
                    }
                    //Assess potential predators
                    for (int k = 0; k < species[i].predators.Count; k++)
                    {
                        for (int l = 0; l < species[i].predators[k].Creatures.Count; l++)
                        {
                            detected = false;
                            // check surroundings
                            float distanceAway = Vector2.Distance(species[i].Creatures[j].Position, species[i].predators[k].Creatures[l].Position);
                            // random detection roll
                            // distanceAway, detection, random
                            // if it was already chasing something it wants to keep chasing
                            // if it was already evading it wants to get as far away as possible
                            if (species[i].Creatures[j].state == 2 && species[i].Creatures[j].Predator == species[i].predators[k].Creatures[l])
                            {
                                // currently getting chased
                                if (distanceAway < species[i].Creatures[j].detection + 0.30 * species[i].Creatures[j].detection)    //if detected
                                    detected = true;
                            }
                            else if (distanceAway < species[i].Creatures[j].detection)
                                detected = true;

                            if (detected)
                            {
                                // check state
                                if (species[i].Creatures[j].state == 0 || species[i].Creatures[j].state == 1 || species[i].Creatures[j].state == 3) // wandering, feeding, or chasing
                                {
                                    species[i].Creatures[j].state = 2;
                                    species[i].Creatures[j].Predator = species[i].predators[k].Creatures[l];
                                }

                                else if (species[i].Creatures[j].state == 2) // evading
                                {
                                    // check what it is evading
                                    // compare it to target predator
                                    if (species[i].Creatures[j].Predator == species[i].predators[k].Creatures[l])
                                    {
                                        // keep evading
                                    }
                                    else if (pred && species[i].Creatures[j].Predator != null) // hopefully this will fix the crash
                                    {
                                        float predDistance = Vector2.Distance(species[i].Creatures[j].Position, species[i].Creatures[j].Predator.Position); // still crashing here
                                        if (distanceAway < predDistance)
                                        {
                                            // new predator
                                            species[i].Creatures[j].Predator = species[i].predators[k].Creatures[l];
                                        }
                                    }

                                    //????????????????????????????????????????random goal??


                                }
                            }
                                                        
                            
                            else if (species[i].Creatures[j].state == 2 && species[i].Creatures[j].Predator.Prey != species[i].Creatures[j] && species[i].Creatures[j].state != 1) // evading and not detected
                            {         
                                // this caused a lot of bugs - fixed
                                species[i].Creatures[j].state = 0;
                                species[i].Creatures[j].flora = null;
                            }
                            
                            
                        }

                    }




                    //TODO: break if being chased (or thirsty?)
                   // if (species[i].Creatures[j].state == 2)
                   //    break;



                    if (species[i].Creatures[j].diet == 1)   //assess potential prey if carnivore
                    {
                        for (int k = 0; k < species[i].prey.Count; k++)
                        {
                            for (int l = 0; l < species[i].prey[k].Creatures.Count; l++)    //Cycle through prey
                            {
                                detected = false;
                                // check surroundings
                                float distanceAway = Vector2.Distance(species[i].Creatures[j].Position, species[i].prey[k].Creatures[l].Position);
                                // random detection roll
                                // distanceAway, detection, random
                                // if it was already chasing something it wants to keep chasing
                                // if it was already evading it wants to get as far away as possible

                                if (species[i].Creatures[j].state == 1 && species[i].Creatures[j].Prey == species[i].prey[k].Creatures[l]) // fixme
                                {
                                    // currently chasing something
                                    if (distanceAway < species[i].Creatures[j].detection + 0.30 * species[i].Creatures[j].detection)
                                        detected = true;
                                    else
                                        species[i].Creatures[j].state = 0;
                                }

                                else if (distanceAway < species[i].Creatures[j].detection)
                                    detected = true;


                                if (detected)
                                {
                                    if (species[i].Creatures[j].state == 1) // chasing
                                    {
                                        if (species[i].Creatures[j].Prey == species[i].prey[k].Creatures[l]) // this is the target
                                        {
                                            //Check if killed
                                            float predDistance = Vector2.Distance(species[i].Creatures[j].Position, species[i].Creatures[j].Prey.Position);
                                            if (predDistance < species[i].Creatures[j].height / 2)
                                            {
                                                // killed it
                                                species[i].Creatures[j].state = 3;
                                                //species[i].Creatures[j].Feed(species[i].Creatures[j].Prey.EnergyValue);
                                                //species[i].Creatures[j].energy += species[i].Creatures[j].Prey.EnergyValue;
                                                species[i].prey[k].Creatures[l].state = 4; // dead
                                            }
                                            // todo: switch to feed state
                                            // doing the feeding in here for now
                                        }
                                        else
                                        {
                                            //Priority prey system?? Easier or more valuable prey have higher prioirity?
                                            float preyDistance = Vector2.Distance(species[i].Creatures[j].Position, species[i].Creatures[j].Prey.Position);
                                            if (distanceAway < preyDistance)
                                            {
                                                // new prey
                                                // not sure if we should have this or not? 
                                                // this makes it easier for prey to escape, but easier for pred to sneak up on prey
                                                species[i].Creatures[j].Prey = species[i].prey[k].Creatures[l];
                                            }
                                        }
                                        // another creature could have killed the prey
                                        if (species[i].Creatures[j].Prey.state == 4)
                                        {
                                            //TODO: switch to feed state instead of wanderking off
                                            //Allow creature to finish chasing so its close to corpse before feeding
                                            //species[i].Creatures[j].state = 3;
                                        }
                                    }

                                    else if (species[i].Creatures[j].state == 0) //wandering
                                    {
                                        // start chasing
                                        species[i].Creatures[j].Prey = species[i].prey[k].Creatures[l];
                                        if(species[i].prey[k].Creatures[l].state != 4)
                                            species[i].Creatures[j].state = 1;
                                    }
                                }

                                //Additional loop for interacting with non interacting species?
                            }
                        }
                    }

                    // if wandering loop through members of same species so they don't overlap
                    //TODO: too much computation? further avoidance necessary?
                    /*
                    if (species[i].Creatures[j].state == 0 || species[i].Creatures[j].state == 3)   //feeding?
                    {
                        for (int k = 0; k < species[i].Creatures.Count; k++)
                        {
                            if (k != j)
                                if (Vector2.Distance(species[i].Creatures[j].Position, species[i].Creatures[k].Position) < 10)
                                {
                                    species[i].Creatures[j].avoid(species[i].Creatures[k].Position);
                                }
                        }
                    }
                    */

                    if (species[i].Creatures[j].diet == 0 && species[i].Creatures[j].state == 0) //if wandering herbivore, look for plants
                    {

                        float newDistance;
                        float floraDistance;
                        if (species[i].Creatures[j].flora == null)
                        {
                            // we only want this on the initial one
                            //species[i].Creatures[j].flora = flora[random.Next(flora.Count)]; // try this for now
                            
                            // it would be cool to get them to choose a random flower in the crop

                            species[i].Creatures[j].flora = flora[0];

                            floraDistance = Vector2.Distance(species[i].Creatures[j].Position, species[i].Creatures[j].flora.position);
                            for (int k = 0; k < flora.Count; k++)
                            {
                                //new distance has a 400px variance so that different creatures behave differently
                                newDistance = Vector2.Distance(species[i].Creatures[j].Position, flora[k].position);
                                //Console.WriteLine(newDistance.ToString());
                                //newDistance += random.Next(-400, 400); 

                                //new target if closer, RANDOM offset added to create some variety and stop creatures travelling in a horde
                                if (newDistance < floraDistance)
                                {
                                    species[i].Creatures[j].flora = flora[k];
                                    floraDistance = newDistance;
                                }

                            }

                            // choose random plant from that crop
                            species[i].Creatures[j].flora = species[i].Creatures[j].flora.theCrop.plants[random.Next(species[i].Creatures[j].flora.theCrop.plants.Count)];
                        }
                       // Console.WriteLine(i.ToString() + ", " + j.ToString() + ": " + Vector2.Distance(species[i].Creatures[j].Position, species[i].Creatures[j].flora.position).ToString());
                    }

                    //Not detected

                    //TODO: Some of the not detected code is redundant or belongs above and should be changed in future

                    // check state
                    if (species[i].Creatures[j].state == 0) // wandering
                    {
                        // find closest food or water
                        if (species[i].Creatures[j].flora != null && species[i].Creatures[j].flora.state == 1)
                        {
                            // new random goal
                            species[i].Creatures[j].flora = null;
                            species[i].Creatures[j].randomGoal(map.width, map.height);
                        }
                        if (species[i].Creatures[j].Prey != null && species[i].Creatures[j].Prey.state == 4)
                        {
                            // new random goal
                            species[i].Creatures[j].Prey = null;
                            species[i].Creatures[j].randomGoal(map.width, map.height);
                        }
                        if (species[i].Creatures[j].diet == 0 && flora.Count > 0)
                        {
                            //What ??????????????????????? (prevents a bug from trying to find a null flora position)
                            if (species[i].Creatures[j].flora == null)
                            {
                                species[i].Creatures[j].flora = flora[0];
                            }
                            float floraDistance = Vector2.Distance(species[i].Creatures[j].Position, species[i].Creatures[j].flora.position);

                            if (floraDistance < species[i].Creatures[j].flora.width / 2 + 10)
                            {
                                // start feeding
                                // start the feeding timer
                                species[i].Creatures[j].feedTimer = TimeSpan.Zero;
                                species[i].Creatures[j].state = 3;
                            }

                           // species[i].Creatures[j].Prey = null;
                        }
                    }
                    else if (species[i].Creatures[j].state == 3) // feeding
                    {
                        // if the feed timer allows it, eat
                        species[i].Creatures[j].feedTimer += gameTime.ElapsedGameTime;
                        // add energyValue to food
                        // keep eating!
                        if (species[i].Creatures[j].feedTimer > TimeSpan.FromSeconds(1)) // feeding timer
                        {
                            species[i].Creatures[j].feedTimer = TimeSpan.Zero;
                            if (species[i].Creatures[j].diet == 0 && species[i].Creatures[j].flora.state != 1)
                            {
                                // its eating a plant
                                species[i].Creatures[j].Feed(species[i].Creatures[j].flora.foodValue);
                                species[i].Creatures[j].flora.size--;
                                species[i].Creatures[j].energy += species[i].Creatures[j].flora.foodValue;
                                //TODO: better position calculations
                                string points = (species[i].Creatures[j].flora.foodValue.ToString());
                                baseGame.oPoints += species[i].Creatures[j].flora.foodValue;
                                baseGame.user.effects.addFloatingString(points, baseGame.realToRelative(species[i].Creatures[j].Position - Vector2.UnitY * 20), Color.DarkGreen);
                                if (species[i].Creatures[j].flora.size <= 0)
                                {
                                    // kill the plant
                                    species[i].Creatures[j].flora.state = 1;
                                    species[i].Creatures[j].state = 0;
                                    species[i].Creatures[j].flora = null;
                                }
                            }
                            else if (species[i].Creatures[j].diet == 0 && species[i].Creatures[j].flora.state == 1)
                            {
                                // stop feeding look for more food
                                species[i].Creatures[j].state = 0;
                                species[i].Creatures[j].flora = null;
                            }
                            //carnivore feeding
                            else if (species[i].Creatures[j].diet == 1 && species[i].Creatures[j].Prey.state == 4)
                            {
                                species[i].Creatures[j].Feed(1);
                                species[i].Creatures[j].energy += 15;
                                species[i].Creatures[j].Prey.foodValue -= 5;
                                baseGame.oPoints += 15;
                                baseGame.user.effects.addFloatingString("5", baseGame.realToRelative(species[i].Creatures[j].Position - Vector2.UnitY * 20), Color.Red);
                            }
                            else //wander away
                                species[i].Creatures[j].state = 0;
                        }
                    }

                    float distanceToCenter = Vector2.Distance(species[i].Creatures[j].Position, map.center);
                    // avoid the boundary
                    if (distanceToCenter > (map.height / 2 - 100))
                    {
                        // the max distance will be the height of the map
                        species[i].Creatures[j].turnToCenter(distanceToCenter, map.center, Math.Min(map.height, map.width));
                    }


                    // reduce energy if chasing or evading
                    if (species[i].Creatures[j].state == 1 || species[i].Creatures[j].state == 2)
                    {
                        if (species[i].Creatures[j].sprintTime > TimeSpan.FromSeconds(species[i].Creatures[j].stamina))
                        {
                            // stop sprinting
                            species[i].Creatures[j].state = 0;
                        }
                    }
                    else if (species[i].Creatures[j].state == 0)
                    {
                        if (species[i].Creatures[j].restTime > TimeSpan.FromSeconds(species[i].Creatures[j].stamina / 2))
                        {
                            // allowed to sprint again
                            species[i].Creatures[j].sprintTime = TimeSpan.Zero;
                            species[i].Creatures[j].restTime = TimeSpan.Zero;
                        }
                    }


                    // reproduction
                    // check food vs foodCap
                    if (species[i].Creatures[j].food >= species[i].Creatures[j].foodCap)
                    {
                        // reproduce
                        species[i].reproduce(species[i].Creatures[j]);
                        species[i].Creatures[j].food = 0;
                        //up points
                        baseGame.ePoints += 1;
                    }
                }
            }
            // update species and creatures
            for (int i = 0; i < species.Count; i++)
            {
                species[i].update(gameTime);
                if (species[i].Creatures.Count == 0)
                {
                    species.RemoveAt(i);
                }
            }

            // update flora
            for (int i = 0; i < flora.Count; i++)
            {

                if (flora[i].state == 1)
                {
                    // dead
                    flora.RemoveAt(i);
                    i--;
                }
            }

            // update map
            map.update(gameTime);

            // add flora
            flora.Clear();
            for (int i = 0; i < map.crops.Count; i++)
            {
                for (int j = 0; j < map.crops[i].plants.Count; j++)
                {
                    if ((leakActive && Vector2.Distance(map.crops[i].plants[j].position, baseGame.relativeToReal(leakPosition)) < 150))
                        map.crops[i].plants[j].state = 1;
                    flora.Add(map.crops[i].plants[j]);
                }
            }

            if (leakActive)
            {
                leakActive = false;
            }

        }
                
            
            


        public void rescanSpecies()
        {
            // this method iterates through the species and sets what is prey and what is predator
            for (int i = 0; i < species.Count; i++)
            {
                species[i].predators.Clear();
                species[i].prey.Clear();
                for (int j = 0; j < species.Count; j++)
                {
                    if (species[i] != species[j])
                    {
                        // check prey
                        if (species[i].Stats.size > species[j].Stats.size && species[i].Stats.diet == 1)
                        {
                            // it can feed on the other species
                            species[i].prey.Add(species[j]);
                        }
                        else if (species[i].Stats.size < species[j].Stats.size && species[j].Stats.diet == 1)
                        {
                            // it will be hunted by the other species
                            species[i].predators.Add(species[j]);
                        }
                        else if (species[i].Stats.size == species[j].Stats.size && species[i].Stats.diet == 1 && species[j].Stats.diet == 0)
                        {
                            // it can feed on the herbivore
                            species[i].prey.Add(species[j]);
                        }
                        else if (species[i].Stats.size == species[j].Stats.size && species[j].Stats.diet == 1 && species[i].Stats.diet == 0)
                        {
                            // it can be fed on by the carnivore
                            species[i].predators.Add(species[j]);
                        }
                        // if its equal then they are not predator nor prey
                    }
                }
            }
        }

        public void reposition(float offset)
        {

            for (int i = 0; i < species.Count; i++)
            {
                for (int j = 0; j < species[i].Creatures.Count; j++)
                {
                    species[i].Creatures[j].position.X -= offset;
                    species[i].Creatures[j].position.Y -= offset;
                }
            }


            for (int i = 0; i < map.crops.Count; i++)
            {
                for (int j = 0; j < map.crops[i].plants.Count; j++)
                {
                    map.crops[i].plants[j].position.X -= offset;
                    map.crops[i].plants[j].position.Y -= offset;
                }
            }
            //for (int i = 0; i < flora.Count; i++)
			//{
                //   flora[i].position.X -= offset;
                //   flora[i].position.Y -= offset;
			    
			//}

        }


        public void draw(GameTime gameTime, SpriteBatch spriteBatch, Texture2D spriteSheet, Vector3 offset)
        {
            //frame calculation is done here for the time being
            int frame = ((int)Math.Round(gameTime.TotalGameTime.TotalMilliseconds / 150)) % 4;

            // draw the species then the plants
            for (int i = 0; i < species.Count; i++)
            {
                species[i].draw(gameTime, spriteBatch, spriteSheet, offset, frame);
            }
            // draw map
            map.draw(gameTime, spriteBatch, spriteSheet, offset, 0);
        }

        public void clearEcosystem()
        {
            speciesTemp.Clear();
        }

        // static functions used to model the ecosystems behaviour



        // Species characterists struct..should be moved somewhere else!
        public struct speciesStats
        {
            public byte diet; //0 Herbivore, 1 Carnivore, 2 Omnivore
            public short size;
            public short detection;
            public short speed;
            public short energyCap;
            public short foodCap;
            public short waterCap;
            public short energyValue; // this is how much the creature is "worth" when it is turned into food
            public float agility;
            public Color color;
        }

        public struct floraStats
        {
            public short foodValue;
            public short size;
            public short energyValue;
        }

    }
}
