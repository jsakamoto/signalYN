using System;
using System.Net;
using System.Net.Mail;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Toolbelt.DynamicBinderExtension;

namespace AskTheAudienceNow.Test
{
    [TestClass]
    public class LogToFileAndSendMailListenerTest
    {
        [TestMethod]
        public void CreateSmtpClientTest()
        {
            var listener = new LogToFileAndSendMailListener();
            var smtpClient = (SmtpClient)listener
                .ToDynamic()
                .CreateSmtpClient("{Host:'test.example.com',Port:123,UserName:'fizz',Password:'buzz'}");

            smtpClient.Host.Is("test.example.com");
            smtpClient.Port.Is(123);

            var credential = smtpClient.Credentials.IsInstanceOf<NetworkCredential>();
            credential.UserName.Is("fizz");
            credential.Password.Is("buzz");
        }
    }
}
