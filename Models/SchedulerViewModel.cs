namespace CpuScheduler.Models
{
    public class SchedulerViewModel
    {
        public List<Process> Processes { get; set; } = new();
        public List<ScheduleResult>? Results { get; set; }
        public string SelectedAlgorithm { get; set; } = "FCFS";
        public double AverageWaitingTime { get; set; }
        public double AverageTurnaroundTime { get; set; }
        public int TotalExecutionTime { get; set; }
        public string? ErrorMessage { get; set; }
    }
}
