# Feedback & Status Communication

> Part of [Production UI Standards](../production-ui-standards.md)

---

## Toast Notifications

```csharp
public enum ToastType { Success, Error, Warning, Info }

public class ToastNotification : Form
{
    public ToastNotification(string message, ToastType type, int durationMs = 3000)
    {
        FormBorderStyle = FormBorderStyle.None;
        ShowInTaskbar = false;
        TopMost = true;
        Size = new Size(300, 60);
        StartPosition = FormStartPosition.Manual;

        // Position bottom-right
        var screen = Screen.PrimaryScreen.WorkingArea;
        Location = new Point(screen.Right - Width - 20, screen.Bottom - Height - 20);

        BackColor = type switch
        {
            ToastType.Success => AppColors.Success,
            ToastType.Error => AppColors.Danger,
            ToastType.Warning => AppColors.Warning,
            ToastType.Info => AppColors.Info,
            _ => AppColors.Info
        };

        var lbl = new Label
        {
            Text = message,
            ForeColor = type == ToastType.Warning ? AppColors.TextPrimary : Color.White,
            Dock = DockStyle.Fill,
            TextAlign = ContentAlignment.MiddleCenter
        };
        Controls.Add(lbl);

        // Auto-close
        var timer = new Timer { Interval = durationMs };
        timer.Tick += (s, e) => Close();
        timer.Start();
    }

    public static void Show(string message, ToastType type = ToastType.Info)
    {
        var toast = new ToastNotification(message, type);
        toast.Show();
    }
}

// Usage
ToastNotification.Show("Record saved!", ToastType.Success);
ToastNotification.Show("Connection failed", ToastType.Error);
```

---

## Status Bar Manager

```csharp
public class StatusBarManager
{
    private readonly ToolStripStatusLabel lblStatus;
    private readonly ToolStripProgressBar progressBar;
    private readonly ToolStripStatusLabel lblRecordCount;

    public StatusBarManager(StatusStrip strip)
    {
        lblStatus = new ToolStripStatusLabel { Text = "Ready", Spring = true };
        progressBar = new ToolStripProgressBar { Visible = false };
        lblRecordCount = new ToolStripStatusLabel { Text = "0 records" };

        strip.Items.AddRange(new ToolStripItem[] { lblStatus, progressBar, lblRecordCount });
    }

    public void SetStatus(string message) => lblStatus.Text = message;

    public void SetRecordCount(int count) => lblRecordCount.Text = $"{count:N0} records";

    public void SetBusy(string message)
    {
        lblStatus.Text = message;
        progressBar.Visible = true;
        progressBar.Style = ProgressBarStyle.Marquee;
    }

    public void SetReady()
    {
        lblStatus.Text = "Ready";
        progressBar.Visible = false;
    }
}
```

---

## Loading States

```csharp
// Show loading during async operations
public async Task LoadDataAsync()
{
    ShowLoadingOverlay();
    statusBar.SetBusy("Loading data...");
    dgvData.Enabled = false;

    try
    {
        var data = await _service.GetAllAsync();
        dgvData.DataSource = data;
        statusBar.SetRecordCount(data.Count);
    }
    catch (Exception ex)
    {
        ToastNotification.Show("Failed to load data", ToastType.Error);
    }
    finally
    {
        HideLoadingOverlay();
        statusBar.SetReady();
        dgvData.Enabled = true;
    }
}
```

---

## Progress Indicators

```csharp
// For operations with known progress
public async Task ExportDataAsync(IProgress<int> progress)
{
    progressBar.Visible = true;
    progressBar.Style = ProgressBarStyle.Blocks;
    progressBar.Maximum = 100;

    for (int i = 0; i <= 100; i += 10)
    {
        await Task.Delay(100); // Simulate work
        progressBar.Value = i;
        progress?.Report(i);
    }

    progressBar.Visible = false;
}

// For operations with unknown duration
progressBar.Style = ProgressBarStyle.Marquee; // Indeterminate
```

---

## Checklist

- [ ] Loading indicators for operations >200ms
- [ ] Success/error toast notifications
- [ ] Status bar updates during operations
- [ ] Progress bar for long operations
- [ ] Cursor changes (WaitCursor) during operations
