using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace MoodleBot.Models.Migrations
{
    public partial class AddBotEmojisTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "DateUpdated",
                table: "Logs",
                type: "datetimeoffset",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime");

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "DateCreated",
                table: "Logs",
                type: "datetimeoffset",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime");

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "DateUpdated",
                table: "BotMessages",
                type: "datetimeoffset",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime");

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "DateCreated",
                table: "BotMessages",
                type: "datetimeoffset",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime");

            migrationBuilder.CreateTable(
                name: "BotEmojis",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EmojisName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    StatusId = table.Column<int>(type: "int", nullable: true),
                    EmojisTypeId = table.Column<int>(type: "int", nullable: false),
                    Emojis = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DateCreated = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    DateUpdated = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BotEmojis", x => x.Id);
                });

            migrationBuilder.Sql("INSERT INTO [dbo].[BotEmojis] " +
                            "([EmojisName], [StatusId], [EmojisTypeId], [Emojis], [DateCreated], [DateUpdated]) VALUES " +
                            "(N'COURSE_NOT_ENROLLED', 1, 1, N'🔲', GETUTCDATE(), GETUTCDATE())," +
                            "(N'COURSE_ENROLLED', 2, 1, N'☑️', GETUTCDATE(), GETUTCDATE())," +
                            "(N'COURSE_IN_PROGRESS', 3, 1, N'💬', GETUTCDATE(), GETUTCDATE())," +
                            "(N'COURSE_COMPLETED', 4, 1, N'✅', GETUTCDATE(), GETUTCDATE())," +
                            "(N'COURSE_COMPLETED_PASS', 5, 1, N'✅', GETUTCDATE(), GETUTCDATE())," +
                            "(N'COURSE_COMPLETED_FAIL', 6, 1, N'❌', GETUTCDATE(), GETUTCDATE())," +
                            "(N'ACTIVITY_NOT_STARTED', -1, 2, N'🔲', GETUTCDATE(), GETUTCDATE())," +
                            "(N'ACTIVITY_IN_PROGRESS', 0, 2, N'💬', GETUTCDATE(), GETUTCDATE())," +
                            "(N'ACTIVITY_COMPLETED', 1, 2, N'✅', GETUTCDATE(), GETUTCDATE())," +
                            "(N'ACTIVITY_COMPLETED_PASS', 2, 2, N'✅', GETUTCDATE(), GETUTCDATE())," +
                            "(N'ACTIVITY_COMPLETED_FAIL', 3, 2, N'❌', GETUTCDATE(), GETUTCDATE())," +
                            "(N'STATUS_UNKNOWN', NULL, 3, N'❓', GETUTCDATE(), GETUTCDATE())," +
                            "(N'GRADE_GREEN', NULL, 3, N'🟢', GETUTCDATE(), GETUTCDATE())," +
                            "(N'GRADE_RED', NULL, 3, N'🔴', GETUTCDATE(), GETUTCDATE())," +
                            "(N'ATTEMPT', NULL, 3, N'#️⃣', GETUTCDATE(), GETUTCDATE())," +
                            "(N'ANSWER_CORRECT', NULL, 3, N'✅', GETUTCDATE(), GETUTCDATE())," +
                            "(N'ANSWER_INCORRECT', NULL, 3, N'❌', GETUTCDATE(), GETUTCDATE())," +
                            "(N'SUMMARY', NULL, 3, N'🗒️', GETUTCDATE(), GETUTCDATE())," +
                            "(N'ACTIVITY_FINISHED', NULL, 3, N'👏', GETUTCDATE(), GETUTCDATE())," +
                            "(N'PAGE_NEXT', NULL, 4, N'⏩', GETUTCDATE(), GETUTCDATE())," +
                            "(N'PAGE_PREVIOUS', NULL, 4, N'⏪', GETUTCDATE(), GETUTCDATE())");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BotEmojis");

            migrationBuilder.AlterColumn<DateTime>(
                name: "DateUpdated",
                table: "Logs",
                type: "datetime",
                nullable: false,
                oldClrType: typeof(DateTimeOffset),
                oldType: "datetimeoffset");

            migrationBuilder.AlterColumn<DateTime>(
                name: "DateCreated",
                table: "Logs",
                type: "datetime",
                nullable: false,
                oldClrType: typeof(DateTimeOffset),
                oldType: "datetimeoffset");

            migrationBuilder.AlterColumn<DateTime>(
                name: "DateUpdated",
                table: "BotMessages",
                type: "datetime",
                nullable: false,
                oldClrType: typeof(DateTimeOffset),
                oldType: "datetimeoffset");

            migrationBuilder.AlterColumn<DateTime>(
                name: "DateCreated",
                table: "BotMessages",
                type: "datetime",
                nullable: false,
                oldClrType: typeof(DateTimeOffset),
                oldType: "datetimeoffset");
        }
    }
}
