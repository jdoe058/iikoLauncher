﻿using iikoLauncher.Infrastructure.Commands;
using iikoLauncher.Models;
using iikoLauncher.ViewModels.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

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

        private bool CanLaunchOfficeCommandExecute(object p) => false;

        private void OnLaunchOfficeCommandExecuted(object p)
        { 
            throw new NotImplementedException();
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
