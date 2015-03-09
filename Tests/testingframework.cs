using System;
using Cycles;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests
{
    class TestingFramework
    {
        public static void test(String testname)
        {
            String folder = "C:\\Users\\David\\Desktop\\UHSAdorment\\TestingProj\\";
            String fname = testname + ".uhs";
            String header = testname + ".h";
            String source = testname + ".cpp";
            String compareHeader = "Compare Files\\" + header;
            String compareSource = "Compare Files\\" + source;

            EnvDTE.DTE dte2 = (EnvDTE.DTE)System.Runtime.InteropServices.Marshal.

            GetActiveObject("VisualStudio.DTE.14.0");
            UHSFile uhs = new UHSFile(folder + fname,
                    "TestingProj", dte2);
            uhs.parse();


            String newHeader = System.IO.File.ReadAllText(folder + header);
            StringAssert.Contains(newHeader, "#pragma once");

            if (System.IO.File.Exists(folder + compareHeader))
            {
                String oldHeader = System.IO.File.ReadAllText(folder + compareHeader);
                Assert.AreEqual(oldHeader, newHeader);
            }


            if (System.IO.File.Exists(folder + compareSource))
            {
                String newSource = System.IO.File.ReadAllText(folder + source);
                String oldSource = System.IO.File.ReadAllText(folder + compareSource);
                Assert.AreEqual(oldSource, newSource);
            }


        }
    }
}
