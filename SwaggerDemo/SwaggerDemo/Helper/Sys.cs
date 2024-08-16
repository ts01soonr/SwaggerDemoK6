using System.Net.Sockets;
using System.Text;
using System.Diagnostics;
using SwaggerDemo.Model;

namespace SwaggerDemo.Helper
{
    public class Sys
    {
        public static bool isDevelopment = false;
        public static string Run(string command)
        {

            // Initialize a new process
            Process process = new Process();
            process.StartInfo.FileName = "cmd.exe";
            process.StartInfo.Arguments = $"/C {command}";
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.CreateNoWindow = true;

            try
            {
                // Start the process
                process.Start();

                // Read the output stream
                string result = process.StandardOutput.ReadToEnd();

                // Wait for the process to exit
                process.WaitForExit();

                // Output the result
                Console.WriteLine(result);
                return result;
            }
            catch (Exception ex)
            {
                // Handle any errors
                Console.WriteLine($"An error occurred: {ex.Message}");
                return "ERROR";
            }
        }
        public static string Tel(String host, int port, String message)
        {
            // Connect to the server
            //string serverAddress = "127.0.0.1"; // localhost
            //int port = 8888;
            try
            {
                TcpClient client = new TcpClient(host, port);
                Console.WriteLine("Connected to server.");

                // Send Welcome message
                NetworkStream stream = client.GetStream();
                byte[] buffer = new byte[1024];
                int bytesRead = stream.Read(buffer, 0, buffer.Length);
                string receivedMessage = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                //Console.WriteLine("Welcome message: " + receivedMessage);

                //string message = "run dir";
                string randomID = "id_" + new Random().Next();
                string result = "";
                byte[] messageBytes = Encoding.UTF8.GetBytes($"{message}\necho {randomID}\n");
                stream.Write(messageBytes, 0, messageBytes.Length);
                //Console.WriteLine("Message sent.");
                // Receive a response from the server
                do
                {
                    bytesRead = stream.Read(buffer, 0, buffer.Length);
                    receivedMessage = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                    result += receivedMessage;
                    //receivedMessage = receivedMessage.Trim();
                    //Console.WriteLine("Received message1: " + receivedMessage);
                } while (!receivedMessage.Contains(randomID));
                result = result.Replace($"echo {randomID} OK", "").Trim();
                //Console.WriteLine("Final result:" + result);
                // Receive a response from the server
                //bytesRead = stream.Read(buffer, 0, buffer.Length);
                //receivedMessage = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                //receivedMessage = receivedMessage.Trim();
                //Console.WriteLine("Received message2: " + receivedMessage);
                // Close the connection
                client.Close();
                return result;
            }
            catch (Exception e)
            {
                return e.Message;
            }
            //Console.WriteLine("Client closed.");
        }
        public static ProcessInfo Get_CPU_Memory(string name)
        {
            if (name.Length == 0 || name.Equals("info",StringComparison.InvariantCultureIgnoreCase)) 
                return Get_CPU_Memory();
            string processName = name;
            ProcessInfo processInfo = new ProcessInfo();
            try
            {
                // Get the process by name
                Process[] processes = Process.GetProcessesByName(processName);
                if (processes.Length == 0)
                {
                    Console.WriteLine($"Process '{processName}' not found.");
                    return processInfo;
                }
                Process process = processes[0];
                // Initialize variables to calculate CPU usage
                TimeSpan previousTotalProcessorTime = process.TotalProcessorTime;
                DateTime previousTime = DateTime.Now;
                {
                    // Get the current process memory usage
                    float memoryUsageMB = process.WorkingSet64 / (1024 * 1024);

                    // Calculate CPU usage
                    TimeSpan currentTotalProcessorTime = process.TotalProcessorTime;
                    DateTime currentTime = DateTime.Now;

                    double cpuUsage = (currentTotalProcessorTime - previousTotalProcessorTime).TotalMilliseconds /
                                      (currentTime - previousTime).TotalMilliseconds /
                                      Environment.ProcessorCount * 100;

                    // Update the previous values
                    previousTotalProcessorTime = currentTotalProcessorTime;
                    previousTime = currentTime;

                    // Display the CPU and memory usage
                    Console.WriteLine($"Process: {processName}");
                    Console.WriteLine($"CPU Usage: {cpuUsage:F2}%");
                    Console.WriteLine($"Memory Usage: {memoryUsageMB:F2} MB");
                    Console.WriteLine("----------------------------");

                    processInfo.processId = process.Id;
                    processInfo.processName = processName;
                    processInfo.cpuUsage = $"{cpuUsage:F2} %";
                    processInfo.memoryUsageMB = $"{memoryUsageMB:F2} MB";
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
            return processInfo;
        }

        public static ProcessInfo Get_CPU_Memory()
        {
            ProcessInfo processInfo = new ProcessInfo();
            try
            {
                // Get the current
                Process process = Process.GetCurrentProcess();
                // Initialize variables to calculate CPU usage
                TimeSpan previousTotalProcessorTime = process.TotalProcessorTime;
                DateTime previousTime = DateTime.Now;
                {
                    // Get the current process memory usage
                    float memoryUsageMB = process.WorkingSet64 / (1024 * 1024);
                    string processName = process.ProcessName;
                    // Calculate CPU usage
                    TimeSpan currentTotalProcessorTime = process.TotalProcessorTime;
                    DateTime currentTime = DateTime.Now;

                    double cpuUsage = (currentTotalProcessorTime - previousTotalProcessorTime).TotalMilliseconds /
                                      (currentTime - previousTime).TotalMilliseconds /
                                      Environment.ProcessorCount * 100;

                    // Update the previous values
                    previousTotalProcessorTime = currentTotalProcessorTime;
                    previousTime = currentTime;

                    // Display the CPU and memory usage
                    Console.WriteLine($"Process Name: {processName}");
                    Console.WriteLine($"CPU Usage: {cpuUsage:F2}%");
                    Console.WriteLine($"Memory Usage: {memoryUsageMB:F2} MB");
                    Console.WriteLine("----------------------------");

                    processInfo.processId = process.Id;
                    processInfo.processName = processName;
                    processInfo.cpuUsage = $"{cpuUsage:F2} %";
                    processInfo.memoryUsageMB = $"{memoryUsageMB:F2} MB";
                    //await Task.Delay(1000); // Wait for 1 second before the next update
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
            return processInfo;
        }


    }
    
}
