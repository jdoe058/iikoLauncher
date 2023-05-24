using iikoLauncher.Infrastructure.Commands;
using iikoLauncher.Models;
using iikoLauncher.ViewModels.Base;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;

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


        private ObservableCollection<Server> _Servers;

        public ObservableCollection<Server> ServerList
        {
            get { return _Servers; }
            set { _Servers = value; }
        }


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

        private bool CanLaunchOfficeCommandExecute(object p) => true;

        private void OnLaunchOfficeCommandExecuted(object p)
        {
            Server server = p as Server;

            //string version = Attr["Version"].Value;
            
            string address = server.Address;
            string port = server.Port;
            string protocol = string.IsNullOrEmpty(port) ? "https" : "http";
            string suffix = "/resto";

            string url = $"{protocol}://{address}";
            if (!string.IsNullOrEmpty(port))
            {
                url += $":{port}";
            } else
            {
                port = "443";
            }

            XmlReader reader;

            try
            {
                reader = XmlReader.Create(url + "/resto/get_server_info.jsp?encoding=UTF-8");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return;
            }

            XElement xml = XDocument.Load(reader).Element("r");
            string s = xml.Element("version")?.Value;
            bool isChain = Equals(xml.Element("edition")?.Value, "chain");

            string launchExec = Path.Combine(isChain ? @"C:\Program files\iiko\iikoChain" : @"C:\Program files\iiko\iikoRMS", s.Substring(0, s.Length - 2), @"BackOffice.exe");

            if (!File.Exists(launchExec))
            {
                MessageBox.Show($"Отсутствует офис\n\n{launchExec}");
                return;
            }

            string configDir = Path.Combine(Environment.ExpandEnvironmentVariables(@"%AppData%\iiko\"), isChain ? "Chain" : "RMS", address, "config");

            Directory.CreateDirectory(configDir);
         
            XDocument xdoc = new XDocument(
                new XElement("config",
                    new XElement("ServersList",
                        new XElement("Protocol", protocol),
                        new XElement("ServerAddr", address),
                        new XElement("ServerSubUrl", suffix),
                        new XElement("Port", port),
                        new XElement("IsPresent", false)
                    ),
                    new XElement("Login", server.Login)
                )
            );
            xdoc.Save(Path.Combine(configDir, @"backclient.config.xml"));

            
            string launchParam = $"/password={server.Password} /AdditionalTmpFolder={address}";
            //string launchParam = $"/AdditionalTmpFolder={address}";

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

            XmlSerializer serializer = new XmlSerializer(typeof(Servers));

            //string file = Path.Combine();

            //
            StreamReader reader = new StreamReader(Environment.ExpandEnvironmentVariables(@"%USERPROFILE%\Desktop\iikoLauncher.xml"));
            Servers s = (Servers)serializer.Deserialize(reader);
            ServerList = new ObservableCollection<Server>(s.Server);
            reader.Close();
        }
    }
}
