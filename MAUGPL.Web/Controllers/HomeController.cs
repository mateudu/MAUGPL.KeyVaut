using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using MAUGPL.Web.Infrastructure;
using MAUGPL.Web.Infrastructure.KeyVault;
using Microsoft.AspNetCore.Mvc;
using MAUGPL.Web.Models;
using Microsoft.Extensions.Configuration;

namespace MAUGPL.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly IKeyVaultManager _keyVaultManager;
        private readonly IConfiguration _configuration;

        public HomeController(
            IKeyVaultManager keyVaultManager,
            IConfiguration configuration
        )
        {
            _keyVaultManager = keyVaultManager;
            _configuration = configuration;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> GetKeyVaultEntry()
        {
            var secret = await _keyVaultManager.GetSecretAsync(Consts.TestSecretName);
            return Ok(new ConfigValueViewModel
            {
                Value = secret
            });
        }

        public IActionResult GetAppSettingsEntry()
        {
            var secret = _configuration[Consts.TestValueKey];
            return Ok(new ConfigValueViewModel
            {
                Value = secret
            });
        }
    }
}
