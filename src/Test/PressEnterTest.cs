using System.IO;
using System.Linq;
using Aspenlaub.Net.GitHub.CSharp.Paleface;
using Aspenlaub.Net.GitHub.CSharp.Pegh.Entities;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using OpenQA.Selenium.Appium;

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
            var windowsElementSearchSpec = WindowsElementSearchSpec.Create("window", "Document - WordPad");
            var windowsChildElementSearchSpec = WindowsElementSearchSpec.Create("document", "Rich Text Window");
            windowsElementSearchSpec.WindowsChildElementSearchSpecs.Add(windowsChildElementSearchSpec);
            AppiumWebElement element = WindowsElementSearcher.SearchWindowsElement(windowsElementSearchSpec);
            Assert.IsNotNull(element, "Wordpad not found");
            element = element.FindElementsByWindowsElementSearchSpec(windowsChildElementSearchSpec).FirstOrDefault();
            Assert.IsNotNull(element, "Wordpad document not found");
            element.Click();
            element.SendKeys(Keys.Control + 'o' + Keys.Control);
            var sut = new PressEnterAgent();
            var fileName = new Folder(Path.GetTempPath()).FullName + @"\PressEnter.txt";
            const string testText = "It worked!";
            File.WriteAllText(fileName, testText);
            Assert.IsTrue(sut.EnterFileNameAndPressEnter(fileName));
            Assert.IsTrue(element.Text.Contains(testText));
        }
    }
}
