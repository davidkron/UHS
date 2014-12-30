using Cycles.Converting.CloneUtils;
using Cycles.Converting.CodeHolders;
using Cycles.Utils;
using EnvDTE;
using Microsoft.VisualStudio.VCCodeModel;

namespace Cycles.Converting
{
    internal class UHSConverter
    {
        //Parses uhs items and generates both headers and source items
        public static void parseitem(VCCodeElement elem, ProjectItem sourcetarget, CodeHolder headertarget)
        {
            VCCodeElement newelem = null;
            string functionbody = "";

            switch (elem.Kind)
            {
                case vsCMElement.vsCMElementParameter:
                    newelem = headertarget.add(elem as VCCodeParameter);
                    return;

                case vsCMElement.vsCMElementFunction:
                    VCCodeFunction func = elem as VCCodeFunction;
                    newelem = headertarget.add(func);
                    
                    tryWhileFail.execute(()=>{
                        functionbody = func.BodyText;
                    });

                    break;

                case vsCMElement.vsCMElementClass:
                    VCCodeClass cs = elem as VCCodeClass;
                    newelem = headertarget.add(cs);
                    break;

                case vsCMElement.vsCMElementNamespace:
                    newelem = headertarget.add(elem as VCCodeNamespace);
                    break;

                case vsCMElement.vsCMElementVariable:
                    VCCodeVariable v = elem as VCCodeVariable;
                    VCCodeVariable headerVar = headertarget.add(v) as VCCodeVariable;
                    if (headertarget is FileHolder)
                    {
                        VCCodeVariable sourceVar = (sourcetarget.FileCodeModel as VCFileCodeModel).AddVariable(v.Name, v.Type, -1, v.Access) as VCCodeVariable;
                        ImplementationMover.addExtern((VCCodeElement)headerVar);
                        sourceVar.InitExpression = v.InitExpression;
                    }

                    return;

                case vsCMElement.vsCMElementAttribute:
                    newelem = (headertarget).add(elem as VCCodeAttribute);
                    break;

                case vsCMElement.vsCMElementVCBase:
                    newelem = (headertarget).add(elem as VCCodeBase);
                    break;

                case vsCMElement.vsCMElementIncludeStmt:
                    newelem = headertarget.add(elem as VCCodeInclude);
                    break;

                case vsCMElement.vsCMElementMacro:
                    newelem = headertarget.add(elem as VCCodeMacro);
                    break;

                case vsCMElement.vsCMElementEnum:
                    newelem = headertarget.add(elem as VCCodeEnum);
                    break;

                case vsCMElement.vsCMElementStruct:
                    VCCodeStruct cstruct = elem as VCCodeStruct;
                    newelem = headertarget.add(elem as VCCodeStruct);
                    break;

                default:
                    throw new System.NotImplementedException("U need to handle dis one David" + elem.Kind.ToString());
            }

            if (newelem == null)
            {
                throw new System.NotImplementedException("Failed adding" + elem.Kind.ToString() + " to a " + headertarget.holdingType);
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("Added: " + elem.Kind.ToString() + " to a " + headertarget.holdingType);
            }

            // Iterate children
            ParseChildren(elem, sourcetarget, newelem);

            if (newelem.Kind == vsCMElement.vsCMElementFunction)
            {
                ImplementationMover.moveImplementation(newelem as VCCodeFunction, headertarget, sourcetarget, functionbody);
            }
        }

        private static void ParseChildren(VCCodeElement elem, ProjectItem sourcetarget, VCCodeElement newelem)
        {
            System.Collections.IEnumerator num = null;
            tryWhileFail.execute(() =>
            {
                num = elem.Children.GetEnumerator();
            });
            var holder = CodeHolder.newHolder(newelem, sourcetarget.Name);
            while (num.MoveNext())
            {
                VCCodeElement el = num.Current as VCCodeElement;
                System.Diagnostics.Debug.WriteLine("Entering: " + el.Name + " of type " + el.Kind);
                parseitem(el, sourcetarget, holder);
            }
        }
    }
}