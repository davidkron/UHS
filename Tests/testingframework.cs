using System;
using Cycles;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Runtime.ExceptionServices;

namespace Tests
{
    class TestingFramework
    {
        public static void test(String testname)
        {
            String folder = "C:\\Users\\David\\Desktop\\UHSAdorment\\TestingProj\\";
            String testPath = folder + testname;
            String fname = testPath + ".uhs";
            String header = testPath + ".hpp";
            String source = testPath + ".cpp";
            String compareHeader = folder + "Compare Files\\" + testname + ".hpp";
            String compareSource = folder + "Compare Files\\" + testname + ".cpp";

            EnvDTE.DTE dte2 = (EnvDTE.DTE)System.Runtime.InteropServices.Marshal.
            GetActiveObject("VisualStudio.DTE.14.0");

            try
            {

                /*
                        The header file should contain #pragma once before, if it exists
                */
                if (System.IO.File.Exists(header))
                {
                    String previousHeaderContents = System.IO.File.ReadAllText(header);
                    StringAssert.Contains(previousHeaderContents, "#pragma once");
                }

                /*
                        Parse
                */
                UHSFile uhs = new UHSFile(fname,"TestingProj", dte2);
                uhs.parse();


                /*
                        The header file should contain #pragma once afer
                */
                String newHeader = System.IO.File.ReadAllText(header);
                StringAssert.Contains(newHeader, "#pragma once");

                if (System.IO.File.Exists(compareHeader))
                {
                    String oldHeader = System.IO.File.ReadAllText(compareHeader);
                    Assert.AreEqual(oldHeader, newHeader);
                }

                if (System.IO.File.Exists(compareSource))
                {
                    String newSource = System.IO.File.ReadAllText(source);
                    String oldSource = System.IO.File.ReadAllText(compareSource);
                    Assert.AreEqual(oldSource, newSource);
                }
            }
            finally
            {
                /*
                    Ensure header contains pragma once even after failed conversion
                */
                System.IO.File.WriteAllText(header, "#pragma once\r\n");
            }
        }


        public static void unsafe_test(String testname)
        {

        }
    }
}
