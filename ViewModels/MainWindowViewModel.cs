using iikoLauncher.ViewModels.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iikoLauncher.ViewModels
{
    internal class MainWindowViewModel : ViewModel
    {
        #region Заголовок окна
        private string _Title = "iiko Launcher";

        /// <summary>Заголовок окна : string</summary>
        public string Title 
        {
            get => _Title;
            set => Set(ref _Title, value);
        }
        #endregion
    }
}
