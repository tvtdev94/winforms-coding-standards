# Resource Management in WinForms

## 📋 Overview

Resource management is critical in WinForms applications to prevent memory leaks, ensure optimal performance, and maintain application stability. Proper handling of disposable resources through the `IDisposable` pattern and `using` statements is essential for building robust applications.

**Key Concepts**:
- Understanding managed vs. unmanaged resources
- Implementing the `IDisposable` pattern correctly
- Using `using` statements for automatic cleanup
- Managing WinForms-specific resources (Graphics, Controls, Images)
- Preventing event handler memory leaks

---

## 🎯 Why This Matters

### Memory Leaks
Failing to dispose resources leads to memory leaks that accumulate over time:
- **Graphics objects** (Brush, Pen, Font) consume GDI resources (limited to ~10,000 per process)
- **File handles** remain open, preventing file access
- **Database connections** exhaust connection pool
- **Event subscriptions** keep objects alive indefinitely

### Performance Degradation
Undisposed resources cause:
- Increased memory pressure → more frequent garbage collection
- Slower application response times
- UI freezing and stuttering
- Eventually: OutOfMemoryException or application crashes

### Resource Exhaustion
Windows has finite resources:
- **GDI objects**: Limited per process (~10,000)
- **User objects**: Limited per process (~10,000)
- **File handles**: System-wide limitation
- **Connection pool**: Database connection limits

---

## IDisposable Pattern

### When to Implement IDisposable

Implement `IDisposable` when your class:

1. **Holds unmanaged resources** (file handles, database connections, GDI objects)
2. **Contains IDisposable dependencies**
3. **Subscribes to events** on long-lived objects
4. **Allocates large memory buffers**

```csharp
// Example 1: Class with unmanaged resources
public class ImageProcessor : IDisposable
{
    private Graphics graphics;
    private Bitmap bitmap;

    public ImageProcessor(int width, int height)
    {
        bitmap = new Bitmap(width, height);
        graphics = Graphics.FromImage(bitmap);
    }

    public void Dispose()
    {
        graphics?.Dispose();
        bitmap?.Dispose();
    }
}

// Example 2: Class with IDisposable dependencies
public class DatabaseLogger : IDisposable
{
    private SqlConnection connection;
    private FileStream logFile;

    public DatabaseLogger(string connectionString, string logPath)
    {
        connection = new SqlConnection(connectionString);
        logFile = new FileStream(logPath, FileMode.Append);
    }

    public void Dispose()
    {
        connection?.Dispose();
        logFile?.Dispose();
    }
}

// Example 3: Class with event subscriptions
public class FormMonitor : IDisposable
{
    private Form targetForm;

    public FormMonitor(Form form)
    {
        targetForm = form;
        targetForm.Resize += OnFormResize;
        targetForm.Move += OnFormMove;
    }

    private void OnFormResize(object sender, EventArgs e) { }
    private void OnFormMove(object sender, EventArgs e) { }

    public void Dispose()
    {
        if (targetForm != null)
        {
            targetForm.Resize -= OnFormResize;
            targetForm.Move -= OnFormMove;
        }
    }
}
```

### Standard Dispose Pattern

The **full IDisposable pattern** with finalizer for classes handling unmanaged resources:

```csharp
public class ResourceHandler : IDisposable
{
    // Managed resources
    private SqlConnection connection;
    private FileStream fileStream;

    // Unmanaged resources (IntPtr, handles, etc.)
    private IntPtr unmanagedHandle;

    // Track disposal state
    private bool disposed = false;

    public ResourceHandler(string connectionString, string filePath)
    {
        connection = new SqlConnection(connectionString);
        fileStream = new FileStream(filePath, FileMode.OpenOrCreate);
        unmanagedHandle = NativeMethods.CreateHandle(); // Example
    }

    // Public Dispose method
    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this); // Prevent finalizer from running
    }

    // Protected virtual Dispose method
    protected virtual void Dispose(bool disposing)
    {
        if (!disposed)
        {
            if (disposing)
            {
                // Dispose managed resources
                connection?.Dispose();
                fileStream?.Dispose();
            }

            // Free unmanaged resources
            if (unmanagedHandle != IntPtr.Zero)
            {
                NativeMethods.CloseHandle(unmanagedHandle);
                unmanagedHandle = IntPtr.Zero;
            }

            disposed = true;
        }
    }

    // Finalizer (only if class has unmanaged resources)
    ~ResourceHandler()
    {
        Dispose(disposing: false);
    }

    // Helper method to check if disposed
    private void ThrowIfDisposed()
    {
        if (disposed)
        {
            throw new ObjectDisposedException(GetType().Name);
        }
    }

    public void DoWork()
    {
        ThrowIfDisposed();
        // Use resources safely
    }
}
```

### Simplified Pattern for Managed Resources Only

If your class **only** has managed resources (no unmanaged resources), use this simpler pattern:

```csharp
public class ManagedResourceHandler : IDisposable
{
    private SqlConnection connection;
    private FileStream fileStream;
    private bool disposed = false;

    public void Dispose()
    {
        if (!disposed)
        {
            connection?.Dispose();
            fileStream?.Dispose();
            disposed = true;
        }
    }
}
```

### Form Disposal

WinForms Forms have built-in disposal logic. Always extend it properly:

```csharp
public partial class CustomerForm : Form
{
    private DatabaseService dbService;
    private Timer autoSaveTimer;
    private Bitmap backgroundImage;

    public CustomerForm(DatabaseService service)
    {
        InitializeComponent();

        dbService = service;
        autoSaveTimer = new Timer { Interval = 30000 };
        backgroundImage = new Bitmap("background.png");
    }

    // Override Dispose from Form base class
    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            // Dispose managed resources not in components
            dbService?.Dispose();
            autoSaveTimer?.Dispose();
            backgroundImage?.Dispose();

            // components is handled by designer-generated code
            components?.Dispose();
        }

        base.Dispose(disposing); // Always call base
    }
}
```

**Designer-generated code** (in `CustomerForm.Designer.cs`):

```csharp
private System.ComponentModel.IContainer components = null;

protected override void Dispose(bool disposing)
{
    if (disposing && (components != null))
    {
        components.Dispose();
    }
    base.Dispose(disposing);
}
```

---

## Using Statement

### Basic Using

The `using` statement ensures automatic disposal even if exceptions occur:

```csharp
// Syntax
using (var resource = new DisposableResource())
{
    // Use resource
} // Automatically calls Dispose()

// Example 1: File operations
public void SaveToFile(string path, string content)
{
    using (var writer = new StreamWriter(path))
    {
        writer.WriteLine(content);
    } // writer.Dispose() called automatically
}

// Example 2: Graphics operations
public void DrawCustomControl(Graphics g, Rectangle bounds)
{
    using (var brush = new SolidBrush(Color.Blue))
    using (var pen = new Pen(Color.Black, 2))
    {
        g.FillRectangle(brush, bounds);
        g.DrawRectangle(pen, bounds);
    }
}

// Under the hood, using is equivalent to:
{
    var resource = new DisposableResource();
    try
    {
        // Use resource
    }
    finally
    {
        resource?.Dispose();
    }
}
```

### Using Declaration (C# 8+)

Simplified syntax for cleaner code:

```csharp
// C# 8+ using declaration
public void ProcessImage(string imagePath)
{
    using var bitmap = new Bitmap(imagePath);
    using var graphics = Graphics.FromImage(bitmap);

    // bitmap and graphics disposed at end of method scope
    graphics.FillRectangle(Brushes.White, 0, 0, bitmap.Width, bitmap.Height);

} // Automatic disposal here

// Multiple resources with declaration
public async Task ProcessDataAsync(string dbConnection, string filePath)
{
    using var connection = new SqlConnection(dbConnection);
    using var command = connection.CreateCommand();
    using var fileStream = new FileStream(filePath, FileMode.Create);

    await connection.OpenAsync();
    // Use resources

} // All disposed in reverse order of declaration
```

### Multiple Resources

```csharp
// ❌ Wrong: Nested using blocks (hard to read)
using (var connection = new SqlConnection(connectionString))
{
    using (var command = connection.CreateCommand())
    {
        using (var reader = command.ExecuteReader())
        {
            // Deep nesting
        }
    }
}

// ✅ Correct: Chained using (C# 7 and earlier)
using (var connection = new SqlConnection(connectionString))
using (var command = connection.CreateCommand())
using (var reader = command.ExecuteReader())
{
    // All disposed in reverse order
}

// ✅ Best: Using declarations (C# 8+)
using var connection = new SqlConnection(connectionString);
using var command = connection.CreateCommand();
using var reader = command.ExecuteReader();

// Clean, readable code
```

---

## Common WinForms Resources

### Controls and Components

```csharp
// ✅ Correct: Controls in containers are auto-disposed
public class CustomerForm : Form
{
    private Button btnSave; // Disposed by Form.components
    private TextBox txtName; // Disposed by Form.components

    public CustomerForm()
    {
        InitializeComponent();
    }
}

// ✅ Correct: Dynamically created controls added to container
private void AddDynamicButton()
{
    var button = new Button { Text = "Dynamic", Location = new Point(10, 10) };
    this.Controls.Add(button); // Form will dispose it
}

// ❌ Wrong: Dynamically created controls not added to container
private void CreateOrphanControl()
{
    var button = new Button(); // Memory leak - never disposed
    // Not added to any container
}

// ✅ Correct: Dispose dynamically created controls not in container
private Button tempButton;

public void CreateTemporaryControl()
{
    tempButton = new Button();
    // Use it temporarily
}

protected override void Dispose(bool disposing)
{
    if (disposing)
    {
        tempButton?.Dispose(); // Manual disposal needed
        components?.Dispose();
    }
    base.Dispose(disposing);
}
```

### Graphics Objects

Graphics objects consume **unmanaged GDI resources** and MUST be disposed:

```csharp
// ❌ Wrong: Graphics objects not disposed
private void PaintPanel_Wrong(object sender, PaintEventArgs e)
{
    var brush = new SolidBrush(Color.Blue); // LEAK!
    var pen = new Pen(Color.Black, 2); // LEAK!
    var font = new Font("Arial", 12); // LEAK!

    e.Graphics.FillRectangle(brush, 0, 0, 100, 100);
    e.Graphics.DrawString("Text", font, brush, 10, 10);
}

// ✅ Correct: Using statements for disposal
private void PaintPanel_Correct(object sender, PaintEventArgs e)
{
    using (var brush = new SolidBrush(Color.Blue))
    using (var pen = new Pen(Color.Black, 2))
    using (var font = new Font("Arial", 12))
    {
        e.Graphics.FillRectangle(brush, 0, 0, 100, 100);
        e.Graphics.DrawString("Text", font, brush, 10, 10);
    }
}

// ✅ Correct: Using declarations (C# 8+)
private void PaintPanel_Modern(object sender, PaintEventArgs e)
{
    using var brush = new SolidBrush(Color.Blue);
    using var pen = new Pen(Color.Black, 2);
    using var font = new Font("Arial", 12);

    e.Graphics.FillRectangle(brush, 0, 0, 100, 100);
    e.Graphics.DrawString("Text", font, brush, 10, 10);
}

// ⚠️ Exception: System brushes don't need disposal
private void UseSystemBrushes(Graphics g)
{
    g.FillRectangle(Brushes.Blue, 0, 0, 100, 100); // No disposal needed
    g.DrawString("Text", SystemFonts.DefaultFont, Brushes.Black, 10, 10);
}
```

### Image and Bitmap Disposal

```csharp
// ❌ Wrong: Image loaded but never disposed
public class ImageViewer : Form
{
    private PictureBox pictureBox;

    public void LoadImage(string path)
    {
        pictureBox.Image = new Bitmap(path); // LEAK - replaces old image
    }
}

// ✅ Correct: Dispose old image before replacing
public class ImageViewer : Form
{
    private PictureBox pictureBox;

    public void LoadImage(string path)
    {
        var oldImage = pictureBox.Image;
        pictureBox.Image = new Bitmap(path);
        oldImage?.Dispose(); // Clean up old image
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            pictureBox.Image?.Dispose(); // Clean up final image
            components?.Dispose();
        }
        base.Dispose(disposing);
    }
}

// ✅ Best: Helper method for safe image replacement
public void SetImage(string path)
{
    using var newImage = new Bitmap(path);
    ReplaceImage(new Bitmap(newImage)); // Clone to avoid disposed image
}

private void ReplaceImage(Image newImage)
{
    var oldImage = pictureBox.Image;
    pictureBox.Image = newImage;
    oldImage?.Dispose();
}
```

### Icon Disposal

```csharp
// ✅ Correct: Icon disposal
public class CustomForm : Form
{
    public CustomForm()
    {
        using (var iconStream = Assembly.GetExecutingAssembly()
            .GetManifestResourceStream("App.icon.ico"))
        {
            if (iconStream != null)
            {
                this.Icon = new Icon(iconStream);
            }
        }
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            this.Icon?.Dispose();
            components?.Dispose();
        }
        base.Dispose(disposing);
    }
}
```

### File and Stream Handling

```csharp
// ❌ Wrong: Stream not disposed
public string ReadFile_Wrong(string path)
{
    var reader = new StreamReader(path); // LEAK!
    return reader.ReadToEnd();
}

// ✅ Correct: Using statement
public string ReadFile_Correct(string path)
{
    using var reader = new StreamReader(path);
    return reader.ReadToEnd();
}

// ✅ Correct: Multiple streams
public void CopyFile(string source, string destination)
{
    using var sourceStream = new FileStream(source, FileMode.Open);
    using var destStream = new FileStream(destination, FileMode.Create);
    sourceStream.CopyTo(destStream);
}
```

### Timers

```csharp
// System.Windows.Forms.Timer
public class AutoSaveForm : Form
{
    private System.Windows.Forms.Timer autoSaveTimer;

    public AutoSaveForm()
    {
        autoSaveTimer = new System.Windows.Forms.Timer
        {
            Interval = 30000 // 30 seconds
        };
        autoSaveTimer.Tick += AutoSave_Tick;
        autoSaveTimer.Start();
    }

    private void AutoSave_Tick(object sender, EventArgs e)
    {
        // Auto-save logic
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            autoSaveTimer?.Stop();
            autoSaveTimer?.Dispose();
            components?.Dispose();
        }
        base.Dispose(disposing);
    }
}

// System.Timers.Timer
public class BackgroundProcessor : IDisposable
{
    private System.Timers.Timer processingTimer;

    public BackgroundProcessor()
    {
        processingTimer = new System.Timers.Timer(5000);
        processingTimer.Elapsed += ProcessData;
        processingTimer.Start();
    }

    private void ProcessData(object sender, System.Timers.ElapsedEventArgs e)
    {
        // Processing logic
    }

    public void Dispose()
    {
        processingTimer?.Stop();
        processingTimer?.Dispose();
    }
}
```

### BackgroundWorker

```csharp
public class DataProcessorForm : Form
{
    private BackgroundWorker worker;

    public DataProcessorForm()
    {
        worker = new BackgroundWorker
        {
            WorkerReportsProgress = true,
            WorkerSupportsCancellation = true
        };

        worker.DoWork += Worker_DoWork;
        worker.ProgressChanged += Worker_ProgressChanged;
        worker.RunWorkerCompleted += Worker_RunWorkerCompleted;
    }

    private void Worker_DoWork(object sender, DoWorkEventArgs e) { }
    private void Worker_ProgressChanged(object sender, ProgressChangedEventArgs e) { }
    private void Worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e) { }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            if (worker != null)
            {
                // Unsubscribe events to prevent leaks
                worker.DoWork -= Worker_DoWork;
                worker.ProgressChanged -= Worker_ProgressChanged;
                worker.RunWorkerCompleted -= Worker_RunWorkerCompleted;

                worker.Dispose();
            }

            components?.Dispose();
        }
        base.Dispose(disposing);
    }
}
```

---

## Event Handler Memory Leaks

### The Problem

Event subscriptions create **strong references** that prevent garbage collection:

```csharp
// ❌ Memory leak example
public class DataService
{
    public event EventHandler DataChanged;
}

public class MonitorForm : Form
{
    private DataService dataService;

    public MonitorForm(DataService service)
    {
        dataService = service;
        dataService.DataChanged += OnDataChanged; // Strong reference!
    }

    private void OnDataChanged(object sender, EventArgs e)
    {
        // Update UI
    }

    // Form closed but DataService still holds reference to this form
    // Form cannot be garbage collected - MEMORY LEAK!
}
```

### Solutions

#### Solution 1: Unsubscribe in Dispose

```csharp
// ✅ Correct: Unsubscribe in Dispose
public class MonitorForm : Form
{
    private DataService dataService;

    public MonitorForm(DataService service)
    {
        dataService = service;
        dataService.DataChanged += OnDataChanged;
    }

    private void OnDataChanged(object sender, EventArgs e)
    {
        // Update UI
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            if (dataService != null)
            {
                dataService.DataChanged -= OnDataChanged; // Unsubscribe!
            }
            components?.Dispose();
        }
        base.Dispose(disposing);
    }
}
```

#### Solution 2: Weak Event Pattern

```csharp
// ✅ Weak event pattern (advanced)
public class WeakEventHandler<TEventArgs> where TEventArgs : EventArgs
{
    private readonly WeakReference targetReference;
    private readonly MethodInfo method;

    public WeakEventHandler(EventHandler<TEventArgs> handler)
    {
        targetReference = new WeakReference(handler.Target);
        method = handler.Method;
    }

    public void Handler(object sender, TEventArgs e)
    {
        var target = targetReference.Target;
        if (target != null)
        {
            method.Invoke(target, new object[] { sender, e });
        }
    }
}
```

#### Solution 3: Anonymous Method Issues

```csharp
// ❌ Wrong: Anonymous method prevents unsubscription
public class BadForm : Form
{
    private DataService dataService;

    public BadForm(DataService service)
    {
        dataService = service;
        dataService.DataChanged += (s, e) => UpdateUI(); // Can't unsubscribe!
    }

    private void UpdateUI() { }
}

// ✅ Correct: Store delegate reference for unsubscription
public class GoodForm : Form
{
    private DataService dataService;
    private EventHandler dataChangedHandler;

    public GoodForm(DataService service)
    {
        dataService = service;
        dataChangedHandler = (s, e) => UpdateUI();
        dataService.DataChanged += dataChangedHandler;
    }

    private void UpdateUI() { }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            if (dataService != null)
            {
                dataService.DataChanged -= dataChangedHandler;
            }
            components?.Dispose();
        }
        base.Dispose(disposing);
    }
}
```

---

## Best Practices

### ✅ DO

1. **Always use `using` for IDisposable objects**
```csharp
using var connection = new SqlConnection(connectionString);
using var command = connection.CreateCommand();
```

2. **Dispose Graphics objects (Brush, Pen, Font)**
```csharp
using var brush = new SolidBrush(Color.Blue);
using var pen = new Pen(Color.Black, 2);
```

3. **Dispose replaced images in PictureBox**
```csharp
var oldImage = pictureBox.Image;
pictureBox.Image = newImage;
oldImage?.Dispose();
```

4. **Unsubscribe from events in Dispose**
```csharp
protected override void Dispose(bool disposing)
{
    if (disposing)
    {
        service.DataChanged -= OnDataChanged;
    }
    base.Dispose(disposing);
}
```

5. **Implement IDisposable for classes with disposable fields**
```csharp
public class Manager : IDisposable
{
    private DatabaseConnection connection;
    public void Dispose() => connection?.Dispose();
}
```

6. **Use null-conditional operator when disposing**
```csharp
resource?.Dispose(); // Safe even if null
```

7. **Dispose in reverse order of creation**
```csharp
fileStream?.Dispose();
connection?.Dispose();
```

8. **Call base.Dispose() in Form disposal**
```csharp
protected override void Dispose(bool disposing)
{
    if (disposing) { /* cleanup */ }
    base.Dispose(disposing); // Always call base
}
```

9. **Stop timers before disposing**
```csharp
timer?.Stop();
timer?.Dispose();
```

10. **Use ObjectDisposedException for disposed objects**
```csharp
private void ThrowIfDisposed()
{
    if (disposed) throw new ObjectDisposedException(GetType().Name);
}
```

### ❌ DON'T

1. **Don't forget to dispose Graphics objects**
```csharp
// ❌ Wrong
var brush = new SolidBrush(Color.Blue); // LEAK
```

2. **Don't dispose objects you don't own**
```csharp
// ❌ Wrong
private void Paint(object sender, PaintEventArgs e)
{
    e.Graphics.Dispose(); // DON'T! You don't own this
}
```

3. **Don't dispose System.Brushes, SystemFonts, etc.**
```csharp
// ❌ Wrong
Brushes.Blue.Dispose(); // DON'T! Shared resource
```

4. **Don't create resources in loops without disposal**
```csharp
// ❌ Wrong
for (int i = 0; i < 1000; i++)
{
    var font = new Font("Arial", 12); // LEAK
}
```

5. **Don't forget Form.components disposal**
```csharp
// ❌ Wrong - missing components disposal
protected override void Dispose(bool disposing)
{
    // Missing: components?.Dispose();
    base.Dispose(disposing);
}
```

6. **Don't use disposed objects**
```csharp
// ❌ Wrong
using var bitmap = new Bitmap(100, 100);
bitmap.Dispose();
pictureBox.Image = bitmap; // Already disposed!
```

7. **Don't rely on finalizers for timely cleanup**
```csharp
// ❌ Wrong - finalizer is unpredictable
~MyClass() { CleanupResources(); } // May never run
```

8. **Don't subscribe to static events without unsubscribing**
```csharp
// ❌ Wrong
StaticEventSource.SomeEvent += Handler; // Prevents GC forever
```

---

## Complete Working Examples

### Example 1: Form with Proper Resource Disposal

```csharp
public partial class CustomerManagementForm : Form
{
    // Managed by designer
    private System.ComponentModel.IContainer components = null;

    // Custom disposable resources
    private DatabaseService dbService;
    private System.Windows.Forms.Timer autoSaveTimer;
    private Bitmap logoImage;
    private BackgroundWorker dataLoader;
    private EventHandler<DataChangedEventArgs> dataChangedHandler;

    public CustomerManagementForm(DatabaseService service)
    {
        InitializeComponent();

        dbService = service;

        // Load logo
        logoImage = new Bitmap("logo.png");

        // Setup auto-save timer
        autoSaveTimer = new System.Windows.Forms.Timer { Interval = 30000 };
        autoSaveTimer.Tick += AutoSave_Tick;
        autoSaveTimer.Start();

        // Setup background worker
        dataLoader = new BackgroundWorker();
        dataLoader.DoWork += DataLoader_DoWork;
        dataLoader.RunWorkerCompleted += DataLoader_Completed;

        // Subscribe to service events
        dataChangedHandler = OnDataChanged;
        dbService.DataChanged += dataChangedHandler;
    }

    private void AutoSave_Tick(object sender, EventArgs e) { /* Auto-save */ }
    private void DataLoader_DoWork(object sender, DoWorkEventArgs e) { /* Load data */ }
    private void DataLoader_Completed(object sender, RunWorkerCompletedEventArgs e) { /* Update UI */ }
    private void OnDataChanged(object sender, DataChangedEventArgs e) { /* Refresh */ }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            // Dispose timer
            autoSaveTimer?.Stop();
            autoSaveTimer?.Dispose();

            // Dispose background worker
            if (dataLoader != null)
            {
                dataLoader.DoWork -= DataLoader_DoWork;
                dataLoader.RunWorkerCompleted -= DataLoader_Completed;
                dataLoader.Dispose();
            }

            // Unsubscribe from events
            if (dbService != null)
            {
                dbService.DataChanged -= dataChangedHandler;
            }

            // Dispose images
            logoImage?.Dispose();

            // Dispose database service
            dbService?.Dispose();

            // Dispose designer components
            components?.Dispose();
        }

        base.Dispose(disposing);
    }
}
```

### Example 2: Image Cache with Proper Disposal

```csharp
public class ImageCache : IDisposable
{
    private readonly Dictionary<string, Bitmap> cache = new();
    private readonly object lockObject = new();
    private bool disposed = false;

    public Bitmap GetImage(string path)
    {
        ThrowIfDisposed();

        lock (lockObject)
        {
            if (cache.TryGetValue(path, out var cachedImage))
            {
                return cachedImage;
            }

            var image = new Bitmap(path);
            cache[path] = image;
            return image;
        }
    }

    public void RemoveImage(string path)
    {
        ThrowIfDisposed();

        lock (lockObject)
        {
            if (cache.TryGetValue(path, out var image))
            {
                image.Dispose();
                cache.Remove(path);
            }
        }
    }

    public void Clear()
    {
        ThrowIfDisposed();

        lock (lockObject)
        {
            foreach (var image in cache.Values)
            {
                image.Dispose();
            }
            cache.Clear();
        }
    }

    private void ThrowIfDisposed()
    {
        if (disposed)
        {
            throw new ObjectDisposedException(nameof(ImageCache));
        }
    }

    public void Dispose()
    {
        if (!disposed)
        {
            lock (lockObject)
            {
                foreach (var image in cache.Values)
                {
                    image.Dispose();
                }
                cache.Clear();
            }
            disposed = true;
        }
    }
}
```

### Example 3: Database Connection Management

```csharp
public class CustomerRepository : IDisposable
{
    private readonly string connectionString;
    private bool disposed = false;

    public CustomerRepository(string connectionString)
    {
        this.connectionString = connectionString;
    }

    public async Task<List<Customer>> GetAllCustomersAsync()
    {
        ThrowIfDisposed();

        var customers = new List<Customer>();

        using var connection = new SqlConnection(connectionString);
        using var command = new SqlCommand("SELECT * FROM Customers", connection);

        await connection.OpenAsync();
        using var reader = await command.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            customers.Add(new Customer
            {
                Id = reader.GetInt32(0),
                Name = reader.GetString(1),
                Email = reader.GetString(2)
            });
        }

        return customers;
    }

    private void ThrowIfDisposed()
    {
        if (disposed)
        {
            throw new ObjectDisposedException(nameof(CustomerRepository));
        }
    }

    public void Dispose()
    {
        disposed = true;
    }
}
```

---

## Debugging Memory Leaks

### Tools

1. **Visual Studio Diagnostic Tools**
   - Memory Usage tool
   - .NET Object Allocation Tracking
   - Heap snapshots

2. **JetBrains dotMemory**
   - Memory profiling
   - Leak detection
   - Object retention paths

3. **ANTS Memory Profiler**
   - Real-time monitoring
   - Snapshot comparison
   - Leak identification

### Finding Leaks

```csharp
// Use diagnostic code to track instances
public class LeakDetector
{
    private static int instanceCount = 0;

    public LeakDetector()
    {
        Interlocked.Increment(ref instanceCount);
        Debug.WriteLine($"Instances created: {instanceCount}");
    }

    ~LeakDetector()
    {
        Interlocked.Decrement(ref instanceCount);
        Debug.WriteLine($"Instances finalized: {instanceCount}");
    }
}
```

### Prevention Checklist

- [ ] All IDisposable fields disposed
- [ ] All event handlers unsubscribed
- [ ] All Graphics objects disposed
- [ ] All Images/Bitmaps disposed
- [ ] All Timers stopped and disposed
- [ ] All BackgroundWorkers disposed
- [ ] All database connections closed
- [ ] All file streams closed
- [ ] Static event subscriptions cleaned up
- [ ] base.Dispose() called in Forms

---

## Related Topics

- [Performance Optimization](/home/user/winforms-coding-standards/docs/best-practices/performance.md)
- [Async/Await Pattern](/home/user/winforms-coding-standards/docs/best-practices/async-await.md)
- [Thread Safety](/home/user/winforms-coding-standards/docs/best-practices/thread-safety.md)
- [Error Handling & Logging](/home/user/winforms-coding-standards/docs/best-practices/error-handling.md)

---

**Last Updated**: 2025-11-07
**Lines**: ~450
