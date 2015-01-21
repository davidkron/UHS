using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cycles.Utils
{
    class tryWhileFail
    {
        static int total_retries = 0;
        public delegate void Del();
        const int MAXTRIES = 4;

        public static void execute(Del function, bool thrw = true)
        {
            bool failed = true;
            int tries = 0;
            while (failed)
            {
                tries++;
                try
                {
                    function();
                    failed = false;
                }
                catch (System.Exception e)
                {
                    if (tries >= MAXTRIES)
                    {
                        if (thrw)
                        {
                            System.Diagnostics.Debug.WriteLine("EXCEPTION ROOF");
                            System.Diagnostics.Debug.WriteLine(e);
                            throw;
                        }
                        else if (total_retries < MAXTRIES)
                        {
                            total_retries++;
                            tries = 0;
                        }
                        else
                        {
                            thrw = true;
                        }
                    }
                }
            }
        }
    }
}
