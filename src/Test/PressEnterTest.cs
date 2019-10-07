using System.Collections.Generic;
using System.IO;
using Aspenlaub.Net.GitHub.CSharp.Paleface;
using Aspenlaub.Net.GitHub.CSharp.Pegh.Entities;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;

namespace Aspenlaub.Net.GitHub.CSharp.PressEnter.Test {
    [TestClass]
    public class PressEnterTest {
        protected WindowsElementSearcher WindowsElementSearcher = new WindowsElementSearcher();

        [TestInitialize]
        public void Initialize() {
            TestProcessHelper.ShutDownRunningProcesses(TestProcessHelper.ProcessType.WordPad);
            TestProcessHelper.LaunchProcess(TestProcessHelper.ProcessType.WordPad);
        }

        [TestCleanup]
        public void Cleanup() {
            TestProcessHelper.ShutDownRunningProcesses(TestProcessHelper.ProcessType.WordPad);
        }

        [TestMethod]
        public void CanPressEnter() {
            var windowsElementSearchSpec = WindowsElementSearchSpec.Create("", "Document - WordPad");
            var windowsChildElementSearchSpec = WindowsElementSearchSpec.Create("", "Rich Text Window");
            windowsElementSearchSpec.WindowsChildElementSearchSpecs.Add(windowsChildElementSearchSpec);
            var element = WindowsElementSearcher.SearchWindowsElement(windowsElementSearchSpec);
            Assert.IsNotNull(element, "Wordpad not found");
            var log = new List<string>();
            element = WindowsElementSearcher.SearchWindowsElement(element, windowsChildElementSearchSpec, log);
            Assert.IsNotNull(element, "Wordpad document not found");
            element.Click();
            element.SendKeys(Keys.Control + 'o' + Keys.Control);
            var sut = new PressEnterAgent();
            var fileName = new Folder(Path.GetTempPath()).FullName + @"\PressEnter.txt";
            const string testText = "It worked!";
            File.WriteAllText(fileName, testText);
            Assert.IsTrue(sut.EnterFileNameAndPressEnter(fileName, log));
            Assert.IsTrue(element.Text.Contains(testText));
        }
    }
}
