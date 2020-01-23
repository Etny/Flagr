using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Flagr.States
{
    abstract class State
    {

        public virtual void Update(DeltaTime deltaTime) 
        {
           // Program.AppForm.Redraw();
            Program.AppForm.Redraw();
        }


    }
}
