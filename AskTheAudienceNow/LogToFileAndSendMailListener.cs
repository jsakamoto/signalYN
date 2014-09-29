using System;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace AskTheAudienceNow
{
    public class LogToFileAndSendMailListener : TraceListener
    {
        public override void WriteLine(string message)
        {
            //NOP
        }

        public override void Write(string message)
        {
            //NOP
        }

        public override void TraceEvent(TraceEventCache eventCache, string source, TraceEventType eventType, int id, string message)
        {
            if (this.Filter != null)
            {
                var shouldTrace = this.Filter.ShouldTrace(eventCache, source, eventType, id, message, null, null, null);
                if (!shouldTrace) return;
            }

            try { LogToFile(message); }
            catch { }

            try { SendNotifyMail(); }
            catch { }
        }

        private void LogToFile(string message)
        {
            var context = HttpContext.Current;
            var server = context != null ? context.Server : null;
            var dir = server != null ? server.MapPath("~/App_Data/Log") : null;
            if (dir == null) return;
            if (Directory.Exists(dir) == false) Directory.CreateDirectory(dir);

            var timeZoneId = ConfigurationManager.AppSettings["site:timezone"] ?? "UTC";
            var timeZoneInfo = default(TimeZoneInfo);
            try { timeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById(timeZoneId); }
            catch { timeZoneInfo = TimeZoneInfo.Utc; }
            var now = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, timeZoneInfo);
            var nowStr = string.Format("{0:yyyy-MM-dd HH.mm.ss} UTC{1}{2:hhmm}",
                now,
                timeZoneInfo.BaseUtcOffset.Hours < 0 ? "-" : "+",
                timeZoneInfo.BaseUtcOffset);
            var fnameBase = nowStr + " {0:D3}.txt";
            var logFilePath = Enumerable.Range(1, int.MaxValue)
                .Select(n => Path.Combine(dir, string.Format(fnameBase, n)))
                .First(path => File.Exists(path) == false);

            File.WriteAllLines(logFilePath, new[] { 
                nowStr,
                message
            });
        }

        private void SendNotifyMail()
        {
            var smtpConfigJson = ConfigurationManager.AppSettings["smtp:config"];
            if (string.IsNullOrWhiteSpace(smtpConfigJson)) return;
            using (var smtpClient = CreateSmtpClient(smtpConfigJson))
            {
                var from = ConfigurationManager.AppSettings["errormail.from"];
                var to = ConfigurationManager.AppSettings["errormail.to"];
                var subject = ConfigurationManager.AppSettings["errormail.subject"];
                var mailMsg = new MailMessage(from, to)
                {
                    Subject = subject
                };

                smtpClient.Send(mailMsg);
            }
        }

        private SmtpClient CreateSmtpClient(string smtpConfigJson)
        {
            var smtpClient = JsonConvert.DeserializeObject<SmtpClient>(smtpConfigJson);
            var credential = JsonConvert.DeserializeObject<NetworkCredential>(smtpConfigJson);
            smtpClient.Credentials = credential;
            return smtpClient;
        }
    }
}