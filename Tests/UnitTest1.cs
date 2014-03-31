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
		UHSFile uhs = new UHSFile("C:\\Users\\David\\Dropbox\\LaptopBackup\\Projects\\AdornmentTest\\TestingProj\\" + fname,
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
    }
}
