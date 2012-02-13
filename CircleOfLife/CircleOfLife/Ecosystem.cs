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
            //temporary method for killing off species
            bool kill;

            // iterate through all species and all creatures for each species

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

        //static functions used to model the ecosystems behaviour



        //Species characterists struct..should be moved somewhere else!
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
