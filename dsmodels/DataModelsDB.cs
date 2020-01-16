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
        public DbSet<SellerListingItemSpecific> SellerListingItemSpecifics { get; set; }
        public DbSet<OrderHistoryItemSpecific> OrderHistoryItemSpecifics { get; set; }

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
        public DbSet<VEROBrands> VEROBrands { get; set; }
        public DbSet<SellerListing> SellerListings { get; set; }
        public DbSet<SupplierItem> SupplierItems { get; set; }

        public string GetUserIDFromName(string username)
        {
            var id = this.AspNetUsers.Where(r => r.UserName == username).Select(s => s.Id).First();
            return id;
        }

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

        //public async Task<Listing> GetPostedListing(int sourceID, string supplierItemID)
        //{
        //    var found = await this.Listings.FirstOrDefaultAsync(r => r.SourceID == sourceID && r.SupplierItemID == supplierItemID);
        //    return found;
        //}

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

        public async Task<bool> UpdatePrice(Listing listing, decimal price, decimal supplierPrice)
        {
            bool ret = false;
            var rec = await this.Listings.FirstOrDefaultAsync(r => r.ListedItemID == listing.ListedItemID);
            if (rec != null)
            {
                ret = true;
                rec.ListingPrice = price;
                rec.SupplierItem.SupplierPrice = supplierPrice;
                rec.Updated = DateTime.Now;

                this.Entry(rec).State = EntityState.Modified;
                this.SaveChanges();
            }
            return ret;
        }
        public async Task OrderHistorySaveToList(OrderHistory oh)
        {
            string ret = null;
            try
            {
                var rec = await this.OrderHistory.FirstOrDefaultAsync(r => r.ItemID == oh.ItemID);
                if (rec != null)
                {
                    rec.ToListing = oh.ToListing;
                    this.Entry(rec).State = EntityState.Modified;
                    await this.SaveChangesAsync();
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
                ret = dsutil.DSUtil.ErrMsg("OrderHistorySaveToList", exc);
                dsutil.DSUtil.WriteFile(_logfile, ret, "admin");
            }
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
            this.SearchHistory.Add(sh);
            await this.SaveChangesAsync();
            return sh;
        }
        public async Task<SearchHistory> SearchHistoryUpdate(SearchHistory sh, params string[] changedPropertyNames)
        {
            string ret = null;
            try
            {
                //var found = this.SearchHistory.Find(sh.ID);
                this.SearchHistory.Attach(sh);
                foreach (var propertyName in changedPropertyNames)
                {
                    this.Entry(sh).Property(propertyName).IsModified = true;
                }            
                await this.SaveChangesAsync();
                return sh;
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
                ret = dsutil.DSUtil.ErrMsg("SearchHistoryUpdate", exc);
                dsutil.DSUtil.WriteFile(_logfile, ret, "admin");
            }
            return null;
        }
        //public async Task<SearchHistory> SearchHistoryUpdate_CalculateMatch(SearchHistory sh)
        //{
        //    string ret = null;
        //    try
        //    {
        //        var found = this.SearchHistory.Find(sh.ID);
        //        found.CalculateMatch = sh.CalculateMatch;
        //        await this.SaveChangesAsync();
        //        return sh;
        //    }
        //    catch(DbEntityValidationException e)
        //    {
        //        foreach (var eve in e.EntityValidationErrors)
        //        {
        //            ret = string.Format("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:\n", eve.Entry.Entity.GetType().Name, eve.Entry.State);
        //            foreach (var ve in eve.ValidationErrors)
        //            {
        //                ret += string.Format("- Property: \"{0}\", Error: \"{1}\"\n", ve.PropertyName, ve.ErrorMessage);
        //            }
        //        }
        //        dsutil.DSUtil.WriteFile(_logfile, ret, "admin");
        //    }
        //    catch (Exception exc)
        //    {
        //        ret = dsutil.DSUtil.ErrMsg("SearchHistoryUpdate_CalculateMatch", exc);
        //        dsutil.DSUtil.WriteFile(_logfile, ret, "admin");
        //    }
        //    return null;
        //}
        //public async Task<SearchHistory> SearchHistoryUpdate_Updated(SearchHistory sh)
        //{
        //    string ret = null;
        //    try
        //    {
        //        var found = this.SearchHistory.Find(sh.ID);
        //        found.Updated = sh.Updated;
        //        await this.SaveChangesAsync();
        //        return sh;
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
        //        dsutil.DSUtil.WriteFile(_logfile, ret, "admin");
        //    }
        //    catch (Exception exc)
        //    {
        //        ret = dsutil.DSUtil.ErrMsg("SearchHistoryUpdate_Updated", exc);
        //        dsutil.DSUtil.WriteFile(_logfile, ret, "admin");
        //    }
        //    return null;
        //}
        //public async Task<SearchHistory> SearchHistoryUpdate_Running(SearchHistory sh)
        //{
        //    string ret = null;
        //    try
        //    {
        //        var found = this.SearchHistory.Find(sh.ID);
        //        found.Running= sh.Running;
        //        await this.SaveChangesAsync();
        //        return sh;
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
        //        dsutil.DSUtil.WriteFile(_logfile, ret, "admin");
        //    }
        //    catch (Exception exc)
        //    {
        //        ret = dsutil.DSUtil.ErrMsg("SearchHistoryUpdate_Running", exc);
        //        dsutil.DSUtil.WriteFile(_logfile, ret, "admin");
        //    }
        //    return null;
        //}

        /// <summary>
        /// Comletely remove a scan from SearchHistory, OrderHistory and OrderHistoryDetails
        /// </summary>
        /// <param name="rptNumber"></param>
        /// <returns></returns>
        public async Task HistoryRemove(string connStr, int rptNumber)
        {
            string ret = null;
            try
            {
                
                OrderHistoryItemSpecificRemove(connStr, rptNumber);
                var fromDate = new DateTime(2000, 1, 1);
                await HistoryDetailRemove(rptNumber, fromDate);

                this.OrderHistory.RemoveRange(this.OrderHistory.Where(x => x.RptNumber == rptNumber));
                await this.SaveChangesAsync();

                var sh = new SearchHistory() { ID = rptNumber };
                this.SearchHistory.Attach(sh);
                this.SearchHistory.Remove(sh);
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
                dsutil.DSUtil.WriteFile(_logfile, ret, "admin");
            }
            catch (Exception exc)
            {
                ret = dsutil.DSUtil.ErrMsg("HistoryRemove", exc);
                dsutil.DSUtil.WriteFile(_logfile, ret, "admin");
            }
        }

        /// <summary>
        /// Remove records from OrderHistoryDetails for some seller from fromDate
        /// </summary>
        /// <param name="rptNumber"></param>
        /// <param name="fromDate"></param>
        /// <returns></returns>
        public async Task<bool> HistoryDetailRemove(int rptNumber, DateTime fromDate)
        {
            bool retValue = false;
            string ret = null;
            int numToDelete = 0;
            try
            {
                this.OrderHistoryDetails.RemoveRange(this.OrderHistoryDetails.Where(p => p.OrderHistory.RptNumber == rptNumber && p.DateOfPurchase >= fromDate));
                await this.SaveChangesAsync();
                retValue = true;
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
                ret = dsutil.DSUtil.ErrMsg("HistoryDetailRemove", exc);
                dsutil.DSUtil.WriteFile(_logfile, ret, "admin");
            }
            return retValue;
        }

        /// <summary>
        /// Calculate date to start removing history from
        /// </summary>
        /// <param name="rptNumber"></param>
        /// <returns></returns>
        public DateTime? fromDateToScan(int rptNumber)
        {
            DateTime fromDate = DateTime.Now;
          
            try {
                var its = this.OrderHistoryDetails.Where(p => p.OrderHistory.RptNumber == rptNumber).OrderByDescending(o => o.DateOfPurchase).ToList();
                var lastSoldItem = its.FirstOrDefault();
                if (lastSoldItem != null)
                {
                    // date seller last sold an item
                    var lastSold = lastSoldItem.DateOfPurchase;
                    lastSold = lastSold.AddDays(-1);
                    DateTime tempDate = new DateTime(lastSold.Year, lastSold.Month, lastSold.Day);
                    return tempDate;
                }
                else
                {
                    // if we don't have anything, scan 30 days
                    return DateTime.Now.AddDays(-30);
                }
            }
            catch (Exception exc)
            {
                string ret = dsutil.DSUtil.ErrMsg("fromDateToScan", exc);
                dsutil.DSUtil.WriteFile(_logfile, ret, "admin");
                return null;
            }
        }
        public async Task DeleteListingRecord(string sellerItemId)
        {
            try
            {
                var found = OrderHistory.FirstOrDefault(p => p.ItemID == sellerItemId);
                if (found == null)
                {
                    // don't remove itemspecifics if found in OrderHistory (a scan)

                    // first remove item specifics
                    this.SellerListingItemSpecifics.RemoveRange(this.SellerListingItemSpecifics.Where(x => x.SellerItemID == sellerItemId));
                    await this.SaveChangesAsync();
                }

                var listing = Listings.FirstOrDefault(p => p.ItemID == sellerItemId);
                if (listing != null)
                {
                    //var sh = new Listing() { ItemID = sellerItemId, ID = listing.ID };
                    this.Listings.Attach(listing);
                    this.Listings.Remove(listing);
                    await this.SaveChangesAsync();

                    var sl = new SellerListing() { ItemID = sellerItemId };
                    this.SellerListings.Attach(sl);
                    this.SellerListings.Remove(sl);
                    await this.SaveChangesAsync();
                }
            }
            catch (Exception exc)
            {
                string ret = dsutil.DSUtil.ErrMsg("DeleteListingRecord", exc);
                dsutil.DSUtil.WriteFile(_logfile, ret, "admin");
            }
        }

        /// <summary>
        /// Remember OrderHistory has property, OrderHistoryDetais
        /// and OrderHistoryDetails was already removed from fromDate in HistoryDetailRemove.
        /// </summary>
        /// <param name="oh"></param>
        /// <param name="fromDate"></param>
        /// <returns></returns>
        public string OrderHistorySave(OrderHistory oh, DateTime fromDate)
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
                else
                {
                    var found = OrderHistory.FirstOrDefault(p => p.ItemID == oh.ItemID);
                    if (found != null)
                    {
                        oh.OrderHistoryDetails.ToList().ForEach(c => c.ItemID = found.ItemID);
                        OrderHistoryDetails.AddRange(oh.OrderHistoryDetails.Where(p => p.DateOfPurchase >= fromDate));
                        this.SaveChanges();
                    }
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

        /// <summary>
        /// Is this finally the correct UPDATE pattern?
        /// </summary>
        /// <param name="specific"></param>
        public void SellerListingItemSpecificUpdate_notused(SellerListingItemSpecific specific)
        {
            string output = null;
            try
            {
                var found = this.SellerListingItemSpecifics.AsNoTracking().SingleOrDefault(p => p.SellerItemID == specific.SellerItemID && p.ItemName == "UPC");
                if (found == null)
                {
                    this.SellerListingItemSpecifics.Add(specific);
                    this.SaveChanges();
                }
                else
                {
                    specific.ID = found.ID;
                    this.SellerListingItemSpecifics.Attach(specific);
                    this.Entry(specific).State = EntityState.Modified;
                    this.SaveChanges();
                }
            }
            catch (DbEntityValidationException e)
            {
                output = GetValidationErr(e);
                dsutil.DSUtil.WriteFile(_logfile, "SellerListingItemSpecificUpdate: " + specific.SellerItemID + " " + output, "admin");
            }
            catch (Exception exc)
            {
                output = exc.Message;
                string msg = dsutil.DSUtil.ErrMsg("ItemSpecificUpdate", exc);
                dsutil.DSUtil.WriteFile(_logfile, "ItemSpecificUpdate: " + specific.SellerItemID + " " + msg, "admin");
            }
        }
        public void OrderHistoryItemSpecificUpdate(OrderHistoryItemSpecific specific)
        {
            string output = null;
            try
            {
                var found = this.OrderHistoryItemSpecifics.AsNoTracking().SingleOrDefault(p => p.SellerItemID == specific.SellerItemID && p.ItemName == "UPC");
                if (found == null)
                {
                    this.OrderHistoryItemSpecifics.Add(specific);
                    this.SaveChanges();
                }
                else
                {
                    specific.ID = found.ID;
                    this.OrderHistoryItemSpecifics.Attach(specific);
                    this.Entry(specific).State = EntityState.Modified;
                    this.SaveChanges();
                }
            }
            catch (DbEntityValidationException e)
            {
                output = GetValidationErr(e);
                dsutil.DSUtil.WriteFile(_logfile, "OrderHistoryItemSpecificUpdate: " + specific.SellerItemID + " " + output, "admin");
            }
            catch (Exception exc)
            {
                output = exc.Message;
                string msg = dsutil.DSUtil.ErrMsg("ItemSpecificUpdate", exc);
                dsutil.DSUtil.WriteFile(_logfile, "ItemSpecificUpdate: " + specific.SellerItemID + " " + msg, "admin");
            }
        }
        public void OrderHistoryUpdate(OrderHistory orderHistory, params string[] changedPropertyNames)
        {
            string output = null;
            try
            {
                if (orderHistory.ItemID == "254295059622")
                {
                    int stop = 99;
                }
                var found = this.OrderHistory.AsNoTracking().SingleOrDefault(p => p.ItemID == orderHistory.ItemID);
                orderHistory.ProposePrice = Math.Round(orderHistory.ProposePrice.Value, 2);
                if (found == null)
                {
                    this.OrderHistory.Add(orderHistory);
                    this.SaveChanges();
                }
                else
                {
                    orderHistory.ItemID = found.ItemID;
                    this.OrderHistory.Attach(orderHistory);
                    foreach (var propertyName in changedPropertyNames)
                    {
                        this.Entry(orderHistory).Property(propertyName).IsModified = true;
                    }
                    this.SaveChanges();
                }
            }
            catch (DbEntityValidationException e)
            {
                output = GetValidationErr(e);
                dsutil.DSUtil.WriteFile(_logfile, "OrderHistoryUpdate: " + orderHistory.ItemID + " " + output, "admin");

            }
            catch (Exception exc)
            {
                output = exc.Message;
                string msg = dsutil.DSUtil.ErrMsg("OrderHistoryUpdate", exc);
                dsutil.DSUtil.WriteFile(_logfile, "OrderHistoryUpdate: " + orderHistory.ItemID + " " + msg, "admin");
            }
        }
        /// <summary>
        /// Now stored when running seller scrape.
        /// </summary>
        /// <param name="specifics"></param>
        /// <returns></returns>
        public async Task<string> SellerListingItemSpecificSave(List<SellerListingItemSpecific> specifics)
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
                var found = await this.SellerListingItemSpecifics.FirstOrDefaultAsync(p => p.SellerItemID == itemID);
                if (found != null)
                {
                    this.SellerListingItemSpecifics.RemoveRange(this.SellerListingItemSpecifics.Where(x => x.SellerItemID == itemID));
                }
                foreach (var item in specifics)
                {
                    if (item.ItemValue.Length > 700)
                    {
                        dsutil.DSUtil.WriteFile(_logfile, "SellerListingItemSpecificSave: ItemValue truncated: " + item.ItemName + " -> " + item.ItemValue, "admin");
                        item.ItemValue = item.ItemValue.Substring(0, 700);
                    }
                    this.SellerListingItemSpecifics.Add(item);
                }
                await this.SaveChangesAsync();
            }
            catch (DbEntityValidationException e)
            {
                output = GetValidationErr(e);
                dsutil.DSUtil.WriteFile(_logfile, "SellerListingItemSpecificSave: " + itemID  + " " + output, "admin");

                output = DumpSellerListingItemSpecifics(specifics);
                dsutil.DSUtil.WriteFile(_logfile, output, "admin");
            }
            catch (Exception exc)
            {
                output = exc.Message;
                string msg = dsutil.DSUtil.ErrMsg("SellerListingItemSpecificSave", exc);
                dsutil.DSUtil.WriteFile(_logfile, "SellerListingItemSpecificSave: " + itemID + " " + msg, "admin");

                output = DumpSellerListingItemSpecifics(specifics);
                dsutil.DSUtil.WriteFile(_logfile, output, "admin");
            }
            return output;
        }
        public async Task<string> OrderHistoryItemSpecificSave(List<OrderHistoryItemSpecific> specifics)
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
                var found = await this.OrderHistoryItemSpecifics.FirstOrDefaultAsync(p => p.SellerItemID == itemID);
                if (found != null)
                {
                    this.OrderHistoryItemSpecifics.RemoveRange(this.OrderHistoryItemSpecifics.Where(x => x.SellerItemID == itemID));
                }
                foreach (var item in specifics)
                {
                    if (item.ItemValue.Length > 700)
                    {
                        dsutil.DSUtil.WriteFile(_logfile, "OrderHistoryItemSpecificSave: ItemValue truncated: " + item.ItemName + " -> " + item.ItemValue, "admin");
                        item.ItemValue = item.ItemValue.Substring(0, 700);
                    }
                    this.OrderHistoryItemSpecifics.Add(item);
                }
                await this.SaveChangesAsync();
            }
            catch (DbEntityValidationException e)
            {
                output = GetValidationErr(e);
                dsutil.DSUtil.WriteFile(_logfile, "OrderHistoryItemSpecificSave: " + itemID + " " + output, "admin");

                output = DumpOrderHistoryItemSpecifics(specifics);
                dsutil.DSUtil.WriteFile(_logfile, output, "admin");
            }
            catch (Exception exc)
            {
                output = exc.Message;
                string msg = dsutil.DSUtil.ErrMsg("OrderHistoryItemSpecificSave", exc);
                dsutil.DSUtil.WriteFile(_logfile, "OrderHistoryItemSpecificSave: " + itemID + " " + msg, "admin");

                output = DumpOrderHistoryItemSpecifics(specifics);
                dsutil.DSUtil.WriteFile(_logfile, output, "admin");
            }
            return output;
        }
        public static List<OrderHistoryItemSpecific> CopyFromOrderHistory(List<SellerListingItemSpecific> specifics)
        {
            var target = new List<OrderHistoryItemSpecific>();
            foreach(var i in specifics)
            {
                var specific = new OrderHistoryItemSpecific();
                specific.SellerItemID = i.SellerItemID;
                specific.ItemName = i.ItemName;
                specific.ItemValue = i.ItemValue;
                specific.Flags = i.Flags;
                
                target.Add(specific);
            }
            return target;
        }
        public static string DumpSellerListingItemSpecifics(List<SellerListingItemSpecific> specifics)
        {
            string output = null;
            if (specifics.Count > 0)
            {
                output = "ItemID: " + specifics[0].SellerItemID + "\n";
            }
            foreach (var spec in specifics)
            {
                output += "ItemName: " + spec.ItemName + " -> " + spec.ItemValue + "\n";
            }
            return output;
        }
        public static string DumpOrderHistoryItemSpecifics(List<OrderHistoryItemSpecific> specifics)
        {
            string output = null;
            if (specifics.Count > 0)
            {
                output = "ItemID: " + specifics[0].SellerItemID + "\n";
            }
            foreach (var spec in specifics)
            {
                output += "ItemName: " + spec.ItemName + " -> " + spec.ItemValue + "\n";
            }
            return output;
        }

        /*
        public async Task ListingSaveAsync_old(Listing listing, string userID)
        {
            try
            {
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

                    found.SupplierItem.SupplierPrice = listing.SupplierItem.SupplierPrice;
                    found.ListingPrice = listing.ListingPrice;
                    //found.SourceID = listing.SourceID;

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
                string msg = dsutil.DSUtil.ErrMsg("ERROR ListingSaveAsync itemid: " + listing.ItemID, exc);
                dsutil.DSUtil.WriteFile(_logfile, msg, "admin");
            }
        }
        */
        public async Task ListingSaveAsync(Listing listing, string userID, params string[] changedPropertyNames)
        {
            try
            {
                // var found = await this.Listings.Include(x => x.ItemSpecifics.Select(y => y.Listing)).FirstOrDefaultAsync(r => r.ItemId == listing.ItemId);
                var found = await this.Listings.AsNoTracking().FirstOrDefaultAsync(r => r.ItemID == listing.ItemID);
                if (found == null)
                {
                    listing.Created = DateTime.Now;
                    listing.CreatedBy = userID;
                    Listings.Add(listing);
                }
                else
                {
                    this.Listings.Attach(listing);
                    foreach (var propertyName in changedPropertyNames)
                    {
                        if (propertyName == "SupplierItem.SupplierPrice")
                        {
                            this.Entry(listing.SupplierItem).Property("SupplierPrice").IsModified = true;
                        }
                        else
                        {
                            this.Entry(listing).Property(propertyName).IsModified = true;
                        }
                    }
                }
                await this.SaveChangesAsync();
            }
            catch (Exception exc)
            {
                string msg = dsutil.DSUtil.ErrMsg("ERROR ListingSaveAsync itemid: " + listing.ItemID, exc);
                dsutil.DSUtil.WriteFile(_logfile, msg, "admin");
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
                dsutil.DSUtil.WriteFile(_logfile, msg, "admin");
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
                dsutil.DSUtil.WriteFile(_logfile, msg, "admin");
            }
        }
        public async Task SellerProfileSave(SellerProfile sellerProfile, params string[] changedPropertyNames)
        {
            try
            {
                var found = await this.SellerProfiles.AsNoTracking().FirstOrDefaultAsync(r => r.Seller == sellerProfile.Seller);
                if (found == null)
                {
                    SellerProfiles.Add(sellerProfile);
                }
                else
                {
                    //found.Note = sellerProfile.Note;
                    //found.Updated = DateTime.Now;
                    //found.UpdatedBy = sellerProfile.UpdatedBy;
                    //this.Entry(found).State = EntityState.Modified;

                    this.SellerProfiles.Attach(sellerProfile);
                    foreach (var propertyName in changedPropertyNames)
                    {
                        this.Entry(sellerProfile).Property(propertyName).IsModified = true;
                    }
                }
                await this.SaveChangesAsync();
            }
            catch (Exception exc)
            {
                string msg = dsutil.DSUtil.ErrMsg("", exc);
                dsutil.DSUtil.WriteFile(_logfile, msg, "admin");
            }
        }
        public List<ListingView> GetListings(int storeID)
        {
            //var found = this.ListingsView.Include("SellerListing").Where(p => p.StoreID == storeID).ToList();
            var found = this.ListingsView.Where(p => p.StoreID == storeID).ToList();
            return found;
        }

        public async Task<Listing> GetListing(string itemId)
        {
            var found = await this.Listings.FirstOrDefaultAsync(r => r.ItemID == itemId);
            return found;
        }
        public async Task<Listing> ListingGet(string itemID)
        {
            try
            {
                var listing = await this.Listings.Include(p => p.SellerListing).Include(p => p.SupplierItem).FirstOrDefaultAsync(r => r.ItemID == itemID);
                if (listing == null)
                {
                    return null;
                }
                // not sure why listing.SupplierItem is null after this line, so load manually....
                var si = this.GetSupplierItem(itemID);
                listing.SupplierItem = si;
                return listing;
            }
            catch (Exception exc)
            {
                string msg = dsutil.DSUtil.ErrMsg("ListingGet, itemID: " + itemID, exc);
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

        public async Task<bool> ListedItemIDUpdate(Listing listing, string listedItemID, string userId, bool listedWithAPI, string listedResponse, DateTime? updated = null)
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

        public async Task<bool> OOSUpdate(string listedItemID, bool OOS)
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

        public static bool OrderHistoryItemSpecificRemove(string connStr, int rptNumber)
        {
            try
            {
                var r = new UserSettingsView();
                using (SqlConnection connection = new SqlConnection(connStr))
                {
                    SqlCommand cmd = new SqlCommand("sp_OrderHistoryItemSpecificRemove", connection);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@rptNumber", rptNumber);
                    connection.Open();

                    cmd.ExecuteNonQuery();
                }
                return true;
            }
            catch (Exception exc)
            {
                string ret = dsutil.DSUtil.ErrMsg("ItemSpecificRemove", exc);
                dsutil.DSUtil.WriteFile(_logfile, ret, "admin");
                return false;
            }
        }

        public IQueryable<TimesSold> GetSalesData(int rptNumber, DateTime dateFrom, int storeID, string itemID)
        {
            if (rptNumber == 0)
            {
                var result = GetSalesDataAll(dateFrom, storeID);
                return result;
            }
            else
            {
                var result = GetScanData(rptNumber, dateFrom, storeID, itemID);
                return result;
            }
        }

        protected IQueryable<TimesSold> GetSalesDataAll(DateTime dateFrom, int storeID)
        {
            var data = Database.SqlQuery<TimesSold>(
                "exec sp_Report @dateFrom, @storeID, @minSoldQty",
                new SqlParameter("dateFrom", dateFrom),
                new SqlParameter("storeID", storeID),
                new SqlParameter("minSoldQty", 3)
                ).AsQueryable();
            return data;
        }
        /// <summary>
        /// Used to fetch a particular scan.
        /// </summary>
        /// <param name="rptNumber"></param>
        /// <param name="dateFrom"></param>
        /// <param name="storeID"></param>
        /// <param name="itemID"></param>
        /// <returns></returns>
        protected IQueryable<TimesSold> GetScanData(int rptNumber, DateTime dateFrom, int storeID, string itemID)
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

        /// <summary>
        /// When we're doing product find on walmart, we're keying off UPC and MPN
        /// Of course, it's possible the item is already captured in SuppliterItem by a another scrape so we have to take into account
        /// the item might exist.
        /// So when saving SupplierItem during a scan, use this method.
        /// </summary>
        /// <param name="UPC"></param>
        /// <param name="MPN"></param>
        /// <param name="item"></param>
        /// <param name="changedPropertyNames"></param>
        public void SupplierItemUpdateScrape(string UPC, string MPN, SupplierItem item, params string[] changedPropertyNames)
        {
            string ret = null;
            try
            {
                if (UPC == "844702079724")
                {
                    int stop = 99;
                }
                SupplierItem found = null;
                if (!string.IsNullOrEmpty(UPC))
                {
                    found = this.SupplierItems.AsNoTracking().FirstOrDefault(p => p.UPC == UPC);
                    if (found != null)
                    {
                        found.MPN = item.MPN;
                    }
                }
                else
                {
                    if (!string.IsNullOrEmpty(MPN))
                    {
                        found = this.SupplierItems.AsNoTracking().FirstOrDefault(p => p.MPN == MPN);
                        if (found != null)
                        {
                            found.UPC = item.UPC;
                        }
                        else
                        {
                            // if the MPN isn't in SupplierItem, the UPC might already be there
                            found = this.SupplierItems.AsNoTracking().FirstOrDefault(p => p.UPC == item.UPC);
                            if (found != null)
                            {
                                found.MPN = item.MPN;
                            }
                        }
                    }
                }
                if (found != null)
                {
                    item.ID = found.ID;
                    this.SupplierItems.Attach(item);
                    foreach (var propertyName in changedPropertyNames)
                    {
                        this.Entry(item).Property(propertyName).IsModified = true;
                    }
                    this.SaveChanges();
                }
                else
                {
                    item.Updated = DateTime.Now;
                    this.SupplierItems.Add(item);
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
                string header = string.Format("SupplierItemAdd UPC: {0} MPN: {1}", UPC, MPN);
                ret = dsutil.DSUtil.ErrMsg(header, exc);
                dsutil.DSUtil.WriteFile(_logfile, ret, "admin");
            }
        }
        
        public void SupplierItemUpdate(SupplierItem item, params string[] changedPropertyNames)
        {
            string ret = null;
            try
            {
                var found = this.SupplierItems.AsNoTracking().SingleOrDefault(p => p.ID == item.ID);
                if (found == null)
                {
                    this.SupplierItems.Add(item);
                    this.SaveChanges();
                }
                else
                {
                    item.ID = found.ID;
                    this.SupplierItems.Attach(item);
                    foreach (var propertyName in changedPropertyNames)
                    {
                        this.Entry(item).Property(propertyName).IsModified = true;
                    }
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
                string header = string.Format("SupplierItemUpdate ID: {0}", item.ID);
                ret = dsutil.DSUtil.ErrMsg(header, exc);
                dsutil.DSUtil.WriteFile(_logfile, ret, "admin");
            }
        }
        
        public bool IsVERO(string brand)
        {
            var exists = this.VEROBrands.SingleOrDefault(p => p.Brand == brand);
            return (exists != null);
        }

        public int? LatestRptNumber(string seller)
        {
            try
            {
                var rpt = this.SearchHistory.Where(p => p.Seller == seller).OrderByDescending(item => item.ID).FirstOrDefault();
                //var items = from t1 in this.SearchHistory.Where(p => p.Seller == seller)
                //            join t2 in this.OrderHistory on t1.ID equals t2.RptNumber into notes
                //            select notes.Max(x => (int?)x.RptNumber);
                //var rpt = items.FirstOrDefault();
                if (rpt != null)
                {
                    return rpt.ID;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception exc)
            {
                string ret = dsutil.DSUtil.ErrMsg("LatestRptNumber", exc);
                dsutil.DSUtil.WriteFile(_logfile, ret, "admin");
                return null;
            }
        }

        /// <summary>
        /// Given itemID, get the supplier item - first need product id from ItemSpecifics
        /// </summary>
        /// <param name="itemID"></param>
        /// <returns></returns>
        public SupplierItem GetSupplierItem(string itemID)
        {
            SupplierItem supplierItem = null;
            bool isUPC = false;
            var spec = this.OrderHistoryItemSpecifics.FirstOrDefault(p => p.SellerItemID == itemID && p.ItemName == "UPC");
            if (spec == null)
            {
                spec = this.OrderHistoryItemSpecifics.FirstOrDefault(p => p.SellerItemID == itemID && p.ItemName == "MPN");
            }
            else
            {
                isUPC = true;
            }
            if (spec != null)
            {
                if (isUPC)
                {
                    // 12.24.2019 Found case where UPC is duplicated in SupplierItem - might have something to do with first locating
                    // an MPN which might have various versions that lead back to same UPC
                    // Need to decide how to handle.
                    // For now, use FirstOrdDefault instead of SingleOrDefault.
                    supplierItem = this.SupplierItems.FirstOrDefault(p => p.UPC == spec.ItemValue);
                    // seller might supply both UPC and MPN (in ItemSpecifics) but both were not collected off supplier website
                    if (supplierItem == null)
                    {
                        spec = this.OrderHistoryItemSpecifics.FirstOrDefault(p => p.SellerItemID == itemID && p.ItemName == "MPN");
                        if (spec != null)
                        {
                            supplierItem = this.SupplierItems.SingleOrDefault(p => p.MPN == spec.ItemValue);
                        }
                    }
                }
                else
                {
                    supplierItem = this.SupplierItems.SingleOrDefault(p => p.MPN == spec.ItemValue);
                }
            }
            return supplierItem;
        }
        public List<SearchHistory> GetSellers(int storeID)
        {
            var sellers = this.SearchHistory.Where(p => p.StoreID == storeID).ToList();
            return sellers;
        }
    }
}
