using System;
using EnvDTE;
using Microsoft.VisualStudio.VCCodeModel;
using Cycles.Converting;
using Microsoft.VisualStudio.VCProjectEngine;
using CodeGenerator.Utils;
using CodeGenerator.Converting.CodeHolders;

namespace Cycles
{
    public class CodeSplitter
    {
        private Project dteproj;
        private VCProject vcProj;
        public bool converting = false;

        public CodeSplitter(Project dteproj, VCProject vcProj)
        {
            this.dteproj = dteproj;
            this.vcProj = vcProj;
        }

        public void convert(VCFile file, VCFile h, VCFile s)
        {
            converting = true;

            ProjectItem header = h.Object as ProjectItem;
            ProjectItem source = s.Object as ProjectItem;

            System.IO.File.WriteAllText(h.FullPath, String.Empty);
            (source.FileCodeModel as VCFileCodeModel).StartPoint.CreateEditPoint().Delete(
                (source.FileCodeModel as VCFileCodeModel).EndPoint);
            (header.FileCodeModel as VCFileCodeModel).StartPoint.CreateEditPoint().Delete(
               (header.FileCodeModel as VCFileCodeModel).EndPoint);

            tryWhileFail.execute(() =>
            {
                vcProj.Save();
                dteproj.Save();
            }, false);

            EnvDTE.ProjectItem uhsfile = file.Object as ProjectItem;
            VCFileCodeModel vcfile = null, vcheader = null, vcsource = null;
            tryWhileFail.execute(() =>
            {
                vcfile = uhsfile.FileCodeModel as VCFileCodeModel;
                vcheader = header.FileCodeModel as VCFileCodeModel;
                vcsource = source.FileCodeModel as VCFileCodeModel;
            });

            if (vcfile == null)
            {
                throw new Exceptions.UHSNotCppException();
            }

            try
            {
                Generate(header, source, vcfile);
            }
            catch (Exception)
            {
                tryWhileFail.execute(() =>
                {
                    System.Diagnostics.Debug.WriteLine("Exception caught, have to reparse....");
                    System.IO.File.WriteAllText(h.FullPath, String.Empty);
                    Generate(header, source, vcfile);
                });
            }

            if (!System.IO.File.ReadAllText(h.FullPath).StartsWith("#pragma once"))
            {
                vcheader.StartPoint.CreateEditPoint().Insert("#pragma once\r\n");
            }

            bool hasHeader = false;
            foreach (VCCodeInclude inc in vcsource.Includes)
            {
                if (inc.Name == header.Name)
                {
                    hasHeader = true; break;
                }
            }
            if (!hasHeader)
                vcsource.AddInclude("\"" + header.Name + "\"");

            dteproj.Save();
            converting = false;
        }

        private static void Generate(ProjectItem header, ProjectItem source, VCFileCodeModel vcfile)
        {
            System.Collections.IEnumerator num = null;
            tryWhileFail.execute(() =>
            {
                num = vcfile.CodeElements.GetEnumerator();
            });
            while (num.MoveNext())
            {
                VCCodeElement el = num.Current as VCCodeElement;
                UHSConverter.parseitem(el, source, CodeHolder.newHolder(header.FileCodeModel as VCFileCodeModel));
            }
        }

    }
}


