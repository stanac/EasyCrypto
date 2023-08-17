namespace EasyCrypto;

/// <summary>
/// Token for reporting progress of action and offering a way to cancel the operation. Warning, you should never reuse instance of this class!
/// </summary>
public class ReportAndCancellationToken
{
    private bool _canceled;
        
    internal int NumberOfIterations { get; set; }
    private int _lastReportedInt = -1;

    /// <summary>
    /// Gets a value indicating whether operation can report progress.
    /// </summary>
    /// <value>
    /// <c>true</c> if operation can report progress; otherwise, <c>false</c>.
    /// </value>
    public bool? CanReportProgress { get; internal set; }

    /// <summary>
    /// Gets a value indicating whether operation is canceled.
    /// </summary>
    /// <value>
    /// <c>true</c> if operation is canceled; otherwise, <c>false</c>.
    /// </value>
    public bool IsCanceled => _canceled;

    /// <summary>
    /// Cancels the operation.
    /// </summary>
    public void Cancel() => _canceled = true;

    /// <summary>
    /// If set reports progress with value of type double where value can be between 0.0 and 1.0
    /// </summary>
    public Action<double> ReportProgress { get; set; }
        
    /// <summary>
    /// Gets or sets the intensity of progress reporting.
    /// </summary>
    /// <value>
    /// The intensity of reporting.
    /// </value>
    public ProgressReportIntensity IntensityOfProgressReporting { get; set; } = ProgressReportIntensity.Optimized;

    internal void ReportProgressInternal(int iterationsTaken)
    {
        if (!(CanReportProgress ?? false)) return;
        if (ReportProgress == null) return;
        if (!ShouldReportProgress(iterationsTaken)) return;

        ReportProgress((double)iterationsTaken / NumberOfIterations);
    }
        
    internal bool ShouldReportProgress(int actionsTaken)
    {
        if (IntensityOfProgressReporting == ProgressReportIntensity.Intensive || NumberOfIterations <= 100)
        {
            return true;
        }
        int val = (int)Math.Round((actionsTaken * 100.0) / NumberOfIterations);
        if (val > _lastReportedInt)
        {
            _lastReportedInt = val;
            return true;
        }
        return false;
    }
        
}