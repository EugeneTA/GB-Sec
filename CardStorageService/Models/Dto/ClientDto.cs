using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace CardStorageService.Models.Dto
{
    public class ClientDto
    {
        public int ClientId { get; set; }
        public string? SecondName { get; set; }
        public string? Name { get; set; }
        public string? Patronymic { get; set; }
    }
}
