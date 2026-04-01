using Microsoft.AspNetCore.Mvc;

namespace TrendifyV1.Controllers
{
    public class InfoController : Controller
    {
        public IActionResult About() => View();
        public IActionResult FAQ() => View();
        public IActionResult Contacts() => View();
        public IActionResult Shipping() => View();
        public IActionResult Returns() => View();
        public IActionResult Terms() => View();
        public IActionResult Rules() => View();
        public IActionResult Privacy() => View();
    }
}
