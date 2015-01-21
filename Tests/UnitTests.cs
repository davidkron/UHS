using Cycles;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace Tests
{
    [TestClass]
    public class UnitTests
    {
        public void test(String fname)
        {
            EnvDTE.DTE dte2 = (EnvDTE.DTE)System.Runtime.InteropServices.Marshal.
                
            GetActiveObject("VisualStudio.DTE.14.0");
            UHSFile uhs = new UHSFile("C:\\Users\\David\\Desktop\\UHSAdorment\\TestingProj\\" + fname,
                    "TestingProj", dte2);
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
        public void MainTest()
        {
            test("main.uhs");
        }

        [TestMethod]
        public void Classtest()
        {
            test("classes.uhs");
        }

        [TestMethod]
        public void Structs()
        {
            test("structs.uhs");
        }

        [TestMethod]
        public void Keywords()
        {
            test("keywords.uhs");
        }

        [TestMethod]
        public void Templates()
        {
            test("templates.uhs");
        }

        [TestMethod]
        public void BraceMacro()
        {
            test("bracemacro.uhs");
        }

        [TestMethod]
        public void TestInclude()
        {
            test("include.uhs");
        }
    }
}