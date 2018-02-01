using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
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
        public async Task<ActionResult> Edit(DateItem di, HttpPostedFileBase image = null)
        {
            if (ModelState.IsValid)
            {
                if (image != null)
                {
                    string[] names = image.FileName.Split('.');
                    string newName = Guid.NewGuid().ToString() + "." + names[names.Length - 1];
                    //image.SaveAs(Server.MapPath("~/_dynamic/") + newName);
                    //save image

                    var img = await ImageHelper.RezizeImage(System.Drawing.Image.FromStream(image.InputStream, true, true), 800, 600);
                    using (MemoryStream s = new MemoryStream())
                    {
                        img.Save(s, System.Drawing.Imaging.ImageFormat.Jpeg);
                        await S3Helper.SaveToS3(s, "joshua-web-alexa-bucket", "UploadsFromAlexa", newName);
                    }


                    di.Images = new List<Image>();
                    Image i = new Image();
                    i.ImagePath = "https://s3-eu-west-1.amazonaws.com/joshua-web-alexa-bucket/UploadsFromAlexa/" + newName;
                    di.Images.Add(i);

                }
                DynamoHelper.UpdateDateItem(di, image);

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
        public async Task<ActionResult> Create(DateItem di, HttpPostedFileBase image = null)
        {
            if (ModelState.IsValid)
            {
                if (image != null)
                {
                    string[] names = image.FileName.Split('.');
                    string newName = Guid.NewGuid().ToString() + "." + names[names.Length - 1];
                    //image.SaveAs(Server.MapPath("~/_dynamic/") + newName);
                    //save image
                    var img = await ImageHelper.RezizeImage(System.Drawing.Image.FromStream(image.InputStream, true, true), 800, 600);
                    using (MemoryStream s = new MemoryStream())
                    {
                        img.Save(s, System.Drawing.Imaging.ImageFormat.Jpeg);
                        await S3Helper.SaveToS3(s, "joshua-web-alexa-bucket", "UploadsFromAlexa", newName);
                    }
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