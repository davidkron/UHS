using EnvDTE;
using Microsoft.VisualStudio.VCCodeModel;
using System;

namespace CodeGenerator.Converting.CodeHolders
{

    public class ClassHolder : GenericHolder, Addable<VCCodeClass>, Addable<VCCodeEnum>, Addable<VCCodeFunction>
        , Addable<VCCodeStruct>,Addable<VCCodeBase>, Addable<VCCodeVariable>
    {
        public string sourcefile;
        public bool isTemplate;

        public ClassHolder(VCCodeStruct str, string sourcefile)
            : base(str, typeof(VCCodeStruct))
        {
            init(str.IsTemplate, sourcefile);
        }

        public ClassHolder(VCCodeClass cls, string sourcefile)
            : base(cls, typeof(VCCodeClass))
        {
            init(cls.IsTemplate, sourcefile);
        }

        public void init(bool istemplate,string sourcefile)
        {
            this.sourcefile = sourcefile;
            isTemplate = istemplate;
            string name = vcInterface.Name;
            System.Diagnostics.Debug.WriteLine("NAME: " + name + " Templated: " + isTemplate.ToString());
        }


        public VCCodeClass add(VCCodeClass t)
        {
            EnvDTE.vsCMAccess Access = vsCMAccess.vsCMAccessPrivate;
            object interfaces = t.ImplementedInterfaces.Count > 0 ? t.ImplementedInterfaces : null;
            return AddClass(t.Name, null, interfaces, Access) as VCCodeClass;
        }

        public VCCodeEnum add(VCCodeEnum t)
        {
            return AddEnum(t.Name, t.Bases, t.Access) as VCCodeEnum;
        }

        public VCCodeFunction add(VCCodeFunction oldfunc)
        {
            var functionkind = oldfunc.FunctionKind;
            functionkind &= ~vsCMFunction.vsCMFunctionInline;
            var location = sourcefile;
            if (isTemplate)
                location = null;
            VCCodeFunction fun = AddFunction(oldfunc.Name, functionkind, oldfunc.Type, oldfunc.Access, location);
            fun.IsDelete = oldfunc.IsDelete;
            sync();
            return fun;
        }

        public VCCodeStruct add(VCCodeStruct t)
        {
            EnvDTE.vsCMAccess Access = vsCMAccess.vsCMAccessPrivate;
            object interfaces = t.ImplementedInterfaces.Count > 0 ? t.ImplementedInterfaces : null;
            return AddStruct(t.Name, null, interfaces, Access) as VCCodeStruct;
        }

        public VCCodeBase add(VCCodeBase _base)
        {
            return AddBase(_base.DisplayName,_base.Access);
        }

        public VCCodeVariable add(VCCodeVariable t)
        {
            dynamic initExpression = t.InitExpression;
            String location = sourcefile;
            if (!t.IsShared || isTemplate)
                location = null;

            var variable = AddVariable(t.Name, t.Type, t.Access, location) as VCCodeVariable;
            variable.IsConstant = t.IsConstant;
            variable.InitExpression = initExpression;
            return variable;
        }
    }
}