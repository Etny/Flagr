using Flagr.UI;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;

namespace Flagr.States
{
    class SpeedState : State
    {

        private List<Boat> boatTemplates = new List<Boat>();
        private List<Boat> boats = new List<Boat>();
        private List<Boat> deadBoats = new List<Boat>();
        private float boatSpeed = 160f;
        private int maxBoats = 3;
        private float maxBoatSpawnDelay = 2.6f;
        private float spawnCountdown = 2.6f;

        public static int WaterLine = Program.Height / 5 * 2;
        private int waterHeight = 150;
        private int waterPoints = 51;
        private Color waterColor = Color.DeepSkyBlue;
        private FancyWater water;

        private float boatScale = .75f;

        private AnswerBlock[] blocks;


        public SpeedState()
        {
            water = new FancyWater(WaterLine, waterHeight, waterPoints, waterColor);

            Size blockSize = new Size(Program.Width / 3 + 1, Program.Height - (WaterLine + waterHeight));
            blocks = new AnswerBlock[3];
            blocks[0] = new AnswerBlock(new Point(0, WaterLine + waterHeight), Color.Red, blockSize, "A", 0);
            blocks[1] = new AnswerBlock(new Point(blockSize.Width, WaterLine + waterHeight), Color.Green, blockSize, "B", 1);
            blocks[2] = new AnswerBlock(new Point(blockSize.Width * 2, WaterLine + waterHeight), Color.Blue, blockSize, "C", 2);

            ParseBoatData();

            AddBoat();
        }

        private void ParseBoatData()
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(Properties.Resources.boatData);

            XmlNodeList boatList = doc.DocumentElement.ChildNodes;

            for(int i = 0; i < boatList.Count; i++)
                boatTemplates.Add(new Boat(boatList.Item(i), boatScale));
        }

        private void AddBoat()
        {
            Boat boat = boatTemplates[0].Clone();

            foreach(AnswerBlock block in blocks)
            {
                if (block.Enabled)
                    continue;

                block.NewQuestion(boat);
                break;
            }

            boat.CreateImage();

            boats.Add(boat);


            if (boats.Count < maxBoats) spawnCountdown = maxBoatSpawnDelay;
        }

        public override void Scroll(MouseEventArgs e)
        {
            water.MovePoint(water.ClosestPoint(Program.Input.MouseLocation.X), Math.Sign(e.Delta) * 5);

           // if(boats.Count < 3) AddBoat();
        }

        public override void Update(DeltaTime deltaTime)
        {
            if(spawnCountdown > 0)
            {
                spawnCountdown -= deltaTime.Seconds;

                if (spawnCountdown <= 0 && boats.Count < maxBoats)
                    AddBoat();      
            }

            water.Update(deltaTime);

            foreach (Boat boat in boats)
            {
                boat.Update(deltaTime, water, boatSpeed);
                if (boat.OutOfBounds) deadBoats.Add(boat);
            }

            foreach(Boat dead in deadBoats)
            {
                dead.CurrentBlock.Enabled = false;
                boats.Remove(dead);
                if(spawnCountdown <= 0) spawnCountdown = maxBoatSpawnDelay;
            }

            deadBoats.Clear();

            foreach (AnswerBlock block in blocks)
                block.Update(deltaTime);

            Draw();
        }

        private void Draw()
        {
            graphics.FillRectangle(Brushes.White, 0, 0, Program.Width, Program.Height);

            foreach (Boat boat in boats)
                boat.Draw(graphics);

            foreach (AnswerBlock block in blocks)
                block.Draw(graphics);

            water.Draw(graphics);
        }

    }
}
