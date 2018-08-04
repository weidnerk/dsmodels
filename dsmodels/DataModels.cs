using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dsmodels
{
    [Table("PostedListings")]
    public class PostedListing
    {
        public string EbaySeller { get; set; }
        [Key]
        public string EbayItemID { get; set; }
        public string EbayUrl { get; set; }
        public int SourceID { get; set; }
        public string SupplierItemID { get; set; }
        public string SourceUrl { get; set; }
        public decimal SupplierPrice { get; set; }
        public string Title { get; set; }
        public decimal Price { get; set; }
        public string Description { get; set; }
        public string Pictures { get; set; }
        public int CategoryID { get; set; }
        public string PrimaryCategoryID { get; set; }
        public string PrimaryCategoryName { get; set; }
        public string ListedItemID { get; set; }
        public DateTime? Listed { get; set; }
        public DateTime? Removed { get; set; }
        public byte ListedQty { get; set; }
        public bool? InventoryException { get; set; }
    }
    public class Listing
    {
        public string ItemId { get; set; }
        public string Title { get; set; }
        public string ListingTitle { get; set; }
        public string EbayUrl { get; set; }
        public string Description { get; set; }
        public decimal SupplierPrice { get; set; }
        public string PictureUrl { get; set; }   // store picture urls as a semi-colon delimited string
        public decimal ListingPrice { get; set; }
        public string Source { get; set; }
        public string PrimaryCategoryID { get; set; }
        public string PrimaryCategoryName { get; set; }
        public byte SourceId { get; set; }

        public int Qty { get; set; }
    }


}
