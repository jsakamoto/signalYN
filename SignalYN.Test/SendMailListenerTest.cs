using System;
using System.Net;
using System.Net.Mail;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Toolbelt.DynamicBinderExtension;

namespace SignalYN.Test
{
    [TestClass]
    public class SendMailListenerTest
    {
        [TestMethod]
        public void CreateSmtpClientTest()
        {
            var listener = new SendMailListener();
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
