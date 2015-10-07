using Business;
using Model;
using Model.Extended;
using Model.Extended.Extension;
using Newtonsoft.Json.Linq;
using Notocol.Models;
using Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Notocol.Controllers
{

    public class NotesController : Controller
    {
               
        public ActionResult NoteList(long sourceID, bool ownAtTop = false)
        {
            AnnotationHelper notesHelper = new AnnotationHelper();
            
            AnnotationRepository objAnnotationRepository = new AnnotationRepository();
            
            return PartialView("NoteList", notesHelper.GetNoteList(sourceID, ownAtTop, Utility.GetCurrentUserID()));

        }

        public ActionResult Note(long ID = 0)
        {
            Annotation annotation = new AnnotationRepository().GetAnnotation(ID);
            if (annotation == null)
                return View(annotation);   //passing null
            return View(new AnnotationHelper().GetNoteData(annotation));
        }
    }
}