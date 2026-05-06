using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography.Xml;
using Web.Models;

namespace Web.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options): base(options){}

    public DbSet<ApplicationUser> Users => Set<ApplicationUser>();
    public DbSet<Station> Stations => Set<Station>();
    public DbSet<Bike> Bikes => Set<Bike>();
    public DbSet<Rental> Rentals => Set<Rental>();

    public DbSet<BikeStatusHistory> BikeStatusHistory => Set<BikeStatusHistory>();



    // Nastavi vazby a seed databaze.
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);


        modelBuilder.Entity<ApplicationUser>(entity =>
        {
            entity.ToTable("Users");
            entity.HasIndex(user => user.Email).IsUnique();
        });

        modelBuilder.Entity<Station>(entity =>
        {
            entity.ToTable("Stations");
            entity.Property(station => station.Latitude).HasPrecision(9, 6);
            entity.Property(station => station.Longitude).HasPrecision(9, 6);
        }
        );

        modelBuilder.Entity<Bike>(entity =>
        {
            entity.ToTable("Bikes");
            entity.HasIndex(bike => bike.Code).IsUnique();
            entity.Property(bike => bike.Status).HasConversion<string>().HasMaxLength(30);

            entity.HasOne(bike => bike.CurrentStation)
                .WithMany(station => station.Bikes)
                .HasForeignKey(bike => bike.CurrentStationId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        modelBuilder.Entity<Rental>(entity =>
        {
            entity.ToTable("Rentals");
            entity.Property(rental => rental.Price).HasPrecision(10, 2);
            entity.HasOne(rental => rental.User).WithMany(user => user.Rentals)
                .HasForeignKey(rental => rental.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(rental => rental.Bike).WithMany(bike => bike.Rentals)
                .HasForeignKey(rental => rental.BikeId)
                .OnDelete(DeleteBehavior.Restrict);


            entity.HasOne(rental => rental.StartStation).WithMany(station => station.StartedRentals)
                .HasForeignKey(rental => rental.StartStationId)
                .OnDelete(DeleteBehavior.Restrict);


            entity.HasOne(rental => rental.EndStation).WithMany(station => station.FinishedRentals)
                .HasForeignKey(rental => rental.EndStationId)
                .OnDelete(DeleteBehavior.Restrict);


        });



        modelBuilder.Entity<BikeStatusHistory>(entity =>
        {
            entity.ToTable("BikeStatusHistory");
            entity.Property(history => history.NewStatus).HasConversion<string>().HasMaxLength(30);

            entity.HasOne(history => history.Bike)
                .WithMany(bike => bike.StatusHistory)
                .HasForeignKey(history => history.BikeId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(history => history.Station)
                .WithMany()
                .HasForeignKey(history => history.StationId)
                .OnDelete(DeleteBehavior.SetNull);


            entity.HasOne(history => history.Rental)
                .WithMany()
                .HasForeignKey(history => history.RentalId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        modelBuilder.Entity<Station>().HasData(
            new Station { Id = 1, Name = "Skola", Address = "Vsb", Latitude = 1.000000m, Longitude = 1.000000m },
            new Station { Id = 2, Name = "Frydek", Address = "Frydecka", Latitude = 2.000000m, Longitude = 2.000000m },
            new Station { Id = 3, Name = "Koleje", Address = "Studentska", Latitude = 3.000000m, Longitude = 3.000000m }
        );

        modelBuilder.Entity<Bike>().HasData(
            new Bike { Id = 1, Code = "BIkE-001", Status = BikeStatus.Available, CurrentStationId = 1 },
            new Bike { Id = 2, Code = "BIkE-002", Status = BikeStatus.Available, CurrentStationId = 1 },
            new Bike { Id = 3, Code = "BIkE-003", Status = BikeStatus.Available, CurrentStationId = 2 },
            new Bike { Id = 4, Code = "BIkE-004", Status = BikeStatus.Service, CurrentStationId = 3 },
            new Bike { Id = 5, Code = "BIkE-005", Status = BikeStatus.Available, CurrentStationId = 3}
        );
    }
}
