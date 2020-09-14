using AutoMapper;
using CoreApi.Common.Options;
using CoreApi.Contract.DatabaseContracts;
using CoreApi.Entity.Models;
using JetBrains.Annotations;

namespace CoreApi.Core.Configuration
{
    [UsedImplicitly]
    public class AutoMapperConfiguration : Profile
    {
        private readonly WallOptions _wallOptions;

        public AutoMapperConfiguration(WallOptions wallOptions)
        {
            _wallOptions = wallOptions;
            
            CreateMap<User, UserContract>()
                .ForMember(dest => dest.Password, opts => opts.Ignore());

            CreateMap<UserContract, User>();
            
            CreateMap<Note, NoteContract>().ReverseMap();
        }
    }
}
