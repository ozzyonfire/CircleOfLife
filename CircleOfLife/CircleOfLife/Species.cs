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
    class Species
    {
        private string name;
        private short population;
        private Ecosystem.speciesStats stats;
        private List<Creature> creatures = new List<Creature>(100);
        public List<Species> predators = new List<Species>(10);
        public List<Species> prey = new List<Species>(10);

        //accesors
        public Ecosystem.speciesStats Stats { get { return stats; } }
        public List<Creature> Creatures { get { return creatures; } }

        //graphics

        //perks?
        
        
        //Extras???
        private short generations;

        public Species(String speciesName, Ecosystem.speciesStats speciesStats, short xPos, short yPos)
        {
            name = speciesName;
            population = 1;
            stats = speciesStats;
            creatures.Add(new Creature(xPos, yPos, speciesStats));
        }

        public void update()
        {
            for (int i = 0; i < creatures.Count; i++)
            {
                creatures[i].update();
            }
        }

        public void draw(ref GraphicsDeviceManager graphics, ref SpriteBatch spriteBatch, ref Texture2D spriteSheet)
        {
            for (int i = 0; i < creatures.Count; i++)
            {
                creatures[i].draw(ref graphics, ref spriteBatch, ref spriteSheet);
            }
        }

        public void reproduce(Creature parent)
        {
            creatures.Add(new Creature((short)parent.Position.X,(short)parent.Position.Y, stats));
        }

    }
}
