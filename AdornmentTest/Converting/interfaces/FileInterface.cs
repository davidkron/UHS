using Cycles.Converting.interfaces;
using EnvDTE;
using Microsoft.VisualStudio.VCCodeModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cycles.Converting
{
    class FileInterface : CodeHolder2<VCFileCodeModel>,Addable<VCCodeVariable>
        , Addable<VCCodeNamespace>, Addable<VCCodeClass>, Addable<VCCodeEnum>, Addable<VCCodeMacro>, Addable<VCCodeFunction>, Addable<VCCodeStruct>, Addable<VCCodeInclude>
    {
        public FileInterface(VCFileCodeModel cls)
            : base(cls)
        {
        }

        public VCCodeClass add(VCCodeClass t)
        {
            EnvDTE.vsCMAccess Access = vsCMAccess.vsCMAccessPrivate;
            object interfaces = t.ImplementedInterfaces.Count > 0 ? t.ImplementedInterfaces : null;
            var vcClass = vcInterface.AddClass(t.Name, -1, null, interfaces, Access) as VCCodeClass;
            tryWhileFail.execute(() =>
            {
                foreach (VCCodeParameter param in t.TemplateParameters)
                {
                    vcClass.AddTemplateParameter(param.Name, param.Type, -1);
                }
            });
            return vcClass;
        }

        public VCCodeInclude add(VCCodeInclude vCCodeInclude)
        {
            return vcInterface.AddInclude(vCCodeInclude.DisplayName);
        }

        public VCCodeStruct add(VCCodeStruct t)
        {
            EnvDTE.vsCMAccess Access = vsCMAccess.vsCMAccessPrivate;
            object interfaces = t.ImplementedInterfaces.Count > 0 ? t.ImplementedInterfaces : null;
            return vcInterface.AddStruct(t.Name, -1, null, interfaces, Access) as VCCodeStruct;
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

        public VCCodeNamespace add(VCCodeNamespace nspace)
        {
            return vcInterface.AddNamespace(nspace.Name, -1) as VCCodeNamespace;
        }

        public VCCodeFunction add(VCCodeFunction t)
        {
            return vcInterface.AddFunction(t.Name, t.FunctionKind, t.Type, -1, t.Access) as VCCodeFunction;
        }


        public VCCodeMacro add(VCCodeMacro macro)
        {
            string smacro = macro.StartPoint.CreateEditPoint().GetText(macro.EndPoint); 
            String value = smacro.Substring(macro.Name.Count() + "#define  ".Count()) + "\n";
            return vcInterface.AddMacro(macro.Name, value, -1);
        }

        public VCCodeVariable add(VCCodeVariable t)
        {
            var v = vcInterface.AddVariable(t.Name, t.Type, -1, t.Access) as VCCodeVariable;
            v.IsConstant = t.IsConstant;
            return v;
        }
    }
}
