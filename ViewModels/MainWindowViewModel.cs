using iikoLauncher.Infrastructure.Commands;
using iikoLauncher.Models;
using iikoLauncher.ViewModels.Base;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Xml;
using System.Xml.Linq;

namespace iikoLauncher.ViewModels
{
    internal class MainWindowViewModel : ViewModel
    {
        #region Title : string - Заголовок окна

        /// <summary>Заголовок окна</summary>
        private string _Title = "iiko Launcher";

        /// <summary>Заголовок окна</summary>
        public string Title 
        {
            get => _Title;
            set => Set(ref _Title, value);
        }

        #endregion

        #region Status : string - Статус программы

        /// <summary>Статус программы</summary>
        private string _Status = "Готов";

        /// <summary>Статус программы</summary>
        public string Status
        {
            get => _Status;
            set => Set(ref _Status, value);
        }

        #endregion

        #region SelectedServer : Server - Выбранный сервер

        /// <summary>Выбранный сервер</summary>
        private Server _SelectedServer;

        /// <summary>Выбранный сервер</summary>
        public Server SelectedServer
        {
            get => _SelectedServer;
            set => Set(ref _SelectedServer, value);
        }

        #endregion

        #region Login : string - Логин
        /// <summary>Логин</summary>
        private string _Login = "admin";

        /// <summary>Логин</summary>
        public string Login
        {
            get => _Login;
            set => Set(ref _Login, value);
        }
        #endregion

        #region Password : string - Пароль
        /// <summary>Пароль</summary>
        private string _Password = "password";

        /// <summary>Пароль</summary>
        public string Password
        {
            get => _Password;
            set => Set(ref _Password, value);
        }
        #endregion

        #region Команды

        #region CloseApplicationCommand
        public ICommand CloseApplicationCommand { get; }

        private bool CanCloseApplicationCommandExecute(object p) => true;

        private void OnCloseApplicationCommandExecuted(object p) 
        {
            Application.Current.Shutdown();
        }

        #endregion

        #region DownloadOfficeCommand

        public ICommand DownloadOfficeCommand { get; }

        private bool CanDownloadOfficeCommandExecute(object p) => false;

        private void OnDownloadOfficeCommandExecuted(object p) { throw new NotImplementedException(); }
        #endregion

        #region LaunchOfficeCommand
        public ICommand LaunchOfficeCommand { get; }

        private string isLaunchPath(bool isChain, string version) 
        {
            string launchExec = Path.Combine(isChain ? @"C:\Program files\iiko\iikoChain" : @"C:\Program files\iiko\iikoRMS", version, @"BackOffice.exe");
            return File.Exists(launchExec) ? launchExec : "";
        }

        private bool CanLaunchOfficeCommandExecute(object p) 
        {
            var Attr = p as XmlAttributeCollection;

            if (Attr is null) return false;

            bool isChain = Equals(Attr["IsChain"]?.Value, "True");
            string version = Attr["Version"].Value;

            return !String.IsNullOrEmpty(isLaunchPath(isChain, version));
        }
        

        private void OnLaunchOfficeCommandExecuted(object p)
        {
            var Attr = p as XmlAttributeCollection;

            bool isChain = Equals(Attr["IsChain"]?.Value,"True");
            string version = Attr["Version"].Value;

            string pattern = @"^(https?)://([-\.\w]+):?(\d*)(/resto)$";

            GroupCollection gc = Regex.Match(Attr["URL"].Value, pattern).Groups;

            string protocol = gc[1].Value;
            string address = gc[2].Value;
            string port = gc[3].Value != string.Empty ? gc[3].Value : "443";
            string suffix = gc[4].Value;

            string configDir = Path.Combine(Environment.ExpandEnvironmentVariables(@"%AppData%\iiko"), isChain ? @"Chain" : @"RMS", address, @"config");

            Directory.CreateDirectory(configDir);
         
            XDocument xdoc = new XDocument(
                new XElement("config",
                    new XElement("ServersList",
                        new XElement("Protocol", protocol),
                        new XElement("ServerAddr", address),
                        new XElement("ServerSubUrl", suffix),
                        new XElement("Port", port),
                        new XElement("IsPresent", true)
                    )
                )
            );
            xdoc.Save(Path.Combine(configDir, @"backclient.config.xml"));
                        

            string launchExec = Path.Combine(isChain ? @"C:\Program files\iiko\iikoChain" : @"C:\Program files\iiko\iikoRMS", version, @"BackOffice.exe");
            //string launchParam = $" /login={Login} /password={Password} /AdditionalTmpFolder={address}";
            string launchParam = $"/AdditionalTmpFolder={address}";

            System.Diagnostics.Process.Start(launchExec, launchParam);
        }
        #endregion

        #endregion

        public MainWindowViewModel()
        {
            #region Команды

            CloseApplicationCommand = new LambdaCommand(OnCloseApplicationCommandExecuted, CanCloseApplicationCommandExecute);

            DownloadOfficeCommand = new LambdaCommand(OnDownloadOfficeCommandExecuted, CanDownloadOfficeCommandExecute);

            LaunchOfficeCommand = new LambdaCommand(OnLaunchOfficeCommandExecuted, CanLaunchOfficeCommandExecute);

            #endregion

        }
    }
}
