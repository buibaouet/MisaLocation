using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace MISA.LocationV2.Web.Controllers
{
    public class SynchronizedController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
