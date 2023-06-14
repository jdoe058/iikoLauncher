using iikoLauncher.Infrastructure.Commands;
using iikoLauncher.Models;
using iikoLauncher.ViewModels.Base;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
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

        public ObservableCollection<Server> ServerList { get; set; }


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

            string address = server.Address;

            if (string.IsNullOrWhiteSpace(address))
            {
                using (Process proc = new Process())
                {
                    proc.StartInfo.FileName = Environment.ExpandEnvironmentVariables(Properties.Settings.Default.AnyDeskPath);
                    proc.StartInfo.Arguments = $"{server.Login} --with-password";
                    proc.StartInfo.UseShellExecute = false;
                    proc.StartInfo.RedirectStandardInput = true;
                    _ = proc.Start();
                    proc.StandardInput.WriteLine(string.IsNullOrWhiteSpace(server.Password)
                        ? Properties.Settings.Default.AnyDeskPassword
                        : server.Password);
                }
                return;
            }

            string port = server.Port;
            string protocol = "http";
            string server_info_url = protocol;

            if (string.IsNullOrEmpty(port))
            {
                port = "443";
                protocol += "s";
                server_info_url += $"s://{address}";
            }
            else
            {
                server_info_url += $"://{address}:{port}";
            }

            XmlReader reader;

            try
            {
                reader = XmlReader.Create(server_info_url + "/resto/get_server_info.jsp?encoding=UTF-8");
            }
            catch (Exception ex)
            {
                _ = MessageBox.Show(ex.Message);
                return;
            }

            XElement xml = XDocument.Load(reader).Element("r");

            string fullVersion = xml.Element("version")?.Value;
            bool isChain = Equals(xml.Element("edition")?.Value, "chain");
            string launchExec = Path.Combine(Environment.ExpandEnvironmentVariables(isChain 
                ? Properties.Settings.Default.IikoChainPath
                : Properties.Settings.Default.IikoRMSPath),
                fullVersion.Substring(0, fullVersion.Length - 2), @"BackOffice.exe");

            if (!File.Exists(launchExec))
            {
                _ = MessageBox.Show($"Отсутствует офис\n\n{launchExec}");
                return;
            }

            DirectoryInfo di = Directory.CreateDirectory(Path.Combine(
                Environment.ExpandEnvironmentVariables(@"%AppData%\iiko"), isChain ? "Chain" : "RMS", address, "config"));
            new XDocument(
                new XElement("config",
                    new XElement("ServersList",
                        new XElement("Protocol", protocol),
                        new XElement("ServerAddr", address),
                        new XElement("ServerSubUrl", "/resto"),
                        new XElement("Port", port),
                        new XElement("IsPresent", false)
                    ),
                    new XElement("Login", string.IsNullOrWhiteSpace(server.Login)
                        ? Properties.Settings.Default.IikoLogin
                        : server.Login)
                )
            ).Save(Path.Combine(di.FullName, @"backclient.config.xml"));

            _ = Process.Start(launchExec, $"/password={(string.IsNullOrWhiteSpace(server.Password)?Properties.Settings.Default.IikoPassword: server.Password) } /AdditionalTmpFolder={address}");
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

            StreamReader reader = new StreamReader(Environment.ExpandEnvironmentVariables(Properties.Settings.Default.ConnectionListPath));
            Servers s = (Servers)serializer.Deserialize(reader);
            ServerList = new ObservableCollection<Server>(s.Server);
            reader.Close();

            string headerLine = string.Join(";", ServerList[0].GetType().GetProperties().Select(p => p.Name));
            var dataLines = from conn in ServerList
                            let dataLine = string.Join(";", conn.GetType().GetProperties().Select(p => p.GetValue(conn)))
                            select dataLine;
            var csvData = new List<string>
            {
                headerLine
            };
            csvData.AddRange(dataLines);
            System.IO.File.WriteAllLines(@".\iikoLauncher.csv", csvData); 
        }
    }
}
