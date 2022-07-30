using _5S_OS_C.Utils;
using CommunityToolkit.WinUI.UI.Controls;
using Newtonsoft.Json;
using _5S_OS_C;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using TestCS;
using Windows.Foundation;
using ProcessInfoProj;

namespace _5S_OS_C.Models
{
    public class TaskManagerModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public TaskScheduler UISyncContext { get; set; }

        public ProcessInfo processInfo { get; set; } = new();
        public RangeObservableCollection<ProcessData> Processes = new();

        public ClientServer clientServer { get; set; } = new();
        public ObservableCollection<string> Clients { get; set; } = new ObservableCollection<string>();

        public bool IsBusy { get; set; }

        public void SendToClient()
        {
            ProcessData[] dataArr = Array.Empty<ProcessData>();
            lock (Processes)
            {
                dataArr = Processes.ToArray();
            }

            try
            {
                string data = JsonConvert.SerializeObject(dataArr);
                clientServer.SendMessage(data);
            }
            catch (Exception ex)
            {
                Task.Run(() => { }).ContinueWith((t) =>
                  {
                      ContentDialogEx.Exception(MainWindow.Current.Content.XamlRoot, "Unable send data to client", ex.Message, "Ok");
                  }, UISyncContext);
            }
        }

        public void Listener(Tuple<string, IPEndPoint> package)
        {
            if (package.Item1 == int.MinValue.ToString() || package.Item1 == int.MaxValue.ToString())
            {
                if (clientServer.IsHost)
                {
                    Task.Run(() => { }).ContinueWith((t) =>
                    {
                        Clients.Clear();
                        foreach (var client in clientServer.Clients)
                        {
                            Clients.Add(client.Item1 + ":" + client.Item2.ToString());
                        }
                    }, UISyncContext);
                }
                else
                {
                    if (package.Item1 == int.MaxValue.ToString())
                    {
                        Task.Run(() => { }).ContinueWith((t) =>
                        {
                            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(clientServer)));
                        }, UISyncContext);
                    }
                }
                return;
            }

            try
            {
                ProcessData[] data = Array.Empty<ProcessData>();
                try
                {
                    data = JsonConvert.DeserializeObject<ProcessData[]>(package.Item1);
                }
                catch (Exception)
                {
                    Task.Run(() => { }).ContinueWith((t) =>
                      {
                          PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(clientServer)));
                      }, UISyncContext);
                    return;
                }

                Task addTask = Task.Run(() =>
                {
                });
                addTask.ContinueWith((t) =>
                {
                    lock (Processes)
                    {
                        Processes.Clear();
                        Processes.AddRange(data);
                    }
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Processes)));
                }, UISyncContext);
            }
            catch (Exception)
            {
                return;
            }
        }

        public void GetProcesses()
        {
            IsBusy = true;
            List<ProcessData> temp = new();
            Task task = Task.Run(() =>
            {
                temp = processInfo.GetProcessDataList();
            });
            task.ContinueWith((t) =>
            {
                lock (Processes)
                {
                    Processes.Clear();
                    Processes.AddRange(temp);
                }
                IsBusy = false;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Processes)));
            }, UISyncContext);
        }

        public void Sorting(object sender, CommunityToolkit.WinUI.UI.Controls.DataGridColumnEventArgs e)
        {
            var dg = (DataGrid)sender;
            ObservableCollection<ProcessData> temp = Processes;

            if (e.Column.Header.ToString() == "Name")
            {
                if (e.Column.SortDirection == null || e.Column.SortDirection == DataGridSortDirection.Descending)
                {
                    temp = new ObservableCollection<ProcessData>(from item in Processes
                                                                 orderby item.Name ascending
                                                                 select item);
                    e.Column.SortDirection = DataGridSortDirection.Ascending;
                }
                else
                {
                    temp = new ObservableCollection<ProcessData>(from item in Processes
                                                                 orderby item.Name descending
                                                                 select item);
                    e.Column.SortDirection = DataGridSortDirection.Descending;
                }
            }
            else if (e.Column.Header.ToString() == "PID")
            {
                if (e.Column.SortDirection == null || e.Column.SortDirection == DataGridSortDirection.Descending)
                {
                    temp = new ObservableCollection<ProcessData>(from item in Processes
                                                                 orderby item.PID ascending
                                                                 select item);
                    e.Column.SortDirection = DataGridSortDirection.Ascending;
                }
                else
                {
                    temp = new ObservableCollection<ProcessData>(from item in Processes
                                                                 orderby item.PID descending
                                                                 select item);
                    e.Column.SortDirection = DataGridSortDirection.Descending;
                }
            }
            else if (e.Column.Header.ToString() == "CPU")
            {
                if (e.Column.SortDirection == null || e.Column.SortDirection == DataGridSortDirection.Descending)
                {
                    temp = new ObservableCollection<ProcessData>(from item in Processes
                                                                 orderby item.CPU ascending
                                                                 select item);
                    e.Column.SortDirection = DataGridSortDirection.Ascending;
                }
                else
                {
                    temp = new ObservableCollection<ProcessData>(from item in Processes
                                                                 orderby item.CPU descending
                                                                 select item);
                    e.Column.SortDirection = DataGridSortDirection.Descending;
                }
            }
            else if (e.Column.Header.ToString() == "Memory")
            {
                if (e.Column.SortDirection == null || e.Column.SortDirection == DataGridSortDirection.Descending)
                {
                    temp = new ObservableCollection<ProcessData>(from item in Processes
                                                                 orderby item.Memory ascending
                                                                 select item);
                    e.Column.SortDirection = DataGridSortDirection.Ascending;
                }
                else
                {
                    temp = new ObservableCollection<ProcessData>(from item in Processes
                                                                 orderby item.Memory descending
                                                                 select item);
                    e.Column.SortDirection = DataGridSortDirection.Descending;
                }
            }

            Processes.Clear();
            foreach (var sortedProcess in temp)
            {
                Processes.Add(sortedProcess);
            }

            // add code to handle sorting by other columns as required

            //Remove sorting indicators from other columns
            foreach (var dgColumn in dg.Columns)
            {
                if (dgColumn.Header.ToString() != e.Column.Header.ToString())
                {
                    dgColumn.SortDirection = null;
                }
            }

        }
    }

}
