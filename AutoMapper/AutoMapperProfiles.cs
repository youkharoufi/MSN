using AutoMapper;
using MSN.Models;

namespace MSN.AutoMapper
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles() 
        {
            CreateMap<RegisterUser, ApplicationUser>();
        
        }
    }
}
