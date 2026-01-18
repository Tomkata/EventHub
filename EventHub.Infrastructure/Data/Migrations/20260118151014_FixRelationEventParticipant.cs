using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EventHub.Data.Migrations
{
    /// <inheritdoc />
    public partial class FixRelationEventParticipant : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EventParticipants_Events_EventId1",
                table: "EventParticipants");

            migrationBuilder.DropIndex(
                name: "IX_EventParticipants_EventId1",
                table: "EventParticipants");

            migrationBuilder.DropColumn(    
                name: "EventId1",
                table: "EventParticipants");
        }       

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "EventId1",
                table: "EventParticipants",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_EventParticipants_EventId1",
                table: "EventParticipants",
                column: "EventId1");

            migrationBuilder.AddForeignKey(
                name: "FK_EventParticipants_Events_EventId1",
                table: "EventParticipants",
                column: "EventId1",
                principalTable: "Events",
                principalColumn: "Id");
        }
    }
}
