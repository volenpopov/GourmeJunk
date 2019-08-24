using GourmeJunk.Models.InputModels.Orders;
using GourmeJunk.Models.ViewModels.Orders;
using GourmeJunk.Services.Contracts;
using GourmeJunk.Web.Common;
using GourmeJunk.Web.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Stripe;
using System.Security.Claims;
using System.Threading.Tasks;

namespace GourmeJunk.Web.Controllers
{
    public class ShoppingCartController : Controller
    {
        private readonly IShoppingCartService shoppingCartService;
        private readonly IOrdersService ordersService;
        private readonly IUsersService usersService;

        public ShoppingCartController(
            IShoppingCartService shoppingCartService,
            IOrdersService ordersService,
            IUsersService usersService)
        {
            this.shoppingCartService = shoppingCartService;
            this.ordersService = ordersService;
            this.usersService = usersService;
        }
        
        [Authorize]
        public async Task<IActionResult> Index()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;

            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

            var shoppingCartOrderViewModel = await this.shoppingCartService.GetShoppingCartViewModelAsync(userId);

            var couponCode = HttpContext.Session.GetString(WebConstants.SESSION_NAME_COUPON_CODE);

            if (!string.IsNullOrWhiteSpace(couponCode))
            {
                shoppingCartOrderViewModel.CouponName = couponCode;

                shoppingCartOrderViewModel = await this.shoppingCartService.ApplyCouponToCartAsync(shoppingCartOrderViewModel);
            }

            return View(shoppingCartOrderViewModel);
        }

        [Authorize]
        public IActionResult AddCoupon(string couponName)
        {
            if (!string.IsNullOrWhiteSpace(couponName))
            {
                HttpContext.Session.SetString(WebConstants.SESSION_NAME_COUPON_CODE, couponName);
            }

            return RedirectToAction(nameof(Index));
        }

        [Authorize]
        public IActionResult RemoveCoupon()
        {
            HttpContext.Session.SetString(WebConstants.SESSION_NAME_COUPON_CODE, string.Empty);
            
            return RedirectToAction(nameof(Index));
        }

        [Authorize]
        public async Task<IActionResult> IncrementItemQuantity(string menuItemId)
        {
            var userId = GetUserId();

            await this.shoppingCartService.IncrementItemQuantityAsync(userId, menuItemId);            

            return RedirectToAction(nameof(Index));
        }

        [Authorize]
        public async Task<IActionResult> DecrementItemQuantity(string menuItemId)
        {
            var userId = GetUserId();

            var itemIsToBeRemoved = await this.shoppingCartService.CheckIfItemIsLastAndIsToBeRemovedAsync(userId, menuItemId);

            if (itemIsToBeRemoved)
            {
                return RedirectToAction(nameof(RemoveItem), new { menuItemId });
            }
            else
            {
                await this.shoppingCartService.DecrementItemQuantityAsync(userId, menuItemId);
            }
                      
            return RedirectToAction(nameof(Index));
        }

        [Authorize]
        public async Task<IActionResult> RemoveItem(string menuItemId)
        {
            var userId = GetUserId();

            await this.shoppingCartService.RemoveItemAsync(userId, menuItemId);

            var userIndividualItemsCount = await this.shoppingCartService.GetUserIndividualItemsCountAsync(userId);

            HttpContext.Session.SetInt32(WebConstants.SESSION_NAME_SHOPPING_CART_INDIVIDUAL_ITEMS_COUNT, userIndividualItemsCount);

            return RedirectToAction(nameof(Index));
        }
        
        [HttpPost]
        [Authorize]        
        public async Task<IActionResult> Index(OrderSummaryInputModel model)
        {
            var formData = HttpContext.Request.Form;

            var itemsIds = formData[WebConstants.OrderItem.ITEM_ID_PROPERTY];
            var itemsNames = formData[WebConstants.OrderItem.ITEM_NAME_PROPERTY];
            var itemsPrices = formData[WebConstants.OrderItem.ITEM_PRICE_PROPERTY];
            var itemsCount = formData[WebConstants.OrderItem.ITEM_COUNT_PROPERTY];

            var pickupName = await this.usersService.GetUserFirstLastNameByIdAsync(model.UserId);

            var orderSummaryViewModel = this.ordersService.GetOrderSummaryViewModel(model, itemsIds, itemsNames, itemsPrices, itemsCount, pickupName);

            TempData.Put(nameof(OrderSummaryViewModel), orderSummaryViewModel);

            return RedirectToAction(nameof(OrderSummary), orderSummaryViewModel.OrderItems);
        }
       
        [Authorize]
        public IActionResult OrderSummary()
        {
            var orderSummaryViewModel = TempData.Get<OrderSummaryViewModel>(nameof(OrderSummaryViewModel));
            TempData.Remove(nameof(OrderSummaryViewModel));

            HttpContext.Session.SetInt32(WebConstants.SESSION_NAME_SHOPPING_CART_INDIVIDUAL_ITEMS_COUNT, WebConstants.SESSION_SHOPPING_CART_INITIAL_INDIVIDUAL_ITEMS_COUNT);

            return View(orderSummaryViewModel);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> OrderSummary(OrderInputModel model, string stripeEmail, string stripeToken)
        {                       
            if (!ModelState.IsValid)
            {
                return RedirectToAction("Error", "Home");
            }

            var formData = HttpContext.Request.Form;

            var itemsIds = formData[WebConstants.OrderItem.ITEM_ID_PROPERTY];            
            var itemsCount = formData[WebConstants.OrderItem.ITEM_COUNT_PROPERTY];
            
            var orderId = await this.ordersService.CreateOrderAsync(model, itemsIds, itemsCount, stripeEmail, stripeToken);

            return RedirectToAction("Confirm", "Order", new { orderId });
        }

        private string GetUserId()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;

            return claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;
        }
    }
}