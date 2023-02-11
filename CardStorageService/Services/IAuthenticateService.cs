using CardStorageService.Models.Dto;
using CardStorageService.Models.Requests.Authentication;

namespace CardStorageService.Services
{
    public interface IAuthenticateService
    {
        AuthenticationResponse Login(AuthenticationRequest authenticationRequest);

        public SessionDto GetSession(string sessionToken);
    }
}
