using System.Text.Json;
using CpuScheduler.Models;
using CpuScheduler.Services;
using Microsoft.AspNetCore.Mvc;

namespace CpuScheduler.Controllers
{
    public class SchedulerController : Controller
    {
        private readonly ISchedulerService _scheduler;
        private const string SessionKey = "ProcessList";

        public SchedulerController(ISchedulerService scheduler)
        {
            _scheduler = scheduler;
        }

        private List<Process> GetSessionProcesses()
        {
            var json = HttpContext.Session.GetString(SessionKey);
            if (string.IsNullOrEmpty(json))
                return new List<Process>();
            return JsonSerializer.Deserialize<List<Process>>(json) ?? new List<Process>();
        }

        private void SaveSessionProcesses(List<Process> processes)
        {
            HttpContext.Session.SetString(SessionKey, JsonSerializer.Serialize(processes));
        }

        [HttpGet]
        public IActionResult Index()
        {
            var processes = GetSessionProcesses();
            var vm = new SchedulerViewModel
            {
                Processes = processes,
                SelectedAlgorithm = "FCFS"
            };
            return View(vm);
        }

        [HttpPost]
        public IActionResult AddProcess(SchedulerViewModel vm, int arrivalTime, int burstTime)
        {
            // Validate arrival time
            if (arrivalTime < 0)
            {
                var procs = GetSessionProcesses();
                vm.Processes = procs;
                vm.ErrorMessage = "Arrival Time must be a non-negative integer (>= 0).";
                return View("Index", vm);
            }

            // Validate burst time
            if (burstTime < 1)
            {
                var procs = GetSessionProcesses();
                vm.Processes = procs;
                vm.ErrorMessage = "Burst Time must be a positive integer (>= 1).";
                return View("Index", vm);
            }

            var processes = GetSessionProcesses();
            int nextId = processes.Count > 0 ? processes.Max(p => p.Id) + 1 : 1;

            processes.Add(new Process
            {
                Id = nextId,
                ArrivalTime = arrivalTime,
                BurstTime = burstTime
            });

            SaveSessionProcesses(processes);

            vm.Processes = processes;
            vm.Results = null;
            vm.ErrorMessage = null;
            return View("Index", vm);
        }

        [HttpPost]
        public IActionResult RemoveProcess(SchedulerViewModel vm, int index)
        {
            var processes = GetSessionProcesses();

            if (index >= 0 && index < processes.Count)
            {
                processes.RemoveAt(index);
            }

            SaveSessionProcesses(processes);

            vm.Processes = processes;
            vm.Results = null;
            vm.ErrorMessage = null;
            return View("Index", vm);
        }

        [HttpPost]
        public IActionResult ClearAll()
        {
            HttpContext.Session.Remove(SessionKey);
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult Run(SchedulerViewModel vm)
        {
            var processes = GetSessionProcesses();

            if (processes.Count == 0)
            {
                vm.Processes = processes;
                vm.ErrorMessage = "Please add at least one process before running the algorithm.";
                return View("Index", vm);
            }

            List<ScheduleResult> results;

            if (vm.SelectedAlgorithm == "SJF")
            {
                results = _scheduler.RunSJF(processes);
            }
            else
            {
                results = _scheduler.RunFCFS(processes);
            }

            vm.Processes = processes;
            vm.Results = results;

            if (results.Count > 0)
            {
                vm.AverageWaitingTime = Math.Round(results.Average(r => r.WaitingTime), 2);
                vm.AverageTurnaroundTime = Math.Round(results.Average(r => r.TurnaroundTime), 2);
                vm.TotalExecutionTime = results.Max(r => r.FinishTime);
            }

            vm.ErrorMessage = null;
            return View("Index", vm);
        }
    }
}
