# GourmeJunk - demo restaurant for online food ordering
(Course project for my ASP.NET Core course at SoftUni)

## Tech Stack: ASP.NET Core MVC 2.2, EF Core, SQL Server, JavaScript, JQuery, Bootstrap

### Summary:

**Guest users** can browse the restaurant's menu by categories and also register/login. There is an option for the users to register, using their Facebook or Google account. Upon successfull registration the user will be notified by email.

**Registered users/Customers** can add, remove, or manage the quantity of items in their shopping cart. They can also place an order, use a discount coupon on it, checkout and pay (using Stripe API demo). Customers will get notified by email that their order has been placed or if in any case it gets cancelled (by the user or by the restaurant's staff). Customers can also track the status of their current order and view all their past orders.

**Restaurant Manager/Admin** The restaurant's manager is the only person that can add, remove or update menu items, categories, subcategories and coupons. The manager also can create new users and authorize them with Kitchen or Reception rights, or lock/unlock currently existing users.

**Cooks** can review the submitted orders and decide which ones to start cooking based on the order pickup time, items and description. They can mark orders as currently being cooked or ready, or even cancel an order.

**Reception** can review all orders that have been marked as ready by the cooks and search through them by user's name, phone number or email.  Upon pickup they can mark the order as delivered.


