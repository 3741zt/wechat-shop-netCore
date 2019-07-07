using ztbiyesheji.Models;
using Microsoft.EntityFrameworkCore;

namespace ztbiyesheji.EntityFramework
{
    public class DemoDbContext : DbContext { 


         public DemoDbContext(DbContextOptions<DemoDbContext> options) : base(options)
    {
    }
        public DbSet<AdminUser> AdminUser { get; set; }
        public DbSet<UserAddresses> UserAddresses { get; set; }
        public DbSet<GoodsDetail> GoodsDetail { get; set; }
        public DbSet<GoodsDetailImage> GoodsDetailImage { get; set; }
        public DbSet<GoodsClassification> GoodsClassification { get; set; }
        public DbSet<RotationImage> RotationImage { get; set; }
        public DbSet<GoodsCommodities> GoodsCommodities { get; set; }
        public DbSet<GoodsInfo> GoodsInfo { get; set; }
        public DbSet<UserApp> UserApp { get; set; }
        public DbSet<GoodsCarts> GoodsCarts { get; set; }
        public DbSet<GoodsOrders> GoodsOrders { get; set; }
        public DbSet<UserGoodsCollection> UserGoodsCollection { get; set; }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
    }
}
}
