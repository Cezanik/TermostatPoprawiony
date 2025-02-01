using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Termostat.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddZaproszenia : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Zaproszenia",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NadawcaId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    OdbiorcaId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    DataWyslania = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Zaproszenia", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Zaproszenia_AspNetUsers_NadawcaId",
                        column: x => x.NadawcaId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Zaproszenia_AspNetUsers_OdbiorcaId",
                        column: x => x.OdbiorcaId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Zaproszenia_NadawcaId",
                table: "Zaproszenia",
                column: "NadawcaId");

            migrationBuilder.CreateIndex(
                name: "IX_Zaproszenia_OdbiorcaId",
                table: "Zaproszenia",
                column: "OdbiorcaId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Zaproszenia");
        }
    }
}
