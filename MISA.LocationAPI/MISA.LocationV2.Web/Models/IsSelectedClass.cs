using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MISA.LocationAPI.Extensions
{
    /// <summary>
    /// Static class cung cấp một extension method cho IHtmlHelper
    /// Do phiên bản đang sử dụng của template INSPINIA không hoàn toàn tương thích với ASP.NET Core 3.1
    /// code tham khảo từ https://stackoverflow.com/questions/20410623/how-to-add-active-class-to-html-actionlink-in-asp-net-mvc
    /// </summary>
    public static class IsSelectedClass 
    {
        public static string IsSelected(this IHtmlHelper htmlHelper, string controller, string action, string cssClass = "selected")
        {
            string currentAction = htmlHelper.ViewContext.RouteData.Values["action"] as string;
            string currentController = htmlHelper.ViewContext.RouteData.Values["controller"] as string;

            IEnumerable<string> acceptedActions = (action ?? currentAction).Split(',');
            IEnumerable<string> acceptedControllers = (controller ?? currentController).Split(',');

            return acceptedActions.Contains(currentAction) && acceptedControllers.Contains(currentController) ?
                cssClass : String.Empty;
        }
    }
}
