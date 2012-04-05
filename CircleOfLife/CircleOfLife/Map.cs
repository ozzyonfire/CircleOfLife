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
        public Vector2 center;
        int cropNumber;
        public int width;
        public int height;

        public void intialize(int width, int height)
        {
            this.width = width;
            this.height = height;
            this.cropNumber = width / 150;

            center = new Vector2(width / 2, height / 2);

            // choose random points to be crops within the map
            for (int i = 0; i < cropNumber; i++)
            {
                int x = random.Next(width);
                int y = random.Next(height);

                Vector2 point = new Vector2(x, y);
                float minD = Math.Min(height / 2, width / 2);
                if (Vector2.Distance(center, point) >= minD - 100)
                {
                    // not within the circle so generate new points
                    i--;
                }
                else
                {
                    Crop field = new Crop(random.Next(3, 8), System.Environment.TickCount + i, 4);
                    Environment grass;
                    if (random.Next(0, 2) == 0)
                    {
                        grass = new Environment("grass", 5, 5, 5, (short)x, (short)y, 0, System.Environment.TickCount + 1, field);
                    }
                    else
                    {
                        grass = new Environment("grass", 2, 2, 5, (short)x, (short)y, 1, System.Environment.TickCount + 1, field);
                    }
                    field.addPlant(grass);
                    crops.Add(field);
                }
            }            
        }

        public void update(GameTime gameTime)
        {
            // if there are less than the required number of crops then add some new ones
            if (crops.Count < cropNumber)
            {
                int i = 0;

                while (i == 0)
                {
                    int x = random.Next(width);
                    int y = random.Next(height);

                    Vector2 point = new Vector2(x, y);
                    float minD = Math.Min(height / 2, width / 2);
                    if (Vector2.Distance(center, point) < minD - 100)
                    {
                        // within the circle so generate new points
                        i++;
                        
                        Crop field = new Crop(random.Next(3, 8), System.Environment.TickCount, 4);
                        Environment grass;
                        if (random.Next(0, 2) == 0)
                        {
                            grass = new Environment("grass", 2, 2, 5, (short)x, (short)y, 1, System.Environment.TickCount + 1, field);
                        }
                        else
                        {
                            grass = new Environment("grass", 5, 5, 5, (short)x, (short)y, 0, System.Environment.TickCount + 1, field);
                        }
                        field.addPlant(grass);
                        crops.Add(field);
                    }

                }
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
                    crops[i].grow(gameTime, this.width, this.height);
                }
            }
            
        }

        public void draw(GameTime gameTime, SpriteBatch spriteBatch, Texture2D spriteSheet, Vector3 offset, int frame)
        {
            // draw the water

            // draw the crops
            for (int i = 0; i < crops.Count; i++)
            {
                crops[i].draw(ref spriteBatch, ref spriteSheet, offset, frame);
            }            
        }

    }
}
