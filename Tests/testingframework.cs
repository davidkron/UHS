using System;
using Cycles;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Runtime.ExceptionServices;
using System.IO;

namespace Tests
{
    public class UhsTest
    {
        static String folder;

        String testPath;
        public String uhsfile;
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

		public String getFolder()
		{
			DirectoryInfo target = new DirectoryInfo(Directory.GetCurrentDirectory());
			return target.Parent.Parent.Parent.FullName + "\\TestingProj\\";
		}

        public UhsTest(String testname)
        {
			folder = getFolder();
            testPath = folder + testname;
			uhsfile = testPath + ".uhs";
            header = testPath + ".hpp";
            source = testPath + ".cpp";
            compareHeader = folder + "Compare Files\\" + testname + ".hpp";
            compareSource = folder + "Compare Files\\" + testname + ".cpp";
			EnvDTE.DTE dte2 = (EnvDTE.DTE)System.Runtime.InteropServices.Marshal.
            GetActiveObject("VisualStudio.DTE.14.0");
			Assert.IsNotNull(dte2);
            uhsFile = new UHSFile(uhsfile, dte2, "TestingProj");
            headerExistedBefore = System.IO.File.Exists(header);
            compareHeaderExists = System.IO.File.Exists(compareHeader);
            compareSourceExists = System.IO.File.Exists(compareSource);


			if(headerExistedBefore)
				previousHeaderContents = System.IO.File.ReadAllText(header);
			if (compareHeaderExists)
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

		public static void testString(string testString, out string headerContent, out string sourceContent)
		{
			UhsTest test = new UhsTest("testingfile");
			System.IO.File.WriteAllText(test.uhsfile, testString);
			test.convert();
			headerContent = test.newHeaderContents;
			sourceContent = test.newSourceContents;
        }

        public static void test(string testname)
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
