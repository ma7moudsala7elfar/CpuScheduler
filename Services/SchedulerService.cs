using CpuScheduler.Models;

namespace CpuScheduler.Services
{
    public class SchedulerService : ISchedulerService
    {
        /// <summary>
        /// First-Come, First-Served scheduling.
        /// Processes are sorted by arrival time and executed in that order.
        /// </summary>
        public List<ScheduleResult> RunFCFS(List<Process> processes)
        {
            var sorted = processes.OrderBy(p => p.ArrivalTime).ThenBy(p => p.Id).ToList();
            var results = new List<ScheduleResult>();
            int currentTime = 0;

            foreach (var proc in sorted)
            {
                int start = Math.Max(currentTime, proc.ArrivalTime);
                int finish = start + proc.BurstTime;
                int waiting = start - proc.ArrivalTime;
                int turnaround = finish - proc.ArrivalTime;

                results.Add(new ScheduleResult
                {
                    ProcessId = $"P{proc.Id}",
                    ArrivalTime = proc.ArrivalTime,
                    BurstTime = proc.BurstTime,
                    StartTime = start,
                    FinishTime = finish,
                    WaitingTime = waiting,
                    TurnaroundTime = turnaround
                });

                currentTime = finish;
            }

            return results;
        }

        /// <summary>
        /// Shortest Job First (Non-Preemptive) scheduling.
        /// At each scheduling decision, the arrived process with the shortest burst is selected.
        /// </summary>
        public List<ScheduleResult> RunSJF(List<Process> processes)
        {
            var remaining = processes.ToList();
            var results = new List<ScheduleResult>();
            int currentTime = 0;

            while (remaining.Count > 0)
            {
                // Find all processes that have arrived by currentTime
                var available = remaining.Where(p => p.ArrivalTime <= currentTime).ToList();

                if (available.Count == 0)
                {
                    // No process has arrived yet — jump to the earliest arrival
                    currentTime = remaining.Min(p => p.ArrivalTime);
                    available = remaining.Where(p => p.ArrivalTime <= currentTime).ToList();
                }

                // Pick the one with the shortest burst; break ties by arrival, then by Id
                var selected = available
                    .OrderBy(p => p.BurstTime)
                    .ThenBy(p => p.ArrivalTime)
                    .ThenBy(p => p.Id)
                    .First();

                int start = Math.Max(currentTime, selected.ArrivalTime);
                int finish = start + selected.BurstTime;
                int waiting = start - selected.ArrivalTime;
                int turnaround = finish - selected.ArrivalTime;

                results.Add(new ScheduleResult
                {
                    ProcessId = $"P{selected.Id}",
                    ArrivalTime = selected.ArrivalTime,
                    BurstTime = selected.BurstTime,
                    StartTime = start,
                    FinishTime = finish,
                    WaitingTime = waiting,
                    TurnaroundTime = turnaround
                });

                currentTime = finish;
                remaining.Remove(selected);
            }

            return results;
        }
    }
}
