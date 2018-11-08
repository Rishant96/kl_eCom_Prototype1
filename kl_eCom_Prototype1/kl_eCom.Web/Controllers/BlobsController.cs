using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.Azure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;

namespace kl_eCom.Web.Controllers
{
    public class BlobsController : Controller
    {
        

        // GET: Blobs
        public ActionResult Index()
        {
            return View();
        }
    }
}