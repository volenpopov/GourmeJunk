using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using GourmeJunk.Data.Common.Repositories;
using GourmeJunk.Data.Models;
using GourmeJunk.Models.InputModels._AdminInputModels;
using GourmeJunk.Models.ViewModels.Coupons;
using GourmeJunk.Models.ViewModels.Home;
using GourmeJunk.Services.Common;
using GourmeJunk.Services.Contracts;
using GourmeJunk.Services.Mapping;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace GourmeJunk.Services
{
    public class CouponsService : ICouponsService
    {
        private readonly IDeletableEntityRepository<Coupon> couponsRepository;

        public CouponsService(IDeletableEntityRepository<Coupon> couponsRepository)
        {
            this.couponsRepository = couponsRepository;
        }

        public async Task<IEnumerable<CouponViewModel>> GetAllAsync()
        {
            var couponsModels = await this.couponsRepository
                .AllAsNoTracking()
                .To<CouponViewModel>()
                .ToArrayAsync();

            return couponsModels;
        }

        public async Task<bool> CheckIfCouponExistsAsync(string couponName)
        {
            return await this.couponsRepository
                .AllAsNoTracking()
                .AnyAsync(coupon => coupon.Name == couponName);
        }

        public async Task<bool> CheckIfCouponExistsAsync(string couponId, string couponName)
        {
            return await this.couponsRepository
                .AllAsNoTracking()
                .AnyAsync(menuItem => menuItem.Name == couponName && menuItem.Id != couponId);
        }

        public async Task CreateCouponAsync(CouponCreateInputModel model, IFormFile image)
        {
            var coupon = await this.couponsRepository
                .AllWithDeleted()
                .SingleOrDefaultAsync(cpn => cpn.Name == model.Name);

            if (coupon == null)
            {
                coupon = new Coupon();

                await this.OverrideCouponProps(coupon, model, image);

                await this.couponsRepository.AddAsync(coupon);
            }
            else if (coupon.IsDeleted)
            {
                this.couponsRepository.Undelete(coupon);

                await this.OverrideCouponProps(coupon, model, image);
            }

            await this.couponsRepository.SaveChangesAsync();
        }

        public async Task<TViewModel> GetCouponModelByIdAsync<TViewModel>(string couponId)
        {
            var coupon = await this.couponsRepository
                .AllAsNoTracking()
                .Where(cpn => cpn.Id == couponId)
                .To<TViewModel>()
                .FirstOrDefaultAsync();

            if (coupon == null)
            {
                throw new NullReferenceException(string.Format(ServicesDataConstants.NULL_REFERENCE_ID, nameof(Coupon), couponId));
            }
           
            return coupon;
        }

        public async Task EditCouponAsync(CouponEditInputModel model, IFormFile image)
        {
            var currentCoupon = await this.GetCouponByIdAsync(model.Id);

            var newCouponAsExistingDeletedCoupon = await this.couponsRepository
                .AllWithDeleted()
                .SingleOrDefaultAsync(cpn => cpn.Name == model.Name);

            if (newCouponAsExistingDeletedCoupon != null && newCouponAsExistingDeletedCoupon.IsDeleted)
            {
                this.couponsRepository.Delete(currentCoupon);

                this.couponsRepository.Undelete(newCouponAsExistingDeletedCoupon);

                await this.OverrideCouponProps(newCouponAsExistingDeletedCoupon, model, image);
            }
            else
            {
                await this.OverrideCouponProps(currentCoupon, model, image);
            }

            await this.couponsRepository.SaveChangesAsync();
        }

        public async Task DeleteCouponAsync(string id)
        {
            var coupon = await this.GetCouponByIdAsync(id);

            coupon.Image = null;

            this.couponsRepository.Delete(coupon);

            await this.couponsRepository.SaveChangesAsync();
        }

        public async Task<IEnumerable<IndexCouponViewModel>> GetAllIndexCouponsModelsAsync()
        {
            return await this.couponsRepository
                .AllAsNoTracking()
                .Where(cpn => cpn.IsActive && cpn.Image.Length > 0)
                .To<IndexCouponViewModel>()
                .ToArrayAsync();
        }

        private async Task<Coupon> GetCouponByIdAsync(string couponId)
        {
            var coupon = await this.couponsRepository
                .All()
                .SingleOrDefaultAsync(cpn => cpn.Id == couponId);

            if (coupon == null)
            {
                throw new NullReferenceException(string.Format(ServicesDataConstants.NULL_REFERENCE_ID, nameof(Coupon), couponId));
            }

            return coupon;
        }

        private async Task OverrideCouponProps(Coupon coupon, CouponCreateInputModel model, IFormFile image)
        {
            coupon.Name = model.Name;
            coupon.CouponType = model.CouponType;
            coupon.Discount = model.Discount;
            coupon.MinimumOrderAmount = model.MinimumOrderAmount;
            coupon.IsActive = model.IsActive;

            if (image != null)
            {
                var extension = Path.GetExtension(image.FileName).ToLower();

                if (extension != ServicesDataConstants.JPG_EXTENSION && extension != ServicesDataConstants.PNG_EXTENSION)
                {
                    throw new ArgumentException(string.Format(ServicesDataConstants.INVALID_IMG_TYPE, extension));
                }

                coupon.Image = await this.GetImageBytes(image);
            }
        }

        private async Task<byte[]> GetImageBytes(IFormFile image)
        {
            byte[] imageBytes = null;

            using (var fileStream = image.OpenReadStream())
            {
                using (var memoryStream = new MemoryStream())
                {
                    await fileStream.CopyToAsync(memoryStream);

                    imageBytes = memoryStream.ToArray();
                }
            }

            return imageBytes;
        }
    }
}
