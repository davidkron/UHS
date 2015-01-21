using Cycles.Utils;
using EnvDTE;
using Microsoft.VisualStudio.VCCodeModel;

namespace Cycles.Converting.CloneUtils
{
    internal class ImplementationMover
    {
        public static void moveImplementation(VCCodeFunction oldfunc, ProjectItem source)
        {
            TextPoint start = null, end = null, namestart = null;

            tryWhileFail.execute(() =>
            {
                start = oldfunc.GetStartPoint(vsCMPart.vsCMPartBodyWithDelimiter);
                end = oldfunc.GetEndPoint(vsCMPart.vsCMPartBodyWithDelimiter);

                namestart = oldfunc.GetStartPoint();
            });

            string content = oldfunc.BodyText;
            VCCodeFunction sourcefunction = (source.FileCodeModel as VCFileCodeModel).AddFunction(oldfunc.FullName, oldfunc.FunctionKind, oldfunc.Type, -1, oldfunc.Access) as VCCodeFunction;

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

            EditPoint headerImplementation = start.CreateEditPoint();
            headerImplementation.Delete(end);
            headerImplementation.Insert(";");
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