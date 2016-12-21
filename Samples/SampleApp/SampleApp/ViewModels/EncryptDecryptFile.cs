using System;
using System.IO;
using System.Windows;
using System.Windows.Input;

namespace SampleApp.ViewModels
{
    public class EncryptDecryptFile : ObservableObject
    {
        private string _encryptInputFile;
        private string _decryptInputFile;
        private int _progress;
        private EasyCrypto.ReportAndCancellationToken _token;
        private string _password;
        private bool _operationInProgress;

        public string EncryptInputFile
        {
            get { return _encryptInputFile; }
            set
            {
                if (value != _encryptInputFile)
                {
                    _encryptInputFile = value;
                    RaisePropertyChanged();
                    RaisePropertyChanged(nameof(EncryptOutputFile));
                }
            }
        }

        public string EncryptOutputFile
        {
            get
            {
                if (string.IsNullOrEmpty(EncryptInputFile)) return null;
                return EncryptInputFile + ".encrypted";
            }
        }
        
        public string DecryptInputFile
        {
            get { return _decryptInputFile; }
            set
            {
                if (value != _decryptInputFile)
                {
                    _decryptInputFile = value;
                    RaisePropertyChanged();
                    RaisePropertyChanged(nameof(DecryptOutputFile));
                }
            }
        }

        public string DecryptOutputFile
        {
            get
            {
                if (string.IsNullOrEmpty(DecryptInputFile)) return null;
                return DecryptInputFile.Substring(0, DecryptInputFile.Length - ".encrypted".Length);
            }
        }

        public bool OperationInProgress
        {
            get { return _operationInProgress; }
            set
            {
                if (value != _operationInProgress)
                {
                    _operationInProgress = value;
                    RaisePropertyChanged();
                }
            }
        }

        public int Progress
        {
            get { return _progress; }
            set
            {
                if (value != _progress)
                {
                    _progress = value;
                    RaisePropertyChanged();
                }
            }
        }

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

        public ICommand SetFileToEncryptCommand => new Command(SetFileToEncrypt);
        public ICommand SetFileToDecryptCommand => new Command(SetFileToDecrypt);
        public ICommand EncryptCommand => new Command(Encrypt);
        public ICommand DecryptCommand => new Command(Decrypt);
        public ICommand CancelCommand => new Command(ResetProgress);

        private void SetFileToEncrypt()
        {
            Microsoft.Win32.OpenFileDialog dialog = new Microsoft.Win32.OpenFileDialog();
            if (dialog.ShowDialog() == true)
            {
                EncryptInputFile = dialog.FileName;
            }
            else
            {
                EncryptInputFile = "";
            }
        }

        private void SetFileToDecrypt()
        {
            Microsoft.Win32.OpenFileDialog dialog = new Microsoft.Win32.OpenFileDialog();
            dialog.Filter = "encrypted files (*.encrypted)|*.encrypted";
            if (dialog.ShowDialog() == true)
            {
                DecryptInputFile = dialog.FileName;
            }
            else
            {
                DecryptInputFile = "";
            }
        }

        private async void Encrypt()
        {
            if (!ValidateInputFilePathAndPassword(EncryptInputFile)) return;

            _progress = 0;
            _token = new EasyCrypto.ReportAndCancellationToken();
            SetProgressHandler();
            OperationInProgress = true;
            try
            {
                await EasyCrypto.AesFileEncrytion.EncryptWithPasswordAsync(
                    EncryptInputFile, EncryptOutputFile, Password, true, _token);
                if (_token.IsCanceled)
                {
                    MessageBox.Show("Canceled");
                }
                else
                {
                    MessageBox.Show("Completed");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error occurred: " + ex);
                ResetProgress();
            }
            finally
            {
                ResetProgress();
            }
        }
    
        private async void Decrypt()
        {
            if (!ValidateInputFilePathAndPassword(DecryptInputFile)) return;

            EasyCrypto.ValidationResult validationResult;
            using (Stream s = new FileStream(DecryptInputFile, FileMode.Open))
            {
                validationResult = EasyCrypto.AesEncryption.ValidateEncryptedDataWithPassword(s, Password);
            }
            if (!validationResult.IsValid)
            {
                if (!validationResult.KeyIsValid)
                {
                    MessageBox.Show("Password is not valid.");
                }
                else
                {
                    MessageBox.Show(validationResult.ErrorMessage);
                }
                return;
            }

            _progress = 0;
            _token = new EasyCrypto.ReportAndCancellationToken();
            SetProgressHandler();
            OperationInProgress = true;
            try
            {
                await EasyCrypto.AesFileEncrytion.DecryptWithPasswordAsync(
                    DecryptInputFile, DecryptOutputFile, Password, true, _token);
                if (_token.IsCanceled)
                {
                    MessageBox.Show("Canceled");
                }
                else
                {
                    MessageBox.Show("Completed");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error occurred: " + ex);
                ResetProgress();
            }
            finally
            {
                ResetProgress();
            }
        }

        private void ResetProgress()
        {
            if (_token != null)
            {
                _token.Cancel();
                Progress = 0;
            }
            OperationInProgress = false;
        }

        private bool ValidateInputFilePathAndPassword(string path)
        {
            string message = "";
            if (string.IsNullOrEmpty(path))
            {
                message = "Input file is not set. ";
            }
            else if (!File.Exists(path))
            {
                message = "Input file does not exists anymore. ";
            }
            if (string.IsNullOrEmpty(Password))
            {
                message += "Password is not set.";
            }
            if (!string.IsNullOrWhiteSpace(message))
            {
                MessageBox.Show(message.Trim());
            }
            return string.IsNullOrWhiteSpace(message);
        }

        private void SetProgressHandler()
        {
            if (_token != null)
            {
                _token.IntensityOfProgressReporting = EasyCrypto.ProgressReportIntensity.Intensive;
                _token.ReportProgress = progress =>
                {
                    Progress = (int)(progress * 100);
                };
            }
        }
    }
}
