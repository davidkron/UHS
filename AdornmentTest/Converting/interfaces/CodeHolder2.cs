using Cycles.Converting;
using EnvDTE;
using Microsoft.VisualStudio.VCCodeModel;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cycles.Converting.interfaces
{
    public class CodeHolder
    {
        public System.Type holdingType;

        public CodeHolder(System.Type holdingType)
        {
            this.holdingType = holdingType;
        }

        public VCCodeElement add<T>(T elem)
        {
            return t_add<T>(elem);
        }

        public VCCodeElement t_add<T>(T elem)
        {
            if (this is Addable<T>)
            {
                return (this as Addable<T>).add(elem) as VCCodeElement;
            }
            else throw new NotImplementedException("Cant add \"" + typeof(T) + "\" to a " + holdingType);
        }

        public static CodeHolder newHolder(VCCodeElement NewElem,string sourcetarget)
        {
            switch (NewElem.Kind)
            {
                case vsCMElement.vsCMElementClass:
                    return new ClassInterface(NewElem as VCCodeClass, sourcetarget);
                case vsCMElement.vsCMElementStruct:
                    return new StructInterface(NewElem as VCCodeStruct, sourcetarget);
                case vsCMElement.vsCMElementNamespace:
                    return new NamespaceInterface(NewElem as VCCodeNamespace, sourcetarget);
                case vsCMElement.vsCMElementFunction:
                    return null;
                default:
                    throw new System.ArgumentException("invalid type of parent: " + (NewElem as VCCodeElement).Kind);
            }
        }

        public static CodeHolder newHolder(VCFileCodeModel NewElem)
        {
            return new FileInterface(NewElem);
        }

        public static CodeHolder newHolder(VCCodeClass NewElem,string sourcetarget)
        {
            return new ClassInterface(NewElem, sourcetarget);
        }

        public static CodeHolder newHolder(VCCodeNamespace NewElem, string sourcetarget)
        {
            return new NamespaceInterface(NewElem, sourcetarget);
        }
    };

    public class CodeHolder2<holdType> : CodeHolder where holdType : class
    {
        public holdType vcInterface;
        private VCCodeStruct st;
        public CodeHolder2(holdType data)
            : base(typeof(holdType))
        {
            vcInterface = data;
        }

        public CodeHolder2(VCCodeElement data)
            : base(typeof(holdType))
        {
            vcInterface = data as holdType;
        }
    };
}
