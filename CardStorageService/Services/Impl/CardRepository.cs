using CardStorageService.Data;
using System.Linq;

namespace CardStorageService.Services.Impl
{
    public class CardRepository : ICardRepositoryService
    {
        #region Services
        private readonly ILogger<CardRepository> _logger;
        private readonly CardStorageServiceDbContext _dbContext;

        #endregion

        #region Constructor

        public CardRepository(
            ILogger<CardRepository> logger,
            CardStorageServiceDbContext dbContext)
        {
            _logger = logger;
            _dbContext = dbContext;
        }

        #endregion

        #region Public methods

        public Guid Create(Card data)
        {
            if (data != null)
            {
                var client = _dbContext.Clients.FirstOrDefault<Client>(client => client.ClientId == data.ClientId);
                if (client == null) throw new Exception("Client not found");

                _dbContext.Cards.Add(data);
                var result = _dbContext.SaveChanges();
                if (result > 0)
                    return data.CardId;

            }

            return Guid.Empty;
        }

        public int Delete(Guid id)
        {
            if (id != Guid.Empty)
            {
                var card = GetById(id);
                if (card != null)
                {
                    _dbContext.Cards.Remove(card);
                    var result = _dbContext.SaveChanges();
                    if (result > 0)
                        return result;
                }
            }

            return 0;
        }

        public IList<Card> GetAll()
        {
            return _dbContext.Cards.ToList();
        }

        public Card GetById(Guid id)
        {
            return _dbContext.Cards.FirstOrDefault<Card>(card => card.CardId == id);
        }

        public IList<Card> GetByClientId(int id)
        {
            return _dbContext.Cards.Where(card => card.ClientId == id).ToList();
        }

        public int Update(Card data)
        {
            if (data != null)
            {
                var card = GetById(data.CardId);
                if (card != null)
                {
                    if (card.ClientId != data.ClientId)
                    {
                        var client = _dbContext.Clients.FirstOrDefault<Client>(client => client.ClientId == data.ClientId);
                        if (client == null) throw new Exception($"Not fount client with id: {data.ClientId}");
                        card.ClientId = client.ClientId;
                    }

                    card.Number = data.Number;
                    card.Name = data.Name;
                    card.CVV2 = data.CVV2;
                    card.ExpDate = data.ExpDate;

                    _dbContext.Cards.Update(card);
                    var result = _dbContext.SaveChanges();
                    if (result > 0)
                        return result;
                }
            }

            return 0;
        }

        #endregion
    }
}
