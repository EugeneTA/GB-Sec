using CardStorageService.Data;

namespace CardStorageService.Services
{
    public interface ICardRepositoryService: IRepository<Card, Guid>
    {
        public IList<Card> GetByClientId(int id);
    }
}
