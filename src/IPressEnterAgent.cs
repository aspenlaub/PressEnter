using System.Collections.Generic;

namespace Aspenlaub.Net.GitHub.CSharp.PressEnter {
    public interface IPressEnterAgent {
        bool EnterFileNameAndPressEnter(string fileName, string windowName, List<string> log);
    }
}