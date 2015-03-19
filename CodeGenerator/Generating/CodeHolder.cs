using EnvDTE;
using Microsoft.VisualStudio.VCCodeModel;
using System;

namespace CodeGenerator.Converting.CodeHolders
{
    public class CodeHolder
    {
        public System.Type holdingType;

        public CodeHolder()
        {
        }

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

        public static CodeHolder newHolder(VCCodeElement NewElem, string sourcetarget)
		{
			switch (NewElem.Kind)
			{
                case vsCMElement.vsCMElementClass:
                    return new ClassHolder(NewElem as VCCodeClass, sourcetarget);
                case vsCMElement.vsCMElementStruct:
                    return new ClassHolder(NewElem as VCCodeStruct, sourcetarget);
                case vsCMElement.vsCMElementNamespace:
                    return new NamespaceHolder(NewElem as VCCodeNamespace);
                case vsCMElement.vsCMElementFunction:
                    return new FunctionHolder(NewElem as VCCodeFunction);
                case vsCMElement.vsCMElementEnum:
                    return new EnumHolder(NewElem as VCCodeEnum, sourcetarget);
                default:
                    return new EmptyHolder(NewElem.Kind);
            }
        }

        public static CodeHolder newHolder(VCFileCodeModel NewElem)
        {
            return new FileHolder(NewElem);
        }
    };

    class EmptyHolder : CodeHolder
    {
        public EmptyHolder(vsCMElement kind)
        {
            holdingType = typeof(EmptyHolder);
        }
    }

    public class SpecificHolder<holdType> : CodeHolder where holdType : class
    {
        public holdType vcInterface;
        public SpecificHolder(holdType data)
            : base(typeof(holdType))
        {
            vcInterface = data;
        }

        public SpecificHolder(VCCodeElement data)
            : base(typeof(holdType))
        {
            vcInterface = data as holdType;
        }
    };
}
