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
        //sprite location base
        Rectangle spriteRectangle;
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
                    spriteRectangle = new Rectangle(0, 0, 100, 100);
                    break;
                default:
                    spriteRectangle = new Rectangle(0, 0, 100, 100);
                    break;
            }

            color = Color.Green;

        }

        public void draw(ref SpriteBatch spriteBatch, ref Texture2D spriteSheet)
        {
            spriteBatch.Draw(spriteSheet, position, spriteRectangle, color, 0, new Vector2(0), 0.1f * size, SpriteEffects.None, 0.0f);
        }

        public Environment grow()
        {
            float x = (float)random.Next(-100, 100);
            float y = (float)random.Next(-100, 100);

            return new Environment(this.name, this.foodValue, this.energyValue, this.size, (short)(this.position.X + x), (short)(this.position.Y + y), this.type, System.Environment.TickCount);
        }
    }
}
