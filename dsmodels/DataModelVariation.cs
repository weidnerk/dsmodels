using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dsmodels
{
    /*
     * IMPORTANT -  these names can't change since the have to match incoming xml from GetSingleItem
     * 
     */

    /// <summary>
    /// has to be called 'Variation' to match incoming xml from GetSingleItem
    /// </summary>
    public class Variation
    {
        public string StartPrice { get; set; }
        public string Quantity { get; set; }
        public SellerVariationSpecifics VariationSpecifics { get; set; }
        public SellingStatus SellingStatus { get; set; }
    }
    public class SellerVariationSpecifics
    {
        public NameValueList NameValueList { get; set; }
    }
    public class NameValueList
    {
        public string Name { get; set; }
        public string Value { get; set; }
    }
    public class SellingStatus
    {
        public string QuantitySold { get; set; }
        public string QuantitySoldByPickupInStore { get; set; }
    }
}
