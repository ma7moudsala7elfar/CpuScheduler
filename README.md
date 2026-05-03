# CPU Scheduling Simulator

A web-based simulator built with ASP.NET Core MVC that demonstrates CPU scheduling algorithms with visual Gantt charts. Users can input processes with arrival and burst times, select a scheduling algorithm, and instantly view the scheduling results including a color-coded Gantt chart, per-process metrics, and average statistics.

## Algorithms Implemented

### FCFS (First Come First Serve)

FCFS is the simplest CPU scheduling algorithm. Processes are executed in the exact order they arrive in the ready queue. The CPU is allocated to the first process that requests it and runs to completion before moving to the next. It is non-preemptive by nature, meaning once a process starts executing, it cannot be interrupted. While straightforward to implement, FCFS can suffer from the **convoy effect**, where short processes are stuck waiting behind a long-running process.

### SJF Non-Preemptive (Shortest Job First)

SJF selects the process with the smallest burst time from the set of processes that have arrived in the ready queue. In the non-preemptive variant, once a process begins execution, it runs to completion before the scheduler re-evaluates. If two processes have the same burst time, they are scheduled in FCFS order. SJF is provably optimal in terms of minimizing average waiting time, but it requires advance knowledge of each process's burst time.

## Project Structure

```
CpuScheduler/
├── Controllers/
│   └── SchedulerController.cs        # Handles HTTP requests, session management, and action routing
├── Models/
│   ├── Process.cs                     # Data model representing a single process (Id, ArrivalTime, BurstTime)
│   ├── ScheduleResult.cs              # Data model for per-process scheduling output (start, finish, waiting, turnaround)
│   └── SchedulerViewModel.cs          # View model combining process list, results, averages, and selected algorithm
├── Services/
│   ├── ISchedulerService.cs           # Interface defining the scheduling service contract (RunFCFS, RunSJF)
│   └── SchedulerService.cs            # Concrete implementation of FCFS and SJF scheduling logic
├── Views/
│   ├── Scheduler/
│   │   └── Index.cshtml               # Main Razor view with process input form, Gantt chart, and results table
│   ├── _ViewImports.cshtml            # Global Razor directives and tag helper imports
│   └── _ViewStart.cshtml              # Shared view startup configuration
├── Properties/
│   └── launchSettings.json            # Development server URLs and environment settings
├── wwwroot/
│   ├── css/
│   │   ├── scheduler.css              # Primary stylesheet for the scheduler dashboard (dark theme, layout, animations)
│   │   └── site.css                   # Global site-wide base styles
│   ├── js/
│   │   ├── scheduler.js               # Client-side logic for Gantt chart rendering and UI interactions
│   │   └── site.js                    # Global site-wide JavaScript utilities
│   ├── lib/                           # Third-party client libraries (Bootstrap, jQuery, jQuery Validation)
│   └── favicon.ico                    # Browser tab icon
├── Program.cs                         # Application entry point: service registration, middleware pipeline, routing
├── CpuScheduler.csproj                # MSBuild project file targeting .NET 8
├── CpuScheduler.sln                   # Visual Studio solution file
├── appsettings.json                   # Application configuration (logging, allowed hosts)
└── appsettings.Development.json       # Development-specific configuration overrides
```

## Architecture

### MVC Pattern

This project follows the **Model-View-Controller** architectural pattern:

- **Model** (`Models/`): Defines the data structures used throughout the application. `Process` represents input data, `ScheduleResult` holds computed output per process, and `SchedulerViewModel` aggregates everything the view needs to render.
- **View** (`Views/`): Razor `.cshtml` templates that render the HTML UI. The main `Index.cshtml` view displays the algorithm selector, the process input form, the Gantt chart, metric cards, and the results table.
- **Controller** (`Controllers/`): The `SchedulerController` receives HTTP requests, coordinates between the session store and the service layer, and returns the appropriate view with populated data.

### Service Layer

The scheduling logic is deliberately extracted into a dedicated **Service Layer** behind the `ISchedulerService` interface, with `SchedulerService` as its implementation. This separation follows the **Single Responsibility Principle**: the controller handles HTTP concerns (request routing, session management, model binding), while the service encapsulates pure algorithmic logic. This makes the scheduling algorithms independently testable, reusable, and swappable without modifying controller code.

The service is registered in the DI container with a **Scoped** lifetime in `Program.cs`:

```csharp
builder.Services.AddScoped<ISchedulerService, SchedulerService>();
```

### Session-Based State Management

The list of processes is persisted across requests using **ASP.NET Core's in-memory session storage**. This allows users to add multiple processes one at a time without losing previous entries. The session is configured with a 30-minute idle timeout in `Program.cs`:

```csharp
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});
```

## Getting Started

### Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0) (or later)

### Installation

1. Clone the repository:

   ```bash
   git clone https://github.com/your-username/CpuScheduler.git
   cd CpuScheduler
   ```

2. Restore dependencies and run the application:

   ```bash
   dotnet run
   ```

3. Open your browser and navigate to:

   ```
   https://localhost:7139
   ```

   Or using HTTP:

   ```
   http://localhost:5213
   ```

## How to Use

1. **Select an Algorithm** -- Choose either **FCFS** or **SJF Non-Preemptive** from the algorithm dropdown.
2. **Enter Process Details** -- Input the **Arrival Time** and **Burst Time** for a process.
3. **Add the Process** -- Click the **Add Process** button. The process appears in the process list.
4. **Repeat** -- Add as many processes as needed by repeating steps 2-3.
5. **Run the Algorithm** -- Click **Run Algorithm** to execute the selected scheduling algorithm.
6. **View Results**:
   - **Gantt Chart**: A horizontal, color-coded bar chart showing the timeline of process execution.
   - **Metric Cards**: Summary cards displaying Average Waiting Time, Average Turnaround Time, and Total Execution Time.
   - **Result Table**: A detailed table listing each process with its Arrival Time, Burst Time, Start Time, Finish Time, Waiting Time, and Turnaround Time.

## Scheduling Algorithm Formulas

The following formulas are used to compute scheduling metrics for each process:

| Metric                     | Formula                                    |
| -------------------------- | ------------------------------------------ |
| Start Time                 | `max(current_time, arrival_time)`          |
| Finish Time                | `start_time + burst_time`                  |
| Waiting Time               | `start_time - arrival_time`                |
| Turnaround Time            | `finish_time - arrival_time`               |
| Average Waiting Time       | `sum(waiting_time) / n`                    |
| Average Turnaround Time    | `sum(turnaround_time) / n`                 |

Where `current_time` tracks the CPU clock and `n` is the total number of processes.

## Tech Stack

| Layer       | Technology                          |
| ----------- | ----------------------------------- |
| Backend     | ASP.NET Core MVC (.NET 8)           |
| Views       | Razor Pages (`.cshtml`)             |
| Styling     | Vanilla CSS (custom dark theme)     |
| Scripting   | Vanilla JavaScript                  |
| State       | In-memory Session storage           |
| Libraries   | Bootstrap, jQuery, jQuery Validation |

## License

This project is licensed under the [MIT License](https://opensource.org/licenses/MIT).
