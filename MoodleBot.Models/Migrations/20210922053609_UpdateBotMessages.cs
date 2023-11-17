using Microsoft.EntityFrameworkCore.Migrations;

namespace MoodleBot.Models.Migrations
{
    public partial class UpdateBotMessages : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("INSERT INTO[dbo].[BotMessages]" +
                "(MessageName, TypeId, Message, LanguageISOCode, DateCreated, DateUpdated) VALUES" +
                "('DOWNLOAD_CERTIFICATE', 3, N'Press {0} to download your certificate\r\n', 'en', GETUTCDATE(), GETUTCDATE())," +
                "('DOWNLOAD_CERTIFICATE', 3, N'Pressione {0} para baixar seu certificado\r\n', 'pt', GETUTCDATE(), GETUTCDATE())," +
                "('CERTIFICATE_WAITING_MESSAGE', 3, N'Please wait! it will take few seconds to provide your certificate.', 'en', GETUTCDATE(), GETUTCDATE())," +
                "('CERTIFICATE_WAITING_MESSAGE', 3, N'Por favor, espere! levará alguns segundos para fornecer seu certificado.', 'pt', GETUTCDATE(), GETUTCDATE())," +
                "('CERTIFICATE_NOT_FOUND', 3, N'Opps! Certificate is not availble for this course, We are waiting on completion of course.', 'en', GETUTCDATE(), GETUTCDATE())," +
                "('CERTIFICATE_NOT_FOUND', 3, N'Ops! O certificado não está disponível para este curso, estamos aguardando a conclusão do curso.', 'pt', GETUTCDATE(), GETUTCDATE())");

            migrationBuilder.Sql("INSERT INTO[dbo].[BotEmojis]" +
                "([EmojisName], [StatusId], [EmojisTypeId], [Emojis], [DateCreated], [DateUpdated]) VALUES" +
                "(N'DOWNLOAD_CERTIFICATE', NULL, 3, N'⏬', GETUTCDATE(), GETUTCDATE())");

            migrationBuilder.Sql("UPDATE BotMessages SET Message = N'Pressione {0} para ver mais itens\r\n', DateUpdated = GETUTCDATE() WHERE MessageName = 'PAGE_NEXT' AND LanguageISOCode = 'pt'" +
                "UPDATE BotMessages SET Message = N'Pressione {0} para ver os itens anteriores\r\n', DateUpdated = GETUTCDATE() WHERE MessageName = 'PAGE_PREVIOUS' AND LanguageISOCode = 'pt'" +
                "UPDATE BotMessages SET Message = N'Pressione 0 para voltar à página da lista de cursos, para ver outros cursos\r\n', DateUpdated = GETUTCDATE() WHERE MessageName = 'ACTIVITY_GO_BACK_TO_COURSE' AND LanguageISOCode = 'pt'");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
