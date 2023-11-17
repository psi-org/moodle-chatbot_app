using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace MoodleBot.Models.Migrations
{
    public partial class AddCreateUserMessageTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "UserCreationQuestion",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MessageName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Question = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MessageType = table.Column<int>(type: "int", nullable: false),
                    MessagePosition = table.Column<int>(type: "int", nullable: false),
                    IsRequired = table.Column<bool>(type: "bit", nullable: false),
                    ValidationName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ValidationMessage = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DateCreated = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    DateUpdated = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserCreationQuestion", x => x.Id);
                });

            migrationBuilder.Sql("INSERT INTO UserCreationQuestion (MessageName, Question, MessageType, MessagePosition, IsRequired, ValidationName, ValidationMessage, DateCreated, DateUpdated) VALUES" +
                "('FIRST_NAME', N'Please type your first name', 1, 1, 1, 'ValidateFirstName', N'⚠️ Please provide your First Name', GETUTCDATE(), GETUTCDATE())," +
                "('LAST_NAME', N'Please type your last name', 1, 2, 1, 'ValidateLastName', N'⚠️ Please provide your Last Name', GETUTCDATE(), GETUTCDATE())," +
                "('PASSWORD', N'*Choose your password:* Your password shall have at least 6 characters.', 1, 3, 1, 'ValidatePassword', N'⚠️ Invalid format. Your password shall have at least 6 characters', GETUTCDATE(), GETUTCDATE())," +
                "('CONFIRM_PASSWORD', N'Please confirm your password', 1, 4, 1, 'ValidateConfirmPassword', N'⚠️ Your password is not the same! Please type the same password.', GETUTCDATE(), GETUTCDATE())," +
                "('EMAIL', N'Please type your email', 1, 5, 0, 'ValidateEmail', N'⚠️ Invalid format. Please add your email.', GETUTCDATE(), GETUTCDATE())," +
                //"('GENDER', N'What is your gender?\r\n\r\n1) Male\r\n2) Female\r\n3) Other', 1, 6, 0, 'ValidateGenderOption', N'⚠️ Invalid input. Please select one of the options available.', GETUTCDATE(), GETUTCDATE())," +
                //"('DATE_OF_BIRTH', N'What is your date of birth? Format YYYY-MM-DD', 1, 7, 0, 'ValidateBirthDate', N'⚠️ Invalid format. Your DOB is not valid. You must be older than 18 years old to register on the platform. Please try again.', GETUTCDATE(), GETUTCDATE())," +
                //"('HEALTH_WORKER_TYPE', N'Which type of health worker are you?\r\n\r\n1) Nurse\r\n2) Doctor\r\n3) Other', 1, 8, 0, 'ValidateHealthWorkerType', N'⚠️ Invalid input. Please select one of the options available.', GETUTCDATE(), GETUTCDATE())," +
                //"('HEALTH_WORKER_NUMBER', N'What is your health worker number? ', 1, 9, 0, 'ValidateHealthWorkerNumber', N'⚠️ Invalid format. Your health worker number shall between 2-20 characters', GETUTCDATE(), GETUTCDATE())," +
                "('PREVIOUS_QUESTION', N'Press {0} to go back to previous question', 2, 10, 0, '', '', GETUTCDATE(), GETUTCDATE())," +
                "('NEXT_QUESTION', N'Press {0} to jump to the next question', 2, 11, 0, '', '', GETUTCDATE(), GETUTCDATE())," +
                "('QUESTION_TITLE', N'*Question {0} of {1}:*\r\n\r\n', 3, 12, 0, '', '', GETUTCDATE(), GETUTCDATE())," +
                "('USER_DETAIL_SUMMARY', N'🗒️ *Registration Summary*\r\n\r\n*First name:* $#FirstName#$*Last name:* $#LastName#$\r\n*E-mail:* $#Email#$\r\n*Your password:* $#Password#$\r\n*Country:* $#Country#$\r\n', 3, 13, 0, '', '', GETUTCDATE(), GETUTCDATE())," +
                //"('USER_DETAIL_SUMMARY', N'🗒️ *Registration Summary*\r\n\r\n*First name:* $#FirstName#$*Last name:* $#LastName#$\r\n*E-mail:* $#Email#$\r\n*Your password:* $#Password#$\r\n*Gender:* $#Gender#$\r\n*Date of birth:* $#DOB#$\r\n*Country:* $#Country#$\r\n*Type of health worker:* $#HealthWorkerType#$\r\n*Health worker number:* $#HealthWorkerNumber#$\r\n', 3, 13, 0, '', '', GETUTCDATE(), GETUTCDATE())," +
                "('SUCCESS_ACCOUNT_CREATE', N'✅ Congratulations! you have successfully registered.', 3, 14, 0, '', '', GETUTCDATE(), GETUTCDATE())," +
                "('TERMS_CONDITIONS', N'terms and conditions', 3, 15, 0, '', '', GETUTCDATE(), GETUTCDATE())");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserCreationQuestion");
        }
    }
}
