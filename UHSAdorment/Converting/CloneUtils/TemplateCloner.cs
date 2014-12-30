using Microsoft.VisualStudio.VCCodeModel;
using System;
using System.Collections.Generic;

namespace Cycles.Converting.CloneUtils
{
    internal class TemplateCloner
    {
        public static void CloneTemplates(VCCodeClass from, dynamic to)
        {
            if (from.IsTemplate)
            {
                string templatestring = "template <typename ";
                List<string> templates = new List<string>();
                foreach (VCCodeParameter param in from.TemplateParameters)
                {
                    templates.Add(param.Type.AsFullName);
                }
                templatestring += String.Join(", typename ", templates) + ">\r\n";
                to.StartPoint.CreateEditPoint().Insert(templatestring);
            }
        }
    }
}