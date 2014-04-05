using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Cycles;

namespace Tests
{
    [TestClass]
    public class UnitTest1
    {
        public void test(String fname)
	{
		EnvDTE.DTE dte2 = (EnvDTE.DTE)System.Runtime.InteropServices.Marshal.
		GetActiveObject("VisualStudio.DTE.12.0");
        UHSFile uhs = new UHSFile("C:\\Users\\David\\Dropbox\\programmering\\uhs\\UHSAdorment\\TestingProj\\" + fname,
                "TestingProj",dte2);
		uhs.parse();
	}
	
	[TestMethod]
        public void TestMethod1()
        {
 	        test("test.uhs");
        }

        [TestMethod]
        public void TestEnum()
        {
		    test("enum.uhs");
        }

        [TestMethod]
        public void TestMess()
        {
            test("mess.uhs");
        }

        [TestMethod]
        public void NamespaceTest()
        {
            test("namespace.uhs");
        }

        [TestMethod]
        public void MacroTestHelloWorld()
        {
            test("main.uhs");
        }
        [TestMethod]
        public void Classtest()
        {
            test("classes.uhs");
        }
    }
}
