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
                return RedirectToAction("Index");
            }
            //Model binding occurs and Request Form Field names are matched to variables above
            //Uses Model state in html form helpers 
            //DateTime? date, int? activityId, double? duration, Entry.IntensityLevel? intensity, bool? exclude, string notes
            //Above replaced with Entry entry 

            SetupActivitiesSelectListItems();

            return View(entry);
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

        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            //Get requersted Entry from repository. 
            Entry entry = _entriesRepository.GetEntry((int)id);
            // Return Not found if not found. 
            if (entry == null)
            {
                return HttpNotFound(); 
            }
            //Pass entry into view 
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

            return View();
        }
    }
}