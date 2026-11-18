using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarketplacePR4.Helpers
{

    public class CartItemDTO 
    {
        public int ID { get; set; }
        public int ProductID {  get; set; }
        public int Quantity {  get; set; }
        public ProductDTO Product {  get; set; }
    }

}
