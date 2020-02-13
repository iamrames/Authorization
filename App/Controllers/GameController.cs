using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using App.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace App.Controllers
{
    public class GameController : Controller
    {
        [MinimumAgeAuthorize(5)]
        public IActionResult Index()
        {
            return View();
        }
    }
}