using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarketplacePR4.Helpers
{
    public class OrderDTO
    {
        public int ID {  get; set; }
        public int UserID { get; set; }
        public DateTime DateOfOrder { get; set; }
        public decimal TotalPrice { get; set; }
        public int StatusID { get; set; }
        public List<OrderProductDTO> Products { get; set; }
        public string Adress { get; set; }
    }
}
