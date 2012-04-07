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
        public static Rectangle herbivore = new Rectangle(1000, 400, 150, 150);
        public static Rectangle herbivoreTail = new Rectangle(1000, 700, 150, 150);
        public static Rectangle herbivoreEyes = new Rectangle(1000, 850, 150, 150);
        public static Rectangle carnivore = new Rectangle(1000, 550, 150, 150);
        public static Rectangle carnivoreTail = new Rectangle(1000, 1000, 150, 150);
        public static Rectangle carnivoreEyes = new Rectangle(1000, 1150, 150, 150);
        public static Rectangle mormo = new Rectangle(0, 1400, 100, 100);
        public static Rectangle hudBackground = new Rectangle(0, 1000, 50, 150);
        public static Rectangle menuButton = new Rectangle(50, 1000, 150, 150);
        public static Rectangle addButton = new Rectangle(700, 1000, 150, 150);
        public static Rectangle flower = new Rectangle(0, 1700, 100, 100);
        public static Rectangle plant = new Rectangle(300, 1700, 100, 100);
        public static Rectangle title = new Rectangle(1300, 0, 900, 300);
        public static Rectangle leak = new Rectangle(1000, 0, 300, 300);


        public static String description = "Here lies a description of a perk.\n I sure hope it works";

    }
}
