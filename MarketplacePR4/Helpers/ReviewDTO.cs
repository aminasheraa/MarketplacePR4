using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarketplacePR4.Helpers
{
    public class ReviewDTO
    {
        public int ID {  get; set; }
        public DateTime Date { get; set; }
        public decimal Rating { get; set; }
        public string Description { get; set; }
        public string UserName { get; set; }
        public string ProductName { get; set; }
    }
}
