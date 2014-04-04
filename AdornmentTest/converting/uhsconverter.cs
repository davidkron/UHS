using EnvDTE;
using Microsoft.VisualStudio.VCCodeModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cycles.converting
{
    class uhsconverter
    {
        //Parses uhs items and generates both headers and source items

        public static void parseitem(VCCodeElement elem, ProjectItem sourcetarget, CodeHolder headertarget)
        {
            VCCodeElement newelem = null;

            switch (elem.Kind)
            {
                case vsCMElement.vsCMElementFunction:
                    VCCodeFunction func = elem as VCCodeFunction;
                    newelem = headertarget.AddFunction(func.Name, func.FunctionKind, func.Type, func.Access, sourcetarget.Name) as VCCodeElement;
                    (newelem as VCCodeFunction).BodyText = func.BodyText;
                    foreach (VCCodeAttribute attrib in func.Attributes)
                        (newelem as VCCodeFunction).AddAttribute(attrib.Name, attrib.Value);
                    foreach (VCCodeParameter param in func.Parameters)
                        (newelem as VCCodeFunction).AddParameter(param.Name, param.Type);
                    if (headertarget.kind != CodeHolder.holdkind.vcclass)
                    {
                        ImplementationMover.moveImplementation(newelem as VCCodeFunction, sourcetarget);
                    }

                    break;

                case vsCMElement.vsCMElementClass:
                    VCCodeClass cs = elem as VCCodeClass;
                    newelem = headertarget.AddClass(cs.Name, vsCMAccess.vsCMAccessPrivate, null, cs.ImplementedInterfaces.Count > 0 ? cs.ImplementedInterfaces : null) as VCCodeElement;

                    break;

                case vsCMElement.vsCMElementNamespace:
                    VCCodeNamespace ns = elem as VCCodeNamespace;

                    newelem = headertarget.AddNamespace(ns.Name) as VCCodeElement;
                    break;

                case vsCMElement.vsCMElementVariable:
                    VCCodeVariable v = elem as VCCodeVariable;
                    VCCodeVariable headerVar = headertarget.AddVariable(v.Name, v.Type, v.Access, sourcetarget.Name, v.IsShared, v.IsConstant);
                    //v2.InitExpression = v.InitExpression;
                    if (headertarget.kind == CodeHolder.holdkind.vcfile)
                    {
                        VCCodeVariable sourceVar = (sourcetarget.FileCodeModel as VCFileCodeModel).AddVariable(v.Name, v.Type, -1, v.Access) as VCCodeVariable;
                        ImplementationMover.addExtern((VCCodeElement)headerVar);
                        sourceVar.InitExpression = v.InitExpression;
                    }
                    else
                    {
                        headerVar.InitExpression = v.InitExpression;
                    }
                    
                    break;

                case vsCMElement.vsCMElementVCBase:
                    System.Diagnostics.Debug.Write(elem.Kind);
                    VCCodeBase _base = elem as VCCodeBase;
                    System.Diagnostics.Debug.Write(_base.Access);
                    headertarget.getClass().AddBase(_base.DisplayName, -1);
                    break;
                case vsCMElement.vsCMElementIncludeStmt:
                    newelem = headertarget.addInclude(elem as VCCodeInclude);
                    break;
                case vsCMElement.vsCMElementMacro:
                    newelem = headertarget.addMacro(elem as VCCodeMacro);
                    break;
                case vsCMElement.vsCMElementEnum:
                    newelem = (VCCodeElement)headertarget.AddEnum(elem as VCCodeEnum);
                    break;
                default:
                    System.Diagnostics.Debug.WriteLine(elem.Kind.ToString());
                    throw new System.Exception("U need to handle dis one David" + elem.Kind.ToString());
            }


            System.Collections.IEnumerator num = null;
            tryWhileFail.execute(() =>
            {
                num = elem.Children.GetEnumerator();
            });
            while (num.MoveNext())
            {
                VCCodeElement el = num.Current as VCCodeElement;
                if (newelem.Kind != vsCMElement.vsCMElementFunction &&
                    newelem.Kind != vsCMElement.vsCMElementEnum)
                    parseitem(el, sourcetarget, new CodeHolder(newelem));
                System.Diagnostics.Debug.WriteLine(el.Name + " " + el.Kind);
            }
        }
    }
}
