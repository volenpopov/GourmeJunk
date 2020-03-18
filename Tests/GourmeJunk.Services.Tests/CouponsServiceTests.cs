using GourmeJunk.Data.Models;
using GourmeJunk.Models.InputModels._AdminInputModels;
using GourmeJunk.Models.ViewModels.Coupons;
using GourmeJunk.Models.ViewModels.Home;
using GourmeJunk.Services.Common;
using GourmeJunk.Services.Contracts;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace GourmeJunk.Services.Tests
{
    public class CouponsServiceTests : BaseServiceTests
    {
        private ICouponsService couponsServiceMock
            => this.ServiceProvider.GetRequiredService<ICouponsService>();

        private const string TEST_COUPON_NAME = "BIGDEAL";
        private const CouponType TEST_COUPON_TYPE = CouponType.Percent;
        private const int TEST_COUPON_DISCOUNT = 10;
        private const decimal TEST_COUPON_MIN_ORDER_AMOUNT = 100m;        
        private const bool TEST_COUPON_ISACTIVE = true;

        private const string TEST_COUPON_IMAGE_PATH = @"images\TestCoupons\10%Off.JPG";
        private const string TEST_COUPON_IMAGE_CONTENT_TYPE = "image/jpg";

        private const string SECOND_TEST_COUPON_NAME = "3OFF";
        private const CouponType SECOND_TEST_COUPON_TYPE = CouponType.Amount;
        private const int SECOND_TEST_COUPON_DISCOUNT = 3;
        private const bool SECOND_TEST_COUPON_ISACTIVE = true;

        private const string THIRD_TEST_COUPON_NAME = "InactivePromo";
        private const CouponType THIRD_TEST_COUPON_TYPE = CouponType.Amount;
        private const int THIRD_TEST_COUPON_DISCOUNT = 100;
        private const bool THIRD_TEST_COUPON_ISACTIVE = false;

        private const string NEW_TEST_COUPON_NAME = "New";
        private const CouponType NEW_TEST_COUPON_TYPE = CouponType.Percent;
        private const int NEW_TEST_COUPON_DISCOUNT = 3;
        private const decimal NEW_TEST_COUPON_MIN_ORDER_AMOUNT = 50m;
        private const bool NEW_TEST_COUPON_ISACTIVE = true;

        [Fact]
        public async Task GetAllCouponsViewModelsAsync_ReturnsAllCouponsViewModels()
        {
            await this.AddTestingCouponsToDb();

            var coupons = await this.DbContext.Coupons.ToArrayAsync();

            var actual = await this.couponsServiceMock.GetAllCouponsViewModelsAsync();

            var expected = new CouponViewModel[]
            {
                new CouponViewModel
                {
                    Id = coupons[0].Id,
                    Name = TEST_COUPON_NAME,
                    CouponType = TEST_COUPON_TYPE,
                    Discount = TEST_COUPON_DISCOUNT,
                    MinimumOrderAmount = TEST_COUPON_MIN_ORDER_AMOUNT,
                    IsActive = TEST_COUPON_ISACTIVE
                },
                new CouponViewModel
                {
                    Id = coupons[1].Id,
                    Name = SECOND_TEST_COUPON_NAME,
                    CouponType = SECOND_TEST_COUPON_TYPE,
                    Discount = SECOND_TEST_COUPON_DISCOUNT,
                    IsActive = SECOND_TEST_COUPON_ISACTIVE
                },
                new CouponViewModel
                {
                    Id = coupons[2].Id,
                    Name = THIRD_TEST_COUPON_NAME,
                    CouponType = THIRD_TEST_COUPON_TYPE,
                    Discount = THIRD_TEST_COUPON_DISCOUNT,
                    IsActive = THIRD_TEST_COUPON_ISACTIVE
                }
            };

            Assert.IsType<CouponViewModel[]>(actual);
            Assert.Equal(expected.Length, actual.Count());

            Assert.Collection(actual,
                elem1 =>
                {
                    Assert.Equal(expected[0].Id, elem1.Id);
                    Assert.Equal(expected[0].Name, elem1.Name);
                    Assert.Equal(expected[0].CouponType, elem1.CouponType);
                    Assert.Equal(expected[0].Discount, elem1.Discount);
                    Assert.Equal(expected[0].MinimumOrderAmount, elem1.MinimumOrderAmount);
                    Assert.Equal(expected[0].IsActive, elem1.IsActive);
                },
                elem2 =>
                {
                    Assert.Equal(expected[1].Id, elem2.Id);
                    Assert.Equal(expected[1].Name, elem2.Name);
                    Assert.Equal(expected[1].CouponType, elem2.CouponType);
                    Assert.Equal(expected[1].Discount, elem2.Discount);
                    Assert.Equal(expected[1].MinimumOrderAmount, elem2.MinimumOrderAmount);
                    Assert.Equal(expected[1].IsActive, elem2.IsActive);
                },
                elem3 =>
                {
                    Assert.Equal(expected[2].Id, elem3.Id);
                    Assert.Equal(expected[2].Name, elem3.Name);
                    Assert.Equal(expected[2].CouponType, elem3.CouponType);
                    Assert.Equal(expected[2].Discount, elem3.Discount);
                    Assert.Equal(expected[2].MinimumOrderAmount, elem3.MinimumOrderAmount);
                    Assert.Equal(expected[2].IsActive, elem3.IsActive);
                });
        }

        [Fact]
        public async Task GetAllCouponsViewModelsAsync_ReturnsNone_WhenNoCouponsExist()
        {
            var actual = await this.couponsServiceMock.GetAllCouponsViewModelsAsync();

            Assert.Empty(actual);
        }

        [Fact]
        public async Task CheckIfCouponExistsAsync_ReturnsTrueWhenCouponExists()
        {
            await this.AddTestingCouponsToDb();

            var firstActual = await this.couponsServiceMock.CheckIfCouponExistsAsync(TEST_COUPON_NAME);
            var secondActual = await this.couponsServiceMock.CheckIfCouponExistsAsync(SECOND_TEST_COUPON_NAME);
            var thirdActual = await this.couponsServiceMock.CheckIfCouponExistsAsync(THIRD_TEST_COUPON_NAME);

            Assert.True(firstActual);
            Assert.True(secondActual);
            Assert.True(thirdActual);
        }

        [Fact]
        public async Task CheckIfCouponExistsAsync_ReturnsFalseWhenCouponDoesntExist()
        {
            await this.AddTestingCouponsToDb();

            var actual = await this.couponsServiceMock.CheckIfCouponExistsAsync(string.Empty);

            Assert.False(actual);
        }

        [Fact]
        public async Task CheckIfAnotherCouponWithTheSameNameExistsAsync_ReturnsTrueForExistingCoupon()
        {
            await this.AddTestingCouponsToDb();

            //Coupon - BIGDEAL
            var coupon = await this.DbContext.Coupons.FirstAsync();

            var newName = THIRD_TEST_COUPON_NAME;

            //Searching for a coupon with the newName, but with an Id different than the BIGDEAL coupon.
            var actual = await this.couponsServiceMock.CheckIfAnotherCouponWithTheSameNameExistsAsync(coupon.Id, newName);

            Assert.True(actual);
        }

        [Fact]
        public async Task CheckIfAnotherCouponWithTheSameNameExistsAsync_ReturnsFalseWhenNoOtherCouponExist()
        {
            await this.AddTestingCouponsToDb();

            //Coupon - BIGDEAL
            var coupon = await this.DbContext.Coupons.FirstAsync();

            var firstActual = await this.couponsServiceMock.CheckIfAnotherCouponWithTheSameNameExistsAsync(coupon.Id, string.Empty);
            var secondActual = await this.couponsServiceMock.CheckIfAnotherCouponWithTheSameNameExistsAsync(coupon.Id, coupon.Name);
            var thirdActual = await this.couponsServiceMock.CheckIfAnotherCouponWithTheSameNameExistsAsync(string.Empty, string.Empty);

            Assert.False(firstActual);
            Assert.False(secondActual);
            Assert.False(thirdActual);
        }

        [Fact]
        public async Task CreateCouponAsync_SuccessfullyAddsNewlyCreatedCouponToDb()
        {
            var couponInputModel = new CouponCreateInputModel
            {
                Name = TEST_COUPON_NAME,
                CouponType = TEST_COUPON_TYPE,
                Discount = TEST_COUPON_DISCOUNT,
                MinimumOrderAmount = TEST_COUPON_MIN_ORDER_AMOUNT,
                IsActive = TEST_COUPON_ISACTIVE
            };

            await this.couponsServiceMock.CreateCouponAsync(couponInputModel, null);

            Assert.NotEmpty(this.DbContext.Coupons);
        }

        [Fact]
        public async Task CreateCouponAsync_SuccessfullyCreatesNonExistingCouponWithoutImage()
        {
            var couponInputModel = new CouponCreateInputModel
            {
                Name = TEST_COUPON_NAME,
                CouponType = TEST_COUPON_TYPE,
                Discount = TEST_COUPON_DISCOUNT,
                MinimumOrderAmount = TEST_COUPON_MIN_ORDER_AMOUNT,
                IsActive = TEST_COUPON_ISACTIVE
            };

            await this.couponsServiceMock.CreateCouponAsync(couponInputModel, null);

            var actual = await this.DbContext.Coupons.FirstAsync();

            Assert.Equal(TEST_COUPON_NAME, actual.Name);
            Assert.Equal(TEST_COUPON_TYPE, actual.CouponType);
            Assert.Equal(TEST_COUPON_DISCOUNT, actual.Discount);
            Assert.Equal(TEST_COUPON_MIN_ORDER_AMOUNT, actual.MinimumOrderAmount);
            Assert.Equal(TEST_COUPON_ISACTIVE, actual.IsActive);
            Assert.Null(actual.Image);
        }

        [Fact]
        public async Task CreateCouponAsync_SuccessfullyCreatesNonExistingCouponWithImage()
        {
            var couponInputModel = new CouponCreateInputModel
            {
               Name = TEST_COUPON_NAME
            };

            IFormFile image;
            byte[] imageBytes = null;

            using (var fileStream = File.OpenRead(TEST_COUPON_IMAGE_PATH))
            {
                image = InitializeFormFile(fileStream);

                imageBytes = await GetImageBytes(imageBytes, fileStream);

                await this.couponsServiceMock.CreateCouponAsync(couponInputModel, image);
            }

            var actual = await this.DbContext.Coupons.FirstAsync();

            Assert.Equal(couponInputModel.Name, actual.Name);
            Assert.Equal(imageBytes, actual.Image);
        }

        [Fact]
        public async Task CreateCouponAsync_RestoresPreviouslyDeletedCoupon_WhenAnExistingDeletedCouponIsGivenAsInput()
        {
            await this.AddTestingCouponsToDb();
            var initialCouponsCount = 3;

            //3OFF
            var coupon = await this.DbContext.Coupons.Skip(1).Take(1).FirstAsync();

            //Mark 3OFF as deleted
            coupon.IsDeleted = true;
            coupon.DeletedOn = DateTime.UtcNow;

            //Provide new input with 3OFF name and all other props as new props and even add an image
            var couponInputModel = new CouponCreateInputModel
            {
                Name = SECOND_TEST_COUPON_NAME,
                CouponType = NEW_TEST_COUPON_TYPE,
                Discount = NEW_TEST_COUPON_DISCOUNT,
                MinimumOrderAmount = NEW_TEST_COUPON_MIN_ORDER_AMOUNT,
                IsActive = NEW_TEST_COUPON_ISACTIVE
            };

            IFormFile image;
            byte[] imageBytes = null;

            using (var fileStream = File.OpenRead(TEST_COUPON_IMAGE_PATH))
            {
                image = InitializeFormFile(fileStream);

                imageBytes = await GetImageBytes(imageBytes, fileStream);

                await this.couponsServiceMock.CreateCouponAsync(couponInputModel, image);
            }

            Assert.Equal(initialCouponsCount, this.DbContext.Coupons.Count());
            Assert.False(coupon.IsDeleted);
            Assert.Null(coupon.DeletedOn);
            Assert.Equal(couponInputModel.Name, coupon.Name);
            Assert.Equal(couponInputModel.CouponType, coupon.CouponType);
            Assert.Equal(couponInputModel.Discount, coupon.Discount);
            Assert.Equal(couponInputModel.MinimumOrderAmount, coupon.MinimumOrderAmount);
            Assert.Equal(couponInputModel.IsActive, coupon.IsActive);
            Assert.Equal(imageBytes, coupon.Image);
        }

        [Fact]
        public async Task GetCouponModelByIdAsync_ReturnValidCouponModel()
        {
            await this.AddTestingCouponsToDb();

            //BIGDEAL
            var coupon = await this.DbContext.Coupons.FirstAsync();

            var actualCouponViewMdodel = await this.couponsServiceMock.GetCouponModelByIdAsync<CouponViewModel>(coupon.Id);
            var actualCouponCreateViewMdodel = await this.couponsServiceMock.GetCouponModelByIdAsync<CouponCreateViewModel>(coupon.Id);
            var actualCouponViewMdodelExtended = await this.couponsServiceMock.GetCouponModelByIdAsync<CouponViewModelExtended>(coupon.Id);

            Assert.IsType<CouponViewModel>(actualCouponViewMdodel);
            Assert.Equal(coupon.Id, actualCouponViewMdodel.Id);
            Assert.Equal(coupon.Name, actualCouponViewMdodel.Name);
            Assert.Equal(coupon.Discount, actualCouponViewMdodel.Discount);
            Assert.Equal(coupon.MinimumOrderAmount, actualCouponViewMdodel.MinimumOrderAmount);
            Assert.Equal(coupon.IsActive, actualCouponViewMdodel.IsActive);

            Assert.IsType<CouponCreateViewModel>(actualCouponCreateViewMdodel);            
            Assert.Equal(coupon.Name, actualCouponCreateViewMdodel.Name);
            Assert.Equal(coupon.Discount, actualCouponCreateViewMdodel.Discount);
            Assert.Equal(coupon.MinimumOrderAmount, actualCouponCreateViewMdodel.MinimumOrderAmount);
            Assert.Equal(coupon.IsActive, actualCouponCreateViewMdodel.IsActive);

            Assert.IsType<CouponViewModelExtended>(actualCouponViewMdodelExtended);
            Assert.Equal(coupon.Id, actualCouponViewMdodel.Id);
            Assert.Equal(coupon.Name, actualCouponViewMdodel.Name);
            Assert.Equal(coupon.Discount, actualCouponViewMdodel.Discount);
            Assert.Equal(coupon.MinimumOrderAmount, actualCouponViewMdodel.MinimumOrderAmount);
            Assert.Equal(coupon.IsActive, actualCouponViewMdodel.IsActive);
            Assert.Equal(coupon.CouponType, actualCouponViewMdodel.CouponType);
        }

        [Fact]
        public async Task GetCouponModelByIdAsync_ThrowsNullReference_WhenCouponNotFound()
        {
            var exception = await Assert.ThrowsAsync<NullReferenceException>(() =>
                this.couponsServiceMock.GetCouponModelByIdAsync<CouponViewModel>(string.Empty));

            Assert.Equal(string.Format(ServicesDataConstants.NULL_REFERENCE_ID, nameof(Coupon), string.Empty), exception.Message);        
        }

        [Fact]
        public async Task EditCouponAsync_SuccessfullyEditsExistingCouponWithoutNewImage()
        {
            await this.AddTestingCouponsToDb();

            //BIGDEAL
            var coupon = await this.DbContext.Coupons.FirstAsync();

            var couponInputModel = new CouponEditInputModel
            {
                Id = coupon.Id,
                Name = NEW_TEST_COUPON_NAME,
                CouponType = NEW_TEST_COUPON_TYPE,
                Discount = NEW_TEST_COUPON_DISCOUNT,
                MinimumOrderAmount = NEW_TEST_COUPON_MIN_ORDER_AMOUNT,
                IsActive = NEW_TEST_COUPON_ISACTIVE
            };

            await this.couponsServiceMock.EditCouponAsync(couponInputModel, null);

            //Coupon image remains the same, because you need to select a new image, otherwise it the old image remains

            Assert.Equal(couponInputModel.Id, coupon.Id);
            Assert.Equal(couponInputModel.Name, coupon.Name);
            Assert.Equal(couponInputModel.CouponType, coupon.CouponType);
            Assert.Equal(couponInputModel.Discount, coupon.Discount);
            Assert.Equal(couponInputModel.MinimumOrderAmount, coupon.MinimumOrderAmount);
            Assert.Equal(couponInputModel.IsActive, coupon.IsActive);
        }

        [Fact]
        public async Task EditCouponAsync_SuccessfullyEditsCouponImage()
        {
            await this.AddTestingCouponsToDb();

            var coupon = await this.DbContext.Coupons.LastAsync();

            var couponInputModel = new CouponEditInputModel
            {
                Id = coupon.Id
            };

            IFormFile image;
            byte[] imageBytes = null;

            using (var fileStream = File.OpenRead(TEST_COUPON_IMAGE_PATH))
            {
                image = InitializeFormFile(fileStream);

                imageBytes = await GetImageBytes(imageBytes, fileStream);

                await this.couponsServiceMock.EditCouponAsync(couponInputModel, image);
            }

            Assert.Equal(imageBytes, coupon.Image);
        }

        [Fact]
        public async Task EditCouponAsync_DeleteсCurrentCouponAndUndeletesThePreviouslyDeletedCouponNowGivenAsInput()
        {
            await this.AddTestingCouponsToDb();

            //BIGDEAL
            var firstCoupon = await this.DbContext.Coupons.FirstAsync();

            //InactivePromo
            var lastCoupon = await this.DbContext.Coupons.LastAsync();
            lastCoupon.IsDeleted = true;
            lastCoupon.DeletedOn = DateTime.UtcNow;

            await this.DbContext.SaveChangesAsync();

            var couponInputModel = new CouponEditInputModel
            {
                Id = firstCoupon.Id,
                Name = lastCoupon.Name,
                CouponType = NEW_TEST_COUPON_TYPE,
                Discount = NEW_TEST_COUPON_DISCOUNT,
                IsActive = NEW_TEST_COUPON_ISACTIVE,
                MinimumOrderAmount = NEW_TEST_COUPON_MIN_ORDER_AMOUNT
            };

            await this.couponsServiceMock.EditCouponAsync(couponInputModel, null);

            Assert.True(firstCoupon.IsDeleted);
            Assert.NotNull(firstCoupon.DeletedOn);

            Assert.False(lastCoupon.IsDeleted);
            Assert.Null(lastCoupon.DeletedOn);

            Assert.Equal(couponInputModel.Name, lastCoupon.Name);
            Assert.Equal(couponInputModel.CouponType, lastCoupon.CouponType);
            Assert.Equal(couponInputModel.Discount, lastCoupon.Discount);
            Assert.Equal(couponInputModel.MinimumOrderAmount, lastCoupon.MinimumOrderAmount);
            Assert.Equal(couponInputModel.IsActive, lastCoupon.IsActive);
        }

        [Fact]
        public async Task DeleteCouponAsync_SucessfullyDeletesCoupon()
        {
            await this.AddTestingCouponsToDb();

            //BIGDEAL
            var coupon = await this.DbContext.Coupons.FirstAsync();

            Assert.False(coupon.IsDeleted);
            Assert.Null(coupon.DeletedOn);

            await this.couponsServiceMock.DeleteCouponAsync(coupon.Id);

            Assert.True(coupon.IsDeleted);
            Assert.NotNull(coupon.DeletedOn);
            Assert.Null(coupon.Image);
        }

        [Fact]
        public async Task DeleteCouponAsync_ThrowsNullReferenceWhenCouponNotFound()
        {
            var exception = await Assert.ThrowsAsync<NullReferenceException>(() =>
                this.couponsServiceMock.DeleteCouponAsync(string.Empty));

            Assert.Equal(string.Format(ServicesDataConstants.NULL_REFERENCE_ID, nameof(Coupon), string.Empty), exception.Message);
        }
        
        [Fact]
        public async Task GetAllActiveCouponsWithImageIndexCouponsModelsAsync_ReturnsValidCouponsModels()
        {
            await this.AddTestingCouponsToDb();

            var activeCouponsCount = this.DbContext.Coupons
                .Where(coupon => coupon.IsActive && coupon.Image != null)
                .ToArray()
                .Length;

            var actual = await this.couponsServiceMock.GetAllActiveCouponsWithImageIndexCouponsModelsAsync();

            var expectedCoupon = await this.DbContext.Coupons.FirstAsync();            

            Assert.IsType<IndexCouponViewModel[]>(actual);
            Assert.Equal(activeCouponsCount, actual.Count());

            Assert.Collection(actual,
                elem1 =>
                {
                    Assert.Equal(expectedCoupon.IsActive, elem1.IsActive);
                    Assert.Equal(expectedCoupon.Image, elem1.Image);
                });
        }

        [Fact]
        public async Task GetAllActiveCouponsWithImageIndexCouponsModelsAsync_ReturnsNoneWhenNoActiveCoupounsWithImageExist()
        {
            await this.AddTestingCouponsToDb();

            //Making BIGDEAL coupon inactive
            var firstCoupon = await this.DbContext.Coupons.FirstAsync();
            firstCoupon.IsActive = false;

            await this.DbContext.SaveChangesAsync();

            var actual = await this.couponsServiceMock.GetAllActiveCouponsWithImageIndexCouponsModelsAsync();

            Assert.Empty(actual);
        }

        private async Task AddTestingCouponsToDb()
        {
            byte[] testImageBytes = null;

            using (var fileStream = File.OpenRead(TEST_COUPON_IMAGE_PATH))
            {
                using (var memoryStream = new MemoryStream())
                {
                    await fileStream.CopyToAsync(memoryStream);

                    testImageBytes = memoryStream.ToArray();
                }
            }

            await this.DbContext.Coupons.AddAsync(
                new Coupon
                {
                    Name = TEST_COUPON_NAME,
                    CouponType = TEST_COUPON_TYPE,
                    Discount = TEST_COUPON_DISCOUNT,
                    MinimumOrderAmount = TEST_COUPON_MIN_ORDER_AMOUNT,
                    Image = testImageBytes,
                    IsActive = TEST_COUPON_ISACTIVE
                });

            await this.DbContext.Coupons.AddAsync(
                new Coupon
                {
                    Name = SECOND_TEST_COUPON_NAME,
                    CouponType = SECOND_TEST_COUPON_TYPE,
                    Discount = SECOND_TEST_COUPON_DISCOUNT,
                    IsActive = SECOND_TEST_COUPON_ISACTIVE
                });

            await this.DbContext.Coupons.AddAsync(
                new Coupon
                {
                    Name = THIRD_TEST_COUPON_NAME,
                    CouponType = THIRD_TEST_COUPON_TYPE,
                    Discount = THIRD_TEST_COUPON_DISCOUNT,
                    IsActive = THIRD_TEST_COUPON_ISACTIVE
                });

            await this.DbContext.SaveChangesAsync();
        }

        private static IFormFile InitializeFormFile(FileStream fileStream)
        {
            return new FormFile(fileStream, 0, fileStream.Length, null, Path.GetFileName(fileStream.Name))
            {
                Headers = new HeaderDictionary(),
                ContentType = TEST_COUPON_IMAGE_CONTENT_TYPE
            };
        }

        private static async Task<byte[]> GetImageBytes(byte[] imageBytes, FileStream fileStream)
        {
            using (var memoryStream = new MemoryStream())
            {
                await fileStream.CopyToAsync(memoryStream);

                imageBytes = memoryStream.ToArray();
            }

            return imageBytes;
        }
    }
}
