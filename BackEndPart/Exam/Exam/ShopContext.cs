using System;
using System.Data.Entity;
namespace Exam
{
	public class ShopContext : DbContext
	{
		public DbSet<Clothes> Clothes { get; set; }

		public ShopContext(String connectionString) : base(connectionString)
		{
		}

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
			modelBuilder.HasDefaultSchema("public");
			base.OnModelCreating(modelBuilder);

        }
    }
}

