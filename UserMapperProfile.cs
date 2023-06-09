﻿namespace MongoDbApp;

using AutoMapper;

using Model;
using ViewModels;

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