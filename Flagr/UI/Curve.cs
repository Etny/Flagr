using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Flagr.UI
{
    class Curve
    {

        public float Range { get; set; } = 1;

        private float fraction = 1f / 3f;

        public float GetValue(float X)
        {
            X -= Range / 2f;
            X /= Range;
            
            //Yes, I know
            return ((X - ((4 * X * X * X) / 3)) + fraction) / (fraction + fraction);
        }

    }
}
