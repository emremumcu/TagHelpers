namespace TagHelpers.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using System.Security.Claims;

    public class HomeController : Controller
    {
        public IActionResult Index()
        {

            //await HttpContext.SignOutAsync();
            //var identity = new ClaimsIdentity();
            //identity.AddClaim(new Claim(ClaimTypes.Name, "demo"));
            //var principal = new ClaimsPrincipal(identity);
            //await HttpContext.SignInAsync(principal);





            ClaimsIdentity ci = new ClaimsIdentity("auto");
            ci.AddClaim(new Claim(ClaimTypes.Name, "demo"));



            ClaimsPrincipal cp = new ClaimsPrincipal(ci);




            HttpContext.User = cp;

            return View();
        }
    }
}
