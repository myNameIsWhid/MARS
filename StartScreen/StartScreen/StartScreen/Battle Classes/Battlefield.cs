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
    class Battlefield
    {
        public List<Vector2> mesh;
        private Texture2D texture;
        public Rectangle spawnAttack;
        public Rectangle spawnDefend;
        public string type;

        public Battlefield(List<Vector2> mesh, Texture2D texture, string type)
        {
            this.mesh = mesh;
            this.texture = texture;
            this.spawnAttack = new Rectangle(0, 0, 0, 0);
            this.spawnDefend = new Rectangle(0, 0, 0, 0);
            this.type = type;
        }


        public void setSpawns(Rectangle spawn1, Rectangle spawn2)
        {
            this.spawnAttack = spawn1;
            this.spawnDefend = spawn2;
        }

        public void Draw(SpriteBatch sb, GraphicsDevice gd)
        {
            Rectangle screen = new Rectangle(0, 0, gd.Viewport.Width, gd.Viewport.Height);
            sb.Draw(texture, screen, Color.White);
        }
    }
}
