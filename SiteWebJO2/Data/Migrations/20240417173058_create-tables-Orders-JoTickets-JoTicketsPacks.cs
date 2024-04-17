using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SiteWebJO2.Data.Migrations
{
    /// <inheritdoc />
    public partial class createtablesOrdersJoTicketsJoTicketsPacks : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "JoTicketPacks",
                columns: table => new
                {
                    JoTicketPackId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    JoTicketPackName = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    NbAttendees = table.Column<int>(type: "int", nullable: false),
                    ReductionRate = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    JoTicketPackStatus = table.Column<bool>(type: "tinyint(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JoTicketPacks", x => x.JoTicketPackId);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Orders",
                columns: table => new
                {
                    OrderId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    ApplicationUserId = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    OrderDate = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    OrderStatus = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    OrderAmount = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    TransactionId = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Orders", x => x.OrderId);
                    table.ForeignKey(
                        name: "FK_Orders_AspNetUsers_ApplicationUserId",
                        column: x => x.ApplicationUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "JoTickets",
                columns: table => new
                {
                    JoTicketId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    ApplicationUserId = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    JoTicketKey = table.Column<byte[]>(type: "longblob", nullable: false),
                    JoTicketPackId = table.Column<int>(type: "int", nullable: false),
                    JoSessionId = table.Column<int>(type: "int", nullable: false),
                    JoTicketStatus = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    JoTicketPrice = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    OrderId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JoTickets", x => x.JoTicketId);
                    table.ForeignKey(
                        name: "FK_JoTickets_AspNetUsers_ApplicationUserId",
                        column: x => x.ApplicationUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_JoTickets_JoSessions_JoSessionId",
                        column: x => x.JoSessionId,
                        principalTable: "JoSessions",
                        principalColumn: "JoSessionId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_JoTickets_JoTicketPacks_JoTicketPackId",
                        column: x => x.JoTicketPackId,
                        principalTable: "JoTicketPacks",
                        principalColumn: "JoTicketPackId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_JoTickets_Orders_OrderId",
                        column: x => x.OrderId,
                        principalTable: "Orders",
                        principalColumn: "OrderId",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_JoTickets_ApplicationUserId",
                table: "JoTickets",
                column: "ApplicationUserId");

            migrationBuilder.CreateIndex(
                name: "IX_JoTickets_JoSessionId",
                table: "JoTickets",
                column: "JoSessionId");

            migrationBuilder.CreateIndex(
                name: "IX_JoTickets_JoTicketPackId",
                table: "JoTickets",
                column: "JoTicketPackId");

            migrationBuilder.CreateIndex(
                name: "IX_JoTickets_OrderId",
                table: "JoTickets",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_ApplicationUserId",
                table: "Orders",
                column: "ApplicationUserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "JoTickets");

            migrationBuilder.DropTable(
                name: "JoTicketPacks");

            migrationBuilder.DropTable(
                name: "Orders");
        }
    }
}
