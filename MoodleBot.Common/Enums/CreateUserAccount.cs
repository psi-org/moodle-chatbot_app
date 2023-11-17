using System.ComponentModel;

namespace MoodleBot.Common.Enums
{
    public enum CreateUserMessageType
    {
        [Description("UserDetailQuestion")]
        UserDetailQuestion = 1,

        [Description("Generic")]
        Generic = 2,

        [Description("NextPreviousQuestion")]
        NextPreviousQuestion = 3,

        [Description("LanguageSelection")]
        LanguageSelection = 4
    }

    public enum CreateUserMessageName
    {
        USER_ID,
        FIRST_NAME,
        LAST_NAME,
        PASSWORD,
        CONFIRM_PASSWORD,
        EMAIL,
        //GENDER,
        //DATE_OF_BIRTH,
        //HEALTH_WORKER_TYPE,
        //HEALTH_WORKER_NUMBER,
        PREVIOUS_QUESTION,
        NEXT_QUESTION,
        QUESTION_TITLE,
        USER_DETAIL_SUMMARY,
        SUCCESS_ACCOUNT_CREATE,
        SUCCESS_ACCOUNT_UPDATE,
        TERMS_CONDITIONS,
        TERMS_CONDITIONS_CONFIRMATION,
        COUNTRY_CODE_CONFIRMATION,
        SELECT_PREFER_LANGUAGE,
        CHANGE_MOBILE_CODE,
        SELECT_DEFAULT_LANGUAGE,
        SELECT_LANGUAGE_USING_CODE,
        VALIDATE_SUMMARY,
        UPDATE_SUMMARY_DETAILS,
        UNSUCCESS_ACCOUNT_CREATE,
        ASK_MOBILE_COUNTRY_CODE
    }

    /*public enum Gender
    {
        [Description("Male")]
        Male = 1,

        [Description("Female")]
        Female = 2,

        [Description("Other")]
        Other = 3
    }*/

    public enum HealthWorkerType
    {
        [Description("Doctor")]
        Doctor = 1,

        [Description("Nurse")]
        Nurse = 2,

        [Description("Other")]
        Other = 3,

        [Description("Midwife")]
        Midwife = 4
    }
}
