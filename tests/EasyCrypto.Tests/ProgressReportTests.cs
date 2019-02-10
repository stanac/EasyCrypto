using System;
using System.Diagnostics;
using System.Reflection;
using Xunit;

namespace EasyCrypto.Tests
{
    public class ProgressReportTests
    {
        private MethodInfo shouldReportMethod = typeof(ReportAndCancellationToken).GetMethod("ShouldReportProgress", BindingFlags.Instance | BindingFlags.NonPublic);

        private ReportAndCancellationToken GetToken(int iterationsCount)
        {
            var token = new ReportAndCancellationToken();
            var prop = token.GetType().GetProperty("NumberOfIterations", BindingFlags.Instance | BindingFlags.NonPublic);
            prop.SetValue(token, iterationsCount, null);
            return token;
        }

        private bool ShouldReport(ReportAndCancellationToken token, int taken)
        {
            return (bool)shouldReportMethod.Invoke(token, new object[] { taken });
        }

        [Fact]
        public void ProgressShouldBeReportedIfActionCountIsLessThanOrEqualTo100()
        {
            var token = GetToken(100);
            Assert.True(ShouldReport(token, 1));
            Assert.True(ShouldReport(token, 17));
            Assert.True(ShouldReport(token, 33));
            Assert.True(ShouldReport(token, 44));
            Assert.True(ShouldReport(token, 55));
        }

        [Fact]
        public void ProgressShouldBeReportedIfActionCountIsEqualToActionsTaken()
        {
            int c = 99999;
            var token = GetToken(c);
            Assert.True(ShouldReport(token, c));
        }

        [Fact]
        public void ProgressShouldBeReportedAbout100Times()
        {
            Action<int> assertReport = (count) =>
            {
                int report = 0;
                var token = GetToken(count);
                for (int i = 0; i < count; i++)
                {
                    if (ShouldReport(token, i))
                    {
                        report++;
                    }
                }
                bool reportCountInRange = report > 95 && report < 105;
                if (!reportCountInRange && count < 100 && (report > count - 5 && report < count + 5))
                {
                    reportCountInRange = true;
                }
                if (!reportCountInRange)
                {
                    Debugger.Break();
                }
                Assert.True(reportCountInRange);
            };

            for (int i = 1; i < 999; i++)
            {
                assertReport(i);
            }

            for (int i = 12950; i <= 12999; i++)
            {
                assertReport(i);
            }
            
            assertReport(3412950);
        }
    }
}
