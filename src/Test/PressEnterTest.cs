using System.Collections.Generic;
using System.IO;
using Aspenlaub.Net.GitHub.CSharp.Paleface.Entities;
using Aspenlaub.Net.GitHub.CSharp.Paleface.Helpers;
using Aspenlaub.Net.GitHub.CSharp.Paleface.Interfaces;
using Aspenlaub.Net.GitHub.CSharp.Pegh.Entities;
using Autofac;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;

namespace Aspenlaub.Net.GitHub.CSharp.PressEnter.Test {
    [TestClass]
    public class PressEnterTest {
        private readonly IContainer vContainer;

        public PressEnterTest() {
            vContainer = new ContainerBuilder().UsePressEnterAndPaleface().Build();
        }

        [TestInitialize]
        public void Initialize() {
            TestProcessHelper.ShutDownRunningProcesses(TestProcessHelper.ProcessType.WordPad);
            TestProcessHelper.LaunchProcessAsync(TestProcessHelper.ProcessType.WordPad).Wait();
            TestProcessHelper.ShutDownRunningProcesses(TestProcessHelper.ProcessType.Paleface);
            TestProcessHelper.LaunchProcessAsync(TestProcessHelper.ProcessType.Paleface).Wait();
        }

        [TestCleanup]
        public void Cleanup() {
            TestProcessHelper.ShutDownRunningProcesses(TestProcessHelper.ProcessType.WordPad);
            TestProcessHelper.ShutDownRunningProcesses(TestProcessHelper.ProcessType.Paleface);
        }

        [TestMethod]
        public void CanPressEnter() {
            var windowsElementSearchSpec = WindowsElementSearchSpec.Create("", "Document - WordPad");
            var windowsChildElementSearchSpec = WindowsElementSearchSpec.Create("", "Rich Text Window");
            windowsElementSearchSpec.WindowsChildElementSearchSpecs.Add(windowsChildElementSearchSpec);
            var searcher = vContainer.Resolve<IWindowsElementSearcher>();
            var element = searcher.SearchWindowsElement(windowsElementSearchSpec);
            Assert.IsNotNull(element, "Wordpad not found");
            var log = new List<string>();
            element = searcher.SearchWindowsElement(element, windowsChildElementSearchSpec, log);
            Assert.IsNotNull(element, "Wordpad document not found");
            element.Click();
            element.SendKeys(Keys.Control + 'o' + Keys.Control);
            var sut = vContainer.Resolve<IPressEnterAgent>();
            var fileName = new Folder(Path.GetTempPath()).FullName + @"\PressEnter.txt";
            const string testText = "It worked!";
            File.WriteAllText(fileName, testText);
            Assert.IsTrue(sut.EnterFileNameAndPressEnter(fileName, "", log));
            Assert.IsTrue(element.Text.Contains(testText));
        }
    }
}
