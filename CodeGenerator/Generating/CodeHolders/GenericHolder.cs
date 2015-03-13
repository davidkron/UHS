using CodeGenerator.Converting.CodeHolders;
using EnvDTE;
using Microsoft.VisualStudio.VCCodeModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGenerator.Converting
{
    public class GenericHolder : CodeHolder
    {
        public dynamic vcInterface;
        public GenericHolder(dynamic vcCodeInterface, System.Type type)
            : base(type)
        {
            vcInterface = vcCodeInterface;
        }

        public VCCodeFunction AddFunction(string name, vsCMFunction kind, CodeTypeRef type, vsCMAccess access, string location)
        {
            return vcInterface.AddFunction(name, kind, type, -1, access, location) as VCCodeFunction;
        }

        public VCCodeVariable AddVariable(string name, CodeTypeRef type, vsCMAccess access, string location)
        {
            return vcInterface.AddVariable(name, type, -1, access, location) as VCCodeVariable;
        }

        public VCCodeEnum AddEnum(string name, CodeElements bases, vsCMAccess vsCMAccess)
        {
            return vcInterface.AddEnum(name, -1, bases, vsCMAccess) as VCCodeEnum;
        }

        public VCCodeStruct AddStruct(string name, object bases, object interfaces, vsCMAccess vsCMAccess)
        {
            return vcInterface.AddStruct(name, -1, bases, interfaces, vsCMAccess) as VCCodeStruct; ;
        }

        public VCCodeClass AddClass(string name, object bases, object interfaces, vsCMAccess vsCMAccess)
        {
            return vcInterface.AddClass(name, -1, bases, interfaces, vsCMAccess) as VCCodeClass; ;
        }

        public void sync()
        {
            (vcInterface.ProjectItem.FileCodeModel as VCFileCodeModel).Synchronize();
        }

        public VCCodeBase AddBase(string name, vsCMAccess vsCMAccess)
        {
            VCCodeBase _base = vcInterface.AddBase(name, -1) as VCCodeBase;
            _base.Access = vsCMAccess;
            return _base;
        }
    }
}
