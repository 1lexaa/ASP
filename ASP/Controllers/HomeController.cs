using ASP.DATA;
using ASP.Models;
using ASP.Services;
using ASP.Services.Hash;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace ASP.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly TimeService _time_service;
        private readonly DateService _date_service;
        private readonly DtService _dt_service;
        private readonly IHashService _hash_service;
        private readonly DataContext _data_context;
        private readonly IConfiguration _configuration;

        public HomeController(ILogger<HomeController> logger, TimeService time_service, DateService date_service,
                            DtService dt_service, IHashService hash_service, DataContext data_context, IConfiguration configuration)
        {
            _logger = logger;
            _time_service = time_service;
            _date_service = date_service;
            _dt_service = dt_service;
            _hash_service = hash_service;
            _data_context = data_context;
            _configuration = configuration;
        }

        public IActionResult Index() { return View(); }
        public IActionResult Intro() { return View(); }
        public IActionResult Privacy() { return View(); }
        public IActionResult Scheme()
        {
            ViewBag.bagdata = "Data from Bag";
            ViewData["viewdata"] = "Data from ViewData";

            return View();
        }
        public new IActionResult Url() { return View(); }
        public IActionResult Razor() { return View(); }
        public IActionResult Model()
        {
            Models.Home.Model model = new Models.Home.Model()
            {
                Header = "Модели",
                Title = "Передача модели в представлении.",
                Departments = new List<String>
                {
                    "DEPARTMENT 1",
                    "DEPARTMENT 2",
                    "DEPARTMENT 3",
                    "DEPARTMENT 4",
                    "DEPARTMENT 5",
                    "DEPARTMENT 6",
                    "DEPARTMENT 7"
                },
                Products = new List<Models.Home.Product>
                {
                    new Models.Home.Product { Name = "Milk", Price = 50.00},
                    new Models.Home.Product { Name = "Tomato", Price = 20.99},
                    new Models.Home.Product { Name = "Cheese", Price = 70.00},
                    new Models.Home.Product { Name = "Oil", Price = 70.00},
                    new Models.Home.Product { Name = "Cabbage", Price = 30.45},
                    new Models.Home.Product { Name = "Butter", Price = 80.00},
                    new Models.Home.Product { Name = "Eggs", Price = 4.50},
                    new Models.Home.Product { Name = "Water (6l)", Price = 40.99},
                    new Models.Home.Product { Name = "Bread", Price = 6.30}
                }
            };
            return View(model);
        }
        public IActionResult Services()
        {
            ViewData["now"] = _time_service.GetTime();
            ViewData["hashCode"] = _time_service.GetHashCode();
            ViewData["date_now"] = _date_service.GetDate();
            ViewData["date_hashCode"] = _date_service.GetHashCode();
            ViewData["dt_now"] = _dt_service.GetNow();
            ViewData["dt_hashCode"] = _dt_service.GetHashCode();
            ViewData["hash"] = _hash_service.Hash("123");
            return View();
        }
        public IActionResult Context()
        {
            ViewData["UsersCount"] = _data_context.Users.Count();
            return View();
        }
        public IActionResult Sessions(String? number)
        {
            _logger.LogInformation($"number: {number}");

            if (number is not null)
                HttpContext.Session.SetString("number", number);

            ViewData["number"] = HttpContext.Session.GetString("number");
            return View();
        }
        public IActionResult Middleware() { return View(); }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public ViewResult EmailConfirmation()
        {
            ViewData["config"] = _configuration["Smtp:Gmail:Host"];
            return View();
        }
    }
}