# Memory Management in WinForms

## 📋 Overview

Memory management in .NET WinForms applications involves understanding how the Garbage Collector (GC) works, preventing memory leaks, and optimizing memory usage. While .NET provides automatic memory management, improper resource handling can still cause leaks, especially with unmanaged resources, event handlers, and long-lived object references.

This guide covers memory management fundamentals, common leak sources, profiling techniques, and best practices for building memory-efficient WinForms applications.

---

## 🎯 Why This Matters

**Common Memory Problems**:
- **Memory Leaks**: Objects not released, causing growing memory usage
- **OutOfMemoryException**: Application crashes due to excessive memory consumption
- **Performance Degradation**: Increased GC pressure slows down the UI
- **Resource Exhaustion**: File handles, database connections not released
- **Large Object Heap Fragmentation**: Can cause OOM even with available memory

**Real-World Impact**:
- Forms that grow memory usage over time
- Applications that crash after hours of use
- Slow, unresponsive UI due to frequent GC collections
- Server processes consuming excessive memory

---

## Understanding .NET Memory

### Garbage Collection Basics

The .NET Garbage Collector automatically manages memory by:
1. **Allocating memory** for new objects on the managed heap
2. **Tracking object references** to determine reachability
3. **Reclaiming memory** from unreachable objects
4. **Compacting the heap** to reduce fragmentation

**How GC Works**:
```
Object Graph:
[Root] → [ObjectA] → [ObjectB]
         [ObjectC] ← (no references, can be collected)

GC Process:
1. Mark Phase: Mark all reachable objects from roots
2. Sweep Phase: Reclaim unreachable objects
3. Compact Phase: Move objects to eliminate fragmentation
```

### Generations (0, 1, 2)

The GC uses a generational approach:

```csharp
// Generation 0: Newly created objects
var temp = new byte[1024]; // Gen 0

// Generation 1: Survived one GC collection
// Objects move from Gen 0 → Gen 1 after first collection

// Generation 2: Long-lived objects
// Objects move from Gen 1 → Gen 2 after second collection
static List<Customer> _cache = new(); // Likely Gen 2
```

**Generation Characteristics**:
- **Gen 0**: Short-lived objects, collected frequently
- **Gen 1**: Buffer between Gen 0 and Gen 2, collected moderately
- **Gen 2**: Long-lived objects, collected infrequently (expensive)

### Garbage Collection Triggers

GC runs automatically when:
- **Gen 0 threshold reached** (~256 KB by default)
- **System memory pressure** (low available memory)
- **Explicit call** to `GC.Collect()` (rare, discouraged)
- **Large object allocation** (>85 KB)

### Finalization Queue

Objects with finalizers require special handling:

```csharp
public class ResourceHolder
{
    ~ResourceHolder() // Finalizer
    {
        // Cleanup code - runs on finalizer thread
        // NOT recommended - use IDisposable instead
    }
}
```

**Finalization Process**:
1. Object marked for finalization goes to finalization queue
2. GC promotes object to next generation (delays collection)
3. Finalizer thread runs finalizers
4. Object collected in next GC cycle

**Problem**: Finalizers delay collection and can cause memory pressure.

### Managed vs Unmanaged Memory

**Managed Memory** (GC handles automatically):
```csharp
// Managed - GC will collect
List<string> names = new List<string>();
Customer customer = new Customer();
byte[] buffer = new byte[1024];
```

**Unmanaged Memory** (must dispose manually):
```csharp
// Unmanaged - must dispose
using (var stream = new FileStream("file.txt", FileMode.Open)) { }
using (var bitmap = new Bitmap("image.png")) { }
using (var connection = new SqlConnection(connectionString)) { }
using (var brush = new SolidBrush(Color.Red)) { }
```

**Key Distinction**:
- **Managed**: Reference types on managed heap (GC handles)
- **Unmanaged**: OS resources (file handles, GDI objects, database connections)

---

## Common Memory Leak Sources

### Event Handler Leaks

**Problem**: Event subscriptions create strong references that prevent garbage collection.

#### ❌ Leak Example

```csharp
public class PublisherForm : Form
{
    public event EventHandler DataChanged;
}

public class SubscriberForm : Form
{
    private PublisherForm _publisher;

    public SubscriberForm(PublisherForm publisher)
    {
        _publisher = publisher;
        // Leak: Publisher holds reference to this form
        _publisher.DataChanged += OnDataChanged;
    }

    private void OnDataChanged(object sender, EventArgs e)
    {
        // Handle event
    }

    // Problem: Never unsubscribes - form can't be collected even after Close()
}
```

**Why It Leaks**: The publisher holds a reference to the subscriber through the event handler, preventing GC from collecting the subscriber even after it's closed.

#### ✅ Fixed Version

```csharp
public class SubscriberForm : Form
{
    private PublisherForm _publisher;

    public SubscriberForm(PublisherForm publisher)
    {
        _publisher = publisher;
        _publisher.DataChanged += OnDataChanged;
    }

    private void OnDataChanged(object sender, EventArgs e)
    {
        // Handle event
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            // Unsubscribe to break reference
            if (_publisher != null)
            {
                _publisher.DataChanged -= OnDataChanged;
                _publisher = null;
            }
        }
        base.Dispose(disposing);
    }
}
```

### Static References

**Problem**: Static references keep objects alive for the application lifetime.

#### ❌ Leak Example

```csharp
public class FormCache
{
    // Leak: Forms are never removed, held forever
    private static readonly List<Form> _openForms = new List<Form>();

    public static void RegisterForm(Form form)
    {
        _openForms.Add(form); // Strong reference
    }

    // No removal mechanism - forms accumulate
}

public class MainForm : Form
{
    private void btnOpenChild_Click(object sender, EventArgs e)
    {
        var child = new ChildForm();
        FormCache.RegisterForm(child); // Leaked!
        child.Show();
    }
}
```

#### ✅ Fixed Version

```csharp
public class FormCache
{
    // Use WeakReference to allow GC
    private static readonly List<WeakReference<Form>> _openForms = new();

    public static void RegisterForm(Form form)
    {
        _openForms.Add(new WeakReference<Form>(form));

        // Subscribe to closed event to cleanup
        form.FormClosed += (s, e) => RemoveForm(form);
    }

    private static void RemoveForm(Form form)
    {
        _openForms.RemoveAll(wr => !wr.TryGetTarget(out var f) || f == form);
    }

    public static List<Form> GetOpenForms()
    {
        var forms = new List<Form>();
        _openForms.RemoveAll(wr =>
        {
            if (wr.TryGetTarget(out var form))
            {
                forms.Add(form);
                return false;
            }
            return true; // Remove dead references
        });
        return forms;
    }
}
```

### Timer Leaks

**Problem**: Timers hold references to forms and must be disposed.

#### ❌ Leak Example

```csharp
public class StatusForm : Form
{
    private System.Windows.Forms.Timer _refreshTimer;

    public StatusForm()
    {
        _refreshTimer = new System.Windows.Forms.Timer();
        _refreshTimer.Interval = 1000;
        _refreshTimer.Tick += RefreshStatus;
        _refreshTimer.Start();

        // Never disposed - timer holds reference to this form
    }

    private void RefreshStatus(object sender, EventArgs e)
    {
        // Update UI
    }
}
```

#### ✅ Fixed Version

```csharp
public class StatusForm : Form
{
    private System.Windows.Forms.Timer _refreshTimer;

    public StatusForm()
    {
        _refreshTimer = new System.Windows.Forms.Timer();
        _refreshTimer.Interval = 1000;
        _refreshTimer.Tick += RefreshStatus;
        _refreshTimer.Start();
    }

    private void RefreshStatus(object sender, EventArgs e)
    {
        // Update UI
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _refreshTimer?.Stop();
            _refreshTimer?.Dispose();
            _refreshTimer = null;
        }
        base.Dispose(disposing);
    }
}
```

### Control References

**Problem**: Keeping references to disposed controls prevents memory release.

#### ❌ Leak Example

```csharp
public class MainForm : Form
{
    private List<ChildForm> _childForms = new List<ChildForm>();

    private void btnOpenChild_Click(object sender, EventArgs e)
    {
        var child = new ChildForm();
        _childForms.Add(child); // Keeps reference even after Close()
        child.Show();
    }

    // Child forms can close, but reference keeps them in memory
}
```

#### ✅ Fixed Version

```csharp
public class MainForm : Form
{
    private List<ChildForm> _childForms = new List<ChildForm>();

    private void btnOpenChild_Click(object sender, EventArgs e)
    {
        var child = new ChildForm();
        _childForms.Add(child);

        // Remove reference when closed
        child.FormClosed += (s, e) =>
        {
            _childForms.Remove(child);
        };

        child.Show();
    }
}
```

### Cached Data

**Problem**: Unbounded caches grow indefinitely.

#### ❌ Leak Example

```csharp
public class ImageCache
{
    // No size limit - grows forever
    private static Dictionary<string, Bitmap> _cache = new();

    public static Bitmap GetImage(string path)
    {
        if (!_cache.ContainsKey(path))
        {
            _cache[path] = new Bitmap(path); // Leak: never removed
        }
        return _cache[path];
    }
}
```

#### ✅ Fixed Version (LRU Cache with Size Limit)

```csharp
public class ImageCache
{
    private const int MAX_CACHE_SIZE = 50;
    private static LinkedList<string> _accessOrder = new();
    private static Dictionary<string, Bitmap> _cache = new();

    public static Bitmap GetImage(string path)
    {
        if (_cache.TryGetValue(path, out var bitmap))
        {
            // Move to end (most recently used)
            _accessOrder.Remove(path);
            _accessOrder.AddLast(path);
            return bitmap;
        }

        // Add new image
        var newBitmap = new Bitmap(path);
        _cache[path] = newBitmap;
        _accessOrder.AddLast(path);

        // Evict oldest if over limit
        if (_cache.Count > MAX_CACHE_SIZE)
        {
            var oldest = _accessOrder.First.Value;
            _accessOrder.RemoveFirst();
            _cache[oldest]?.Dispose();
            _cache.Remove(oldest);
        }

        return newBitmap;
    }

    public static void Clear()
    {
        foreach (var bitmap in _cache.Values)
        {
            bitmap?.Dispose();
        }
        _cache.Clear();
        _accessOrder.Clear();
    }
}
```

### Image and Bitmap Leaks

**Problem**: GDI objects (Bitmap, Image, Graphics) must be disposed.

#### ❌ Leak Example

```csharp
public class ImageForm : Form
{
    private PictureBox pictureBox;

    private void LoadImage(string path)
    {
        // Leak: Old image not disposed
        pictureBox.Image = new Bitmap(path);

        // Leak: Image loaded every time
        var tempImage = Image.FromFile(path);
        // Never disposed
    }

    private void ProcessImage(Bitmap source)
    {
        // Leak: Clone not disposed
        var modified = (Bitmap)source.Clone();
        // Process modified
        // Never disposed
    }
}
```

#### ✅ Fixed Version

```csharp
public class ImageForm : Form
{
    private PictureBox pictureBox;

    private void LoadImage(string path)
    {
        // Dispose old image
        pictureBox.Image?.Dispose();

        // Load new image
        pictureBox.Image = new Bitmap(path);
    }

    private void ProcessImage(Bitmap source)
    {
        using (var modified = (Bitmap)source.Clone())
        {
            // Process modified
            // Automatically disposed
        }
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            pictureBox.Image?.Dispose();
        }
        base.Dispose(disposing);
    }
}
```

### DataBinding Leaks

**Problem**: BindingSource and related objects must be disposed.

#### ❌ Leak Example

```csharp
public class CustomerForm : Form
{
    private BindingSource _bindingSource;
    private DataGridView dgvCustomers;

    public void LoadCustomers(List<Customer> customers)
    {
        // Leak: Old binding source not disposed
        _bindingSource = new BindingSource();
        _bindingSource.DataSource = customers;
        dgvCustomers.DataSource = _bindingSource;
    }
}
```

#### ✅ Fixed Version

```csharp
public class CustomerForm : Form
{
    private BindingSource _bindingSource;
    private DataGridView dgvCustomers;

    public void LoadCustomers(List<Customer> customers)
    {
        // Dispose old binding source
        _bindingSource?.Dispose();

        _bindingSource = new BindingSource();
        _bindingSource.DataSource = customers;
        dgvCustomers.DataSource = _bindingSource;
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _bindingSource?.Dispose();
        }
        base.Dispose(disposing);
    }
}
```

---

## Memory Profiling

### Tools

**Visual Studio Memory Profiler** (Built-in):
- Heap snapshots
- Memory usage over time
- Object allocation tracking
- .NET Object Allocation tool

**dotMemory** (JetBrains):
- Advanced memory analysis
- Retention paths
- Comparison snapshots
- Automatic leak detection

**ANTS Memory Profiler** (Redgate):
- Real-time memory monitoring
- Instance categorizer
- Memory timeline

**PerfView** (Microsoft, Free):
- Heap snapshots
- GC analysis
- Memory leak detection
- ETW-based profiling

### Finding Leaks

**Heap Snapshot Workflow**:

1. **Take baseline snapshot** when application starts
2. **Perform actions** that should release memory
3. **Force garbage collection** (`GC.Collect(); GC.WaitForPendingFinalizers();`)
4. **Take second snapshot**
5. **Compare snapshots** to find retained objects

**Visual Studio Steps**:
```
1. Debug → Performance Profiler
2. Select ".NET Object Allocation"
3. Start profiling
4. Perform actions (open/close forms)
5. Take Snapshot #1
6. Repeat actions
7. Take Snapshot #2
8. Compare snapshots
9. Look for objects that should be collected but aren't
```

### Object Retention Paths

**Retention Path**: Chain of references keeping an object alive.

Example:
```
[Static Field: FormCache._forms]
    → [List<Form>]
        → [ChildForm instance]
            → [Button controls]
                → [Event handlers]
```

**Finding Retention Paths** (dotMemory/Visual Studio):
1. Find leaked object in heap
2. View "Paths to GC Roots"
3. Identify unexpected reference chain
4. Fix the root cause (usually event handler or static reference)

---

## Prevention Strategies

### Weak References

Use `WeakReference<T>` for caches where objects can be collected if memory is needed.

```csharp
public class DocumentCache
{
    private Dictionary<string, WeakReference<Document>> _cache = new();

    public bool TryGetDocument(string id, out Document doc)
    {
        if (_cache.TryGetValue(id, out var weakRef))
        {
            if (weakRef.TryGetTarget(out doc))
            {
                return true; // Document still alive
            }
            else
            {
                _cache.Remove(id); // Clean up dead reference
            }
        }

        doc = null;
        return false;
    }

    public void AddDocument(string id, Document doc)
    {
        _cache[id] = new WeakReference<Document>(doc);
    }
}
```

**When to Use**:
- Caches where items can be recreated
- Observer patterns with many subscribers
- Tracking active forms/windows

### Unsubscribe from Events

**Pattern: Unsubscribe in Dispose**:

```csharp
public class DataForm : Form
{
    private DataService _dataService;

    public DataForm(DataService dataService)
    {
        _dataService = dataService;
        _dataService.DataChanged += OnDataChanged;
    }

    private void OnDataChanged(object sender, EventArgs e)
    {
        RefreshData();
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            if (_dataService != null)
            {
                _dataService.DataChanged -= OnDataChanged;
                _dataService = null;
            }
        }
        base.Dispose(disposing);
    }
}
```

**Anonymous Methods**: Store reference to unsubscribe.

```csharp
public class NotificationForm : Form
{
    private EventHandler _handler;

    public void Subscribe(NotificationService service)
    {
        _handler = (s, e) => UpdateNotifications();
        service.NotificationReceived += _handler;
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing && _handler != null)
        {
            // Can now unsubscribe
            service.NotificationReceived -= _handler;
        }
        base.Dispose(disposing);
    }
}
```

### Dispose Pattern

**Correct IDisposable Implementation**:

```csharp
public class ResourceManager : IDisposable
{
    private SqlConnection _connection; // Unmanaged
    private List<Customer> _cache; // Managed
    private bool _disposed = false;

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this); // No need for finalizer
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                // Dispose managed resources
                _connection?.Dispose();
                _cache?.Clear();
                _cache = null;
            }

            // Free unmanaged resources (if any)
            // Not common in typical WinForms apps

            _disposed = true;
        }
    }
}
```

**Using Statement** (Recommended):

```csharp
// Automatic disposal
using (var manager = new ResourceManager())
{
    manager.LoadData();
} // Dispose called here

// C# 8+ declaration syntax
using var manager = new ResourceManager();
manager.LoadData();
// Dispose called at end of scope
```

---

## GC Optimization

### GC.Collect() Usage

**Warning**: Rarely needed, can hurt performance.

#### ❌ Don't Do This

```csharp
private void ProcessLargeData()
{
    var data = LoadData();
    ProcessData(data);

    // Bad: Forces expensive full collection
    GC.Collect();
    GC.WaitForPendingFinalizers();
    GC.Collect();
}
```

#### ✅ Acceptable Use Case

```csharp
private void AfterLoadingLargeDataSet()
{
    // Load 1GB of initial data
    LoadInitialDataSet();

    // Force collection to establish baseline before UI becomes interactive
    GC.Collect(2, GCCollectionMode.Forced, blocking: true, compacting: true);
}
```

**Only use when**:
- After large one-time initialization
- Before memory-sensitive operations
- For diagnostic purposes

### Large Object Heap (LOH)

Objects ≥ 85 KB go to LOH (Generation 2).

**Problem**: LOH is not compacted by default → fragmentation.

```csharp
// These go to LOH
byte[] largeBuffer = new byte[85000]; // 85 KB
double[] largeArray = new double[10625]; // 85 KB (8 bytes each)
```

**Solutions**:

```csharp
// 1. Use ArrayPool for large temporary buffers
using System.Buffers;

byte[] buffer = ArrayPool<byte>.Shared.Rent(100000);
try
{
    // Use buffer
}
finally
{
    ArrayPool<byte>.Shared.Return(buffer);
}

// 2. Compact LOH explicitly (.NET 4.5.1+)
GCSettings.LargeObjectHeapCompactionMode = GCLargeObjectHeapCompactionMode.CompactOnce;
GC.Collect();
```

### Memory Pressure

Inform GC about unmanaged memory usage:

```csharp
public class LargeUnmanagedResource : IDisposable
{
    private IntPtr _nativeMemory;
    private const int SIZE = 10_000_000; // 10 MB

    public LargeUnmanagedResource()
    {
        _nativeMemory = Marshal.AllocHGlobal(SIZE);

        // Tell GC about memory pressure
        GC.AddMemoryPressure(SIZE);
    }

    public void Dispose()
    {
        if (_nativeMemory != IntPtr.Zero)
        {
            Marshal.FreeHGlobal(_nativeMemory);

            // Remove memory pressure
            GC.RemoveMemoryPressure(SIZE);

            _nativeMemory = IntPtr.Zero;
        }
    }
}
```

---

## Best Practices

### ✅ DO

1. **Dispose all IDisposable objects**:
```csharp
using (var connection = new SqlConnection(connectionString))
{
    // Use connection
}
```

2. **Unsubscribe from events in Dispose**:
```csharp
protected override void Dispose(bool disposing)
{
    if (disposing)
    {
        _service.DataChanged -= OnDataChanged;
    }
    base.Dispose(disposing);
}
```

3. **Use weak references for caches**:
```csharp
private Dictionary<string, WeakReference<Bitmap>> _imageCache = new();
```

4. **Dispose images and bitmaps**:
```csharp
pictureBox.Image?.Dispose();
pictureBox.Image = new Bitmap(path);
```

5. **Limit cache sizes**:
```csharp
if (_cache.Count > MAX_SIZE)
{
    EvictOldestEntry();
}
```

6. **Use ArrayPool for large temporary buffers**:
```csharp
var buffer = ArrayPool<byte>.Shared.Rent(size);
```

7. **Null out references after disposal**:
```csharp
_connection?.Dispose();
_connection = null;
```

8. **Profile memory usage regularly**:
```
Use Visual Studio Memory Profiler during development
```

9. **Implement IDisposable for classes holding resources**:
```csharp
public class DataManager : IDisposable
```

10. **Use using declarations (C# 8+)**:
```csharp
using var stream = File.OpenRead(path);
// Disposed at end of method
```

11. **Clear collections when done**:
```csharp
_cache.Clear();
_listeners.Clear();
```

12. **Monitor GC collections in production**:
```csharp
GC.CollectionCount(0); // Track Gen 0 collections
```

### ❌ DON'T

1. **Don't keep references to closed forms**:
```csharp
// Bad
private List<Form> _forms = new(); // Never cleared
```

2. **Don't forget to unsubscribe from events**:
```csharp
// Bad
publisher.Event += Handler; // Leaked
```

3. **Don't use finalizers unless absolutely necessary**:
```csharp
// Bad - use IDisposable instead
~MyClass() { }
```

4. **Don't call GC.Collect() unnecessarily**:
```csharp
// Bad
GC.Collect(); // Let GC decide
```

5. **Don't create unbounded caches**:
```csharp
// Bad
static Dictionary<string, object> _cache = new(); // Grows forever
```

6. **Don't clone images without disposing**:
```csharp
// Bad
var copy = (Bitmap)original.Clone(); // Leaked
```

7. **Don't forget to dispose timers**:
```csharp
// Bad
private Timer _timer = new Timer(); // Never disposed
```

8. **Don't keep static event handlers**:
```csharp
// Bad
static void Handler() { }
someObject.Event += Handler; // Keeps someObject alive
```

9. **Don't ignore disposal in derived classes**:
```csharp
// Bad
protected override void Dispose(bool disposing)
{
    // Forgot to call base.Dispose(disposing)
}
```

10. **Don't mix using statement with manual disposal**:
```csharp
// Bad
using (var stream = new FileStream(...))
{
    stream.Dispose(); // Double disposal
}
```

---

## Monitoring Memory Usage

### Performance Counters

```csharp
using System.Diagnostics;

public class MemoryMonitor
{
    private PerformanceCounter _privateBytes;
    private PerformanceCounter _gen0Collections;
    private PerformanceCounter _gen1Collections;
    private PerformanceCounter _gen2Collections;

    public MemoryMonitor()
    {
        var processName = Process.GetCurrentProcess().ProcessName;

        _privateBytes = new PerformanceCounter(
            ".NET CLR Memory", "# Bytes in all Heaps", processName);

        _gen0Collections = new PerformanceCounter(
            ".NET CLR Memory", "# Gen 0 Collections", processName);

        _gen1Collections = new PerformanceCounter(
            ".NET CLR Memory", "# Gen 1 Collections", processName);

        _gen2Collections = new PerformanceCounter(
            ".NET CLR Memory", "# Gen 2 Collections", processName);
    }

    public MemoryStats GetStats()
    {
        return new MemoryStats
        {
            TotalMemoryBytes = _privateBytes.NextValue(),
            Gen0Collections = (int)_gen0Collections.NextValue(),
            Gen1Collections = (int)_gen1Collections.NextValue(),
            Gen2Collections = (int)_gen2Collections.NextValue(),
            ManagedMemoryMB = GC.GetTotalMemory(false) / 1024.0 / 1024.0
        };
    }
}

public class MemoryStats
{
    public float TotalMemoryBytes { get; set; }
    public int Gen0Collections { get; set; }
    public int Gen1Collections { get; set; }
    public int Gen2Collections { get; set; }
    public double ManagedMemoryMB { get; set; }
}
```

### Diagnostic Tools

```csharp
public class MemoryDiagnostics
{
    private Timer _monitorTimer;
    private const long THRESHOLD_MB = 500;

    public void StartMonitoring()
    {
        _monitorTimer = new Timer(CheckMemory, null, 0, 10000); // Every 10s
    }

    private void CheckMemory(object state)
    {
        var memoryMB = GC.GetTotalMemory(false) / 1024.0 / 1024.0;

        if (memoryMB > THRESHOLD_MB)
        {
            LogWarning($"High memory usage: {memoryMB:F2} MB");

            // Optional: Take heap snapshot
            TakeHeapSnapshot();
        }
    }

    private void TakeHeapSnapshot()
    {
        // Log generation sizes
        var gen0 = GC.CollectionCount(0);
        var gen1 = GC.CollectionCount(1);
        var gen2 = GC.CollectionCount(2);

        Debug.WriteLine($"GC Collections - Gen0: {gen0}, Gen1: {gen1}, Gen2: {gen2}");
    }

    private void LogWarning(string message)
    {
        // Log to file or monitoring service
        Debug.WriteLine($"[MEMORY WARNING] {message}");
    }
}
```

---

## Memory Leak Checklist

Use this checklist during code reviews:

- [ ] All IDisposable objects disposed (using statements)
- [ ] Event handlers unsubscribed in Dispose
- [ ] No static references to forms or controls
- [ ] Timers disposed properly
- [ ] Images and bitmaps disposed
- [ ] BindingSource and data-binding objects disposed
- [ ] No unbounded caches (implement size limits)
- [ ] Weak references used for optional caches
- [ ] No circular references preventing GC
- [ ] Resources nulled out after disposal
- [ ] No finalizers (use IDispospose instead)
- [ ] Thread-safe disposal for multi-threaded scenarios
- [ ] Memory profiling performed on long-running scenarios
- [ ] No keeping references to closed forms

---

## Troubleshooting

### "OutOfMemoryException" Despite Available Memory

**Cause**: Large Object Heap fragmentation.

**Solution**:
```csharp
// Enable LOH compaction
GCSettings.LargeObjectHeapCompactionMode = GCLargeObjectHeapCompactionMode.CompactOnce;
GC.Collect();

// Or use ArrayPool to reuse large buffers
var buffer = ArrayPool<byte>.Shared.Rent(100_000);
```

### Memory Grows Over Time

**Cause**: Memory leak (event handlers, static references, caches).

**Solution**:
1. Profile with Visual Studio Memory Profiler
2. Take before/after snapshots
3. Find retention paths
4. Fix root cause (unsubscribe, weak references, cache limits)

### Frequent Gen 2 Collections

**Cause**: Long-lived objects being collected frequently.

**Solution**:
- Reduce object lifetimes
- Use object pooling
- Avoid promoting objects to Gen 2

### High Memory Usage in UI

**Cause**: Loading too many images, controls, or data at once.

**Solution**:
```csharp
// Virtualize lists
dataGridView.VirtualMode = true;

// Load images on demand
pictureBox.LoadAsync(imagePath);

// Use pagination
LoadPage(pageIndex, pageSize);
```

---

## Related Topics

- **[Resource Management](resource-management.md)** - Disposing resources correctly
- **[Performance Optimization](performance.md)** - General performance tips
- **[DataGridView Best Practices](../ui-ux/datagridview-practices.md)** - Virtual mode for large datasets
- **[Async/Await Pattern](async-await.md)** - Async resource management

---

**Key Takeaways**:
- .NET GC is automatic but not magic - you must dispose unmanaged resources
- Event handlers are the #1 cause of WinForms memory leaks
- Always unsubscribe from events in Dispose
- Profile regularly to catch leaks early
- Use weak references for caches where appropriate
- Implement IDisposable correctly for classes holding resources
- Monitor memory usage in production environments
