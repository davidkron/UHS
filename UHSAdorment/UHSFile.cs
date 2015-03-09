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
                generator.convert(uhs,header,source);
            }
        }

        public void load(string filename)
        {
            string dir = System.IO.Path.GetDirectoryName(filename);
            string rawfname = System.IO.Path.GetFileNameWithoutExtension(filename);

            uhs = project.findFile(filename);

            string filter = (uhs.Parent as VCProjectItem).ItemName;
            if (filter == "Source Files" || filter == "Header Files")
                uhs.Move(project.unifiles);

            string headerpath = dir + "\\" + rawfname + ".hpp";
            string alternativeHeaderPath = dir + "\\" + rawfname + ".h";
            string sourcepath = dir + "\\" + rawfname + ".cpp";

            /*
                    FIND IF ALLREADY EXISTS
            */
            header = project.findHeader(headerpath);
            if (header == null) 
                header = project.findHeader(alternativeHeaderPath);
            source = project.findSource(sourcepath);

            /*
                    NOT FOUND - CREATE
            */
            if (header == null)
            {
                if (!project.headers.CanAddFile(headerpath))
                    throw new Exception("Could not add file");

                System.IO.File.WriteAllText(headerpath, "#pragma once\r\n");
                header = project.headers.AddFile(headerpath);
            }
            if (source == null)
            {
                if (!project.sources.CanAddFile(sourcepath))
                    throw new Exception("Could not add file");

                    System.IO.File.CreateText(sourcepath);
                    source = project.sources.AddFile(sourcepath);
            }
        }
    }
}
