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
        private List<Environment> flora = new List<Environment>(50);
        private List<Water> water = new List<Water>(50);
        Random random = new Random();

        public void intialize()
        {
            // choose size to be 800 x 800

            // choose 5 random points to be centers of food
            for (int i = 0; i < 5; i++)
            {
                int x = random.Next(800);
                int y = random.Next(800);

                Environment grass = new Environment("grass", 10, 10, 5, (short)x, (short)y);
                flora.Add(grass);
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
            for (int i = 0; i < flora.Count(); i++)
            {
                flora.Add(flora[i].grow());
            }
        }

        public void draw(GameTime gameTime, SpriteBatch spriteBatch, Texture2D spriteSheet)
        {
            // draw the water

            // draw the grass
            for (int i = 0; i < flora.Count; i++)
            {
                flora[i].draw(ref spriteBatch, ref spriteSheet);
            }
        }

    }
}
