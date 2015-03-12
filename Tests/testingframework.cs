using System;
using Cycles;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Runtime.ExceptionServices;

namespace Tests
{
    class TestingFramework
    {
        static String folder = "C:\\Users\\David\\Desktop\\UHSAdorment\\TestingProj\\"
        static EnvDTE.DTE dte2 = (EnvDTE.DTE)System.Runtime.InteropServices.Marshal.
        GetActiveObject("VisualStudio.DTE.14.0");
        
        class UhsTest{
            String testPath;
            String fname;
            String header;
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
            
            public void UhsTest(String testname)
            {
                testPath = folder + testname;
                fname = testPath + ".uhs";
                header = testPath + ".hpp";
                source = testPath + ".cpp";
                compareHeader = folder + "Compare Files\\" + testname + ".hpp";
                compareSource = folder + "Compare Files\\" + testname + ".cpp";
                previousHeaderContents = System.IO.File.ReadAllText(header);
                new UHSFile(fname,"TestingProj", dte2);
                headerExistedBefore = System.IO.File.Exists(header);
                compareHeaderExists = System.IO.File.Exists(compareHeader);
                comparesourceExists = System.IO.File.Exists(compareSource);
                compareHeaderContents = System.IO.File.ReadAllText(compareHeader);
                compareSourceContents = System.IO.File.ReadAllText(compareSource);
            }
            
            public void convert()
            {
                uhsFile.parse();
                newHeaderContents = System.IO.File.ReadAllText(header);
                newSourceContents = System.IO.File.ReadAllText(source);
            }
        }
    
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
                test.parse();

                if (test.compareHeaderExists)
                {
                    Assert.AreEqual(test.compareHeaderContents, test.headerContents);
                }

                if (test.compareSourceExists)
                {
                    Assert.AreEqual(test.compareSourceContents, test.sourceContents);
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
