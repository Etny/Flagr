using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Flagr.Flag
{
    class FlagManager
    {
        private Dictionary<string, string> countryCodes = new Dictionary<string, string>();

        public FlagManager()
        {
            InitializeCountryCodes();

            Console.WriteLine(countryCodes["ae"]);
        }

        private void InitializeCountryCodes()
        {
            string[] lines = Properties.Resources.CountryCodes.Split('\n');
            StringBuilder builder = new StringBuilder();

            foreach (string line in lines)
            {
                string[] words = line.Split(' ');
                string code = words[0].ToLower();

                for(int i=1; i<words.Length; i++)
                {
                    string current = words[i].Substring(0, 1) + words[i].Substring(1, words[i].Length-1).ToLower();

                    builder.Append(i == words.Length - 1 ? current : current + ' ');
                }

                countryCodes[code] = builder.ToString();
                builder.Clear();
            }

        }

    }
}
