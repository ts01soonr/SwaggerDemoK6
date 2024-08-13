using SwaggerDemo.Services;
using SwaggerDemo.Helper;
class Program
{
    static void Main(string[] args)
    {
        //String msg=Sys.Tel("localhost",8888,"who");
        //Console.WriteLine(msg);
        //if (true) return;
        var builder = WebApplication.CreateBuilder(args);
        // URL https://localhost:5001/swagger/index.html
        // Add services to the container.
        builder.Services.AddScoped<IClientService, ClientService>();
        builder.Services.AddScoped<IEmployeeService, EmployeeService>();
        builder.Services.AddScoped<IRemoteService, RemoteService>();
        builder.Services.AddControllers();
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
            Sys.isDevelopment = true;
        }

        //app.UseHttpsRedirection();

        app.UseAuthorization();

        app.MapControllers();

        app.Run();
    }


}
