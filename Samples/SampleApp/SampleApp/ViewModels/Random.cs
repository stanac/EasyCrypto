using System;
using System.Windows.Input;

namespace SampleApp.ViewModels
{
    public class Random : ObservableObject
    {
        private readonly EasyCrypto.CryptoRandom _random = new EasyCrypto.CryptoRandom();

        private string _result;

        public string Result
        {
            get { return _result; }
            set
            {
                if (value != _result)
                {
                    _result = value;
                    RaisePropertyChanged();
                }
            }
        }

        public ICommand GenerateIntegerCommand => new Command(GenerateInteger);
        public ICommand GenerateDoubleCommand => new Command(GenerateDouble);
        public ICommand GenerateHexCommand => new Command(GenerateHex);

        private void GenerateInteger()
        {
            Result = _random.NextInt().ToString();
        }

        private void GenerateDouble()
        {
            Result = _random.NextDouble().ToString();
        }

        private void GenerateHex()
        {
            byte[] data = _random.NextBytes(32);
            Result = BitConverter.ToString(data).Replace("-", "");
        }
    }
}
