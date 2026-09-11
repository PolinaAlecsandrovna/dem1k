using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dem1k.Models
{
    public class ProductViewModel
    {
        public int Id { get; set; }
        public string Article { get; set; }
        public string Name { get; set; }
        public string CategoryName { get; set; }
        public string Description { get; set; }
        public string ManufacturerName { get; set; }
        public string SupplierName { get; set; }
        public decimal Price { get; set; }
        public string UnitName { get; set; }
        public int StockQuantity { get; set; }
        public decimal DiscountPercent { get; set; }
        public string ImagePath { get; set; }

        public decimal FinalPrice => Price - (Price * DiscountPercent / 100);

        public string RowBackgroundColor
        {
            get
            {
                if (StockQuantity == 0)
                    return "#ADD8E6"; 
                if (DiscountPercent > 12)
                    return "#F4A460"; 
                return "White"; 
            }
        }

        public bool HasDiscount => DiscountPercent > 0;

        public bool IsDiscountVisible => DiscountPercent > 0;
    }
}
