using BenchmarkDotNet.Attributes;

namespace EasyCrypto.Benchmarks
{
    [MemoryDiagnoser]
    public class PasswordHasherAndValidatorBenchmarks
    {
        private readonly PasswordHasherAndValidator _hasherNew = new PasswordHasherAndValidator(28_000);
        private readonly PasswordHasherAndValidator _hasherNew56k = new PasswordHasherAndValidator(56_000);
        private readonly PasswordHasher _hasherOld = new PasswordHasher();
        private readonly string _password = TokenGenerator.Default.GenerateToken(64);

        [Benchmark]
        public void UsingOldHasher()
        {
            _hasherOld.HashPasswordAndGenerateEmbeddedSalt(_password);
        }

        [Benchmark]
        public void UsingNewHasher()
        {
            _hasherNew.HashPasswordToString(_password);
        }


        [Benchmark]
        public void UsingNewHasher56KIterations()
        {
            _hasherNew56k.HashPasswordToString(_password);
        }
    }
}
