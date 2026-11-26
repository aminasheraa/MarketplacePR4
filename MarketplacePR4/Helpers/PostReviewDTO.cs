using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarketplacePR4.Helpers
{
    public class PostReviewDTO
    {
        public int ProductID { get; set; }
        public decimal Rating { get; set; }
        public string  Description { get; set; }
    }
}
