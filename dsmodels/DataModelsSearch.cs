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
    /// <summary>
    /// Used for scan history page.
    /// </summary>
    [Table("vwSearchHistory")]
    public class SearchHistoryView
    {
        [JsonProperty(PropertyName = "id")]
        public int ID { get; set; }
        [JsonProperty(PropertyName = "seller")]
        public string Seller { get; set; }

        [JsonProperty(PropertyName = "updated")]
        public DateTime Updated { get; set; }
        public string UserId { get; set; }
        public int DaysBack { get; set; }
        public int MinSoldFilter { get; set; }
        public bool? Running { get; set; }
        //public int StoreID { get; set; }
        [JsonProperty(PropertyName = "calculateMatch")]
        public DateTime? CalculateMatch { get; set; }
        [JsonProperty(PropertyName = "userName")]
        public string UserName { get; set; }
        [JsonProperty(PropertyName = "listingCount")]
        public int ListingCount { get; set; }

    }

    [Table("SearchHistory")]
    public class SearchHistory
    {
        [JsonProperty(PropertyName = "id")]
        public int ID { get; set; }
        [JsonProperty(PropertyName = "seller")]
        public string Seller { get; set; }

        [JsonProperty(PropertyName = "updated")]
        public DateTime Updated { get; set; }
        public string UserId { get; set; }
        public int DaysBack { get; set; }
        public int MinSoldFilter { get; set; }
        public bool? Running { get; set; }
        //public int StoreID { get; set; }
        [JsonProperty(PropertyName = "calculateMatch")]
        public DateTime? CalculateMatch { get; set; }

        [ForeignKey("Seller")]
        public SellerProfile SellerProfile { get; set; }

    }
    public class ModelViewTimesSold
    {
        public List<TimesSold> TimesSoldRpt { get; set; }
        public int ListingsProcessed { get; set; }
        public int TotalOrders { get; set; }
        // public int MatchedListings { get; set; }
        public int ItemCount { get; set; }
    }

    public class TimesSold
    {
        public string Src { get; set; }
        public string Seller { get; set; }
        public string EbaySellerTitle { get; set; }
        public int? SoldQty { get; set; }
        public string EbayURL { get; set; }
        public int? RptNumber { get; set; }
        //public string ImageUrl { get; set; }
        public decimal Price { get; set; }  // last sold price
        public decimal EbaySellerPrice { get; set; }
        public DateTime? LastSold { get; set; }
        public string ItemID { get; set; }
        public DateTime? Listed { get; set; }
        public string SellingState { get; set; }
        public string ListingStatus { get; set; }
        public decimal? ListingPrice { get; set; }
        //public bool? IsMultiVariationListing { get; set; }
        public string ShippingServiceName { get; set; }
        public string ShippingServiceCost { get; set; }
        public string SellerUPC { get; set; }
        public string SellerMPN { get; set; }
        public byte? MatchCount { get; set; }
        public byte? MatchType { get; set; }
        public string ItemURL { get; set; }
        public bool? SoldAndShippedBySupplier { get; set; }
        public string SupplierBrand { get; set; }
        public decimal? SupplierPrice { get; set; }
        public string SellerBrand { get; set; }
        public bool? IsSupplierVariation { get; set; }
        public decimal? ProposePrice { get; set; }
        public bool? IsVero { get; set; }
        public string SupplierPicURL { get; set; }
        public bool? ToListing { get; set; }
        public bool? IsSellerVariation { get; set; }
        public DateTime? ListingRecCreated { get; set; }
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public decimal? PriceDelta { get; set; } // not part of db, just calculate for report.
        public int? SourceID { get; set; }
        public bool? IsFreightShipping { get; set; }
        public string Description { get; set; }
    }
    [Table("UpdateToListing")]
    public class UpdateToListing
    {
        public int ID { get; set; }
        public string UserID { get; set; }
        public string ItemID { get; set; }
        public bool ToListing { get; set; }
        public int StoreID { get; set; }
    }
    [Table("OrderHistory")]
    public class OrderHistory
    {
        public string Title { get; set; }
        public int RptNumber { get; set; }
        [ForeignKey("RptNumber")]
        public virtual SearchHistory SearchHistory { get; set; }

        public string EbayURL { get; set; } // this field is also present in SellerListing where more formally belongs but useful to have it on TimesSold page

        public bool ListingEnded { get; set; }
        public int PageNumber { get; set; }
        [Key]
        public string ItemID { get; set; }
        public int? SourceID { get; set; }
        public string SourceDescription { get; set; }
        public string PrimaryCategoryID { get; set; }
        public string PrimaryCategoryName { get; set; }
        public string PictureUrl { get; set; }
        public string Description { get; set; }
        public decimal? EbaySellerPrice { get; set; }   // seller's listed price
        public decimal? ShippingAmount { get; set; }
        public string SellingState { get; set; }
        public string ListingStatus { get; set; }   // Active, Completed
        public bool? IsSellerVariation { get; set; }
        public virtual List<OrderHistoryDetail> OrderHistoryDetails { get; set; }
        public decimal? ProposePrice { get; set; }
        public virtual List<OrderHistoryItemSpecific> ItemSpecifics { get; set; }
        public byte? MatchCount { get; set; }
        public byte? MatchType { get; set; }    // 1=UPC, 2=MPN
        public int? SupplierItemID { get; set; }

    }

    [Table("OrderHistoryDetail")]
    public class OrderHistoryDetail
    {
        [Key]
        public int ID { get; set; }
        public string ItemID { get; set; }
        public decimal Price { get; set; }
        public int Qty { get; set; }
        public DateTime DateOfPurchase { get; set; }
        public string Variation { get; set; }

        [ForeignKey("ItemID")]
        public OrderHistory OrderHistory { get; set; }
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

}
