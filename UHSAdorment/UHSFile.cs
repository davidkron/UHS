using Cycles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cycles
{
    using Microsoft.VisualStudio.Text;
    using Microsoft.VisualStudio.VCProjectEngine;

    public class UHSFile
    {
        private bool hasfiles = false;

        ProjectHolder project;
        UHSGenerator generator;
        VCFile header;
        VCFile source;
        VCFile uhs;

        public UHSFile(String filePath, string projectname, EnvDTE.DTE dte)
        {
            project = new ProjectHolder((EnvDTE80.DTE2)dte, projectname);
            generator = new UHSGenerator(project);
            load(filePath);
        }

        public UHSFile(ITextDocument doc, EnvDTE.DTE dte)
        {
            project = new ProjectHolder(dte);
            generator = new UHSGenerator(project);
            load(doc.FilePath);
        }

        public void parse()
        {
            if (!generator.converting)
            {
                generator.convert(uhs);
            }
        }

        public void load(string filename)
        {
            string dir = System.IO.Path.GetDirectoryName(filename);
            string rawfname = System.IO.Path.GetFileNameWithoutExtension(filename);

            uhs = project.findFile(filename);

            if (!hasfiles)
            {
                string filter = (uhs.Parent as VCProjectItem).ItemName;
                if (filter == "Source Files" || filter == "Header Files")
                    uhs.Move(project.unifiles);

                //AddObjectsFilter(ref currentproject);
                string headerpath = dir + "\\" + rawfname + ".h";
                string sourcepath = dir + "\\" + rawfname + ".cpp";


                if (!System.IO.File.Exists(headerpath))
                {
                    if (project.headers.CanAddFile(headerpath))
                    {
                        System.IO.File.CreateText(headerpath);
                        header = project.headers.AddFile(headerpath);
                    }
                }
                if (!System.IO.File.Exists(sourcepath))
                {
                    if (project.sources.CanAddFile(sourcepath))
                    {
                        System.IO.File.CreateText(sourcepath);
                        source = project.sources.AddFile(sourcepath);
                    }
                }

                hasfiles = true;
            }
        }
    }
}
