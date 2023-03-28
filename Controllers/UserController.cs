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
        var users = await this._userRepository.GetAllUsersAsync();
        var usersListViewModels = this._mapper.Map<List<UserListViewModel>>(users);

        return usersListViewModels;
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> Get(string id)
    {
        var objectId = new ObjectId(id);

        var user = await this._userRepository.GetUserAsync(objectId);
        if (user == null)
        {
            return this.NotFound("User not found");
        }

        var userDetailsViewModel = this._mapper.Map<UserDetailsViewModel>(user);

        return this.Ok(userDetailsViewModel);
    }
}