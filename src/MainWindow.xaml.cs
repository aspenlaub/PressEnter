using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace Aspenlaub.Net.GitHub.CSharp.PressEnter {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    // ReSharper disable once UnusedMember.Global
    public partial class MainWindow {
        protected int NumberOfLoops;
        protected string FileName;
        protected string ResponseFileName;
        protected string Response;
        protected string WindowName;
        protected string[] Arguments;
        protected PressEnterAgent Agent;

        public MainWindow() {
            InitializeComponent();
            Arguments = Environment.GetCommandLineArgs();
            NumberOfLoops = -1;
            FileName = "";
            ResponseFileName = "";
            Response = Properties.Resources.NoUploadWindowFound;
            if (Arguments.Length <= 1) { return; }
            if (!int.TryParse(Arguments[1], out NumberOfLoops)) { return; }

            if (Arguments.Length <= 2) { return; }

            FileName = Arguments[2];

            Agent = new PressEnterAgent();

            if (Arguments.Length <= 3) { return; }

            ResponseFileName = Arguments[3];

            if (Arguments.Length <= 4) { return; }

            WindowName = Arguments[4];
        }

        private async Task<bool> RefreshOnce() {
            Results.Text += "\r\n" + DateTime.Now.ToLongTimeString() + " " + Properties.Resources.Refreshing;

            if (string.IsNullOrWhiteSpace(FileName)) {
                SetOverallResultText(Properties.Resources.FileNameIsEmpty);
                return false;
            }

            if (!File.Exists(FileName)) {
                SetOverallResultText(Properties.Resources.FileWithThatNameDoesNotExist);
                return false;
            }

            var log = new List<string>();
            var okay = await Task.Run(() => Agent.EnterFileNameAndPressEnter(FileName, WindowName, log));
            SetOverallResultText(okay ? Properties.Resources.FileNameEntered : Properties.Resources.NoUploadWindowFound);
            if (!okay) {
                log.Where(s => !string.IsNullOrWhiteSpace(s)).ToList().ForEach(s => Results.Text += "\r\n" + DateTime.Now.ToLongTimeString() + " " + s);
            }
            return okay;
        }

        private void SetOverallResultText(string s) {
            if (Response == Properties.Resources.FileNameEntered) { return; }

            Results.Text += s;
            Response = s;
        }

        private void BringInFront() {
            Activate();
            Topmost = true;
        }

        private async Task Refresh() {
            do {
                var okay = await RefreshOnce();
                Results.ScrollToEnd();
                if (okay) {
                    break;
                }

                if (NumberOfLoops == 0) {
                    Results.Text += "\r\n" + DateTime.Now.ToLongTimeString() + " " + string.Format(Properties.Resources.NumberOfRefrehsIsNow, NumberOfLoops);
                    break;
                }

                if (NumberOfLoops > 0) {
                    NumberOfLoops--;
                    Results.Text += "\r\n" + DateTime.Now.ToLongTimeString() + " " + string.Format(Properties.Resources.NumberOfRefrehsIsNow, NumberOfLoops);
                }
                await Task.Delay(TimeSpan.FromSeconds(5));
            } while (NumberOfLoops != 0);

            await Task.Delay(TimeSpan.FromSeconds(2));
            Close();
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e) {
            BringInFront();

            Results.Text = DateTime.Now.ToLongTimeString() + " " + string.Format(Properties.Resources.NumberOfArguments, Arguments.Length)
                + "\r\n" + DateTime.Now.ToLongTimeString() + " " + string.Format(Properties.Resources.FirstCommandLineArgument, Arguments.Length <= 1 ? "" : Arguments[1])
                + "\r\n" + DateTime.Now.ToLongTimeString() + " " + (NumberOfLoops < 0 ? Properties.Resources.EndlessRefresh : string.Format(Properties.Resources.LimitedNumberOfRefreshs, NumberOfLoops));
            await Refresh();
        }

        private void Window_Closed(object sender, EventArgs e) {
            if (!string.IsNullOrEmpty(ResponseFileName) && File.Exists(ResponseFileName)) {
                WaitUntil(() => ResponseFileWasWritten(), 10000);
            }
            Environment.Exit(0);
        }

        private bool ResponseFileWasWritten() {
            try {
                File.WriteAllText(ResponseFileName, Response);
                return true;
            } catch {
                return false;
            }
        }

        private static void WaitUntil(Func<bool> condition, int miliSeconds) {
            var internalMiliSeconds = 1 + miliSeconds / 20;
            do {
                if (condition()) { return; }

                Thread.Sleep(internalMiliSeconds); // Do not use await Task.Delay here
                miliSeconds = miliSeconds - internalMiliSeconds;
            } while (miliSeconds >= 0);

        }
    }
}
