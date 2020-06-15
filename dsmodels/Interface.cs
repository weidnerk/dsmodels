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
}