using Microsoft.AspNetCore.Mvc;
using QuickSpace.Data;

namespace QuickSpace.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult About()
        {
            return View();
        }
        public IActionResult Contact()
        {
            return View();
        }
        public IActionResult Faq()
        {
            return View();
        }
        public IActionResult Privacy()
        {
            return View();
        }
    }
}
