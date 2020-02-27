using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Flagr.Flags
{
    class FlagManager
    {

        private List<Flag> flags = new List<Flag>();
        private Random rng;

        XmlDocument doc;
        XmlElement flagElement;

        private readonly int MaxWidth;
        private readonly int MaxHeight;

        public FlagManager()
        {
            rng = new Random();

            MaxWidth = Program.Width / 3;
            MaxHeight = Program.Height / 3;
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
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(Properties.Resources.flagData);

            XmlNodeList flagList = doc.DocumentElement.ChildNodes;
           
            for(int i = 0; i < flagList.Count; i++)
            {
                XmlNode node = flagList.Item(i);

                flags.Add(new Flag(node, MaxWidth, MaxHeight));
            }
        }

        private void CreateXml()
        {
            doc = new XmlDocument();
            flagElement = doc.CreateElement("flags");
        }

        private void AppendFlag(Flag f, Image img)
        {
            XmlElement element = doc.CreateElement("flag");

            //Set Country
            XmlElement country = doc.CreateElement("country");
            country.AppendChild(doc.CreateTextNode(f.Country));

            //Set Image
            XmlElement image = doc.CreateElement("image");
            image.AppendChild(doc.CreateTextNode(f.ImageName));

            //Set Scale
            XmlElement scale = doc.CreateElement("scale");
            scale.AppendChild(doc.CreateTextNode(f.Scale + ""));

            //Set Width
            XmlElement width = doc.CreateElement("width");
            width.AppendChild(doc.CreateTextNode(img.Width + ""));

            //Set Height
            XmlElement height = doc.CreateElement("height");
            height.AppendChild(doc.CreateTextNode(img.Height + ""));

            element.AppendChild(country);
            element.AppendChild(image);
            element.AppendChild(scale);
            element.AppendChild(width);
            element.AppendChild(height);
            
            flagElement.AppendChild(element);
        }

        private void SaveXml()
        {
            doc.AppendChild(flagElement);

            String folder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "Flagr");
            String path = Path.Combine(folder, "test.xml");

            if (!Directory.Exists(folder))
                Directory.CreateDirectory(folder);

            if (!File.Exists(path))
                File.Create(path).Close();

            doc.Save(path);
        }

        public void GenerateXmlDoc()
        {
            CreateXml();

            string[] lines = Properties.Resources.CountryCodes.Split('\n');
            StringBuilder builder = new StringBuilder();

            foreach (string line in lines)
            {
                string[] words = line.Replace("\n","").Replace("\r", "").Split(' ');
                string code = words[0].ToLower();

                Bitmap img = (Bitmap)Properties.Resources.ResourceManager.GetObject(code);

                if (img == null) continue;

                for (int i=1; i<words.Length; i++)
                {
                    string current = words[i].Substring(0, 1) + words[i].Substring(1, words[i].Length-1).ToLower();

                    builder.Append(i == words.Length - 1 ? current : current + ' ');
                }

                Flag f = new Flag(img, builder.ToString(), code, MaxWidth, MaxHeight);

                flags.Add(f);
                AppendFlag(f, img);
                builder.Clear();

                f.UnloadImage();
                img.Dispose();
            }

            SaveXml();
        }


    }
}
