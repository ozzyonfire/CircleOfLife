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
    class Ecosystem
    {
        //references
        Game game;

        private List<Species> species = new List<Species>(50);
        private List<Species> speciesTemp = new List<Species>(50);
        private List<Environment> flora = new List<Environment>(50);

        //Random :}
        Random random = new Random();

        //temporary variable for implementing map scrolling
        public Vector2 userView;
        public bool scrollLeft = false;
        public bool scrollRight = false;
        public bool scrollDown = false;
        public bool scrollUp = false;

        
        public Ecosystem(Game game)
        {
            //assign references
            this.game = game;
            //this.COL = (CircleOfLifeGame)game;
            userView = new Vector2(0, 0);
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

        public void addFlora(String name, Texture2D sprite, floraStats stats, short x, short y)
        {
            flora.Add(new Environment(name, sprite, stats.foodValue, stats.energyValue, stats.size, x, y));
        }


        //This is an ultra simplification of what the real system will be..kinda considering each species to be a unit
        //incredibly sloppy
        // we should iterate through all the creatures and perform the necessary tasks
        // so feed, check to reproduce, etc.
        public void  Update(GameTime gameTime)
{
            //Navigation scrolling section
            //the values need to be tuned to make scrolling smooth
            if(scrollLeft)
                userView.X += 3.0f * (gameTime.ElapsedGameTime.Ticks / 100000.0f);
            if (scrollRight)
                userView.X -= 3.0f * (gameTime.ElapsedGameTime.Ticks / 100000.0f);
            if (scrollUp)
                userView.Y += 3.0f * (gameTime.ElapsedGameTime.Ticks / 100000.0f);
            if (scrollDown)
                userView.Y -= 3.0f * (gameTime.ElapsedGameTime.Ticks / 100000.0f);



            species = speciesTemp;

             // detection is based on the hysteresis methods to make transitions smoother
            bool detected = false;
            bool pred = false;
            bool prey = false;
            bool same = false;

            // iterate through all species and all creatures for each species
            for (int i = 0; i < species.Count; i++) // go through all the species
            {
                for (int j = 0; j < species[i].Creatures.Count; j++) // go through the population of creatures for each species
                {
                    for (int k = 0; k < species.Count; k++)
                    {
                        for (int l = 0; l < species[k].Creatures.Count; l++)
                        {
                            if (species[i].Creatures[j] == species[k].Creatures[l])
                            {
                                // this is the creature
                                // FIXME: this causes a minor bug: if there is only 1 creature they wont do anything
                                break;
                            }

                            // check surroundings
                            float distanceAway = Vector2.Distance(species[i].Creatures[j].Position, species[k].Creatures[l].Position);

                            detected = false;
                            // random detection roll
                            // distanceAway, detection, random
                            // if it was already chasing something it wants to keep chasing
                            // if it was already evading it wants to get as far away as possible
                            if (species[i].Creatures[j].state == 2 && species[i].Creatures[j].Predator == species[k].Creatures[l])
                            {
                                // currently getting chased
                                if (distanceAway < species[i].Creatures[j].detection + 0.30 * species[i].Creatures[j].detection)
                                    detected = true;
                            }
                            else if (species[i].Creatures[j].state == 1 && species[i].Creatures[j].Prey == species[k].Creatures[l]) // fixme
                            {
                                // currently chasing something
                                if (distanceAway < species[i].Creatures[j].detection + 0.10 * species[i].Creatures[j].detection)
                                    detected = true;
                            }
                            else
                            {
                                if (distanceAway < species[i].Creatures[j].detection)
                                    detected = true;
                            }

                            // check if species is prey or predator
                            // iterate through behaviour lists   
                            pred = false;
                            prey = false;
                            if (species[i].predators.Contains(species[k]))
                            {
                                // its a predator
                                pred = true;
                            }
                            else if (species[i].prey.Contains(species[k]))
                            {
                                // its a prey
                                prey = true;
                            }
                            else if (species[i].name == species[k].name) 
                            {
                                same = true;
                            }

                            // avoid other creatures of the same type to prevent crowding, except when getting chased
                            if (same && distanceAway < 50 && species[i].Creatures[j].state != 2)
                            {
                                species[i].Creatures[j].avoid(species[k].Creatures[l].Position);
                            }

                            if (detected)
                            {
                                // check state
                                if (species[i].Creatures[j].state == 0) // wandering
                                {
                                    if (prey)
                                    {
                                        // start chasing
                                        species[i].Creatures[j].state = 1;
                                        species[i].Creatures[j].Prey = species[k].Creatures[l];
                                    }
                                    else if (pred)
                                    {
                                        // start evading
                                        species[i].Creatures[j].state = 2;
                                        species[i].Creatures[j].Predator = species[k].Creatures[l];
                                    }
                                    else if (species[i].Creatures[j].flora != null && species[i].Creatures[j].flora.state == 1)
                                    {
                                        // new random goal
                                        species[i].Creatures[j].flora = null;
                                        species[i].Creatures[j].randomGoal();
                                    }
                                }
                                else if (species[i].Creatures[j].state == 1) // chasing
                                {
                                    if (species[i].Creatures[j].Prey == species[k].Creatures[l]) // this is the target
                                    {
                                        if (distanceAway < 10) // change this to an intersection
                                        {
                                            // killed it
                                            species[i].Creatures[j].state = 0;
                                            species[i].Creatures[j].Feed(species[i].Creatures[j].Prey.EnergyValue);
                                            species[i].Creatures[j].energy += species[i].Creatures[j].Prey.EnergyValue;
                                            species[k].Creatures[l].state = 4; // dead
                                        }
                                         // todo: feed
                                        // doing the feeding in here for now
                                    }
                                    else if (prey)
                                    {
                                        float preyDistance = Vector2.Distance(species[i].Creatures[j].Position, species[i].Creatures[j].Prey.Position);
                                        if (distanceAway < preyDistance)
                                        {
                                            // new prey
                                            // not sure if we should have this or not? 
                                            // this makes it easier for prey to escape, but easier for pred to sneak up on prey
                                            species[i].Creatures[j].Prey = species[k].Creatures[l];
                                        }
                                    }
                                    // another creature could have killed the prey
                                    if (species[i].Creatures[j].Prey.state == 4)
                                    {
                                        species[i].Creatures[j].state = 0;
                                    }
                                }
                                else if (species[i].Creatures[j].state == 2) // evading
                                {
                                    // check what it is evading
                                    // compare it to target predator
                                    if (species[i].Creatures[j].Predator == species[k].Creatures[l])
                                    {
                                        // keep evading
                                    }
                                    else if (pred)
                                    {
                                        float predDistance = Vector2.Distance(species[i].Creatures[j].Position, species[i].Creatures[j].Predator.Position);
                                        if (distanceAway < predDistance)
                                        {
                                            // new predator
                                            species[i].Creatures[j].Predator = species[k].Creatures[l];
                                        }
                                    }

                                }
                                else if (species[i].Creatures[j].state == 3) // feeding
                                {
                                    // add energyValue to food
                                    // abandon food if predator
                                    if (pred)
                                    {
                                        // abandon food
                                        species[i].Creatures[j].state = 2;
                                    }
                                }
                            }
                            else // not detected
                            {
                                // check state
                                if (species[i].Creatures[j].state == 0) // wandering
                                {
                                    // find closest food or water
                                    if (species[i].Creatures[j].flora != null && species[i].Creatures[j].flora.state == 1)
                                    {
                                        // new random goal
                                        species[i].Creatures[j].flora = null;
                                        species[i].Creatures[j].randomGoal();
                                    }
                                    if (species[i].Creatures[j].diet == 0 && flora.Count > 0)
                                    {
                                        if (species[i].Creatures[j].flora == null)
                                        {
                                            species[i].Creatures[j].flora = flora[0];
                                        }
                                        float floraDistance = Vector2.Distance(species[i].Creatures[j].Position, species[i].Creatures[j].flora.position);

                                        if (floraDistance < 15)
                                        {
                                            // start feeding
                                            species[i].Creatures[j].state = 3;
                                        }

                                        for (int m = 0; m < flora.Count; m++)
                                        {
                                            float newDistance = Vector2.Distance(species[i].Creatures[j].Position, flora[m].position);
                                            if (newDistance < floraDistance)
                                            {
                                                // new target
                                                species[i].Creatures[j].flora = flora[m];
                                            }
                                        }
                                    }
                                    else if (species[i].Creatures[j].diet == 1)
                                    {
                                        // look for water
                                    }
                                }
                                else if (species[i].Creatures[j].state == 1) // chasing
                                {
                                    // check what its chasing
                                    // if it is the target creature then it killed it or evaded it
                                    // if it killed it set state to feed
                                    if (species[i].Creatures[j].Prey == species[k].Creatures[l])
                                    {
                                        // killed it or it escaped
                                        species[i].Creatures[j].state = 0;
                                    }
                                    // or another creature killed it, so it needs to stop chasing
                                    if (species[i].Creatures[j].Prey.state == 4)
                                    {
                                        // stop chasing
                                        species[i].Creatures[j].state = 0;
                                    }
                                }
                                else if (species[i].Creatures[j].state == 2) // evading
                                {
                                    // check what it is evading
                                    // if its the target then it escaped set state to wander
                                    if (species[i].Creatures[j].Predator == species[k].Creatures[l])
                                    {
                                        // evaded it
                                        species[i].Creatures[j].Predator = null;
                                        species[i].Creatures[j].state = 0;
                                    }
                                }
                                else if (species[i].Creatures[j].state == 3) // feeding
                                {
                                    // add energyValue to food
                                    // keep eating!
                                    if (species[i].Creatures[j].diet == 0 && species[i].Creatures[j].flora.state != 1)
                                    {
                                        // its eating a plant
                                        species[i].Creatures[j].Feed(species[i].Creatures[j].flora.foodValue);
                                        species[i].Creatures[j].flora.size--;
                                        species[i].Creatures[j].energy += species[i].Creatures[j].flora.foodValue;
                                        if (species[i].Creatures[j].flora.size <= 0)
                                        {
                                            // kill the plant
                                            species[i].Creatures[j].flora.state = 1;
                                        }
                                    }
                                    else if (species[i].Creatures[j].diet == 0 && species[i].Creatures[j].flora.state == 1)
                                    {
                                        // stop feeding look for more food
                                        species[i].Creatures[j].state = 0;
                                        species[i].Creatures[j].flora = null;
                                    }
                                }
                            }

                            // reproduction
                            // check food vs foodCap
                            if (species[i].Creatures[j].food >= species[i].Creatures[j].foodCap)
                            {
                                // reproduce
                                species[i].reproduce( species[i].Creatures[j] );
                                species[i].Creatures[j].food = 0;
                            }
                        }
                    }
                }
            }

            // update species and creatures
            for (int i = 0; i < species.Count; i++)
            {
                species[i].update(gameTime);
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
}

        public void rescanSpecies()
        {
            // this method iterates through the species and sets what is prey and what is predator
            for (int i = 0; i < species.Count; i++)
            {
                for (int j = 0; j < species.Count; j++)
                {
                    // check prey
                    if (species[i].Stats.size > species[j].Stats.size)
                    {
                        // it can feed on the other species
                        species[i].prey.Add(species[j]);
                    }
                    else if (species[i].Stats.size < species[j].Stats.size)
                    {
                        // it will be hunted by the other species
                        species[i].predators.Add(species[j]);
                    }
                    // if its equal then they are not predator nor prey
                }
            }
        }

        public void draw(GameTime gameTime, SpriteBatch spriteBatch, Texture2D spriteSheet)
        {

            // draw the species then the plants
            for (int i = 0; i < species.Count; i++)
            {
                species[i].draw(gameTime, spriteBatch, spriteSheet,userView);
            }
            for (int i = 0; i < flora.Count; i++)
            {
                //flora[i].draw(ref graphics, ref spriteBatch, ref flora[i].sprite);
            }


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
