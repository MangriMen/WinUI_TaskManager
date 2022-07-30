using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Toolkit.Mvvm.ComponentModel;

namespace ProcessInfoProj
{
    public class ProcessData
    {
        protected string _name;
        protected string _pid;
        protected string _userName;
        protected string _cpu;
        protected string _memory;

        protected bool isRunning = false;

        public virtual bool IsRunning
        {
            get
            {
                return isRunning;
            }
            protected set
            {
                isRunning = value;
            }
        }

        public string Name
        {
            get => _name;
            set => _name = value;
        }
        public string PID
        {
            get => _pid;
            set => _pid = value;
        }
        public string UserName
        {
            get => _userName;
            set => _userName = value;
        }
        public string CPU
        {
            get => _cpu;
            set => _cpu = value;
        }
        public string Memory
        {
            get => _memory;
            set => _memory = value;
        }

        public bool Equals(ProcessData other)
        {
            if (other is null)
                return false;

            return this.Name == other.Name && this.PID == other.PID;
        }

        public override bool Equals(object obj) => Equals(obj as ProcessData);
        public override int GetHashCode() => (Name, PID).GetHashCode();

        public virtual void Update()
        {
            //PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Name)));
            //PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(PID)));
            //PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(UserName)));
            //PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CPU)));
            //PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Memory)));
        }
    }

    public class ProcessWrapper : ProcessData
    {
        Process process_;
        PerformanceCounter ram_;
        PerformanceCounter cpu_;

        public new string Name
        {
            get
            {
                try
                {
                    _name = process_.ProcessName;
                }
                catch (Exception)
                {
                    _name = "Unknown";
                }

                return _name;
            }
        }
        public new string PID
        {
            get
            {
                try
                {
                    _pid = process_.Id.ToString();
                }
                catch (Exception)
                {
                    _pid = "Unknown";
                }

                return _pid;
            }
        }
        public new string UserName
        {
            get
            {
                try
                {
                    _userName = ProcessEx.GetProcessUser(process_);
                }
                catch (Exception)
                {
                    _userName = "Unknown";
                }

                return _userName;
            }
        }
        public new string CPU
        {
            get
            {
                try
                {
                    _cpu = Math.Round(cpu_.NextValue() / Environment.ProcessorCount).ToString();
                }
                catch (Exception)
                {
                    _cpu = "Unknown";
                }

                return _cpu;
            }
        }
        public new string Memory
        {
            get
            {
                try
                {
                    _memory = Math.Round(ram_.NextValue() / 1024).ToString() + " K";
                }
                catch (Exception)
                {
                    _memory = "Unknown";
                }

                return _memory;
            }
        }

        public ProcessWrapper(Process process)
        {
            process_ = process;
            process_.Exited += Process__Exited;
            ram_ = new PerformanceCounter("Process", "Working Set - Private", process_.ProcessName, true);
            cpu_ = new PerformanceCounter("Process", "% Processor Time", process_.ProcessName, true);
            IsRunning = true;
        }

        private void Process__Exited(object sender, EventArgs e)
        {
            IsRunning = false;
        }

        public override void Update()
        {
            process_?.Refresh();
            //Name = process_.ProcessName;
            //PID = process_.Id.ToString();
            //UserName = ProcessEx.GetProcessUser(process_);
            //CPU = Math.Round((cpu_.NextValue() / Environment.ProcessorCount)).ToString();
            //Memory = Math.Round((ram_.NextValue() / 1024)).ToString() + " K";
        }

        public ProcessData GetProcessData()
        {
            try
            {
                return new()
                {
                    Name = process_.ProcessName,
                    PID = process_.Id.ToString(),
                    UserName = ProcessEx.GetProcessUser(process_),
                    CPU = Math.Round(cpu_.NextValue() / Environment.ProcessorCount).ToString(),
                    Memory = Math.Round(ram_.NextValue() / 1024).ToString() + " K",
                };
            }
            catch (Exception)
            {
                return new();
            }
        }
    }
}
