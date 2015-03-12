using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using EnvDTE;
using Microsoft.VisualStudio.VCCodeModel;
using Microsoft.VisualStudio.VCProjectEngine;
using Cycles;
using Cycles.Utils;
using Cycles.Converting;
using Cycles.Converting.CodeHolders;

namespace Cycles
{
    public class UHSGenerator
    {
        public ProjectHolder project;
        public bool converting = false;

        public UHSGenerator(ProjectHolder project)
        {
            this.project = project;
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
                project.vcProj.Save();
                project.dteproj.Save();
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
                System.Windows.MessageBox.Show("The UHS format needs to be interpreted as C++ code by visual studio. "
                    + "Currently you have to set this manually by going TOOLS->Options->Text Editor->File Extensions and adding the extension uhs using editor C++. "
                    + "The UHS format wont even work partly otherwise.\n\n If you still get this error it means visual studio is stuck parsing your solution, delete *.suo and *.sdf and restart");
                throw new Exceptions.UHSNotCppException();
            }

            bool recreated = false;

            try
            {
                Generate(header, source, vcfile);
            }
            catch (Exception e)
            {
                tryWhileFail.execute(() =>
                {
                    recreated = true;
                    System.IO.File.WriteAllText(h.FullPath, String.Empty);
                    Generate(header, source, vcfile);
                });
            }

            if(!System.IO.File.ReadAllText(h.FullPath).StartsWith("#pragma once"))
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

            project.dteproj.Save();
            converting = false;
        }

        private static void Generate(ProjectItem header, ProjectItem source, VCFileCodeModel vcfile)
        {
            System.Diagnostics.Debug.WriteLine("Retrying....");


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


