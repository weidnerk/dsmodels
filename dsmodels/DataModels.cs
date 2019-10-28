using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dsmodels
{

    [Table("SearchHistory")]
    public class SearchHistory
    {
        public int Id { get; set; }
        public string Seller { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime Updated { get; set; }
        public string UserId { get; set; }
        public int DaysBack { get; set; }
        public int MinSoldFilter { get; set; }
        public bool? Running { get; set; }
        public int StoreID { get; set; }
    }


    // Listing is used for the research reporting.
    // SingleItem is used for the detail page.
    // Case can be made to just use the Listing class.
    [Table("Listing")]
    public class Listing
    {
        [Key]
        public string ItemId { get; set; }  // ebay eller listing id
        public string Title { get; set; }
        public string ListingTitle { get; set; }
        public List<OrderHistory> Orders { get; set; }
        public string EbayUrl { get; set; }
        public string Description { get; set; }
        public decimal SupplierPrice { get; set; }
        public string PictureUrl { get; set; }   // store picture urls as a semi-colon delimited string
        public decimal ListingPrice { get; set; }
        public string SourceUrl { get; set; }          // source url
        public string PrimaryCategoryID { get; set; }
        public string PrimaryCategoryName { get; set; }
        public int Qty { get; set; }
        public string ListingStatus { get; set; }
        public DateTime? Listed { get; set; }
        public byte? SourceID { get; set; }
        public string ListedItemID { get; set; }
        public string SupplierItemID { get; set; }
        public bool OOS { get; set; }
        public string Seller { get; set; }
        public DateTime? Updated { get; set; }
        public bool Variation { get; set; }
        public string VariationDescription { get; set; }
        public decimal SellerPrice { get; set; }
        public bool? CheckShipping { get; set; }         // no supplier shipping issues (like back-ordered)
        public bool? CheckSource { get; set; }           // that supplier is walmart
        public bool? CheckVero { get; set; }
        public bool? CheckCategory { get; set; }
        public byte? CheckCompetition { get; set; }
        public bool? CheckSellerShipping { get; set; }   // seller offers free shipping
        public decimal Profit { get; set; }
        public double ProfitMargin { get; set; }
        public int RptNumber { get; set; }
        public byte SellerSold { get; set; }
        public string ListedBy { get; set; }
        public bool? CheckSupplierPrice { get; set; }    // confirm supplier's price
        public bool? CheckSupplierItem { get; set; }     // make sure supplier item is same item as seller
        public bool ListedWithAPI { get; set; }
        public bool? CheckMainCompetitor { get; set; }
        public string ListedResponse { get; set; }
        public DateTime? ListedUpdated { get; set; }
        public virtual List<ItemSpecific> ItemSpecifics { get; set; }
        public bool? CheckSupplierPics { get; set; }
        public int StoreID { get; set; }
        public bool? CheckIsVariation { get; set; }
        public bool? CheckVariationURL { get; set; }
    }

    [Table("ItemSpecific")]
    public class ItemSpecific
    {
        [JsonProperty(PropertyName = "id")]
        public int Id { get; set; }

        [JsonProperty(PropertyName = "sellerItemId")]
        
        public string SellerItemId { get; set; }
        
        [JsonProperty(PropertyName = "itemName")] 
        public string ItemName { get; set; }
        
        [JsonProperty(PropertyName = "itemValue")] 
        public string ItemValue { get; set; }

        [ForeignKey("SellerItemId")]
        [JsonProperty(PropertyName = "listing")]
        public Listing Listing { get; set; }
    }
    public class ShippingCostSummary
    {
        public string ShippingServiceCost { get; set; }
        public string ShippingServiceName { get; set; }
    }

    public class Dashboard
    {
        [JsonProperty(PropertyName = "oos")]
        public int OOS { get; set; }
        [JsonProperty(PropertyName = "notListed")]
        public int NotListed { get; set; }
    }

    [Table("OrderHistory")]
    public class OrderHistory
    {
        public int ID { get; set; }
        public string Title { get; set; }
        public string SellerPrice { get; set; }
        public string Qty { get; set; }
        //public string DateOfPurchaseStr { get; set; }
        public DateTime? DateOfPurchase { get; set; }
        public int RptNumber { get; set; }
        public string EbayUrl { get; set; }

        public string ImageUrl { get; set; }
        public bool ListingEnded { get; set; }
        public int PageNumber { get; set; }
        public string ItemId { get; set; }
        public int SourceID { get; set; }
        public string SupplierItemId { get; set; }
        public string SourceDescription { get; set; }
        public string PrimaryCategoryID { get; set; }
        public string PrimaryCategoryName { get; set; }
        public string PictureUrl { get; set; }
        public string Description { get; set; }
        public string EbaySeller { get; set; }
        public decimal? EbaySellerPrice { get; set; }
        public int CategoryId { get; set; }
        public decimal? ShippingAmount { get; set; }
        public string SellingState { get; set; }
        public string ListingStatus { get; set; }   // Active, Completed
        public bool? IsMultiVariationListing { get; set; }
        public string ShippingServiceName { get; set; }
        public string ShippingServiceCost { get; set; }
    }

    [Table("vwSellerOrderHistory")]  // this view is not updatable
    public class SellerOrderHistory
    {
        public int ID { get; set; }
        public string Title { get; set; }
        public string SellerPrice { get; set; }
        public string Qty { get; set; }
        //public string DateOfPurchaseStr { get; set; }
        public DateTime? DateOfPurchase { get; set; }
        public int RptNumber { get; set; }
        public string EbayUrl { get; set; }

        public string ImageUrl { get; set; }
        public bool ListingEnded { get; set; }
        public int PageNumber { get; set; }
        public string ItemId { get; set; }
        public DateTime? Listed { get; set; }               // listed by user
        public string SellingState { get; set; }
        public string ListingStatus { get; set; }           // status of scanned eBay seller
        public decimal? ListingPrice { get; set; }          // user's listing price
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
    [Table("WalItems")]
    public class WalItem
    {
        [JsonProperty(PropertyName = "id")]
        public int ID { get; set; }                 // identity field - but still can use this class if no db involved
        public string Title { get; set; }
        public string Description { get; set; }

        [JsonProperty(PropertyName = "price")]
        public decimal Price { get; set; }
        public string DetailUrl { get; set; }
        public int CategoryID { get; set; }
        public string ItemId { get; set; }

        public string PictureUrl { get; set; }
        public bool OutOfStock { get; set; }
        public bool ShippingNotAvailable { get; set; }
        [JsonProperty(PropertyName = "fulfilledByWalmart")]
        public bool FulfilledByWalmart { get; set; }
    }


    public class AppIDSelect
    {
        public string value { get; set; }
        public string viewValue { get; set; }
    }


    [Table("SellerProfile")]
    public class SellerProfile
    {
        [Key]
        public string Seller { get; set; }
        [JsonProperty(PropertyName = "note")]
        public string Note { get; set; }
        [JsonProperty(PropertyName = "updated")] 
        public DateTime Updated { get; set; }
        [JsonProperty(PropertyName = "updatedBy")] 
        public string UpdatedBy { get; set; }
        public string UserID { get; set; }
    }

}
