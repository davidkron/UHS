using EnvDTE;
using Microsoft.VisualStudio.VCCodeModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cycles.Converting.interfaces
{
    public class ClassInterface : CodeHolder2<VCCodeClass>, Addable<VCCodeClass>, Addable<VCCodeEnum>, Addable<VCCodeFunction>
        , Addable<VCCodeStruct>, Addable<VCCodeVariable>
    {
        public string sourcefile;

        public ClassInterface(VCCodeClass cls, string sourcefile)
            : base(cls)
        {
            this.sourcefile = sourcefile;
        }

        public VCCodeClass add(VCCodeClass t)
        {
            EnvDTE.vsCMAccess Access = vsCMAccess.vsCMAccessPrivate;
            object interfaces = t.ImplementedInterfaces.Count > 0 ? t.ImplementedInterfaces : null;
            return vcInterface.AddClass(t.Name, -1, null, interfaces, Access) as VCCodeClass;
        }

        public VCCodeEnum add(VCCodeEnum t)
        {
            VCCodeEnum enm = vcInterface.AddEnum(t.Name, -1, t.Bases, t.Access) as VCCodeEnum;

            foreach (VCCodeAttribute att in t.Attributes)
                enm.AddAttribute(att.Name, att.Value);
            foreach (VCCodeVariable memb in t.Members)
                enm.AddMember(memb.Name, memb.InitExpression);

            return enm;
        }

        public VCCodeFunction add(VCCodeFunction t)
        {
            var functionkind = t.FunctionKind;
            functionkind &= ~vsCMFunction.vsCMFunctionInline;
            return vcInterface.AddFunction(t.Name, functionkind, t.Type, -1, t.Access, sourcefile) as VCCodeFunction; 
        }

        public VCCodeStruct add(VCCodeStruct t)
        {
            EnvDTE.vsCMAccess Access = vsCMAccess.vsCMAccessPrivate;
            object interfaces = t.ImplementedInterfaces.Count > 0 ? t.ImplementedInterfaces : null;
            return vcInterface.AddStruct(t.Name, -1, null, interfaces, Access) as VCCodeStruct;
        }

        public VCCodeVariable add(VCCodeVariable t)
        {
            String location = sourcefile;
            if (!t.IsShared)
                location = null;

            var v = vcInterface.AddVariable(t.Name, t.Type, -1, t.Access, location) as VCCodeVariable;
            v.IsConstant = t.IsConstant;
            return v;
        }
    }
}
