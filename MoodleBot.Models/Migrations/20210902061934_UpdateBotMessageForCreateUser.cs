using Microsoft.EntityFrameworkCore.Migrations;

namespace MoodleBot.Models.Migrations
{
    public partial class UpdateBotMessageForCreateUser : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                "UPDATE BotMessages SET Message = 'You are not registed in Kiira.\r\n\r\nPress 1 to create a new account', DateUpdated = GETUTCDATE() WHERE MessageName = 'USER_NOT_REGISTERD'");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                "UPDATE BotMessages SET Message = 'Opps!, You are not register with *Moodle*, you need register with *Moodle* using your Whatsapp number. Please refer following link for singup in *Moodle*. https://bigfoot.kassai.ao/', DateUpdated = GETUTCDATE() WHERE MessageName = 'USER_NOT_REGISTERD'");
        }
    }
}
