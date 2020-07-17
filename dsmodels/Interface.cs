using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace dsmodels
{
    public interface ISupplierItem
    {
        int ArrivalDateFlag { get; set; }
        DateTime? Arrives { get; set; }
        int? BusinessDaysArrives { get; set; }
        List<string> CanList { get; set; }
        string Description { get; set; }
        int ID { get; set; }
        bool? IsFreightShipping { get; set; }
        bool? IsVariation { get; set; }
        bool? IsVERO { get; set; }
        string ItemID { get; set; }
        string ItemURL { get; set; }
        string MPN { get; set; }
        bool OutOfStock { get; set; }
        bool ShippingNotAvailable { get; set; }
        bool? SoldAndShippedBySupplier { get; set; }
        int SourceID { get; set; }
        string SupplierBrand { get; set; }
        string SupplierPicURL { get; set; }
        decimal? SupplierPrice { get; set; }
        List<SupplierVariation> SupplierVariation { get; set; }
        string UPC { get; set; }
        DateTime Updated { get; set; }
        List<string> usItemId { get; set; }
        string VariationName { get; set; }
        List<string> VariationPicURL { get; set; }
        List<string> Warning { get; set; }
    }
    public interface IUserSettingsView
    {
        string APIEmail { get; set; }
        string AppID { get; set; }
        int ApplicationID { get; set; }
        AspNetUser AspNetUser { get; set; }
        string CertID { get; set; }
        string DevID { get; set; }
        int ebayKeyID { get; set; }
        int eBayKeyID { get; set; }
        double FinalValueFeePct { get; set; }
        string FirstName { get; set; }
        byte HandlingTime { get; set; }
        bool IsVA { get; set; }
        byte MaxShippingDays { get; set; }
        string PaymentProfile { get; set; }
        string PayPalEmail { get; set; }
        double PctProfit { get; set; }
        bool RepricerEmail { get; set; }
        string ReturnProfile { get; set; }
        bool SalesPermission { get; set; }
        string ShippingProfile { get; set; }
        int StoreID { get; set; }
        string StoreName { get; set; }
        string Token { get; set; }
        string UserID { get; set; }
        string UserName { get; set; }
    }
    public interface ISalesOrder
    {
        string Buyer { get; set; }
        string BuyerHandle { get; set; }
        decimal BuyerPaid { get; set; }
        string BuyerState { get; set; }
        DateTime? CancelClose { get; set; }
        DateTime? CancelOpen { get; set; }
        string CancelStatus { get; set; }
        string CreatedBy { get; set; }
        DateTime DatePurchased { get; set; }
        string eBayOrderNumber { get; set; }
        decimal FinalValueFee { get; set; }
        decimal I_Paid { get; set; }
        int ID { get; set; }
        string ListedItemID { get; set; }
        int ListingID { get; set; }
        string OrderID { get; set; }
        string OrderStatus { get; set; }
        decimal PayPalFee { get; set; }
        decimal Profit { get; set; }
        double ProfitMargin { get; set; }
        int Qty { get; set; }
        DateTime? ReturnClose { get; set; }
        DateTime? ReturnOpen { get; set; }
        string ReturnStatus { get; set; }
        decimal SalesTax { get; set; }
        decimal ShippingCost { get; set; }
        decimal SubTotal { get; set; }
        string SupplierOrderNumber { get; set; }
        decimal Total { get; set; }
        string TrackingNumber { get; set; }
    }
    public interface IShippingPolicy
    {
        bool GlobalShipping { get; set; }
        int HandlingTime { get; set; }
        string Name { get; set; }
        string ShippingService { get; set; }
    }
    public interface IStoreAnalysis
    {
        List<string> DBIsMissingItems { get; set; }
        int InActive { get; set; }
        List<string> QtyMismatch { get; set; }
    }
    public interface IRepository
    {
        //DbSet<AppSettings> AppSettings { get; set; }
        //DbSet<AspNetUser> AspNetUsers { get; set; }
        //DbSet<ListingItemSpecific> ListingItemSpecifics { get; set; }
        //DbSet<ListingLog> ListingLogs { get; set; }
        //DbSet<ListingLogView> ListingLogViews { get; set; }
        //DbSet<ListingNote> ListingNotes { get; set; }
        //DbSet<ListingNoteView> ListingNotesView { get; set; }
        //DbSet<Listing> Listings { get; set; }
        //DbSet<ListingView> ListingsView { get; set; }
        //DbSet<OrderHistory> OrderHistory { get; set; }
        //DbSet<OrderHistoryDetail> OrderHistoryDetails { get; set; }
        //DbSet<OrderHistoryItemSpecific> OrderHistoryItemSpecifics { get; set; }
        //DbSet<SalesOrder> SalesOrders { get; set; }
        //DbSet<SearchHistory> SearchHistory { get; set; }
        //DbSet<SearchHistoryView> SearchHistoryView { get; set; }
        //DbSet<SellerListingItemSpecific> SellerListingItemSpecifics { get; set; }
        //DbSet<SellerListing> SellerListings { get; set; }
        //DbSet<SellerOrderHistory> SellerOrderHistory { get; set; }
        //DbSet<SellerProfile> SellerProfiles { get; set; }
        //DbSet<SourceCategories> SourceCategories { get; set; }
        //DbSet<StoreProfile> StoreProfiles { get; set; }
        //DbSet<SupplierItem> SupplierItems { get; set; }
        //DbSet<UpdateToListing> UpdateToListing { get; set; }
        //DbSet<UserProfileKeys> UserProfileKeys { get; set; }
        //DbSet<UserProfileKeysView> UserProfileKeysView { get; set; }
        //DbSet<UserProfile> UserProfiles { get; set; }
        //DbSet<UserProfileView> UserProfileViews { get; set; }
        //DbSet<UserSettings> UserSettings { get; set; }
        //DbSet<UserSettingsView> UserSettingsView { get; set; }
        //DbSet<UserStore> UserStores { get; set; }
        //DbSet<UserStoreView> UserStoreView { get; set; }
        //DbSet<UserToken> UserTokens { get; set; }
        //DbSet<VEROBrands> VEROBrands { get; set; }

        bool CanRunScan(string userid, string seller);
        string ClearOrderHistory(int rptNumber);
        Task<string> DeleteListingRecordAsync(IUserSettingsView settings, int listingID, bool force);
        void DetachAll();
        void DetachAllEntities();
        DateTime? fromDateToScan(int rptNumber);
        string GetAppSetting(IUserSettingsView settings, string settingName);
        IQueryable<ListingView> GetListingBySupplierURL(int storeID, string URL);
        IQueryable<ListingView> GetListings(int storeID, bool unlisted, bool listed);
        IQueryable<TimesSold> GetSalesData(int rptNumber, DateTime dateFrom, int storeID, string itemID);
        SellerListing GetSellerListing(string itemID);
        List<SellerProfile> GetSellers();
        StoreProfile GetStoreProfile(int storeID);
        ISupplierItem GetSupplierItem(int id);
        ISupplierItem GetSupplierItem(string itemID);
        ISupplierItem GetSupplierItemByURL(string URL);
        string GetToken(int storeID, string userID);
        string GetToken(IUserSettingsView settings);
        string getUrl(int categoryId);
        string GetUserIDFromName(string username);
        UserProfile GetUserProfile(string userid);
        UserProfileKeys GetUserProfileKeys(int id);
        UserProfileKeysView GetUserProfileKeysView(int storeID, string userID);
        UserProfileView GetUserProfileView(string userid);
        IUserSettingsView GetUserSettingsView(string connStr, string userID);
        UserSettingsView GetUserSettingsView(string connStr, string userID, int storeID);
        List<UserStoreView> GetUserStores(string userID);
        Task<bool> HistoryDetailRemove(int rptNumber, DateTime fromDate);
        Task HistoryRemove(string connStr, int rptNumber);
        bool IsVERO(string brand);
        Task<List<ListingNoteView>> ItemNotes(string itemID, int storeID);
        int? LatestRptNumber(string seller);
        Task<bool> ListedItemIDUpdate(Listing listing, string listedItemID, string userId, bool listedWithAPI, string listedResponse, DateTime? updated = null);
        Listing ListingGet(int listingID);
        Listing ListingGet(string listedItemID);
        Listing ListingGet(string itemID, int storeID);
        Task ListingLogAdd(ListingLog log);
        List<ListingLogView> ListingLogGet(int listingID);
        Task<Listing> ListingSaveAsync(IUserSettingsView settings, Listing listing, bool updateItemSpecifics, params string[] changedPropertyNames);
        Task NoteSave(ListingNote note);
        Task<string> OrderHistoryItemSpecificSave(List<OrderHistoryItemSpecific> specifics);
        void OrderHistoryItemSpecificUpdate(OrderHistoryItemSpecific specific);
        string OrderHistorySave(OrderHistory oh, DateTime fromDate);
        void OrderHistoryUpdate(OrderHistory orderHistory, params string[] changedPropertyNames);
        string ProdIDExists(string UPC, string MPN, int storeID);
        bool SalesExists(int listingID);
        Task<SalesOrder> SalesOrderAddAsync(SalesOrder salesOrder);
        bool SalesOrderExists(string supplierOrderNumber);
        Task SalesOrderSaveAsync(IUserSettingsView settings, SalesOrder salesOrder, params string[] changedPropertyNames);
        Task<SearchHistory> SearchHistoryAdd(SearchHistory sh);
        SearchHistory SearchHistoryUpdate(SearchHistory sh, params string[] changedPropertyNames);
        bool SellerExists(string seller);
        Task<string> SellerListingItemSpecificSave(SellerListing sellerListing);
        void SellerListingItemSpecificUpdate_notused(SellerListingItemSpecific specific);
        Task<SellerProfile> SellerProfileGet(string seller);
        Task SellerProfileSave(SellerProfile sellerProfile, params string[] changedPropertyNames);
        int SourceIDFromCategory(int categoryId);
        Task StoreAddAsync(string userID, StoreProfile profile);
        Task StoreProfileAddAsync(StoreProfile profile);
        Task StoreProfileUpdate(StoreProfile storeProfile, params string[] changedPropertyNames);
        Task<string> SupplierItemDelete(IUserSettingsView settings, int ID);
        ISupplierItem SupplierItemGet(int ID);
        void SupplierItemUpdateByID(SupplierItem item, params string[] changedPropertyNames);
        void SupplierItemUpdateByProdID(string UPC, string MPN, SupplierItem item, params string[] changedPropertyNames);
        Task<bool> UpdatePrice(Listing listing, decimal price, decimal supplierPrice);
        Task UpdateToListingRemove(UpdateToListing obj);
        Task<string> UpdateToListingSave(UpdateToListing updateToList, params string[] changedPropertyNames);
        Task<string> UserProfileDeleteAsync(UserProfile profile);
        Task<UserProfileKeys> UserProfileKeysUpdate(UserProfileKeys keys, params string[] changedPropertyNames);
        Task UserProfileSaveAsync(UserProfile profile, params string[] changedPropertyNames);
        Task<UserSettingsView> UserSettingsSaveAsync(string connStr, UserSettings settings, params string[] changedPropertyNames);
        Task UserStoreAddAsync(UserStore userStore);
        Task UserTokenUpdate(UserToken userToken, params string[] changedPropertyNames);
        DataContext Context { get; set; }
    }
    public interface IDashboard
    {
        int Listed { get; set; }
        int NotListed { get; set; }
        int OOS { get; set; }
        double? RepricerElapsedTime { get; set; }
        DateTime? RepricerLastRan { get; set; }
    }
}