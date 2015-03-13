using Microsoft.VisualStudio.VCCodeModel;

namespace CodeGenerator.Converting.CodeHolders
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
