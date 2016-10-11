using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Treehouse.FitnessFrog.Data;
using Treehouse.FitnessFrog.Models;

namespace Treehouse.FitnessFrog.Controllers
{
    public class EntriesController : Controller
    {
        private EntriesRepository _entriesRepository = null;

        public EntriesController()
        {
            _entriesRepository = new EntriesRepository();
        }

        private void SetupActivitiesSelectListItems()
        {
            ViewBag.ActivitiesSelectListItems = new SelectList(
                 Data.Data.Activities, "Id", "Name");
        }

        private void ValidateEntry(Entry entry)
        {
            //If there aren't any "Duration" field validation errors 
            //then make sure that the duration is greater than "0". 
            if (ModelState.IsValidField("Duration") && entry.Duration <= 0)
            {
                ModelState.AddModelError("Duration", "The Duration field value must be greater than '0'.");
            }
        }

        public ActionResult Index()
        {
            List<Entry> entries = _entriesRepository.GetEntries();

            // Calculate the total activity.
            double totalActivity = entries
                .Where(e => e.Exclude == false)
                .Sum(e => e.Duration);

            // Determine the number of days that have entries.
            int numberOfActiveDays = entries
                .Select(e => e.Date)
                .Distinct()
                .Count();

            ViewBag.TotalActivity = totalActivity;
            ViewBag.AverageDailyActivity = (totalActivity / (double)numberOfActiveDays);

            return View(entries);
        }

        public ActionResult Add()
        {
            var entry = new Entry()
            {
                Date = DateTime.Today
            };

            SetupActivitiesSelectListItems();


            return View(entry);
        }

        [HttpPost]
        public ActionResult Add(Entry entry)
        {
            ValidateEntry(entry);

            if (ModelState.IsValid)
            {
                _entriesRepository.AddEntry(entry);

                TempData["Message"] = "Your Entry was successfully added.";

                return RedirectToAction("Index");
            }

            SetupActivitiesSelectListItems();

            return View(entry);
        }

        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Entry entry = _entriesRepository.GetEntry((int)id);

            // Return Not found if not found. 
            if (entry == null)
            {
                return HttpNotFound(); 
            }

            //Pass entry into view 
            SetupActivitiesSelectListItems();
            return View(entry);
        }

        [HttpPost]
        public ActionResult Edit(Entry entry)
        {
            //Validate Entry
            ValidateEntry(entry);

            //If valid use repository to update then redirect to "Index Entries page" 
            if (ModelState.IsValid)
            {
                _entriesRepository.UpdateEntry(entry);
                TempData["Message"] = "Your Entry was successfully updated.";
                return RedirectToAction("Index");
            }
            SetupActivitiesSelectListItems();


            return View(entry); 
        }

        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            //Retrieve Entry for user review 
            Entry entry = _entriesRepository.GetEntry((int)id);

            if (entry == null)
            {
                return HttpNotFound();
            }
            
            return View(entry);
        }

        [HttpPost]
        public ActionResult Delete(int id)
        {
            //Delete Entry 
            _entriesRepository.DeleteEntry(id);
            TempData["Message"] = "Your Entry was successfully deleted.";

            //Redirect to "Entries Index" 
            return RedirectToAction("Index"); 
        }


    }
}