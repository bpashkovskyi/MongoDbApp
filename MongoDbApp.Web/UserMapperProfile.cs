using AutoMapper;

using MongoDbApp.Web.Model;
using MongoDbApp.Web.ViewModels;

namespace MongoDbApp.Web;

public class UserMapperProfile : Profile
{
    public UserMapperProfile()
    {
        this.CreateMap<User, UserListViewModel>();
        this.CreateMap<User, UserDetailsViewModel>();

        this.CreateMap<UserCreateViewModel, User>();
        this.CreateMap<UserUpdateViewModel, User>();
    }
}