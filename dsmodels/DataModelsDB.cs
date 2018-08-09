using System;
using System.Collections.Generic;
using System.Data.Entity;
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

        public async Task<PostedListing> GetPostedListing(string listedItemId)
        {
            var found = await this.PostedListings.FirstOrDefaultAsync(r => r.ListedItemID == listedItemId);
            return found;
        }

    }
}
