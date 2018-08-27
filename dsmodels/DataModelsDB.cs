using System;
using System.Collections.Generic;
using System.Data.Entity;
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

        public DbSet<PostedListing> PostedListings { get; set; }

        //public async Task<PostedListing> GetPostedListing(string listedItemId)
        //{
        //    var found = await this.PostedListings.FirstOrDefaultAsync(r => r.ListedItemID == listedItemId);
        //    return found;
        //}

        public async Task<PostedListing> GetPostedListing(int sourceID, string supplierItemID)
        {
            var found = await this.PostedListings.FirstOrDefaultAsync(r => r.SourceID == sourceID && r.SupplierItemID == supplierItemID);
            return found;
        }

        public async Task<PostedListing> GetPostedListing(string itemId)
        {
            var found = await this.PostedListings.FirstOrDefaultAsync(r => r.EbayItemID == itemId);
            return found;
        }

        // listedItemId is my id
        public async Task<PostedListing> GetPostedListingFromListId(string listedItemId)
        {
            var found = await this.PostedListings.FirstOrDefaultAsync(r => r.ListedItemID == listedItemId);
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

        public async Task PostedListingSaveAsync(PostedListing listing)
        {
            var found = await this.PostedListings.FirstOrDefaultAsync(r => r.SourceID == listing.SourceID && r.SupplierItemID == listing.SupplierItemID);
            if (found == null)
                PostedListings.Add(listing);
            else
            {
                found.EbaySeller = listing.EbaySeller;
                found.Price = listing.Price;
                found.CategoryID = listing.CategoryID;
                found.SupplierItemID = listing.SupplierItemID;
                found.SourceUrl = listing.SourceUrl;
                found.Pictures = listing.Pictures;
                found.Title = listing.Title;
                found.Title = listing.Title;
                found.EbayUrl = listing.EbayUrl;
                found.PrimaryCategoryID = listing.PrimaryCategoryID;
                found.PrimaryCategoryName = listing.PrimaryCategoryName;
                found.Description = listing.Description;
                found.SourceID = listing.SourceID;  // 1=sams
                found.ListedQty = listing.ListedQty;
                this.Entry(found).State = EntityState.Modified;
            }
            await this.SaveChangesAsync();
        }

        public static PostedListing CopyStagedToPostedListing(StagedListing staged)
        {
            var p = new PostedListing();

            p.SourceID = staged.SourceID;
            p.SupplierItemID = staged.SupplierItemID;
            p.SourceUrl = staged.SourceUrl;
            p.SupplierPrice = staged.SupplierPrice;
            p.Title = staged.Title;
            p.Price = staged.Price;
            p.Description = staged.Description;
            p.Pictures = staged.Pictures;
            p.CategoryID = staged.CategoryID;
            p.PrimaryCategoryID = staged.PrimaryCategoryID;
            p.PrimaryCategoryName = staged.PrimaryCategoryName;
            p.ListedItemID = staged.ListedItemID;
            p.Listed = staged.Listed;
            p.Removed = staged.Removed;
            p.ListedQty = staged.ListedQty;
            p.InventoryException = staged.InventoryException;

            return p;
        }

        public async Task<bool> UpdatePrice(PostedListing listing, decimal price)
        {
            bool ret = false;
            var rec = await this.PostedListings.FirstOrDefaultAsync(r => r.ListedItemID == listing.ListedItemID);
            if (rec != null)
            {
                ret = true;
                rec.Price = price;
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


    }
}
