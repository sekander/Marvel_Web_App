using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MarvelWebApp.Models;

namespace MarvelWebApp.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<Comic> Comics { get; set; }
        // public DbSet<Category> Categories { get; set; }
        public DbSet<ShoppingCart> ShoppingCarts { get; set; }
        public DbSet<ShoppingCartItem> ShoppingCartItems { get; set; }
        public DbSet<Payment> Payments { get; set; }


        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

             // Configure the IdentityRole entity properties
            builder.Entity<IdentityRole>(entity =>
            {
                // Set the Id column length to 191, which is within the 767-byte limit
                entity.Property(e => e.Id)
                    .HasMaxLength(191);  // Set the Id column length to 191 characters to avoid index size issue

                // Limit column length to 191 characters for utf8mb4 compatibility
                entity.Property(e => e.Name)
                    .HasMaxLength(191)  // Set the max length for the Name field
                    .HasColumnType("varchar(191)");  // Explicitly set the column type

                entity.Property(e => e.NormalizedName)
                    .HasMaxLength(191)  // Set the max length for the NormalizedName field
                    .HasColumnType("varchar(191)");  // Explicitly set the column type

                entity.Property(e => e.ConcurrencyStamp)
                    .HasMaxLength(191)  // Set the max length for the ConcurrencyStamp field
                    .HasColumnType("varchar(191)");  // Explicitly set the column type
            });

            builder.Entity<ApplicationUser>(entity =>
            {
                entity.Property(e => e.Id)
                    .HasMaxLength(191);  // Set UserName column length to 191
                // Set the maximum length of UserName and NormalizedUserName to 191 to avoid index size issues
                entity.Property(e => e.UserName)
                    .HasMaxLength(191);  // Set UserName column length to 191

                entity.Property(e => e.NormalizedUserName)
                    .HasMaxLength(191);  // Set NormalizedUserName column length to 191

                // Set the maximum length of Email and NormalizedEmail to 191
                entity.Property(e => e.Email)
                    .HasMaxLength(191);  // Set Email column length to 191

                entity.Property(e => e.NormalizedEmail)
                    .HasMaxLength(191);  // Set NormalizedEmail column length to 191

                // Set the ConcurrencyStamp column length to 191
                entity.Property(e => e.ConcurrencyStamp)
                    .HasMaxLength(191);  // Set ConcurrencyStamp column length to 191

                // Optional: Set the PasswordHash, SecurityStamp, and other columns if required
                entity.Property(e => e.PasswordHash)
                    .HasMaxLength(191);  // Set PasswordHash column length to 191

                entity.Property(e => e.SecurityStamp)
                    .HasMaxLength(191);  // Set SecurityStamp column length to 191

            });

            // Configure the IdentityUserLogin table to fix column size issues
            builder.Entity<IdentityUserLogin<string>>(entity =>
            {
                entity.Property(e => e.LoginProvider).HasMaxLength(191);  // Reduce length
                entity.Property(e => e.ProviderKey).HasMaxLength(191);  // Reduce length
                entity.Property(e => e.ProviderDisplayName).HasMaxLength(191);  // Optional, reduce if needed
            });

             // Configure AspNetUserTokens to fix column size issues
            builder.Entity<IdentityUserToken<string>>(entity =>
            {
                // Reduce the length of these columns and ensure the right charset
                entity.Property(e => e.LoginProvider).HasMaxLength(191).HasCharSet("utf8");  // Reduce length and use utf8 charset
                entity.Property(e => e.Name).HasMaxLength(191).HasCharSet("utf8");  // Reduce length and use utf8 charset
                entity.Property(e => e.Value).HasColumnType("TEXT").HasCharSet("utf8");  // Use TEXT for longtext columns if not indexed
            });




 // Configure relationships
            builder.Entity<Order>()
                .HasOne(o => o.User)
                .WithMany(u => u.Orders)
                .HasForeignKey(o => o.UserID);

            builder.Entity<OrderItem>()
                .HasOne(oi => oi.Order)
                .WithMany(o => o.OrderItems)
                .HasForeignKey(oi => oi.OrderID);

            builder.Entity<OrderItem>()
                .HasOne(oi => oi.Comic)
                .WithMany(c => c.OrderItems)
                .HasForeignKey(oi => oi.ComicID);

            // builder.Entity<Comic>()
            //     .HasOne(c => c.Category)
            //     .WithMany(cat => cat.Comics)
            //     .HasForeignKey(c => c.CategoryID);

            builder.Entity<ShoppingCartItem>()
                .HasOne(sci => sci.ShoppingCart)
                .WithMany(sc => sc.ShoppingCartItems)
                .HasForeignKey(sci => sci.ShoppingCartID);

            builder.Entity<ShoppingCartItem>()
                .HasOne(sci => sci.Comic)
                .WithMany(c => c.ShoppingCartItems)
                // .HasForeignKey(sci => sci.);
                .HasForeignKey(sci => sci.ComicID);

            builder.Entity<Payment>()
                .HasOne(p => p.Order)
                .WithOne(o => o.Payment)
                .HasForeignKey<Payment>(p => p.OrderID);
            


            // You might also want to add an index on the NormalizedName if it is part of the query.
            //  builder.Entity<IdentityRole>()
                //  .HasIndex(r => r.NormalizedName)
                //  .HasName("IX_Roles_NormalizedName")
                //  .HasDatabaseName("IX_Roles_NormalizedName");  // Ensure that the index uses a prefix size to avoid the 767-byte limit
                 // .HasColumnType("varchar(191)");  // Ensure the column type is correctly set for utf8mb4
                 // .HasMaxLength(191);  // Ensure that the index uses a prefix size to avoid the 767-byte limit
            }

        // Add DbSets for any other models if needed
    }
}

