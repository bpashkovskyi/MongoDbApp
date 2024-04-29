using AutoMapper;

using Microsoft.AspNetCore.Mvc;

using MongoDB.Bson;

using MongoDbApp.Web.Model;
using MongoDbApp.Web.Repositories;
using MongoDbApp.Web.ViewModels;

namespace MongoDbApp.Web.Controllers;

[Route("api/[controller]")]
[ApiController]
public class UserController : ControllerBase
{
    private readonly IUserRepository userRepository;
    private readonly IMapper mapper;

    public UserController(IUserRepository userRepository, IMapper mapper)
    {
        this.userRepository = userRepository;
        this.mapper = mapper;
    }

    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var users = await this.userRepository.GetAllUsersAsync();
        var usersListViewModels = this.mapper.Map<List<UserListViewModel>>(users);

        return this.Ok(usersListViewModels);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> Get(string id)
    {
        var objectId = new ObjectId(id);

        var user = await this.userRepository.GetUserAsync(objectId);
        if (user == null)
        {
            return this.NotFound("User not found");
        }

        var userDetailsViewModel = this.mapper.Map<UserDetailsViewModel>(user);

        return this.Ok(userDetailsViewModel);
    }

    [HttpPost]
    public async Task<IActionResult> Post(UserCreateViewModel userCreateViewModel)
    {
        var user = this.mapper.Map<User>(userCreateViewModel);
        await this.userRepository.AddUserAsync(user);

        var userDetailsViewModel = this.mapper.Map<UserDetailsViewModel>(user);

        return this.Ok(userDetailsViewModel);
    }

    [HttpPut]
    public async Task<IActionResult> Put(UserUpdateViewModel userUpdateViewModel)
    {
        var user = this.mapper.Map<User>(userUpdateViewModel);
        await this.userRepository.UpdateUserAsync(user);

        var userDetailsViewModel = this.mapper.Map<UserDetailsViewModel>(user);

        return this.Ok(userDetailsViewModel);
    }

    [HttpPatch("name")]
    public async Task<IActionResult> UpdateName([FromBody] UserUpdateNameViewModel userUpdateNameViewModel)
    {
        await this.userRepository.UpdateUserNameAsync(new ObjectId(userUpdateNameViewModel.Id), userUpdateNameViewModel.Name);

        return this.Ok();
    }

    [HttpPatch("role")]
    public async Task<IActionResult> UpdateRole([FromBody] UserUpdateRoleViewModel userUpdateRoleViewModel)
    {
        await this.userRepository.UpdateUserRoleAsync(new ObjectId(userUpdateRoleViewModel.Id), userUpdateRoleViewModel.Role);

        return this.Ok();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id)
    {
        await this.userRepository.DeleteUserAsync(new ObjectId(id));

        return this.NoContent();
    }
}