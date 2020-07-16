using System;
using System.Collections.Generic;

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
}