using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;


namespace CircleOfLife
{
    /// <summary>
    /// This is a game component that implements IUpdateable.
    /// </summary>
    public class Water 
    {
        public short x;
        public short y;
        public short radius;

        public Water(short x, short y, short radius)
        {
            this.x = x;
            this.y = y;
            this.radius = radius;
        }
        
    }
}
