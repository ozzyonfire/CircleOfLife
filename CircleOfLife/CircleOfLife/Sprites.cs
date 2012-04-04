using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace CircleOfLife
{
    static class Sprites
    {
        public static Rectangle chaosBoundary = new Rectangle(0, 0, 1000, 1000);
        public static Rectangle herbivore = new Rectangle(1000, 700, 150, 150);
        public static Rectangle carnivore = new Rectangle(1000, 550, 150, 150);
        public static Rectangle mormo = new Rectangle(0, 1400, 100, 100);
        public static Rectangle hudBackground = new Rectangle(0, 1000, 50, 150);
        public static Rectangle torchButton = new Rectangle(50, 1000, 150, 150);
        public static Rectangle flower = new Rectangle(0, 1700, 100, 100);
        public static Rectangle plant = new Rectangle(300, 1700, 100, 100);


        public static String description = "Here lies a description of a perk.\n I sure hope it works";
    }
}
