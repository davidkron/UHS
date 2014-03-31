using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cycles
{
    class tryWhileFail
    {
        public delegate void Del();
        const int MAXTRIES = 20;

        public static void execute(Del function)
        {
            bool failed = true;
            int tries = 0;
            while (failed)
            {
                try
                {
                    function();
                    failed = false;
                    tries++;
                }
                catch (System.Exception e)
                {
                    if (tries >= MAXTRIES)
                        throw e;
                    System.Diagnostics.Debug.WriteLine(e);
                }
            }
        }
    }
}
