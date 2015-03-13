using Microsoft.VisualStudio.VCCodeModel;
using System;

namespace CodeGenerator.Converting.CodeHolders
{
    internal class EnumHolder : SpecificHolder<VCCodeEnum>, Addable<VCCodeVariable>, Addable<VCCodeAttribute>
    {
        private String sourcefile;

        public EnumHolder(VCCodeEnum en, string sourcetarget)
            : base(en)
        {
            sourcefile = sourcetarget;
        }

        public VCCodeAttribute add(VCCodeAttribute t)
        {
            return vcInterface.AddAttribute(t.Name, t.Value) as VCCodeAttribute;
        }


        public VCCodeVariable add(VCCodeVariable t)
        {
            return vcInterface.AddMember(t.Name, t.InitExpression);
        }
    }
}