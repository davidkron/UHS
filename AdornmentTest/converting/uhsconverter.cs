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
                    newelem = headertarget.add(func);//.AddFunction(func.Name, func.FunctionKind, func.Type, func.Access, sourcetarget.Name) as VCCodeElement;
                    VCCodeFunction newFunc = newelem as VCCodeFunction;
                    //tryWhileFail.execute(() =>
                    { (newelem as VCCodeFunction).BodyText = func.BodyText; }
                    //);
                    foreach (VCCodeAttribute attrib in func.Attributes)
                        newFunc.AddAttribute(attrib.Name, attrib.Value);
                    foreach (VCCodeParameter param in func.Parameters)
                        newFunc.AddParameter(param.Name, param.Type, -1);
                    if (! (headertarget is ClassInterface)
                        && !(headertarget is StructInterface))//TODO: Not needed as long as struct derives class
                    {
                            ImplementationMover.moveImplementation(newFunc, sourcetarget);
                    }

                    break;

                case vsCMElement.vsCMElementClass:
                    VCCodeClass cs = elem as VCCodeClass;
                    newelem = headertarget.add(cs) as VCCodeElement;
                    break;
                case vsCMElement.vsCMElementNamespace:
                    newelem = headertarget.add(elem as VCCodeNamespace) as VCCodeElement;
                    break;

                case vsCMElement.vsCMElementVariable:
                    VCCodeVariable v = elem as VCCodeVariable;
                    VCCodeVariable headerVar = headertarget.add(v) as VCCodeVariable;//(v.Name, v.Type, v.Access, sourcetarget.Name, v.IsShared, v.IsConstant);
                    //v2.InitExpression = v.InitExpression;
                    if (headertarget is FileInterface)
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
                    (headertarget as ClassInterface).vcInterface.AddBase(_base.DisplayName, -1);
                    break;
                case vsCMElement.vsCMElementIncludeStmt:
                    newelem = headertarget.add(elem as VCCodeInclude);//addInclude(elem as VCCodeInclude);
                    break;
                case vsCMElement.vsCMElementMacro:
                    newelem = headertarget.add(elem as VCCodeMacro);//addMacro(elem as VCCodeMacro);
                    break;
                case vsCMElement.vsCMElementEnum:
                    newelem = (VCCodeElement)headertarget.add(elem as VCCodeEnum);
                    break;
                case vsCMElement.vsCMElementStruct:

                    VCCodeStruct cstruct = elem as VCCodeStruct;
                    newelem = headertarget.add(elem as VCCodeStruct);//AddStruct(cstruct.Name, vsCMAccess.vsCMAccessPrivate, null, cstruct.ImplementedInterfaces.Count > 0 ? cstruct.ImplementedInterfaces : null) as VCCodeElement;
                    break;
                default:
                    System.Diagnostics.Debug.WriteLine(elem.Kind.ToString());
                    throw new System.NotImplementedException("U need to handle dis one David" + elem.Kind.ToString());
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
                    parseitem(el, sourcetarget, CodeHolder.newHolder(newelem, sourcetarget.Name));// new CodeHolder(newelem));
                System.Diagnostics.Debug.WriteLine(el.Name + " " + el.Kind);
            }
        }
    }
}
