using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cycles
{
    using System;
    using EnvDTE;
    using Microsoft.VisualStudio.VCProjectEngine;
    using Cycles.Utils;
    
    public class ProjectHolder
    {
        public EnvDTE.Project dteproj;
        public VCProject proj;
        public VCFilter headers;
        public VCFilter sources;
        public VCFilter unifiles;

	    public ProjectHolder(EnvDTE80.DTE2 enviro)
	    {
            EnvDTE.Project proj = null;
            tryWhileFail.execute(() =>
            {
                proj = (EnvDTE.Project)enviro.ActiveSolutionProjects[0];
            });
            load(proj);
	    }

        internal void load(EnvDTE.Project dteproj)
        {
            this.dteproj = dteproj;
            proj = (VCProject)dteproj.Object;
            headers = findFilter("Header Files");
            sources = findFilter("Source Files");
            unifiles = findFilter("Unified Files");
            if (unifiles == null)
                unifiles = proj.AddFilter("Unified Files");
        }
        public ProjectHolder(EnvDTE80.DTE2 enviro,String projectname)
        {
            while (dteproj == null)
            {
                tryWhileFail.execute(() =>
                {
                    foreach (EnvDTE.Project project in enviro.Solution.Projects)
                    {
                        if (project.Name == projectname)
                        {
                            dteproj = project;
                            break;
                        }
                    }
                });
            }

            load(dteproj);
        }
        public VCFilter findFilter(String fname, VCFilter filter = null)
        {
            dynamic filters;

            if (filter == null)
            {
                filters = proj.Filters;
            }
            else
            {
                filters = filter.Filters;
            }

            foreach (VCFilter f in filters)
            {
                if (f.Name == fname)
                    return f;
                VCFilter found = findFilter(fname, f);
                if (found != null)
                    return found;
            }

            return null;
        }
        public VCFile findFile(String fname,VCFilter filter = null)
        {
            dynamic files;
            dynamic filters;

            if (filter == null)
            {
                files = proj.Files;
                filters = proj.Filters;
            }
            else
            {
                files = filter.Files;
                filters = filter.Filters;
            }

            foreach(VCFile f in files)
            {
                if (fname == f.FullPath)
                    return f;
            }

            foreach (VCFilter f in filters)
            {
                VCFile found = findFile(fname, f);
                if (found != null)
                    return found;
            }

            return null;
        }

        internal VCFile findHeader(string fname)
        {
            return findFile(fname, headers);
        }

        internal VCFile findSource(string fname)
        {
            return findFile(fname, sources);
        }
    }
}
