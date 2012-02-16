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
        public Texture2D sprite;

        public Environment(String name, Texture2D sprite, short foodValue, short energyValue, short size, short xPos, short yPos)
        {
            this.name = name;
            this.foodValue = foodValue;
            this.size = size;
            this.energyValue = energyValue;
            this.position = new Vector2((float)xPos, (float)yPos);
            this.sprite = sprite;
        }

        public void draw(ref GraphicsDeviceManager graphics, ref SpriteBatch spriteBatch, ref Texture2D spriteSheet)
        {
            spriteBatch.Draw(spriteSheet, position, null, Color.White, 0, new Vector2(0), 0.3f, SpriteEffects.None, 0.0f);
        }
    }
}
