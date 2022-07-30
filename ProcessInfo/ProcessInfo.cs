using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProcessInfoProj
{
    public class ProcessInfo
    {
        Process[] _processes;
        List<ProcessWrapper> _processWrappers;

        public List<Process> Processes
        {
            get => _processes.ToList();
        }

        public List<ProcessWrapper> ProcessWrappers
        {
            get => _processWrappers;
        }

        public ProcessInfo()
        {
            _processes = Process.GetProcesses();
            _processWrappers = new();

            for (long i = 0; i < _processes.Length; i++)
            {
                _processWrappers.Add(new ProcessWrapper(_processes[i]));
            }
        }

        public List<Process> GetNewProcessList()
        {
            _processes = Process.GetProcesses();
            return Processes;
        }

        public List<ProcessData> GetProcessDataList()
        {
            List<ProcessData> processDataList = new();

            for (int i = 0; i < _processWrappers.Count; i++)
            {
                if (!_processWrappers[i].IsRunning)
                {
                    _processWrappers.RemoveAt(i);
                }
                else
                {
                    processDataList.Add(_processWrappers[i].GetProcessData());
                }
            }

            return processDataList;
        }
    }
}
