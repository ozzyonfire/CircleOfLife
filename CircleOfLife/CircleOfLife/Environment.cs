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
    public class Environment
    {
        public String name;
        public short foodValue;
        public short size;
        public Vector2 position;
        public Vector2 origin;
        public short state;
        public short energyValue;
        public int type;
        TimeSpan growTime;
        Random random;

        public float width;
        public float height;

        int frame;
        //sprite location base
        Rectangle spriteRectangle;
        public RotatedRectangle body;
        Color color;
        float orientation;  //keep things random

        public Environment(String name, short foodValue, short energyValue, short size, short xPos, short yPos, int type, int randSeed)
        {
            this.name = name;
            this.foodValue = foodValue;
            this.size = size;
            this.energyValue = energyValue;
            this.position = new Vector2((float)xPos, (float)yPos);
            this.type = type;
            this.frame = 0;
            random = new Random(randSeed);

            switch (type)
            {
                case 0:
                    spriteRectangle = Sprites.flower;
                    break;
                default:
                    spriteRectangle = Sprites.flower;
                    break;
            }

            float w = size * 0.1f * spriteRectangle.Width;
            float h = size * 0.1f * spriteRectangle.Height;

            orientation = random.Next(62) * 0.1f;

            origin = new Vector2(spriteRectangle.Width / 2, spriteRectangle.Height / 2);

            body = new CircleOfLife.RotatedRectangle(new Rectangle(xPos, yPos, (int)w, (int)h), orientation);

            color = Color.Green;
        }


        public void update(GameTime gameTime)
        {
            float w = size * 0.1f * spriteRectangle.Width;
            float h = size * 0.1f * spriteRectangle.Height;

            this.width = w;
            this.height = h;

            growTime += gameTime.ElapsedGameTime;

            if (growTime >= TimeSpan.FromMilliseconds(100) && frame < 2)
            {
                frame++;
            }
        }

        public void draw(ref SpriteBatch spriteBatch, ref Texture2D spriteSheet, Vector3 offset)

        {
            //float x = offset.Z * offset.Z / 2 * tx * (tx / 2 - position.X + offset.X);
            //float y = offset.Z * offset.Z / 2 * ty * (ty / 2 - position.Y + offset.Y);
            //switch between different flower types based on orientation for example purposes
            if(orientation < 4.5f)
                spriteRectangle.X = 300 + 100 * ((frame) % 4);
            else
                spriteRectangle.X = 100 * ((frame) % 4);

            spriteBatch.Draw(spriteSheet, new Vector2((int)(offset.Z * (position.X + offset.X)), (int)(offset.Z * (position.Y + offset.Y))), spriteRectangle, color, orientation, origin, 0.1f * size * offset.Z, SpriteEffects.None, 0.9f);
            //spriteBatch.Draw(spriteSheet, new Rectangle(body.X + (int)offset.X, body.Y + (int)offset.Y, body.Width, body.Height), Color.Blue);

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
