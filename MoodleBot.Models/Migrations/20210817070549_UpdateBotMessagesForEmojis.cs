using Microsoft.EntityFrameworkCore.Migrations;

namespace MoodleBot.Models.Migrations
{
    public partial class UpdateBotMessagesForEmojis : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("INSERT INTO BotMessages " +
                "(MessageName, TypeId, Message, DateCreated, DateUpdated) VALUES " +
                "('PAGE_NEXT', 8, N'Press {0} to see more items\r\n', GETUTCDATE(), GETUTCDATE())," +
                "('PAGE_PREVIOUS', 8, N'Press {0} to see previous items\r\n', GETUTCDATE(), GETUTCDATE())");

            migrationBuilder.Sql(
                "UPDATE BotMessages SET Message = N'👏 Congratulations! You have finished this activity.', DateUpdated = GETUTCDATE() WHERE MessageName = 'USER_GREETINGS_ON_ACTIVITY_COMPLETE'" +
                "UPDATE BotMessages SET Message = N'⚠️ Invalid input. Please select one of the options available, or press *#* to go back to the previous page.', DateUpdated = GETUTCDATE() WHERE MessageName = 'INVALID_INPUT' " +
                "UPDATE BotMessages SET Message = N'Press 0 to go back to the course list page, to see other courses\r\n', DateUpdated = GETUTCDATE() WHERE MessageName = 'ACTIVITY_GO_BACK_TO_COURSE'");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            //To Revert back changes to for pag related messages
            migrationBuilder.Sql("DELETE FROM BotMessages WHERE TypeId = 8");


            //To Revert back changes to for emojis related messages
            migrationBuilder.Sql(
                "UPDATE BotMessages SET Message = N'Congratulations! You have finished this activity.' WHERE MessageName = 'USER_GREETINGS_ON_ACTIVITY_COMPLETE'" +
                "UPDATE BotMessages SET Message = N'Invalid input. Please select one of the options available, or press *#* to go back to the previous page.' WHERE MessageName = 'INVALID_INPUT'");
        }
    }
}
