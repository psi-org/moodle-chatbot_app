using Microsoft.EntityFrameworkCore.Migrations;

namespace MoodleBot.Models.Migrations
{
    public partial class UpdateBotMessagesForFeedbackActivity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                "INSERT INTO BotMessages(MessageName, TypeId, Message, DateCreated, DateUpdated) " +
                    "VALUES ('FEEDBACK_ACTIVITY_ALREADY_COMPLETED', 9, 'You have already provided feedback. To conitinue please go with following actions.', GETUTCDATE(), GETUTCDATE())" +
                "UPDATE BotMessages SET Message = '------\r\n*Actions:*\r\n\r\n{0}\r\n{1}\r\n', DateUpdated = GETUTCDATE() WHERE MessageName = 'SUMMARY_ACTION'" +
                "UPDATE BotMessages SET Message = '{0}) Go back to the course menu', DateUpdated = GETUTCDATE() WHERE MessageName = 'ACTIVITY_ACTION_COURSE_MENU'" +
                "UPDATE BotMessages SET Message = '{0}) Show course summary', DateUpdated = GETUTCDATE() WHERE MessageName = 'ACTIVITY_ACTION_SHOW_COURSE_SUMMARY'" +
                "UPDATE BotMessages SET Message = '{0}) Repeat this activity', DateUpdated = GETUTCDATE() WHERE MessageName = 'ACTIVITY_ACTION_REPEAT_ACTIVITTY'" +
                "UPDATE BotMessages SET Message = '{0}) Go to the next activity', DateUpdated = GETUTCDATE() WHERE MessageName = 'ACTIVITY_ACTION_NEXT_ACTIVITY'");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                "DELETE FROM BotMessages WHERE MessageName = 'FEEDBACK_ACTIVITY_ALREADY_COMPLETED'" +
                "UPDATE BotMessages SET Message = '------\r\n*Actions:*\r\n\r\n{0}\r\n{1}\r\n{2}\r\n', DateUpdated = GETUTCDATE() WHERE MessageName = 'SUMMARY_ACTION'" +
                "UPDATE BotMessages SET Message = '0) Go back to the course menu', DateUpdated = GETUTCDATE() WHERE MessageName = 'ACTIVITY_ACTION_COURSE_MENU'" +
                "UPDATE BotMessages SET Message = '1) Show course summary', DateUpdated = GETUTCDATE() WHERE MessageName = 'ACTIVITY_ACTION_SHOW_COURSE_SUMMARY'" +
                "UPDATE BotMessages SET Message = '2) Repeat this activity', DateUpdated = GETUTCDATE() WHERE MessageName = 'ACTIVITY_ACTION_REPEAT_ACTIVITTY'" +
                "UPDATE BotMessages SET Message = '3) Go to the next activity', DateUpdated = GETUTCDATE() WHERE MessageName = 'ACTIVITY_ACTION_NEXT_ACTIVITY'");
        }
    }
}
