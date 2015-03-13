using CodeGenerator.Converting.CodeHolders;
using CodeGenerator.Utils;
using EnvDTE;
using Microsoft.VisualStudio.VCCodeModel;

namespace CodeGenerator.Converting.CloneUtils
{
    internal class ImplementationMover
    {
        public static void moveImplementation(VCCodeFunction oldfunc, CodeHolder headertarget, ProjectItem source, string body)
        {
            TextPoint start = null, end = null, namestart = null;
            body = body.Trim(new char[] { '\t', ' ' });

            if (headertarget is ClassHolder)
            {
                //The source function is created by automaticly visual studio
                oldfunc.BodyText = body;// Doesnt help prettyprinting -> oldfunc.CodeModel.Synchronize();
            }
            else
            {
                tryWhileFail.execute(() =>
                {
                    start = oldfunc.GetStartPoint(vsCMPart.vsCMPartBodyWithDelimiter);
                    end = oldfunc.GetEndPoint(vsCMPart.vsCMPartBodyWithDelimiter);

                    namestart = oldfunc.GetStartPoint();
                });

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
                    sourcefunction.BodyText = body;
                });

                EditPoint headerImplementation = start.CreateEditPoint();
                headerImplementation.Delete(end);
                headerImplementation.Insert(";");
            }
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