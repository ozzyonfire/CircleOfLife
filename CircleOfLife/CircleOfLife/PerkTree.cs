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

using Nuclex.UserInterface;
using Nuclex.Input;
using Nuclex.Game;

namespace CircleOfLife
{
    class PerkTree
    {
        public List<Perk> perks = new List<Perk>(15);
        public int offset;
        //coordinates of full tree
        public int x;
        public int y;
        public int height;
        public int width;

        //currently selected perk node
        Perk selectedPerkNode = null;

        public PerkTree()
        {
            add("Herbivore", new Vector2(450, 150));
            add("Carnivore", new Vector2(700, 150));
            add("Claws", new Vector2(200, 300));
            add("Tail", new Vector2(450, 300));
            add("Eyes", new Vector2(700, 300));
            add("Humps", new Vector2(950, 300));
            add("Claws", new Vector2(200, 450));
            add("Tail", new Vector2(450, 450));
            add("Eyes", new Vector2(700, 450));
            add("Humps", new Vector2(950, 450));
        }

        public void add(String name, Vector2 position)
        {
            Perk newPerk = new Perk(name, position);
            //checks to see if the new perk expands the bounds of the entire perk tree and adjusts
            if (x == 0) //assume this means unitialized
            {
                x = (int)position.X;
                y = (int)position.Y;
                width = newPerk.width;
                height = newPerk.height;
            }
            else
            {
                if (position.X < x)
                {
                    width = x - (int)position.X + width;
                    x = (int)position.X;
                }
                if (position.Y < y)
                {
                    height = y - (int)position.Y + height;
                    y = (int)position.Y;
                }
                
                if (position.X + newPerk.width - x > width)
                    width = (int)position.X + newPerk.width - x;
                if (position.Y + newPerk.height - y > height)
                    height = (int)position.Y + newPerk.height - y;
            }
            perks.Add(newPerk);
        }
        public void mouseOver(int x, int y)
        {
            for (int i = 0; i < perks.Count; i++)
            {
                perks[i].mouseOver(x,y,offset);
            }
        }

        public void mouseClick(int x, int y)
        {
            for (int i = 0; i < perks.Count; i++)
            {
                if (perks[i].mouseOver(x, y, offset))
                {
                    if (selectedPerkNode != null)
                        selectedPerkNode.Selected = false;
                    selectedPerkNode = perks[i];
                    perks[i].Selected = true;
                }
            }
        }

        public void draw(GameTime gameTime, SpriteBatch spriteBatch, Texture2D spriteSheet, CircleOfLifeGame.GameFonts gameFonts)
        {
            for (int i = 0; i < perks.Count; i++)
            {
                perks[i].draw(gameTime, spriteBatch, spriteSheet, gameFonts,offset);
            }
        }
    }

    class Perk
    {
        public String name;
        public int ID;
        public String info;
        public int cost;
        public int X;
        public int Y;

        public bool bought;
        
        bool selected;
        bool hover;


        public bool Selected
        {
            set
            {
                selected = value;
                if (value)
                    bgColor = Color.Red;
            }
            get { return selected; }
        }
        public bool Hover
        {
            set
            {
                hover = value;
                if(!selected)
                    if (value && !selected)
                        bgColor = Color.Green;
                    else
                        bgColor = Color.White;
            }
            get { return selected; }
        }

        public int width = 200;
        public int height = 100;
        Color bgColor = Color.White;


        Rectangle topLeft = new Rectangle(0, 1500, 5, 27);
        Rectangle top = new Rectangle(5, 1500, 40, 27);
        Rectangle topRight = new Rectangle(45, 1500, 5, 27);
        Rectangle left = new Rectangle(0, 1527, 5, 19);
        Rectangle right = new Rectangle(45, 1527, 5, 19);
        Rectangle bottomLeft = new Rectangle(0, 1546, 5, 4);
        Rectangle bottom = new Rectangle(0, 1546, 40, 4);
        Rectangle bottomRight = new Rectangle(45, 1546, 5, 4);
        public Perk(String name, Vector2 position)
        {
            this.name = name;
            this.X = (int)position.X;
            this.Y = (int)position.Y;
        }

        //tests if true and sets hover property
        public bool mouseOver(int x, int y, int offset)
        {

            if ((x >= X + offset) && (x <= X + width + offset) && (y >= Y) && (y <= Y + height))
            {
                Hover = true;
                return true;
            }
            else
            {
                Hover = false;
                return false;
            }
        }

        public void draw(GameTime gameTime, SpriteBatch spriteBatch, Texture2D spriteSheet, CircleOfLifeGame.GameFonts gameFonts,int offset)
        {
            spriteBatch.Draw(spriteSheet, new Rectangle(X + offset, Y, 5, 27), topLeft, bgColor, 0.0f, new Vector2(0, 0), SpriteEffects.None, 0.1f);
            spriteBatch.Draw(spriteSheet, new Rectangle(X + 5 + offset, Y, width - 10, 27), top, bgColor, 0.0f, new Vector2(0, 0), SpriteEffects.None, 0.1f);
            spriteBatch.Draw(spriteSheet, new Rectangle(X + width - 5 + offset, Y, 5, 27), topRight, bgColor, 0.0f, new Vector2(0, 0), SpriteEffects.None, 0.1f);
            spriteBatch.Draw(spriteSheet, new Rectangle(X + offset, Y + 27, 5, height - 27), left, bgColor, 0.0f, new Vector2(0, 0), SpriteEffects.None, 0.1f);
            spriteBatch.Draw(spriteSheet, new Rectangle(X + width - 5 + offset, Y + 27, 5, height - 27), right, bgColor, 0.0f, new Vector2(0, 0), SpriteEffects.None, 0.1f);
            spriteBatch.Draw(spriteSheet, new Rectangle(X + offset, Y + height, 5, 4), bottomLeft, bgColor, 0.0f, new Vector2(0, 0), SpriteEffects.None, 0.1f);
            spriteBatch.Draw(spriteSheet, new Rectangle(X + 5 + offset, Y + height, width - 10, 4), bottom, bgColor, 0.0f, new Vector2(0, 0), SpriteEffects.None, 0.1f);
            spriteBatch.Draw(spriteSheet, new Rectangle(X + width - 5 + offset, Y + height, 5, 4), bottomRight, bgColor, 0.0f, new Vector2(0, 0), SpriteEffects.None, 0.1f);
            spriteBatch.Draw(spriteSheet, new Rectangle(X + offset, Y + 50, width, 4), bottom, bgColor, 0.0f, new Vector2(0, 0), SpriteEffects.None, 0.1f);


            spriteBatch.DrawString(gameFonts.Header, name, new Vector2(X + 15 + offset, Y), Color.White, 0.0f, new Vector2(0.0f, 0.0f), 1.0f, SpriteEffects.None, 0.0f);
            spriteBatch.DrawString(gameFonts.Header, "Cost", new Vector2(X + 15 + offset, Y + 27), Color.Red, 0.0f, new Vector2(0.0f, 0.0f), 1.0f, SpriteEffects.None, 0.0f);
            spriteBatch.DrawString(gameFonts.Header, "Effects\nMore Effects:)", new Vector2(X + 15 + offset, Y + 50), Color.Black, 0.0f, new Vector2(0.0f, 0.0f), 1.0f, SpriteEffects.None, 0.0f);
            //spriteBatch.Draw(spriteSheet, new Rectangle(X, Y + 25, width, 50), bodyRectangle, Color.White);
            //spriteBatch.Draw(spriteSheet, new Rectangle(X, Y + 74, width, 50), bodyRectangle, Color.White);
        }
    }
}
