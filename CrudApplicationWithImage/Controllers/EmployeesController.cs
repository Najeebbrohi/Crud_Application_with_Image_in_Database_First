using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using CrudApplicationWithImage.Models;

namespace CrudApplicationWithImage.Controllers
{
    public class EmployeesController : Controller
    {
        private CustomAuthEntities db = new CustomAuthEntities();

        // GET: Employees
        public ActionResult Index()
        {
            return View(db.Employees.ToList());
        }

        // GET: Employees/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Employee employee = db.Employees.Find(id);
            if (employee == null)
            {
                return HttpNotFound();
            }
            return View(employee);
        }

        // GET: Employees/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Employees/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Employee employee)
        {
            try
            {
                if (ModelState.IsValid && employee.File != null)
                {
                    string filename = Path.GetFileName(employee.File.FileName);
                    //string _filename = DateTime.Now.ToString("hhmmssfff") + filename;
                    string _filename = Guid.NewGuid().ToString() + filename;
                    string path = Path.Combine(Server.MapPath("~/Content/Images/"), _filename);
                    employee.Image = "~/Content/Images/" + _filename;

                    db.Employees.Add(employee);
                    if (employee.File.ContentLength < 1000000)
                    {
                        if(db.SaveChanges() > 0)
                        {
                            employee.File.SaveAs(path);
                            return RedirectToAction("Index");
                        }
                    }
                    else
                    {
                        ViewBag.Msg = "File Must be less than or equal to 1MB";
                    }
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }

            return View(employee);
        }

        // GET: Employees/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Employee employee = db.Employees.Find(id);
            Session["imgPath"] = employee.Image;
            if (employee == null)
            {
                return HttpNotFound();
            }
            return View(employee);
        }

        // POST: Employees/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Employee employee)
        {
            if (ModelState.IsValid)
            {
                if(employee.File != null)
                {
                    string filename = Path.GetFileName(employee.File.FileName);
                    string file = Guid.NewGuid().ToString() + filename;
                    string path = Path.Combine(Server.MapPath("~/Content/Images/"), file);
                    employee.Image = "~/Content/Images/" + file;

                    if (employee.File.ContentLength < 1000000)
                    {
                        db.Entry(employee).State = EntityState.Modified;
                        string OldImgPath = Request.MapPath(Session["imgPath"].ToString());

                        if (db.SaveChanges() > 0)
                        {
                            employee.File.SaveAs(path);
                            if (System.IO.File.Exists(OldImgPath))
                            {
                                System.IO.File.Delete(OldImgPath);
                            }
                        }
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        ViewBag.Msg = "Image should be less than or equal to 1MB";
                    }
                }
                else
                {
                    employee.Image = Session["imgPath"].ToString();
                    db.Entry(employee).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
            }
            return View(employee);
        }

        // GET: Employees/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Employee employee = db.Employees.Find(id);
            if (employee == null)
            {
                return HttpNotFound();
            }
            return View(employee);
        }

        // POST: Employees/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Employee employee = db.Employees.Find(id);
            string CurrentImg = Request.MapPath(employee.Image);

            db.Employees.Remove(employee);
            if (db.SaveChanges() > 0)
            {
                if (System.IO.File.Exists(CurrentImg))
                {
                    System.IO.File.Delete(CurrentImg);
                }
            }
            return RedirectToAction("Index");
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
