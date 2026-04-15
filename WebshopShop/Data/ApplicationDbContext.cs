using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using WebshopShop.Models;

namespace WebshopShop.Data
{
    public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : IdentityDbContext(options)
    {
        public DbSet<Category> Categories { get; set; }
        public DbSet<Manufacturer> Manufacturers { get; set; }
        public DbSet<Address> Addresses { get; set; }
        public DbSet<Item> Items { get; set; }
        public DbSet<Picture> Pictures { get; set; }
        public DbSet<Cart> Carts { get; set; }
        public DbSet<CartItem> CartItems { get; set; }
        public DbSet<Invoice> Invoices { get; set; }
        public DbSet<ItemInvoice> ItemInvoices { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<News> News { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<CartItem>()
                .HasKey(cartItem => new { cartItem.CartId, cartItem.ItemId });

            modelBuilder.Entity<ItemInvoice>()
                .HasKey(itemInvoice => new { itemInvoice.ItemId, itemInvoice.InvoiceId });

            modelBuilder.Entity<Invoice>()
                .HasOne(invoice => invoice.CustomerAddress)
                .WithMany(address => address.CustomerInvoices)
                .HasForeignKey(invoice => invoice.CustomerAddressId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Invoice>()
                .HasOne(invoice => invoice.SellerAddress)
                .WithMany(address => address.SellerInvoices)
                .HasForeignKey(invoice => invoice.SellerAddressId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Invoice>()
                .Property(invoice => invoice.Total)
                .HasPrecision(18, 2);

            modelBuilder.Entity<Item>()
                .Property(item => item.Price)
                .HasPrecision(18, 2);

            modelBuilder.Entity<ItemInvoice>()
                .Property(itemInvoice => itemInvoice.Price)
                .HasPrecision(18, 2);

            modelBuilder.Entity<Review>()
                .Property(review => review.Score)
                .HasPrecision(3, 1);
        }
    }
}