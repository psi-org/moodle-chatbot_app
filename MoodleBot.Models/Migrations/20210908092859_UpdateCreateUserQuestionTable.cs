using Microsoft.EntityFrameworkCore.Migrations;

namespace MoodleBot.Models.Migrations
{
    public partial class UpdateCreateUserQuestionTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "LanguageISOCode",
                table: "UserCreationQuestion",
                type: "nvarchar(20)",
                nullable: true);

            migrationBuilder.Sql(
                "UPDATE UserCreationQuestion SET LanguageISOCode = 'en', DateUpdated = GETUTCDATE() " +
                "ALTER TABLE UserCreationQuestion ALTER COLUMN LanguageISOCode nvarchar(20) NOT NULL; ");

            migrationBuilder.Sql("TRUNCATE TABLE UserCreationQuestion");
            migrationBuilder.Sql("TRUNCATE TABLE BotDataEntity");

            migrationBuilder.Sql("UPDATE BotEmojis SET Emojis = N'⬇️', DateUpdated = GETUTCDATE() WHERE EmojisName = 'PAGE_NEXT'" +
                "UPDATE BotEmojis SET Emojis = N'⬆️', DateUpdated = GETUTCDATE() WHERE EmojisName = 'PAGE_PREVIOUS'");

            migrationBuilder.Sql("INSERT INTO UserCreationQuestion (MessageName, Question, MessageType, MessagePosition, IsRequired, ValidationName, ValidationMessage, LanguageISOCode, DateCreated, DateUpdated) VALUES" +
                "('FIRST_NAME', N'Please type your first name', 1, 1, 1, 'ValidateFirstName', N'⚠️ Please provide your First Name', 'en', GETUTCDATE(), GETUTCDATE())," +
                "('LAST_NAME', N'Please type your last name', 1, 2, 1, 'ValidateLastName', N'⚠️ Please provide your Last Name', 'en', GETUTCDATE(), GETUTCDATE())," +
                "('PASSWORD', N'*Choose your password:* Your password shall have at least 6 characters.', 1, 3, 1, 'ValidatePassword', N'⚠️ Invalid format. Your password shall have at between 6 to 12 characters', 'en', GETUTCDATE(), GETUTCDATE())," +
                "('CONFIRM_PASSWORD', N'Please confirm your password', 1, 4, 1, 'ValidateConfirmPassword', N'⚠️ Your password is not the same! Please type the same password.', 'en', GETUTCDATE(), GETUTCDATE())," +
                "('EMAIL', N'Please type your email', 1, 5, 0, 'ValidateEmail', N'⚠️ Invalid format. Please add your email.', 'en', GETUTCDATE(), GETUTCDATE())," +
                //"('GENDER', N'What is your gender?\r\n\r\n1) Other\r\n2) Male\r\n3) Female', 1, 6, 0, 'ValidateGenderOption', N'⚠️ Invalid input. Please select one of the options available.', 'en', GETUTCDATE(), GETUTCDATE())," +
                //"('DATE_OF_BIRTH', N'What is your date of birth? *Format YYYY-MM-DD*', 1, 7, 0, 'ValidateBirthDate', N'⚠️ Invalid format. Your DOB is not valid. You must be older than 18 years old to register on the platform. Please try again.', 'en', GETUTCDATE(), GETUTCDATE())," +
                //"('HEALTH_WORKER_TYPE', N'Which type of health worker are you?\r\n\r\n1) Doctor\r\n2) Nurse\r\n3) Other\r\n4) Midwife', 1, 8, 0, 'ValidateHealthWorkerType', N'⚠️ Invalid input. Please select one of the options available.', 'en', GETUTCDATE(), GETUTCDATE())," +
                //"('HEALTH_WORKER_NUMBER', N'What is your health worker number? ', 1, 9, 0, 'ValidateHealthWorkerNumber', N'⚠️ Invalid format. Your health worker number shall between 2-20 characters', 'en', GETUTCDATE(), GETUTCDATE())," +
                //"('USER_DETAIL_SUMMARY', N'🗒️ *Registration Summary*\r\n\r\n*First name:* $#FIRST_NAME#$\r\n*Last name:* $#LAST_NAME#$\r\n*Your password:* $#PASSWORD#$\r\n*E-mail:* $#EMAIL#$\r\n*Gender:* $#GENDER#$\r\n*Date of birth:* $#DATE_OF_BIRTH#$\r\n*Type of health worker:* $#HEALTH_WORKER_TYPE#$\r\n*Health worker number:* $#HEALTH_WORKER_NUMBER#$\r\n', 2, 10, 0, 'ValidateRegistrationSummaryAction', '', 'en', GETUTCDATE(), GETUTCDATE())," +
                "('USER_DETAIL_SUMMARY', N'🗒️ *Registration Summary*\r\n\r\n*First name:* $#FIRST_NAME#$\r\n*Last name:* $#LAST_NAME#$\r\n*Your password:* $#PASSWORD#$\r\n*E-mail:* $#EMAIL#$\r\n', 2, 10, 0, 'ValidateRegistrationSummaryAction', '', 'en', GETUTCDATE(), GETUTCDATE())," +
                "('TERMS_CONDITIONS', N'terms and conditions', 2, 11, 0, 'ValidateTermPolicyConfirmation', '', 'en', GETUTCDATE(), GETUTCDATE())," +
                "('TERMS_CONDITIONS_CONFIRMATION', N'Press 1 to Agree with the Terms of use and Privacy policy of the platform.\r\nPress 2 to disagree with Terms of use and Privacy policy of the platform, Moodle account will not created and your data will not store.', 2, 12, 0, '', '', 'en', GETUTCDATE(), GETUTCDATE())," +
                "('SUCCESS_ACCOUNT_CREATE', N'✅ Congratulations! you have successfully registered.', 2, 13, 0, '', '', 'en', GETUTCDATE(), GETUTCDATE())," +
                "('QUESTION_TITLE', N'*Question {0} of {1}:*\r\n\r\n', 2, 14, 0, '', '', 'en', GETUTCDATE(), GETUTCDATE())," +
                "('PREVIOUS_QUESTION', N'Press {0} to go back to previous question', 3, 15, 0, '', '', 'en', GETUTCDATE(), GETUTCDATE())," +
                "('NEXT_QUESTION', N'Press {0} to jump to the next question', 3, 16, 0, '', '', 'en', GETUTCDATE(), GETUTCDATE())," +
                "('COUNTRY_CODE_CONFIRMATION', N'Your mobile number country code is {0}({1}), Your prefer language is *{2}* \r\n', 4, 17, 0, '', '', 'en', GETUTCDATE(), GETUTCDATE())," +
                "('SELECT_PREFER_LANGUAGE', N'1) Go with prefer *{0}* language\r\n', 4, 18, 0, '', '', 'en', GETUTCDATE(), GETUTCDATE())," +
                "('CHANGE_MOBILE_CODE', N'2) Change your Mobile number country code\r\n', 4, 19, 0, '', '', 'en', GETUTCDATE(), GETUTCDATE())," +
                "('SELECT_DEFAULT_LANGUAGE', N'3) Continue with *English* language\r\n', 4, 20, 0, '', '', 'en', GETUTCDATE(), GETUTCDATE())," +
                "('SELECT_LANGUAGE_USING_CODE', N'Your prefer language is *{0}* for mobile country code is *{1}*\r\n', 4, 21, 0, '', '', 'en', GETUTCDATE(), GETUTCDATE())," +
                "('VALIDATE_SUMMARY', N'1) Continue with registration process\r\n', 2, 22, 0, '', '', 'en', GETUTCDATE(), GETUTCDATE())," +
                "('UPDATE_SUMMARY_DETAILS', N'2) Insert user details again', 2, 23, 0, '', '', 'en', GETUTCDATE(), GETUTCDATE())," +
                "('UNSUCCESS_ACCOUNT_CREATE', 'Something went wrong while creating your user account in Moodle. Please try after sometime.', 2, 24, 0, '', '', 'en', GETUTCDATE(), GETUTCDATE())," +
                "('ASK_MOBILE_COUNTRY_CODE', N'Please enter mobile number country code.', 2, 25, 0, '', '', 'en', GETUTCDATE(), GETUTCDATE())");

            migrationBuilder.Sql("INSERT INTO UserCreationQuestion(MessageName, Question, MessageType, MessagePosition, IsRequired, ValidationName, ValidationMessage, LanguageISOCode, DateCreated, DateUpdated) VALUES" +
                "('FIRST_NAME', N'Por favor, digite seu primeiro nome', 1, 1, 1, 'ValidateFirstName', N'⚠️ Por favor, forneça seu primeiro nome', 'pt', GETUTCDATE(), GETUTCDATE())," +
                "('LAST_NAME', N'Por favor digite seu sobrenome', 1, 2, 1, 'ValidateLastName', N'⚠️ Por favor digite seu sobrenome', 'pt', GETUTCDATE(), GETUTCDATE())," +
                "('PASSWORD', N'*Escolha sua senha:* Sua senha deve ter no mínimo 6 caracteres.', 1, 3, 1, 'ValidatePassword', N'⚠️ Formato Inválido. Sua senha deve ter entre 6 a 12 caracteres', 'pt', GETUTCDATE(), GETUTCDATE())," +
                "('CONFIRM_PASSWORD', N'Por favor, confirme sua senha', 1, 4, 1, 'ValidateConfirmPassword', N'⚠️ Sua senha não é a mesma! Por favor, digite a mesma senha.', 'pt', GETUTCDATE(), GETUTCDATE())," +
                "('EMAIL', N'Por favor digite seu email', 1, 5, 0, 'ValidateEmail', N'⚠️ Formato Inválido. Por favor, adicione seu e-mail.', 'pt', GETUTCDATE(), GETUTCDATE())," +
                //"('GENDER', N'Qual é o seu gênero?\r\n\r\n1) Outro\r\n2) Masculino\r\n3) Feminino', 1, 6, 0, 'ValidateGenderOption', N'⚠️ Entrada inválida. Selecione uma das opções disponíveis.', 'pt', GETUTCDATE(), GETUTCDATE())," +
                //"('DATE_OF_BIRTH', N'Qual a sua data de nascimento? *Formato AAAA-MM-DD*', 1, 7, 0, 'ValidateBirthDate', N'⚠️ Formato Inválido. Seu DOB não é válido. Você deve ter mais de 18 anos para se registrar na plataforma. Por favor, tente novamente.', 'pt', GETUTCDATE(), GETUTCDATE())," +
                //"('HEALTH_WORKER_TYPE', N'Que tipo de profissional de saúde você é?\r\n\r\n1) Médico\r\n2) Enfermeira\r\n3) Outro', 1, 8, 0, 'ValidateHealthWorkerType', N'⚠️ Entrada inválida. Selecione uma das opções disponíveis.', 'pt', GETUTCDATE(), GETUTCDATE())," +
                //"('HEALTH_WORKER_NUMBER', N'Qual é o seu número de profissional de saúde?', 1, 9, 0, 'ValidateHealthWorkerNumber', N'⚠️ Formato Inválido. O número do seu profissional de saúde deve ter entre 2 e 20 caracteres', 'pt', GETUTCDATE(), GETUTCDATE())," +
                "('USER_DETAIL_SUMMARY', N'🗒️ *Resumo do Registro*\r\n\r\n*Primeiro nome:* $#FIRST_NAME#$\r\n*Último nome:* $#LAST_NAME#$\r\n*Sua senha:* $#PASSWORD#$\r\n*O email:* $#EMAIL#$\r\n', 2, 10, 0, 'ValidateRegistrationSummaryAction', '', 'pt', GETUTCDATE(), GETUTCDATE())," +
                //"('USER_DETAIL_SUMMARY', N'🗒️ *Resumo do Registro*\r\n\r\n*Primeiro nome:* $#FIRST_NAME#$\r\n*Último nome:* $#LAST_NAME#$\r\n*Sua senha:* $#PASSWORD#$\r\n*O email:* $#EMAIL#$\r\n*Gênero:* $#GENDER#$\r\n*Data de nascimento:* $#DATE_OF_BIRTH#$\r\n*Tipo de profissional de saúde:* $#HEALTH_WORKER_TYPE#$\r\n*Número do trabalhador de saúde:* $#HEALTH_WORKER_NUMBER#$\r\n', 2, 10, 0, 'ValidateRegistrationSummaryAction', '', 'pt', GETUTCDATE(), GETUTCDATE())," +
                "('TERMS_CONDITIONS', N'termos e Condições', 2, 11, 0, 'ValidateTermPolicyConfirmation', '', 'pt', GETUTCDATE(), GETUTCDATE())," +
                "('TERMS_CONDITIONS_CONFIRMATION', N'Pressione 1 para concordar com os Termos de uso e Política de Privacidade da plataforma.\r\nPressione 2 para discordar dos Termos de Uso e Política de Privacidade da plataforma, a conta Moodle não será criada e seus dados não serão armazenados.', 2, 12, 0, '', '', 'pt', GETUTCDATE(), GETUTCDATE())," +
                "('SUCCESS_ACCOUNT_CREATE', N'✅ Parabéns! Você se registrou com sucesso.', 2, 13, 0, '', '', 'pt', GETUTCDATE(), GETUTCDATE())," +
                "('QUESTION_TITLE', N'*Questão {0} de {1}:*\r\n\r\n', 2, 14, 0, '', '', 'pt', GETUTCDATE(), GETUTCDATE())," +
                "('PREVIOUS_QUESTION', N'Pressione {0} para voltar à pergunta anterior', 3, 15, 0, '', '', 'pt', GETUTCDATE(), GETUTCDATE())," +
                "('NEXT_QUESTION', N'Pressione {0} para pular para a próxima pergunta', 3, 16, 0, '', '', 'pt', GETUTCDATE(), GETUTCDATE())," +
                "('COUNTRY_CODE_CONFIRMATION', N'O código do país do seu celular é {0}({1}), Seu idioma preferido é *{2}* \r\n', 4, 17, 0, '', '', 'pt', GETUTCDATE(), GETUTCDATE())," +
                "('SELECT_PREFER_LANGUAGE', N'1) Use o idioma preferencial *{0}*\r\n', 4, 18, 0, '', '', 'pt', GETUTCDATE(), GETUTCDATE())," +
                "('CHANGE_MOBILE_CODE', N'2) Altere o código do país do seu número de celular\r\n', 4, 19, 0, '', '', 'pt', GETUTCDATE(), GETUTCDATE())," +
                "('SELECT_DEFAULT_LANGUAGE', N'3) Continue com o idioma *inglês*\r\n', 4, 20, 0, '', '', 'pt', GETUTCDATE(), GETUTCDATE())," +
                "('SELECT_LANGUAGE_USING_CODE', N'Seu idioma preferido é *{0}* para código de país móvel é *{1}*\r\n', 4, 21, 0, '', '', 'pt', GETUTCDATE(), GETUTCDATE())," +
                "('VALIDATE_SUMMARY', N'1) Continue com o processo de registro\r\n', 2, 22, 0, '', '', 'pt', GETUTCDATE(), GETUTCDATE())," +
                "('UPDATE_SUMMARY_DETAILS', N'2) Insira os detalhes do usuário novamente', 2, 23, 0, '', '', 'pt', GETUTCDATE(), GETUTCDATE())," +
                "('UNSUCCESS_ACCOUNT_CREATE', 'Algo deu errado ao criar sua conta de usuário no Moodle. Por favor, tente depois de algum tempo.', 2, 24, 0, '', '', 'pt', GETUTCDATE(), GETUTCDATE())," +
                "('ASK_MOBILE_COUNTRY_CODE', N'Insira o código do país do número do celular.', 2, 25, 0, '', '', 'pt', GETUTCDATE(), GETUTCDATE())");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LanguageISOCode",
                table: "UserCreationQuestion");
        }
    }
}
