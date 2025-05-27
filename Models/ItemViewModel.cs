using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HotelierTask.Models
{
    public class ItemViewModel
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "This Field Required")]
        public int? SelectedSectionId { get; set; }
        [Required(ErrorMessage = "This Field Required")]

        public int? SelectedCounterId { get; set; }
        [Required(ErrorMessage = "This Field Required")]

        public int? SelectedCategoryId { get; set; }
        [Required(ErrorMessage = "This Field Required")]

        public int ItemCode { get; set; }
        [Required(ErrorMessage = "This Field Required")]

        public string ItemName { get; set; }
        public string ItemShortName { get; set; }
        public decimal? SaleRate { get; set; }
        public decimal? ItemDiscPerc { get; set; }
        public DateTime? DiscEndDate { get; set; }
        public bool IsMostUsed { get; set; }

        public List<SelectListItem> Sections { get; set; }
        public List<SelectListItem> Counters { get; set; }
        public List<SelectListItem> Categories { get; set; }

        
        public List<ItemListDTO> ItemList { get; set; }
    }

    public class ItemListDTO
    {
        public int Id { get; set; }
        public int ItemCode { get; set; }
        public string ItemName { get; set; }
        public string ItemShortName { get; set; }
        public decimal? SaleRate { get; set; }
        public decimal? ItemDiscPerc { get; set; }
        public string CategoryName { get; set; }
        public string CounterName { get; set; }
        public string SectionName { get; set; }
    }
}
