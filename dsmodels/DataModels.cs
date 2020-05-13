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
    public class DTO
    {
        public List<string> FieldNames { get; set; }

    }
    public class ListingDTO : DTO
    {
        public Listing Listing { get; set; }
    }
    public class UserSettingsDTO : DTO
    {
        public UserSettings UserSettings { get; set; }
    }
    public class UpdateToListingDTO : DTO
    {
        public UpdateToListing UpdateToListing { get; set; }
    }
    public class SalesOrderDTO : DTO
    {
        public SalesOrder SalesOrder { get; set; }
    }
    public class eBayKeysDTO : DTO
    {
        public UserProfileKeys eBayKeys { get; set; }
        public string Token { get; set; }
        public int StoreID { get; set; }
    }
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

    /// <summary>
    /// Used on listings page.
    /// 01.14.2020 This class had inherited from Listing but when upating/delete from Listing table (not vwListing) would get
    /// and error:
    /// "View or function 'dbo.vwListing' is not updatable because the modification affects multiple base tables."
    /// so had to remove base class and duplicate fields.
    /// </summary>
    [Table("vwListing")]
    public class ListingView
    {

        // Returned by view as read/only
        //[DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public string ListedByName { get; set;}
        //[DatabaseGenerated(DatabaseGeneratedOption.Computed)] 
        public string CreatedByName { get; set; }
        //[DatabaseGenerated(DatabaseGeneratedOption.Computed)] 
        public string Seller { get; set; }
        //[DatabaseGenerated(DatabaseGeneratedOption.Computed)] 
        public string Title { get; set; }
        [Key]
        public int ID { get; set; }
        public string ItemID { get; set; }
        [ForeignKey("ItemID")]                                      // works with or w/out
        public virtual SellerListing SellerListing { get; set; }    // listing will typically be based on some seller's listing
        public string ListingTitle { get; set; }
        public string Description { get; set; }
        public string PictureUrl { get; set; }                      // store picture urls as a semi-colon delimited string
        public decimal ListingPrice { get; set; }
        public string SourceURL { get; set; }
        public string PrimaryCategoryID { get; set; }
        public string PrimaryCategoryName { get; set; }
        public int Qty { get; set; }
        public DateTime? Listed { get; set; }
        public string ListedItemID { get; set; }
        public DateTime? Updated { get; set; }
        //public string UpdatedBy { get; set; }
        public bool? CheckShipping { get; set; }         // no supplier shipping issues (like back-ordered)
        public bool? CheckSource { get; set; }           // that supplier is walmart
        public bool? CheckVero { get; set; }
        public bool? CheckCategory { get; set; }
        public byte? CheckCompetition { get; set; }
        public bool? CheckSellerShipping { get; set; }   // seller offers free shipping
        public decimal Profit { get; set; }
        public double ProfitMargin { get; set; }
        public int RptNumber { get; set; }
        public string ListedBy { get; set; }
        public string ListedUpdatedBy { get; set; }
        public bool? CheckSupplierPrice { get; set; }    // confirm supplier's price
        public bool? CheckSupplierItem { get; set; }     // make sure supplier item is same item as seller
        public bool ListedWithAPI { get; set; }
        public byte? CheckMainCompetitor { get; set; }
        public string ListedResponse { get; set; }
        public DateTime? ListedUpdated { get; set; }

        public bool? CheckSupplierPics { get; set; }
        public int StoreID { get; set; }
        public bool? CheckIsVariation { get; set; }
        public bool? CheckVariationURL { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? Created { get; set; }
        //[DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public string UpdatedByName { get; set; }
        public string  SupplierPicURL { get; set; }
    }

    /// <summary>
    /// Was originally created to move seller parts out of Listing table when I stored from the SellerListing page.
    /// That has chanced in that now created when user clicks "Store to listing" (see method, StoreToListing) where
    /// a Lising record gets created and a corresponding SellerListing record becomes a property of that Listing record.
    /// Can exist independant of a Listing
    /// </summary>
    [Table("SellerListing")]
    public class SellerListing
    {
        [Key]
        public string ItemID { get; set; }                      // ebay eller listing id
        public string Title { get; set; }
        public string EbayURL { get; set; }
        public string Seller { get; set; }
        public byte SellerSold { get; set; }
        public decimal SellerPrice { get; set; }

        public string ListingStatus { get; set; }
        public bool Variation { get; set; }

        // This value can also be found in Variation.VariationSpecifics.NameValueList.Name for each variation since that is how it arrives from eBay
        public string VariationName { get; set; }    

        public virtual List<SellerListingItemSpecific> ItemSpecifics { get; set; }
        [ForeignKey("ItemID")]
        public virtual List<Listing> Listings { get; set; }     // seller listing could be in multiple stores
        public string PictureURL { get; set; }
        public string Description { get; set; }
        public string PrimaryCategoryID { get; set; }
        public string PrimaryCategoryName { get; set; }
        public int? Qty { get; set; }
        public DateTime Updated { get; set; }
        [NotMapped]
        public List<Variation> Variations { get; set; }
    }

    [Table("Listing")]
    public class Listing
    {
        [Key]
        //[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        public string ItemID { get; set; }

        [NotMapped]
        [ForeignKey("ItemID")]
        public virtual SellerListing SellerListing { get; set; }
        public string ListingTitle { get; set; }
        public string Description { get; set; }
        public string PictureURL { get; set; }                      // store picture urls as a semi-colon delimited string
        public decimal ListingPrice { get; set; }
        public string PrimaryCategoryID { get; set; }
        public string PrimaryCategoryName { get; set; }
        public int Qty { get; set; }
        public DateTime? Listed { get; set; }
        public string ListedItemID { get; set; }            // ID created for my listing, not the seller's itemID
        public DateTime? Updated { get; set; }
        public string UpdatedBy { get; set; }
        public bool? CheckShipping { get; set; }         // no supplier shipping issues (like back-ordered)
        public bool? CheckSource { get; set; }           // that supplier is walmart
        public bool? CheckVero { get; set; }
        public bool? CheckCategory { get; set; }
        public byte? CheckCompetition { get; set; }
        public bool? CheckSellerShipping { get; set; }   // seller offers free shipping
        public decimal Profit { get; set; }
        public double ProfitMargin { get; set; }
        public int RptNumber { get; set; }
        public string ListedBy { get; set; }
        public string ListedUpdatedBy { get; set; }
        public bool? CheckSupplierPrice { get; set; }    // confirm supplier's price
        public bool? CheckSupplierItem { get; set; }     // make sure supplier item is same item as seller
        public bool ListedWithAPI { get; set; }
        public byte? CheckMainCompetitor { get; set; }
        public string ListedResponse { get; set; }
        public DateTime? ListedUpdated { get; set; }
        
        public bool? CheckSupplierPics { get; set; }
        public int StoreID { get; set; }
        public bool? CheckIsVariation { get; set; }
        public bool? CheckVariationURL { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? Created { get; set; }
        public string UPC { get; set; }
        public string MPN { get; set; }

        public int SupplierID { get; set; }
        [ForeignKey("SupplierID")]
        public virtual SupplierItem SupplierItem { get; set; }
        public DateTime? Ended { get; set; }
        public string EndedBy { get; set; }
        public List<string> Warning { get; set; }
        public virtual List<ListingItemSpecific> ItemSpecifics { get; set; }    // lazy load
        public string eBaySellerURL { get; set; }
        public double PctProfit { get; set; }
        public bool InActive { get; set; }
    }

    // SellerItemID here can either point to SellerListing or OrderHistory.
    // In the case of OrderHistory, look at the SellerScraper program.  As we store the scan results,
    // we call GetSingleItem to fields like description and the seller listing's item specifics.
    //
    // In the case of SellerListing, look at StoreToListing. 
    //
    // This also means I can't put a FK on ItemSpecific.SellerItemID
    //
    // When do we delete from this table?  When we delete a scan - See sp_ItemSpecificRemove
    [Table("SellerListingItemSpecific")]
    public class SellerListingItemSpecific
    {
        [JsonProperty(PropertyName = "id")]
        public int ID { get; set; }

        [JsonProperty(PropertyName = "sellerItemId")]
        public string SellerItemID { get; set; }
        
        [JsonProperty(PropertyName = "itemName")] 
        public string ItemName { get; set; }
        
        [JsonProperty(PropertyName = "itemValue")] 
        public string ItemValue { get; set; }

        [ForeignKey("SellerItemID")]
        [JsonProperty(PropertyName = "listing")]
        public SellerListing SellerListing { get; set; }
        public bool? Flags { get; set; }
        public DateTime? Updated { get; set; }
    }
    [Table("OrderHistoryItemSpecific")]
    public class OrderHistoryItemSpecific
    {
        [JsonProperty(PropertyName = "id")]
        public int ID { get; set; }

        [JsonProperty(PropertyName = "sellerItemId")]
        public string SellerItemID { get; set; }

        [JsonProperty(PropertyName = "itemName")]
        public string ItemName { get; set; }

        [JsonProperty(PropertyName = "itemValue")]
        public string ItemValue { get; set; }

        // reads "SellerItemID in this class is the FK to the PK in OrderHistory"
        [ForeignKey("SellerItemID")]    
        [JsonProperty(PropertyName = "orderHistory")]
        public OrderHistory OrderHistory { get; set; }
        public bool? Flags { get; set; }
    }
    [Table("ListingItemSpecific")]
    public class ListingItemSpecific
    {
        [JsonProperty(PropertyName = "id")]
        public int ID { get; set; }

        [JsonProperty(PropertyName = "listingID")]
        public int ListingID { get; set; }

        [JsonProperty(PropertyName = "itemName")]
        public string ItemName { get; set; }

        [JsonProperty(PropertyName = "itemValue")]
        public string ItemValue { get; set; }

        [ForeignKey("ListingID")]
        [JsonProperty(PropertyName = "listing")]
        public Listing Listing { get; set; }
        public bool? Flags { get; set; }
        public DateTime? Updated { get; set; }
    }
    public class ShippingCostSummary
    {
        public string ShippingServiceCost { get; set; }
        public string ShippingServiceName { get; set; }
    }

    /// <summary>
    /// "Quick" aggregations that fit on dashboard.
    /// </summary>
    public class Dashboard
    {
        [JsonProperty(PropertyName = "listed")]
        public int Listed { get; set; }
        [JsonProperty(PropertyName = "oos")]
        public int OOS { get; set; }
        [JsonProperty(PropertyName = "notListed")]
        public int NotListed { get; set; }
    }

    /// <summary>
    /// Pull more extensive metrics from actual eBay store.
    /// </summary>
    public class StoreAnalysis
    {
        [JsonProperty(PropertyName = "dbIsMissingItems")]
        public List<string> DBIsMissingItems { get; set; }
        [JsonProperty(PropertyName = "qtyMismatch")]
        public List<string> QtyMismatch { get; set; }
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

    [Table("SourceCategories")]
    public class SourceCategories
    {
        public int ID { get; set; }
        public int SourceID { get; set; }
        public string Category { get; set; }
        public string SubCategory { get; set; }
        public string URL { get; set; }
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
        public bool Active { get; set; }
        public virtual List<SearchHistory> SearchHistory { get; set; }
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

    public class UserProfileVM
    {
        public string userName { get; set; }
        public string AppID { get; set; }
        public string DevID { get; set; }
        public string CertID { get; set; }
        public string UserToken { get; set; }
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public string ApplicationID { get; set; }
    }

    /// <summary>
    /// Create a response after searching for prodID on Walmart
    /// </summary>
    public class WalmartSearchProdIDResponse
    {
        [JsonProperty(PropertyName = "count")]
        public byte Count { get; set; } // how many results after searching given term on supplier site
        [JsonProperty(PropertyName = "url")]
        public string URL { get; set; } // supplier URL
    }

    [Table("VEROBrands")]
    public class VEROBrands
    {
        public int ID { get; set; }
        public string Brand { get; set; }
    }

    /// <summary>
    /// Records created when click Calculate Match
    /// When do you delete from this table?  Nothing really in place - can delete when the itemID is no longer used in the Listing table.
    /// </summary>
    [Table("SupplierItem")]
    public class SupplierItem
    {
        // Listing has a FK in SupplierItem and SellerListing but I used different data annotations to get it working.
        // Need to research the reasoning.

        // https://entityframework.net/one-to-one-relationship
        public int ID { get; set; }
        public string ItemURL { get; set; }
        public bool? SoldAndShippedBySupplier { get; set; }
        public string SupplierBrand { get; set; }
        public decimal? SupplierPrice { get; set; }
        public bool? IsVariation { get; set; }
        public string SupplierPicURL { get; set; }
        public string UPC { get; set; } // Walmart doesn't always give you a UPC number so must be nullable in DB
        public string MPN { get; set; }
        public string Description { get; set; }
        public string ItemID { get; set; }  // this is supplier item number, not to be confused with ebay seller itemID

        // Item may say "Delivery not available", then it may or may not show "out of stock" depending on the zip code
        // which makes sens since you have to go to the store.
        public bool OutOfStock { get; set; }
        public bool ShippingNotAvailable { get; set; }
        //[ForeignKey("ID")]
        //public virtual List<Listing> Listing { get; set; }
        //public virtual Listing Listing { get; set; }

        [NotMapped]
        public DateTime? Arrives { get; set; }
        [NotMapped]
        public int? BusinessDaysArrives { get; set; }
        [NotMapped]
        public bool? IsVERO { get; set; }
        public DateTime Updated { get; set; }
        public int SourceID { get; set; }
        [NotMapped]
        public List<string> VariationPicURL { get; set; }
        public bool? IsFreightShipping { get; set; }

        [NotMapped]
        public string VariationName { get; set; }   // variation descriptor, like 'Color'
        [NotMapped]
        public List<string> usItemId { get; set; }
        [NotMapped]
        public List<SupplierVariation> SupplierVariation { get; set; }
        [NotMapped]
        public List<string> CanList { get; set; }
        [NotMapped]
        public List<string> Warning { get; set; }
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
    [Table("SalesOrder")]
    public class SalesOrder
    {
        [JsonProperty(PropertyName = "id")]
        public int ID { get; set; }
        [JsonProperty(PropertyName = "listingID")]
        public int ListingID { get; set; }
        [JsonProperty(PropertyName = "listedItemID")]
        public string ListedItemID { get; set; }
        [JsonProperty(PropertyName = "datePurchased")]
        public DateTime DatePurchased { get; set; }
        [JsonProperty(PropertyName = "qty")]
        public int Qty { get; set; }

        // Had a UC on this field since supplier should not have duplicate order numbers but we might get same order number
        // by different suppliers.  This constraint has been implemented as a trigger, TR_SalesOrder_Add.
        [JsonProperty(PropertyName = "supplierOrderNumber")]
        public string SupplierOrderNumber { get; set; }

        // API gives you OrderID which is not same as order number - not sure, I don't use it, don't require it
        [JsonProperty(PropertyName = "ebayOrderNumber")]
        public string eBayOrderNumber { get; set; }
        [JsonProperty(PropertyName = "buyer")]
        public string Buyer { get; set; }
        [JsonProperty(PropertyName = "buyerHandle")]
        public string BuyerHandle { get; set; }
        [JsonProperty(PropertyName = "buyerPaid")]
        public decimal BuyerPaid { get; set; }  // I think in my case BuyerPaid and Total are same thing since my terms are "fixed price, buy now"
        [JsonProperty(PropertyName = "i_paid")]
        public decimal I_Paid { get; set; }
        [JsonProperty(PropertyName = "buyerState")]
        public string BuyerState { get; set; }
        [JsonProperty(PropertyName = "total")]
        public decimal Total { get; set; }
        [JsonProperty(PropertyName = "subTotal")]
        public decimal SubTotal { get; set; }           // looks to be what the item actually sold for (my list price)
        [JsonProperty(PropertyName = "salesTax")]
        public decimal SalesTax { get;  set; }
        [JsonProperty(PropertyName = "shippingCost")]
        public decimal ShippingCost { get; set; }
        [JsonProperty(PropertyName = "finalValueFee")]
        public decimal FinalValueFee { get; set; }
        [JsonProperty(PropertyName = "payPalFee")]
        public decimal PayPalFee { get; set; }
        [JsonProperty(PropertyName = "profit")]
        public decimal Profit { get; set; }
        [JsonProperty(PropertyName = "profitMargin")]
        public double ProfitMargin { get; set; }
        [JsonProperty(PropertyName = "orderStatus")]
        public string OrderStatus { get; set; }
        [JsonProperty(PropertyName = "trackingNumber")]
        public string TrackingNumber { get; set; }
        [JsonProperty(PropertyName = "createdBy")]
        public string CreatedBy { get; set; }
    }

    /// <summary>
    /// Intent is response from GetOrders() vs db table, SalesOrder.
    /// </summary>
    public class GetOrdersResponse
    {
        public string BuyerHandle { get; set; }
        public string Buyer { get; set; }
        public DateTime DatePurchased { get; set; }
        public decimal BuyerPaid { get; set; }
        public string BuyerState { get; set; }
        public string ItemID { get; set; }
    }

    [Table("AppSettings")]
    public class AppSettings
    {
        [Key]
        public string SettingName { get; set; }
        public string SettingValue { get; set; }
    }

    /// <summary>
    /// Created to display price information generated when calculating wm price.
    /// </summary>
    public class PriceProfit
    {
        [JsonProperty(PropertyName = "breakEven")]
        public decimal BreakEven { get; set; }
        [JsonProperty(PropertyName = "proposePrice")]
        public decimal ProposePrice { get; set; }
    }
    public class SupplierVariation
    {
        public string ItemID { get; set; }
        public string URL { get; set; }
        public string Variation { get; set; }   // actual variation like 'Red' or 'Black'
        public decimal? Price { get; set; }
        public List<string> Images { get; set; }
    }
    [Table("vwListingLog")]
    public class ListingLogView
    {
        public int ID { get; set; }
        public int ListingID { get; set; }
        [JsonProperty(PropertyName = "note")]
        public string Note { get; set; }
        [JsonProperty(PropertyName = "created")]
        public DateTime Created { get; set; }
        [JsonProperty(PropertyName = "message")]
        public string Message { get; set; }
        [JsonProperty(PropertyName = "userName")]
        public string UserName { get; set; }
    }
    [Table("ListingLog")]
    public class ListingLog
    {
        [JsonProperty(PropertyName = "id")]
        public int ID { get; set; }
        [JsonProperty(PropertyName = "listingID")]
        public int ListingID { get; set; }
        [JsonProperty(PropertyName = "note")]
        public string Note { get; set; }
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        [JsonProperty(PropertyName = "created")]
        public DateTime Created { get; set; }
        [JsonProperty(PropertyName = "msgID")]
        public int MsgID { get; set; }
        public string UserID { get; set; }
    }
    public class eBayBusinessPolicies
    {
        [JsonProperty(PropertyName = "shippingPolicies")]
        public List<ShippingPolicy> ShippingPolicies { get; set; }
        [JsonProperty(PropertyName = "returnPolicies")]
        public List<ReturnPolicy> ReturnPolicies { get; set; }
        [JsonProperty(PropertyName = "paymentPolicies")]
        public List<PaymentPolicy> PaymentPolicies { get; set; }
    }
    public class ShippingPolicy
    {
        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }
        [JsonProperty(PropertyName = "handlingTime")]
        public int HandlingTime { get; set; }
        [JsonProperty(PropertyName = "shippingService")]
        public string ShippingService { get; set; }
        [JsonProperty(PropertyName = "globalShipping")]
        public bool GlobalShipping { get; set; }
    }
    public class PaymentPolicy
    {
        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }
        [JsonProperty(PropertyName = "paypalEmailAddress")]
        public string PaypalEmailAddress { get; set; }
    }
    public class ReturnPolicy
    {
        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }
        [JsonProperty(PropertyName = "shippingCostPaidByOption")]
        public string ShippingCostPaidByOption { get; set; }
    }
}
