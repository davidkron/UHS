using Cycles;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace Tests
{
    [TestClass]
    public class ContainsPragmaOnce
    {
        String[] testfiles = {"test","enum","mess","main","classes","structs",
        "keywords","temlpates","bracemacro","include","sample"};
        
        [TestMethod]
        public void testContainsPragmaOnce()
        {
            foreach(String testname in testfiles){
                UhsTest test = new UhsTest(testname);
                test.parse();
                if(!test.newHeaderContents.Contains("#pragma once"))
                {
                    System.IO.File.WriteAllText(test.header, "#pragma once\r\n");
                    throw new FormatException("Needs to contain pragma once before ran");
                }
            }
        }
    }
