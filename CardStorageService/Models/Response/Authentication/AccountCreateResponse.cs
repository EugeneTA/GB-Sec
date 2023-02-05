﻿namespace CardStorageService.Models.Response.Authentication
{
    public class AccountCreateResponse: OperationResult
    { 
        public string? EMail { get; set; }
        public string? Password { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? SecondName { get; set; }
    }
}
