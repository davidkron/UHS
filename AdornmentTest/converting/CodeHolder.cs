using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EnvDTE;
using Microsoft.VisualStudio.VCCodeModel;
using EnvDTE80;
using Cycles.Converting;


    namespace Cycles.Converting
    {
        public class CodeHolder3
        {
            object data;

            public enum Holdkind
            {
                VCClass, VCFile, VCNamespace,
                VCFunction,
                VCStruct
            }

            public Holdkind kind;

            public CodeHolder3(VCFileCodeModel VCFile)
            {
                kind = Holdkind.VCFile;
                data = VCFile;
            }

            public CodeHolder3(VCCodeElement NewElem)
            {
                data = NewElem;
                switch (NewElem.Kind)
                {
                    case vsCMElement.vsCMElementClass:
                        kind = Holdkind.VCClass;
                        break;
                    case vsCMElement.vsCMElementStruct:
                        kind = Holdkind.VCStruct;
                        break;
                    case vsCMElement.vsCMElementNamespace:
                        kind = Holdkind.VCNamespace;
                        break;
                    case vsCMElement.vsCMElementFunction:
                        kind = Holdkind.VCFunction;
                        break;
                    default:
                        throw new System.ArgumentException("invalid type of parent: " + NewElem.Kind);
                }
            }


            public VCCodeEnum AddEnum(VCCodeEnum oldEnum)
            {
                VCCodeEnum enm = null;
                switch (kind)
                {
                    case Holdkind.VCStruct:
                        enm = (data as VCCodeStruct).AddEnum(oldEnum.Name, -1, oldEnum.Bases, oldEnum.Access) as VCCodeEnum;
                        break;
                    case Holdkind.VCClass:
                        enm = (data as VCCodeClass).AddEnum(oldEnum.Name, -1, oldEnum.Bases, oldEnum.Access) as VCCodeEnum;
                        break;
                    case Holdkind.VCFile:
                        break;
                    case Holdkind.VCNamespace:
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

            public VCCodeVariable AddVariable(string Name, object Type, vsCMAccess Access, string Location, bool isstatic, bool constant)
            {
                if (!isstatic)
                    Location = null;

                VCCodeVariable var;
                switch (kind)
                {
                    case Holdkind.VCFile:
                        var = (data as VCFileCodeModel).AddVariable(Name, Type, -1, Access) as VCCodeVariable;
                        break;
                    case Holdkind.VCNamespace:
                        var = (data as VCCodeNamespace).AddVariable(Name, Type, -1, Access) as VCCodeVariable;
                        break;
                    default:
                        throw new System.NotSupportedException("unknown kind");
                }

                var.IsConstant = constant;
                return var;
            }

            public VCCodeFunction AddFunction(string Name, vsCMFunction functionkind, object Type, vsCMAccess Access, string location)
            {
                switch (kind)
                {
                    case Holdkind.VCClass:
                        functionkind &= ~vsCMFunction.vsCMFunctionInline;
                        return (data as VCCodeClass).AddFunction(Name, functionkind, Type, -1, Access, location) as VCCodeFunction;
                    case Holdkind.VCStruct:
                        functionkind &= ~vsCMFunction.vsCMFunctionInline;
                        return (data as VCCodeStruct).AddFunction(Name, functionkind, Type, -1, Access, location) as VCCodeFunction;
                    case Holdkind.VCFile:
                    case Holdkind.VCNamespace:
                        return (data as VCCodeNamespace).AddFunction(Name, functionkind, Type, -1, Access) as VCCodeFunction;
                    default:
                        throw new System.NotSupportedException("unknown kind");
                }
            }

            public VCCodeStruct AddStruct(string Name, vsCMAccess Access, object Bases = null, object ImplementedInterfaces = null)
            {
                switch (kind)
                {
                    case Holdkind.VCStruct:
                    case Holdkind.VCClass:
                    case Holdkind.VCFile:
                        return (data as FileCodeModel).AddStruct(Name, -1, Bases, ImplementedInterfaces, Access) as VCCodeStruct;
                    case Holdkind.VCNamespace:
                        return (data as VCCodeNamespace).AddStruct(Name, -1, Bases, ImplementedInterfaces, Access) as VCCodeStruct;
                    default:
                        throw new System.Exception("unknown kind");
                }
            }

            public VCCodeClass AddClass(VCCodeClass cs)
            {
                EnvDTE.vsCMAccess Access = vsCMAccess.vsCMAccessPrivate;

                object interfaces = cs.ImplementedInterfaces.Count > 0 ? cs.ImplementedInterfaces : null;

                switch (kind)
                {
                    case Holdkind.VCClass:
                        return (data as VCCodeClass).AddClass(cs.Name, -1, null, interfaces, Access) as VCCodeClass;
                    case Holdkind.VCFile:
                        return (data as VCFileCodeModel).AddClass(cs.Name, -1, null, interfaces, Access) as VCCodeClass;
                    case Holdkind.VCNamespace:
                        return (data as VCCodeNamespace).AddClass(cs.Name, -1, null, interfaces, Access) as VCCodeClass;
                    default:
                        throw new System.Exception("unknown kind");
                }
            }
        }
    }
