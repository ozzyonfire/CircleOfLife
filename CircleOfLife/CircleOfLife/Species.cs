using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace CircleOfLife
{
    class Species
    {
        // Properties of Species

        // User defined attributes
        string Name;
        float Population;
        Texture2D Graphic;
        float Size;
        float Sight;
        float Speed;
        // Not sure if we will have these as base stats or just part of perks
        float Attack;
        float Defence;

        // Non-user defined attributes
        float Hunger;
        float Thirst;
        float Energy;
        // add perks

        public void Initialize()
        {
        }

        public void Update()
        {
        }

        public void Draw(SpriteBatch spriteBatch)
        {
        }
    }
}
