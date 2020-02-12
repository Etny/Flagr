using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Flagr.UI
{
    abstract class Curve
    {

        public float Range { get; set; } = 1;

        public abstract float GetValue(float f);

    }
}
