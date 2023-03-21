namespace MongoDbApp
{
    using AutoMapper;

    using MongoDbApp.Model;
    using MongoDbApp.ViewModels;

    public class UserMapperProfile : Profile
    {
        public UserMapperProfile()
        {
            this.CreateMap<User, UserListViewModel>();
            this.CreateMap<User, UserDetailsViewModel>();
        }
    }
}
