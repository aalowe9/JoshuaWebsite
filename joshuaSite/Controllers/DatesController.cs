using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using joshuaSite.Models;
using joshuaSite.Utilities;

namespace joshuaSite.Controllers
{
    public class DatesController : Controller
    {

        [Authorize]
        public ActionResult Manager()
        {
            return View(DynamoHelper.GetAllDates("JWLDateTable"));
        }

        [Authorize]
        public ActionResult Edit(string id)
        {
            return View(DynamoHelper.GetDate(id));
        }

        [HttpPost]
        [Authorize]
        public ActionResult Edit(DateItem di, HttpPostedFileBase image = null)
        {
            if (ModelState.IsValid)
            {
                if (image != null)
                {
                    string[] names = image.FileName.Split('.');
                    string newName = Guid.NewGuid().ToString() + "." + names[names.Length - 1];
                    image.SaveAs(Server.MapPath("~/_dynamic/") + newName);
                    //save image
                    S3Helper.SaveToS3(Server.MapPath("~/_dynamic/") + newName, "joshua-web-alexa-bucket",
                        "UploadsFromAlexa", newName);


                    di.Images = new List<Image>();
                    Image i = new Image();
                    i.ImagePath = "https://s3-eu-west-1.amazonaws.com/joshua-web-alexa-bucket/UploadsFromAlexa/" +
                                  newName;
                    di.Images.Add(i);

                }
                DynamoHelper.UpdateDateItem(di);

                return RedirectToAction("Manager");
            }

            return View(di);
        }

        [Authorize]
        public ActionResult Delete(string id)
        {
            DynamoHelper.DeleteDateItem(id);
            return RedirectToAction("Manager");
        }

        [Authorize]
        public ActionResult Create()
        {
            return View(new DateItem());
        }

        [HttpPost]
        [Authorize]
        public ActionResult Create(DateItem di, HttpPostedFileBase image = null)
        {
            if (ModelState.IsValid)
            {
                if (image != null)
                {
                    string[] names = image.FileName.Split('.');
                    string newName = Guid.NewGuid().ToString() + "." + names[names.Length - 1];
                    image.SaveAs(Server.MapPath("~/_dynamic/") + newName);
                    //save image
                    S3Helper.SaveToS3(Server.MapPath("~/_dynamic/") + newName, "joshua-web-alexa-bucket","UploadsFromAlexa", newName);

                    di.Images = new List<Image>();
                    Image i = new Image();
                    i.ImagePath = "https://s3-eu-west-1.amazonaws.com/joshua-web-alexa-bucket/UploadsFromAlexa/" + newName;
                    di.Images.Add(i);

                }

                di.DateString = di.DateTime.ToString("dd-MM-yyyy");
                di.FullDateStamp = di.DateTime;
                di.ID = Guid.NewGuid().ToString();
                
                DynamoHelper.CreateDateItem(di);
                return RedirectToAction("Manager");
            }
            else
            {
                return View(di);
            }
        }

    }
}