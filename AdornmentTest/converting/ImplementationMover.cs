using EnvDTE;
using Microsoft.VisualStudio.VCCodeModel;
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
            bool failed = true;

            tryWhileFail.execute(() =>
            {
                start = oldfunc.GetStartPoint(vsCMPart.vsCMPartBodyWithDelimiter);
                end = oldfunc.GetEndPoint(vsCMPart.vsCMPartBodyWithDelimiter);

                namestart = oldfunc.GetStartPoint();
            });

            string content = oldfunc.BodyText;
            VCCodeFunction sourcefunction = null;
            failed = true;

            if (oldfunc.FunctionKind.HasFlag(vsCMFunction.vsCMFunctionConstructor) && parent != null && parent.Kind == vsCMElement.vsCMElementClass)
            {
                sourcefunction = (parent as VCCodeClass).AddFunction(oldfunc.Name, vsCMFunction.vsCMFunctionConstructor, null, -1, oldfunc.Access, source.Name) as VCCodeFunction;
            }
            else
                sourcefunction = (source.FileCodeModel as VCFileCodeModel).AddFunction(oldfunc.FullName, oldfunc.FunctionKind, oldfunc.Type, -1, oldfunc.Access) as VCCodeFunction;
            failed = false;


            foreach (VCCodeParameter param in oldfunc.Parameters)
            {
                sourcefunction.AddParameter(param.Name, param.Type, -1);
            }

            foreach (VCCodeParameter param in oldfunc.TemplateParameters)
            {
                sourcefunction.AddTemplateParameter(param.Name, param.Type, -1);
            }

            sourcefunction.BodyText = content;

            EditPoint implementation = start.CreateEditPoint();
            implementation.Delete(end);
            implementation.Insert(";");

            /*if (oldfunc.Parent == null || oldfunc.Parent as CodeClass == null)
            {
                failed = true;
                while (failed)
                {
                    try
                    {
                        namestart.CreateEditPoint().Insert("extern ");
                        failed = false;
                    }
                    catch (System.Exception e)
                    { }
                }
            }*/
        }
    }
}
