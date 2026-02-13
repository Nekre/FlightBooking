using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace FlightBooking.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Airports",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    City = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Airports", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "Airports",
                columns: new[] { "Id", "City", "Code", "Name" },
                values: new object[,]
                {
                    { 1, "İstanbul", "IST", "İstanbul Havalimanı" },
                    { 2, "Ankara", "ESB", "Esenboğa Havalimanı" },
                    { 3, "London", "LHR", "London Heathrow" },
                    { 4, "Antalya", "AYT", "Antalya Havalimanı" },
                    { 5, "İzmir", "IZM", "İzmir Alsancak Havalimanı" },
                    { 6, "Gaziantep", "GZT", "Gaziantep Havalimanı" },
                    { 7, "Kayseri", "KYA", "Kayseri Havalimanı" },
                    { 8, "Adana", "GNY", "Adana Havalimanı" },
                    { 9, "İstanbul", "SAW", "Sabiha Gökçen Havalimanı" },
                    { 10, "Muğla", "DLM", "Dalaman Havalimanı" },
                    { 11, "New York", "NYC", "John F. Kennedy Airport" },
                    { 12, "Los Angeles", "LAX", "Los Angeles International" },
                    { 13, "Paris", "CDG", "Paris Charles de Gaulle" },
                    { 14, "Amsterdam", "AMS", "Amsterdam Schiphol" },
                    { 15, "Dubai", "DXB", "Dubai International" },
                    { 16, "Tokyo", "NRT", "Narita International" },
                    { 17, "Sydney", "SYD", "Sydney Airport" },
                    { 18, "Singapore", "SIN", "Singapore Changi" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Airports");
        }
    }
}
