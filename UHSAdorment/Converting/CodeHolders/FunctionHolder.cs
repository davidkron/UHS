using Cycles.Converting.CodeHolders;
using Cycles.Utils;
using Microsoft.VisualStudio.VCCodeModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UHSAdorment.Converting
{
    class FunctionHolder : SpecificHolder<VCCodeFunction>, Addable<VCCodeParameter>
    {
        public FunctionHolder(VCCodeFunction func)
            : base(func)
        {

        }

        public VCCodeParameter add(VCCodeParameter t)
        {
            return vcInterface.AddParameter(t.Name, t.Type, -1) as VCCodeParameter;
        }
    }
}
