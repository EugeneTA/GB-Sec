namespace CardStorageService.Models.Requests.Client
{
    public class UpdateClientRequest
    {
        public int Id { get; set; }
        public string? SecondName { get; set; }
        public string? Name { get; set; }
        public string? Patronymic { get; set; }
    }
}
