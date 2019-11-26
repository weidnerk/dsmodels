using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dsmodels
{
    public class DataModelsDB : DbContext
    {
        readonly static string _logfile = "log.txt";
        static DataModelsDB()
        {
            //do not try to create a database 
            Database.SetInitializer<DataModelsDB>(null);
        }

        public DataModelsDB()
            : base("name=OPWContext")
        {
        }
        public DbSet<Listing> Listings { get; set; }
        public DbSet<ListingView> ListingsView { get; set; }
        public DbSet<ItemSpecific> ItemSpecifics { get; set; }

        public DbSet<SourceCategories> SourceCategories { get; set; }
        public DbSet<SearchHistory> SearchHistory { get; set; }
        public DbSet<OrderHistory> OrderHistory { get; set; }
        public DbSet<OrderHistoryDetail> OrderHistoryDetails { get; set; }
        public DbSet<UserSettings> UserSettings { get; set; }   // user's current selection
        //public DbSet<UserSettingsView> UserSettingsView { get; set; }
        public DbSet<UserProfile> UserProfiles { get; set; }
        public DbSet<AspNetUser> AspNetUsers { get; set; }
        public DbSet<SellerProfile> SellerProfiles { get; set; }
        public DbSet<StoreProfile> StoreProfiles { get; set; }
        public DbSet<ListingNote> ListingNotes { get; set; }
        public DbSet<ListingNoteView> ListingNotesView { get; set; }
        public DbSet<SellerOrderHistory> SellerOrderHistory { get; set; }
        public DbSet<SearchHistoryView> SearchHistoryView { get; set; }
        public DbSet<SearchReport> SearchResults { get; set; }

        public string GetUserIDFromName(string username)
        {
            var id = this.AspNetUsers.Where(r => r.UserName == username).Select(s => s.Id).First();
            return id;
        }

        /// <summary>
        /// return appids for a user
        /// </summary>
        /// <param name="userid"></param>
        /// <returns></returns>
        //public List<AppIDSelect> GetAppIDs(string userid)
        //{
        //    var x = from a in this.UserSettingsView
        //            where a.UserID == userid
        //            select new AppIDSelect
        //            {
        //                value = a.AppID,
        //                viewValue = a.AppID

        //            };
        //    return x.ToList();
        //}

        public UserSettings GetUserSetting(string userid)
        {
            var setting = this.UserSettings.Find(userid, 1);
            return setting;
        }

        /// <summary>
        /// Return user profile based in his appID setting in UserSetting
        /// </summary>
        /// <param name="userid"></param>
        /// <returns></returns>
        public UserSettingsView GetUserSettings(string connStr, string userid)
        {
            try
            {
                // match composite key, UserId/ApplicationID; ApplicationID=1 for ds109
                //var setting = this.UserSettingsView.Find(userid);
                var setting = GetUserSetting(connStr, userid, 1);
                if (setting != null)
                {
                    return setting;
                }
                return null;
            }
            catch (Exception exc)
            {
                return null;
            }
        }

        public UserProfile GetUserProfile(string userid)
        {
            var profile = this.UserProfiles.Where(r => r.UserID == userid).First();
            return profile;
        }

        public int SourceIDFromCategory(int categoryId)
        {
            int sourceId = 0;
            var category = this.SourceCategories.Where(r => r.ID == categoryId).FirstOrDefault();
            if (category != null)
                sourceId = category.SourceID;
            return sourceId;
        }

        public async Task<Listing> GetPostedListing(int sourceID, string supplierItemID)
        {
            var found = await this.Listings.FirstOrDefaultAsync(r => r.SourceID == sourceID && r.SupplierItemID == supplierItemID);
            return found;
        }

        public async Task<Listing> GetPostedListing(string itemId)
        {
            var found = await this.Listings.FirstOrDefaultAsync(r => r.ListedItemID == itemId);
            return found;
        }

        // listedItemId is my id
        public async Task<Listing> GetPostedListingFromListId(string listedItemId)
        {
            var found = await this.Listings.FirstOrDefaultAsync(r => r.ListedItemID == listedItemId);
            return found;
        }

        //public List<StagedListing> GetToList(int categoryId, int listedQty)
        //{
        //    List<StagedListing> data =
        //        Database.SqlQuery<StagedListing>(
        //        "select * from dbo.fnToListFinal(@categoryId, @listedQty)",
        //        new SqlParameter("@categoryId", categoryId),
        //        new SqlParameter("@listedQty", listedQty))
        //    .ToList();
        //    return data;
        //}
        /*
        public async Task PostedListingSaveAsync(Listing listing)
        {
            var found = await this.Listings.FirstOrDefaultAsync(r => r.SourceID == listing.SourceID && r.SupplierItemID == listing.SupplierItemID);
            if (found == null)
                Listings.Add(listing);
            else
            {
                //found.EbaySeller = listing.EbaySeller;
                //found.Price = listing.Price;
                //found.CategoryID = listing.CategoryID;
                found.SupplierItemID = listing.SupplierItemID;
                //found.SourceUrl = listing.SourceUrl;
                //found.Pictures = listing.Pictures;
                found.Title = listing.Title;
                found.Title = listing.Title;
                found.EbayUrl = listing.EbayUrl;
                found.PrimaryCategoryID = listing.PrimaryCategoryID;
                found.PrimaryCategoryName = listing.PrimaryCategoryName;
                found.Description = listing.Description;
                found.SourceID = listing.SourceID;  // 1=sams
                //found.ListedQty = listing.ListedQty;
                this.Entry(found).State = EntityState.Modified;
            }
            await this.SaveChangesAsync();
        }
        */
        public async Task<bool> UpdatePrice(Listing listing, decimal price, decimal supplierPrice)
        {
            bool ret = false;
            var rec = await this.Listings.FirstOrDefaultAsync(r => r.ListedItemID == listing.ListedItemID);
            if (rec != null)
            {
                ret = true;
                rec.ListingPrice = price;
                rec.SupplierPrice = supplierPrice;
                rec.Updated = DateTime.Now;

                using (var context = new DataModelsDB())
                {
                    // Pass the entity to Entity Framework and mark it as modified
                    context.Entry(rec).State = EntityState.Modified;
                    context.SaveChanges();
                }
            }
            return ret;
        }

        public string getUrl(int categoryId)
        {
            string url = null;
            var r = this.SourceCategories.Find(categoryId);
            if (r != null)
                url = r.URL;
            return url;
        }

        public async Task<SearchHistory> SearchHistoryAdd(SearchHistory sh)
        {
            try
            {
                SearchHistory.Add(sh);
                await this.SaveChangesAsync();
                return sh;
            }
            catch (Exception exc)
            {
            }
            return null;
        }
        public async Task<SearchHistory> SearchHistoryUpdate(SearchHistory sh)
        {
            try
            {
                this.Entry(sh).State = EntityState.Modified;
                await this.SaveChangesAsync();
            }
            catch (Exception exc)
            {
            }
            return null;
        }

        public async Task HistoryRemove(int rptNumber)
        {
            try
            {
                var parent = this.OrderHistory.Include(p => p.OrderHistoryDetails)
                    .Where(p => p.RptNumber == rptNumber).ToList();

                foreach(var p in parent)
                {
                    foreach (var child in p.OrderHistoryDetails.ToList())
                    {
                        this.OrderHistoryDetails.Remove(child);
                    }
                }

                this.OrderHistory.RemoveRange(this.OrderHistory.Where(x => x.RptNumber == rptNumber));
                await this.SaveChangesAsync();

                var sh = new SearchHistory() { ID = rptNumber };
                this.SearchHistory.Attach(sh);
                this.SearchHistory.Remove(sh);
                await this.SaveChangesAsync();
            }
            catch (Exception exc)
            {
            }
        }

        public async Task DeleteListingRecord(string sellerItemId)
        {
            try
            {
                // first remove item specifics
                this.ItemSpecifics.RemoveRange(this.ItemSpecifics.Where(x => x.SellerItemID == sellerItemId));
                await this.SaveChangesAsync();

                var sh = new Listing() { ItemID = sellerItemId };
                this.Listings.Attach(sh);
                this.Listings.Remove(sh);
                await this.SaveChangesAsync();
            }
            catch (Exception exc)
            {
            }
        }

        //public async Task AppIDRemove(string appID)
        //{
        //    try
        //    {
        //        this.UserSettingsView.RemoveRange(this.UserSettingsView.Where(x => x.AppID == appID));
        //        await this.SaveChangesAsync();
        //    }
        //    catch (Exception exc)
        //    {
        //    }
        //}

        public string OrderHistorySave(OrderHistory oh)
        {
            string ret = string.Empty;
            try
            {
                // Looks like case where variations on same listing sold and returned individually by GetCompletedItems
                // by I will get an error trying to save the same itemId/rptNumber so remove 
                var itemExists = OrderHistory.SingleOrDefault(r => r.ItemID == oh.ItemID && r.RptNumber == oh.RptNumber);
                if (itemExists == null)
                {
                    OrderHistory.Add(oh);
                    this.SaveChanges();
                }
            }
            catch (DbEntityValidationException e)
            {
                ret = GetValidationErr(e);
                dsutil.DSUtil.WriteFile(_logfile, "OrderHistorySave: " + oh.ItemID + " " + ret, "admin");
            }
            catch (Exception exc)
            {
                string msg = dsutil.DSUtil.ErrMsg("OrderHistorySave", exc);
                dsutil.DSUtil.WriteFile(_logfile, "OrderHistorySave: " + oh.ItemID + " " + msg, "admin");
                ret += exc.Message;
            }
            return ret;
        }

        protected static string GetValidationErr(DbEntityValidationException e)
        {
            string ret = null;
            foreach (var eve in e.EntityValidationErrors)
            {
                ret = string.Format("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:\n", eve.Entry.Entity.GetType().Name, eve.Entry.State);
                foreach (var ve in eve.ValidationErrors)
                {
                    ret += string.Format("- Property: \"{0}\", Error: \"{1}\"\n", ve.PropertyName, ve.ErrorMessage);
                }
            }
            return ret;
        }
        //public string OrderHistoryDetailSave(List<OrderHistoryDetail> oh, int id)
        //{
        //    string ret = string.Empty;
        //    try
        //    {
        //        foreach (OrderHistoryDetail item in oh)
        //        {
        //            item.OrderHistoryID = id;
        //            OrderHistoryDetails.Add(item);
        //        }
        //        this.SaveChanges();
        //    }
        //    catch (DbEntityValidationException e)
        //    {
        //        foreach (var eve in e.EntityValidationErrors)
        //        {
        //            ret = string.Format("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:\n", eve.Entry.Entity.GetType().Name, eve.Entry.State);
        //            foreach (var ve in eve.ValidationErrors)
        //            {
        //                ret += string.Format("- Property: \"{0}\", Error: \"{1}\"\n", ve.PropertyName, ve.ErrorMessage);
        //            }
        //        }
        //    }
        //    catch (Exception exc)
        //    {
        //        ret = exc.Message;
        //    }
        //    return ret;
        //}

        public async Task<string> ItemSpecificSave(List<ItemSpecific> specifics)
        {
            string output = null;
            string itemID = null;
            try
            {
                // don't replace a UPC or MPN with another seller's value of 'Does not apply'
                bool ret = specifics.Remove(specifics.SingleOrDefault(p => p.ItemName == "UPC" && p.ItemValue.ToUpper() == "DOES NOT APPLY"));
                ret = specifics.Remove(specifics.SingleOrDefault(p => p.ItemName == "MPN" && p.ItemValue.ToUpper() == "DOES NOT APPLY"));

                // i've also seen seller's use underscores in MPN - is that a valid character in a Walmart MPN?

                itemID = specifics[0].SellerItemID;
                var found = await this.ItemSpecifics.FirstOrDefaultAsync(p => p.SellerItemID == itemID);
                if (found != null)
                {
                    this.ItemSpecifics.RemoveRange(this.ItemSpecifics.Where(x => x.SellerItemID == itemID));
                }
                foreach (var item in specifics)
                {
                    this.ItemSpecifics.Add(item);
                }
                await this.SaveChangesAsync();
            }
            catch (DbEntityValidationException e)
            {
                output = GetValidationErr(e);
                dsutil.DSUtil.WriteFile(_logfile, "ItemSpecificSave: " + itemID  + " " + output, "admin");

                output = DumpItemSpecifics(specifics);
                dsutil.DSUtil.WriteFile(_logfile, output, "admin");
            }
            catch (Exception exc)
            {
                output = exc.Message;
                string msg = dsutil.DSUtil.ErrMsg("ItemSpecificSave", exc);
                dsutil.DSUtil.WriteFile(_logfile, "ItemSpecificSave: " + itemID + " " + msg, "admin");

                output = DumpItemSpecifics(specifics);
                dsutil.DSUtil.WriteFile(_logfile, output, "admin");
            }
            return output;
        }
        public static string DumpItemSpecifics(List<ItemSpecific> specifics)
        {
            string output = null;
            foreach (var spec in specifics)
            {
                output += "ItemName: " + spec.ItemName + " -> " + spec.ItemValue + "\n";
            }
            return output;
        }

        public async Task ListingSave(Listing listing, string userID)
        {
            try
            {
                /*
                var specifics = new List<ItemSpecific>();

                // i don't know how long these values can be - match max widths in ItemSpecific table
                foreach (ItemSpecific specific in listing.ItemSpecifics)
                {
                    var new_specific = new ItemSpecific();
                    if (specific.ItemName.Length > 100)
                    {
                        new_specific.ItemName = specific.ItemName.Substring(0, 100);
                    }
                    else
                    {
                        new_specific.ItemName = specific.ItemName;
                    }
                    if (specific.ItemValue.Length > 300)
                    {
                        new_specific.ItemValue = specific.ItemValue.Substring(0, 300);
                    }
                    else
                    {
                        new_specific.ItemValue = specific.ItemValue;
                    }
                    new_specific.ItemValue = specific.ItemValue;
                    specifics.Add(new_specific);
                }
                listing.ItemSpecifics = specifics;
                */

                // var found = await this.Listings.Include(x => x.ItemSpecifics.Select(y => y.Listing)).FirstOrDefaultAsync(r => r.ItemId == listing.ItemId);
                var found = await this.Listings.FirstOrDefaultAsync(r => r.ItemID == listing.ItemID);
                if (found == null)
                {
                    listing.Created = DateTime.Now;
                    listing.CreatedBy = userID;
                    Listings.Add(listing);
                }
                else
                {
                    this.Entry(found).State = EntityState.Modified;

                    // https://stackoverflow.com/questions/10822656/entity-framework-include-multiple-levels-of-properties
                    // this.Entry(found).Property(e => e.ItemSpecifics.Select(y => y.Listing)).IsModified = true;

                    found.SupplierPrice = listing.SupplierPrice;
                    found.ListingPrice = listing.ListingPrice;
                    if (listing.SourceID.HasValue)
                    {
                        found.SourceID = listing.SourceID;
                    }

                    // User may have just copied in supplier URL and clicked Save in which case we don't have pictures
                    // but can't see storing picture urls each time, so check first
                    if (string.IsNullOrEmpty(found.PictureUrl))
                    {
                        found.PictureUrl = listing.PictureUrl;
                    }
                    found.PictureUrl = listing.PictureUrl;
                    // found.Title = listing.Title;
                    found.ListingTitle = listing.ListingTitle;
                    // found.EbayUrl = listing.EbayUrl;
                    // found.PrimaryCategoryID = listing.PrimaryCategoryID;
                    // found.PrimaryCategoryName = listing.PrimaryCategoryName;
                    found.Description = listing.Description;
                    // found.SourceID = listing.SourceID;
                    found.Qty = listing.Qty;
                    // found.Seller = listing.Seller;
                    // found.ItemSpecifics = listing.ItemSpecifics; // store when created, don't need to update
                    found.Profit = listing.Profit;
                    found.ProfitMargin = listing.ProfitMargin;

                    if (listing.CheckCategory.HasValue)
                    {
                        found.CheckCategory = listing.CheckCategory;
                    }
                    if (listing.CheckCompetition.HasValue)
                    {
                        found.CheckCompetition = listing.CheckCompetition;
                    }
                    if (listing.CheckMainCompetitor.HasValue)
                    {
                        found.CheckMainCompetitor = listing.CheckMainCompetitor;
                    }
                    if (listing.CheckSellerShipping.HasValue)
                    {
                        found.CheckSellerShipping = listing.CheckSellerShipping;
                    }
                    if (listing.CheckShipping.HasValue)
                    {
                        found.CheckShipping = listing.CheckShipping;
                    }
                    if (listing.CheckSource.HasValue)
                    {
                        found.CheckSource = listing.CheckSource;
                    }
                    if (listing.CheckSupplierItem.HasValue)
                    {
                        found.CheckSupplierItem = listing.CheckSupplierItem;
                    }
                    if (listing.CheckSupplierPics.HasValue)
                    {
                        found.CheckSupplierPics = listing.CheckSupplierPics;
                    }
                    if (listing.CheckSupplierPrice.HasValue)
                    {
                        found.CheckSupplierPrice = listing.CheckSupplierPrice;
                    }
                    if (listing.CheckVero.HasValue)
                    {
                        found.CheckVero = listing.CheckVero;
                    }
                    if (listing.CheckIsVariation.HasValue)
                    {
                        found.CheckIsVariation = listing.CheckIsVariation;
                    }
                    if (listing.CheckVariationURL.HasValue)
                    {
                        found.CheckVariationURL = listing.CheckVariationURL;
                    }
                    found.Updated = DateTime.Now;
                    found.UpdatedBy = userID;
                }
                await this.SaveChangesAsync();
            }
            catch (Exception exc)
            {
                string msg = dsutil.DSUtil.ErrMsg("", exc);
            }
        }
        public async Task NoteSave(ListingNote note)
        {
            try
            {
                note.Updated = DateTime.Now;
                ListingNotes.Add(note);
                await this.SaveChangesAsync();
            }
            catch (Exception exc)
            {
                string msg = dsutil.DSUtil.ErrMsg("NoteSave", exc);
            }
        }
        public async Task<List<ListingNoteView>> ItemNotes(string itemID, int storeID)
        {
            var notes = await this.ListingNotesView.Where(p => p.ItemID == itemID && p.StoreID == storeID).OrderBy(o => o.Updated).ToListAsync();
            return notes;
        }
        public async Task OOSSave(Listing listing)
        {
            try
            {
                var found = await this.Listings.FirstOrDefaultAsync(r => r.ItemID == listing.ItemID);
                if (found == null)
                {
                    // error
                    //   Listings.Add(listing);
                }
                else
                {
                    found.OOS = listing.OOS;
                    found.Updated = DateTime.Now;
                    this.Entry(found).State = EntityState.Modified;
                }
                await this.SaveChangesAsync();
            }
            catch (Exception exc)
            {
                string msg = dsutil.DSUtil.ErrMsg("", exc);
            }
        }
        public async Task SellerProfileSave(SellerProfile sellerProfile)
        {
            try
            {
                var found = await this.SellerProfiles.FirstOrDefaultAsync(r => r.Seller == sellerProfile.Seller);
                if (found == null)
                {
                    sellerProfile.Updated = DateTime.Now;
                    SellerProfiles.Add(sellerProfile);
                }
                else
                {
                    found.Note = sellerProfile.Note;
                    found.Updated = DateTime.Now;
                    found.UpdatedBy = sellerProfile.UpdatedBy;
                    this.Entry(found).State = EntityState.Modified;
                }
                await this.SaveChangesAsync();
            }
            catch (Exception exc)
            {
                string msg = dsutil.DSUtil.ErrMsg("", exc);
            }
        }
        public List<ListingView> GetListings(int storeID)
        {
            var found = this.ListingsView.Where(p => p.StoreID == storeID).ToList();
            return found;
        }

        public async Task<Listing> GetListing(string itemId)
        {
            var found = await this.Listings.FirstOrDefaultAsync(r => r.ItemID == itemId);
            return found;
        }
        public async Task<Listing> ListingGet(string itemId)
        {
            try
            {
                var listing = await this.Listings.Include(p => p.SellerListing).FirstOrDefaultAsync(r => r.ItemID == itemId);
                return listing;
            }
            catch (Exception exc)
            {
                string msg = dsutil.DSUtil.ErrMsg("", exc);
                dsutil.DSUtil.WriteFile(_logfile, msg, "admin");
                return null;
            }
        }

        public async Task<SellerProfile> SellerProfileGet(string seller)
        {
            try
            {
                var sellerprofile = await this.SellerProfiles.FirstOrDefaultAsync(r => r.Seller == seller);
                return sellerprofile;
            }
            catch (Exception exc)
            {
                string msg = dsutil.DSUtil.ErrMsg("", exc);
                dsutil.DSUtil.WriteFile(_logfile, msg, "admin");
                return null;
            }
        }

        public async Task<bool> UpdateListedItemID(Listing listing, string listedItemID, string userId, bool listedWithAPI, string listedResponse, DateTime? updated = null)
        {
            string errStr = null; 
            bool ret = false;
            try
            {
                // find item by looking up seller's listing id
                var rec = await this.Listings.FirstOrDefaultAsync(r => r.ItemID == listing.ItemID);
                if (rec != null)
                {
                    ret = true;
                    rec.ListedItemID = listedItemID;
                    if (updated.HasValue)
                    {
                        rec.ListedUpdatedBy = userId;
                        rec.ListedUpdated = DateTime.Now;
                    }
                    else
                    {
                        rec.ListedBy = userId;
                        rec.Listed = listing.Listed;
                    }
                    
                    rec.ListedWithAPI = listedWithAPI;
                    rec.ListedResponse = listedResponse;

                    // Pass the entity to Entity Framework and mark it as modified
                    this.Entry(rec).State = EntityState.Modified;
                    this.SaveChanges();
                }
            }
            catch (DbEntityValidationException e)
            {
                foreach (var eve in e.EntityValidationErrors)
                {
                    errStr = string.Format("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:\n", eve.Entry.Entity.GetType().Name, eve.Entry.State);
                    foreach (var ve in eve.ValidationErrors)
                    {
                        errStr += string.Format("- Property: \"{0}\", Error: \"{1}\"\n", ve.PropertyName, ve.ErrorMessage);
                    }
                }
                dsutil.DSUtil.WriteFile(_logfile, errStr, "admin");
            }
            catch (Exception exc)
            {
                errStr = dsutil.DSUtil.ErrMsg("UpdateListedItemID", exc);
                dsutil.DSUtil.WriteFile(_logfile, errStr, "admin");
            }
            return ret;
        }

        public async Task<bool> UpdateOOS(string listedItemID, bool OOS)
        {
            bool ret = false;
            // find item by looking up seller's listing id
            var rec = await this.Listings.FirstOrDefaultAsync(r => r.ListedItemID == listedItemID);
            if (rec != null)
            {
                ret = true;
                rec.OOS = OOS;

                // Pass the entity to Entity Framework and mark it as modified
                this.Entry(rec).State = EntityState.Modified;
                this.SaveChanges();
            }
            return ret;
        }

        public async Task<bool> UpdateRemovedDate(Listing listing)
        {
            bool ret = false;
            var rec = await this.Listings.FirstOrDefaultAsync(r => r.ListedItemID == listing.ListedItemID);
            if (rec != null)
            {
                ret = true;
                //rec.Removed = DateTime.Now;

                using (var context = new DataModelsDB())
                {
                    // Pass the entity to Entity Framework and mark it as modified
                    context.Entry(rec).State = EntityState.Modified;
                    context.SaveChanges();
                }
            }
            return ret;
        }

        /// <summary>
        /// Users get to have their own sellers.
        /// 
        /// Another question: should a seller be tied to a store?  Can only list a seller's items in a single store?
        /// Saying no right now.  Thinking about Lior's testing listing same exact items in different stores for testing. 
        /// </summary>
        /// <param name="userid"></param>
        /// <param name="seller"></param>
        /// <returns></returns>
        public bool CanRunScan(string userid, string seller)
        {
            bool ret = false;
            //var settings =GetUserSettings(userid);
            var sellerrec = this.SearchHistory.Where(r => r.Seller == seller).ToList();
            if (sellerrec.Count > 0)
            { 
                var rec = sellerrec.Where(r => r.UserId == userid).Count();
                if (rec == 0)
                {
                    ret = false;
                }
                else
                {
                    ret = true;
                }
            }
            else
            {
                // didn't find SearchHistory records (no scans) but still might be in SellerProfile
                var profiles = this.SellerProfiles.Where(r => r.Seller == seller).ToList();
                if (profiles.Count > 0)
                {
                    var rec = profiles.Where(r => r.UserID == userid).Count();
                    if (rec == 0)
                    {
                        ret = false;
                    }
                    else
                    {
                        ret = true;
                    }
                }
                else
                {
                    ret = true;
                }
            }
            return ret;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="p"></param>
        /// <param name="userid"></param>
        /// <returns>null if success</returns>
        public async Task<string> UserProfileSaveAsync(UserSettingsView p, string userid)
        {
            string ret = null;
            try
            {
                //var profile = this.UserProfiles.Where(k => k.AppID == p.AppID).FirstOrDefault();
                //if (profile == null)
                //{
                //    var newprofile = new UserProfile();
                //    newprofile.AppID = p.AppID;
                //    newprofile.CertID = p.CertID;
                //    newprofile.DevID = p.DevID;
                //    newprofile.UserToken = p.UserToken;
                //    newprofile.UserID = userid;
                //    UserProfiles.Add(newprofile);
                //}

                var settings = this.UserSettings.Find(userid, 1);
                if (settings != null)
                {
                    var storeProfile = StoreProfiles.Find(GetUserSetting(userid).StoreID);
                    // storeProfile.AppID = p.AppID;
                    settings.UserID = userid;
                    // settings.ApplicationID = 1;
                    this.Entry(settings).State = EntityState.Modified;
                }
                else
                {
                    //var newprofile = new UserProfile();
                    //newprofile.AppID = p.AppID;
                    //newprofile.CertID = p.CertID;
                    //newprofile.DevID = p.DevID;
                    //newprofile.UserToken = p.UserToken;
                    //newprofile.Id = user.Id;
                    //newprofile.Firstname = p.Firstname;
                    //newprofile.Lastname = p.Lastname;
                    //UserProfiles.Add(newprofile);
                }
                await this.SaveChangesAsync();
            }
            catch (DbEntityValidationException e)
            {
                foreach (var eve in e.EntityValidationErrors)
                {
                    ret = string.Format("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:\n", eve.Entry.Entity.GetType().Name, eve.Entry.State);
                    foreach (var ve in eve.ValidationErrors)
                    {
                        ret += string.Format("- Property: \"{0}\", Error: \"{1}\"\n", ve.PropertyName, ve.ErrorMessage);
                    }
                }
            }
            catch (Exception exc)
            {
                ret = dsutil.DSUtil.ErrMsg("UserProfileSaveAsync", exc);
                dsutil.DSUtil.WriteFile(_logfile, ret, "admin");
            }
            return ret;
        }

        /// <summary>
        /// Had 'UserSettingsView' marked as a [Table] but EF kept giving model validation errors after I moved Framework to 4.7.2
        /// </summary>
        /// <param name="connStr"></param>
        /// <param name="userId"></param>
        /// <param name="applicationId"></param>
        /// <returns></returns>
        public static UserSettingsView GetUserSetting(string connStr, string userId, int applicationId)
        {
            try
            {
                var r = new UserSettingsView();
                using (SqlConnection connection = new SqlConnection(connStr))
                {
                    SqlCommand cmd = new SqlCommand("sp_UserSetting", connection);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@userID", userId);
                    cmd.Parameters.AddWithValue("@applicationID", applicationId);
                    connection.Open();

                    SqlDataReader reader = cmd.ExecuteReader();
                    if (reader.Read())
                    {
                        r.AppID = reader["AppID"].ToString();
                        r.DevID = reader["DevID"].ToString();
                        r.CertID = reader["CertID"].ToString();
                        r.Token = reader["Token"].ToString();
                        r.UserID = userId;
                        r.FirstName = reader["FirstName"].ToString();
                        r.StoreName = reader["StoreName"].ToString();
                        r.StoreID = Convert.ToInt32(reader["StoreID"].ToString());
                    }
                }
                return r;
            }
            catch (Exception exc)
            {
                string ret = dsutil.DSUtil.ErrMsg("GetUserSetting", exc);
                dsutil.DSUtil.WriteFile(_logfile, ret, "admin");
                return null;
            }
        }
        public List<SearchReport> GetSearchReport(int categoryId)
        {
            int sourceId = SourceIDFromCategory(categoryId);
            if (sourceId == 2)
            {
                List<SearchReport> data =
                    Database.SqlQuery<SearchReport>(
                    "select * from dbo.fnWalPriceCompare(@categoryId)",
                    new SqlParameter("@categoryId", categoryId))
                .ToList();
                return data;
            }

            if (sourceId == 1)
            {
                List<SearchReport> data =
                Database.SqlQuery<SearchReport>(
                "select * from dbo.fnPriceCompare(@categoryId)",
                new SqlParameter("@categoryId", categoryId))
                .ToList();
                return data;
            }
            return null;
        }

        public IQueryable<TimesSold> GetScanData(int rptNumber, DateTime dateFrom, int storeID, string itemID)
        {
            var p = new SqlParameter();
            p.ParameterName = "itemID";
            if (!string.IsNullOrEmpty(itemID))
            {
                p.Value = itemID;
            }
            else
            {
                p.Value = DBNull.Value;
            }

            var data = Database.SqlQuery<TimesSold>(
                "exec sp_GetScanReport @rptNumber, @dateFrom, @storeID, @itemID",
                new SqlParameter("rptNumber", rptNumber),
                new SqlParameter("dateFrom", dateFrom),
                new SqlParameter("storeID", storeID),
                p
                ).AsQueryable();
            return data;
        }
        public void UpdateOrderHistory(int rptNumber, string itemID, WalmartSearchProdIDResponse response)
        {
            string ret = null;
            try
            {
                var found = this.OrderHistory.FirstOrDefault(p => p.RptNumber == rptNumber && p.ItemID == itemID);
                if (found != null)
                {
                    found.WMCount = (byte)response.Count;
                    found.WMUrl = response.URL;
                    found.SoldAndShippedBySupplier = response.SoldAndShippedByWalmart;
                    found.SupplierBrand = response.SupplierBrand;
                    found.WMPrice = response.Price;
                    this.SaveChanges();
                }
            }
            catch (DbEntityValidationException e)
            {
                foreach (var eve in e.EntityValidationErrors)
                {
                    ret = string.Format("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:\n", eve.Entry.Entity.GetType().Name, eve.Entry.State);
                    foreach (var ve in eve.ValidationErrors)
                    {
                        ret += string.Format("- Property: \"{0}\", Error: \"{1}\"\n", ve.PropertyName, ve.ErrorMessage);
                    }
                }
                dsutil.DSUtil.WriteFile(_logfile, ret, "admin");
            }
            catch (Exception exc)
            {
                ret = dsutil.DSUtil.ErrMsg("UpdateOrderHistory", exc);
                dsutil.DSUtil.WriteFile(_logfile, ret, "admin");
            }
        }

    }
}
