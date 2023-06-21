using iikoLauncher.Infrastructure.Commands;
using iikoLauncher.Models;
using iikoLauncher.ViewModels.Base;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace iikoLauncher.ViewModels
{
    internal class MainWindowViewModel : ViewModel
    {
        #region Свойства
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
        #region ConnectionFilter : string - Фильтр подключений
        /// <summary>Фильтр подключений</summary>
        private string _connectionFilter = "";

        /// <summary>Фильтр подключений</summary>
        public string ConnectionFilter
        {
            get => _connectionFilter;
            set
            {
                if (Set(ref _connectionFilter, value))
                {
                    _collection?.Refresh();
                }
            }
        }

        #endregion
        #region SelectedConnection : Connection - Выбранное подключение
        /// <summary>Выбранное подключение</summary>
        private Server _SelectedConnection;

        /// <summary>Выбранное подключение</summary>
        public Server SelectedConnection
        {
            get => _SelectedConnection;
            set
            {
                if(Set(ref _SelectedConnection, value))
                {
                    _collection.Refresh();
                }
            }
        }
        #endregion
        public string AnyDeskPath { get; set; } = Environment.ExpandEnvironmentVariables(Properties.Settings.Default.AnyDeskPath);
        public string IikoRMSPath { get; set; } = Environment.ExpandEnvironmentVariables(Properties.Settings.Default.IikoRMSPath);
        public string IikoChainPath { get; set; } = Environment.ExpandEnvironmentVariables(Properties.Settings.Default.IikoChainPath);
        public string ConnectionListPath { get; set; } = Environment.ExpandEnvironmentVariables(Properties.Settings.Default.ConnectionListPath);
        public string SaveConnectionListPath { get; set; } = Environment.ExpandEnvironmentVariables(@"C:\test.xml");
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
        #region ClearAndFocusCommand

        public ICommand ClearAndFocusCommand { get; }

        private bool CanClearAndFocusCommandExecute(object p) => true;

        private void OnClearAndFocusCommandExecuted(object p) 
        {
            TextBox box = p as TextBox;
            box.Text = "";
            box.Focus();
        }
        #endregion
        #region LaunchAnyDeskCommand
        public ICommand LaunchAnyDeskCommand { get; }
        public bool CanLaunchAnyDeskCommandExecute(object p)
        {
            return p is Server s && string.IsNullOrWhiteSpace(s.Address);
        }

        public void OnLaunchAnyDeskCommandExecuted(object p)
        {
            if (!(p is Server s))
            {
                return;
            }

            using (Process proc = new Process())
            {
                proc.StartInfo.FileName = AnyDeskPath;
                proc.StartInfo.Arguments = $"{s.Login} --with-password";
                proc.StartInfo.UseShellExecute = false;
                proc.StartInfo.RedirectStandardInput = true;
                _ = proc.Start();
                proc.StandardInput.WriteLine(s.Password);
            }
        }

        #endregion
        #region LaunchOfficeCommand
        public ICommand LaunchOfficeCommand { get; }
        private bool CanLaunchOfficeCommandExecute(object p)
        {
            return p is Server s && !string.IsNullOrWhiteSpace(s.Address);
        }
        private void OnLaunchOfficeCommandExecuted(object p)
        {
            if (!(p is Server server))
            {
                return;
            }

            string address = server.Address;

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
            string launchExec = Path.Combine(isChain ? IikoChainPath : IikoRMSPath,
                fullVersion.Substring(0, fullVersion.Length - 2), @"BackOffice.exe");

            string type = isChain ? "Chain" : "RMS";

            if (!File.Exists(launchExec))
            {
                if (MessageBox.Show($"Скачать установщик офиса версии {fullVersion}",
                    "Отсутствует офис",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question
                    ) == MessageBoxResult.Yes)
                {
                    _ = Process.Start(@".\curl", "-u partners:partners#iiko"
                        + Environment.ExpandEnvironmentVariables($" -o %USERPROFILE%\\Downloads\\{fullVersion}.Setup.{type}.BackOffice.exe")
                        + $"ftp://ftp.iiko.ru/release_iiko/{fullVersion}/Setup/Offline/Setup.{type}.BackOffice.exe");
                }
                return;
            }

            DirectoryInfo di = Directory.CreateDirectory(Path.Combine(
                Environment.ExpandEnvironmentVariables(@"%AppData%\iiko"), type, address, "config"));
            new XDocument(
                new XElement("config",
                    new XElement("ServersList",
                        new XElement("Protocol", protocol),
                        new XElement("ServerAddr", address),
                        new XElement("ServerSubUrl", "/resto"),
                        new XElement("Port", port),
                        new XElement("IsPresent", false)
                    ),
                    new XElement("Login", server.Login)
                )
            ).Save(Path.Combine(di.FullName, @"backclient.config.xml"));
            _ = Process.Start(launchExec, $"/password={server.Password} /AdditionalTmpFolder={address}");
        }
        #endregion
        #region LoadConnectionCommand
        public ICommand LoadConnectionListCommand { get; }

        private bool CanLoadConnectionListCommandExecute(object p) => true;

        private void OnLoadConnectionListCommandExecuted(object p)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(List<Server>));

            using (StreamReader reader = new StreamReader(ConnectionListPath))
            {
                List<Server> s = (List<Server>)serializer.Deserialize(reader);
                ServerList = new ObservableCollection<Server>(s);
            }
        }
        #endregion
        #region SaveConnectionListCommand
        public ICommand SaveConnectionListCommand { get; }
        public bool CanSaveConnectionListCommandExecute(object p) => true;
        public void OnSaveConnectionListCommandExecuted(object p)
        {
            if (File.Exists(SaveConnectionListPath))
            {
                string s = Path.ChangeExtension(SaveConnectionListPath, DateTime.Now.ToString("yyMMdd-HHmmss") + ".xml");
                File.Move(SaveConnectionListPath, s);
            }
            XmlSerializer xml = new XmlSerializer(typeof(List<Server>));
            using (StreamWriter writer = new StreamWriter(SaveConnectionListPath))
            {
                xml.Serialize(writer, ServerList.ToList());
            }
        }
        #endregion
        #region DoubleConnection
        public ICommand DoubleConnectionCommand { get; }
        public bool CanDoubleConnectionCommandExecute(object p) => false;

        public void OnDoubleConnectionCommandExecuted(object p)
        {
        }
        #endregion
        #endregion


        public ObservableCollection<Server> ServerList { get; set; }
        private readonly ICollectionView _collection;
        public MainWindowViewModel()
        {
            #region Команды
            LaunchOfficeCommand = new LambdaCommand(OnLaunchOfficeCommandExecuted, CanLaunchOfficeCommandExecute);
            LaunchAnyDeskCommand = new LambdaCommand(OnLaunchAnyDeskCommandExecuted, CanLaunchAnyDeskCommandExecute);
            ClearAndFocusCommand = new LambdaCommand(OnClearAndFocusCommandExecuted, CanClearAndFocusCommandExecute);
            DoubleConnectionCommand = new LambdaCommand(OnDoubleConnectionCommandExecuted, CanDoubleConnectionCommandExecute);
            CloseApplicationCommand = new LambdaCommand(OnCloseApplicationCommandExecuted, CanCloseApplicationCommandExecute);
            LoadConnectionListCommand = new LambdaCommand(OnLoadConnectionListCommandExecuted, CanLoadConnectionListCommandExecute);
            SaveConnectionListCommand = new LambdaCommand(OnSaveConnectionListCommandExecuted, CanSaveConnectionListCommandExecute);
            #endregion

            try
            {
                OnLoadConnectionListCommandExecuted(ConnectionListPath);
            }

            catch (Exception ex)
            {
                _ = MessageBox.Show($"Нет доступа к файлу настроек.\n\n{ex}");
                return;
            }

            string headerLine = string.Join(";", ServerList[0].GetType().GetProperties().Select(p => p.Name));
            IEnumerable<string> dataLines = from conn in ServerList
                            let dataLine = string.Join(";", conn.GetType().GetProperties().Select(p => p.GetValue(conn)))
                            select dataLine;
            List<string> csvData = new List<string>
            {
                headerLine
            };
            csvData.AddRange(dataLines);

            try
            {
                //File.WriteAllLines(@"G:\Мой Диск\iikoLauncher\iikoLauncher.csv", csvData);
            }
            catch (Exception ex)
            {
                _ = MessageBox.Show($"Нет доступа к итоговому файлу.\nВозможно он открыт в Excel'е.\n\n{ex}");
                return;
            }

            _collection = CollectionViewSource.GetDefaultView(ServerList);
            _collection.Filter += OnConnectionFiltred;
            _collection.SortDescriptions.Add(new SortDescription("ClientName",ListSortDirection.Ascending));
            _collection.SortDescriptions.Add(new SortDescription("Name",ListSortDirection.Ascending));
            _collection.GroupDescriptions.Add(new PropertyGroupDescription("ClientName"));
        }

        private bool OnConnectionFiltred(object obj)
        {
            Server s = obj as Server;
            return (s?.Name?.IndexOf(_connectionFilter, StringComparison.OrdinalIgnoreCase) > -1)
                || (s?.ClientName?.IndexOf(_connectionFilter, StringComparison.OrdinalIgnoreCase) > -1);
        }
    }
}
