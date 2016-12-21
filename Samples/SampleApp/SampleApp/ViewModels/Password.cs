using EasyCrypto;
using System.Windows;
using System.Windows.Input;

namespace SampleApp.ViewModels
{
    public class Password: ObservableObject
    {
        private string _generatedPasswordInput;
        private string _passwordInput;
        private string _validatePasswordInput;
        private string _validateHashInput;
        private readonly PasswordHasher _hasher = new PasswordHasher();

        public string GeneratedPasswordInput
        {
            get { return _generatedPasswordInput; }
            set
            {
                if (value != _generatedPasswordInput)
                {
                    _generatedPasswordInput = value;
                    RaisePropertyChanged();
                }
            }
        }

        public string PasswordInput
        {
            get { return _passwordInput; }
            set
            {
                if (value != _passwordInput)
                {
                    _passwordInput = value;
                    RaisePropertyChanged();
                }
            }
        }

        public string ValidatePasswordInput
        {
            get { return _validatePasswordInput; }
            set
            {
                if (value != _validatePasswordInput)
                {
                    _validatePasswordInput = value;
                    RaisePropertyChanged();
                }
            }
        }

        public string ValidateHashInput
        {
            get { return _validateHashInput; }
            set
            {
                if (value != _validateHashInput)
                {
                    _validateHashInput = value;
                    RaisePropertyChanged();
                }
            }
        }

        public ICommand GeneratePasswordCommand => new Command(GeneratePassword);
        public ICommand HashPasswordCommand => new Command(HashPassword);
        public ICommand ValidatePasswordCommand => new Command(ValidatePassword);

        private void GeneratePassword()
        {
            GeneratedPasswordInput = PasswordGenerator.GenerateStatic();
        }

        private void HashPassword()
        {
            if (string.IsNullOrEmpty(PasswordInput))
            {
                MessageBox.Show("Password is not set.");
                return;
            }
            PasswordInput = _hasher.HashPasswordAndGenerateEmbeddedSaltAsString(PasswordInput);
        }

        private void ValidatePassword()
        {
            if (string.IsNullOrEmpty(ValidatePasswordInput) || string.IsNullOrEmpty(ValidateHashInput))
            {
                MessageBox.Show("Validate password and validate hash needs to be set");
            }
            bool result = false;
            try
            {
                result = _hasher.ValidatePasswordWithEmbeddedSalt(ValidatePasswordInput, ValidateHashInput);
            }
            catch // not valid base64
            {
                result = false;
            }
            MessageBox.Show($"Is password valid: {result}");
        }
    }
}
