using CardStorageService.Models.Dto;

namespace CardStorageService.Models.Response.Account
{
    public class DeleteAccountResponse: OperationResult
    {
        public int? Result { get; set; }
    }
}
