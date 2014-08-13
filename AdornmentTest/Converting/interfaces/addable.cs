using Microsoft.VisualStudio.VCCodeModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cycles.Converting.interfaces
{

    interface Addable<T>
    {
        T add(T t);
    }
}
