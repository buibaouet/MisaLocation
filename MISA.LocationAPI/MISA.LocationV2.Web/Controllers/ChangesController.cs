using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace MISA.LocationV2.Web.Controllers
{
    /// <summary>
    /// Class ChangesController quản lý các màn hình của mục Changes
    /// </summary>
    /// Created by nmthang - 18/05/2020
    [Authorize]
    public class ChangesController : Controller
    {
        public ChangesController()
        {

        }

        /// <summary>
        /// Phương thức Index trả về View của trang Changes
        /// </summary>
        /// <returns>View của trang Changes</returns>
        /// <remarks></remarks>
        /// Created by nmthang - 18/05/2020
        public IActionResult Index()
        {
            return View();
        }

    }
}