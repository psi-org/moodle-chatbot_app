using Microsoft.EntityFrameworkCore.Migrations;

namespace MoodleBot.Models.Migrations
{
    public partial class AddNewLanguageCodeColumnInBotMessageTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "LanguageISOCode",
                table: "BotMessages",
                type: "nvarchar(20)",
                nullable: true);

            migrationBuilder.Sql(
                "UPDATE BotMessages SET LanguageISOCode = 'en', DateUpdated = GETUTCDATE() " +
                "ALTER TABLE BotMessages ALTER COLUMN LanguageISOCode nvarchar(20) NOT NULL; ");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LanguageISOCode",
                table: "BotMessages");
        }
    }
}
