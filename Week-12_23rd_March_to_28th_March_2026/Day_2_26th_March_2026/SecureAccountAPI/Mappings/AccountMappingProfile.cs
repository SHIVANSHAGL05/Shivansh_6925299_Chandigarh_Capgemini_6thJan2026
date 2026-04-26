using AutoMapper;
using SecureAccountAPI.DTOs;
using SecureAccountAPI.Models;

namespace SecureAccountAPI.Mappings
{
    public class AccountMappingProfile : Profile
    {
        public AccountMappingProfile()
        {
            CreateMap<Account, AccountDetailsDto>()
                .ForMember(dest => dest.MaskedAccountNumber, opt =>
                    opt.MapFrom(src => MaskAccountNumber(src.AccountNumber)));
        }

        private string MaskAccountNumber(string accountNumber)
        {
            if (string.IsNullOrWhiteSpace(accountNumber) || accountNumber.Length < 4)
                return "****";

            return new string('*', accountNumber.Length - 4) + accountNumber.Substring(accountNumber.Length - 4);
        }
    }
}
