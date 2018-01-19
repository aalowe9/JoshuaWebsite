﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Compilation;
using System.Web.Mvc;
using System.Web.Security;
using Amazon.Runtime;
using joshuaSite.Models;

namespace joshuaSite.Controllers
{
    public class LoginController : Controller
    {
        // GET: Login
        public ActionResult Index()
        {

            return View(new LoginModel());
        }

        [HttpPost]
        public ActionResult Index(LoginModel user)
        {

            if (ModelState.IsValid)
            {
                string u = ConfigurationManager.AppSettings["UserName"];
                string p = ConfigurationManager.AppSettings["Password"];

                if (user.UserName == u &&
                    user.Password == p)
                {
                    FormsAuthentication.SetAuthCookie(user.UserName, true);
                    return RedirectToAction("Manager", "Login");
                }
            }
            return View(user);
        }

        [Authorize]
        public ActionResult Manager()
        {
            //get all dates out of dynamo and allow editing

            //show all dates in grid with edit button


            return View();
        }
    }
}