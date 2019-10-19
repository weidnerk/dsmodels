using System;
using System.Collections.Generic;
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
        public DbSet<ItemSpecific> ItemSpecifics { get; set; }

        //public DbSet<PostedListing> PostedListings { get; set; }
        public DbSet<SourceCategories> SourceCategories { get; set; }
        public DbSet<SearchHistory> SearchHistory { get; set; }
        public DbSet<OrderHistory> OrderHistory { get; set; }
        public DbSet<UserSettings> UserSettings { get; set; }   // user's current selection
        public DbSet<UserProfileView> UserProfilesView { get; set; }
        public DbSet<UserProfile> UserProfiles { get; set; }
        public DbSet<AspNetUser> AspNetUsers { get; set; }
        public DbSet<SellerProfile> SellerProfiles { get; set; }

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
        public List<AppIDSelect> GetAppIDs(string userid)
        {
            var x = from a in this.UserProfilesView
                    where a.UserID == userid
                    select new AppIDSelect
                    {
                        value = a.AppID,
                        viewValue = a.AppID

                    };
            return x.ToList();
        }

        /// <summary>
        /// Return user profile based in his appID setting in UserSetting
        /// </summary>
        /// <param name="userid"></param>
        /// <returns></returns>
        public UserProfileView GetUserProfile(string userid)
        {
            try
            {
                // match composite key, UserId/ApplicationID; ApplicationID=1 for ds109
                var setting = this.UserSettings.Find(userid, 1);
                if (setting != null)    // user may have registered but has not updated API keys yet
                {
                    var profile = this.UserProfilesView.Where(r => r.AppID == setting.AppID).First();

                    // db issue storing first/last name in UserProfile since user may have multiple API keys
                    var names = this.UserProfilesView.Where(p => p.UserID == userid && p.Firstname != null).First();
                    profile.Firstname = names.Firstname;
                    profile.Lastname = names.Lastname;

                    profile.UserName = this.AspNetUsers.Where(r => r.Id == userid).Select(s => s.UserName).First();
                    return profile;
                }
                return null;
            }
            catch (Exception exc)
            {
                return null;
            }
        }

        public UserProfileView GetUserProfile(string userid, string appID)
        {
            var profile = this.UserProfilesView.Where(r => r.AppID == appID).First();

            // db issue storing first/last name in UserProfile since user may have multiple API keys
            var names = this.UserProfilesView.Where(p => p.UserID == userid && p.Firstname != null).First();
            profile.Firstname = names.Firstname;
            profile.Lastname = names.Lastname;
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

        public List<StagedListing> GetToList(int categoryId, int listedQty)
        {
            List<StagedListing> data =
                Database.SqlQuery<StagedListing>(
                "select * from dbo.fnToListFinal(@categoryId, @listedQty)",
                new SqlParameter("@categoryId", categoryId),
                new SqlParameter("@listedQty", listedQty))
            .ToList();
            return data;
        }

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
                this.OrderHistory.RemoveRange(this.OrderHistory.Where(x => x.RptNumber == rptNumber));
                await this.SaveChangesAsync();

                var sh = new SearchHistory() { Id = rptNumber };
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
                this.ItemSpecifics.RemoveRange(this.ItemSpecifics.Where(x => x.SellerItemId == sellerItemId));
                await this.SaveChangesAsync();

                var sh = new Listing() { ItemId = sellerItemId };
                this.Listings.Attach(sh);
                this.Listings.Remove(sh);
                await this.SaveChangesAsync();
            }
            catch (Exception exc)
            {
            }
        }

        public async Task AppIDRemove(string appID)
        {
            try
            {
                this.UserProfilesView.RemoveRange(this.UserProfilesView.Where(x => x.AppID == appID));
                await this.SaveChangesAsync();
            }
            catch (Exception exc)
            {
            }
        }

        public string OrderHistorySave(List<OrderHistory> oh, int rptNumber, bool listingEnded)
        {
            string ret = string.Empty;
            try
            {
                foreach (OrderHistory item in oh)
                {
                    item.RptNumber = rptNumber;
                    item.ListingEnded = listingEnded;
                    OrderHistory.Add(item);
                }
                this.SaveChanges();
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
                ret = exc.Message;
            }
            return ret;
        }

        public async Task ListingSave(Listing listing)
        {
            try
            {
                // var found = await this.Listings.Include(x => x.ItemSpecifics.Select(y => y.Listing)).FirstOrDefaultAsync(r => r.ItemId == listing.ItemId);
                var found = await this.Listings.FirstOrDefaultAsync(r => r.ItemId == listing.ItemId);
                if (found == null)
                {
                    Listings.Add(listing);
                }
                else
                {
                    this.Entry(found).State = EntityState.Modified;

                    // https://stackoverflow.com/questions/10822656/entity-framework-include-multiple-levels-of-properties
                    // this.Entry(found).Property(e => e.ItemSpecifics.Select(y => y.Listing)).IsModified = true;

                    found.ListingPrice = listing.ListingPrice;
                    // found.Source = listing.Source;
                    // found.PictureUrl = listing.PictureUrl;
                    // found.Title = listing.Title;
                    found.ListingTitle = listing.ListingTitle;
                    // found.EbayUrl = listing.EbayUrl;
                    // found.PrimaryCategoryID = listing.PrimaryCategoryID;
                    // found.PrimaryCategoryName = listing.PrimaryCategoryName;
                    // found.Description = listing.Description;
                    // found.SourceID = listing.SourceID;
                    found.Qty = listing.Qty;
                    // found.Seller = listing.Seller;
                    found.Note = listing.Note;
                    // found.ItemSpecifics = listing.ItemSpecifics; // store when created, don't need to update
                    found.Profit = listing.Profit;
                    found.ProfitMargin = listing.ProfitMargin;
                    found.Updated = DateTime.Now;
                }
                await this.SaveChangesAsync();
            }
            catch (Exception exc)
            {
                string msg = dsutil.DSUtil.ErrMsg("", exc);
            }
        }
        public async Task NoteSave(Listing listing)
        {
            try
            {
                var found = await this.Listings.FirstOrDefaultAsync(r => r.ItemId == listing.ItemId);
                if (found == null)
                {
                    // error
                    //   Listings.Add(listing);
                }
                else
                {
                    found.Note = listing.Note;
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
        public async Task OOSSave(Listing listing)
        {
            try
            {
                var found = await this.Listings.FirstOrDefaultAsync(r => r.ItemId == listing.ItemId);
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
        public List<Listing> GetListings()
        {
            var found = this.Listings.ToList();
            return found;
        }

        public async Task<Listing> GetListing(string itemId)
        {
            var found = await this.Listings.FirstOrDefaultAsync(r => r.ItemId == itemId);
            return found;
        }
        public async Task<Listing> ListingGet(string itemId)
        {
            try
            {
                var listing = await this.Listings.FirstOrDefaultAsync(r => r.ItemId == itemId);
                return listing;
            }
            catch (Exception exc)
            {
                string msg = dsutil.DSUtil.ErrMsg("", exc);
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
                return null;
            }
        }

        public async Task<bool> UpdateListedItemID(Listing listing, string listedItemID, string userId, bool listedWithAPI, string listedResponse, DateTime? updated = null)
        {
            string errStr; 
            bool ret = false;
            try
            {
                // find item by looking up seller's listing id
                var rec = await this.Listings.FirstOrDefaultAsync(r => r.ItemId == listing.ItemId);
                if (rec != null)
                {
                    ret = true;
                    rec.ListedItemID = listedItemID;
                    if (updated.HasValue)
                    {
                        rec.ListedUpdated = DateTime.Now;
                    }
                    else
                    {
                        rec.Listed = listing.Listed;
                    }
                    rec.ListedBy = userId;
                    rec.ListedWithAPI = listedWithAPI;
                    rec.ListedResponse = listedResponse;

                    //using (var context = new DataModelsDB())
                    //{
                        // Pass the entity to Entity Framework and mark it as modified
                        this.Entry(rec).State = EntityState.Modified;
                        this.SaveChanges();
                    //}
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
            }
            catch (Exception exc)
            {
                errStr = dsutil.DSUtil.ErrMsg("UpdateListedItemID", exc);
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

                using (var context = new DataModelsDB())
                {
                    // Pass the entity to Entity Framework and mark it as modified
                    context.Entry(rec).State = EntityState.Modified;
                    context.SaveChanges();
                }
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
        /// 
        /// </summary>
        /// <param name="p"></param>
        /// <param name="userid"></param>
        /// <returns>null if success</returns>
        public async Task<string> UserProfileSaveAsync(UserProfileView p, string userid)
        {
            string ret = null;
            try
            {
                var profile = this.UserProfiles.Where(k => k.AppID == p.AppID).FirstOrDefault();
                if (profile == null)
                {
                    var newprofile = new UserProfile();
                    newprofile.AppID = p.AppID;
                    newprofile.CertID = p.CertID;
                    newprofile.DevID = p.DevID;
                    newprofile.UserToken = p.UserToken;
                    newprofile.UserID = userid;
                    UserProfiles.Add(newprofile);
                }

                var settings = this.UserSettings.Find(userid, 1);
                if (settings != null)
                {
                    settings.AppID = p.AppID;
                    settings.UserID = userid;
                    settings.ApplicationID = 1;
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
            }
            return ret;
        }

    }
}
