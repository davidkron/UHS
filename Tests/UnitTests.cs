using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using EnvDTE;
using Cycles.Converting;
using Microsoft.QualityTools.Testing.Fakes;
using Microsoft.VisualStudio.VCCodeModel;

namespace Tests
{
	[TestClass]
	public class UnitTests
	{
		[TestMethod]
		public void TestMoveFunctionBody()
		{
			string headerContent = null, sourceContent = null;
			String uhsContent = @"
int main()
{
	return 5;
}";
			TestingFramework.testString(uhsContent,out headerContent, out sourceContent);

			StringAssert.Contains(sourceContent, "return 5");
			StringAssert.Contains(headerContent, "main();");
        }
	}
}
