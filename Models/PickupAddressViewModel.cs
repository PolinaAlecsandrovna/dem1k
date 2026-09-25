using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dem1k.Models
{
    public class PickupAddressViewModel
    {
        public long Id { get; set; }
        public string FullAddress { get; set; }
        public override string ToString() => FullAddress; 
    }
}
