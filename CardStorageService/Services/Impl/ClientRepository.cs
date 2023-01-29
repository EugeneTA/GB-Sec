using CardStorageService.Data;
using Microsoft.EntityFrameworkCore;

namespace CardStorageService.Services.Impl
{
    public class ClientRepository : IClientRepositoryService
    {
        #region Services
        private readonly ILogger<ClientRepository> _logger;
        private readonly CardStorageServiceDbContext _dbContext;

        #endregion

        #region Constructor
        
        public ClientRepository(
            ILogger<ClientRepository> logger,
            CardStorageServiceDbContext dbContext)
        {
            _logger = logger;
            _dbContext = dbContext;
        }

        #endregion

        #region Public methods

        public int Create(Client data)
        {
            if (data != null)
            {
                _dbContext.Clients.Add(data);
                var result = _dbContext.SaveChanges();
                if (result > 0)
                    return data.ClientId;
            }

            return 0;
        }

        public int Delete(int id)
        {
            if (id > 0)
            {
                var client = GetById(id);
                if (client != null)
                {
                    _dbContext.Clients.Remove(client);
                    var result = _dbContext.SaveChanges();
                    if (result > 0)
                        return result;
                }
            }

            return 0;
        }

        public IList<Client> GetAll()
        {
            return _dbContext.Clients.ToList();
        }

        public Client GetById(int id)
        {
            return _dbContext.Clients.FirstOrDefault<Client>(client => client.ClientId == id);
        }

        public int Update(Client data)
        {
            if (data != null)
            {
                var client = GetById(data.ClientId);
                if (client != null)
                {
                    client.Name = data.Name;
                    client.SecondName = data.SecondName;
                    client.Patronymic = data.Patronymic;
                    _dbContext.Clients.Update(client);
                }
                var result = _dbContext.SaveChanges();
                if (result > 0)
                    return result;
            }

            return 0;
        }

        #endregion
    }
}
