using EnvDTE;
using Microsoft.VisualStudio.VCCodeModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cycles.Converting.interfaces
{
    public class NamespaceInterface : CodeHolder2<VCCodeNamespace>, Addable<VCCodeClass>, Addable<VCCodeEnum>, Addable<VCCodeFunction>
        , Addable<VCCodeStruct>, Addable<VCCodeVariable>, Addable<VCCodeNamespace>
    {
        public string sourcefile;

        public NamespaceInterface(VCCodeNamespace cls, string sourcefile)
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

        public VCCodeEnum add(VCCodeEnum oldEnum)
        {
                var enm = vcInterface.AddEnum(oldEnum.Name, -1, oldEnum.Bases, oldEnum.Access) as VCCodeEnum;
                foreach (VCCodeAttribute att in oldEnum.Attributes)
                    enm.AddAttribute(att.Name, att.Value);
                foreach (VCCodeVariable memb  in oldEnum.Members)
                    enm.AddMember(memb.Name,memb.InitExpression);
                return enm;
        }

        public VCCodeFunction add(VCCodeFunction t)
        {
            return vcInterface.AddFunction(t.Name, t.FunctionKind, t.Type, -1, t.Access) as VCCodeFunction; 
        }

        public VCCodeStruct add(VCCodeStruct t)
        {
            return vcInterface.AddStruct(t.Name, -1, t.Bases, t.ImplementedInterfaces, t.Access) as VCCodeStruct;
        }
        public VCCodeNamespace add(VCCodeNamespace nspace)
        {
            return vcInterface.AddNamespace(nspace.Name, -1) as VCCodeNamespace;
        }

        public VCCodeVariable add(VCCodeVariable t)
        {
            var v = vcInterface.AddVariable(t.Name, t.Type, -1, t.Access) as VCCodeVariable;
            v.IsConstant = t.IsConstant;
            return v;
        }
    }
}
