using AutoMapper;
using CardStorageService.Data;
using CardStorageService.Models.Dto;
using CardStorageService.Models.Requests.Account;
using CardStorageService.Models.Requests.Card;
using CardStorageService.Models.Requests.Client;
using CardStorageService.Services.Impl;

namespace CardStorageService.Mapper
{
    public class MapperProfile: Profile
    {
        public MapperProfile()
        {
            CreateMap<Account, AccountDto>();
            CreateMap<CreateAccountRequest, Account>();
            CreateMap<Client, ClientDto>();
            CreateMap<CreateClientRequest, Card>();
            CreateMap<UpdateClientRequest, Card>();
            CreateMap<Card, CardDto>();
            CreateMap<CreateCardRequest, Card>();
            CreateMap<UpdateCardRequest, Card>();
        }
    }
}
