using Microsoft.EntityFrameworkCore.Migrations;

namespace MoodleBot.Models.Migrations
{
    public partial class UpdateBotMessagesAndEmojis : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                "UPDATE BotEmojis SET Emojis = N'00', DateUpdated = GETUTCDATE() WHERE EmojisName = 'PAGE_PREVIOUS'" +
                "UPDATE BotEmojis SET Emojis = N'99', DateUpdated = GETUTCDATE() WHERE EmojisName = 'PAGE_NEXT'" +
                "UPDATE BotEmojis SET Emojis = N'88', DateUpdated = GETUTCDATE() WHERE EmojisName = 'DOWNLOAD_CERTIFICATE'");

            migrationBuilder.Sql("INSERT INTO BotEmojis" +
                "(EmojisName, EmojisTypeId, Emojis, DateCreated, DateUpdated) VALUES" +
                "('RESTRICTED_ITEM', 3, N'🚫', GETUTCDATE(), GETUTCDATE())");

            migrationBuilder.Sql("INSERT INTO BotMessages" +
                "(MessageName, TypeId, Message, DateCreated, DateUpdated, LanguageISOCode) VALUES" +
                "('OPTIONAL', 2, N'Optional', GETUTCDATE(), GETUTCDATE(), 'en')," +
                "('OPTIONAL', 2, N'Opcional', GETUTCDATE(), GETUTCDATE(), 'pt')");

            migrationBuilder.Sql(
                "UPDATE UserCreationQuestion SET MessageType = 2, MessagePosition = 26, DateUpdated = GETUTCDATE() WHERE MessageName = 'PASSWORD'" +
                "UPDATE UserCreationQuestion SET MessageType = 2, MessagePosition = 27, DateUpdated = GETUTCDATE() WHERE MessageName = 'CONFIRM_PASSWORD'" +
                "UPDATE UserCreationQuestion SET MessagePosition = 1, DateUpdated = GETUTCDATE() WHERE MessageName = 'TERMS_CONDITIONS'" +
                "UPDATE UserCreationQuestion SET MessagePosition = 2, DateUpdated = GETUTCDATE() WHERE MessageName = 'FIRST_NAME'" +
                "UPDATE UserCreationQuestion SET MessagePosition = 3, DateUpdated = GETUTCDATE() WHERE MessageName = 'LAST_NAME'" +
                //"UPDATE UserCreationQuestion SET MessagePosition = 4, IsRequired = 1, DateUpdated = GETUTCDATE() WHERE MessageName = 'GENDER'" +
                //"UPDATE UserCreationQuestion SET MessagePosition = 5, IsRequired = 1, DateUpdated = GETUTCDATE() WHERE MessageName = 'HEALTH_WORKER_TYPE'" +
                "UPDATE UserCreationQuestion SET MessagePosition = 4, DateUpdated = GETUTCDATE() WHERE MessageName = 'EMAIL'" +
                //"UPDATE UserCreationQuestion SET MessagePosition = 8, DateUpdated = GETUTCDATE() WHERE MessageName = 'HEALTH_WORKER_NUMBER'" +
                "UPDATE UserCreationQuestion SET MessagePosition = 5, DateUpdated = GETUTCDATE() WHERE MessageName = 'USER_DETAIL_SUMMARY'");
            /*
            migrationBuilder.Sql(
                "UPDATE UserCreationQuestion SET Question = 'What is your gender?\r\n\r\n1) Male\r\n2) Female\r\n3) Other', DateUpdated = GETUTCDATE() WHERE MessageName = 'GENDER' AND LanguageISOCode = 'en'" +
                "UPDATE UserCreationQuestion SET Question = 'Qual é o seu gênero?\r\n\r\n1) Masculino\r\n2) Feminino\r\n3) Outro', DateUpdated = GETUTCDATE() WHERE MessageName = 'GENDER' AND LanguageISOCode = 'pt'");*/
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                "UPDATE BotEmojis SET Emojis = N'⬆', DateUpdated = GETUTCDATE() WHERE EmojisName = 'PAGE_PREVIOUS'" +
                "UPDATE BotEmojis SET Emojis = N'⬇️', DateUpdated = GETUTCDATE() WHERE EmojisName = 'PAGE_NEXT'" +
                "UPDATE BotEmojis SET Emojis = N'⏬', DateUpdated = GETUTCDATE() WHERE EmojisName = 'DOWNLOAD_CERTIFICATE'");

            migrationBuilder.Sql("DELETE FROM BotEmojis WHERE EmojisName = 'RESTRICTED_ITEM'");

            migrationBuilder.Sql("DELETE FROM BotMessages WHERE MessageName = 'OPTIONAL'");

            migrationBuilder.Sql(
                "UPDATE UserCreationQuestion SET MessageType = 1, MessagePosition = 3, DateUpdated = GETUTCDATE() WHERE MessageName = 'PASSWORD'" +
                "UPDATE UserCreationQuestion SET MessageType = 1, MessagePosition = 4, DateUpdated = GETUTCDATE() WHERE MessageName = 'CONFIRM_PASSWORD'" +
                "UPDATE UserCreationQuestion SET MessagePosition = 11, DateUpdated = GETUTCDATE() WHERE MessageName = 'TERMS_CONDITIONS'" +
                "UPDATE UserCreationQuestion SET MessagePosition = 1, DateUpdated = GETUTCDATE() WHERE MessageName = 'FIRST_NAME'" +
                "UPDATE UserCreationQuestion SET MessagePosition = 2, DateUpdated = GETUTCDATE() WHERE MessageName = 'LAST_NAME'" +
               //"UPDATE UserCreationQuestion SET MessagePosition = 6, IsRequired = 0, DateUpdated = GETUTCDATE() WHERE MessageName = 'GENDER'" +
                //"UPDATE UserCreationQuestion SET MessagePosition = 8, IsRequired = 0, DateUpdated = GETUTCDATE() WHERE MessageName = 'HEALTH_WORKER_TYPE'" +
                "UPDATE UserCreationQuestion SET MessagePosition = 3, DateUpdated = GETUTCDATE() WHERE MessageName = 'EMAIL'" +
                //"UPDATE UserCreationQuestion SET MessagePosition = 9, DateUpdated = GETUTCDATE() WHERE MessageName = 'HEALTH_WORKER_NUMBER'" +
                "UPDATE UserCreationQuestion SET MessagePosition = 4, DateUpdated = GETUTCDATE() WHERE MessageName = 'USER_DETAIL_SUMMARY'");
            /*
            migrationBuilder.Sql(
                "UPDATE UserCreationQuestion SET Question = 'What is your gender?\r\n\r\n1) Other\r\n2) Male\r\n3) Female', DateUpdated = GETUTCDATE() WHERE MessageName = 'GENDER' AND LanguageISOCode = 'en'" +
                "UPDATE UserCreationQuestion SET Question = 'Qual é o seu gênero?\r\n\r\n1) Outro\r\n2) Masculino\r\n3) Feminino', DateUpdated = GETUTCDATE() WHERE MessageName = 'GENDER' AND LanguageISOCode = 'pt'");*/
        }
    }
}
