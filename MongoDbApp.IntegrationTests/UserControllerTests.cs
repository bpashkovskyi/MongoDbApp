using AutoMapper;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;

using MongoDB.Bson;
using MongoDB.Driver;

using MongoDbApp.Web;
using MongoDbApp.Web.Controllers;
using MongoDbApp.Web.Model;
using MongoDbApp.Web.Repositories;
using MongoDbApp.Web.ViewModels;

namespace MongoDbApp.IntegrationTests
{
    [TestClass]
    public class UserControllerTests : ControllerTestsBase
    {
        private readonly UserController userController;

        public UserControllerTests()
        {
            var mongoClient = new MongoClient("mongodb://localhost:27017");
            mongoClient.DropDatabase("TestUserDatabase");

            var mongoDatabase = mongoClient.GetDatabase("TestUserDatabase");
            var userCollection = mongoDatabase.GetCollection<User>("Users");

            var userRepository = new UserRepository(userCollection, new MemoryDistributedCache(new OptionsWrapper<MemoryDistributedCacheOptions>(new MemoryDistributedCacheOptions())));

            var mapperConfiguration = new MapperConfiguration(mapperConfigurationExpression =>
            {
                mapperConfigurationExpression.AddProfile<UserMapperProfile>();
            });

            var mapper = mapperConfiguration.CreateMapper();

            this.userController = new UserController(userRepository, mapper);
        }

        [TestMethod]
        public async Task GetAllUserShouldSucceed()
        {
            // Arrange
            var userCreateViewModel = new UserCreateViewModel
            {
                Email = "list test email",
                Name = "list test name",
                Role = "student"
            };

            GetOkResultModel<UserDetailsViewModel>(await this.userController.Post(userCreateViewModel));

            // Act
            var userListViewModels = GetOkResultModel<List<UserListViewModel>>(await this.userController.Get());
            var userListViewModel = userListViewModels!.FirstOrDefault(u => u.Name == "list test name");


            // Assert
            Assert.IsNotNull(userListViewModel);
        }


        [TestMethod]
        public async Task GetUserShouldSucceedIfUserWasFound()
        {
            // Arrange
            var userCreateViewModel = new UserCreateViewModel
            {
                Email = "test email",
                Name = "test name",
                Role = "student"
            };

            var userDetailsModel = GetOkResultModel<UserDetailsViewModel>(await this.userController.Post(userCreateViewModel));

            var userId = userDetailsModel!.Id;

            // Act
            var userDetailsModel2 = GetOkResultModel<UserDetailsViewModel>(await this.userController.Get(userId.ToString()));


            // Assert
            Assert.IsNotNull(userDetailsModel2);
        }

        [TestMethod]
        public async Task GetUserShouldFailIfUserWasNotFound()
        {
            // Act
            var actionResult2 = await this.userController.Get(new ObjectId().ToString());

            var notFoundResult = actionResult2 as NotFoundObjectResult;
            var errorMessage = notFoundResult!.Value as string;

            // Assert
            Assert.AreEqual("User not found", errorMessage);
        }

        [TestMethod]
        public async Task PostUserShouldSucceed()
        {
            // Arrange
            var userCreateViewModel = new UserCreateViewModel
            {
                Email = "test email",
                Name = "test name",
                Role = "student"
            };

            var userDetailsModel = GetOkResultModel<UserDetailsViewModel>(await this.userController.Post(userCreateViewModel));

            var userId = userDetailsModel!.Id;

            // Act
            var userDetailsModel2 = GetOkResultModel<UserDetailsViewModel>(await this.userController.Get(userId.ToString()));


            // Assert
            Assert.IsNotNull(userDetailsModel2);
        }

        [TestMethod]
        public async Task PutUserShouldSucceed()
        {
            // Arrange
            var userCreateViewModel = new UserCreateViewModel
            {
                Email = "test email",
                Name = "test name",
                Role = "student"
            };

            var userDetailsModel = GetOkResultModel<UserDetailsViewModel>(await this.userController.Post(userCreateViewModel));

            var userId = userDetailsModel!.Id;

            var userUpdateViewModel = new UserUpdateViewModel
            {
                Id = userId.ToString(),
                Email = "test email updated",
                Name = "test name",
                Role = "student"
            };

            // Act

            await this.userController.Put(userUpdateViewModel);

            var userDetailsModel2 = GetOkResultModel<UserDetailsViewModel>(await this.userController.Get(userId.ToString()));


            // Assert
            Assert.AreEqual("test email updated", userDetailsModel2.Email);
        }

        [TestMethod]
        public async Task UpdateNameShouldSucceed()
        {
            // Arrange
            var userCreateViewModel = new UserCreateViewModel
            {
                Email = "test email",
                Name = "test name",
                Role = "student"
            };

            var userDetailsModel = GetOkResultModel<UserDetailsViewModel>(await this.userController.Post(userCreateViewModel));

            var userId = userDetailsModel!.Id;

            var updateNameViewModel = new UserUpdateNameViewModel
            {
                Id = userId.ToString(),
                Name = "test name updated",
            };

            // Act
            await this.userController.UpdateName(updateNameViewModel);

            var userDetailsModel2 = GetOkResultModel<UserDetailsViewModel>(await this.userController.Get(userId.ToString()));


            // Assert
            Assert.AreEqual("test name updated", userDetailsModel2!.Name);
        }

        [TestMethod]
        public async Task UpdateRoleShouldSucceed()
        {
            // Arrange
            var userCreateViewModel = new UserCreateViewModel
            {
                Email = "test email",
                Name = "test name",
                Role = "student"
            };

            var userDetailsModel = GetOkResultModel<UserDetailsViewModel>(await this.userController.Post(userCreateViewModel));

            var userId = userDetailsModel!.Id;

            var updateRoleViewModel = new UserUpdateRoleViewModel
            {
                Id = userId.ToString(),
                Role = "student updated",
            };

            // Act
            await this.userController.UpdateRole(updateRoleViewModel);

            var userDetailsModel2 = GetOkResultModel<UserDetailsViewModel>(await this.userController.Get(userId.ToString()));


            // Assert
            Assert.AreEqual("student updated", userDetailsModel2!.Role);
        }

        [TestMethod]
        public async Task DeleteShouldSucceed()
        {
            // Arrange
            var userCreateViewModel = new UserCreateViewModel
            {
                Email = "test email",
                Name = "test name",
                Role = "student"
            };

            var userDetailsModel = GetOkResultModel<UserDetailsViewModel>(await this.userController.Post(userCreateViewModel));

            var userId = userDetailsModel!.Id;

            // Act
            await this.userController.Delete(userId.ToString());

            var actionResult2 = await this.userController.Get(new ObjectId().ToString());

            var notFoundResult = actionResult2 as NotFoundObjectResult;
            var errorMessage = notFoundResult!.Value as string;

            // Assert
            Assert.AreEqual("User not found", errorMessage);
        }
    }
}