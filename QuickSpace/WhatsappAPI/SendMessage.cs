using Twilio;
using Twilio.Rest.Api.V2010.Account;

namespace QuickSpace.WhatsappAPI
{
    public class SendMessage
    {
        public static bool Send(string to, string body) {
            bool result = false; try
            {
                var accountSid = "AC60487f376a1014a8f9ce8487f4debee6";
                var authToken = "b56c686fdf94e79fe3af702505ec4466";
                TwilioClient.Init(accountSid, authToken);

                var messageOptions = new CreateMessageOptions(to);
                messageOptions.MessagingServiceSid = "MGb742048aaafbc497d14c00d628263da9";
                messageOptions.Body = body;

                var message = MessageResource.Create(messageOptions);
                result = true;
                //MessageBox.Show(message.Body);
            }
            catch (Exception) { 
                result = false;
            }
            return result;
        }     
    }
}
