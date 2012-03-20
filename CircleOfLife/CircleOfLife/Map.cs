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
    class Map
    {
        private List<Crop> crops = new List<Crop>(100);
        private List<Water> water = new List<Water>(50);
        Random random = new Random();
        

        public void intialize()
        {
            // choose size to be 800 x 800

            // choose 4 random points to be crops
            for (int i = 0; i < 4; i++)
            {
                int x = random.Next(800);
                int y = random.Next(800);

                Environment grass = new Environment("grass", 10, 10, 5, (short)x, (short)y, 0, System.Environment.TickCount + i + 1);
                Crop field = new Crop(grass, random.Next(3,8), System.Environment.TickCount + i, 4);
                crops.Add(field);
            }

            // choose 3 random points for water
            for (int i = 0; i < 3; i++)
            {
                int x = random.Next(800);
                int y = random.Next(800);

                short radius = (short)random.Next(100);

                Water pond = new Water((short)x, (short)y, radius);
                water.Add(pond);
            }

            
        }

        public void update(GameTime gameTime)
        {
                // grow the grass
                for (int i = 0; i < crops.Count(); i++)
                {
                    crops[i].grow(gameTime);
                }
        }

        public void draw(GameTime gameTime, SpriteBatch spriteBatch, Texture2D spriteSheet)
        {
            // draw the water

            // draw the crops
            for (int i = 0; i < crops.Count; i++)
            {
                crops[i].draw(ref spriteBatch, ref spriteSheet);
            }            
        }

    }
}
