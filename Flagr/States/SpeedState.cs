using Flagr.UI;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Flagr.States
{
    class SpeedState : State
    {

        private XmlNode boatNode;
        private List<Boat> boats = new List<Boat>();

        private int waterLine = Program.Height / 2;
        private int waterHeight = 200;
        private Color waterColor = Color.DeepSkyBlue;

        private float boatScale = .75f;


        public SpeedState()
        {
            ParseBoatData();

            AddBoat(boatNode);
        }

        private void ParseBoatData()
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(Properties.Resources.boatData);

            XmlNodeList boatList = doc.DocumentElement.ChildNodes;

            boatNode = boatList.Item(0);
        }

        private void AddBoat(XmlNode boatNode)
        {
            Boat boat = new Boat(boatNode, boatScale);

            boat.SetLocation(Program.Width / 2, waterLine - boat.Size.Height + boat.Weight);
            boat.SetFlag(Program.Flags.GetRandomFlag());
            boat.Container.Flag.LoadImage();

            boats.Add(boat);
        }

        public override void Update(DeltaTime deltaTime)
        {

            Draw();
        }

        private void Draw()
        {
            graphics.FillRectangle(Brushes.White, 0, 0, Program.Width, Program.Height);

            foreach (Boat boat in boats)
                boat.Draw(graphics);

            graphics.FillRectangle(new SolidBrush(waterColor), 0, waterLine, Program.Width, waterHeight);
        }

    }
}
