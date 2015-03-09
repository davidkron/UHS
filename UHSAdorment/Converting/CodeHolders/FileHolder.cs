using Cycles.Converting.CodeHolders;
using Cycles.Utils;
using EnvDTE;
using Microsoft.VisualStudio.VCCodeModel;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Cycles.Converting.CodeHolders
{
    internal class FileHolder : SpecificHolder<VCFileCodeModel>, Addable<VCCodeVariable>
        , Addable<VCCodeNamespace>, Addable<VCCodeClass>, Addable<VCCodeEnum>, Addable<VCCodeMacro>, Addable<VCCodeFunction>, Addable<VCCodeStruct>, Addable<VCCodeInclude>
    {
        public FileHolder(VCFileCodeModel cls)
            : base(cls)
        {
        }

        public VCCodeClass add(VCCodeClass t)
        {
            EnvDTE.vsCMAccess Access = vsCMAccess.vsCMAccessPrivate;
            object interfaces = t.ImplementedInterfaces.Count > 0 ? t.ImplementedInterfaces : null;
            var vcClass = vcInterface.AddClass(t.Name, -1, null, interfaces, Access) as VCCodeClass;
            CloneUtils.TemplateCloner.CloneTemplates(t, vcClass);
            vcInterface.Synchronize();
            return vcClass;
        }

        public VCCodeStruct add(VCCodeStruct t)
        {
            EnvDTE.vsCMAccess Access = vsCMAccess.vsCMAccessPrivate;
            object interfaces = t.ImplementedInterfaces.Count > 0 ? t.ImplementedInterfaces : null;
            return vcInterface.AddStruct(t.Name, -1, null, interfaces, Access) as VCCodeStruct;
        }

        public VCCodeInclude add(VCCodeInclude vCCodeInclude)
        {
            TextPoint End = vCCodeInclude.EndPoint;
            var endEdit = End.CreateEditPoint();
            endEdit.CharLeft();

            if (endEdit.GetText(End) == ">"){
                return vcInterface.AddInclude("<" + vCCodeInclude.FullName + ">");
            }

            return vcInterface.AddInclude(vCCodeInclude.DisplayName);
        }


        public VCCodeEnum add(VCCodeEnum t)
        {
            return vcInterface.AddEnum(t.Name, -1, t.Bases, t.Access) as VCCodeEnum;
        }

        public VCCodeNamespace add(VCCodeNamespace nspace)
        {
            return vcInterface.AddNamespace(nspace.Name, -1) as VCCodeNamespace;
        }

        public VCCodeFunction add(VCCodeFunction t)
        {
            VCCodeFunction func = vcInterface.AddFunction(t.Name, t.FunctionKind, t.Type, -1, t.Access) as VCCodeFunction;
            
            //func.AddTemplateParameter("T", null ,- 1);
            //tryWhileFail.execute(() =>
            //{
            //    foreach (VCCodeParameter param in t.TemplateParameters)
            //    {
            //        func.AddTemplateParameter(param.Name, t.Type, -1);
            //    }
            //});
            return func;
        }

        public VCCodeMacro add(VCCodeMacro macro)
        {
            string smacro = macro.StartPoint.CreateEditPoint().GetText(macro.EndPoint);
            int start = macro.Name.Count() + "#define  ".Count();
            int total_argument_length = 0;
            foreach (VCCodeParameter param in macro.Parameters){
                total_argument_length += param.Name.Length;
            }
            if (macro.Parameters.Count> 0)
            {
                int commas = macro.Parameters.Count- 1;
                int parantheses = 2;
                start += total_argument_length + commas + parantheses;
            }
            string value = smacro.Substring(start) + "\n";
            VCCodeMacro newMacro = vcInterface.AddMacro(macro.Name, value, -1);
            foreach (VCCodeParameter param in macro.Parameters)
            {
                newMacro.AddParameter(param.Name);
            }
            return newMacro;
        }

        public VCCodeVariable add(VCCodeVariable t)
        {
            var v = vcInterface.AddVariable(t.Name, t.Type, -1, t.Access) as VCCodeVariable;
            v.IsConstant = t.IsConstant;
            return v;
        }
    }
}