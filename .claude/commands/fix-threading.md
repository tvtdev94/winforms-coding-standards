---
description: Fix cross-thread UI access issues in WinForms
---

You are tasked with fixing cross-thread UI access issues in WinForms code.

## Workflow

1. **Ask the user**:
   - Which file has threading issues?
   - What operations are running on background threads?

2. **Read the code** to identify threading problems:
   - UI updates from background threads
   - Direct control access without InvokeRequired check
   - Missing synchronization

3. **Common Threading Issues**:

### Issue 1: Updating UI from Background Thread

❌ **Wrong - Will throw exception**:
```csharp
private void StartBackgroundWork()
{
    Task.Run(() =>
    {
        // This will throw InvalidOperationException
        lblStatus.Text = "Processing...";
        progressBar.Value = 50;
    });
}
```

✅ **Correct - Using Invoke**:
```csharp
private void StartBackgroundWork()
{
    Task.Run(() =>
    {
        // Safe UI update
        UpdateStatus("Processing...");
        UpdateProgress(50);
    });
}

private void UpdateStatus(string message)
{
    if (lblStatus.InvokeRequired)
    {
        lblStatus.Invoke(new Action(() => UpdateStatus(message)));
        return;
    }
    lblStatus.Text = message;
}

private void UpdateProgress(int value)
{
    if (progressBar.InvokeRequired)
    {
        progressBar.Invoke(new Action(() => UpdateProgress(value)));
        return;
    }
    progressBar.Value = value;
}
```

### Issue 2: Using BackgroundWorker (Legacy but still common)

❌ **Wrong - Direct UI access**:
```csharp
private void btnProcess_Click(object sender, EventArgs e)
{
    var worker = new BackgroundWorker();
    worker.DoWork += (s, args) =>
    {
        for (int i = 0; i <= 100; i++)
        {
            Thread.Sleep(50);
            progressBar.Value = i; // Wrong!
            lblStatus.Text = $"Progress: {i}%"; // Wrong!
        }
    };
    worker.RunWorkerAsync();
}
```

✅ **Correct - Using ReportProgress**:
```csharp
private void btnProcess_Click(object sender, EventArgs e)
{
    var worker = new BackgroundWorker
    {
        WorkerReportsProgress = true
    };

    worker.DoWork += Worker_DoWork;
    worker.ProgressChanged += Worker_ProgressChanged;
    worker.RunWorkerCompleted += Worker_RunWorkerCompleted;

    btnProcess.Enabled = false;
    worker.RunWorkerAsync();
}

private void Worker_DoWork(object sender, DoWorkEventArgs e)
{
    var worker = (BackgroundWorker)sender;
    for (int i = 0; i <= 100; i++)
    {
        Thread.Sleep(50);
        // Report progress to UI thread
        worker.ReportProgress(i, $"Processing item {i}");
    }
}

private void Worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
{
    // This runs on UI thread - safe to update UI
    progressBar.Value = e.ProgressPercentage;
    lblStatus.Text = e.UserState?.ToString() ?? "";
}

private void Worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
{
    // This runs on UI thread
    btnProcess.Enabled = true;
    if (e.Error != null)
    {
        MessageBox.Show($"Error: {e.Error.Message}", "Error",
            MessageBoxButtons.OK, MessageBoxIcon.Error);
    }
    else
    {
        MessageBox.Show("Processing completed!", "Success",
            MessageBoxButtons.OK, MessageBoxIcon.Information);
    }
}
```

### Issue 3: Modern Async/Await Pattern (Recommended)

✅ **Best - Using async/await**:
```csharp
private async void btnProcess_Click(object sender, EventArgs e)
{
    try
    {
        btnProcess.Enabled = false;
        progressBar.Value = 0;
        lblStatus.Text = "Starting...";

        // Create progress reporter
        var progress = new Progress<int>(value =>
        {
            // This callback runs on UI thread automatically
            progressBar.Value = value;
            lblStatus.Text = $"Processing: {value}%";
        });

        // Run on background thread
        await ProcessDataAsync(progress);

        // Back on UI thread after await
        lblStatus.Text = "Completed!";
        MessageBox.Show("Processing completed!", "Success",
            MessageBoxButtons.OK, MessageBoxIcon.Information);
    }
    catch (Exception ex)
    {
        MessageBox.Show($"Error: {ex.Message}", "Error",
            MessageBoxButtons.OK, MessageBoxIcon.Error);
    }
    finally
    {
        btnProcess.Enabled = true;
    }
}

private async Task ProcessDataAsync(IProgress<int> progress)
{
    await Task.Run(() =>
    {
        for (int i = 0; i <= 100; i++)
        {
            Thread.Sleep(50);
            // Report progress (will be marshaled to UI thread)
            progress?.Report(i);
        }
    });
}
```

### Issue 4: Complex UI Updates

```csharp
private async void btnLoadData_Click(object sender, EventArgs e)
{
    try
    {
        SetLoadingState(true);

        // Load data on background thread
        var data = await _service.LoadDataAsync();

        // Back on UI thread - safe to update UI
        UpdateUI(data);
    }
    catch (Exception ex)
    {
        ShowError(ex.Message);
    }
    finally
    {
        SetLoadingState(false);
    }
}

private void SetLoadingState(bool isLoading)
{
    // All UI updates here run on UI thread
    btnLoadData.Enabled = !isLoading;
    progressBar.Visible = isLoading;
    Cursor = isLoading ? Cursors.WaitCursor : Cursors.Default;
}

private void UpdateUI(List<Customer> data)
{
    // Safe to update UI - we're on UI thread after await
    dgvCustomers.DataSource = new BindingList<Customer>(data);
    lblRecordCount.Text = $"Records: {data.Count}";
}
```

### Issue 5: Thread-Safe Helper Method

```csharp
/// <summary>
/// Executes an action on the UI thread. Can be called from any thread.
/// </summary>
private void InvokeOnUIThread(Action action)
{
    if (InvokeRequired)
    {
        Invoke(action);
    }
    else
    {
        action();
    }
}

// Usage
private void SomeBackgroundMethod()
{
    // ... background work ...

    InvokeOnUIThread(() =>
    {
        lblStatus.Text = "Updated from background";
        progressBar.Value = 75;
    });
}
```

4. **Analysis Checklist**:
   - [ ] Identify all background operations (Task.Run, Thread, Timer, etc.)
   - [ ] Check for direct UI control access in background code
   - [ ] Verify InvokeRequired checks before UI updates
   - [ ] Check for proper async/await usage
   - [ ] Ensure Progress<T> used for progress reporting
   - [ ] Verify thread-safe operations

5. **Recommended Patterns**:
   - ✅ **Prefer**: async/await with Progress<T>
   - ✅ **Alternative**: Invoke/BeginInvoke pattern
   - ⚠️ **Legacy**: BackgroundWorker (only for old codebases)
   - ❌ **Never**: Direct UI access from background threads

6. **Show the user**:
   - Fixed code with proper thread synchronization
   - Explanation of changes made
   - Pattern used (async/await, Invoke, BackgroundWorker)
   - Warnings about remaining threading issues (if any)
