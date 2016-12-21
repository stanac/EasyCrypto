using System.Windows;
using System.Windows.Input;

namespace SampleApp.ViewModels
{
    public abstract class EncryptDecryptBase: ObservableObject
    {
        private string _password;
        private string _encrypt;
        private string _decrypt;

        public string Password
        {
            get { return _password; }
            set
            {
                if (value != _password)
                {
                    _password = value;
                    RaisePropertyChanged();
                }
            }
        }

        public string EncryptInput
        {
            get { return _encrypt; }
            set
            {
                if (value != _encrypt)
                {
                    _encrypt = value;
                    RaisePropertyChanged();
                }
            }
        }

        public string DecryptInput
        {
            get { return _decrypt; }
            set
            {
                if (value != _decrypt)
                {
                    _decrypt = value;
                    RaisePropertyChanged();
                }
            }
        }

        public ICommand EncryptCommand => new Command(Encrypt);
        public ICommand DecryptCommand => new Command(Decrypt);
        
        protected abstract void Encrypt();

        protected abstract void Decrypt();

        protected virtual bool ValidatePassword()
        {
            if (string.IsNullOrEmpty(Password))
            {
                MessageBox.Show("Password is not set.");
                return false;
            }
            return true;
        }
    }
}
