﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace WeatherStation.HealthPortal.Controllers
{
    public class HomeController : Controller
    {

        public async Task<ActionResult> Index()
        {
            await Services.GremlinService.CreateConditionGraphAsync();

            return View();
        }
    }
}