using SwaggerDemo.Model;
using System.Diagnostics;
using System.Net.Sockets;
using System.Text;

namespace SwaggerDemo.Services
{
    public class RemoteService : IRemoteService
    {   

        private static List<Remote> _remotes = new List<Remote>();
        private static int _id = 0;
        public RemoteService() 
        {
            Init();
        }
        private void Init()
        {
            if (_remotes.Count == 0)
            {
                _remotes = new List<Remote>() {
                    new Remote() {
                        Id = _id++,
                        Host = "localhost",
                        Port = 8888,
                        Command = "set ?",
                        Result = "S= : T=3 : D=20 OK",
                        Usage = "10 ms"
                    },
                    new Remote() {
                        Id = _id++,
                        Host = "localhost",
                        Port = 8888,
                        Command = "echo %T%",
                        Result = "echo 3 OK",
                        Usage = "20 ms"
                    },
                };

            }

        }
        public Remote Add(Remote remote)
        {
            if (remote.Id == 0) remote.Id = _id;
            _id++;
            _remotes.Add(Execute(remote));
            Console.WriteLine(_remotes.Count);
            return remote;
        }

        public bool Delete(int id)
        {
            if (id == -1 || id == -2) {
                if(id == -2) GC.Collect(); //Invoke GC
                _remotes.Clear();
                return _remotes.Count == 0;
            }
            var existing = _remotes.Find(x => x.Id == id);
            if (existing != null)
            {
                _remotes.Remove(existing);
                return true;
            }
            else
                return false;
        }

        public List<Remote> GetAll()
        {
            return _remotes;
        }

        public Remote GetById(int id)
        {
            return _remotes.Where(x => x.Id == id).FirstOrDefault();
        }
        private Remote Execute(Remote remote)
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            try
            {
                TcpClient socket = new TcpClient(remote.Host, remote.Port);
                Console.WriteLine($"Connected to {remote.Host}:{remote.Port} and execute {remote.Command}");
                // Get Welcome message
                NetworkStream stream = socket.GetStream();
                byte[] buffer = new byte[1024];
                int bytesRead = stream.Read(buffer, 0, buffer.Length);
                string receivedMessage = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                // Send Command
                string randomID = "id_" + new Random().Next();
                string result = "";
                byte[] messageBytes = Encoding.UTF8.GetBytes($"{remote.Command}\necho {randomID}\n");
                stream.Write(messageBytes, 0, messageBytes.Length);
                do
                {
                    bytesRead = stream.Read(buffer, 0, buffer.Length);
                    receivedMessage = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                    result += receivedMessage;
                } while (!receivedMessage.Contains(randomID));
                result = result.Replace($"echo {randomID} OK", "").Trim();
                remote.Result = result;
                socket.Close();
                //return result;
            }
            catch (Exception e)
            {
                remote.Result = e.Message;
                //return e.Message;
            }
            stopwatch.Stop();
            remote.Usage = (int) stopwatch.Elapsed.TotalMilliseconds+" ms";
            return remote;
        }
    }
}
