using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using App.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace App.Controllers
{
    public class ReportsController : Controller
    {
        private readonly IAuthorizationService _authorizationService;

        public ReportsController(IAuthorizationService authorizationService)
        {
            _authorizationService = authorizationService;
        }

        public async Task<IActionResult> Index(int documentId)
        {
            var document = new Document()
            {
                Id = 1,
                Title = "My report",
                AuthorName = "Ervis"
            };

            var authorized = await _authorizationService.AuthorizeAsync(User, document, "EditPolicy");
            if (authorized.Succeeded) return View(document);
            return Forbid();
        }
    }
}