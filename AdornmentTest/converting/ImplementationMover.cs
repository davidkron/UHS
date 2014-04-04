using EnvDTE;
using Microsoft.VisualStudio.VCCodeModel;
using Microsoft.VisualStudio.VCProjectEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cycles.converting
{
    class ImplementationMover
    {
        public static void moveImplementation(VCCodeFunction oldfunc, ProjectItem source, VCCodeElement parent = null)
        {
            TextPoint start = null, end = null, namestart = null;

            tryWhileFail.execute(() =>
            {
                start = oldfunc.GetStartPoint(vsCMPart.vsCMPartBodyWithDelimiter);
                end = oldfunc.GetEndPoint(vsCMPart.vsCMPartBodyWithDelimiter);

                namestart = oldfunc.GetStartPoint();
            });

            string content = oldfunc.BodyText;
            VCCodeFunction sourcefunction = null;
            
            if (oldfunc.FunctionKind.HasFlag(vsCMFunction.vsCMFunctionConstructor) && parent != null && parent.Kind == vsCMElement.vsCMElementClass)
            {
                sourcefunction = (parent as VCCodeClass).AddFunction(oldfunc.Name, vsCMFunction.vsCMFunctionConstructor, null, -1, oldfunc.Access, source.Name) as VCCodeFunction;
            }
            else
                sourcefunction = (source.FileCodeModel as VCFileCodeModel).AddFunction(oldfunc.FullName, oldfunc.FunctionKind, oldfunc.Type, -1, oldfunc.Access) as VCCodeFunction;
            
            foreach (VCCodeParameter param in oldfunc.Parameters)
            {
                sourcefunction.AddParameter(param.Name, param.Type, -1);
            }

            foreach (VCCodeParameter param in oldfunc.TemplateParameters)
            {
                sourcefunction.AddTemplateParameter(param.Name, param.Type, -1);
            }

            tryWhileFail.execute(() =>
            {
                sourcefunction.BodyText = content;
            });

            EditPoint implementation = start.CreateEditPoint();
            implementation.Delete(end);
            implementation.Insert(";");

            string uhs = oldfunc.ProjectItem.Name;
            string header = uhs.Remove(uhs.Length - 2) + ".h";
            //(source.FileCodeModel as VCFileCodeModel).AddInclude(header);
        }

        public static void addExtern(VCCodeElement elem)
        {
                elem.GetStartPoint().CreateEditPoint().Insert("extern ");
        }

        public static void moveImplementation(VCCodeVariable v, ProjectItem sourcetarget)
        {
            var v2 = (sourcetarget as VCFileCodeModel).AddVariable(v.Name, v.Type, -1, v.Access);
            addExtern((VCCodeElement)v2);
        }
    }
}
