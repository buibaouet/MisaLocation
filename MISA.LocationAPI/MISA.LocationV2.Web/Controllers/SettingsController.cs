using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace MISA.LocationV2.Web.Controllers
{
    /// <summary>
    /// Class SettingsController hiển thị, quản lý các màn hình của mục Settings
    /// </summary>
    /// Created by nmthang - 18/05/2020
    [Authorize]
    public class SettingsController : Controller
    {

        public SettingsController()
        {

        }

        /// <summary>
        /// Phương thức Index trả về View của trang Settings
        /// </summary>
        /// <returns>View của trang Settings</returns>
        /// <remarks></remarks>
        /// Created by nmthang - 18/05/2020
        public IActionResult Index()
        {
            return View();
        }

    }
}