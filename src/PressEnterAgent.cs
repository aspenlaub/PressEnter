using System.Collections.Generic;
using Aspenlaub.Net.GitHub.CSharp.Paleface.Entities;
using Aspenlaub.Net.GitHub.CSharp.Paleface.Interfaces;
using OpenQA.Selenium;

namespace Aspenlaub.Net.GitHub.CSharp.PressEnter {
    public class PressEnterAgent : IPressEnterAgent {
        protected readonly ITextBoxFactory TextBoxFactory;
        protected readonly IWindowsElementSearcher WindowsElementSearcher;

        public PressEnterAgent(ITextBoxFactory textBoxFactory, IWindowsElementSearcher windowsElementSearcher) {
            TextBoxFactory = textBoxFactory;
            WindowsElementSearcher = windowsElementSearcher;
        }

        public bool EnterFileNameAndPressEnter(string fileName, string windowName, List<string> log) {
            var windowsChildElementSearchSpec = WindowsElementSearchSpec.Create("#32770", string.IsNullOrWhiteSpace(windowName) ? "Open" : "");
            var windowsGrandChildElementSearchSpec = WindowsElementSearchSpec.Create(UiClassNames.Edit, "File name:");
            windowsChildElementSearchSpec.WindowsChildElementSearchSpecs.Add(windowsGrandChildElementSearchSpec);
            var windowsElementSearchSpec = string.IsNullOrWhiteSpace(windowName)
                ? WindowsElementSearchSpec.Create("", "Desktop 1")
                : WindowsElementSearchSpec.Create(UiClassNames.Window, windowName);
            windowsElementSearchSpec.WindowsChildElementSearchSpecs.Add(windowsChildElementSearchSpec);

            var element = WindowsElementSearcher.SearchWindowsElement(windowsElementSearchSpec, log);

            if (element == null) {
                return false;
            }

            element.SendKeys("");
            element = WindowsElementSearcher.SearchWindowsElement(element, windowsGrandChildElementSearchSpec, log);
            if (element == null) {
                return false;
            }

            element.SendKeys("");
            var textBox = TextBoxFactory.Create(element);
            textBox.Clear();
            textBox.Text = fileName;
            if (textBox.Text != fileName) {
                return false;
            }

            element.SendKeys(Keys.Enter);
            return true;
        }
    }
}
