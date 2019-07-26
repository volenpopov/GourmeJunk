using System.Collections.Generic;
using System.Threading.Tasks;
using GourmeJunk.Data.Common.Repositories;
using GourmeJunk.Data.Models;
using GourmeJunk.Models.ViewModels.Coupons;
using GourmeJunk.Services.Contracts;
using GourmeJunk.Services.Mapping;
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
    }
}
