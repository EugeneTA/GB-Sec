using CardStorageService.Models.Dto;
using CardStorageService.Models.Requests.Authentication;
using CardStorageService.Models.Response.Authentication;

namespace CardStorageService.Services
{
    public interface IAuthenticateService
    {
        AuthenticationResponse Login(AuthenticationRequest authenticationRequest);

        public SessionDto GetSession(string sessionToken);
    }
}
