using SwaggerDemo.Model;

namespace SwaggerDemo.Services
{
    public interface IRemoteService
    {
        List<Remote> GetAll();
        Remote GetById(int id);
        Remote Add(Remote remote);
        bool Delete(int id);
    }
}
