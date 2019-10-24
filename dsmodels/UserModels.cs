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

    }

    // ds109 user may use multiple ebay keysets
    [Table("UserProfileKeys")]
    public class UserProfileKeys
    {
        public int ID { get; set; }
        public string UserID { get; set; }
        public string AppID { get; set; }
        public string DevID { get; set; }
        public string CertID { get; set; }
    }

    // ds109 user may be associated with multiple store tokens
    [Table("UserToken")]
    public class UserToken
    {
        [Key]
        [Column(Order = 1)]
        public string UserID { get; set; }
        [Key]
        [Column(Order = 2)]
        public int StoreID { get; set; }
        public string Token { get; set; }
    }

    [Table("UserSettings")]
    public class UserSettings
    {
        [Key]
        [Column(Order = 1)]
        public string UserID { get; set; }
        [Key]
        [Column(Order = 2)]
        public int ApplicationID { get; set; }
        public int StoreID { get; set; }    // What store is user currently working on?
        public int KeysID { get; set; }     // User can use different key sets
    }

    [Table("StoreProfile")]
    public class StoreProfile
    {
        public int ID { get; set; }
        public string StoreName { get; set; }
    }

    // Which stores does user have access to?
    [Table("UserStore")]
    public class UserStore
    {
        public string UserID { get; set; }
        public int StoreID { get; set; }
    }

    /// <summary>
    /// </summary>
    [Table("vwUserSettings")]
    public class UserSettingsView
    {
        [Key]
        [JsonProperty(PropertyName = "userID")]
        public string UserID { get; set; }

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
    }

}
