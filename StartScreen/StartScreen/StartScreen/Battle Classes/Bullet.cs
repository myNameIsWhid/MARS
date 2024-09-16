using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;


namespace StartScreen
{
    class Bullet
    {
        Texture2D texture;
        Rectangle[] source;
        int animIndex;
        public double heading;
        Rectangle rect;
        int size;
        int speed;
        public int damage;

        public Bullet(Rectangle source, Texture2D texture, int size, int speed, int damage, Soldier origin)
        {
            animIndex = 0;
            this.source = new Rectangle[1];
            this.source[0] = source;
            this.damage = damage;
            this.speed = speed;
            this.texture = texture;
            this.heading = (origin.degree * (Math.PI / 180));
            this.size = size;
            rect = new Rectangle((int)origin.center.X, (int)origin.center.Y, size, (int)(size * 6));
        }

        public Bullet(Rectangle[] source, Texture2D texture, int size, int speed, int damage, Soldier origin)
        {
            animIndex = 0;
            this.source = source;
            this.damage = damage;
            this.speed = speed;
            this.texture = texture;
            this.heading = (origin.degree * (Math.PI / 180));
            this.size = size;
            rect = new Rectangle((int)origin.center.X, (int)origin.center.Y, size, (int)(size * 6.5));
        }

        public Bullet(Bullet other, Vector2 center, double degree)
        {
            animIndex = 0;
            this.source = other.source;
            this.damage = other.damage;
            this.texture = other.texture;
            this.heading = degree * (Math.PI / 180);
            this.rect = other.rect;
            this.speed = other.speed;
            this.size = other.size;
            this.rect.X = (int)center.X;
            this.rect.Y = (int)center.Y;
        }

        public void Update(GameTime gameTime)
        {
            animIndex = animIndex % source.Length;
            rect.X += (int)(speed * Math.Cos((float)heading));
            rect.Y += (int)(speed * Math.Sin((float)heading));
            if (gameTime.ElapsedGameTime.Ticks % 5 == 0 && source.Length > 1)
                animIndex++;
        }

        public bool isOffScreen(GraphicsDevice gd)
        {
            if (rect.X < 0 || rect.X > gd.Viewport.Width)
                if (rect.Y < 0 || rect.Y > gd.Viewport.Height)
                    return true;
            return false;
        }

        public void Draw(SpriteBatch sb)
        {
            sb.Draw(texture, rect, source[animIndex], Color.White, (float)(heading + (Math.PI / 2)), new Vector2(source[animIndex].Width / 2, source[animIndex].Height / 2), SpriteEffects.None, 1);
        }

        public Rectangle getRect()
        {
            return rect;
        }
    }
}
