using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dem1k.Models
{
    public class OrderViewModel
    {
        public int Id { get; set; }
        public string Article { get; set; }
        public string StatusName { get; set; }
        public string PickupAddress { get; set; }
        public DateTime OrderDate { get; set; }
        public DateTime? DeliveryDate { get; set; }
    }
}
