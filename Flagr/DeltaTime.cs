using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Flagr
{
    struct DeltaTime
    {
        public float Seconds { get; set; }
        public int Milliseconds { get; set; }

        public DeltaTime(int Milliseconds)
        {
            this.Milliseconds = Milliseconds;
            this.Seconds = (float)(Milliseconds / 1000f);
        }

        public DeltaTime(float Seconds)
        {
            this.Seconds = Seconds;
            this.Milliseconds = (int)(Seconds * 1000);
        }

    }
}
