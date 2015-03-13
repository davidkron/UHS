using System;
using Cycles;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Runtime.ExceptionServices;

namespace Tests
{
    public class UhsTest
    {
        static String folder = "C:\\Users\\David\\Desktop\\UHSAdorment\\TestingProj\\";

        String testPath;
        String fname;
        public String header;
        String source;
        String compareHeader;
        String compareSource;
        public String newHeaderContents;
        public String newSourceContents;
        public String previousHeaderContents;
        public String compareHeaderContents;
        public String compareSourceContents;
        public bool headerExistedBefore;
        public bool compareHeaderExists;
        public bool compareSourceExists;
        UHSFile uhsFile;

        public UhsTest(String testname)
        {
            testPath = folder + testname;
            fname = testPath + ".uhs";
            header = testPath + ".hpp";
            source = testPath + ".cpp";
            compareHeader = folder + "Compare Files\\" + testname + ".hpp";
            compareSource = folder + "Compare Files\\" + testname + ".cpp";
            previousHeaderContents = System.IO.File.ReadAllText(header);
            EnvDTE.DTE dte2 = (EnvDTE.DTE)System.Runtime.InteropServices.Marshal.
            GetActiveObject("VisualStudio.DTE.14.0");
            uhsFile = new UHSFile(fname, dte2, "TestingProj");
            headerExistedBefore = System.IO.File.Exists(header);
            compareHeaderExists = System.IO.File.Exists(compareHeader);
            compareSourceExists = System.IO.File.Exists(compareSource);
            if(compareHeaderExists)
                compareHeaderContents = System.IO.File.ReadAllText(compareHeader);
            if(compareSourceExists)
                compareSourceContents = System.IO.File.ReadAllText(compareSource);
        }

        public void convert()
        {
            uhsFile.parse();
            newHeaderContents = System.IO.File.ReadAllText(header);
            newSourceContents = System.IO.File.ReadAllText(source);
        }
    }



    class TestingFramework
    {    
        public static void test(String testname)
        {
            
            UhsTest test = new UhsTest(testname);
        
            try
            {
                /*
                        The header file should contain #pragma once before, if it exists
                */
                if (test.headerExistedBefore)
                {
                    if(!test.previousHeaderContents.Contains("#pragma once"))
                    {
                        throw new FormatException("Needs to contain pragma once before ran");
                    }
                }

                // PARSE
                test.convert();

                if (test.compareHeaderExists)
                {
                    Assert.AreEqual(test.compareHeaderContents, test.newHeaderContents);
                }

                if (test.compareSourceExists)
                {
                    Assert.AreEqual(test.compareSourceContents, test.newSourceContents);
                }
            }
            catch(FormatException e)
            {
                /*
                    Ensure header contains pragma once even after failed conversion
                */
                System.IO.File.WriteAllText(test.header, "#pragma once\r\n");
                ExceptionDispatchInfo.Capture(e).Throw();
            }
        }
    }
}
