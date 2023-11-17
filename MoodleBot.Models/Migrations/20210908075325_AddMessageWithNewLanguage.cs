using Microsoft.EntityFrameworkCore.Migrations;

namespace MoodleBot.Models.Migrations
{
    public partial class AddMessageWithNewLanguage : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("INSERT INTO[dbo].[BotMessages] " +
                "(MessageName, TypeId, Message, LanguageISOCode, DateCreated, DateUpdated) VALUES" +
                "('WELCOME_MESSAGE', 1, N'Bem-vindo à plataforma de e-Learning Kiira', 'pt', GETUTCDATE(), GETUTCDATE())," +
                "('WELCOME_IMAGE', 1, N'https://stblobbftdev001.blob.core.windows.net/images/Kiira%20Logo%20white.png', 'pt', GETUTCDATE(), GETUTCDATE())," +
                "('USER_WELCOME', 2, N'Olá, {0} {1}. Bem vindo de volta!\r\n\r\n', 'pt', GETUTCDATE(), GETUTCDATE())," +
                "('USER_NOT_REGISTERD', 2, N'Você não está registrado em Kiira.\r\nPressione 1 para criar uma nova conta', 'pt', GETUTCDATE(), GETUTCDATE())," +
                "('USER_GREETINGS', 2, N'Obrigado, {0} {1} por escolher os cursos moodle!', 'pt', GETUTCDATE(), GETUTCDATE())," +
                "('USER_GREETINGS_ON_ACTIVITY_COMPLETE', 2, N'Parabéns! Você terminou esta atividade.', 'pt', GETUTCDATE(), GETUTCDATE())," +
                "('COURSE_LIST_INFO', 3, N'Abaixo você encontrará uma lista de cursos disponíveis para você. Por favor, digite o número do curso que você deseja ver:\r\n\r\n', 'pt', GETUTCDATE(), GETUTCDATE())," +
                "('COURSE_LIST', 3, N'*{0} - {1}*\r\n      *Status:* {2}, {3} %\r\n      *Grau:* {4}\r\n\r\n', 'pt', GETUTCDATE(), GETUTCDATE())," +
                "('COURSE_NOT_FOUND', 3, N'O curso não foi encontrado para você. Por favor, verifique depois de algum tempo.', 'pt', GETUTCDATE(), GETUTCDATE())," +
                "('COURSE_SUMMARY', 3, N'*Resumo do Curso:*\r\n\r\n*Status:* {0}, {1} %\r\n*Grau:* {2}\r\n*Começou em:* {3}\r\n*Completo em:* {4}\r\n', 'pt', GETUTCDATE(), GETUTCDATE())," +
                "('COURSE_ACTION_CURRENT_COURSE', 3, N'0) Continue com o curso atual', 'pt', GETUTCDATE(), GETUTCDATE())," +
                "('COURSE_ACTION_SHOW_MENU', 3, N'1) Volte para o menu do curso', 'pt', GETUTCDATE(), GETUTCDATE())," +
                "('ACTIVITY_LIST_INFO', 4, N'*{0} - {1}*\r\n\r\n*Atividades:*\r\nSelecione um dos números abaixo para iniciar a atividade desde o início.\r\n\r\n', 'pt', GETUTCDATE(), GETUTCDATE())," +
                "('ACTIVITY_LIST', 4, N'*{0} - {1}*\r\n      *Status:* {2}\r\n      *Grau:* {3}\r\n      *Tentar:* {4}, {5}\r\n\r\n', 'pt', GETUTCDATE(), GETUTCDATE())," +
                "('ACTIVITY_NOT_FOUND', 4, N'A atividade não foi encontrada para o curso selecionado. Plesae escolhe outro curso.', 'pt', GETUTCDATE(), GETUTCDATE())," +
                "('ACTIVITY_GO_BACK_TO_COURSE', 4, N'Pressione 0 para voltar à página da lista de cursos, para ver outros cursos\r\n', 'pt', GETUTCDATE(), GETUTCDATE())," +
                "('ACTIVITY_SELECTION_CONFIRM_TITLE', 4, N'*{0} - {1}*\r\n\r\n{2}\r\n\r\n', 'pt', GETUTCDATE(), GETUTCDATE())," +
                "('ACTIVITY_SELECTION_CONFIRM', 4, N'Pressione 1 para iniciar o {0}\r\n', 'pt', GETUTCDATE(), GETUTCDATE())," +
                "('SUMMARY_ACTION', 4, N'------\r\n*Ações:*\r\n\r\n{0}\r\n{1}\r\n', 'pt', GETUTCDATE(), GETUTCDATE())," +
                "('ACTIVITY_SUMMARY', 4, N'*Resumo de Atividades:*\r\n\r\n*Começou em:* {0}\r\n*Completo em:* {1}\r\n*Tempo gasto:* {2}\r\n', 'pt', GETUTCDATE(), GETUTCDATE())," +
                "('ACTIVITY_ACTION_COURSE_MENU', 4, N'{0}) Volte para o menu do curso', 'pt', GETUTCDATE(), GETUTCDATE())," +
                "('ACTIVITY_ACTION_SHOW_COURSE_SUMMARY', 4, N'{0}) Mostrar resumo do curso', 'pt', GETUTCDATE(), GETUTCDATE())," +
                "('ACTIVITY_ACTION_REPEAT_ACTIVITTY', 4, N'{0}) Repita esta atividade', 'pt', GETUTCDATE(), GETUTCDATE())," +
                "('ACTIVITY_ACTION_NEXT_ACTIVITY', 4, N'{0}) Vá para a próxima atividade', 'pt', GETUTCDATE(), GETUTCDATE())," +
                "('GO_BACK_TO_ACTIVITY', 4, N'Pressione 0 para voltar à página anterior', 'pt', GETUTCDATE(), GETUTCDATE())," +
                "('LESSON_ACTION_CONTINUE', 5, N'Pressione 1 para continuar\r\n', 'pt', GETUTCDATE(), GETUTCDATE())," +
                "('LESSON_ACTION_GO_BACK_TO_ACTIVITY', 5, N'Pressione 0 para voltar à página de Atividades\r\n', 'pt', GETUTCDATE(), GETUTCDATE())," +
                "('QUESTIION_ATTEMPT_COUNT', 6, N'Esta é a sua tentativa #{0}.', 'pt', GETUTCDATE(), GETUTCDATE())," +
                "('QUESTIION_TEMPLATE', 6, N'*{0}*: {1}\r\n\r\n', 'pt', GETUTCDATE(), GETUTCDATE())," +
                "('QUESTIION_OPTION_TEMPLATE', 6, N'{0}) {1}\r\n', 'pt', GETUTCDATE(), GETUTCDATE())," +
                "('QUIZ_SUMMARY', 6, N'*Marcas:* {0} fora de {1}\r\n*Grau:* {2} %\r\n*Tentar:* {3}, {4}\r\n\r\n', 'pt', GETUTCDATE(), GETUTCDATE())," +
                "('INVALID_INPUT', 7, N'Entrada inválida. Selecione uma das opções disponíveis ou pressione *#* para voltar à página anterior.', 'pt', GETUTCDATE(), GETUTCDATE())," +
                "('PAGE_NEXT', 8, N'Pressione {0} para ver mais itens\r\n', 'pt', GETUTCDATE(), GETUTCDATE())," +
                "('PAGE_PREVIOUS', 8, N'Pressione {0} para ver os itens anteriores\r\n', 'pt', GETUTCDATE(), GETUTCDATE())," +
                "('FEEDBACK_ACTIVITY_ALREADY_COMPLETED', 9, N'Você já forneceu feedback. Para continuar, siga as seguintes ações.', 'pt', GETUTCDATE(), GETUTCDATE())");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DELETE FROM BotMessages WHERE LanguageISOCode = 'pt'");
        }
    }
}
