using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Termostat.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddHarmonogramModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Harmonogram",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nazwa = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Start = table.Column<int>(type: "int", nullable: false),
                    End = table.Column<int>(type: "int", nullable: false),
                    DocelowaTemperatura = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    MieszkanieId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Harmonogram", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Harmonogram_Mieszkania_MieszkanieId",
                        column: x => x.MieszkanieId,
                        principalTable: "Mieszkania",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Harmonogram_MieszkanieId",
                table: "Harmonogram",
                column: "MieszkanieId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Harmonogram");
        }
    }
}
