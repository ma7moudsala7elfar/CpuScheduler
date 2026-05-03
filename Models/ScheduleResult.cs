namespace CpuScheduler.Models
{
    public class ScheduleResult
    {
        public string ProcessId { get; set; } = string.Empty;
        public int ArrivalTime { get; set; }
        public int BurstTime { get; set; }
        public int StartTime { get; set; }
        public int FinishTime { get; set; }
        public int WaitingTime { get; set; }
        public int TurnaroundTime { get; set; }
    }
}
