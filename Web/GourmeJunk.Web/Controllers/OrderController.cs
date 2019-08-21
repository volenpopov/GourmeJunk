using GourmeJunk.Services.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;

namespace GourmeJunk.Web.Controllers
{
    public class OrderController : Controller
    {
        private readonly IOrdersService ordersService;

        public OrderController(IOrdersService ordersService)
        {
            this.ordersService = ordersService;
        }

        public IActionResult Index()
        {
            return View();
        }

        [Authorize]
        public async Task<IActionResult> Confirm(string orderId)
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

            var orderConfirmViewModel = await this.ordersService.GetOrderConfirmViewModelAsync(orderId, userId);

            return View(orderConfirmViewModel);
        }

        [Authorize]
        public async Task<IActionResult> OrderHistory(int productPage = 1)
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

            var ordersHistoryViewModel = await this.ordersService.GetOrdersHistoryListViewModelAsync(userId, productPage);

            return View(ordersHistoryViewModel);
        }

        [Authorize]
        public async Task<IActionResult> GetOrderDetails(string id)
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

            var model = await this.ordersService.GetOrderConfirmViewModelAsync(id, userId);

            return PartialView("_IndividualOrderDetails", model);
        }

        [Authorize]
        public async Task<IActionResult> GetOrderStatus(string id)
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

            var orderStatus = await this.ordersService.GetOrderStatusAsync(id, userId);

            return PartialView("_OrderStatus", orderStatus);
        }
    }
}