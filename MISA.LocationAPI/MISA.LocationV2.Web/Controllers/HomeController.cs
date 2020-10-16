using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MISA.LocationAPI.Models;

namespace MISA.LocationAPI.Controllers
{
    /// <summary>
    /// Class HomeController quản lý các màn hình ở mục Home
    /// </summary>
    /// Created by nmthang - 18/05/2020
    
   [Authorize]
    public class HomeController : Controller
    {
        public HomeController()
        {
           
        }

        /// <summary>
        /// Phương thức Index trả về View của trang chủ
        /// </summary>
        /// <returns>View của trang Home</returns>
        /// <remarks></remarks>
        /// Created by nmthang - 18/05/2020
        public IActionResult Index()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
