using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace MoodleBot.Models.Migrations
{
    public partial class AddBotMessagesTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BotMessages",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MessageName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TypeId = table.Column<int>(type: "int", nullable: false),
                    Message = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DateCreated = table.Column<DateTimeOffset>(type: "DateTimeOffset", nullable: false),
                    DateUpdated = table.Column<DateTimeOffset>(type: "DateTimeOffset", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BotMessages", x => x.Id);
                });

            migrationBuilder.Sql("INSERT INTO [dbo].[BotMessages] " +
                "(MessageName, TypeId, Message, DateCreated, DateUpdated) VALUES " +
                "('WELCOME_MESSAGE', 1, 'Welcome to Kiira e-Learning Platform', GETUTCDATE(), GETUTCDATE()), " +
                "('WELCOME_IMAGE', 1, 'https://stblobbftdev001.blob.core.windows.net/images/Kiira%20Logo%20white.png', GETUTCDATE(), GETUTCDATE())," +
                "('USER_WELCOME', 2, 'Hello, {0} {1}. Welcome back!\r\n\r\n', GETUTCDATE(), GETUTCDATE())," +
                "('USER_NOT_REGISTERD', 2, 'Opps!, You are not register with *Moodle*, you need register with *Moodle* using your Whatsapp number. Please refer following link for singup in *Moodle*. https://bigfoot.kassai.ao/', GETUTCDATE(), GETUTCDATE())," +
                "('USER_GREETINGS', 2, 'Thank you, {0} {1} for choosing moodle courses!', GETUTCDATE(), GETUTCDATE())," +
                "('USER_GREETINGS_ON_ACTIVITY_COMPLETE', 2, 'Congratulations! You have finished this activity.', GETUTCDATE(), GETUTCDATE())," +
                "('COURSE_LIST_INFO', 3, 'Below you may find a list of courses available for you. Please type the course number you want to see:\r\n\r\n', GETUTCDATE(), GETUTCDATE())," +
                "('COURSE_LIST', 3, '*{0} - {1}*\r\n      *Status:* {2}, {3} %\r\n      *Grade:* {4}\r\n\r\n', GETUTCDATE(), GETUTCDATE())," +
                "('COURSE_NOT_FOUND', 3, 'Course is not found for you. Please check after some time.', GETUTCDATE(), GETUTCDATE())," +
                "('COURSE_SUMMARY', 3, '*Course Summary:*\r\n\r\n*Status:* {0}, {1} %\r\n*Grade:* {2}\r\n*Started on:* {3}\r\n*Completed on:* {4}\r\n', GETUTCDATE(), GETUTCDATE())," +
                "('COURSE_ACTION_CURRENT_COURSE', 3, '0) Continue with currect course', GETUTCDATE(), GETUTCDATE())," +
                "('COURSE_ACTION_SHOW_MENU', 3, '1) Go back to the course menu', GETUTCDATE(), GETUTCDATE())," +
                "('ACTIVITY_LIST_INFO', 4, '*{0} - {1}*\r\n\r\n*Activities:*\r\nSelect one of the numbers below to start the activity from the beginning.\r\n\r\n', GETUTCDATE(), GETUTCDATE())," +
                "('ACTIVITY_LIST', 4, '*{0} - {1}*\r\n      *Status:* {2}\r\n      *Grade:* {3}\r\n      *Attempt:* {4}, {5}\r\n\r\n', GETUTCDATE(), GETUTCDATE())," +
                "('ACTIVITY_NOT_FOUND', 4, 'Activity is not found for selected course. Plesae choose other course.', GETUTCDATE(), GETUTCDATE())," +
                "('ACTIVITY_GO_BACK_TO_COURSE', 4, 'Press 0 to go back to the course list page, to see other courses', GETUTCDATE(), GETUTCDATE())," +
                "('ACTIVITY_SELECTION_CONFIRM_TITLE', 4, '*{0} - {1}*\r\n\r\n{2}\r\n\r\n', GETUTCDATE(), GETUTCDATE())," +
                "('ACTIVITY_SELECTION_CONFIRM', 4, 'Press 1 to start the {0}\r\n', GETUTCDATE(), GETUTCDATE())," +
                "('SUMMARY_ACTION', 4, '------\r\n*Actions:*\r\n\r\n{0}\r\n{1}\r\n{2}\r\n', GETUTCDATE(), GETUTCDATE())," +
                "('ACTIVITY_SUMMARY', 4, '*Activity Summary:*\r\n\r\n*Started on:* {0}\r\n*Completed on:* {1}\r\n*Time taken:* {2}\r\n', GETUTCDATE(), GETUTCDATE())," +
                "('ACTIVITY_ACTION_COURSE_MENU', 4, '0) Go back to the course menu', GETUTCDATE(), GETUTCDATE())," +
                "('ACTIVITY_ACTION_SHOW_COURSE_SUMMARY', 4, '1) Show course summary', GETUTCDATE(), GETUTCDATE())," +
                "('ACTIVITY_ACTION_REPEAT_ACTIVITTY', 4, '2) Repeat this activity', GETUTCDATE(), GETUTCDATE())," +
                "('ACTIVITY_ACTION_NEXT_ACTIVITY', 4, '3) Go to the next activity', GETUTCDATE(), GETUTCDATE())," +
                "('GO_BACK_TO_ACTIVITY', 4, 'Press 0 to go back to the previous page', GETUTCDATE(), GETUTCDATE())," +
                "('LESSON_ACTION_CONTINUE', 5, 'Press 1 to continue\r\n', GETUTCDATE(), GETUTCDATE())," +
                "('LESSON_ACTION_GO_BACK_TO_ACTIVITY', 5, 'Press 0 to go back to the Activities page\r\n', GETUTCDATE(), GETUTCDATE())," +
                "('QUESTIION_ATTEMPT_COUNT', 6, 'This is your attempt #{0}.', GETUTCDATE(), GETUTCDATE())," +
                "('QUESTIION_TEMPLATE', 6, '*{0}*: {1}\r\n\r\n', GETUTCDATE(), GETUTCDATE())," +
                "('QUESTIION_OPTION_TEMPLATE', 6, '{0}) {1}\r\n', GETUTCDATE(), GETUTCDATE())," +
                "('QUIZ_SUMMARY', 6, '*Marks:* {0} out of {1}\r\n*Grade:* {2} %\r\n*Attempt:* {3}, {4}\r\n\r\n', GETUTCDATE(), GETUTCDATE())," +
                "('INVALID_INPUT', 7, 'Invalid input. Please select one of the options available, or press *#* to go back to the previous page.', GETUTCDATE(), GETUTCDATE())");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BotMessages");
        }
    }
}
