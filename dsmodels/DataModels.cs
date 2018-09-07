using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dsmodels
{
    // hate this - StagedListing is nearly identical to PostedListing but has one additional field that does not exist in the PostedListing table
    public class StagedListing
    {
        public string EbaySeller { get; set; }
        public string EbayItemID { get; set; }
        public string EbayUrl { get; set; }
        [Key]
        [Column(Order = 1)]
        public int SourceID { get; set; }
        [Key]
        [Column(Order = 2)]
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
        public byte SoldQty { get; set; }
        public string Availability { get; set; }
        public string Limit { get; set; }

        public decimal MinPrice { get; set; }
    }

    [Table("PostedListings")]
    public class PostedListing
    {
        public string EbaySeller { get; set; }
        public string EbayItemID { get; set; }
        public string EbayUrl { get; set; }
        [Key]
        [Column(Order = 1)]
        public int SourceID { get; set; }
        [Key]
        [Column(Order = 2)]
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
        public DateTime? Updated { get; set; }
    }

    // Listing is used for the research reporting.
    // SingleItem is used for the detail page.
    // Case can be made to just use the Listing class.
    [Table("Listing")]
    public class Listing
    {
        [Key]
        public string ItemId { get; set; }
        public string Title { get; set; }
        public string ListingTitle { get; set; }
        public List<OrderHistory> Orders { get; set; }
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
        public string ListingStatus { get; set; }

    }
    [Table("OrderHistory")]
    public class OrderHistory
    {
        public int ID { get; set; }
        public string Title { get; set; }
        public string SupplierPrice { get; set; }
        public string Qty { get; set; }
        //public string DateOfPurchaseStr { get; set; }
        public DateTime? DateOfPurchase { get; set; }
        public int RptNumber { get; set; }
        public string EbayUrl { get; set; }

        public string ImageUrl { get; set; }
        public bool ListingEnded { get; set; }
        public int PageNumber { get; set; }
        public string ItemId { get; set; }
    }

    [Table("SourceCategories")]
    public class SourceCategories
    {
        public int ID { get; set; }
        public int SourceID { get; set; }
        public string Category { get; set; }
        public string SubCategory { get; set; }
        public string URL { get; set; }
    }

}
