using MataPizza.Backend.Models;
using Microsoft.EntityFrameworkCore;

namespace MataPizza.Backend.Data
{
    public class MataPizzaDbContext : DbContext
    {
        public MataPizzaDbContext(DbContextOptions<MataPizzaDbContext> options) : base(options)
        {

        }
        public DbSet<Pizza> Pizzas { get; set; }
        public DbSet<PizzaType> PizzaTypes { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderDetail> OrderDetails { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configure the Pizza entity
            modelBuilder.Entity<Pizza>(entity =>
            {
                entity.HasKey(p => p.PizzaId);
                entity.Property(p => p.PizzaId)
                    .HasColumnName("pizza_id");
                entity.Property(p => p.PizzaTypeId)
                    .HasColumnName("pizza_type_id")
                    .IsRequired();
                entity.Property(p => p.Size)
                    .HasColumnName("size")
                    .IsRequired();
                entity.Property(p => p.Price)
                    .HasColumnName("price")
                    .HasColumnType("decimal(6,2)");

                entity.HasOne(p => p.PizzaType)
                        .WithMany(pt => pt.Pizzas)
                        .HasForeignKey(p => p.PizzaTypeId);
            });
            // Configure the PizzaType entity
            modelBuilder.Entity<PizzaType>(entity =>
            {
                entity.HasKey(pt => pt.PizzaTypeId);
                entity.Property(pt => pt.PizzaTypeId)
                    .HasColumnName("pizza_type_id");
                entity.Property(pt => pt.Name)
                    .HasColumnName("name")
                    .IsRequired();
                entity.Property(pt => pt.Category)
                    .HasColumnName("category")
                    .IsRequired();
                entity.Property(pt => pt.Ingredients)
                    .HasColumnName("ingredients");
            });
            // Configure the Order entity
            modelBuilder.Entity<Order>(entity =>
            {
                entity.HasKey(o => o.OrderId);
                entity.Property(o => o.OrderId)
                    .HasColumnName("order_id");
                entity.Property(o => o.OrderDate)
                    .HasColumnName("date")
                    .IsRequired();
                entity.Property(o => o.OrderTime)
                    .HasColumnName("time")
                    .IsRequired();
            });
            // Configure the OrderDetail entity
            modelBuilder.Entity<OrderDetail>(entity =>
            {
                entity.HasKey(od => od.OrderDetailId);
                entity.Property(od => od.OrderDetailId)
                    .HasColumnName("order_details_id");
                entity.Property(od => od.OrderId)
                    .HasColumnName("order_id")
                    .IsRequired();
                entity.Property(od => od.PizzaId)
                    .HasColumnName("pizza_id")
                    .IsRequired();
                entity.Property(od => od.Quantity)
                    .HasColumnName("quantity")
                    .IsRequired();

                entity.HasOne(od => od.Order)
                    .WithMany(o => o.OrderDetails)
                    .HasForeignKey(od => od.OrderId);

                entity.HasOne(od => od.Pizza)
                    .WithMany(p => p.OrderDetails)
                    .HasForeignKey(od => od.PizzaId);
            });
        }
    }
}
