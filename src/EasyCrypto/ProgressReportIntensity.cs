using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyCrypto
{
    /// <summary>
    /// Used to decide how often to report progress
    /// </summary>
    public enum ProgressReportIntensity
    {
        /// <summary>
        /// Recommended value, report about 100 times or less
        /// </summary>
        Optimized,

        /// <summary>
        /// Reports progress as soon as possible, after each iteration
        /// </summary>
        Intensive
    }
}
