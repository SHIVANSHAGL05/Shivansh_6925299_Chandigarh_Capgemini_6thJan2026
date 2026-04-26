using AutoMapper;
using MaskingAccountAPI.DTOs;
using MaskingAccountAPI.Models;

namespace MaskingAccountAPI.Mappings
{
    public class AccountMappingProfile : Profile
    {
        public AccountMappingProfile()
        {
            CreateMap<Account, MaskedAccountDto>()
                .ForMember(dest => dest.MaskedAccountNumber, opt =>
                    opt.MapFrom(src => MaskFirstSix(src.AccountNumber)));
        }

        private string MaskFirstSix(string accountNumber)
        {
            if (string.IsNullOrWhiteSpace(accountNumber))
                return "****";

            if (accountNumber.Length <= 6)
                return new string('X', accountNumber.Length);

            return new string('X', 6) + accountNumber.Substring(6);
        }
    }
}
