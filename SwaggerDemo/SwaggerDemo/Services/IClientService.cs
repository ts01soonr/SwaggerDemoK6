using SwaggerDemo.Model;

namespace SwaggerDemo.Services
{
    public interface IClientService
    {
        List<Client> GetAll();
        Client GetById(int id);
        Client Add(Client employee);
        bool Delete(int id);
    }
}
