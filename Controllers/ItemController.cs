using HotelierTask.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace HotelierTask.Controllers
{
    public class ItemController : Controller
    {
        private MyTaskDBEntities db = new MyTaskDBEntities();

        // GET: Item
        public ActionResult Index()
        {
            var model = new ItemViewModel
            {
                Sections = db.Sections.Where(x => x.Active).Select(s => new SelectListItem
                {
                    Value = s.Id.ToString(),
                    Text = s.SectionName
                }).ToList(),
                Counters = new List<SelectListItem>(),
                Categories = new List<SelectListItem>(),
                ItemCode = db.Items.Any() ? db.Items.Max(i => i.ItemCode) + 1 : 1,
                ItemList = GetItemList()
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Save(ItemViewModel model)
        {
            if (ModelState.IsValid)
            {
                Item item;
                if (model.Id > 0)
                {
                    item = db.Items.Find(model.Id);
                    if (item == null) return HttpNotFound();
                }
                else
                {
                    item = new Item
                    {
                        CreatedAt = DateTime.Now
                    };
                    db.Items.Add(item);
                }

                item.CategoryId = model.SelectedCategoryId.Value;
                item.ItemCode = model.ItemCode;
                item.ItemName = model.ItemName;
                item.ItemShortName = model.ItemShortName;
                item.SaleRate = model.SaleRate;
                item.ItemDiscPerc = model.ItemDiscPerc;
                item.DiscEndDate = model.DiscEndDate;
                item.IsMostUsed = model.IsMostUsed;
                item.Active = true;
                item.UpdatedAt = DateTime.Now;

                db.SaveChanges();
                return RedirectToAction("Index");
            }

            model.Sections = db.Sections.Where(x => x.Active).Select(s => new SelectListItem
            {
                Value = s.Id.ToString(),
                Text = s.SectionName
            }).ToList();
            model.Counters = new List<SelectListItem>();
            model.Categories = new List<SelectListItem>();
            model.ItemList = GetItemList();

            return View("Index", model);
        }

        public ActionResult Edit(int id)
        {
            var item = db.Items.Include("Category.Counter.Section").FirstOrDefault(i => i.Id == id);
            if (item == null) return HttpNotFound();

            var model = new ItemViewModel
            {
                Id = item.Id,
                ItemCode = item.ItemCode,
                ItemName = item.ItemName,
                ItemShortName = item.ItemShortName,
                SaleRate = item.SaleRate,
                ItemDiscPerc = item.ItemDiscPerc,
                DiscEndDate = item.DiscEndDate,
                IsMostUsed = (bool)item.IsMostUsed,

                SelectedCategoryId = item.CategoryId,
                SelectedCounterId = item.Category.CounterId,
                SelectedSectionId = item.Category.Counter.SectionId,

                Sections = db.Sections.Where(x => x.Active).Select(s => new SelectListItem
                {
                    Value = s.Id.ToString(),
                    Text = s.SectionName
                }).ToList(),
                Counters = db.Counters.Where(c => c.SectionId == item.Category.Counter.SectionId && c.Active)
                                      .Select(c => new SelectListItem
                                      {
                                          Value = c.Id.ToString(),
                                          Text = c.CounterName
                                      }).ToList(),
                Categories = db.Categories.Where(cat => cat.CounterId == item.Category.CounterId && cat.Active)
                                          .Select(cat => new SelectListItem
                                          {
                                              Value = cat.Id.ToString(),
                                              Text = cat.CategoryName
                                          }).ToList(),

                ItemList = GetItemList()
            };

            return View("Index", model);
        }

        public ActionResult Delete(int id)
        {
            var item = db.Items.Find(id);
            if (item != null)
            {
                db.Items.Remove(item);
                db.SaveChanges();
            }
            return RedirectToAction("Index");
        }

        public JsonResult GetCounters(int sectionId)
        {
            var counters = db.Counters
                .Where(c => c.SectionId == sectionId && c.Active)
                .Select(c => new { c.Id, c.CounterName })
                .ToList();

            return Json(counters, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetCategories(int counterId)
        {
            var categories = db.Categories
                .Where(c => c.CounterId == counterId && c.Active)
                .Select(c => new { c.Id, c.CategoryName })
                .ToList();

            return Json(categories, JsonRequestBehavior.AllowGet);
        }

        private List<ItemListDTO> GetItemList()
        {
            var items = (from item in db.Items
                         join category in db.Categories on item.CategoryId equals category.Id
                         join counter in db.Counters on category.CounterId equals counter.Id
                         join section in db.Sections on counter.SectionId equals section.Id
                         where item.Active
                         select new ItemListDTO
                         {
                             Id = item.Id,
                             ItemCode = item.ItemCode,
                             ItemName = item.ItemName,
                             ItemShortName = item.ItemShortName,
                             SaleRate = item.SaleRate,
                             ItemDiscPerc = item.ItemDiscPerc,
                             CategoryName = category.CategoryName,
                             CounterName = counter.CounterName,
                             SectionName = section.SectionName
                         }).ToList();

            return items;
        }
    }
}