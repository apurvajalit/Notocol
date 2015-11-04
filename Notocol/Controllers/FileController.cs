using Business;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Notocol.Controllers
{
    public class FileController : Controller
    {
        public FileResult GetFile(long userId, int version, string fileName)
        {
            string filePath = new FileUploadHelper().GetFilePath(userId, version, fileName);
            if (filePath != null)
                return File(filePath, "application/pdf");
            else return null;
        }

        public FileResult GetTempFile(string id)
        {
            string filePath = new FileUploadHelper().GetTempFilePath(id);
            if (filePath != null)
                return File(filePath, "application/pdf");
            else return null;
        }

        // GET: File
        public ActionResult UploadFile()
        {
            return View("FileUploader");
        }


    }
}