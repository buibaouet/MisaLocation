using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using MISA.LocationAPI.Models;
using MISA.LocationV2.UI.Properties;
using Newtonsoft.Json;

namespace MISA.LocationAPI.LocationAPIControllers
{
    /// <summary>
    /// Controller xử lý đăng nhập / đăng xuất của người dùng
    /// </summary>
    /// Created by nmthang - 09/06/2020
    [Route("[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        public readonly IConfiguration configuration;

        public UserController(IConfiguration _config)
        {
            configuration = _config;
        }
        /// <summary>
        /// Phương thức xử lý sau khi người dùng nhập thông tin đăng nhập và submit
        /// </summary>
        /// <param name="user">đói tượng chứa thông tin người dùng đã gửi lên</param>
        /// <returns>Màn hình trang chủ nếu đăng nhập thành công, màn hình login nếu thất bại</returns>
        /// Created by nmthang - 09/06/2020
        [Route("login")]
        [HttpPost]
        public IActionResult Login([FromForm] UserModel user)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(Resources.errorSendForm);
            }
            if(user.Username == null || user.Password == null)
            {
                return RedirectToAction("Index", "Login", new { message = Resources.enterForm});
            }

            var authCon = new AuthenticationController(configuration);

            var userToken = authCon.Login(user);

            if (!String.IsNullOrEmpty(userToken))
            {
                HttpContext.Session.SetString("JWToken", userToken);
                return RedirectToAction("Index", "Home");
            }

            return RedirectToAction("Index", "Login", new { message = Resources.errorPass});
        }

        /// <summary>
        /// Phương thức xử lý khi người dùng đăng xuất
        /// </summary>
        /// <returns>Màn hình trang đăng nhập</returns>
        /// Created by nmthang - 09/06/2020
        [Route("logout")]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Login");
        }
    }
}