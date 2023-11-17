using Microsoft.EntityFrameworkCore.Migrations;

namespace MoodleBot.Models.Migrations
{
    public partial class UpdateCourseActivityListInfoMessage : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("UPDATE BotMessages SET " +
                "Message = N'Below you may find a list of $#COURSE_COUNT#$ courses available for you. Please type the course number you want to see:\r\n\r\n', DateUpdated = GETUTCDATE()" +
                "WHERE LanguageISOCode = 'en' AND MessageName = 'COURSE_LIST_INFO'");

            migrationBuilder.Sql("UPDATE BotMessages SET " +
                "Message = N'Abaixo você encontrará uma lista de $#COURSE_COUNT#$ cursos disponíveis para você. Por favor, digite o número do curso que você deseja ver:\r\n\r\n', DateUpdated = GETUTCDATE()" +
                "WHERE LanguageISOCode = 'pt' AND MessageName = 'COURSE_LIST_INFO'");

            migrationBuilder.Sql("UPDATE BotMessages SET " +
                "Message = N'*{0} - {1}*\r\n\r\n*Activities:*\r\nSelect one of the numbers below to start the activity from the beginning. Total $#ACTIVITY_COUNT#$ activities available.\r\n\r\n', DateUpdated = GETUTCDATE()" +
                "WHERE LanguageISOCode = 'en' AND MessageName = 'ACTIVITY_LIST_INFO'");

            migrationBuilder.Sql("UPDATE BotMessages SET " +
                "Message = N'*{0} - {1}*\r\n\r\n*Atividades:*\r\nSelecione um dos números abaixo para iniciar a atividade desde o início. Total de $#ACTIVITY_COUNT#$ atividades disponíveis.\r\n\r\n', DateUpdated = GETUTCDATE()" +
                "WHERE LanguageISOCode = 'pt' AND MessageName = 'ACTIVITY_LIST_INFO'");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("UPDATE BotMessages SET " +
                "Message = 'Below you may find a list of courses available for you. Please type the course number you want to see:\r\n\r\n', DateUpdated = GETUTCDATE()" +
                "WHERE LanguageISOCode = 'en' AND MessageName = 'COURSE_LIST_INFO'");

            migrationBuilder.Sql("UPDATE BotMessages SET " +
               "Message = 'Abaixo você encontrará uma lista de cursos disponíveis para você. Por favor, digite o número do curso que você deseja ver:\r\n\r\n', DateUpdated = GETUTCDATE()" +
               "WHERE LanguageISOCode = 'pt' AND MessageName = 'COURSE_LIST_INFO'");

            migrationBuilder.Sql("UPDATE BotMessages SET " +
                "Message = N'*{0} - {1}*\r\n\r\n*Activities:*\r\nSelect one of the numbers below to start the activity from the beginning.\r\n\r\n', DateUpdated = GETUTCDATE()" +
                "WHERE LanguageISOCode = 'en' AND MessageName = 'ACTIVITY_LIST_INFO'");

            migrationBuilder.Sql("UPDATE BotMessages SET " +
                "Message = N'*{0} - {1}*\r\n\r\n*Atividades:*\r\nSelecione um dos números abaixo para iniciar a atividade desde o início.\r\n\r\n', DateUpdated = GETUTCDATE()" +
                "WHERE LanguageISOCode = 'pt' AND MessageName = 'ACTIVITY_LIST_INFO'");
        }
    }
}
