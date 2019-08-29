using GourmeJunk.Data.Models;
using GourmeJunk.Models.ViewModels.Users;
using GourmeJunk.Services.Common;
using GourmeJunk.Services.Contracts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace GourmeJunk.Services.Tests
{
    public class UsersServiceTests : BaseServiceTests
    {
        private IUsersService usersServiceMock
            => this.ServiceProvider.GetRequiredService<IUsersService>();

        private const string TEST_USER_FIRSTNAME = "John";
        private const string TEST_USER_LASTNAME = "Jones";
        private const string TEST_USER_EMAIL = "john@yahoo.com";
        private const string TEST_USER_ADDRESS = "Sofia, bul. Vitosha 100";
        private const int TEST_USER_CART_SINGLE_ITEM_QUANTITY = 5;

        private const string SECOND_TEST_USER_FIRSTNAME = "Pesho";
        private const string SECOND_TEST_USER_LASTNAME = "Petrov";
        private const string SECOND_TEST_USER_EMAIL = "pesho@abv.bg";
        private const string SECOND_TEST_USER_ADDRESS = "Varna, Random 15 str.";

        [Fact]
        public async Task GetAllUsersViewModels_ReturnsValidModels()
        {
            await this.AddTestingUsersToDb();

            var expectedCount = this.DbContext.Users.Count();
            var expectedFirstUser = await this.DbContext.Users.FirstAsync();
            var expectedSecondUser = await this.DbContext.Users.LastAsync();

            var actual = await this.usersServiceMock.GetAllUsersViewModels();

            Assert.IsType<UserViewModel[]>(actual);
            Assert.Equal(expectedCount, actual.Count());

            Assert.Collection(actual,
                elem1 =>
                {
                    Assert.Equal(expectedFirstUser.Id, elem1.Id);
                    Assert.Equal(expectedFirstUser.FirstName, elem1.FirstName);
                    Assert.Equal(expectedFirstUser.LastName, elem1.LastName);
                    Assert.Equal(expectedFirstUser.Email, elem1.Email);
                },
                elem2 =>
                {
                    Assert.Equal(expectedSecondUser.Id, elem2.Id);
                    Assert.Equal(expectedSecondUser.FirstName, elem2.FirstName);
                    Assert.Equal(expectedSecondUser.LastName, elem2.LastName);
                    Assert.Equal(expectedSecondUser.Email, elem2.Email);
                });
        }

        [Fact]
        public async Task GetAllUsersViewModels_ReturnsNoneWhenNoUsersExist()
        {
            var actual = await this.usersServiceMock.GetAllUsersViewModels();

            Assert.Empty(actual);
        }

        [Fact]
        public async Task LockUserAsync_SucessfullyLocksUser()
        {
            await this.AddTestingUsersToDb();

            var user = await this.DbContext.Users.FirstAsync();

            await this.usersServiceMock.LockUserAsync(user.Id);

            Assert.True(DateTime.UtcNow < user.LockoutEnd);
        }

        [Fact]
        public async Task LockUserAsync_ThrowsNullReferenceWhenUserNotFound()
        {
            var exception = await Assert.ThrowsAsync<NullReferenceException>(() =>
                this.usersServiceMock.LockUserAsync(string.Empty));

            Assert.Equal(string.Format(
                    ServicesDataConstants.NULL_REFERENCE_ID,
                    ServicesDataConstants.USER,
                    string.Empty), 
                exception.Message);        
        }

        [Fact]
        public async Task UnLockUserAsync_SucessfullyUnlocksUser()
        {
            await this.AddTestingUsersToDb();

            var user = await this.DbContext.Users.FirstAsync();

            await this.usersServiceMock.LockUserAsync(user.Id);

            await this.usersServiceMock.UnLockUserAsync(user.Id);

            Assert.True(DateTime.UtcNow > user.LockoutEnd);
        }

        [Fact]
        public async Task UnLockUserAsync_ThrowsNullReferenceWhenUserNotFound()
        {
            var exception = await Assert.ThrowsAsync<NullReferenceException>(() =>
                this.usersServiceMock.UnLockUserAsync(string.Empty));

            Assert.Equal(string.Format(
                    ServicesDataConstants.NULL_REFERENCE_ID,
                    ServicesDataConstants.USER,
                    string.Empty),
                exception.Message);
        }

        [Fact]
        public async Task GetUserIndividualItemsCount_ReturnsZeroWhenUserDoesntHaveInitializedCart()
        {
            await this.AddTestingUsersToDb();

            var user = await this.DbContext.Users.FirstAsync();

            var expected = 0;

            var actual = await this.usersServiceMock.GetUserIndividualItemsCount(user.Email);

            Assert.Equal(expected, actual);
        }

        [Fact]
        public async Task GetUserIndividualItemsCount_ReturnsZeroWhenThereIsInitializedEmptyCartForUser()
        {
            await this.AddTestingUsersWithEmptyShoppingCarts();

            var user = await this.DbContext.Users.FirstAsync();

            var expected = 0;

            var actual = await this.usersServiceMock.GetUserIndividualItemsCount(user.Email);

            Assert.Equal(expected, actual);
        }

        [Fact]
        public async Task GetUserIndividualItemsCount_ReturnsValidCountWhenThereIsCartWithItems()
        {
            await this.AddTestingUserWithShoppingCartsWithItems();

            var user = await this.DbContext.Users.FirstAsync();

            var expected = 1;

            var actual = await this.usersServiceMock.GetUserIndividualItemsCount(user.Email);

            Assert.Equal(expected, actual);
        }

        [Fact]
        public async Task GetUserIndividualItemsCount_ThrowsNullReferenceWhenUserNotFound()
        {
            var exception = await Assert.ThrowsAsync<NullReferenceException>(() =>
                this.usersServiceMock.GetUserIndividualItemsCount(string.Empty));

            Assert.Equal(string.Format(
                    ServicesDataConstants.NULL_REFERENCE_USER_EMAIL,
                    string.Empty),
                exception.Message);
        }

        [Fact]
        public async Task GetUserFirstLastNameByIdAsync_ReturnsValidName()
        {
            await this.AddTestingUsersToDb();

            var user = await this.DbContext.Users.FirstAsync();

            var expected = $"{user.FirstName} {user.LastName}";

            var actual = await this.usersServiceMock.GetUserFirstLastNameByIdAsync(user.Id);

            Assert.Equal(expected, actual);
        }

        [Fact]
        public async Task GetUserFirstLastNameByIdAsync_ThrowsNullReferenceWhenUserNotFound()
        {
            var exception = await Assert.ThrowsAsync<NullReferenceException>(() =>
                this.usersServiceMock.GetUserFirstLastNameByIdAsync(string.Empty));

            Assert.Equal(string.Format(
                    ServicesDataConstants.NULL_REFERENCE_ID,
                    ServicesDataConstants.USER,
                    string.Empty),
                exception.Message);
        }       

        [Fact]
        public async Task GetUserByIdAsync_ReturnsValidUser()
        {
            await this.AddTestingUsersToDb();

            var user = await this.DbContext.Users.FirstAsync();

            var actual = await this.usersServiceMock.GetUserByIdAsync(user.Id);

            Assert.Equal(user, actual);
        }

        [Fact]
        public async Task GetUserByIdAsync_ThrowsNullReferenceWhenUserNotFound()
        {
            var exception = await Assert.ThrowsAsync<NullReferenceException>(() =>
                this.usersServiceMock.GetUserByIdAsync(string.Empty));

            Assert.Equal(string.Format(
                    ServicesDataConstants.NULL_REFERENCE_ID,
                    ServicesDataConstants.USER,
                    string.Empty),
                exception.Message);
        }

        private async Task AddTestingUsersToDb()
        {
            await this.DbContext.Users.AddAsync(
                new GourmeJunkUser
                {
                    FirstName = TEST_USER_FIRSTNAME,
                    LastName = TEST_USER_LASTNAME,
                    Email = TEST_USER_EMAIL,
                    Address = TEST_USER_ADDRESS
                });

            await this.DbContext.Users.AddAsync(
                new GourmeJunkUser
                {
                    FirstName = SECOND_TEST_USER_FIRSTNAME,
                    LastName = SECOND_TEST_USER_LASTNAME,
                    Email = SECOND_TEST_USER_EMAIL,
                    Address = SECOND_TEST_USER_ADDRESS
                });

            await this.DbContext.SaveChangesAsync();
        }

        private async Task AddTestingUsersWithEmptyShoppingCarts()
        {
            await this.AddTestingUsersToDb();

            var firsUser = await this.DbContext.Users.FirstAsync();

            await this.DbContext.ShoppingCarts.AddAsync(
                new ShoppingCart
                {
                    UserId = firsUser.Id                    
                });            

            await this.DbContext.SaveChangesAsync();
        }

        private async Task AddTestingUserWithShoppingCartsWithItems()
        {
            await this.AddTestingUsersWithEmptyShoppingCarts();

            var firstCart = await this.DbContext.ShoppingCarts.FirstAsync();
            
            await this.DbContext.ShoppingCartMenuItems.AddAsync(
                new ShoppingCartMenuItems
                {
                    ShoppingCartId = firstCart.Id,
                    MenuItemId = null,
                    Count = TEST_USER_CART_SINGLE_ITEM_QUANTITY
                });

            await this.DbContext.SaveChangesAsync();
        }
    }
}
