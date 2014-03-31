using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using EnvDTE;
using Microsoft.VisualStudio.VCCodeModel;
using Microsoft.VisualStudio.VCProjectEngine;
using Cycles;

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
            //intellisence = new intellicon(this.project.GetSDF());
        }

        public UHSGenerator(ProjectHolder project)
        {
            this.project = project;
        }

        /*public void print(EnvDTE.CodeElements elems, EnvDTE.ProjectItem targetFile)
        {
            foreach (EnvDTE.CodeElement el in elems)
            {
                //System.Diagnostics.Debug.WriteLine(el.ProjectItem.Name);
                if (targetFile == null || el.ProjectItem.Name == targetFile.Name)
                {
                    if (el.Kind == EnvDTE.vsCMElement.vsCMElementFunction)
                    {
                        //(el as EnvDTE.CodeFunction).AddAttribute("extern", "extern");
                    }

                    System.Diagnostics.Debug.WriteLine(el.Name + " " + el.Kind);
                    print(el.Children, null);
                }
                else
                    print(el.Children, targetFile);
            }
        }*/

        //Parses uhs items and generates both headers and source items
        public void parseitem(VCCodeElement elem, ProjectItem sourcetarget, CodeHolder headertarget)
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
                    if (headertarget.kind == CodeHolder.holdkind.vcfile)
                    {
                        moveImplementation(newelem as VCCodeFunction, sourcetarget);
                    }
                    
                    break;

                case vsCMElement.vsCMElementClass:
                    VCCodeClass cs = elem as VCCodeClass;

                    /*Array ar = null;
                    tryWhileFail.execute(() =>
                    {
                        ar = Array.CreateInstance(typeof(String), cs.Bases.Count);
                    });
                    int i = 0;
                    foreach(VCCodeBase b in cs.Bases)
                    {
                        ar.SetValue("Public " + b.DisplayName, i);
                        i++;
                    }*/
                    newelem = headertarget.AddClass(cs.Name, vsCMAccess.vsCMAccessPrivate, null, cs.ImplementedInterfaces.Count > 0 ? cs.ImplementedInterfaces : null) as VCCodeElement;
                    
                    break;

                case vsCMElement.vsCMElementNamespace:
                    VCCodeNamespace ns = elem as VCCodeNamespace;

                    newelem = headertarget.AddNamespace(ns.Name) as VCCodeElement;
                    break;

                case vsCMElement.vsCMElementVariable:
                    VCCodeVariable v = elem as VCCodeVariable;
                    VCCodeVariable v2 = headertarget.AddVariable(v.Name, v.Type, v.Access, sourcetarget.Name, v.IsShared, v.IsConstant);

                    break;

                case vsCMElement.vsCMElementVCBase:
                    System.Diagnostics.Debug.Write(elem.Kind);
                    VCCodeBase _base = elem as VCCodeBase;
                    //_base.Access = vsCMAccess.vsCMAccessPrivate;
                    //_base.Class.Access = vsCMAccess.vsCMAccessPrivate;
                    System.Diagnostics.Debug.Write(_base.Access);                    
                    headertarget.getClass().AddBase(_base.DisplayName,-1);
                    break;
                case vsCMElement.vsCMElementIncludeStmt:
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
            while(num.MoveNext())
            {
                VCCodeElement el = num.Current as VCCodeElement;
                if (newelem.Kind != vsCMElement.vsCMElementFunction &&
                    newelem.Kind != vsCMElement.vsCMElementEnum)
                    parseitem(el, sourcetarget, new CodeHolder(newelem));
                System.Diagnostics.Debug.WriteLine(el.Name + " " + el.Kind);
            }
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
            //Move uhs to header
            //System.IO.File.WriteAllText(header.FullPath, System.IO.File.ReadAllText(file.FullPath));

            //project.dteproj.Save();
            //Parse header
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
                parseitem(el, source, new CodeHolder(header.FileCodeModel as VCFileCodeModel));
            }
            
            project.dteproj.Save();

            converting = false;
            
            //Reopen docs
            if (hOpen) header.Open();
            if (sOpen) source.Open();
            //Move implementations to cpp
            //add declaring keywords (extend etc)
        }

        public void moveImplementation(VCCodeFunction oldfunc, ProjectItem source, VCCodeElement parent = null)
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


