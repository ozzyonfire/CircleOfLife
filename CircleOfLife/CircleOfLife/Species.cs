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
    public class Species
    {
        public string name;
        private short population;
        private Ecosystem.speciesStats stats;
        private List<Creature> tempCreatures = new List<Creature>(100);
        private List<Creature> creatures = new List<Creature>(100);
        public List<Species> predators = new List<Species>(10);
        public List<Species> prey = new List<Species>(10);

        //accesors
        public Ecosystem.speciesStats Stats { get { return stats; } }
        public List<Creature> Creatures { get { return creatures; } }


        //perks? only 10 for now
        public bool[] perks = new bool[10];
        
        //Extras???
        private short generations;

        public Species(string speciesName, Ecosystem.speciesStats speciesStats)
        {
            name = speciesName;
            population = 1;
            stats = speciesStats;
        }

        public void addCreature(int xPos, int yPos)
        {
            tempCreatures.Add(new Creature(xPos, yPos, stats));
        }

        public void update(GameTime gameTime)
        {
            creatures = tempCreatures;
            for (int i = 0; i < creatures.Count; i++)
            {
                creatures[i].update(gameTime);
                if (creatures[i].state == 6)
                {
                    // dead
                    creatures.RemoveAt(i);
                    i--;
                }
            }
        }

        public void draw(GameTime gameTime, SpriteBatch spriteBatch, Texture2D spriteSheet, Vector3 offset, int frame)
        {
            for (int i = 0; i < creatures.Count; i++)
            {
                creatures[i].draw(gameTime, spriteBatch, spriteSheet, offset, frame);
            }
        }

        public void reproduce(Creature parent)
        {
            creatures.Add(new Creature((short)parent.Position.X,(short)parent.Position.Y, stats));
        }

    }
}
