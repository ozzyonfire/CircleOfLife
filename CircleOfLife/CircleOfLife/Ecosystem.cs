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
        private List<Species> species= new List<Species>(50);

        //Random :}
        Random random = new Random();

        public Ecosystem()
        {
        }
        
        public void addSpecies(String name, speciesStats stats, short x, short y)
        {
            //Check if valid input
            species.Add(new Species(name,stats,x,y));
        }

        //This is an ultra simplification of what the real system will be..kinda considering each species to be a unit
        //incredibly sloppy
        // we should iterate through all the creatures and perform the necessary tasks
        // so feed, check to reproduce, etc.
        public void update()
        {

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
                                break;
                            }

                            // check if species is prey or predator
                            // iterate through behaviour lists                

                            // check surroundings
                            float distanceAway = Vector2.Distance(species[i].Creatures[j].Position, species[k].Creatures[l].Position);

                            bool detected = false;
                            // random detection roll
                            // distanceAway, detection, random
                            if (distanceAway < 150)
                                detected = true;

                            if (detected)
                            {
                                // check state
                                if (species[i].Creatures[j].state == 0) // wandering
                                {
                                    if (species[k].Creatures[l].diet == 0) // temp
                                    {
                                        // start chasing
                                        species[i].Creatures[j].state = 1;
                                        species[i].Creatures[j].Prey = species[k].Creatures[l];
                                    }
                                    else if (species[k].Creatures[l].diet == 1) // temp
                                    {
                                        // start evading
                                        species[i].Creatures[j].state = 2;
                                        species[i].Creatures[j].Predator = species[k].Creatures[l];
                                    }
                                }
                                else if (species[i].Creatures[j].state == 1) // chasing
                                {
                                    if (species[i].Creatures[j].Prey == species[k].Creatures[l])
                                    {
                                        if (distanceAway < 30)
                                        {
                                            // killed it
                                            species[i].Creatures[j].state = 0;
                                            species[i].Creatures[j].Feed(species[i].Creatures[j].Prey.EnergyValue);
                                            species[k].Creatures.RemoveAt(l);
                                            l--;
                                        }
                                         // todo: feed
                                        // doing the feeding in here for now
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
                                    else if (species[k].Creatures[l].diet == 1)
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
                                }
                            }
                            else
                            {
                                // check state
                                if (species[i].Creatures[j].state == 0) // wandering
                                {
                                    // find food or water
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
                                }
                                else if (species[i].Creatures[j].state == 2) // evading
                                {
                                    if (species[i].Creatures[j].Predator == species[k].Creatures[l])
                                    {
                                        // evaded it
                                        species[i].Creatures[j].state = 0;
                                    }
                                    // check what it is evading
                                    // if its the target then it escaped set state to wander
                                }
                                else if (species[i].Creatures[j].state == 3) // feeding
                                {
                                    // add energyValue to food
                                    // keep eating!
                                    species[i].Creatures[j].state = 0;
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

            /*

                for (int i = 0; i < species.Count; i++)
                {
                    kill = false;
                    if (species[i].Stats.diet == 0) // herbivore
                    {
                        for (int j = 0; j < species.Count; j++)
                        {
                            float test = Vector2.Distance(species[i].Creatures[0].Position, species[j].Creatures[0].Position);
                            if (species[j].Stats.diet == 1 &&
                                Vector2.Distance(species[i].Creatures[0].Position, species[j].Creatures[0].Position) < 30)
                            {
                                kill = true;
                                // consume dead creature for food
                                species[j].Creatures[0].Feed(species[i].Creatures[0].EnergyValue);
                                break;
                            }
                        }
                        if (kill)
                        {
                            species.RemoveAt(i);
                            i--; //change index so species isn't skipped
                        }
                    }

                }
             */

            for (int i = 0; i < species.Count; i++)
            {
                species[i].update();
            }
        }

        public void draw(ref GraphicsDeviceManager graphics, ref SpriteBatch spriteBatch, ref Texture2D spriteSheet)
        {
            for (int i = 0; i < species.Count; i++)
            {
                species[i].draw(ref graphics, ref spriteBatch, ref spriteSheet);
            }
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
        }

    }
}
