using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace EventHub.Data.Migrations
{
    /// <inheritdoc />
    public partial class SeedEventsData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Events",
                columns: new[] { "Id", "Address", "CategoryId", "CreatedAt", "Description", "EndDate", "ImagePath", "LocationId", "MaxParticipants", "OrganizerId", "StartDate", "Title" },
                values: new object[,]
                {
                    { new Guid("a0000001-0000-0000-0000-000000000001"), "Rowing Canal, Plovdiv", new Guid("11111111-1111-1111-1111-111111111111"), new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Open-air rock concert featuring Bulgarian bands.", new DateTime(2026, 6, 20, 23, 30, 0, 0, DateTimeKind.Unspecified), "images/events/concert.jpg", new Guid("46ee85a7-5da3-42b3-96d2-b58fd2b8cfc9"), 800, "9dbe8efe-5e69-44a9-aacb-e75f81724fc5", new DateTime(2026, 6, 20, 18, 0, 0, 0, DateTimeKind.Unspecified), "Rock the Night Festival" },
                    { new Guid("a0000002-0000-0000-0000-000000000002"), "Inter Expo Center, Sofia", new Guid("22222222-2222-2222-2222-222222222222"), new DateTime(2025, 1, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), "Annual conference for software engineers and tech leaders.", new DateTime(2026, 3, 16, 17, 0, 0, 0, DateTimeKind.Unspecified), "images/events/conference.jpg", new Guid("b285237b-5ddc-449f-bf3a-c9cf5e805910"), 600, "9dbe8efe-5e69-44a9-aacb-e75f81724fc5", new DateTime(2026, 3, 15, 9, 0, 0, 0, DateTimeKind.Unspecified), "Tech Innovators Conference 2026" },
                    { new Guid("a0000003-0000-0000-0000-000000000003"), "Sea Garden, Varna", new Guid("33333333-3333-3333-3333-333333333333"), new DateTime(2025, 1, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), "Annual half marathon open to professionals and amateurs.", new DateTime(2026, 4, 10, 14, 0, 0, 0, DateTimeKind.Unspecified), "images/events/spot-event.jpg", new Guid("2975d8e3-7c0c-4a92-a7e2-655b2cb349f4"), 1000, "9dbe8efe-5e69-44a9-aacb-e75f81724fc5", new DateTime(2026, 4, 10, 8, 0, 0, 0, DateTimeKind.Unspecified), "City Half Marathon" },
                    { new Guid("a0000004-0000-0000-0000-000000000004"), "Art Gallery Boris Denev", new Guid("44444444-4444-4444-4444-444444444444"), new DateTime(2025, 1, 12, 0, 0, 0, 0, DateTimeKind.Unspecified), "Exhibition of contemporary Bulgarian artists.", new DateTime(2026, 5, 20, 19, 0, 0, 0, DateTimeKind.Unspecified), "images/events/spot-event.jpg", new Guid("c85b0784-ea5b-4e53-b0e1-527e5963bfef"), 300, "9dbe8efe-5e69-44a9-aacb-e75f81724fc5", new DateTime(2026, 5, 5, 10, 0, 0, 0, DateTimeKind.Unspecified), "Modern Art Expo" },
                    { new Guid("a0000005-0000-0000-0000-000000000005"), "Tech Hub Ruse", new Guid("55555555-5555-5555-5555-555555555555"), new DateTime(2025, 1, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), "Practical workshop covering EF Core and Web APIs.", new DateTime(2026, 2, 22, 17, 0, 0, 0, DateTimeKind.Unspecified), "images/events/workshop.jpg", new Guid("d80720a5-6e69-44a5-87dd-997df1e4ddc8"), 50, "9dbe8efe-5e69-44a9-aacb-e75f81724fc5", new DateTime(2026, 2, 22, 10, 0, 0, 0, DateTimeKind.Unspecified), "ASP.NET Core Hands-on Workshop" },
                    { new Guid("a0000006-0000-0000-0000-000000000006"), "Central Beach, Burgas", new Guid("66666666-6666-6666-6666-666666666666"), new DateTime(2025, 1, 18, 0, 0, 0, 0, DateTimeKind.Unspecified), "Street food, live music, and local craft beer.", new DateTime(2026, 7, 12, 23, 0, 0, 0, DateTimeKind.Unspecified), "images/events/festival-event.jpg", new Guid("fa600614-8aa2-441f-86fa-3b04bd8fb796"), 900, "9dbe8efe-5e69-44a9-aacb-e75f81724fc5", new DateTime(2026, 7, 10, 12, 0, 0, 0, DateTimeKind.Unspecified), "Summer Food & Music Festival" },
                    { new Guid("a0000007-0000-0000-0000-000000000007"), "Coworking Space Sofia", new Guid("77777777-7777-7777-7777-777777777777"), new DateTime(2025, 1, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), "Networking meetup for startup founders and investors.", new DateTime(2026, 1, 30, 21, 0, 0, 0, DateTimeKind.Unspecified), "images/events/meet-up.jpg", new Guid("b285237b-5ddc-449f-bf3a-c9cf5e805910"), 120, "9dbe8efe-5e69-44a9-aacb-e75f81724fc5", new DateTime(2026, 1, 30, 18, 30, 0, 0, DateTimeKind.Unspecified), "Startup Founders Meetup" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Events",
                keyColumn: "Id",
                keyValue: new Guid("a0000001-0000-0000-0000-000000000001"));

            migrationBuilder.DeleteData(
                table: "Events",
                keyColumn: "Id",
                keyValue: new Guid("a0000002-0000-0000-0000-000000000002"));

            migrationBuilder.DeleteData(
                table: "Events",
                keyColumn: "Id",
                keyValue: new Guid("a0000003-0000-0000-0000-000000000003"));

            migrationBuilder.DeleteData(
                table: "Events",
                keyColumn: "Id",
                keyValue: new Guid("a0000004-0000-0000-0000-000000000004"));

            migrationBuilder.DeleteData(
                table: "Events",
                keyColumn: "Id",
                keyValue: new Guid("a0000005-0000-0000-0000-000000000005"));

            migrationBuilder.DeleteData(
                table: "Events",
                keyColumn: "Id",
                keyValue: new Guid("a0000006-0000-0000-0000-000000000006"));

            migrationBuilder.DeleteData(
                table: "Events",
                keyColumn: "Id",
                keyValue: new Guid("a0000007-0000-0000-0000-000000000007"));
        }
    }
}
