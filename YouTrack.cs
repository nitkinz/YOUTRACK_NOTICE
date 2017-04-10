using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using log4net;
using YouTrackSharp.Infrastructure;
using YouTrackSharp.Issues;
using System.Net.Mail;
using System.Net;

namespace YOUTRACK_NOTICE
{
    public partial class YouTrack : ServiceBase
    {
        public YouTrack()
        {
            InitializeComponent();
        }

        private static ILog _log;
        private static System.Timers.Timer _timer;
        private static Connection _connection;
        private static IssueManagement _issueManagement;
        //youtrack
        private const string LOGIN = "";
        private const string PASSWORD = "";
        private const string HOST = "";
        private const int PORT = ;
        private const string PROJECT_IDENTIFICATOR = "";
        private const int START_NUM_TASKS_FOR_FIND = ;
        private Stopwatch _stopwatch = new Stopwatch();

        //mail
        private static string _smtpServer = "";
        private static string _fromEmail = "";
        private static string _passwordEmail = "";
        private static List<string> _dima = new List<string>();
        static List<string> _den = new List<string>();
        static List<string> _srg = new List<string>();
        static List<string> _wlad = new List<string>();
        private object _sync;

        protected override void OnStart(string[] args)
        {
            try
            {
                _sync = new object();

                FileInfo f = new FileInfo("log4net.config");
                log4net.Config.XmlConfigurator.Configure(f);
                _log = LogManager.GetLogger(typeof(Program));

                _log.Debug("OnStart");
                _connection = new Connection(HOST, port: PORT, useSSL: false, path: null);
                _connection.Authenticate(LOGIN, PASSWORD);

                _issueManagement = new IssueManagement(_connection);
                var iis = _issueManagement.GetAllIssuesForProject(PROJECT_IDENTIFICATOR, Int32.MaxValue, start: START_NUM_TASKS_FOR_FIND);
                foreach (dynamic issue in iis)
                {
                    if (issue.assigneeName == "nitkin")
                    {
                        _dima.Add(issue.Id.ToString());
                    }
                    if (issue.assigneeName == "denneeeee")
                    {
                        _den.Add(issue.Id.ToString());
                    }
                    if (issue.assigneeName == "sgmm")
                    {
                        _srg.Add(issue.Id.ToString());
                    }
                    if (issue.assigneeName == "Wlad-popow")
                    {
                        _wlad.Add(issue.Id.ToString());
                    }
                }

                _timer = new System.Timers.Timer();
                _timer.Interval = 60000; // каждую минуту
                _timer.Elapsed += OnTimer;

                _timer.Start();
            }
            catch (Exception ex)
            {
                _log.Error(ex);
                throw;
            }

        }

        //public void onDebug()
        //{
        //    OnStart(null);
        //}

        /// <summary>
        /// Отправка письма на почтовый ящик C# mail send
        /// </summary>
        /// <param name="smtpServer">Имя SMTP-сервера</param>
        /// <param name="from">Адрес отправителя</param>
        /// <param name="password">пароль к почтовому ящику отправителя</param>
        /// <param name="mailto">Адрес получателя</param>
        /// <param name="caption">Тема письма</param>
        /// <param name="message">Сообщение</param>
        /// <param name="attachFile">Присоединенный файл</param>
        public static void SendMail(string mailto, string caption, string message, string attachFile = null)
        {
            try
            {
                MailMessage mail = new MailMessage();
                mail.From = new MailAddress(_fromEmail);
                mail.To.Add(new MailAddress(mailto));
                mail.Subject = caption;
                mail.Body = message;
                if (!string.IsNullOrEmpty(attachFile))
                    mail.Attachments.Add(new Attachment(attachFile));
                SmtpClient client = new SmtpClient();
                client.Host = _smtpServer;
                client.Port = 587;
                client.EnableSsl = true;
                client.Credentials = new NetworkCredential(_fromEmail.Split('@')[0], _passwordEmail);
                client.DeliveryMethod = SmtpDeliveryMethod.Network;
                client.Send(mail);
                mail.Dispose();
            }
            catch (Exception e)
            {
                _log.Error("Mail.Send: " + e.Message);             
            }
        }

        public void OnTimer(object sender, System.Timers.ElapsedEventArgs args)
        {
            lock (_sync)
            {

                _log.Debug("START FIND");
                _stopwatch.Start();

                try
                {
                    var iis = _issueManagement.GetAllIssuesForProject(PROJECT_IDENTIFICATOR, Int32.MaxValue,
                        start: START_NUM_TASKS_FOR_FIND);
                    List<string> taskOfnitkin = new List<string>();
                    List<string> taskOfdenneeeee = new List<string>();
                    List<string> taskOfsgmm = new List<string>();
                    List<string> taskOfWlad = new List<string>();

                    foreach (dynamic issue in iis)
                    {
                        if (issue.assigneeName == "nitkin")
                        {
                            taskOfnitkin.Add(issue.Id.ToString());
                        }
                        if (issue.assigneeName == "denneeeee")
                        {
                            taskOfdenneeeee.Add(issue.Id.ToString());
                        }
                        if (issue.assigneeName == "sgmm")
                        {
                            taskOfsgmm.Add(issue.Id.ToString());
                        }
                        if (issue.assigneeName == "Wlad-popow")
                        {
                            taskOfWlad.Add(issue.Id.ToString());
                        }
                    }

                    FindTask(taskOfnitkin, _dima, "");
                 //   FindTask(taskOfdenneeeee, _dima, "x"); TODO
                 //   FindTask(taskOfsgmm, _dima, "x");
                //    FindTask(taskOfnitkin, _dima, "x");

                    _dima = taskOfnitkin;
                    _den = taskOfdenneeeee;
                    _srg = taskOfsgmm;
                    _wlad = taskOfWlad;

                    _stopwatch.Stop();
                    _log.Debug(_stopwatch.ElapsedMilliseconds);
                }
                catch (Exception ex)
                {
                    _log.Error(ex);
                    throw;
                }
            }
        }

        private void FindTask(List<string> tasks, List<string> userExistTask, string email)
        {
            foreach (var task in tasks)
            {
                if (!userExistTask.Contains(task))
                {
                    SendMail(email, "НОВАЯ ЗАДАЧА", "NEW TASK = " + task);

                    _log.Debug("NEW TASK=" + task);
                }
            }

            foreach (var task in _dima)
            {
                if (!tasks.Contains(task))
                {
                    SendMail(email, "СНЯТИЕ ЗАДАЧИ", "DELETE TASK = " + task);

                    _log.Debug("DELETE TASK=" + task);
                }
            }

        }

        protected override void OnStop()
        {
            _log.Debug("OnStop");
            _connection = null;
            _issueManagement = null;
            _timer.Elapsed -= OnTimer;
            _timer.Stop();
            _timer = null;
        }
    }
}
