using System.Collections.Generic;
using System.Linq;
using Aspenlaub.Net.GitHub.CSharp.Paleface;
using OpenQA.Selenium;
using OpenQA.Selenium.Appium;

namespace Aspenlaub.Net.GitHub.CSharp.PressEnter {
    public class PressEnterAgent {
        protected WindowsElementSearcher WindowsElementSearcher = new WindowsElementSearcher();

        public bool EnterFileNameAndPressEnter(string fileName, List<string> log) {
            var windowsElementSearchSpec = WindowsElementSearchSpec.Create("window", "");
            var windowsGrandChildElementSearchSpec = new WindowsElementSearchSpec { Name = "File name:", LocalizedControlType = "edit" };
            var element = FindFileDialog(windowsElementSearchSpec, windowsGrandChildElementSearchSpec, log);
            if (element == null) {
                windowsElementSearchSpec = WindowsElementSearchSpec.Create("pane", "");
                windowsElementSearchSpec.NameMustNotBeEmpty = true;
                windowsElementSearchSpec.NameDoesNotContain = "Desktop";
                element = FindFileDialog(windowsElementSearchSpec, windowsGrandChildElementSearchSpec, log);
                log.Insert(0, Properties.Resources.NoPaneWasFound);
                if (element == null) {
                    return false;
                }
            }

            element = element.FindElementsByWindowsElementSearchSpec(windowsGrandChildElementSearchSpec).FirstOrDefault();
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

        private AppiumWebElement FindFileDialog(WindowsElementSearchSpec windowsElementSearchSpec, WindowsElementSearchSpec windowsGrandChildElementSearchSpec,
                List<string> log) {
            var windowsChildElementSearchSpec = WindowsElementSearchSpec.Create("dialog", "");
            windowsChildElementSearchSpec.WindowsChildElementSearchSpecs.Add(windowsGrandChildElementSearchSpec);
            windowsElementSearchSpec.WindowsChildElementSearchSpecs.Add(windowsChildElementSearchSpec);
            return WindowsElementSearcher.SearchWindowsElement(windowsElementSearchSpec, log);
        }
    }
}
