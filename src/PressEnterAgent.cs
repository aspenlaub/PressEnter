using System.Collections.Generic;
using Aspenlaub.Net.GitHub.CSharp.Paleface;
using OpenQA.Selenium;

namespace Aspenlaub.Net.GitHub.CSharp.PressEnter {
    public class PressEnterAgent {
        protected WindowsElementSearcher WindowsElementSearcher = new WindowsElementSearcher();

        public bool EnterFileNameAndPressEnter(string fileName, List<string> log) {
            var windowsElementSearchSpec = WindowsElementSearchSpec.Create("", "Desktop 1");
            var windowsChildElementSearchSpec = WindowsElementSearchSpec.Create("", "Open");
            windowsChildElementSearchSpec.ClassNames.AddRange(new [] { "#32769", "#32770" });
            windowsElementSearchSpec.WindowsChildElementSearchSpecs.Add(windowsChildElementSearchSpec);
            var windowsGrandChildElementSearchSpec = WindowsElementSearchSpec.Create(UiClassNames.Edit, "File name:");
            windowsChildElementSearchSpec.WindowsChildElementSearchSpecs.Add(windowsGrandChildElementSearchSpec);

            var element = WindowsElementSearcher.SearchWindowsElement(windowsElementSearchSpec, log);
            if (element == null) {
                return false;
            }

            element = WindowsElementSearcher.SearchWindowsElement(element, windowsGrandChildElementSearchSpec, log);
            if (element == null) {
                return false;
            }

            element.Click();
            var textBox = new TextBox(element);
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
