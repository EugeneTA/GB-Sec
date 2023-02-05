using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace CardStorageService.Models.Dto
{
    public class CardDto
    {
        public Guid CardId { get; set; }
        public int ClientId { get; set; }
        public string? Number { get; set; }
        public string? Name { get; set; }
        public string? CVV2 { get; set; }
        public string? ExpDate { get; set; }
    }
}
