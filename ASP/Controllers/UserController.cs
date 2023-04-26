using ASP.DATA;
using ASP.DATA.Entity;
using ASP.Models.User;
using ASP.Services.Hash;
using ASP.Services.Kdf;
using ASP.Services.Random;
using ASP.Services.Validation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using System.ComponentModel.Design;
using System.IO;
using System.Linq.Expressions;
using System.Security.Claims;

namespace ASP.Controllers
{
    //[Route("User")]
    public class UserController : Controller
    {
        Random _rand = new Random();
        private readonly IHashService _hash_service;
        private readonly ILogger<UserController> _logger;
        private readonly DataContext _data_context;
        private readonly IRandomService _random_service;
        private readonly IKdfService _kdf_service;
        private readonly IValidationService _validation_service;
        private readonly IConfiguration _configuration;


        public UserController(IHashService hash_service, ILogger<UserController> logger, DataContext data_context, IRandomService random_service,
                            IKdfService kdf_service, IValidationService validation_service, IConfiguration configuration)
        {
            _hash_service = hash_service;
            _logger = logger;
            _data_context = data_context;
            _random_service = random_service;
            _kdf_service = kdf_service;
            _validation_service = validation_service;
            _configuration = configuration;
        }

        public IActionResult Index() { return View(); }
        public IActionResult Registration() { return View(); }
        public IActionResult RegisterUser(UserRegistrationModel user_registration_model)
        {
            UserValidationModel validation_results = new UserValidationModel();
            String avatar_filename = null!;
            bool is_model_valid = true;

            if (!_validation_service.Validate(user_registration_model.Login!, ValidationTerms.Login))
            {
                is_model_valid = false;
                validation_results.LoginMessage = $"Логин не соответсвует шаблону";
            }
            if (!_validation_service.Validate(user_registration_model.Email!, ValidationTerms.Email))
            {
                is_model_valid = false;
                validation_results.EmailMessage = $"Email не соответсвует шаблону";
            }

            if (user_registration_model.RepeatPassword != user_registration_model.Password)
            {
                is_model_valid = false;
                ModelState.AddModelError("RepeatPassword", "Your passwords do not match. Try again!");
            }
            if (_data_context.Users.Any(u => u.Login.ToLower() == user_registration_model.Login!.ToLower()))
            {
                is_model_valid = false;
                validation_results.LoginMessage = $"Логин '{user_registration_model.Login}' уже употребляется";
            }
            if (user_registration_model.Avatar is not null)
            {
                String ext = Path.GetExtension(user_registration_model.Avatar.FileName);
                String hash = (_hash_service.Hash(user_registration_model.Avatar.FileName + Guid.NewGuid()))[..16];

                avatar_filename = hash + ext;

                string path = "wwwroot/Avatars/" + avatar_filename;

                if (System.IO.File.Exists(path))
                {
                    is_model_valid = false;
                    ModelState.AddModelError("Avatar", "Файл с таким именем уже существует.");
                }
                else if (true/*user_registration_model.Avatar.Length <= 1024*/)
                {
                    using (var fileStream = new FileStream(path, FileMode.Create)) { user_registration_model.Avatar.CopyTo(fileStream); }

                    ViewData["avatarFilename"] = avatar_filename;
                }
            }

            if (is_model_valid)
            {
                String salt = _random_service.RandomString(8);
                User user = new User()
                {
                    Id = Guid.NewGuid(),
                    Login = user_registration_model.Login!,
                    PasswordSalt = salt,
                    PasswordHash = _kdf_service.GetDerivedKey(user_registration_model.Password, salt),
                    Avatar = _random_service.RandomFileName(20),
                    Email = user_registration_model.Email!,
                    RealName = user_registration_model.Name!,
                    RegisterDt = DateTime.Now,
                    LastEnterDt = null,
                    EmailCode = _random_service.ConfirmCode(6)
                };

                _data_context.Users.Add(user);
                _data_context.SaveChanges();

                ViewData["validation_results"] = validation_results;
                return View(user_registration_model);
            }
            else
            {
                ModelState.AddModelError("Avatar", "Файл должен быть не более 1 КБ");
                ViewData["validation_results"] = validation_results;
                return View("Registration");
            }
        }

        [HttpPost]
        public String AuthUser()
        {
            StringValues login_values = Request.Form["user-login"];
            StringValues password_values = Request.Form["user-password"];

            if (login_values.Count == 0 || password_values.Count == 0)
                return "No login or password";

            User? user = _data_context.Users.Where(u => u.Login == login_values[0]).FirstOrDefault();

            if (user is not null)
            {
                if (user.PasswordHash == _kdf_service.GetDerivedKey(password_values[0] ?? "", user.PasswordSalt))
                {
                    HttpContext.Session.SetString("authUserId", user.Id.ToString());
                    return $"OK";
                }
            }

            return $"--Rejected--\nLogin: {login_values[0]}\nPassword: {password_values[0]}";
        }

        public IActionResult Logout()  // Когда пользователь выходит из системы, этот метод вызывается для удаления информации об
        {                             // аутентифицированном пользователе из контекста запроса и сессии.
            HttpContext.Items.Remove("authUser");  // Удаляет элемент authUser из объекта HttpContext.Items.
            HttpContext.Session.Remove("authUserId");  // Удаляет значение authUserId из объекта HttpContext.Session.
            return RedirectToAction("Index", "Home");
        }

        public IActionResult Profile([FromRoute] String id)
        {
            //_logger.LogInformation(id);
            DATA.Entity.User? user = _data_context.Users.FirstOrDefault(u => u.Login == id);

            if (user is not null)
            {
                Models.User.ProfileModel model = new Models.User.ProfileModel(user);

                if (String.IsNullOrEmpty(model.Avatar))
                    model.Avatar = "v1ymsp6yua3xf0izjdai.png";

                if (HttpContext.User.Identity is not null && HttpContext.User.Identity.IsAuthenticated)
                {
                    String userLogin = HttpContext.User.Claims.First(claim => claim.Type == ClaimTypes.NameIdentifier).Value;

                    if (model.Login == userLogin)
                        model.IsPersonal = true;
                }
                return View(model);
            }
            else
                return NotFound();
        }

        [HttpPut]
        public JsonResult Update([FromBody] UserUpdateModel model)
        {
            if (model is null)
                return Json(new { status = "Error", data = "No or invalid Data" });
            if (HttpContext.User.Identity?.IsAuthenticated != true)
                return Json(new { status = "Error", data = "Unauthenticated" });

            User? user = null;

            try { user = _data_context.Users.Find(Guid.Parse(HttpContext.User.Claims.First(claim => claim.Type == ClaimTypes.Sid).Value)); }
            catch { }

            if (user is null)
                return Json(new { status = "Error", data = "Anautorized" });

            switch (model.Field)
            {
                case "realname":
                    if (!_validation_service.Validate(model.Value, ValidationTerms.RealName))
                        return Json(new { status = "Error", data = $"Validation field {model.Field}, value = '{model.Value}'" });

                    user.RealName = model.Value;
                    _data_context.SaveChanges();
                    return Json(new { status = "Ok", data = $"Changet {user.RealName}" });
                default:
                    return Json(new { status = "Error", data = $"Unknown field {model.Field}" });
            }
        }
    }
}