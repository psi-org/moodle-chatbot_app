using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace MoodleBot.Models.Migrations
{
    public partial class CreateCountryLanguageTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CountryLanguage",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ISO = table.Column<string>(type: "nvarchar(20)", nullable: true),
                    TLD = table.Column<string>(type: "nvarchar(20)", nullable: true),
                    CallingCode = table.Column<string>(type: "nvarchar(100)", nullable: true),
                    LanguageName = table.Column<string>(type: "nvarchar(100)", nullable: true),
                    LanguageISO = table.Column<string>(type: "nvarchar(20)", nullable: true),
                    DateCreated = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    DateUpdated = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CountryLanguage", x => x.Id);
                });

            migrationBuilder.Sql("INSERT INTO [dbo].[CountryLanguage] " +
                    "(ISO, TLD, CallingCode, LanguageName, LanguageISO, DateCreated, DateUpdated) VALUES" +
                    "(N'AO', N'.ao', N'244', N'Português', N'pt', GETUTCDATE(), GETUTCDATE())," +
                    "(N'BF', N'.bf', N'226', N'Français', N'fr', GETUTCDATE(), GETUTCDATE())," +
                    "(N'BI', N'.bi', N'257', N'English', N'en', GETUTCDATE(), GETUTCDATE())," +
                    "(N'BJ', N'.bj', N'229', N'Français', N'fr', GETUTCDATE(), GETUTCDATE())," +
                    "(N'BW', N'.bw', N'267', N'English', N'en', GETUTCDATE(), GETUTCDATE())," +
                    "(N'CD', N'.cd', N'243', N'Français', N'fr', GETUTCDATE(), GETUTCDATE())," +
                    "(N'CG', N'.cg', N'242', N'Français', N'fr', GETUTCDATE(), GETUTCDATE())," +
                    "(N'CI', N'.ci', N'225', N'Français', N'fr', GETUTCDATE(), GETUTCDATE())," +
                    "(N'CM', N'.cm', N'237', N'English', N'en', GETUTCDATE(), GETUTCDATE())," +
                    "(N'CV', N'.cv', N'238', N'Português', N'pt', GETUTCDATE(), GETUTCDATE())," +
                    "(N'GA', N'.ga', N'241', N'Français', N'fr', GETUTCDATE(), GETUTCDATE())," +
                    "(N'GH', N'.gh', N'233', N'English', N'en', GETUTCDATE(), GETUTCDATE())," +
                    "(N'GM', N'.gm', N'220', N'English', N'en', GETUTCDATE(), GETUTCDATE())," +
                    "(N'GN', N'.gn', N'224', N'Français', N'fr', GETUTCDATE(), GETUTCDATE())," +
                    "(N'GQ', N'.gq', N'240', N'Português', N'pt', GETUTCDATE(), GETUTCDATE())," +
                    "(N'GW', N'.gw', N'245', N'Português', N'pt', GETUTCDATE(), GETUTCDATE())," +
                    "(N'KE', N'.ke', N'254', N'English', N'en', GETUTCDATE(), GETUTCDATE())," +
                    "(N'LR', N'.lr', N'231', N'English', N'en', GETUTCDATE(), GETUTCDATE())," +
                    "(N'LS', N'.ls', N'266', N'English', N'en', GETUTCDATE(), GETUTCDATE())," +
                    "(N'ML', N'.ml', N'223', N'Français', N'fr', GETUTCDATE(), GETUTCDATE())," +
                    "(N'MW', N'.mw', N'265', N'English', N'en', GETUTCDATE(), GETUTCDATE())," +
                    "(N'MZ', N'.mz', N'258', N'Português', N'pt', GETUTCDATE(), GETUTCDATE())," +
                    "(N'NA', N'.na', N'264', N'English', N'en', GETUTCDATE(), GETUTCDATE())," +
                    "(N'NE', N'.ne', N'227', N'Français', N'fr', GETUTCDATE(), GETUTCDATE())," +
                    "(N'NG', N'.ng', N'234', N'English', N'en', GETUTCDATE(), GETUTCDATE())," +
                    "(N'SC', N'.sc', N'248', N'English', N'en', GETUTCDATE(), GETUTCDATE())," +
                    "(N'SD', N'.sd', N'249', N'English', N'en', GETUTCDATE(), GETUTCDATE())," +
                    "(N'SL', N'.sl', N'232', N'English', N'en', GETUTCDATE(), GETUTCDATE())," +
                    "(N'SN', N'.sn', N'221', N'Français', N'fr', GETUTCDATE(), GETUTCDATE()), " +
                    "(N'ST', N'.st', N'239', N'Português', N'pt', GETUTCDATE(), GETUTCDATE())," +
                    "(N'TG', N'.tg', N'228', N'Français', N'fr', GETUTCDATE(), GETUTCDATE())," +
                    "(N'TZ', N'.tz', N'255', N'English', N'en', GETUTCDATE(), GETUTCDATE())," +
                    "(N'ZA', N'.za', N'27', N'English', N'en', GETUTCDATE(), GETUTCDATE()),	" +
                    "(N'ZM', N'.zm', N'260', N'English', N'en', GETUTCDATE(), GETUTCDATE())," +
                    "(N'ZW', N'.zw', N'263', N'English', N'en', GETUTCDATE(), GETUTCDATE())");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CountryLanguage");
        }
    }
}
