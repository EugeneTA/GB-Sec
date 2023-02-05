using CardStorageService.Models.Dto;
using Microsoft.AspNetCore.Components.Authorization;

namespace CardStorageService.Models.Requests.Authentication
{
    public class AuthenticationResponse
    {
        public AuthenticationStatus Status { get; set; }

        public SessionDto Session { get; set; }
    }
}
