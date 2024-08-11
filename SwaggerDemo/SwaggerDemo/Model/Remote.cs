namespace SwaggerDemo.Model
{
    public class Remote
    {
        public int Id { get; set; }
        public string Host { get; set; } = null!;
        public int Port { get; set; }
        public string Command { get; set; } = null!;
        public string Result { get; set; } = null!;
        
        public string Usage { get;set; } = null!;
    }
}
