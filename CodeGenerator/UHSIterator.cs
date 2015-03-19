using CodeGenerator.Converting.CloneUtils;
using CodeGenerator.Converting.CodeHolders;
using CodeGenerator.Utils;
using EnvDTE;
using Microsoft.VisualStudio.VCCodeModel;

namespace Cycles.Converting
{
    public class UHSConverter
    {
		private VCFileCodeModel header;
		private ProjectItem source;
		private VCFileCodeModel vcfile;

		public UHSConverter(VCFileCodeModel header, ProjectItem source, VCFileCodeModel vcfile)
		{
			this.header = header;
			this.source = source;
			this.vcfile = vcfile;
		}

		//Parses uhs items and generates both headers and source items
		private static VCCodeElement cloneElement(VCCodeElement elem, ProjectItem sourcetarget, CodeHolder headertarget)
        {
            VCCodeElement newelem = null;

            switch (elem.Kind)
            {
                case vsCMElement.vsCMElementParameter:
					headertarget.add(elem as VCCodeParameter);
					return null;

                case vsCMElement.vsCMElementFunction:
                    VCCodeFunction func = elem as VCCodeFunction;
                    newelem = headertarget.add(func);

                    break;

                case vsCMElement.vsCMElementClass:
                    VCCodeClass cs = elem as VCCodeClass;
                    newelem = headertarget.add(cs);
                    break;

                case vsCMElement.vsCMElementNamespace:
                    newelem = headertarget.add(elem as VCCodeNamespace);
                    break;

                case vsCMElement.vsCMElementVariable:
                    VCCodeVariable v = elem as VCCodeVariable;
                    VCCodeVariable headerVar = headertarget.add(v) as VCCodeVariable;
                    if (headertarget is FileHolder)
                    {
                        VCCodeVariable sourceVar = (sourcetarget.FileCodeModel as VCFileCodeModel).AddVariable(v.Name, v.Type, -1, v.Access) as VCCodeVariable;
                        ImplementationMover.addExtern((VCCodeElement)headerVar);
                        sourceVar.InitExpression = v.InitExpression;
                    }

                    return null;

                case vsCMElement.vsCMElementAttribute:
                    newelem = (headertarget).add(elem as VCCodeAttribute);
                    break;

                case vsCMElement.vsCMElementVCBase:
                    newelem = (headertarget).add(elem as VCCodeBase);
                    break;

                case vsCMElement.vsCMElementIncludeStmt:
                    newelem = headertarget.add(elem as VCCodeInclude);
                    break;

                case vsCMElement.vsCMElementMacro:
                    newelem = headertarget.add(elem as VCCodeMacro);
                    break;

                case vsCMElement.vsCMElementEnum:
                    newelem = headertarget.add(elem as VCCodeEnum);
                    break;

                case vsCMElement.vsCMElementStruct:
                    VCCodeStruct cstruct = elem as VCCodeStruct;
                    newelem = headertarget.add(elem as VCCodeStruct);
                    break;

                default:
                    throw new System.NotImplementedException("U need to handle dis one David" + elem.Kind.ToString());
            }

			if (newelem == null)
            {
                throw new System.NotImplementedException("Failed adding" + elem.Kind.ToString() + " to a " + headertarget.holdingType);
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("Added: " + elem.Kind.ToString() + " to a " + headertarget.holdingType);
            }


			return newelem;
        }



		public void CloneElementsFromFile()
		{
			var holder = CodeHolder.newHolder(header);
			CloneElements(vcfile.CodeElements, holder);
		}

		private void CloneElements(CodeElements from, CodeHolder target)
		{
			System.Diagnostics.Debug.WriteLine("Iterating children of: " + target.holdingType.ToString());
			System.Collections.IEnumerator num = null;
			tryWhileFail.execute(() =>
			{
				num = from.GetEnumerator();
			});
			while (num.MoveNext())
			{
				VCCodeElement current = num.Current as VCCodeElement;

				VCCodeElement newElement = UHSConverter.cloneElement(current, source, target);

				// Iterate children
				if (newElement != null)
				{
					CloneElements(current.Children, CodeHolder.newHolder(newElement, source.Name));

					if (current.Kind == vsCMElement.vsCMElementFunction)
					{
						string body = "";
						tryWhileFail.execute(() => {
							body = (current as VCCodeFunction).BodyText;
						});
						ImplementationMover.moveImplementation(newElement as VCCodeFunction, target, source, body);
					}
				}
			}
		}
    }
}