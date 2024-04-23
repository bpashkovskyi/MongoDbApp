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
    public class UserControllerTests
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

            var userId = userDetailsModel.Id;

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
            var errorMessage = notFoundResult.Value as string;

            // Assert
            Assert.AreEqual("User not found", errorMessage);
        }

        protected T GetOkResultModel<T>(IActionResult actionResult)
            where T : class
        {

            Assert.IsInstanceOfType<OkObjectResult>(actionResult);
            
            var okObjectResult = (OkObjectResult)actionResult;

            Assert.IsInstanceOfType<T>(okObjectResult.Value);

            return okObjectResult.Value as T;
        }
    }
}