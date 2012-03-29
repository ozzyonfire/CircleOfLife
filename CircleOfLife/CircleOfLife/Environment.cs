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
    class Environment
    {
        public String name;
        public short foodValue;
        public short size;
        public Vector2 position;
        public short state;
        public short energyValue;
        public int type;
        Random random;

        int frameOffset;
        //sprite location base
        Rectangle spriteRectangle;
        public Rectangle body;
        Color color;

        public Environment(String name, short foodValue, short energyValue, short size, short xPos, short yPos, int type, int randSeed)
        {
            this.name = name;
            this.foodValue = foodValue;
            this.size = size;
            this.energyValue = energyValue;
            this.position = new Vector2((float)xPos, (float)yPos);
            this.type = type;

            random = new Random(randSeed);

            switch (type)
            {
                case 0:
                    spriteRectangle = Sprites.herbivore;
                    break;
                default:
                    spriteRectangle = Sprites.herbivore;
                    break;
            }

            float w = size * 0.1f * spriteRectangle.Width;
            float h = size * 0.1f * spriteRectangle.Height;

            body = new Rectangle(xPos, yPos, (int)w, (int)h);

            color = Color.Green;

        }

<<<<<<< HEAD
        public void draw(ref SpriteBatch spriteBatch, ref Texture2D spriteSheet, Vector3 offset, int frame)
=======
        public void update()
        {
            float w = size * 0.1f * spriteRectangle.Width;
            float h = size * 0.1f * spriteRectangle.Height;

            body.Width = (int)w;
            body.Height = (int)h;
        }

        public void draw(ref SpriteBatch spriteBatch, ref Texture2D spriteSheet, Vector2 offset, int frame)
>>>>>>> e6554d83874323f39ae08c157baa9cdb7aaf323e
        {
            //float x = offset.Z * offset.Z / 2 * tx * (tx / 2 - position.X + offset.X);
            //float y = offset.Z * offset.Z / 2 * ty * (ty / 2 - position.Y + offset.Y);
            spriteRectangle.X = 100 * ((frame + frameOffset) % 4);
<<<<<<< HEAD
            spriteBatch.Draw(spriteSheet, new Vector2((int)(offset.Z * (position.X + offset.X)), (int)(offset.Z * (position.Y + offset.Y))), spriteRectangle, color, 0, new Vector2(0), 0.1f * size * offset.Z, SpriteEffects.None, 0.9f);
=======
            spriteBatch.Draw(spriteSheet, new Vector2(position.X + offset.X, position.Y + offset.Y), spriteRectangle, color, 0, new Vector2(0), 0.1f * size, SpriteEffects.None, 0.9f);
            //spriteBatch.Draw(spriteSheet, new Rectangle(body.X + (int)offset.X, body.Y + (int)offset.Y, body.Width, body.Height), Color.Blue);
>>>>>>> e6554d83874323f39ae08c157baa9cdb7aaf323e
        }

        public Environment grow(int width, int height)
        {
            int i = 0;
            float x = (float)random.Next(-100, 100);
            float y = (float)random.Next(-100, 100);

            while (i == 0)
            {
                x = (float)random.Next(-100, 100);
                y = (float)random.Next(-100, 100);

                Vector2 point = new Vector2(this.position.X + x, this.position.Y + y);
                Vector2 center = new Vector2(width / 2, height / 2);

                float minD = Math.Min(height / 2, width / 2);
                if (Vector2.Distance(center, point) < minD - 100)
                {
                    // within the circle so break out and draw crops
                    i++;
                }
            }

            return new Environment(this.name, this.foodValue, this.energyValue, this.size, (short)(this.position.X + x), (short)(this.position.Y + y), this.type, System.Environment.TickCount);

            
        }
    }
}
