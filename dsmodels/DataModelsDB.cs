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
        public DbSet<ListingX> Listings { get; set; }

        //public DbSet<PostedListing> PostedListings { get; set; }
        public DbSet<SourceCategories> SourceCategories { get; set; }
        public DbSet<SearchHistory> SearchHistory { get; set; }
        public DbSet<OrderHistory> OrderHistory { get; set; }
        public DbSet<UserSettings> UserSettings { get; set; }
        public DbSet<UserProfile> UserProfiles { get; set; }
        public DbSet<AspNetUser> AspNetUsers { get; set; }

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
            var x = from a in this.UserProfiles
                    where a.Id == userid
                    select new AppIDSelect
                    {
                        value = a.AppID,
                        viewValue = a.AppID

                    };
            return x.ToList();
        }

        /// <summary>
        /// return user profile based in his appID setting in UserSetting
        /// </summary>
        /// <param name="userid"></param>
        /// <returns></returns>
        public UserProfile GetUserProfile(string userid)
        {
            var setting = this.UserSettings.Find(userid, 1);
            var profile = this.UserProfiles.Where(r => r.AppID == setting.AppID).First();
            return profile;
        }

        //public async Task<PostedListing> GetPostedListing(string listedItemId)
        //{
        //    var found = await this.PostedListings.FirstOrDefaultAsync(r => r.ListedItemID == listedItemId);
        //    return found;
        //}

        public int SourceIDFromCategory(int categoryId)
        {
            int sourceId = 0;
            var category = this.SourceCategories.Where(r => r.ID == categoryId).FirstOrDefault();
            if (category != null)
                sourceId = category.SourceID;
            return sourceId;
        }

        public async Task<ListingX> GetPostedListing(int sourceID, string supplierItemID)
        {
            var found = await this.Listings.FirstOrDefaultAsync(r => r.SourceID == sourceID && r.SupplierItemID == supplierItemID);
            return found;
        }

        public async Task<ListingX> GetPostedListing(string itemId)
        {
            var found = await this.Listings.FirstOrDefaultAsync(r => r.ListedItemID == itemId);
            return found;
        }

        // listedItemId is my id
        public async Task<ListingX> GetPostedListingFromListId(string listedItemId)
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

        public async Task PostedListingSaveAsync(ListingX listing)
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

        //public static ListingX CopyStagedToPostedListing(StagedListing staged)
        //{
        //    var p = new ListingX();

        //    p.SourceID = staged.SourceID;
        //    p.SupplierItemID = staged.SupplierItemID;
        //    p.SourceUrl = staged.SourceUrl;
        //    p.SupplierPrice = staged.SupplierPrice;
        //    p.Title = staged.Title;
        //    p.Price = staged.Price;
        //    p.Description = staged.Description;
        //    p.Pictures = staged.Pictures;
        //    p.CategoryID = staged.CategoryID;
        //    p.PrimaryCategoryID = staged.PrimaryCategoryID;
        //    p.PrimaryCategoryName = staged.PrimaryCategoryName;
        //    p.ListedItemID = staged.ListedItemID;
        //    p.Listed = staged.Listed;
        //    p.Removed = staged.Removed;
        //    p.ListedQty = staged.ListedQty;
        //    p.InventoryException = staged.InventoryException;

        //    return p;
        //}

        public async Task<bool> UpdatePrice(ListingX listing, decimal price)
        {
            bool ret = false;
            var rec = await this.Listings.FirstOrDefaultAsync(r => r.ListedItemID == listing.ListedItemID);
            if (rec != null)
            {
                ret = true;
                //rec.Price = price;
                //rec.Updated = DateTime.Now;

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

        public async Task<SearchHistory> SearchHistorySave(SearchHistory sh)
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

        public async Task ListingSave(ListingX listing)
        {
            try
            {
                var found = await this.Listings.FirstOrDefaultAsync(r => r.ItemId == listing.ItemId);
                if (found == null)
                    Listings.Add(listing);
                else
                {
                    found.ListingPrice = listing.ListingPrice;
                    found.Source = listing.Source;
                    found.PictureUrl = listing.PictureUrl;
                    found.Title = listing.Title;
                    found.ListingTitle = listing.ListingTitle;
                    found.EbayUrl = listing.EbayUrl;
                    found.PrimaryCategoryID = listing.PrimaryCategoryID;
                    found.PrimaryCategoryName = listing.PrimaryCategoryName;
                    found.Description = listing.Description;
                    found.SourceID = listing.SourceID;
                    found.Qty = listing.Qty;
                    this.Entry(found).State = EntityState.Modified;
                }
                await this.SaveChangesAsync();
            }
            catch (Exception exc)
            {
                string msg = dsutil.DSUtil.ErrMsg("", exc);
            }
        }

        public async Task<ListingX> GetListing(string itemId)
        {
            var found = await this.Listings.FirstOrDefaultAsync(r => r.ItemId == itemId);
            return found;
        }
        public async Task<ListingX> ListingGet(string itemId)
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

        public async Task<ListingX> ListingXGet(string itemId)
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

        public async Task<bool> UpdateListedItemID(ListingX listing, string listedItemID)
        {
            bool ret = false;
            // find item by looking up seller's listing id
            var rec = await this.Listings.FirstOrDefaultAsync(r => r.ItemId == listing.ItemId);
            if (rec != null)
            {
                ret = true;
                rec.ListedItemID = listedItemID;
                rec.Listed = listing.Listed;

                using (var context = new DataModelsDB())
                {
                    // Pass the entity to Entity Framework and mark it as modified
                    context.Entry(rec).State = EntityState.Modified;
                    context.SaveChanges();
                }
            }
            return ret;
        }

        public async Task<bool> UpdateRemovedDate(ListingX listing)
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
        public async Task<string> UserSettingSaveAsync(UserSettings p, string username)
        {
            string ret = string.Empty;
            try
            {
                var userid = GetUserIDFromName(username);
                //var user = await UserManager.FindByNameAsync(username);
                //if (user == null)
                //{
                //    return "user does not exist";
                //}

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
                ret = exc.Message;
            }
            return ret;
        }

    }
}
