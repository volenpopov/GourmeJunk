﻿using Microsoft.AspNetCore.Mvc;

namespace GourmeJunk.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CategoryController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}