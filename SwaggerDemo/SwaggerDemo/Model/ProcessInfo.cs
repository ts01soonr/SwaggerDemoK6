namespace SwaggerDemo.Model
{
    public class ProcessInfo
    {
        public int processId { get; set; }
        public string processName { get; set; } = null!;
        public string cpuUsage { get; set; } = null!;
        public string memoryUsageMB { get; set; } = null!;
    }
}
