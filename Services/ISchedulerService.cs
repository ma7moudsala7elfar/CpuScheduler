using CpuScheduler.Models;

namespace CpuScheduler.Services
{
    public interface ISchedulerService
    {
        List<ScheduleResult> RunFCFS(List<Process> processes);
        List<ScheduleResult> RunSJF(List<Process> processes);
    }
}
