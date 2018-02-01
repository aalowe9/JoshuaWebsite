using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using joshuaSite.Models;
using joshuaSite.Utilities;

namespace joshuaSite.Controllers
{
    public class AlexaController : ApiController
    {


        [System.Web.Http.AcceptVerbs("GET", "POST")]
        [System.Web.Http.HttpGet]
        public string CreateDateItem(string id)
        {
            DateItem di = new DateItem();
            di.DateTime = DateTime.Now;
            di.DateString = DateTime.Now.ToString("dd-MM-yyyy");
            di.FullDateStamp = DateTime.Now;
            di.ID = Guid.NewGuid().ToString();
            di.SpecialDate = false;
            di.Synopsis = id;
            di.TextContent = id;

            DynamoHelper.CreateDateItem(di);

            return ("created date item");
        }

    }
}
