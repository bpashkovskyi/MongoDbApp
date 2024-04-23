using MongoDbApp.Model;

namespace MongoDbApp.Controllers;

using AutoMapper;

using Microsoft.AspNetCore.Mvc;

using MongoDB.Bson;

using Repositories;
using ViewModels;

[Route("api/[controller]")]
[ApiController]
public class UserController : ControllerBase
{
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;

    public UserController(IUserRepository userRepository, IMapper mapper)
    {
        this._userRepository = userRepository;
        this._mapper = mapper;
    }

    [HttpGet]
    public async Task<List<UserListViewModel>> Get()
    {
        List<User> users = await this._userRepository.GetAllUsersAsync();
        List<UserListViewModel> usersListViewModels = this._mapper.Map<List<UserListViewModel>>(users);

        return usersListViewModels;
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> Get(string id)
    {
        ObjectId objectId = new ObjectId(id);

        User user = await this._userRepository.GetUserAsync(objectId);
        if (user == null)
        {
            return this.NotFound("User not found");
        }

        UserDetailsViewModel userDetailsViewModel = this._mapper.Map<UserDetailsViewModel>(user);

        return this.Ok(userDetailsViewModel);
    }

    [HttpPost]
    public async Task<IActionResult> Post(UserCreateViewModel userCreateViewModel)
    {
        User user = this._mapper.Map<User>(userCreateViewModel);
        await this._userRepository.AddUserAsync(user);

        UserDetailsViewModel userDetailsViewModel = this._mapper.Map<UserDetailsViewModel>(user);

        return this.Ok(userDetailsViewModel);
    }

    [HttpPut]
    public async Task<IActionResult> Put(UserUpdateViewModel userUpdateViewModel)
    {
        User user = this._mapper.Map<User>(userUpdateViewModel);
        await this._userRepository.UpdateUserAsync(user);

        UserDetailsViewModel userDetailsViewModel = this._mapper.Map<UserDetailsViewModel>(user);

        return this.Ok(userDetailsViewModel);
    }

    [HttpPatch("name")]
    public async Task<IActionResult> UpdateName([FromBody] UserUpdateNameViewModel userUpdateNameViewModel)
    {
        await this._userRepository.UpdateUserNameAsync(new ObjectId(userUpdateNameViewModel.Id), userUpdateNameViewModel.Name);

        return this.Ok();
    }

    [HttpPatch("role")]
    public async Task<IActionResult> UpdateRole([FromBody] UserUpdateRoleViewModel userUpdateRoleViewModel)
    {
        await this._userRepository.UpdateUserRoleAsync(new ObjectId(userUpdateRoleViewModel.Id), userUpdateRoleViewModel.Role);

        return this.Ok();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id)
    {
        await this._userRepository.DeleteUserAsync(new ObjectId(id));

        return this.NoContent();
    }
}