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
    [Table("AspNetUsers")]
    public class AspNetUser
    {
        public string Id { get; set; }
        public string UserName { get; set; }
    }

    [Table("vwUserProfile")]
    public class UserProfileView
    {
        [Key]
        [JsonProperty(PropertyName = "userID")]
        public string UserID { get; set; }
        [JsonProperty(PropertyName = "firstname")]
        public string Firstname { get; set; }
        [JsonProperty(PropertyName = "lastname")]
        public string Lastname { get; set; }
        public DateTime Created { get; set; }
        //[ForeignKey("UserID")]
        //public AspNetUser AspNetUser { get; set; }
        [JsonProperty(PropertyName = "selectedStore")]
        public int? SelectedStore { get; set; }
        [JsonProperty(PropertyName = "username")]
        public string UserName { get; set; }
        [JsonProperty(PropertyName = "isVA")]
        public bool IsVA { get; set; }
        public bool RepricerEmail { get; set; }

    }

    /// <summary>
    /// Augment AspNetUsers - one to one
    /// </summary>
    [Table("UserProfile")]
    public class UserProfile
    {
        [Key]
        [JsonProperty(PropertyName = "userID")]
        public string UserID { get; set; }
        [JsonProperty(PropertyName = "firstname")]
        public string Firstname { get; set; }
        [JsonProperty(PropertyName = "lastname")]
        public string Lastname { get; set; }
        public DateTime Created { get; set; }
        //[ForeignKey("UserID")]
        //public AspNetUser AspNetUser { get; set; }
        [JsonProperty(PropertyName = "selectedStore")]
        public int? SelectedStore { get; set; }
        [JsonProperty(PropertyName = "isVA")]
        public bool IsVA { get; set; }
        public bool RepricerEmail { get; set; }
    }

    // ds109 user may use multiple ebay keysets
    [Table("eBayKeys")]
    public class UserProfileKeys
    {
        [JsonProperty(PropertyName = "id")]
        public int ID { get; set; }
        [JsonProperty(PropertyName = "appID")]
        public string AppID { get; set; }
        [JsonProperty(PropertyName = "devID")]
        public string DevID { get; set; }
        [JsonProperty(PropertyName = "certID")]
        public string CertID { get; set; }
        [JsonProperty(PropertyName = "APIEmail")]
        public string EmailAddress { get; set; }
    }
    /// <summary>
    /// Created to get a store when no settings have been created - just have keys
    /// </summary>
    [Table("vwUserProfileKeys")]
    public class UserProfileKeysView
    {
        [JsonProperty(PropertyName = "id")]
        public int ID { get; set; }
        [Key]
        [Column(Order = 1)]

        [JsonProperty(PropertyName = "userID")]
        public string UserID { get; set; }
        [Key]
        [Column(Order = 2)]
        [JsonProperty(PropertyName = "storeID")]
        public int StoreID { get; set; }
        [JsonProperty(PropertyName = "appID")]
        public string AppID { get; set; }
        [JsonProperty(PropertyName = "devID")]
        public string DevID { get; set; }
        [JsonProperty(PropertyName = "certID")]
        public string CertID { get; set; }
        [JsonProperty(PropertyName = "token")]
        public string Token { get; set; }
    }

    /// <summary>
    /// ds109 user may be associated with multiple store tokens 
    /// 01.22.2020 what exactly do I mean by this?  Why do I associate a user with a token?
    /// </summary>
    [Table("UserToken")]
    public class UserToken
    {
        // Not exactly sure I like userID being part of token or even calling this USerToken.

        [Key]
        [Column(Order = 1)]
        public string UserID { get; set; }
        [Key]
        [Column(Order = 2)]
        public int StoreID { get; set; }
        public string Token { get; set; }
        public int KeysID { get; set; }
    }

    /// <summary>
    /// Current user settings
    /// </summary>
    [Table("UserSettings")]
    public class UserSettings
    {
        [Key]
        [Column(Order = 1)]
        [JsonProperty(PropertyName = "userID")]
        public string UserID { get; set; }
        [JsonProperty(PropertyName = "applicationID")]
        public int ApplicationID { get; set; }
        [Key]
        [Column(Order = 2)]
        [JsonProperty(PropertyName = "storeID")]
        public int StoreID { get; set; }    // What store is user currently working on?
        [JsonProperty(PropertyName = "keysID")]
        public int KeysID { get; set; }     // User can use different key sets
        [ForeignKey("UserID")]
        public AspNetUser AspNetUser { get; set; }
        [JsonProperty(PropertyName = "pctProfit")]
        public double PctProfit { get; set; }
        [JsonProperty(PropertyName = "handlingTime")]
        public byte HandlingTime { get; set; }
        [JsonProperty(PropertyName = "payPalEmail")]
        public string PayPalEmail { get; set; }
        [JsonProperty(PropertyName = "shippingProfile")]
        public string ShippingProfile { get; set; }
        [JsonProperty(PropertyName = "paymentProfile")]
        public string PaymentProfile { get; set; }
        [JsonProperty(PropertyName = "returnProfile")]
        public string ReturnProfile { get; set; }
        [JsonProperty(PropertyName = "maxShippingDays")]
        public byte MaxShippingDays { get; set; }
        [JsonProperty(PropertyName = "finalValueFeePct")]
        public double FinalValueFeePct { get; set; }


    }
    /// <summary>
    /// </summary>
    [Table("vwUserSettings")]
    public class UserSettingsView
    {
        [Key]
        [Column(Order = 1)]
        [JsonProperty(PropertyName = "userID")]
        public string UserID { get; set; }
        [JsonProperty(PropertyName = "applicationID")]
        public int ApplicationID { get; set; }
        [Key]
        [Column(Order = 2)]
        [JsonProperty(PropertyName = "storeID")]
        public int StoreID { get; set; }    // What store is user currently working on?
        [JsonProperty(PropertyName = "keysID")]
        public int eBayKeyID { get; set; }     // User can use different key sets
        [ForeignKey("UserID")]
        public AspNetUser AspNetUser { get; set; }

        [JsonProperty(PropertyName = "userName")]
        public string UserName { get; set; }

        [JsonProperty(PropertyName = "appID")]
        public string AppID { get; set; }

        [JsonProperty(PropertyName = "certID")]
        public string CertID { get; set; }

        [JsonProperty(PropertyName = "devID")]
        public string DevID { get; set; }

        [JsonProperty(PropertyName = "token")]
        public string Token { get; set; }

        [JsonProperty(PropertyName = "storeName")]
        public string StoreName { get; set; }

        [JsonProperty(PropertyName = "firstName")]
        public string FirstName { get; set; }
        [JsonProperty(PropertyName = "pctProfit")] 
        public double PctProfit { get; set; }
        [JsonProperty(PropertyName = "handlingTime")]
        public byte HandlingTime { get; set; }
        [JsonProperty(PropertyName = "maxShippingDays")]
        public byte MaxShippingDays { get; set; }
        [JsonProperty(PropertyName = "payPalEmail")]
        public string PayPalEmail { get; set; }

        [JsonProperty(PropertyName = "paymentProfile")]
        public string PaymentProfile { get; set; }
       
        [JsonProperty(PropertyName = "shippingProfile")]
        public string ShippingProfile { get; set; }
        
        [JsonProperty(PropertyName = "returnProfile")]
        public string ReturnProfile { get; set; }
        
        public int ebayKeyID { get; set; }
        [JsonProperty(PropertyName = "APIEmail")]
        public string APIEmail { get; set; }
        [JsonProperty(PropertyName = "finalValueFeePct")]
        public double FinalValueFeePct { get; set; }
        [JsonProperty(PropertyName = "isVA")]
        public bool IsVA { get; set; }
        public bool RepricerEmail { get; set; }

    }

    /// <summary>
    /// eBay user might not have paid for a store and technically does not have an eBay subscription.
    /// So you might even refer to StoreProfile instead as AccountProfile but it's very easy to think of everything as a "store".
    /// In the database, eBayUserID is non-nullable but StoreName allows nulls.  If user does not have a subscription then he does not have a "store".
    /// 
    /// </summary>
    [Table("StoreProfile")]
    public class StoreProfile
    {
        [JsonProperty(PropertyName = "id")]
        public int ID { get; set; }
        [JsonProperty(PropertyName = "storeName")]
        public string StoreName { get; set; }   // User may not have a store subscription (no store name) in which case use user handle 
        [JsonProperty(PropertyName = "listingLimit")]
        public int ListingLimit { get; set; }
        [JsonProperty(PropertyName = "eBayUserID")]
        public string eBayUserID { get; set; }
        public DateTime RepricerLastRan { get; set; }
    }
    public class eBayStore
    {
        [JsonProperty(PropertyName = "storeName")]
        public string StoreName { get; set; }
        [JsonProperty(PropertyName = "subscription")]
        public string Subscription { get; set; }
    }
    public class eBayUser
    {
        [JsonProperty(PropertyName = "eBayUserID")]
        public string eBayUserID { get; set; }
        [JsonProperty(PropertyName = "payPalEmail")]
        public string PayPalEmail { get; set; } // specifies the default email address the seller uses for receiving PayPal payments.
    }

    /// <summary>
    /// Should listing note be linked to store?
    /// Recall item can go in multiple stores - argue both ways - right now note is linked to store.
    /// </summary>
    [Table("ListingNote")]
    public class ListingNote
    {
        [Key]
        [JsonProperty(PropertyName = "id")]
        public int ID { get; set; }
        [JsonProperty(PropertyName = "storeID")]
        public int StoreID { get; set; }
        [JsonProperty(PropertyName = "itemID")]
        public string ItemID { get; set; }
        [JsonProperty(PropertyName = "note")]
        public string Note { get; set; }
        [JsonProperty(PropertyName = "userID")]
        public string UserID { get; set; }
        [JsonProperty(PropertyName = "updated")]
        public DateTime Updated { get; set; }
    }
    [Table("vwListingNote")]
    public class ListingNoteView : ListingNote
    {
        [JsonProperty(PropertyName = "userName")]
        public string UserName { get; set; }
    }
    /// <summary>
    /// Which stores does user have access to?
    /// As user selects, update StoreID in UserSettings
    /// </summary>
    [Table("UserStore")]
    public class UserStore
    {
        [Key]
        [Column(Order = 1)]
        public string UserID { get; set; }
        [Key]
        [Column(Order = 2)]
        public int StoreID { get; set; }
    }
    [Table("vwUserStore")]
    public class UserStoreView
    {
        [Key]
        [Column(Order = 1)]
        [JsonProperty(PropertyName = "userID")]
        public string UserID { get; set; }
        [Key]
        [Column(Order = 2)]
        [JsonProperty(PropertyName = "storeID")]
        public int StoreID { get; set; }
        [JsonProperty(PropertyName = "storeName")]
        public string StoreName { get; set; }
        [JsonProperty(PropertyName = "eBayUserID")]
        public string eBayUserID { get; set; }
    }

}

