using AutoMapper;
using RoleBasedAccessAPI.DTOs;
using RoleBasedAccessAPI.Models;

namespace RoleBasedAccessAPI.Mappings
{
    public class AccountMappingProfile : Profile
    {
        public AccountMappingProfile()
        {
            CreateMap<Account, AdminAccountDto>();

            CreateMap<Account, UserAccountDto>()
                .ForMember(dest => dest.MaskedAccountNumber, opt =>
                    opt.MapFrom(src => MaskAccountNumber(src.AccountNumber)));
        }

        private string MaskAccountNumber(string accountNumber)
        {
            if (string.IsNullOrWhiteSpace(accountNumber) || accountNumber.Length <= 6)
                return new string('X', accountNumber?.Length ?? 4);

            return new string('X', 6) + accountNumber.Substring(6);
        }
    }
}
