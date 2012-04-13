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





        public static String extinct = "You let a species go extinct! You lose 1000 Order points!";
        public static String gameOver = "You ran out of Order points and your ecosystem failed!";


        public static String goodMorning = "You are Order. You wake up from a millenia long\n" +
                                            "cat nap to discover that the world has been overun by chaos.\n\n" +
                                            "Furious, you set out to restore order by creating an ecosystem, \n" +
                                            "starting with your first creature";

        public static String placeCreature = "You have cleaved out a small section of order from\n " +
                                            "the shadowy chaos. Life has already began to spring \n" +
                                            "up from this order. Place your creatures anywhere in\n"+
                                            "your realm and watch as it thrives!";


        public static String gameInstructions1 = "As your ecosystem grows so will your power!\n(shown in the top right corner)\n" +
                                            "You can influence the world by adding or altering species \nin the menus in" +
                                            "the bottom right corner. \nMove around the world by moving the mouse to the \nedges or zooming with the scroll wheel";

        public static String gameInstructions2 = "You can also directly influence the world by allowing some\n chaos " +
                                            "into your realm, using the right mouse button,\n to clear a small section" +
                                            "This power costs 100 Order Points\n\n"+
                                            "Careful: If your Order drops to zero chaos will conquer!";
    }
}
