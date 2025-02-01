using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Termostat.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddMieszkanieModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Mieszkania",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LiczbaOkien = table.Column<int>(type: "int", nullable: false),
                    LiczbaPokoi = table.Column<int>(type: "int", nullable: false),
                    BazowaTemperatura = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Mieszkania", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Mieszkania_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "MieszkanieWspolokatorzy",
                columns: table => new
                {
                    MieszkanieId = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MieszkanieWspolokatorzy", x => new { x.MieszkanieId, x.UserId });
                    table.ForeignKey(
                        name: "FK_MieszkanieWspolokatorzy_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MieszkanieWspolokatorzy_Mieszkania_MieszkanieId",
                        column: x => x.MieszkanieId,
                        principalTable: "Mieszkania",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Mieszkania_UserId",
                table: "Mieszkania",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_MieszkanieWspolokatorzy_UserId",
                table: "MieszkanieWspolokatorzy",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MieszkanieWspolokatorzy");

            migrationBuilder.DropTable(
                name: "Mieszkania");
        }
    }
}
