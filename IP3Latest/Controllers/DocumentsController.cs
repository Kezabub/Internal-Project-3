using System;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using IP3Latest.Models;
using System.IO;
using PagedList;
using PagedList.Mvc;
using Microsoft.AspNet.Identity;
using System.Web.Security;
using Microsoft.AspNet.Identity.EntityFramework;
using RestSharp;
using RestSharp.Authenticators;

namespace IP3Latest.Controllers
{
    //documents controller main bulk of the code is contained here
    [Authorize (Roles = "Document Author, Distributee, Administrator")]
    public class DocumentsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Documents
        //displays a list of all documents with navigation to all the other pages e.g edit, delete, create, details
        public ActionResult Index(string sortOrder, string currentFilter, string searchString, int? page)
        {

            ViewBag.CurrentSort = sortOrder;
            ViewBag.NameSortParm = String.IsNullOrEmpty(sortOrder) ? "title_desc" : "";
            ViewBag.PriceSortParm = sortOrder == "RevisionNumber" ? "revision_desc" : "RevisionNumber";

            if (searchString != null)
            {

            }
            else
            {
                searchString = currentFilter;
            }

            ViewBag.CurrentFilter = searchString;

            var documents = from o in db.Documents
                            select o;
            var NewDocument = documents.Where(o => o.DocumentAuthor == User.Identity.Name || o.Distributee == User.Identity.GetUserId() && o.DocumentStatus == "Active");

            if (!String.IsNullOrEmpty(searchString))
            {
                documents = documents.Where(s => s.DocTitle.ToUpper().Contains(searchString.ToUpper()));
            }
            switch (sortOrder)
            {
                case "title_desc":
                    documents = documents.OrderByDescending(s => s.DocTitle);
                    break;
                case "RevisionNumber":
                    documents = documents.OrderBy(s => s.RevisionNumber);
                    break;
                case "revision_desc":
                    documents = documents.OrderByDescending(s => s.RevisionNumber);
                    break;
                default:  // Name ascending 
                    documents = documents.OrderBy(s => s.DocTitle);
                    break;
            }



            int pageSize = 10;
            int pageNumber = (page ?? 1);

            return View(documents.ToPagedList(pageNumber, pageSize));

        }

        //same as Index but only displays personal documents eg documents that you made or documents that have been distributeed to you
        public ActionResult MyDocuments(string sortOrder, string currentFilter, string searchString, int? page)
        {
            ViewBag.CurrentSort = sortOrder;
            ViewBag.NameSortParm = String.IsNullOrEmpty(sortOrder) ? "title_desc" : "";
            ViewBag.PriceSortParm = sortOrder == "RevisionNumber" ? "revision_desc" : "RevisionNumber";

            if (searchString != null)
            {

            }
            else
            {
                searchString = currentFilter;
            }
            var um = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext()));
            var user = um.FindById(User.Identity.GetUserId());
            ViewBag.CurrentFilter = searchString;
            var documents = from o in db.Documents.Where(o => o.DocumentAuthor == User.Identity.Name || o.Distributee == user.Email  && o.DocumentStatus == "Active")
                            select o;
            

            if (!String.IsNullOrEmpty(searchString))
            {
                documents = documents.Where(s => s.DocTitle.ToUpper().Contains(searchString.ToUpper()));
            }
            switch (sortOrder)
            {
                case "title_desc":
                    documents = documents.OrderByDescending(s => s.DocTitle);
                    break;
                case "RevisionNumber":
                    documents = documents.OrderBy(s => s.RevisionNumber);
                    break;
                case "revision_desc":
                    documents = documents.OrderByDescending(s => s.RevisionNumber);
                    break;
                default:  // Name ascending 
                    documents = documents.OrderBy(s => s.DocTitle);
                    break;
            }



            int pageSize = 10;
            int pageNumber = (page ?? 1);

            return View(documents.ToPagedList(pageNumber, pageSize));
        }



        // GET: Documents/Details/5
        //returns the details page for a single record
        public ActionResult Details(int? id)
        {
            
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Document document = db.Documents.Find(id);
            if (document == null)
            {
                return HttpNotFound();
            }
            if (User.IsInRole("Distributee") && (document.DocumentStatus == "Draft" || document.DocumentStatus == "Archived") && (document.Distributee != User.Identity.GetUserId()))
            {
                return RedirectToAction("Index");
            }
            return View(document);
        }

        [Authorize (Roles = "Document Author")]
        // GET: Documents/Create
        //returns the create view
        public ActionResult Create()
        {
            var nameQuery = from user in db.Users
                            where user.Roles.Any(r => r.RoleId == "4ba13c9f-2403-45ad-961e-7c5cb6b08bc9")
                            orderby user.FirstName
                            select new
                            {
                                Email = user.Email,
                                Name = user.FirstName + " " + user.LastName
                            };
            ViewBag.Distributee = new SelectList(nameQuery, "Email", "Name");
            return View();
        }

        // POST: Documents/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        /// <summary>
        /// 
        /// </summary>
        /// <param name="document"></param>
        /// <returns></returns>
        /// Allows Document Authors to create New Documents, which requires him to input a title a revision number (that must be unique to that record of documents, the distributee and upload the file associated with that record
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Document Author")]
        public ActionResult Create([Bind(Include = "DocumentID,DocTitle,RevisionNumber,DocumentAuthor,CreationDate,ActivationDate,DocumentStatus,FilePath,Distributee")] Document document, HttpPostedFileBase file)
        {
            try
            {
                if (file.ContentLength > 0)
                {
                    //saves the file to the file store
                    string _FileName = Path.GetFileName(file.FileName);
                    string _path = Path.Combine(Server.MapPath("~/UploadedFiles"), _FileName);
                    file.SaveAs(_path);

                    //sets up variables not inputted by user
                    document.CreationDate = DateTime.Now;
                    document.ActivationDate = DateTime.Now;
                    document.DocumentAuthor = User.Identity.Name;
                    document.DocumentStatus = "Draft";
                    document.FilePath = _path;
                }
                //checks there is no documents with the same title and revision number E.G "Title: Bork RevisionNumber: 1" when this is the record you are trying to make
                bool unique = db.Documents.Any(o => o.DocTitle == document.DocTitle && o.RevisionNumber == document.RevisionNumber && o.DocumentID != document.DocumentID);
                //If that is true the document record will be created
                if (ModelState.IsValid & unique == false)
                {
                    ViewBag.Message = "File Uploaded Successfully!!";
                    db.Documents.Add(document);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
            }
            //if not the error will be caught and a message will be displayed to the user
            catch
            {
                ViewBag.Message = "File upload failed! or document input does not meet standards";
                return View(document);
            }
            var nameQuery = from user in db.Users
                            where user.Roles.Any(r => r.RoleId == "4ba13c9f-2403-45ad-961e-7c5cb6b08bc9")
                            orderby user.FirstName
                            select new
                            {
                                Email = user.Email,
                                Name = user.FirstName + " " + user.LastName
                            };
            ViewBag.Distributee = new SelectList(nameQuery, "Email", "Name", document.Distributee);
            return View(document);
        }

        [Authorize(Roles = "Document Author")]
        // GET: Documents/Edit/5
        //returns the edit view
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Document document = db.Documents.Find(id);
            try
            {
                if (document.DocumentStatus == "Archived" )
                {
                    return RedirectToAction("Index");
                }
                if (document.DocumentStatus == "Active")
                {
                    return RedirectToAction("Index");
                }
                if (document == null)
                {
                    return HttpNotFound();
                }
            }
            catch
            {
                return RedirectToAction("Index");
            }
            //namequery finds all users in the distributee role and populates a dropdownlist with these users
            var nameQuery = from user in db.Users
                            where user.Roles.Any(r => r.RoleId == "4ba13c9f-2403-45ad-961e-7c5cb6b08bc9")
                            orderby user.FirstName
                            select new
                            {
                                Email = user.Email,
                                Name = user.FirstName + " " + user.LastName
                            };
            //Dropdown Lists that allow you to select a distributee and author
            ViewBag.Distributee = new SelectList(nameQuery, "Email", "Name");
            ViewBag.DocumentAuthor = new SelectList(db.Users, "UserName", "UserName", document.DocumentAuthor);
            return View(document);
        }

        // POST: Documents/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        //Edits the selected document only the Document Author can perform this and it can only be done to draft documents
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Document Author")]
        public ActionResult Edit([Bind(Include = "DocumentID,DocTitle,RevisionNumber,DocumentAuthor,CreationDate,ActivationDate,DocumentStatus,FilePath,Distributee")]Document document)
        {
            if (document.DocumentStatus == "Archived")
            {
                return RedirectToAction("Index");
            }
            if (document.DocumentStatus == "Active")
            {
                return RedirectToAction("Index");
            }

            try
            {
                //checks there is no documents with the same title and revision number E.G "Title: Bork RevisionNumber: 1" when this is the record you are trying to make
                bool unique = db.Documents.Any(o => o.DocTitle == document.DocTitle && o.RevisionNumber == document.RevisionNumber && o.DocumentID != document.DocumentID);
                //If that is true the document record will be editted
                if (ModelState.IsValid & unique == false)
                    {
                        db.Entry(document).State = EntityState.Modified;
                        db.SaveChanges();
                        return RedirectToAction("Index");
                    }
            }
            //if not the error will be caught and a message will be displayed to the user
            catch
            {
                ViewBag.Message = "Revision Number Must be Unique!";
                return View(document);
            }
            //namequery finds all users in the distributee role and populates a dropdownlist with these users
            var nameQuery = from user in db.Users
                            where user.Roles.Any(r => r.RoleId == "4ba13c9f-2403-45ad-961e-7c5cb6b08bc9")
                            orderby user.FirstName
                            select new
                            {
                                Email = user.Email,
                                Name = user.FirstName + " " + user.LastName
                            };
            //Dropdown Lists that allow you to select a distributee and author
            ViewBag.Distributee = new SelectList(nameQuery, "Email", "Name", document.Distributee);
            ViewBag.DocumentAuthor = new SelectList(db.Users, "Id", "UserName", document.DocumentAuthor);
            return View(document);
        }

        // GET: Documents/Delete/5
        //returns the delete view
        [Authorize(Roles = "Document Author")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Document document = db.Documents.Find(id);
            try
            {

                if (document == null)
                {
                    return HttpNotFound();
                }
            }
            catch
            {
                return RedirectToAction("Index");
            }
            return View(document);
        }

        // POST: Documents/Delete/5
        //Deletes record from the database
        //Can only be done by the Document Author
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Document Author")]
        public ActionResult DeleteConfirmed(int id)
        {
            Document document = db.Documents.Find(id);
            try
            {
                string path = document.FilePath;
                FileInfo file = new FileInfo(path);
                file.Delete();
                document.FilePath = "none";
                db.Documents.Remove(document);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            catch
            {
                db.Documents.Remove(document);
                db.SaveChanges();
                return RedirectToAction("Index");
            } 
        }

        //changes the status to active or archived as needed also responsible for creating new document revisions
        [Authorize(Roles = "Document Author")]
        public ActionResult ChangeStatus(int? id)
        {
            
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                Document document = db.Documents.Find(id);
            if (document == null)
            {
                return HttpNotFound();
            }
            if (document.DocumentStatus == "Archived")
            {
                return RedirectToAction("Index");
            }
                try
                {
                    if (document.DocumentStatus == "Draft")
                    {
                    //checks for a document with that title and is active exists
                    var check = db.Documents.SingleOrDefault(o => o.DocTitle == document.DocTitle && o.DocumentStatus == "Active");
                    if (check != null)
                    {
                        //if this isnt null set the checked value to archived and the new document to active
                        check.DocumentStatus = "Archived";
                        document.DocumentStatus = "Active";
                        document.ActivationDate = DateTime.Now;
                    }
                    else
                    {
                        //if this was null set the new document to active
                        document.DocumentStatus = "Active";
                        document.ActivationDate = DateTime.Now;
                    }
                        if (ModelState.IsValid)
                        {
                            db.Entry(document).State = EntityState.Modified;
                            db.SaveChanges();
                            ViewBag.Message = "Status of document changed to active";
                            var email = document.Distributee;
                            ResponseModel.SendSimpleMessage(email);
                            return RedirectToAction("Index");
                        }
                    }
                    else if (document.DocumentStatus == "Active")
                    {
                    //creates the new document from the values of its parent document
                    var newdocument = new Document();
                    newdocument.DocTitle = document.DocTitle;
                    newdocument.RevisionNumber = document.RevisionNumber + 1;
                    newdocument.DocumentAuthor = document.DocumentAuthor;
                    newdocument.DocumentStatus = "Draft";
                    //sets the dates to right now and sets the file path to none (an attachment can be added from the details page once it is created)
                    newdocument.CreationDate = DateTime.Now;
                    newdocument.ActivationDate = DateTime.Now;
                    newdocument.FilePath = "none";
                    newdocument.Distributee = document.Distributee;

                    //adds the new document to the database if the model state is valid if not it will return this page without submitting
                    if (ModelState.IsValid)
                        {
                            db.Documents.Add(newdocument);
                            db.SaveChanges();
                            return RedirectToAction("Index");
                        }
                    }
                    else
                    {
                        return View(document);
                    }
                }
                catch
                {
                    return RedirectToAction("Index");
                }
                return View(document);
            }

        //removes the current distributee and changes the value to none
        [Authorize(Roles = "Document Author")]
        public ActionResult RemoveDistributee(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Document document = db.Documents.Find(id);
            if (document == null)
            {
                return HttpNotFound();
            }
            if (document.DocumentStatus == "Archived")
            {
                return RedirectToAction("Index");
            }
            if (document.DocumentStatus == "Activated")
            {
                return RedirectToAction("Index");
            }

            if (document.Distributee != "none")
            {
                document.Distributee = "none";
                if (ModelState.IsValid)
                {
                    db.Entry(document).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
            }
            else
            {
                return RedirectToAction("Index");
            }
            return View(document);
        }

        //finds and deletes the attachment in uploaded files folder then changes the pth to "none"
        [Authorize(Roles = "Document Author")]
        public ActionResult RemoveAttachment(int? id, object sender, EventArgs e)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Document document = db.Documents.Find(id);
            if (document == null)
            {
                return HttpNotFound();
            }
            if (document.DocumentStatus == "Archived")
            {
                return RedirectToAction("Index");
            }
            if (document.DocumentStatus == "Activated")
            {
                return RedirectToAction("Index");
            }

            //locates the file via the path saved in the database and if it exists deletes it once it is deleted the path is set to none and the new value is saved to the database
            string path = document.FilePath;
            FileInfo file = new FileInfo(path);
            if (file.Exists)
            {
                file.Delete();
                ViewBag.Message = "File Deleted";
                document.FilePath = "none";
                if (ModelState.IsValid)
                {
                    db.Entry(document).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
            }
            else
            {
                ViewBag.Message = "This file does not exist";
                return RedirectToAction("Index");
            }
            return View(document);
        }

        [Authorize(Roles = "Document Author")]
        // returns the add attachment view
        public ActionResult AddAttachment(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Document document = db.Documents.Find(id);
            try
            {
                if (document.DocumentStatus == "Archived")
                {
                    return RedirectToAction("Index");
                }
                if (document.DocumentStatus == "Active")
                {
                    return RedirectToAction("Index");
                }
                if (document.FilePath != "none")
                {
                    return RedirectToAction("Index");
                }
                if (document == null)
                {
                    return HttpNotFound();
                }
            }
            catch
            {
                return RedirectToAction("Index");
            }
            return View(document);
        }

        //only accessible if the document is in draft the user is logged in as a document author and there is no current attachment
        //takes in a file from the user saves the file in the storage folder and updates the database path for the record with this new path
        [HttpPost]
        [Authorize(Roles = "Document Author")]
        public ActionResult AddAttachment(int? id, HttpPostedFileBase file)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Document document = db.Documents.Find(id);
            if (document == null)
            {
                return HttpNotFound();
            }
            if (document.DocumentStatus == "Archived")
            {
                return RedirectToAction("Index");
            }
            if (document.DocumentStatus == "Activated")
            {
                return RedirectToAction("Index");
            }
            try
            {
                if (file.ContentLength > 0)
                {
                    string _FileName = Path.GetFileName(file.FileName);
                    string _path = Path.Combine(Server.MapPath("~/UploadedFiles"), _FileName);
                    file.SaveAs(_path);
                    document.FilePath = _path;
                    if (ModelState.IsValid)
                    {
                        db.Entry(document).State = EntityState.Modified;
                        db.SaveChanges();
                        return RedirectToAction("Index");
                    }
                }
            }
            catch
            {
                ViewBag.Message = "File upload failed! or document input does not meet standards";
                return View(document);
            }
            return View(document);
        }


        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}

//Response Model makes use of mailgun API to send emails to distributees when there is documents ready for them to view
public class ResponseModel
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="email"></param>
    /// <returns></returns>
    public static IRestResponse SendSimpleMessage(string email)
    {
        var document = new Document();
        RestClient client = new RestClient();
        client.BaseUrl = new Uri("https://api.mailgun.net/v3");
        client.Authenticator =
                new HttpBasicAuthenticator("api",
                                           "key-f72d70c645350a58d4ce92d0df30c281");
        RestRequest request = new RestRequest();
        request.AddParameter("domain",
                             "sandbox914ac94e81cc46408d1ecd8a6ca9d54d.mailgun.org", ParameterType.UrlSegment);
        request.Resource = "{domain}/messages";
        request.AddParameter("from", "Document Management System <postmaster@sandbox914ac94e81cc46408d1ecd8a6ca9d54d.mailgun.org>");
        request.AddParameter("to", email);
        request.AddParameter("subject", "Document Waiting for you!");
        request.AddParameter("text", "Hi a document has been distributeed to you this can be viewed on the document management system from the my documents page");
        request.Method = Method.POST;
        return client.Execute(request);
    }
}