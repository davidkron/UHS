using Cycles.Converting.interfaces;
using EnvDTE;
using Microsoft.VisualStudio.VCCodeModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cycles.Converting
{
    class StructInterface : CodeHolder2<VCCodeStruct>, Addable<VCCodeClass>, Addable<VCCodeEnum>, Addable<VCCodeFunction>
        , Addable<VCCodeStruct>, Addable<VCCodeVariable>
    {
        String sourcefile;

        public StructInterface(VCCodeStruct st,string sourcetarget) : base(st)
        {
            sourcefile = sourcetarget;
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

        public VCCodeStruct add(VCCodeStruct t)
        {
            throw new NotImplementedException();
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

        public VCCodeFunction add(VCCodeFunction t)
        {
            var functionkind = t.FunctionKind;
            functionkind &= ~vsCMFunction.vsCMFunctionInline;
            return vcInterface.AddFunction(t.Name, functionkind, t.Type, -1, t.Access, sourcefile) as VCCodeFunction; 
        }
    }
}
