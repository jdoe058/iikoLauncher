using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using System.Xml.Serialization;

namespace iikoLauncher.Models
{
    [Serializable()]
    public class Server : INotifyPropertyChanged
    {
        private string _ClientName;
        private string _Name;
        private string _Login;

        [XmlAttribute]
        public string ClientName
        {
            get => _ClientName;
            set
            {
                _ClientName = value;
                OnPropertyChanged();
            }
        }

        [XmlAttribute]
        public string Name
        {
            get => _Name;
            set
            {
                _Name = value;
                OnPropertyChanged();
            }
        }

        [XmlAttribute]
        public string Login {
            get => string.IsNullOrWhiteSpace(_Login)
                ? Properties.Settings.Default.IikoLogin
                : _Login;
            set
            {
                _Login = value;
                OnPropertyChanged();
            }
        }

        [XmlIgnore]
        public string Password
        {
            get => Encoding.UTF8.GetString(Convert.FromBase64String(Crypted_Password is null
                    ? (string.IsNullOrWhiteSpace(Address)
                        ? Properties.Settings.Default.AnyDeskPassword
                        : Properties.Settings.Default.IikoPassword)
                    : Crypted_Password));
            set => Crypted_Password = Convert.ToBase64String(Encoding.UTF8.GetBytes(value));
        }

        [XmlAttribute]
        public string Address { get; set; }

        [XmlAttribute]
        public string Port { get; set; }

        [XmlAttribute]
        public string Crypted_Password;

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string PropertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(PropertyName));
        }
    }
}
