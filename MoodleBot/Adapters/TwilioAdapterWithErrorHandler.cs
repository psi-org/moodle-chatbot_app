using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Adapters.Twilio;
using Microsoft.Bot.Schema;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;

namespace MoodleBot.Adapters
{
    public class TwilioAdapterWithErrorHandler : TwilioAdapter
    {
        public TwilioAdapterWithErrorHandler(IConfiguration configuration, ILogger<TwilioAdapter> logger, ConversationState conversationState = null, UserState userState = null)
                : base(configuration, null, logger)
        {
            OnTurnError = async (turnContext, exception) =>
            {
                // Log any leaked exception from the application.
                logger.LogError($"Exception caught : {exception.Message}");

                var errorMsgText = "Sorry, it looks like something went wrong.";

                // Send a catch-all apology to the user.
                if (exception.Message == "Invalid input attempt exceed.")
                {
                    errorMsgText = "You have exceed invalid input limit.";
                }

                var errorMessage = MessageFactory.Text(errorMsgText, errorMsgText, InputHints.ExpectingInput);
                await turnContext.SendActivityAsync(errorMessage);

                if (conversationState != null)
                {
                    try
                    {
                        // Delete the conversationState for the current conversation to prevent the
                        // bot from getting stuck in a error-loop caused by being in a bad state.
                        // ConversationState should be thought of as similar to "cookie-state" in a Web pages.
                        var resetMessage = MessageFactory.Text("Please start from first, by sending message.", "Please start from first, by sending message.", InputHints.ExpectingInput);
                        await turnContext.SendActivityAsync(resetMessage);
                        await conversationState.DeleteAsync(turnContext);
                        await userState.DeleteAsync(turnContext);
                    }
                    catch (Exception e)
                    {
                        logger.LogError($"Exception caught on attempting to Delete ConversationState : {e.Message}");
                    }
                }
            };
        }
    }
}
