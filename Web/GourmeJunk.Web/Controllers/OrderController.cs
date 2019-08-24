using GourmeJunk.Common;
using GourmeJunk.Models.ViewModels.Orders;
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

            var OrderFullInfoViewModel = await this.ordersService.GetOrderFullInfoViewModelAsync(orderId, userId);

            return View(OrderFullInfoViewModel);
        }

        [Authorize]
        public async Task<IActionResult> OrderHistory(int productPage = 1)
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

            var ordersHistoryViewModel = await this.ordersService.GetOrdersListViewModelAsync(productPage, userId);

            return View(ordersHistoryViewModel);
        }

        [Authorize]
        public async Task<IActionResult> GetOrderDetails(string id)
        {            
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

            var model = new OrderFullInfoViewModel();

            if (User.IsInRole(GlobalConstants.ADMINISTRATOR_ROLE_NAME) || User.IsInRole(GlobalConstants.RECEPTION_ROLE_NAME))
            {
                model = await this.ordersService.GetOrderFullInfoViewModelAsync(id, null);
            }
            else
            {
                model = await this.ordersService.GetOrderFullInfoViewModelAsync(id, userId);
            }        

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

        [Authorize(Roles = 
            GlobalConstants.KITCHEN_ROLE_NAME + "," 
            + GlobalConstants.RECEPTION_ROLE_NAME + ","
            + GlobalConstants.ADMINISTRATOR_ROLE_NAME)]
        public async Task<IActionResult> ManageOrders(int productPage = 1)
        {
            var manageOrdersListViewModel = await this.ordersService.GetManageOrdersListViewModelAsync(productPage);

            return View(manageOrdersListViewModel);
        }

        [Authorize(Roles = GlobalConstants.KITCHEN_ROLE_NAME + "," + GlobalConstants.ADMINISTRATOR_ROLE_NAME)]
        public async Task<IActionResult> OrderPrepare(string id)
        {
            await this.ordersService.UpdateOrderStatusToCookingAsync(id);

            return RedirectToAction(nameof(ManageOrders));
        }

        [Authorize(Roles = GlobalConstants.KITCHEN_ROLE_NAME + "," + GlobalConstants.ADMINISTRATOR_ROLE_NAME)]
        public async Task<IActionResult> OrderReady(string id)
        {
            await this.ordersService.UpdateOrderStatusToReadyAsync(id);

            return RedirectToAction(nameof(ManageOrders));
        }

        [Authorize(Roles = 
            GlobalConstants.KITCHEN_ROLE_NAME + "," + 
            GlobalConstants.ADMINISTRATOR_ROLE_NAME + "," +
            GlobalConstants.RECEPTION_ROLE_NAME)]
        public async Task<IActionResult> OrderCancel(string id)
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

            await this.ordersService.UpdateOrderStatusToCancelledAsync(id, userId);

            return RedirectToAction(nameof(ManageOrders));
        }

        [Authorize(Roles = GlobalConstants.RECEPTION_ROLE_NAME + "," + GlobalConstants.ADMINISTRATOR_ROLE_NAME)]
        public async Task<IActionResult> OrderPickup(string searchEmail, string searchPhone, string searchName, int productPage = 1)
        {            
            var ordersPickupViewModel = await this.ordersService.GetOrdersPickupListViewModelAsync(productPage, searchEmail, searchPhone, searchName);

            return View(ordersPickupViewModel);
        }

        [HttpPost]
        [Authorize(Roles = GlobalConstants.ADMINISTRATOR_ROLE_NAME + "," + GlobalConstants.RECEPTION_ROLE_NAME)]        
        [ActionName(nameof(OrderPickup))]
        public async Task<IActionResult> OrderPickupPost(string id)
        {
            await this.ordersService.UpdateOrderStatusToDeliveredAsync(id);            

            return RedirectToAction(nameof(OrderPickup));
        }
    }
}