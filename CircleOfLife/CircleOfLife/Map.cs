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
        public List<Crop> crops = new List<Crop>(100);
        public List<Water> water = new List<Water>(50);
        Random random = new Random();
        int cropNumber;
        int width;
        int height;

        public void intialize(int width, int height)
        {
            // choose size to be 800 x 800
            this.width = width;
            this.height = height;
            this.cropNumber = width / 200;


            // choose 4 random points to be crops
            for (int i = 0; i < cropNumber; i++)
            {
                int x = random.Next(width);
                int y = random.Next(height);

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
            // if there are less than the required number of crops then add some new ones
            if (crops.Count < cropNumber)
            {
                int x = random.Next(width);
                int y = random.Next(height);
                Environment grass = new Environment("grass", 10, 10, 5, (short)x, (short)y, 0, System.Environment.TickCount + 1);
                Crop field = new Crop(grass, random.Next(3, 8), System.Environment.TickCount, 4);
                crops.Add(field);
            }

            // remove any plants that are dead
            // grow the grass
            for (int i = 0; i < crops.Count(); i++)
            {
                if (crops[i].plants.Count == 0)
                {
                    crops.RemoveAt(i);
                    i--;
                }
                else
                {
                    crops[i].grow(gameTime);
                }
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
