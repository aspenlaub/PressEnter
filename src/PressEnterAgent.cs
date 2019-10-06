﻿using System.Linq;
using Aspenlaub.Net.GitHub.CSharp.Paleface;
using OpenQA.Selenium;
using OpenQA.Selenium.Appium;

namespace Aspenlaub.Net.GitHub.CSharp.PressEnter {
    public class PressEnterAgent {
        protected WindowsElementSearcher WindowsElementSearcher = new WindowsElementSearcher();

        public bool EnterFileNameAndPressEnter(string fileName) {
            var windowsElementSearchSpec = WindowsElementSearchSpec.Create("window", "");
            var windowsChildElementSearchSpec = WindowsElementSearchSpec.Create("dialog", "");
            windowsElementSearchSpec.WindowsChildElementSearchSpecs.Add(windowsChildElementSearchSpec);
            var windowsGrandChildElementSearchSpec = new WindowsElementSearchSpec { Name = "File name:", LocalizedControlType = "edit" };
            windowsChildElementSearchSpec.WindowsChildElementSearchSpecs.Add(windowsGrandChildElementSearchSpec);
            AppiumWebElement element = WindowsElementSearcher.SearchWindowsElement(windowsElementSearchSpec);
            if (element == null) {
                return false;
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
    }
}
