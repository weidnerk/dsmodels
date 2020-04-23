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
        public int SelectedStore { get; set; }
    }

    // ds109 user may use multiple ebay keysets
    [Table("eBayKeys")]
    public class UserProfileKeys
    {
        public int ID { get; set; }
        public string AppID { get; set; }
        public string DevID { get; set; }
        public string CertID { get; set; }
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
    }
    /// <summary>
    /// </summary>
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
        public int KeysID { get; set; }     // User can use different key sets
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
        public byte MaxShippingDays { get; set; }
        [JsonProperty(PropertyName = "payPalEmail")]
        public string PayPalEmail { get; set; }
        public string PaymentProfile { get; set; }
        public string ShippingProfile { get; set; }
        public string ReturnProfile { get; set; }

    }

    [Table("StoreProfile")]
    public class StoreProfile
    {
        public int ID { get; set; }
        public string StoreName { get; set; }
        [JsonProperty(PropertyName = "listingLimit")]
        public int ListingLimit { get; set; }

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
    }

}

