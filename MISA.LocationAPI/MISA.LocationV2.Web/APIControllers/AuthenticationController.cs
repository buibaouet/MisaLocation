using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using MISA.LocationAPI.Models;

namespace MISA.LocationAPI.LocationAPIControllers
{
    /// <summary>
    /// Controller quản lý xác thực người dùng
    /// </summary>
    /// Created by nmthang - 09/06/2020
    [Route("[controller]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly IConfiguration _config;
        public AuthenticationController(IConfiguration config)
        {
            _config = config;
        }

        /// <summary>
        /// Phương thức xác thực người dùng, trả về token nếu hợp lệ
        /// </summary>
        /// <param name="loginUser">đói tượng chứa thông tin đăng nhập của người dùng</param>
        /// <returns>Một chuỗi token nếu thông tin đăng nhập hợp lệ, ngược lại trả về null</returns>
        /// Created by nmthang - 09/06/2020
        [HttpPost]
        [AllowAnonymous]
        [Route("login")]
        public string Login([FromBody]UserModel loginUser)
        {
            UserModel user = AuthenticateUser(loginUser);
            if (user != null)
            {
                var tokenString = GenerateJWTToken(user);
                return tokenString;
            }
            return null;
        }

        /// <summary>
        /// Phương thức private kiểm tra tài khoản người dùng
        /// </summary>
        /// <param name="loginCredentials">đối tượng chứa thông tin đăng nhập</param>
        /// <returns>Người dùng ứng với thông tin đăng nhập</returns>
        /// created by nmthang - 09/06/2020
        private UserModel AuthenticateUser(UserModel loginCredentials)
        {
            var username = _config["login:Username"];
            var password = _config["login:Password"];
            if (username == loginCredentials.Username && password == loginCredentials.Password)
            {
                return loginCredentials;
            }
            return null;
        }

        /// <summary>
        /// Phương thức sinh chuỗi token
        /// </summary>
        /// <param name="userInfo">Đối tượng chứa thông tin đăng nhập</param>
        /// <returns>Một chuỗi token</returns>
        /// Created by nmthang - 09/06/2020
        private string GenerateJWTToken(UserModel userInfo)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:SecretKey"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, userInfo.Username),
                new Claim(ClaimTypes.Name, userInfo.Username),
                new Claim("fullName", userInfo.Lastname + " " + userInfo.Firstname),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            };

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddMinutes(30),
                signingCredentials: credentials
            );
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}