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
        public static Rectangle mormo = new Rectangle(0, 1400, 100, 100);
        public static Rectangle hudBackground = new Rectangle(0, 1000, 50, 150);
        public static Rectangle menuButton = new Rectangle(50, 1000, 150, 150);
        public static Rectangle addButton = new Rectangle(700, 1000, 150, 150);
        public static Rectangle flower = new Rectangle(0, 1700, 100, 100);
        public static Rectangle plant = new Rectangle(300, 1700, 100, 100);
        public static Rectangle title = new Rectangle(1300, 0, 900, 300);
        public static Rectangle leak = new Rectangle(1000, 0, 300, 300);


        public static Rectangle herbivore = new Rectangle(1000, 400, 150, 150);
        public static Rectangle herbivoreTail = new Rectangle(1000, 550, 150, 150);
        public static Rectangle herbivoreEyes = new Rectangle(1000, 700, 150, 150);
        public static Rectangle herbivorePincer = new Rectangle(1000, 850, 150, 150);
        public static Rectangle herbivoreTailPincer = new Rectangle(1000, 1000, 150, 150);
        public static Rectangle herbivoreTailEyes = new Rectangle(1000, 1150, 150, 150);
        public static Rectangle herbivoreEyesPincer = new Rectangle(1000, 1300, 150, 150);
        public static Rectangle herbivoreTailEyesPincer = new Rectangle(1000, 1450, 150, 150);
        public static Rectangle carnivore = new Rectangle(1000, 1600, 150, 150);
        public static Rectangle carnivoreTail = new Rectangle(1000, 1750, 150, 150);
        public static Rectangle carnivoreEyes = new Rectangle(1000, 1900, 150, 150);
        public static Rectangle carnivorePincer = new Rectangle(1000, 2050, 150, 150);
        public static Rectangle carnivoreTailPincer = new Rectangle(1000, 2200, 150, 150);
        public static Rectangle carnivoreEyesPincer = new Rectangle(1000, 2350, 150, 150);
        public static Rectangle carnivoreTailEyes = new Rectangle(1000, 2500, 150, 150);
        public static Rectangle carnivoreTailEyesPincer = new Rectangle(1000, 2650, 150, 150);



        public static String description = "Here lies a description of a perk.\n I sure hope it works";

    }
}
