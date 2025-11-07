# Thread Safety in WinForms

## 📋 Overview

Thread safety is critical in WinForms applications because the UI framework uses a **single-threaded apartment (STA) model**. All UI controls must be accessed from the thread that created them (the UI thread). Attempting to modify UI controls from background threads results in **cross-thread exceptions** and unpredictable behavior.

This guide covers patterns and best practices for safely updating the UI from background threads, handling long-running operations without blocking the UI, and avoiding common threading pitfalls.

**Key Concepts**:
- WinForms controls are not thread-safe
- Only the UI thread can safely update UI controls
- Background threads must marshal calls to the UI thread
- Modern async/await patterns simplify thread safety

---

## 🎯 Why This Matters

### Threading Problems in WinForms

**Cross-Thread Exceptions**:
```csharp
// ❌ This will throw InvalidOperationException
private void btnLoad_Click(object sender, EventArgs e)
{
    Task.Run(() =>
    {
        var data = LoadData();
        // CRASH! Cross-thread operation not valid
        txtResult.Text = data;
    });
}
```

**Race Conditions**:
- Multiple threads accessing shared data
- Unpredictable results due to timing
- Intermittent bugs that are hard to reproduce

**Deadlocks**:
- Thread A waits for Thread B
- Thread B waits for Thread A
- Application freezes completely

**UI Freezing**:
- Long operations on UI thread block user interaction
- Application appears "Not Responding"
- Poor user experience

---

## The UI Thread

### Single-Threaded UI Model

**Why WinForms is Single-Threaded**:

Windows Forms wraps Win32 controls, which are based on a message pump architecture. Each control is associated with a specific thread (its creating thread). Windows sends messages to controls through this thread's message queue.

```csharp
// The UI thread runs a message loop
[STAThread]
static void Main()
{
    Application.EnableVisualStyles();
    Application.SetCompatibleTextRenderingDefault(false);
    Application.Run(new MainForm()); // Message pump starts here
}
```

**UI Thread Restrictions**:

1. **Only the creating thread can access controls**
2. **Long operations block the message pump** (UI freezes)
3. **Events fire on the UI thread** (button clicks, timers, etc.)
4. **Painting requires UI thread** (OnPaint events)

**Cross-Thread Exception Example**:

```csharp
public partial class MainForm : Form
{
    private void btnBadExample_Click(object sender, EventArgs e)
    {
        // Start background thread
        new Thread(() =>
        {
            Thread.Sleep(2000); // Simulate work

            // ❌ InvalidOperationException: Cross-thread operation not valid
            lblStatus.Text = "Done!";
        }).Start();
    }
}
```

The exception message:
```
InvalidOperationException: Cross-thread operation not valid:
Control 'lblStatus' accessed from a thread other than the thread it was created on.
```

### InvokeRequired Pattern

**Checking InvokeRequired**:

Every WinForms control has an `InvokeRequired` property that returns `true` when called from a non-UI thread.

```csharp
private void UpdateLabel(string text)
{
    if (lblStatus.InvokeRequired)
    {
        // We're on wrong thread - marshal to UI thread
        lblStatus.Invoke(new Action(() => UpdateLabel(text)));
    }
    else
    {
        // We're on UI thread - safe to update
        lblStatus.Text = text;
    }
}
```

**Control.Invoke Method**:

`Invoke` executes a delegate on the UI thread **synchronously** (blocking until complete).

```csharp
private void btnInvoke_Click(object sender, EventArgs e)
{
    Task.Run(() =>
    {
        var data = DownloadData(); // Background work

        // Marshal to UI thread and WAIT for completion
        this.Invoke((MethodInvoker)(() =>
        {
            txtResult.Text = data;
            lblStatus.Text = "Complete!";
        }));
    });
}
```

**Control.BeginInvoke Method**:

`BeginInvoke` executes a delegate on the UI thread **asynchronously** (non-blocking).

```csharp
private void btnBeginInvoke_Click(object sender, EventArgs e)
{
    Task.Run(() =>
    {
        var data = DownloadData(); // Background work

        // Marshal to UI thread and continue immediately
        this.BeginInvoke((MethodInvoker)(() =>
        {
            txtResult.Text = data;
            lblStatus.Text = "Complete!";
        }));
    });
}
```

**Invoke vs BeginInvoke Comparison**:

| Feature | Invoke | BeginInvoke |
|---------|--------|-------------|
| Execution | Synchronous | Asynchronous |
| Blocking | Blocks calling thread | Non-blocking |
| Return Value | Can return values | Returns IAsyncResult |
| Use When | Need result immediately | Fire and forget |
| Deadlock Risk | Higher (if UI waits for thread) | Lower |

```csharp
// Invoke - blocks until UI update completes
Task.Run(() =>
{
    var result = (string)this.Invoke(new Func<string>(() =>
    {
        return txtInput.Text; // Get value from UI
    }));
    ProcessData(result);
});

// BeginInvoke - continues immediately
Task.Run(() =>
{
    this.BeginInvoke((MethodInvoker)(() =>
    {
        lblStatus.Text = "Processing..."; // Update UI
    }));
    // Continues immediately without waiting
    DoMoreWork();
});
```

---

## Safe UI Updates

### Pattern 1: Invoke/BeginInvoke

**Basic Pattern**:

```csharp
private void UpdateUI(string message)
{
    if (InvokeRequired)
    {
        BeginInvoke(new Action<string>(UpdateUI), message);
        return;
    }

    lblStatus.Text = message;
    progressBar.Value = 100;
}

private void btnStart_Click(object sender, EventArgs e)
{
    Task.Run(() =>
    {
        // Do work on background thread
        var result = ProcessData();

        // Safely update UI
        UpdateUI($"Result: {result}");
    });
}
```

**Recursive Invoke Helper**:

```csharp
// Reusable helper method
private void SafeInvoke(Action action)
{
    if (InvokeRequired)
    {
        BeginInvoke(action);
    }
    else
    {
        action();
    }
}

// Usage
Task.Run(() =>
{
    var data = LoadData();
    SafeInvoke(() =>
    {
        txtResult.Text = data;
        EnableControls();
    });
});
```

**Generic Invoke Methods**:

```csharp
// Generic helper for any control
public static class ControlExtensions
{
    public static void SafeInvoke(this Control control, Action action)
    {
        if (control.InvokeRequired)
        {
            control.BeginInvoke(action);
        }
        else
        {
            action();
        }
    }

    public static T SafeInvoke<T>(this Control control, Func<T> func)
    {
        if (control.InvokeRequired)
        {
            return (T)control.Invoke(func);
        }
        else
        {
            return func();
        }
    }
}

// Usage
Task.Run(() =>
{
    var data = DownloadData();

    // Update any control safely
    txtResult.SafeInvoke(() => txtResult.Text = data);

    // Get value from UI
    var inputValue = txtInput.SafeInvoke(() => txtInput.Text);
});
```

### Pattern 2: SynchronizationContext

**Using SynchronizationContext.Current**:

```csharp
public partial class MainForm : Form
{
    private readonly SynchronizationContext _uiContext;

    public MainForm()
    {
        InitializeComponent();
        // Capture UI context during construction
        _uiContext = SynchronizationContext.Current;
    }

    private void btnLoad_Click(object sender, EventArgs e)
    {
        Task.Run(() =>
        {
            var data = LoadData();

            // Post to UI thread (async)
            _uiContext.Post(_ =>
            {
                txtResult.Text = data;
            }, null);
        });
    }
}
```

**Post and Send Methods**:

```csharp
// Post - asynchronous (like BeginInvoke)
_uiContext.Post(state =>
{
    lblStatus.Text = "Done!";
}, null);

// Send - synchronous (like Invoke)
_uiContext.Send(state =>
{
    lblStatus.Text = "Done!";
}, null);
```

**When to Use**:
- Sharing UI context with services/libraries
- When control reference isn't available
- Building framework/library code

### Pattern 3: Progress\<T> (RECOMMENDED)

**Modern Async/Await Approach**:

```csharp
public partial class MainForm : Form
{
    private async void btnProcess_Click(object sender, EventArgs e)
    {
        btnProcess.Enabled = false;
        progressBar.Value = 0;

        // Progress automatically marshals to UI thread
        var progress = new Progress<int>(value =>
        {
            progressBar.Value = value;
            lblStatus.Text = $"Processing: {value}%";
        });

        try
        {
            var result = await ProcessDataAsync(progress);
            MessageBox.Show($"Result: {result}");
        }
        finally
        {
            btnProcess.Enabled = true;
        }
    }

    private async Task<string> ProcessDataAsync(IProgress<int> progress)
    {
        return await Task.Run(() =>
        {
            for (int i = 0; i <= 100; i += 10)
            {
                Thread.Sleep(200); // Simulate work
                progress?.Report(i); // Automatically marshaled to UI
            }
            return "Success!";
        });
    }
}
```

**IProgress\<T> Interface**:

```csharp
// Custom progress type
public class DownloadProgress
{
    public long BytesReceived { get; set; }
    public long TotalBytes { get; set; }
    public int Percentage => (int)((BytesReceived * 100) / TotalBytes);
    public string Status { get; set; }
}

private async void btnDownload_Click(object sender, EventArgs e)
{
    var progress = new Progress<DownloadProgress>(p =>
    {
        progressBar.Value = p.Percentage;
        lblStatus.Text = p.Status;
        lblBytes.Text = $"{p.BytesReceived:N0} / {p.TotalBytes:N0} bytes";
    });

    await DownloadFileAsync("https://example.com/file.zip", progress);
}
```

**Why Progress\<T> is Best**:
- ✅ Thread-safe by design
- ✅ Automatic UI thread marshaling
- ✅ Works with async/await
- ✅ Type-safe progress reporting
- ✅ No InvokeRequired checks needed

### Pattern 4: BackgroundWorker

**Legacy Pattern (Still Used)**:

```csharp
public partial class MainForm : Form
{
    private BackgroundWorker _worker;

    public MainForm()
    {
        InitializeComponent();
        InitializeBackgroundWorker();
    }

    private void InitializeBackgroundWorker()
    {
        _worker = new BackgroundWorker
        {
            WorkerReportsProgress = true,
            WorkerSupportsCancellation = true
        };

        _worker.DoWork += Worker_DoWork;
        _worker.ProgressChanged += Worker_ProgressChanged;
        _worker.RunWorkerCompleted += Worker_RunWorkerCompleted;
    }

    private void btnStart_Click(object sender, EventArgs e)
    {
        if (!_worker.IsBusy)
        {
            btnStart.Enabled = false;
            btnCancel.Enabled = true;
            _worker.RunWorkerAsync();
        }
    }

    private void btnCancel_Click(object sender, EventArgs e)
    {
        if (_worker.IsBusy)
        {
            _worker.CancelAsync();
        }
    }

    private void Worker_DoWork(object sender, DoWorkEventArgs e)
    {
        var worker = (BackgroundWorker)sender;

        for (int i = 1; i <= 100; i++)
        {
            if (worker.CancellationPending)
            {
                e.Cancel = true;
                break;
            }

            Thread.Sleep(50); // Simulate work
            worker.ReportProgress(i);
        }

        e.Result = "Processing complete!";
    }

    private void Worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
    {
        // Automatically on UI thread - safe to update controls
        progressBar.Value = e.ProgressPercentage;
        lblStatus.Text = $"Progress: {e.ProgressPercentage}%";
    }

    private void Worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
    {
        // Automatically on UI thread
        btnStart.Enabled = true;
        btnCancel.Enabled = false;

        if (e.Cancelled)
        {
            lblStatus.Text = "Cancelled";
        }
        else if (e.Error != null)
        {
            MessageBox.Show($"Error: {e.Error.Message}");
        }
        else
        {
            lblStatus.Text = e.Result.ToString();
        }
    }
}
```

---

## Async/Await with UI

### async void Event Handlers

**Proper Async Button Click Handlers**:

```csharp
private async void btnLoad_Click(object sender, EventArgs e)
{
    btnLoad.Enabled = false;
    lblStatus.Text = "Loading...";

    try
    {
        // Async work automatically returns to UI thread
        var data = await LoadDataAsync();

        // Safe to update UI here
        txtResult.Text = data;
        lblStatus.Text = "Complete!";
    }
    catch (Exception ex)
    {
        MessageBox.Show($"Error: {ex.Message}", "Error",
            MessageBoxButtons.OK, MessageBoxIcon.Error);
        lblStatus.Text = "Error occurred";
    }
    finally
    {
        btnLoad.Enabled = true;
    }
}

private async Task<string> LoadDataAsync()
{
    // Simulate async operation
    await Task.Delay(2000);
    return "Sample data loaded";
}
```

**Exception Handling**:

```csharp
// ✅ GOOD - Exceptions are caught
private async void btnProcess_Click(object sender, EventArgs e)
{
    try
    {
        await ProcessAsync();
    }
    catch (Exception ex)
    {
        // Handle or log exception
        ShowError(ex);
    }
}

// ❌ BAD - Unhandled exceptions crash app
private async void btnProcess_Click(object sender, EventArgs e)
{
    await ProcessAsync(); // Exception will crash application
}
```

**Cancellation**:

```csharp
private CancellationTokenSource _cts;

private async void btnStart_Click(object sender, EventArgs e)
{
    btnStart.Enabled = false;
    btnCancel.Enabled = true;

    _cts = new CancellationTokenSource();

    try
    {
        await ProcessWithCancellationAsync(_cts.Token);
        lblStatus.Text = "Completed!";
    }
    catch (OperationCanceledException)
    {
        lblStatus.Text = "Cancelled";
    }
    catch (Exception ex)
    {
        MessageBox.Show(ex.Message);
    }
    finally
    {
        btnStart.Enabled = true;
        btnCancel.Enabled = false;
        _cts?.Dispose();
    }
}

private void btnCancel_Click(object sender, EventArgs e)
{
    _cts?.Cancel();
}

private async Task ProcessWithCancellationAsync(CancellationToken token)
{
    for (int i = 0; i < 100; i++)
    {
        token.ThrowIfCancellationRequested();
        await Task.Delay(100, token);
        // Update progress...
    }
}
```

### ConfigureAwait

**ConfigureAwait(false) in Libraries**:

```csharp
// In library/service code - don't need UI thread
public async Task<string> DownloadDataAsync()
{
    using var client = new HttpClient();

    // Don't capture UI context - more efficient
    var response = await client.GetStringAsync("https://api.example.com/data")
        .ConfigureAwait(false);

    // Still on background thread here
    return ProcessResponse(response);
}
```

**ConfigureAwait(true) for UI Updates**:

```csharp
// In UI code - need UI thread
private async void btnLoad_Click(object sender, EventArgs e)
{
    var data = await LoadDataAsync().ConfigureAwait(true); // Default

    // Guaranteed to be on UI thread - safe to update controls
    txtResult.Text = data;
}
```

**When to Use Each**:

```csharp
// ✅ Library/Service code
public async Task<Data> GetDataAsync()
{
    var result = await database.QueryAsync().ConfigureAwait(false);
    return result;
}

// ✅ UI event handler
private async void btnSave_Click(object sender, EventArgs e)
{
    await SaveDataAsync(); // ConfigureAwait(true) implicit
    lblStatus.Text = "Saved!"; // Need UI thread
}
```

### Task.Run for Background Work

**Moving Work Off UI Thread**:

```csharp
private async void btnProcess_Click(object sender, EventArgs e)
{
    btnProcess.Enabled = false;

    // Move CPU-intensive work to thread pool
    var result = await Task.Run(() =>
    {
        // Expensive computation on background thread
        return PerformComplexCalculation();
    });

    // Back on UI thread - safe to update
    txtResult.Text = result.ToString();
    btnProcess.Enabled = true;
}

private double PerformComplexCalculation()
{
    // CPU-intensive work
    double sum = 0;
    for (int i = 0; i < 10_000_000; i++)
    {
        sum += Math.Sqrt(i);
    }
    return sum;
}
```

**Returning Results to UI**:

```csharp
private async void btnLoadImage_Click(object sender, EventArgs e)
{
    var filePath = txtFilePath.Text;

    // Load and process image on background thread
    var bitmap = await Task.Run(() =>
    {
        using var original = new Bitmap(filePath);
        return ResizeImage(original, 800, 600);
    });

    // Update UI on UI thread
    pictureBox.Image = bitmap;
}
```

---

## Thread Synchronization

### lock Statement

**Protecting Shared State**:

```csharp
public class DataCache
{
    private readonly Dictionary<string, string> _cache = new();
    private readonly object _lock = new object();

    public void Set(string key, string value)
    {
        lock (_lock)
        {
            _cache[key] = value;
        }
    }

    public string Get(string key)
    {
        lock (_lock)
        {
            return _cache.TryGetValue(key, out var value) ? value : null;
        }
    }
}
```

**Lock Objects**:

```csharp
// ✅ GOOD - private lock object
private readonly object _lock = new object();

// ❌ BAD - locking on this (external code can lock)
lock (this) { }

// ❌ BAD - locking on string (interned strings are shared)
lock ("myLock") { }

// ❌ BAD - locking on type (global lock)
lock (typeof(MyClass)) { }
```

**Common Mistakes**:

```csharp
// ❌ BAD - Lock scope too large (hurts performance)
private readonly object _lock = new object();
public void ProcessData()
{
    lock (_lock)
    {
        var data = LoadData(); // Slow I/O inside lock!
        ProcessItems(data);
    }
}

// ✅ GOOD - Minimize lock scope
public void ProcessData()
{
    var data = LoadData(); // I/O outside lock

    lock (_lock)
    {
        ProcessItems(data); // Only critical section locked
    }
}
```

### Thread-Safe Collections

**Using Concurrent Collections**:

```csharp
using System.Collections.Concurrent;

public class MessageQueue
{
    private readonly ConcurrentQueue<string> _messages = new();

    public void Enqueue(string message)
    {
        _messages.Enqueue(message); // Thread-safe
    }

    public bool TryDequeue(out string message)
    {
        return _messages.TryDequeue(out message); // Thread-safe
    }
}

// ConcurrentDictionary
private readonly ConcurrentDictionary<int, Customer> _customers = new();

public void AddOrUpdate(Customer customer)
{
    _customers.AddOrUpdate(customer.Id, customer, (key, old) => customer);
}
```

### Avoiding Deadlocks

**Lock Ordering**:

```csharp
// ✅ GOOD - Consistent lock order
private readonly object _lock1 = new();
private readonly object _lock2 = new();

public void Method1()
{
    lock (_lock1)
    {
        lock (_lock2)
        {
            // Work
        }
    }
}

public void Method2()
{
    lock (_lock1) // Same order
    {
        lock (_lock2)
        {
            // Work
        }
    }
}
```

**Timeout Patterns**:

```csharp
// Use Monitor.TryEnter with timeout
if (Monitor.TryEnter(_lock, TimeSpan.FromSeconds(5)))
{
    try
    {
        // Critical section
    }
    finally
    {
        Monitor.Exit(_lock);
    }
}
else
{
    // Timeout - handle deadlock scenario
    throw new TimeoutException("Could not acquire lock");
}
```

---

## Best Practices

### ✅ DOs

1. **Use async/await for I/O Operations**
```csharp
// ✅ Keeps UI responsive
private async void btnLoad_Click(object sender, EventArgs e)
{
    var data = await LoadDataAsync();
    txtResult.Text = data;
}
```

2. **Use Progress\<T> for Progress Reporting**
```csharp
var progress = new Progress<int>(value => progressBar.Value = value);
await ProcessAsync(progress);
```

3. **Disable Controls During Operations**
```csharp
btnStart.Enabled = false;
try
{
    await ProcessAsync();
}
finally
{
    btnStart.Enabled = true;
}
```

4. **Use CancellationToken**
```csharp
private async Task ProcessAsync(CancellationToken token)
{
    token.ThrowIfCancellationRequested();
    await Task.Delay(1000, token);
}
```

5. **Use Task.Run for CPU-Intensive Work**
```csharp
var result = await Task.Run(() => ExpensiveCalculation());
```

6. **Always Handle Exceptions in async void**
```csharp
private async void btnClick(object sender, EventArgs e)
{
    try
    {
        await ProcessAsync();
    }
    catch (Exception ex)
    {
        ShowError(ex);
    }
}
```

7. **Use Thread-Safe Collections**
```csharp
private readonly ConcurrentQueue<string> _queue = new();
```

8. **Use Dedicated Lock Objects**
```csharp
private readonly object _lock = new object();
```

9. **Minimize Lock Scope**
```csharp
var data = PrepareData();
lock (_lock) { UpdateSharedState(data); }
```

10. **Use ConfigureAwait(false) in Libraries**
```csharp
public async Task<Data> GetDataAsync()
{
    return await LoadAsync().ConfigureAwait(false);
}
```

11. **Use Timer.Start/Stop on UI Thread**
```csharp
// System.Windows.Forms.Timer is thread-safe
_timer.Start(); // OK from any thread
```

12. **Dispose CancellationTokenSource**
```csharp
using var cts = new CancellationTokenSource();
await ProcessAsync(cts.Token);
```

### ❌ DON'Ts

1. **Don't Update UI from Background Threads**
```csharp
// ❌ BAD
Task.Run(() => lblStatus.Text = "Done");
```

2. **Don't Use Task.Wait() or .Result on UI Thread**
```csharp
// ❌ BAD - Deadlock risk
var result = SomeAsync().Result; // DEADLOCK!
```

3. **Don't Block UI Thread with Thread.Sleep**
```csharp
// ❌ BAD
Thread.Sleep(5000); // UI freezes
// ✅ GOOD
await Task.Delay(5000);
```

4. **Don't Ignore async void Exceptions**
```csharp
// ❌ BAD - crashes app
private async void btn_Click(object sender, EventArgs e)
{
    await ProcessAsync(); // Unhandled exception
}
```

5. **Don't Lock on this, string, or Type**
```csharp
// ❌ BAD
lock (this) { }
lock ("lock") { }
```

6. **Don't Perform I/O Inside Locks**
```csharp
// ❌ BAD - lock held during slow I/O
lock (_lock) { var data = File.ReadAllText(path); }
```

7. **Don't Create Threads for Short Operations**
```csharp
// ❌ BAD - expensive thread creation
new Thread(() => DoQuickWork()).Start();
// ✅ GOOD
await Task.Run(() => DoQuickWork());
```

8. **Don't Forget to Dispose**
```csharp
// ❌ BAD
var cts = new CancellationTokenSource();
// ✅ GOOD
using var cts = new CancellationTokenSource();
```

9. **Don't Mix Sync and Async Code**
```csharp
// ❌ BAD - "sync over async"
public void LoadData()
{
    var data = LoadDataAsync().Result;
}
```

10. **Don't Use System.Threading.Timer for UI Updates**
```csharp
// ❌ BAD - fires on thread pool thread
var timer = new System.Threading.Timer(_ =>
    lblTime.Text = DateTime.Now.ToString(), null, 0, 1000);
```

---

## Thread Safety Checklist

Quick reference for code review:

- [ ] UI updates only on UI thread (Invoke/await)
- [ ] Long operations are async or on background threads
- [ ] Progress\<T> or InvokeRequired pattern used
- [ ] CancellationToken supported for long operations
- [ ] Exceptions handled in async void event handlers
- [ ] Shared state protected with locks or concurrent collections
- [ ] Lock objects are private and readonly
- [ ] Lock scope minimized
- [ ] No Task.Wait() or .Result on UI thread
- [ ] No Thread.Sleep() on UI thread (use Task.Delay)
- [ ] Controls disabled during operations
- [ ] CancellationTokenSource disposed
- [ ] No I/O operations inside locks
- [ ] Consistent lock ordering (if multiple locks)

---

## Common Mistakes

**1. Deadlock from Blocking on Async**
```csharp
// ❌ PROBLEM
private void btnLoad_Click(object sender, EventArgs e)
{
    var data = LoadDataAsync().Result; // DEADLOCK!
}

// ✅ FIX
private async void btnLoad_Click(object sender, EventArgs e)
{
    var data = await LoadDataAsync();
}
```

**2. Unhandled async void Exception**
```csharp
// ❌ PROBLEM - crashes app
private async void btn_Click(object sender, EventArgs e)
{
    await RiskyOperation();
}

// ✅ FIX
private async void btn_Click(object sender, EventArgs e)
{
    try { await RiskyOperation(); }
    catch (Exception ex) { ShowError(ex); }
}
```

**3. Cross-Thread UI Update**
```csharp
// ❌ PROBLEM
Task.Run(() => lblStatus.Text = "Done");

// ✅ FIX
Task.Run(() => this.BeginInvoke(() => lblStatus.Text = "Done"));
```

**4. Blocking UI Thread**
```csharp
// ❌ PROBLEM
private void btnProcess_Click(object sender, EventArgs e)
{
    Thread.Sleep(5000); // UI frozen
}

// ✅ FIX
private async void btnProcess_Click(object sender, EventArgs e)
{
    await Task.Delay(5000);
}
```

**5. Wrong Timer Type**
```csharp
// ❌ PROBLEM - thread pool thread
var timer = new System.Threading.Timer(_ =>
    lblTime.Text = DateTime.Now.ToString(), null, 0, 1000);

// ✅ FIX - UI thread
var timer = new System.Windows.Forms.Timer { Interval = 1000 };
timer.Tick += (s, e) => lblTime.Text = DateTime.Now.ToString();
timer.Start();
```

**6. Locking on Public Object**
```csharp
// ❌ PROBLEM
lock (this) { _data++; }

// ✅ FIX
private readonly object _lock = new();
lock (_lock) { _data++; }
```

**7. Not Disposing CancellationTokenSource**
```csharp
// ❌ PROBLEM
var cts = new CancellationTokenSource();
await ProcessAsync(cts.Token);

// ✅ FIX
using var cts = new CancellationTokenSource();
await ProcessAsync(cts.Token);
```

**8. Race Condition on Shared State**
```csharp
// ❌ PROBLEM
private int _counter;
Task.Run(() => _counter++); // Race!
Task.Run(() => _counter++);

// ✅ FIX
private int _counter;
lock (_lock) { _counter++; }
```

---

## Related Topics

- [Async/Await Pattern](async-await.md) - Detailed async/await guidance
- [Performance Optimization](performance.md) - Threading and performance
- [Error Handling & Logging](error-handling.md) - Exception handling patterns
- [Resource Management](resource-management.md) - Disposing resources properly

---

**Last Updated**: 2025-11-07
