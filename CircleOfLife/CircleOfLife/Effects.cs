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
    //Class used for animation of random sprite effects in game. such as floating numbers
    public class Effects
    {
        public List<Effect> effects = new List<Effect>(50); //unsure of size
        public Effects()
        {
            
        }

        public void draw(GameTime gameTime, SpriteBatch spriteBatch, Texture2D spriteSheet, CircleOfLifeGame.GameFonts gameFonts)
        {
            for (int i = 0; i < effects.Count; i++)
            {
                if (effects[i].Finished)
                {
                    effects.RemoveAt(i);
                    i--;
                }
                else
                    effects[i].draw(gameTime, spriteBatch, spriteSheet, gameFonts);

            }
        }


        public void addFloatingString(string text, Vector2 position, Color color)
        {
            effects.Add(new floatingString(text,position,color));
        }
    }

    public interface Effect
    {
        bool Finished { get; }
        void draw(GameTime gameTime, SpriteBatch spriteBatch, Texture2D spriteSheet, CircleOfLifeGame.GameFonts gameFonts);
    }

    class floatingString : Effect
    {
        public bool Finished
        { get { return finished; } }

        bool finished = false;
        int frame = 0;
        int frames = 30;
        Vector2 pos;
        string text;
        Color color;

        public floatingString(string text, Vector2 pos, Color color)
        {
            this.text = text;
            this.color = color;
            this.pos = pos;
        }

        public void draw(GameTime gameTime, SpriteBatch spriteBatch, Texture2D spriteSheet, CircleOfLifeGame.GameFonts gameFonts)
        {
            //TODO integrate game time into frame calculation

            pos.Y = pos.Y -1; //guessed effect
            color.A = (byte)(255 - 8 *frame );
            spriteBatch.DrawString(gameFonts.Header, text, pos, color,0.0f,Vector2.Zero,1.0f,SpriteEffects.None,0.1f);

            frame++;
            if (frame == frames)
                finished = true;
        }
    }
}
