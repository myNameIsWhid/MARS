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
    
    class Sounds
    {
        public static Dictionary<string, SoundEffectInstance> sounds = new Dictionary<string, SoundEffectInstance>();
        public static Dictionary<string, float> volumes = new Dictionary<string, float>();
        public static float volume = 1f;
        public static void addSound(string label, SoundEffectInstance effect, float v)
        {
            sounds.Add(label, effect);
            volumes.Add(label, v);
        }

        public static void addSound(string label, SoundEffectInstance effect)
        {
            sounds.Add(label, effect);
            volumes.Add(label, 1f);
        }

        public static void playSound(string label)
        {
            if(sounds.ContainsKey(label))
            {
                sounds[label].Volume = (float)volume * volumes[label];
                sounds[label].Play();
            }
                
        }

        public static void editSound()
        {
            foreach (KeyValuePair<string, SoundEffectInstance> entry in sounds)
            {
                if (entry.Value.State == SoundState.Playing)
                    entry.Value.Volume = (float)Math.Min(volume * volumes[entry.Key],1f);
            }
        }

        public static void stopSound(string label)
        {
            if (sounds.ContainsKey(label))
                sounds[label].Stop();
        }
    }
}
