using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using EnvDTE;
using Microsoft.VisualStudio.VCCodeModel;
using Microsoft.VisualStudio.VCProjectEngine;
using Cycles;
using Cycles.converting;

namespace Cycles
{
    public class UHSGenerator
    {
        //intellicon intellisence;
        public ProjectHolder project;
        public bool converting = false;
        public UHSGenerator(EnvDTE.Project project)
        {
            this.project = new ProjectHolder((EnvDTE80.DTE2)project);
        }

        public UHSGenerator(ProjectHolder project)
        {
            this.project = project;
        }

        void AddClass(VCCodeClass cs, CodeHolder target, VCFileCodeModel source)
        {
            var parent = target.AddClass(cs.Name, cs.Access, cs.Bases.Count > 0 ? cs.Bases : null, cs.ImplementedInterfaces.Count > 0 ? cs.ImplementedInterfaces : null);
        }

        void AddNamespace(VCCodeNamespace ns, CodeHolder target, VCFileCodeModel source)
        {
            VCCodeNamespace parent = target.AddNamespace(ns.Name);

            foreach (VCCodeClass cs in parent.Classes)
            {
                AddClass(cs, new CodeHolder(parent), source);
            }

            foreach (VCCodeFunction f in parent.Functions)
            {
                parent.AddFunction(f.Name, f.FunctionKind, f.Type, -1, f.Access);
                //parent.AddFunction(f.Name, f.FunctionKind, f.Type, -1, f.Access, source.Parent.Name);
            }
        }

        public void convert(VCFile file)
        {
            converting = true;

            //Create header

            VCFile h = project.findHeader(file.FullPath.Split('.')[0] + ".hpp");
            if (h == null)
                h = project.findHeader(file.FullPath.Split('.')[0] + ".h");
            VCFile s = project.findSource(file.FullPath.Split('.')[0] + ".cpp");

            ProjectItem header = h.Object as ProjectItem;
            ProjectItem source = s.Object as ProjectItem;
            
            bool hOpen = header.Document != null;
            bool sOpen = source.Document != null;
            if(hOpen)    
                header.Document.Close();
            if (sOpen)
                source.Document.Close();
            
            System.IO.File.WriteAllText(h.FullPath, String.Empty);
            System.IO.File.WriteAllText(s.FullPath, String.Empty);
            
            tryWhileFail.execute(()=>{
            project.proj.Save();
            project.dteproj.Save();
            });
            
            EnvDTE.ProjectItem uhsfile = file.Object as ProjectItem;
            VCFileCodeModel vcfile = null, vcheader, vcsource;
            tryWhileFail.execute(() =>
            {
                vcfile = uhsfile.FileCodeModel as VCFileCodeModel;
                vcheader = header.FileCodeModel as VCFileCodeModel;
                vcsource = source.FileCodeModel as VCFileCodeModel;
            });

            System.Collections.IEnumerator num = null;
            tryWhileFail.execute(() =>
            {
                num = vcfile.CodeElements.GetEnumerator();
            });
            while (num.MoveNext())
            {
                VCCodeElement el = num.Current as VCCodeElement;
                uhsconverter.parseitem(el, source, new CodeHolder(header.FileCodeModel as VCFileCodeModel));
            }
            
            project.dteproj.Save();

            converting = false;
            
            //Reopen docs
            if (hOpen) header.Open();
            if (sOpen) source.Open();
            //Move implementations to cpp
            //add declaring keywords (extend etc)
        }

        
    }
}


