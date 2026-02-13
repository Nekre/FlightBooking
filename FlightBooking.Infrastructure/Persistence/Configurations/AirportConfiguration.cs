using FlightBooking.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FlightBooking.Infrastructure.Persistence.Configurations;

public class AirportConfiguration : IEntityTypeConfiguration<Airport>
{
    public void Configure(EntityTypeBuilder<Airport> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Code)
            .IsRequired()
            .HasMaxLength(3);

        builder.Property(x => x.Name).IsRequired().HasMaxLength(200);
        
        builder.HasData(
            new Airport { Id = 1, Code = "IST", Name = "İstanbul Havalimanı", City = "İstanbul" },
            new Airport { Id = 2, Code = "ESB", Name = "Esenboğa Havalimanı", City = "Ankara" },
            new Airport { Id = 3, Code = "LHR", Name = "London Heathrow", City = "London" },
            new Airport { Id = 4, Code = "AYT", Name = "Antalya Havalimanı", City = "Antalya" },
            new Airport { Id = 5, Code = "IZM", Name = "İzmir Alsancak Havalimanı", City = "İzmir" },
            new Airport { Id = 6, Code = "GZT", Name = "Gaziantep Havalimanı", City = "Gaziantep" },
            new Airport { Id = 7, Code = "KYA", Name = "Kayseri Havalimanı", City = "Kayseri" },
            new Airport { Id = 8, Code = "GNY", Name = "Adana Havalimanı", City = "Adana" },
            new Airport { Id = 9, Code = "SAW", Name = "Sabiha Gökçen Havalimanı", City = "İstanbul" },
            new Airport { Id = 10, Code = "DLM", Name = "Dalaman Havalimanı", City = "Muğla" },
            new Airport { Id = 11, Code = "NYC", Name = "John F. Kennedy Airport", City = "New York" },
            new Airport { Id = 12, Code = "LAX", Name = "Los Angeles International", City = "Los Angeles" },
            new Airport { Id = 13, Code = "CDG", Name = "Paris Charles de Gaulle", City = "Paris" },
            new Airport { Id = 14, Code = "AMS", Name = "Amsterdam Schiphol", City = "Amsterdam" },
            new Airport { Id = 15, Code = "DXB", Name = "Dubai International", City = "Dubai" },
            new Airport { Id = 16, Code = "NRT", Name = "Narita International", City = "Tokyo" },
            new Airport { Id = 17, Code = "SYD", Name = "Sydney Airport", City = "Sydney" },
            new Airport { Id = 18, Code = "SIN", Name = "Singapore Changi", City = "Singapore" }
        );
    }
}