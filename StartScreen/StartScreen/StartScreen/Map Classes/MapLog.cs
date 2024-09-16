using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System;
using System.Collections.Generic;
using System.Linq;

namespace StartScreen
{
    class MapLog
    {
        //FOR WINNING ANIMATION SEQUENCE
        public static int logTime = 0;
        public Color color;
        public int index;

        public MapLog(Color color, int index)
        {
            this.color = color;
            this.index = index;
        }

        
    }
}
