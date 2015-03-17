using System;

namespace Cycles
{
    using Microsoft.VisualStudio.VCProjectEngine;

    public class UHSFile
    {
        UHSProject project;
        CodeSplitter generator;
        VCFile header;
        VCFile source;
        VCFile uhs;

        public UHSFile(String filePath, EnvDTE.DTE dte, string projectname = null)
        {
            project = new UHSProject(dte, projectname);
            generator = new CodeSplitter(project.dteproj,project.vcProj);
            load(filePath);
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
			if (uhs == null) throw new Exceptions.FileNotInProjectException(filename);
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
