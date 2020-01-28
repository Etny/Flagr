using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Flagr.Flags
{
    class FlagManager
    {

        private List<Flag> flags = new List<Flag>();
        private Random rng;

        public FlagManager()
        {
            rng = new Random();
        }

        public Flag GetRandomFlag()
        {
            return flags[rng.Next(0, flags.Count - 1)];
        }

        public List<Flag> GetFlags()
        {
            return flags;
        }

        public void Populate()
        {
            string[] lines = Properties.Resources.CountryCodes.Split('\n');
            StringBuilder builder = new StringBuilder();

            foreach (string line in lines)
            {
                string[] words = line.Split(' ');
                string code = words[0].ToLower();

                Bitmap img = (Bitmap)Properties.Resources.ResourceManager.GetObject(code);

                if (img == null) continue;

                for (int i=1; i<words.Length; i++)
                {
                    string current = words[i].Substring(0, 1) + words[i].Substring(1, words[i].Length-1).ToLower();

                    builder.Append(i == words.Length - 1 ? current : current + ' ');
                }

                flags.Add(new Flag(img, builder.ToString(), Program.Width/3, Program.Height/3));
                builder.Clear();
            }

        }

    }
}
