using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

    using EnvDTE;
    using Microsoft.VisualStudio.VCCodeModel;
using EnvDTE80;
using Cycles.converting;


    namespace Cycles.converting
    {
        public class CodeHolder
        {
            object data;

            public enum holdkind
            {
                vcclass, vcfile, vcnamespace,
                vcfunction
            }

            public holdkind kind;

            public CodeHolder(VCCodeNamespace ns)
            {
                kind = holdkind.vcnamespace;
                data = ns;
            }

            public CodeHolder(VCFileCodeModel fm)
            {
                kind = holdkind.vcfile;
                data = fm;
            }

            public CodeHolder(VCCodeClass cs)
            {
                kind = holdkind.vcclass;
                data = cs;
            }

            public CodeHolder(VCCodeElement newelem)
            {
                data = newelem;
                switch (newelem.Kind)
                {
                    case vsCMElement.vsCMElementClass:
                        kind = holdkind.vcclass;
                        break;
                    case vsCMElement.vsCMElementNamespace:
                        kind = holdkind.vcnamespace;
                        break;
                    case vsCMElement.vsCMElementFunction:
                        kind = holdkind.vcfunction;
                        break;
                    default:
                        throw new System.Exception("invalid type of parent: " + newelem.Kind);
                }
            }

            public VCCodeNamespace AddNamespace(string name)
            {
                switch (kind)
                {
                    case holdkind.vcclass:
                        throw new System.Exception("Class cant contain namespace");
                    case holdkind.vcfile:
                        return (data as VCFileCodeModel).AddNamespace(name, -1) as VCCodeNamespace;
                    case holdkind.vcnamespace:
                        return (data as VCCodeNamespace).AddNamespace(name, -1) as VCCodeNamespace;
                    default:
                        throw new System.Exception("unknown kind");
                }
            }


            public VCCodeEnum AddEnum(VCCodeEnum oldEnum)
            {
                VCCodeEnum enm;
                switch (kind)
                {
                    case holdkind.vcclass:
                        enm = (data as VCCodeClass).AddEnum(oldEnum.Name, -1, oldEnum.Bases, oldEnum.Access) as VCCodeEnum;
                        break;
                    case holdkind.vcfile:
                        enm = (data as VCFileCodeModel).AddEnum(oldEnum.Name, -1, oldEnum.Bases, oldEnum.Access) as VCCodeEnum;
                        break;
                    case holdkind.vcnamespace:
                        enm = (data as VCCodeNamespace).AddEnum(oldEnum.Name, -1, oldEnum.Bases, oldEnum.Access) as VCCodeEnum;
                        break;
                    default:
                        throw new System.Exception("unknown kind");
                }
                foreach (VCCodeAttribute att in oldEnum.Attributes)
                    enm.AddAttribute(att.Name, att.Value);
                foreach (VCCodeVariable memb  in oldEnum.Members)
                    enm.AddMember(memb.Name,memb.InitExpression);
                return oldEnum;
            }

            public VCCodeVariable AddVariable(string Name, object Type, vsCMAccess Access, string location, bool isstatic, bool constant)
            {
                if (!isstatic)
                    location = null;

                VCCodeVariable var;
                switch (kind)
                {
                    case holdkind.vcclass:
                        var = (data as VCCodeClass).AddVariable(Name, Type, -1, Access, location) as VCCodeVariable;
                        break;
                    case holdkind.vcfile:
                        var = (data as VCFileCodeModel).AddVariable(Name, Type, -1, Access) as VCCodeVariable;
                        break;
                    case holdkind.vcnamespace:
                        var = (data as VCCodeNamespace).AddVariable(Name, Type, -1, Access) as VCCodeVariable;
                        break;
                    default:
                        throw new System.Exception("unknown kind");
                }
                try
                {
                    //var.IsShared = isstatic;

                }
                catch (System.Exception e)
                {
                    //int erro = Marshal.GetLastWin32Error();
                    //System.Diagnostics.Debug.WriteLine(erro);
                }
                var.IsConstant = constant;
                return var;
            }

            public VCCodeFunction AddFunction(string Name, vsCMFunction functionkind, object Type, vsCMAccess Access, string location)
            {
                switch (kind)
                {
                    case holdkind.vcclass:
                        functionkind &= ~vsCMFunction.vsCMFunctionInline;
                        return (data as VCCodeClass).AddFunction(Name, functionkind, Type, -1, Access, location) as VCCodeFunction;
                    case holdkind.vcfile:
                        return (data as VCFileCodeModel).AddFunction(Name, functionkind, Type, -1, Access) as VCCodeFunction;
                    case holdkind.vcnamespace:
                        return (data as VCCodeNamespace).AddFunction(Name, functionkind, Type, -1, Access) as VCCodeFunction;
                    default:
                        throw new System.Exception("unknown kind");
                }
            }

            public VCCodeClass AddClass(string Name, vsCMAccess Access, object Bases = null, object ImplementedInterfaces = null)
            {
                switch (kind)
                {
                    case holdkind.vcclass:
                        return (data as VCCodeClass).AddClass(Name, -1, Bases, ImplementedInterfaces, Access) as VCCodeClass;
                    case holdkind.vcfile:
                        return (data as FileCodeModel).AddClass(Name, -1, Bases, ImplementedInterfaces, Access) as VCCodeClass;
                    case holdkind.vcnamespace:
                        return (data as VCCodeNamespace).AddClass(Name, -1, Bases, ImplementedInterfaces, Access) as VCCodeClass;
                    default:
                        throw new System.Exception("unknown kind");
                }
            }

            /*public CodeElements Classes
            {
                get
                {
                    switch (kind)
                    {
                        case holdkind.vcclass:
                            return (data as VCCodeClass).Classes;
                        case holdkind.vcfile:
                            return (data as VCFileCodeModel).Classes;
                        case holdkind.vcnamespace:
                            return (data as VCCodeNamespace).Classes;
                        default:
                            throw new System.Exception("unknown kind");
                    }
                }
            }

            public CodeElements Functions
            {
                get
                {
                    switch (kind)
                    {
                        case holdkind.vcclass:
                            return (data as VCCodeClass).Functions;
                        case holdkind.vcfile:
                            return (data as VCFileCodeModel).Functions;
                        case holdkind.vcnamespace:
                            return (data as VCCodeNamespace).Functions;
                        default:
                            throw new System.Exception("unknown kind");
                    }
                }
            }


            public object Name
            {
                get
                {
                    switch (kind)
                    {
                        case holdkind.vcclass:
                            return (data as VCCodeClass).Name;
                        case holdkind.vcfile:
                            return (data as VCFileCodeModel).Parent.Name;
                        case holdkind.vcnamespace:
                            return (data as VCCodeNamespace).Name;
                        default:
                            throw new System.Exception("unknown kind");
                    }
                }
            }*/

            internal VCCodeClass getClass()
            {
                System.Diagnostics.Debug.Assert(kind == holdkind.vcclass);
                return data as VCCodeClass;
            }

            internal VCCodeElement addInclude(VCCodeInclude vCCodeInclude)
            {
                System.Diagnostics.Debug.Assert(kind == holdkind.vcfile);
                return (VCCodeElement)(data as VCFileCodeModel).AddInclude(vCCodeInclude.DisplayName);
            }

            internal VCCodeElement addMacro(VCCodeMacro macro)
            {
                System.Diagnostics.Debug.Assert(kind == holdkind.vcfile);
                string smacro = macro.StartPoint.CreateEditPoint().GetText(macro.EndPoint); 
                String value = smacro.Substring(macro.Name.Count() + "#define  ".Count()) + "\n";
                return (VCCodeElement)(data as VCFileCodeModel).AddMacro(macro.Name, value, -1);
            }
        }
    }
