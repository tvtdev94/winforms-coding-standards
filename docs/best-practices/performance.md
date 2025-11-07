# Performance Optimization in WinForms

## 📋 Overview

Performance optimization in WinForms applications is crucial for delivering responsive, smooth user experiences. WinForms applications face unique challenges including UI thread blocking, inefficient rendering, memory leaks, and slow data operations. This guide covers proven techniques to optimize your WinForms applications across UI rendering, data operations, memory management, and more.

**Key Performance Areas**:
- **UI Rendering**: Layout operations, double buffering, control creation
- **Data Operations**: DataGridView optimization, data binding, list operations
- **Memory Management**: Object pooling, proper disposal, collection selection
- **Async Operations**: Responsive UI, parallel processing, caching
- **Database Performance**: Connection pooling, efficient queries, batch operations

---

## 🎯 Why This Matters

**User Experience**:
- Responsive UI prevents frustration and improves user satisfaction
- Smooth scrolling and animations enhance perceived quality
- Fast startup times create positive first impressions

**Scalability**:
- Handle larger datasets without performance degradation
- Support more concurrent operations efficiently
- Reduce resource consumption per user

**Resource Usage**:
- Lower memory footprint reduces system requirements
- Efficient CPU usage extends battery life on laptops
- Better resource management prevents crashes and slowdowns

**Business Impact**:
- Improved productivity for users
- Reduced support costs from performance complaints
- Better application ratings and adoption

---

## UI Rendering Performance

### SuspendLayout/ResumeLayout

**What It Does**:
`SuspendLayout()` and `ResumeLayout()` prevent the form from recalculating control positions during bulk operations. Each control addition or property change normally triggers a layout calculation, causing visible flickering and performance degradation.

**When to Use**:
- Adding multiple controls to a container
- Changing properties of multiple controls
- Programmatically creating complex UI structures
- Bulk configuration during form initialization

**Examples**:

❌ **Slow - Layout calculated 100 times**:
```csharp
public void AddControlsSlow()
{
    for (int i = 0; i < 100; i++)
    {
        var label = new Label
        {
            Text = $"Label {i}",
            Location = new Point(10, i * 25),
            AutoSize = true
        };
        panel1.Controls.Add(label); // Layout recalculated each time
    }
}
```

✅ **Fast - Layout calculated once**:
```csharp
public void AddControlsFast()
{
    panel1.SuspendLayout();
    try
    {
        for (int i = 0; i < 100; i++)
        {
            var label = new Label
            {
                Text = $"Label {i}",
                Location = new Point(10, i * 25),
                AutoSize = true
            };
            panel1.Controls.Add(label);
        }
    }
    finally
    {
        panel1.ResumeLayout(performLayout: true); // Single layout calculation
    }
}
```

**Nested Suspend Example**:
```csharp
public void ConfigureComplexForm()
{
    this.SuspendLayout();
    try
    {
        // Configure main form
        panel1.SuspendLayout();
        panel2.SuspendLayout();
        try
        {
            // Add controls to panels
            AddControlsToPanel1();
            AddControlsToPanel2();
        }
        finally
        {
            panel1.ResumeLayout(false);
            panel2.ResumeLayout(false);
        }
    }
    finally
    {
        this.ResumeLayout(true); // Final layout of entire form
    }
}
```

### Double Buffering

**Reducing Flicker**:
Double buffering eliminates screen flicker by rendering to an off-screen buffer before displaying. This is essential for smooth custom painting and animations.

**Enabling Double Buffering**:

✅ **For Forms**:
```csharp
public class OptimizedForm : Form
{
    public OptimizedForm()
    {
        // Enable double buffering for the form
        this.DoubleBuffered = true;

        // Alternative using SetStyle
        this.SetStyle(
            ControlStyles.AllPaintingInWmPaint |
            ControlStyles.UserPaint |
            ControlStyles.OptimizedDoubleBuffer,
            true);

        this.UpdateStyles();
    }
}
```

✅ **For Custom Controls**:
```csharp
public class FastCustomControl : Control
{
    public FastCustomControl()
    {
        // Enable double buffering for custom control
        this.SetStyle(
            ControlStyles.AllPaintingInWmPaint |
            ControlStyles.UserPaint |
            ControlStyles.OptimizedDoubleBuffer |
            ControlStyles.ResizeRedraw,
            true);

        this.UpdateStyles();
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        base.OnPaint(e);

        // Custom painting code here
        // Rendering happens to buffer, then displayed
        using (var brush = new SolidBrush(Color.Blue))
        {
            e.Graphics.FillRectangle(brush, ClientRectangle);
        }
    }
}
```

### Minimize Repaints

**Control Invalidation**:
Only invalidate the specific regions that need repainting rather than the entire control.

❌ **Slow - Repaints entire control**:
```csharp
public void UpdateProgressBar()
{
    _progressValue += 10;
    this.Invalidate(); // Repaints entire form
}
```

✅ **Fast - Repaints only changed region**:
```csharp
public void UpdateProgressBar()
{
    var oldRect = GetProgressRectangle(_progressValue);
    _progressValue += 10;
    var newRect = GetProgressRectangle(_progressValue);

    // Only invalidate the changed area
    var updateRect = Rectangle.Union(oldRect, newRect);
    this.Invalidate(updateRect);
}

private Rectangle GetProgressRectangle(int value)
{
    return new Rectangle(10, 10, value * 2, 20);
}
```

**Update vs Refresh**:
- `Invalidate()`: Marks region for repainting (asynchronous)
- `Update()`: Forces immediate processing of invalidated regions
- `Refresh()`: Calls `Invalidate()` + `Update()` (expensive!)

✅ **Efficient repainting**:
```csharp
// Batch multiple changes, then update once
public void UpdateMultipleRegions()
{
    this.SuspendLayout();
    try
    {
        region1.Invalidate();
        region2.Invalidate();
        region3.Invalidate();
    }
    finally
    {
        this.ResumeLayout();
        this.Update(); // Process all invalidations at once
    }
}
```

### Control Creation

**Lazy Loading Controls**:
Defer control creation until needed to improve startup time.

❌ **Slow - Creates all tabs at startup**:
```csharp
public class DashboardForm : Form
{
    public DashboardForm()
    {
        InitializeComponent();

        // Creates all expensive controls immediately
        CreateCustomersTab();
        CreateOrdersTab();
        CreateReportsTab();
        CreateSettingsTab();
    }
}
```

✅ **Fast - Creates tabs on demand**:
```csharp
public class DashboardForm : Form
{
    private bool _customersTabLoaded = false;
    private bool _ordersTabLoaded = false;

    public DashboardForm()
    {
        InitializeComponent();

        tabControl.SelectedIndexChanged += TabControl_SelectedIndexChanged;
    }

    private void TabControl_SelectedIndexChanged(object sender, EventArgs e)
    {
        switch (tabControl.SelectedIndex)
        {
            case 0:
                if (!_customersTabLoaded)
                {
                    CreateCustomersTab();
                    _customersTabLoaded = true;
                }
                break;
            case 1:
                if (!_ordersTabLoaded)
                {
                    CreateOrdersTab();
                    _ordersTabLoaded = true;
                }
                break;
        }
    }
}
```

**Control Pooling/Reusing**:
Reuse controls instead of creating new ones repeatedly.

✅ **Control pool implementation**:
```csharp
public class ControlPool<T> where T : Control, new()
{
    private readonly Stack<T> _pool = new Stack<T>();
    private readonly Container _container;

    public ControlPool(Container container, int initialSize = 10)
    {
        _container = container;
        for (int i = 0; i < initialSize; i++)
        {
            _pool.Push(CreateControl());
        }
    }

    private T CreateControl()
    {
        var control = new T { Visible = false };
        _container.Controls.Add(control);
        return control;
    }

    public T Get()
    {
        if (_pool.Count > 0)
            return _pool.Pop();
        return CreateControl();
    }

    public void Return(T control)
    {
        control.Visible = false;
        _pool.Push(control);
    }
}
```

**Creating vs Hiding**:
For controls used repeatedly, hiding is faster than creating/disposing.

✅ **Reuse by hiding**:
```csharp
public class DialogManager
{
    private ProgressDialog _progressDialog;

    public void ShowProgress(string message)
    {
        if (_progressDialog == null)
        {
            _progressDialog = new ProgressDialog();
        }

        _progressDialog.Message = message;
        _progressDialog.Show(); // Reuse existing dialog
    }

    public void HideProgress()
    {
        _progressDialog?.Hide(); // Hide instead of dispose
    }
}
```

---

## Data Operations

### DataGridView Optimization

**Virtual Mode for Large Datasets**:
Virtual mode loads data on-demand, dramatically improving performance with large datasets.

✅ **Virtual mode implementation**:
```csharp
public class FastDataGridView : Form
{
    private List<Customer> _customers;
    private DataGridView dgv;

    public FastDataGridView()
    {
        InitializeComponent();

        // Enable virtual mode
        dgv.VirtualMode = true;
        dgv.RowCount = 100000; // Total rows

        // Handle cell value requests
        dgv.CellValueNeeded += Dgv_CellValueNeeded;
    }

    private void Dgv_CellValueNeeded(object sender, DataGridViewCellValueEventArgs e)
    {
        // Only load data for visible cells
        if (e.RowIndex < _customers.Count)
        {
            var customer = _customers[e.RowIndex];
            e.Value = e.ColumnIndex switch
            {
                0 => customer.Id,
                1 => customer.Name,
                2 => customer.Email,
                _ => null
            };
        }
    }
}
```

**BeginUpdate/EndUpdate Alternative**:
DataGridView doesn't have BeginUpdate, so suspend binding instead.

❌ **Slow - Updates after each row**:
```csharp
public void LoadDataSlow()
{
    dgv.DataSource = null;
    var list = new BindingList<Customer>();
    dgv.DataSource = list;

    foreach (var customer in GetCustomers())
    {
        list.Add(customer); // UI updates after each add
    }
}
```

✅ **Fast - Batch updates**:
```csharp
public void LoadDataFast()
{
    dgv.SuspendLayout();
    try
    {
        var list = new BindingList<Customer>();

        // Add all items while not bound
        foreach (var customer in GetCustomers())
        {
            list.Add(customer);
        }

        // Bind once with all data
        dgv.DataSource = list;
    }
    finally
    {
        dgv.ResumeLayout();
    }
}
```

**Cell Value Caching**:
```csharp
public class CachedDataGridView : DataGridView
{
    private readonly Dictionary<(int row, int col), object> _cache
        = new Dictionary<(int row, int col), object>();

    protected override void OnCellValueNeeded(DataGridViewCellValueEventArgs e)
    {
        var key = (e.RowIndex, e.ColumnIndex);

        if (!_cache.TryGetValue(key, out var value))
        {
            value = LoadCellValue(e.RowIndex, e.ColumnIndex);
            _cache[key] = value;
        }

        e.Value = value;
        base.OnCellValueNeeded(e);
    }

    public void ClearCache()
    {
        _cache.Clear();
    }
}
```

### Efficient Data Binding

**BindingList<T> vs List<T>**:

❌ **Slow - List<T> requires manual refresh**:
```csharp
public void UpdateWithList()
{
    var list = new List<Customer>();
    dgv.DataSource = list;

    list.Add(new Customer { Name = "John" });
    // Must rebind to see changes
    dgv.DataSource = null;
    dgv.DataSource = list;
}
```

✅ **Fast - BindingList<T> auto-updates**:
```csharp
public void UpdateWithBindingList()
{
    var list = new BindingList<Customer>();
    dgv.DataSource = list;

    list.Add(new Customer { Name = "John" }); // UI updates automatically
}
```

**SuspendBinding/ResumeBinding**:
```csharp
public class BindingHelper
{
    public static void UpdateBatch<T>(BindingList<T> list, IEnumerable<T> items)
    {
        // Disable change notifications
        list.RaiseListChangedEvents = false;
        try
        {
            foreach (var item in items)
            {
                list.Add(item);
            }
        }
        finally
        {
            // Re-enable and trigger single update
            list.RaiseListChangedEvents = true;
            list.ResetBindings();
        }
    }
}
```

### List Operations

**BeginUpdate/EndUpdate for ListBox/ComboBox**:

❌ **Slow**:
```csharp
public void LoadComboBoxSlow()
{
    comboBox1.Items.Clear();
    for (int i = 0; i < 10000; i++)
    {
        comboBox1.Items.Add($"Item {i}"); // Redraws each time
    }
}
```

✅ **Fast**:
```csharp
public void LoadComboBoxFast()
{
    comboBox1.BeginUpdate();
    try
    {
        comboBox1.Items.Clear();
        for (int i = 0; i < 10000; i++)
        {
            comboBox1.Items.Add($"Item {i}");
        }
    }
    finally
    {
        comboBox1.EndUpdate(); // Single redraw
    }
}
```

**Item Collection Optimization**:
```csharp
public void LoadComboBoxOptimal()
{
    var items = new object[10000];
    for (int i = 0; i < items.Length; i++)
    {
        items[i] = $"Item {i}";
    }

    comboBox1.BeginUpdate();
    try
    {
        comboBox1.Items.Clear();
        comboBox1.Items.AddRange(items); // Faster than individual adds
    }
    finally
    {
        comboBox1.EndUpdate();
    }
}
```

---

## Image Handling

### Image Loading

**Lazy Loading**:
```csharp
public class ImageGallery : Form
{
    private readonly Dictionary<string, Image> _imageCache
        = new Dictionary<string, Image>();

    private Image GetImage(string path)
    {
        if (!_imageCache.ContainsKey(path))
        {
            _imageCache[path] = Image.FromFile(path);
        }
        return _imageCache[path];
    }

    private void LoadImageLazy(PictureBox pictureBox, string path)
    {
        // Only load when needed
        if (!pictureBox.Visible) return;

        pictureBox.Image = GetImage(path);
    }
}
```

**Async Loading**:
```csharp
public async Task LoadImageAsync(PictureBox pictureBox, string url)
{
    using (var client = new HttpClient())
    {
        var imageBytes = await client.GetByteArrayAsync(url);
        using (var ms = new MemoryStream(imageBytes))
        {
            pictureBox.Image = Image.FromStream(ms);
        }
    }
}
```

**Image Caching**:
```csharp
public class ImageCache : IDisposable
{
    private readonly Dictionary<string, Image> _cache
        = new Dictionary<string, Image>();
    private readonly int _maxCacheSize;

    public ImageCache(int maxCacheSize = 100)
    {
        _maxCacheSize = maxCacheSize;
    }

    public Image GetImage(string path)
    {
        if (_cache.TryGetValue(path, out var cachedImage))
            return cachedImage;

        if (_cache.Count >= _maxCacheSize)
        {
            // Remove oldest entry
            var oldest = _cache.First();
            oldest.Value.Dispose();
            _cache.Remove(oldest.Key);
        }

        var image = Image.FromFile(path);
        _cache[path] = image;
        return image;
    }

    public void Dispose()
    {
        foreach (var image in _cache.Values)
        {
            image.Dispose();
        }
        _cache.Clear();
    }
}
```

**Thumbnail Generation**:
```csharp
public Image CreateThumbnail(string imagePath, int width, int height)
{
    using (var original = Image.FromFile(imagePath))
    {
        return original.GetThumbnailImage(width, height, null, IntPtr.Zero);
    }
}
```

### Image Disposal

**Proper Disposal**:

❌ **Memory leak**:
```csharp
private void LoadImage()
{
    pictureBox1.Image = Image.FromFile("photo.jpg"); // Previous image not disposed
}
```

✅ **Correct disposal**:
```csharp
private void LoadImage()
{
    // Dispose previous image
    var oldImage = pictureBox1.Image;
    pictureBox1.Image = null;
    oldImage?.Dispose();

    // Load new image
    pictureBox1.Image = Image.FromFile("photo.jpg");
}
```

### PictureBox Optimization

**SizeMode Options**:
```csharp
// Normal - No scaling (fastest)
pictureBox1.SizeMode = PictureBoxSizeMode.Normal;

// StretchImage - Scales to fit (slowest if resized frequently)
pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;

// Zoom - Maintains aspect ratio (good balance)
pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;

// AutoSize - Resizes control to image (no scaling overhead)
pictureBox1.SizeMode = PictureBoxSizeMode.AutoSize;
```

**Image Scaling**:
✅ **Pre-scale images**:
```csharp
public Image ScaleImage(Image source, int width, int height)
{
    var scaled = new Bitmap(width, height);
    using (var graphics = Graphics.FromImage(scaled))
    {
        graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
        graphics.DrawImage(source, 0, 0, width, height);
    }
    return scaled;
}

// Use pre-scaled image
pictureBox1.Image = ScaleImage(originalImage, 200, 200);
pictureBox1.SizeMode = PictureBoxSizeMode.Normal; // No runtime scaling
```

---

## Memory Optimization

### Object Pooling

**Reusing Expensive Objects**:
```csharp
public class ObjectPool<T> where T : class, new()
{
    private readonly ConcurrentBag<T> _objects = new ConcurrentBag<T>();
    private readonly Func<T> _objectGenerator;

    public ObjectPool(Func<T> objectGenerator)
    {
        _objectGenerator = objectGenerator ?? (() => new T());
    }

    public T Get()
    {
        return _objects.TryTake(out T item) ? item : _objectGenerator();
    }

    public void Return(T item)
    {
        _objects.Add(item);
    }
}

// Usage
private static ObjectPool<StringBuilder> _stringBuilderPool
    = new ObjectPool<StringBuilder>(() => new StringBuilder(1024));

public string BuildString()
{
    var sb = _stringBuilderPool.Get();
    try
    {
        sb.Clear();
        sb.Append("Hello ");
        sb.Append("World");
        return sb.ToString();
    }
    finally
    {
        _stringBuilderPool.Return(sb);
    }
}
```

### String Operations

**StringBuilder for Concatenation**:

❌ **Slow - Creates many string objects**:
```csharp
public string BuildReportSlow(List<string> lines)
{
    string result = "";
    foreach (var line in lines)
    {
        result += line + Environment.NewLine; // New string each iteration
    }
    return result;
}
```

✅ **Fast - Single string object**:
```csharp
public string BuildReportFast(List<string> lines)
{
    var sb = new StringBuilder(lines.Count * 50); // Pre-allocate
    foreach (var line in lines)
    {
        sb.AppendLine(line);
    }
    return sb.ToString();
}
```

**String Interning**:
```csharp
// Use string interning for repeated strings
public class StatusManager
{
    private static readonly string StatusActive = string.Intern("Active");
    private static readonly string StatusInactive = string.Intern("Inactive");

    public string GetStatus(bool isActive)
    {
        return isActive ? StatusActive : StatusInactive; // Reuses same reference
    }
}
```

### Collection Selection

**List<T> vs LinkedList<T>**:
```csharp
// Use List<T> for random access and iterations
var list = new List<Customer>(capacity: 1000); // Pre-allocate if size known

// Use LinkedList<T> for frequent insertions/deletions in middle
var linkedList = new LinkedList<Customer>();
```

**Dictionary<T> vs HashSet<T>**:
```csharp
// Dictionary for key-value lookups
var customerDict = new Dictionary<int, Customer>(capacity: 1000);

// HashSet for unique values and existence checks
var processedIds = new HashSet<int>();
if (!processedIds.Contains(id))
{
    // Process and add
    processedIds.Add(id);
}
```

**Capacity Pre-allocation**:

❌ **Slow - Multiple reallocations**:
```csharp
var list = new List<Customer>(); // Default capacity: 4
for (int i = 0; i < 10000; i++)
{
    list.Add(new Customer()); // Reallocates at 4, 8, 16, 32, 64...
}
```

✅ **Fast - Single allocation**:
```csharp
var list = new List<Customer>(capacity: 10000); // Pre-allocate
for (int i = 0; i < 10000; i++)
{
    list.Add(new Customer()); // No reallocation needed
}
```

---

## Async Operations

### Async Data Loading

**Keep UI Responsive**:
```csharp
public async Task LoadCustomersAsync()
{
    btnLoad.Enabled = false;
    progressBar1.Visible = true;

    try
    {
        var customers = await _customerService.GetAllAsync();

        // Update UI on UI thread
        dgvCustomers.DataSource = customers;
    }
    catch (Exception ex)
    {
        MessageBox.Show($"Error loading customers: {ex.Message}");
    }
    finally
    {
        btnLoad.Enabled = true;
        progressBar1.Visible = false;
    }
}
```

**Progress Indication**:
```csharp
public async Task LoadLargeDatasetAsync()
{
    var progress = new Progress<int>(percent =>
    {
        progressBar1.Value = percent;
        lblStatus.Text = $"Loading... {percent}%";
    });

    await _dataService.LoadDataAsync(progress);
}

// In service
public async Task LoadDataAsync(IProgress<int> progress)
{
    var items = await GetItemsAsync();
    for (int i = 0; i < items.Count; i++)
    {
        await ProcessItemAsync(items[i]);
        progress?.Report((i + 1) * 100 / items.Count);
    }
}
```

### Parallel Processing

**Task.WhenAll for Parallel Operations**:
```csharp
public async Task LoadMultipleDataSourcesAsync()
{
    var customersTask = _customerService.GetAllAsync();
    var ordersTask = _orderService.GetAllAsync();
    var productsTask = _productService.GetAllAsync();

    // Run in parallel
    await Task.WhenAll(customersTask, ordersTask, productsTask);

    // All tasks completed
    dgvCustomers.DataSource = customersTask.Result;
    dgvOrders.DataSource = ordersTask.Result;
    dgvProducts.DataSource = productsTask.Result;
}
```

**Degree of Parallelism**:
```csharp
public async Task ProcessImagesAsync(List<string> imagePaths)
{
    var options = new ParallelOptions
    {
        MaxDegreeOfParallelism = Environment.ProcessorCount
    };

    await Task.Run(() =>
    {
        Parallel.ForEach(imagePaths, options, imagePath =>
        {
            ProcessImage(imagePath);
        });
    });
}
```

### Caching Strategies

**Memory Cache**:
```csharp
public class CachedDataService
{
    private readonly MemoryCache _cache = MemoryCache.Default;
    private readonly TimeSpan _cacheExpiration = TimeSpan.FromMinutes(5);

    public async Task<List<Customer>> GetCustomersAsync()
    {
        const string cacheKey = "AllCustomers";

        if (_cache.Get(cacheKey) is List<Customer> cached)
        {
            return cached;
        }

        var customers = await _repository.GetAllAsync();

        var policy = new CacheItemPolicy
        {
            AbsoluteExpiration = DateTimeOffset.Now.Add(_cacheExpiration)
        };

        _cache.Set(cacheKey, customers, policy);
        return customers;
    }

    public void InvalidateCache(string key)
    {
        _cache.Remove(key);
    }
}
```

**Time-based Expiration**:
```csharp
public class TimedCache<TKey, TValue>
{
    private readonly Dictionary<TKey, (TValue value, DateTime expiry)> _cache
        = new Dictionary<TKey, (TValue, DateTime)>();
    private readonly TimeSpan _defaultExpiration;

    public TimedCache(TimeSpan defaultExpiration)
    {
        _defaultExpiration = defaultExpiration;
    }

    public bool TryGet(TKey key, out TValue value)
    {
        if (_cache.TryGetValue(key, out var cached))
        {
            if (cached.expiry > DateTime.Now)
            {
                value = cached.value;
                return true;
            }
            _cache.Remove(key); // Expired
        }
        value = default;
        return false;
    }

    public void Set(TKey key, TValue value)
    {
        _cache[key] = (value, DateTime.Now.Add(_defaultExpiration));
    }
}
```

---

## Startup Performance

### Lazy Initialization

**Defer Expensive Operations**:
```csharp
public class MainForm : Form
{
    private Lazy<CustomerService> _customerService;
    private Lazy<ReportEngine> _reportEngine;

    public MainForm()
    {
        InitializeComponent();

        // Defer creation until first use
        _customerService = new Lazy<CustomerService>(() =>
            new CustomerService(new CustomerRepository()));

        _reportEngine = new Lazy<ReportEngine>(() =>
            new ReportEngine(_customerService.Value));
    }

    private void btnCustomers_Click(object sender, EventArgs e)
    {
        // Created on first access
        var customers = _customerService.Value.GetAll();
    }
}
```

### Splash Screens

**Showing Progress**:
```csharp
public class Program
{
    [STAThread]
    static void Main()
    {
        Application.EnableVisualStyles();
        Application.SetCompatibleTextRenderingDefault(false);

        var splash = new SplashScreen();
        splash.Show();
        Application.DoEvents();

        // Background initialization
        InitializeApplication(splash);

        splash.Close();
        Application.Run(new MainForm());
    }

    static void InitializeApplication(SplashScreen splash)
    {
        splash.UpdateStatus("Loading configuration...");
        ConfigManager.Initialize();

        splash.UpdateStatus("Connecting to database...");
        DatabaseContext.Initialize();

        splash.UpdateStatus("Loading plugins...");
        PluginManager.LoadPlugins();
    }
}
```

### Module Loading

**Load Assemblies on Demand**:
```csharp
public class PluginManager
{
    private static Dictionary<string, Assembly> _loadedPlugins
        = new Dictionary<string, Assembly>();

    public static object GetPlugin(string pluginName)
    {
        if (!_loadedPlugins.ContainsKey(pluginName))
        {
            var path = Path.Combine("Plugins", $"{pluginName}.dll");
            _loadedPlugins[pluginName] = Assembly.LoadFrom(path);
        }

        var assembly = _loadedPlugins[pluginName];
        var type = assembly.GetType($"{pluginName}.Plugin");
        return Activator.CreateInstance(type);
    }
}
```

---

## Database Performance

### Connection Pooling

**How It Works**:
Connection pooling reuses database connections instead of creating new ones, significantly reducing overhead.

**Configuration**:
```csharp
// Connection string with pooling enabled (default)
const string connectionString =
    "Server=localhost;Database=MyDb;User Id=user;Password=pass;" +
    "Pooling=true;" +          // Enable pooling (default: true)
    "Min Pool Size=5;" +       // Minimum connections
    "Max Pool Size=100;" +     // Maximum connections
    "Connection Lifetime=0;";  // 0 = no limit

// Use with using to return to pool quickly
using (var connection = new SqlConnection(connectionString))
{
    await connection.OpenAsync();
    // Execute commands
} // Connection returned to pool, not closed
```

### Efficient Queries

**Select Only Needed Columns**:

❌ **Slow - Retrieves unnecessary data**:
```csharp
public async Task<List<CustomerDto>> GetCustomersSlow()
{
    return await _context.Customers
        .ToListAsync(); // Retrieves all columns
}
```

✅ **Fast - Retrieves only needed columns**:
```csharp
public async Task<List<CustomerDto>> GetCustomersFast()
{
    return await _context.Customers
        .Select(c => new CustomerDto
        {
            Id = c.Id,
            Name = c.Name,
            Email = c.Email
        })
        .ToListAsync();
}
```

**Pagination**:
```csharp
public async Task<PagedResult<Customer>> GetCustomersPagedAsync(
    int pageNumber, int pageSize)
{
    var total = await _context.Customers.CountAsync();

    var items = await _context.Customers
        .OrderBy(c => c.Name)
        .Skip((pageNumber - 1) * pageSize)
        .Take(pageSize)
        .ToListAsync();

    return new PagedResult<Customer>
    {
        Items = items,
        TotalCount = total,
        PageNumber = pageNumber,
        PageSize = pageSize
    };
}
```

**Compiled Queries**:
```csharp
// Define compiled query once
private static readonly Func<MyDbContext, int, Task<Customer>>
    GetCustomerByIdCompiled = EF.CompileAsyncQuery(
        (MyDbContext context, int id) =>
            context.Customers.FirstOrDefault(c => c.Id == id));

// Reuse compiled query (faster than dynamic LINQ)
public async Task<Customer> GetCustomerByIdAsync(int id)
{
    return await GetCustomerByIdCompiled(_context, id);
}
```

### Batch Operations

**Bulk Inserts**:

❌ **Slow - Individual inserts**:
```csharp
public async Task SaveCustomersSlow(List<Customer> customers)
{
    foreach (var customer in customers)
    {
        _context.Customers.Add(customer);
        await _context.SaveChangesAsync(); // Database roundtrip each time
    }
}
```

✅ **Fast - Batch insert**:
```csharp
public async Task SaveCustomersFast(List<Customer> customers)
{
    _context.Customers.AddRange(customers);
    await _context.SaveChangesAsync(); // Single database roundtrip
}
```

**Batch Updates**:
```csharp
public async Task UpdateCustomerStatusAsync(List<int> customerIds, string status)
{
    await _context.Customers
        .Where(c => customerIds.Contains(c.Id))
        .ExecuteUpdateAsync(s => s.SetProperty(c => c.Status, status));
    // Single SQL UPDATE statement for all records
}
```

---

## Best Practices

### ✅ DO

1. **Use SuspendLayout/ResumeLayout for bulk UI changes**
   ```csharp
   panel.SuspendLayout();
   try { /* Add controls */ }
   finally { panel.ResumeLayout(true); }
   ```

2. **Enable double buffering for custom painting**
   ```csharp
   this.DoubleBuffered = true;
   ```

3. **Invalidate only changed regions**
   ```csharp
   this.Invalidate(changedRect);
   ```

4. **Use virtual mode for large DataGridViews (>10,000 rows)**
   ```csharp
   dgv.VirtualMode = true;
   ```

5. **Pre-allocate collection capacity when size is known**
   ```csharp
   var list = new List<Customer>(capacity: 10000);
   ```

6. **Use StringBuilder for string concatenation in loops**
   ```csharp
   var sb = new StringBuilder(estimatedSize);
   ```

7. **Dispose images properly**
   ```csharp
   oldImage?.Dispose();
   pictureBox.Image = newImage;
   ```

8. **Use async/await for I/O operations**
   ```csharp
   await LoadDataAsync();
   ```

9. **Implement caching for expensive operations**
   ```csharp
   if (_cache.TryGet(key, out var value)) return value;
   ```

10. **Use BeginUpdate/EndUpdate for ListBox/ComboBox**
    ```csharp
    listBox.BeginUpdate();
    try { /* Add items */ }
    finally { listBox.EndUpdate(); }
    ```

11. **Lazy load controls and data**
    ```csharp
    var service = new Lazy<Service>(() => new Service());
    ```

12. **Use object pooling for frequently created objects**
    ```csharp
    var obj = pool.Get();
    try { /* Use object */ }
    finally { pool.Return(obj); }
    ```

13. **Batch database operations**
    ```csharp
    context.AddRange(items);
    await context.SaveChangesAsync();
    ```

14. **Use connection pooling**
    ```csharp
    using (var conn = new SqlConnection(pooledConnectionString))
    ```

15. **Profile before optimizing**
    ```csharp
    var sw = Stopwatch.StartNew();
    // Operation
    sw.Stop();
    _logger.LogInformation($"Took {sw.ElapsedMilliseconds}ms");
    ```

### ❌ DON'T

1. **Don't call Refresh() unnecessarily**
   ```csharp
   ❌ this.Refresh(); // Forces immediate repaint
   ✅ this.Invalidate(); // Queues repaint
   ```

2. **Don't create controls repeatedly**
   ```csharp
   ❌ var dialog = new Dialog(); // Each time
   ✅ _dialog ??= new Dialog(); // Reuse
   ```

3. **Don't use List<T> for data binding without BindingList<T>**
   ```csharp
   ❌ dgv.DataSource = new List<Customer>();
   ✅ dgv.DataSource = new BindingList<Customer>();
   ```

4. **Don't concatenate strings in loops**
   ```csharp
   ❌ str += item; // Creates new string each time
   ✅ sb.Append(item); // Modifies buffer
   ```

5. **Don't forget to dispose images**
   ```csharp
   ❌ pictureBox.Image = Image.FromFile(path); // Leak
   ✅ oldImage?.Dispose(); pictureBox.Image = newImage;
   ```

6. **Don't load all data at once (use pagination)**
   ```csharp
   ❌ var all = context.Customers.ToList(); // 1M rows
   ✅ var page = context.Customers.Skip(0).Take(100).ToList();
   ```

7. **Don't use synchronous I/O on UI thread**
   ```csharp
   ❌ var data = File.ReadAllText(path);
   ✅ var data = await File.ReadAllTextAsync(path);
   ```

8. **Don't save changes in loops**
   ```csharp
   ❌ foreach (var item in items) { context.Add(item); context.SaveChanges(); }
   ✅ context.AddRange(items); context.SaveChanges();
   ```

9. **Don't ignore capacity for collections**
   ```csharp
   ❌ var list = new List<Customer>(); // Reallocates
   ✅ var list = new List<Customer>(knownSize);
   ```

10. **Don't create bitmaps without disposing**
    ```csharp
    ❌ var bmp = new Bitmap(100, 100); // Leak
    ✅ using var bmp = new Bitmap(100, 100);
    ```

11. **Don't disable connection pooling**
    ```csharp
    ❌ "...Pooling=false;" // Slow
    ✅ "...Pooling=true;" // Fast
    ```

12. **Don't select all columns when you need few**
    ```csharp
    ❌ var all = context.Customers.ToList();
    ✅ var names = context.Customers.Select(c => c.Name).ToList();
    ```

---

## Performance Profiling

### Tools

**Visual Studio Profiler**:
- Built-in CPU and memory profiling
- Performance Explorer window
- PerfTips for inline performance data

**dotMemory** (JetBrains):
- Memory profiling and leak detection
- Snapshot comparison
- Object allocation tracking

**dotTrace** (JetBrains):
- Timeline profiling
- Sampling and tracing modes
- Call tree analysis

**PerfView** (Microsoft):
- Free ETW-based profiler
- CPU, memory, GC analysis
- Advanced performance diagnostics

### Profiling Techniques

**CPU Profiling**:
```csharp
// Measure method execution time
var stopwatch = Stopwatch.StartNew();
ExpensiveOperation();
stopwatch.Stop();
Console.WriteLine($"Took: {stopwatch.ElapsedMilliseconds}ms");
```

**Memory Profiling**:
```csharp
// Track memory allocation
var before = GC.GetTotalMemory(forceFullCollection: true);
CreateManyObjects();
var after = GC.GetTotalMemory(forceFullCollection: true);
Console.WriteLine($"Allocated: {(after - before) / 1024 / 1024}MB");
```

**Finding Bottlenecks**:
1. Use profiler to identify slow methods
2. Focus on methods with highest exclusive time
3. Check for unexpected allocations
4. Look for excessive GC collections

### Measurement

**Stopwatch for Timing**:
```csharp
public class PerformanceLogger
{
    public static async Task<T> MeasureAsync<T>(
        string operationName,
        Func<Task<T>> operation)
    {
        var sw = Stopwatch.StartNew();
        try
        {
            return await operation();
        }
        finally
        {
            sw.Stop();
            Console.WriteLine($"{operationName}: {sw.ElapsedMilliseconds}ms");
        }
    }
}

// Usage
var customers = await PerformanceLogger.MeasureAsync(
    "Load Customers",
    () => _service.GetAllAsync());
```

**Performance Counters**:
```csharp
public class PerformanceMonitor
{
    private readonly PerformanceCounter _cpuCounter;
    private readonly PerformanceCounter _ramCounter;

    public PerformanceMonitor()
    {
        _cpuCounter = new PerformanceCounter(
            "Processor", "% Processor Time", "_Total");
        _ramCounter = new PerformanceCounter(
            "Memory", "Available MBytes");
    }

    public float GetCpuUsage() => _cpuCounter.NextValue();
    public float GetAvailableRAM() => _ramCounter.NextValue();
}
```

---

## Common Performance Issues

1. **UI Thread Blocking**
   - Problem: Long-running operations on UI thread freeze the application
   - Solution: Use async/await and Task.Run for CPU-intensive work

2. **Memory Leaks from Undisposed Resources**
   - Problem: Images, database connections, file handles not disposed
   - Solution: Use using statements and proper disposal patterns

3. **Excessive DataGridView Redraws**
   - Problem: Binding to List<T>, calling Refresh() too often
   - Solution: Use BindingList<T>, virtual mode, suspend layout

4. **String Concatenation in Loops**
   - Problem: Creates new string objects each iteration
   - Solution: Use StringBuilder with pre-allocated capacity

5. **Loading All Data at Once**
   - Problem: Retrieving millions of rows into memory
   - Solution: Implement pagination, lazy loading, or virtual mode

6. **Synchronous I/O Operations**
   - Problem: Blocks UI thread during file/network/database operations
   - Solution: Use async methods (ReadAsync, ToListAsync, etc.)

7. **Creating Controls Repeatedly**
   - Problem: Disposing and recreating forms/controls frequently
   - Solution: Reuse controls, hide instead of dispose

8. **No Connection Pooling**
   - Problem: Creating new database connections for each operation
   - Solution: Enable connection pooling in connection string

9. **Inefficient Queries**
   - Problem: SELECT *, N+1 queries, no indexes
   - Solution: Select only needed columns, use Include(), add indexes

10. **Missing Image Disposal**
    - Problem: PictureBox images not disposed when changed
    - Solution: Dispose old image before assigning new one

11. **Excessive Garbage Collection**
    - Problem: Creating many short-lived objects
    - Solution: Object pooling, struct for small types, reduce allocations

12. **Unoptimized Painting**
    - Problem: Redrawing entire control when small region changed
    - Solution: Use region-based invalidation, double buffering

---

## Complete Working Examples

### Optimized DataGridView with 100K Rows

```csharp
public class FastDataGridForm : Form
{
    private DataGridView dgv;
    private List<Customer> _allCustomers;
    private const int PAGE_SIZE = 100;

    public FastDataGridForm()
    {
        InitializeComponent();
        SetupDataGridView();
    }

    private void SetupDataGridView()
    {
        dgv = new DataGridView
        {
            Dock = DockStyle.Fill,
            VirtualMode = true,
            ReadOnly = true,
            AllowUserToAddRows = false,
            AllowUserToDeleteRows = false,
            DoubleBuffered = true
        };

        dgv.Columns.Add("Id", "ID");
        dgv.Columns.Add("Name", "Name");
        dgv.Columns.Add("Email", "Email");
        dgv.Columns.Add("City", "City");

        dgv.CellValueNeeded += Dgv_CellValueNeeded;

        this.Controls.Add(dgv);
    }

    private async void FastDataGridForm_Load(object sender, EventArgs e)
    {
        await LoadDataAsync();
    }

    private async Task LoadDataAsync()
    {
        // Simulate loading 100K rows
        await Task.Run(() =>
        {
            _allCustomers = Enumerable.Range(1, 100000)
                .Select(i => new Customer
                {
                    Id = i,
                    Name = $"Customer {i}",
                    Email = $"customer{i}@example.com",
                    City = $"City {i % 100}"
                })
                .ToList();
        });

        dgv.RowCount = _allCustomers.Count;
    }

    private void Dgv_CellValueNeeded(object sender, DataGridViewCellValueEventArgs e)
    {
        if (e.RowIndex < 0 || e.RowIndex >= _allCustomers.Count)
            return;

        var customer = _allCustomers[e.RowIndex];
        e.Value = e.ColumnIndex switch
        {
            0 => customer.Id,
            1 => customer.Name,
            2 => customer.Email,
            3 => customer.City,
            _ => null
        };
    }
}
```

### Fast Image Gallery

```csharp
public class ImageGalleryForm : Form
{
    private FlowLayoutPanel flowPanel;
    private ImageCache _imageCache;
    private List<string> _imagePaths;

    public ImageGalleryForm()
    {
        InitializeComponent();
        _imageCache = new ImageCache(maxCacheSize: 50);

        flowPanel = new FlowLayoutPanel
        {
            Dock = DockStyle.Fill,
            AutoScroll = true
        };
        this.Controls.Add(flowPanel);
    }

    private async Task LoadGalleryAsync(string directory)
    {
        _imagePaths = Directory.GetFiles(directory, "*.jpg").ToList();

        flowPanel.SuspendLayout();
        try
        {
            foreach (var path in _imagePaths.Take(20)) // Load first 20
            {
                var thumbnail = await CreateThumbnailAsync(path);
                var pictureBox = new PictureBox
                {
                    Image = thumbnail,
                    SizeMode = PictureBoxSizeMode.Zoom,
                    Size = new Size(150, 150),
                    Cursor = Cursors.Hand
                };

                pictureBox.Click += (s, e) => ShowFullImage(path);
                flowPanel.Controls.Add(pictureBox);
            }
        }
        finally
        {
            flowPanel.ResumeLayout();
        }
    }

    private async Task<Image> CreateThumbnailAsync(string path)
    {
        return await Task.Run(() =>
        {
            using (var original = Image.FromFile(path))
            {
                return original.GetThumbnailImage(150, 150, null, IntPtr.Zero);
            }
        });
    }

    private void ShowFullImage(string path)
    {
        var image = _imageCache.GetImage(path);
        var viewer = new ImageViewerForm(image);
        viewer.ShowDialog();
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _imageCache?.Dispose();
        }
        base.Dispose(disposing);
    }
}
```

### Efficient Startup Form

```csharp
public class OptimizedMainForm : Form
{
    private Lazy<CustomerService> _customerService;
    private Lazy<OrderService> _orderService;
    private TabControl tabControl;
    private bool[] _tabsLoaded;

    public OptimizedMainForm()
    {
        InitializeComponent();

        // Lazy initialization
        _customerService = new Lazy<CustomerService>(
            () => new CustomerService(new CustomerRepository()));
        _orderService = new Lazy<OrderService>(
            () => new OrderService(new OrderRepository()));

        SetupTabs();
    }

    private void SetupTabs()
    {
        tabControl = new TabControl { Dock = DockStyle.Fill };

        tabControl.TabPages.Add("customers", "Customers");
        tabControl.TabPages.Add("orders", "Orders");
        tabControl.TabPages.Add("reports", "Reports");

        _tabsLoaded = new bool[tabControl.TabPages.Count];

        tabControl.SelectedIndexChanged += TabControl_SelectedIndexChanged;
        this.Controls.Add(tabControl);
    }

    private void TabControl_SelectedIndexChanged(object sender, EventArgs e)
    {
        int index = tabControl.SelectedIndex;

        if (_tabsLoaded[index])
            return;

        // Lazy load tab content
        switch (index)
        {
            case 0:
                LoadCustomersTab();
                break;
            case 1:
                LoadOrdersTab();
                break;
            case 2:
                LoadReportsTab();
                break;
        }

        _tabsLoaded[index] = true;
    }

    private async void LoadCustomersTab()
    {
        var dgv = new DataGridView { Dock = DockStyle.Fill };
        tabControl.TabPages[0].Controls.Add(dgv);

        var customers = await _customerService.Value.GetAllAsync();
        dgv.DataSource = new BindingList<Customer>(customers);
    }

    private async void LoadOrdersTab()
    {
        var dgv = new DataGridView { Dock = DockStyle.Fill };
        tabControl.TabPages[1].Controls.Add(dgv);

        var orders = await _orderService.Value.GetAllAsync();
        dgv.DataSource = new BindingList<Order>(orders);
    }

    private void LoadReportsTab()
    {
        var reportViewer = new ReportViewer { Dock = DockStyle.Fill };
        tabControl.TabPages[2].Controls.Add(reportViewer);
    }
}
```

---

## Performance Checklist

### UI Performance
- [ ] SuspendLayout/ResumeLayout used for bulk control operations
- [ ] Double buffering enabled for custom painting
- [ ] Region-based invalidation instead of full control refresh
- [ ] Controls lazy loaded where possible
- [ ] Control pooling for frequently created controls

### Data Performance
- [ ] DataGridView uses virtual mode for large datasets (>10K rows)
- [ ] BindingList<T> used instead of List<T> for data binding
- [ ] BeginUpdate/EndUpdate used for ListBox/ComboBox bulk operations
- [ ] Pagination implemented for large datasets
- [ ] Compiled queries used for frequently executed queries

### Memory Performance
- [ ] Images properly disposed
- [ ] StringBuilder used for string concatenation in loops
- [ ] Collection capacity pre-allocated when size known
- [ ] Object pooling for expensive objects
- [ ] Proper disposal of IDisposable resources

### Async Performance
- [ ] Async/await used for all I/O operations
- [ ] UI remains responsive during long operations
- [ ] Progress indication for long-running tasks
- [ ] Parallel processing where appropriate
- [ ] Caching implemented for expensive operations

### Database Performance
- [ ] Connection pooling enabled
- [ ] Batch operations used instead of loops
- [ ] Only required columns selected
- [ ] Indexes created on frequently queried columns
- [ ] Lazy loading or eager loading used appropriately

### Startup Performance
- [ ] Lazy initialization for expensive objects
- [ ] Splash screen shown during initialization
- [ ] Assemblies loaded on demand
- [ ] Minimal work done in constructor
- [ ] Background loading for non-critical data

---

## Related Topics

- **[Resource Management](resource-management.md)** - Proper disposal and cleanup
- **[DataGridView Best Practices](../ui-ux/datagridview-practices.md)** - Optimizing grid performance
- **[Async/Await Pattern](async-await.md)** - Non-blocking operations
- **[Thread Safety](thread-safety.md)** - Concurrent operations
- **[LINQ Best Practices](../advanced/linq-practices.md)** - Efficient queries

---

**Last Updated**: 2025-11-07
**Related**: Resource Management, DataGridView, Async/Await
