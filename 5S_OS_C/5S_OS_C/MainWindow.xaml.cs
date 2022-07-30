using _5S_OS_C.Models;
using _5S_OS_C.Utils;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Newtonsoft.Json;
using ProcessInfoProj;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace _5S_OS_C
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainWindow : Window
    {
        public TaskScheduler UISyncContext { get; } = TaskScheduler.FromCurrentSynchronizationContext();
        public TaskManagerModel TaskManager { get; set; } = new TaskManagerModel();
        int wantRefreshInterval = 1000;
        bool stopTask = false;

        public MainWindow()
        {
            this.InitializeComponent();
            TaskManager.UISyncContext = UISyncContext;
            TaskManager.PropertyChanged += TaskManager_PropertyChanged;
        }

        private void TaskManager_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            //if (e.PropertyName == nameof(TaskManager.clientServer))
            //{
            //    TaskManager.clientServer.Close();

            //    StartServer.IsEnabled = true;
            //    ConnectServer.Content = "Connect";
            //    ConnectedServerIP.Text = "";
            Task.Run(() => { }).ContinueWith((t) =>
            {
                ContentDialogEx.Exception(Content.XamlRoot, "Server disconnected or error when getting data", "", "Ok");
            }, UISyncContext);
            //}
        }

        private void StartTask()
        {
            Task.Run(() =>
            {
                if (stopTask)
                {
                    return;
                }

                while (TaskManager.IsBusy)
                {
                    Thread.Sleep(wantRefreshInterval);
                }

                RefreshAndSend();

                StartTask();
            });
        }

        private void StartServer_Click(object sender, RoutedEventArgs e)
        {
            if (!TaskManager.clientServer.IsRunning)
            {
                try
                {
                    TaskManager.clientServer.Open("127.0.0.1", "49061", 3, true, TaskManager.Listener);
                }
                catch (Exception ex)
                {
                    ContentDialogEx exDlg = ContentDialogEx.Exception(Content.XamlRoot, "Failed to start ClientServer", ex.Message, "Ok");
                    _ = exDlg.ShowAsync();
                    TaskManager.clientServer.Close();
                    return;
                }

                StartTask();

                ConnectServer.IsEnabled = false;
                StartServer.Content = "Stop Server";
                ShowConnected.IsEnabled = true;
                ShowConnected.Content = "Hide connected";
                ConnectedClients.Visibility = Visibility.Visible;
            }
            else
            {
                TaskManager.clientServer.Close();
                ConnectServer.IsEnabled = true;
                StartServer.Content = "Start Server";
                ShowConnected.IsEnabled = false;
                ShowConnected.Content = "Show connected";
                ConnectedClients.Visibility = Visibility.Collapsed;
            }
        }

        private async void ConnectServer_Click(object sender, RoutedEventArgs e)
        {
            if (!TaskManager.clientServer.IsRunning)
            {
                ContentDialogEx dlg = ContentDialogEx.Request(Content.XamlRoot, "Enter server address", "IP or IP:Port", "Ok");
                await dlg.ShowAsync();

                string[] result = dlg.Result.Split(":");

                if (result[0] == "")
                {
                    return;
                }

                string IP = result[0];
                string port = result.Length > 1 ? result[1] : "49061";

                try
                {
                    TaskManager.clientServer.Open(IP, port, 3, false, TaskManager.Listener);
                }
                catch (Exception ex)
                {
                    ContentDialogEx exDlg = ContentDialogEx.Exception(Content.XamlRoot, "Failed to start ClientServer", ex.Message, "Ok");
                    _ = exDlg.ShowAsync();
                    return;
                }

                StartServer.IsEnabled = false;
                ConnectServer.Content = "Disconnect";
                ConnectedServerIP.Text = IP + ":" + port;
            }
            else
            {
                TaskManager.clientServer.Close();
                StartServer.IsEnabled = true;
                ConnectServer.Content = "Connect";
                ConnectedServerIP.Text = "";
            }
        }

        private void RefreshAndSend()
        {
            TaskManager.GetProcesses();
            TaskManager.SendToClient();
        }

        private void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            TaskManager.GetProcesses();
            TaskManager.SendToClient();
        }

        private void ShowConnected_Click(object sender, RoutedEventArgs e)
        {
            ConnectedClients.Visibility = ConnectedClients.Visibility == Visibility.Collapsed ? Visibility.Visible : Visibility.Collapsed;
            ShowConnected.Content = ConnectedClients.Visibility == Visibility.Collapsed ? "Show connected" : "Hide connected";
        }
    }
}
