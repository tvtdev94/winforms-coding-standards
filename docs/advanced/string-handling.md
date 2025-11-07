# String Handling Best Practices

> **Quick Reference**: Efficient string handling patterns for C# WinForms applications to optimize performance and memory usage.

---

## 📋 Overview

String operations are fundamental to nearly every application, but improper string handling can lead to significant performance degradation and memory waste. In .NET, strings are immutable reference types, meaning every modification creates a new string object in memory. Understanding string internals and best practices is crucial for building efficient WinForms applications.

**What You'll Learn**:
- String immutability and its performance implications
- Efficient string concatenation techniques
- String comparison and manipulation best practices
- Modern C# string features (Span<char>, ranges, interpolation)
- Memory-efficient string operations

---

## 🎯 Why This Matters

**Performance Impact**:
- Inefficient string concatenation in loops can degrade performance by orders of magnitude
- Excessive string allocations increase garbage collection pressure
- Poor string comparisons can cause subtle bugs and security issues

**Memory Usage**:
- Each string concatenation with `+` creates a new object in memory
- Unused strings remain in memory until garbage collected
- String interning can reduce memory for repeated strings

**User Experience**:
- Fast string operations improve UI responsiveness
- Efficient log message building reduces overhead
- Proper string handling prevents out-of-memory exceptions

**Code Quality**:
- Clear string manipulation improves readability
- Modern string features reduce boilerplate code
- Type-safe string formatting prevents runtime errors

---

## String Basics

### String Immutability

**What It Means**:
In .NET, strings are immutable - once created, their contents cannot be changed. Any "modification" creates a new string object while the original remains in memory until garbage collected.

**Why Strings Are Immutable**:
- **Thread Safety**: Multiple threads can safely share string references
- **Security**: String contents (passwords, keys) can't be modified after creation
- **Hash Codes**: Strings can be safely used as dictionary keys
- **Performance**: String literals can be shared across the application

**Performance Implications**:

❌ **Inefficient - Creates 1000 string objects**:
```csharp
public string BuildMessageSlow()
{
    string message = "";
    for (int i = 0; i < 1000; i++)
    {
        message += i.ToString(); // Creates new string each iteration
    }
    return message;
}
```

✅ **Efficient - Creates 1 string object**:
```csharp
public string BuildMessageFast()
{
    var sb = new StringBuilder(4000); // Pre-allocate
    for (int i = 0; i < 1000; i++)
    {
        sb.Append(i);
    }
    return sb.ToString(); // Single string created
}
```

**Memory Visualization**:
```csharp
string str = "Hello";
str += " World"; // Original "Hello" remains in memory
                 // New "Hello World" created
                 // Now have 2 strings in memory
```

### String Interning

**What Is String Interning**:
String interning is a process where .NET maintains a single copy of each unique string literal in a special memory pool. When you reference the same literal multiple times, all references point to the same memory location.

**When It Happens**:
- **Automatically**: For string literals in code
- **Manually**: Using `string.Intern()` method
- **Not automatic**: For runtime-created strings (concatenation, StringBuilder)

**Examples**:

```csharp
// Literals are automatically interned
string s1 = "Hello";
string s2 = "Hello";
bool same = ReferenceEquals(s1, s2); // True - same reference!

// Runtime strings are NOT automatically interned
string s3 = "Hel" + "lo"; // Compiler optimizes to "Hello" literal
string s4 = string.Concat("Hel", "lo"); // Runtime concatenation
bool sameRuntime = ReferenceEquals(s1, s4); // False - different references

// Manual interning
string s5 = string.Intern(s4);
bool nowSame = ReferenceEquals(s1, s5); // True - interned!
```

**When to Use Interning**:

✅ **Good use case - Repeated status strings**:
```csharp
public class StatusManager
{
    // Intern common status strings to save memory
    private static readonly string StatusActive = string.Intern("Active");
    private static readonly string StatusInactive = string.Intern("Inactive");
    private static readonly string StatusPending = string.Intern("Pending");

    public string GetStatus(int code)
    {
        return code switch
        {
            1 => StatusActive,    // Reuses same reference
            2 => StatusInactive,
            3 => StatusPending,
            _ => string.Intern("Unknown")
        };
    }
}
```

❌ **Bad use case - User input**:
```csharp
// Don't intern user input - memory leak!
public void ProcessUserInput(string input)
{
    string interned = string.Intern(input); // Stays in memory forever!
    // Process...
}
```

**Important Notes**:
- Interned strings live for the application lifetime
- Never intern user input or dynamic data
- Use for repeated constant strings only
- Check if already interned with `string.IsInterned()`

---

## String Concatenation

### ❌ Bad: Using + Operator in Loops

**Problem**:
Each `+` operation creates a new string object. In loops, this creates exponential allocations.

**Slow Examples**:

```csharp
// Creates 10,000 string objects
public string BuildReportSlow(List<Customer> customers)
{
    string report = "Customer Report\n";
    report += "================\n";

    foreach (var customer in customers)
    {
        report += $"Name: {customer.Name}\n"; // New string each time
        report += $"Email: {customer.Email}\n";
        report += $"City: {customer.City}\n\n";
    }

    return report;
}

// Creates 1000+ intermediate strings
public string BuildCsvSlow(string[] values)
{
    string csv = "";
    for (int i = 0; i < values.Length; i++)
    {
        csv += values[i];
        if (i < values.Length - 1)
            csv += ","; // Another new string
    }
    return csv;
}
```

**Performance Impact**:
- 10 concatenations: ~2x slower than StringBuilder
- 100 concatenations: ~10x slower
- 1000 concatenations: ~100x slower
- Heavy garbage collection pressure

### ✅ Good: StringBuilder

**When to Use**:
- Concatenating strings in loops (3+ iterations)
- Building strings with unknown final length
- Constructing complex multi-line strings
- Appending to strings repeatedly

**Basic Usage**:

```csharp
public string BuildReportFast(List<Customer> customers)
{
    var sb = new StringBuilder();
    sb.AppendLine("Customer Report");
    sb.AppendLine("================");

    foreach (var customer in customers)
    {
        sb.AppendLine($"Name: {customer.Name}");
        sb.AppendLine($"Email: {customer.Email}");
        sb.AppendLine($"City: {customer.City}");
        sb.AppendLine();
    }

    return sb.ToString();
}
```

**Capacity Pre-allocation**:

❌ **Without capacity - Multiple reallocations**:
```csharp
var sb = new StringBuilder(); // Default capacity: 16
for (int i = 0; i < 10000; i++)
{
    sb.Append("Item "); // Reallocates at 16, 32, 64, 128...
    sb.Append(i);
}
```

✅ **With capacity - Single allocation**:
```csharp
// Estimate: ~10 chars per item * 10000 items
var sb = new StringBuilder(capacity: 100000);
for (int i = 0; i < 10000; i++)
{
    sb.Append("Item ");
    sb.Append(i);
}
```

**Capacity Calculation**:
```csharp
public string BuildLargeReport(List<Order> orders)
{
    // Estimate: 5 lines * 50 chars * order count
    int estimatedCapacity = orders.Count * 250;
    var sb = new StringBuilder(estimatedCapacity);

    foreach (var order in orders)
    {
        sb.AppendLine($"Order ID: {order.Id}");
        sb.AppendLine($"Customer: {order.CustomerName}");
        sb.AppendLine($"Total: {order.Total:C}");
        sb.AppendLine($"Date: {order.Date:d}");
        sb.AppendLine();
    }

    return sb.ToString();
}
```

### String.Concat and String.Join

**String.Concat**:
Better than `+` for known small concatenations.

```csharp
// ❌ Slow - Creates 2 intermediate strings
string fullName = firstName + " " + lastName;

// ✅ Fast - Single allocation
string fullName = string.Concat(firstName, " ", lastName);

// ✅ Also good - String interpolation (C# 10+)
string fullName = $"{firstName} {lastName}";
```

**String.Join**:
Optimal for joining collections with delimiters.

```csharp
// ❌ Slow - Multiple string objects
string csv = "";
for (int i = 0; i < values.Length; i++)
{
    csv += values[i];
    if (i < values.Length - 1)
        csv += ",";
}

// ✅ Fast - Single operation
string csv = string.Join(",", values);

// ✅ Works with LINQ
var names = customers.Select(c => c.Name);
string nameList = string.Join(", ", names);

// ✅ Custom formatting
var formattedList = string.Join(" | ",
    customers.Select(c => $"{c.Name} ({c.Id})"));
```

---

## String Interpolation

### Basic Interpolation

**Syntax**:
String interpolation with `$""` is cleaner and often faster than `string.Format`.

```csharp
// ❌ Verbose and error-prone
string message = string.Format("User {0} logged in at {1}",
    userName, DateTime.Now);

// ✅ Clear and readable
string message = $"User {userName} logged in at {DateTime.Now}";

// ✅ Multi-line interpolation
string report = $@"
Customer Report
Name: {customer.Name}
Email: {customer.Email}
Balance: {customer.Balance:C}
";
```

### Format Strings

**Number Formatting**:
```csharp
decimal price = 1234.56m;
int quantity = 1000000;

string formatted = $"Price: {price:C}";           // "$1,234.56"
string fixed = $"Value: {price:F4}";              // "1234.5600"
string number = $"Count: {quantity:N0}";          // "1,000,000"
string percent = $"Rate: {0.125:P1}";             // "12.5%"
string hex = $"Color: #{255:X2}{128:X2}{64:X2}";  // "#FF8040"
```

**Date Formatting**:
```csharp
DateTime now = DateTime.Now;

string short = $"Date: {now:d}";          // "11/7/2025"
string long = $"Date: {now:D}";           // "Friday, November 7, 2025"
string time = $"Time: {now:t}";           // "2:30 PM"
string full = $"Full: {now:F}";           // "Friday, November 7, 2025 2:30:45 PM"
string custom = $"Custom: {now:yyyy-MM-dd HH:mm}"; // "2025-11-07 14:30"
```

**Alignment**:
```csharp
// Right-align in 10 character field
string aligned = $"Value: {123,10}";      // "Value:        123"

// Left-align in 10 character field
string leftAlign = $"Name: {name,-10}";   // "Name: John      "

// Combine alignment and format
string formatted = $"{price,15:C}";       // "      $1,234.56"
```

**Conditional Formatting**:
```csharp
public string FormatStatus(Order order)
{
    return $"Order {order.Id}: " +
           $"{(order.IsPaid ? "PAID" : "PENDING")} - " +
           $"{order.Total:C}";
}

// With null-coalescing
string displayName = $"User: {user?.Name ?? "Unknown"}";
```

### Interpolated vs Format vs Concat

**Performance Comparison**:

```csharp
// Small, constant strings - all comparable
string s1 = string.Concat("Hello ", name);
string s2 = string.Format("Hello {0}", name);
string s3 = $"Hello {name}";

// Complex formatting - interpolation wins for readability
string formatted = $"Customer {id}: {name} - Balance: {balance:C}";
// vs
string formatted2 = string.Format("Customer {0}: {1} - Balance: {2:C}",
    id, name, balance);

// Many values - StringBuilder is best
var sb = new StringBuilder();
sb.Append("Customer ");
sb.Append(id);
sb.Append(": ");
sb.Append(name);
// ...
```

**When to Use Each**:

| Scenario | Best Choice | Reason |
|----------|------------|--------|
| 2-3 strings, no formatting | String interpolation | Cleanest syntax |
| Formatting numbers/dates | String interpolation | Inline format specifiers |
| Localization (i18n) | `string.Format` | Format string in resource file |
| Loop concatenation | StringBuilder | Avoids allocations |
| Joining array/list | `string.Join` | Single operation |
| Simple 2 strings | `string.Concat` | Slightly faster |

---

## String Comparison

### Ordinal vs Culture-Based

**StringComparison Enum**:
Always specify comparison type explicitly to avoid bugs.

```csharp
public enum StringComparison
{
    CurrentCulture,              // Culture-sensitive, case-sensitive
    CurrentCultureIgnoreCase,    // Culture-sensitive, case-insensitive
    InvariantCulture,            // Invariant culture, case-sensitive
    InvariantCultureIgnoreCase,  // Invariant culture, case-insensitive
    Ordinal,                     // Binary comparison, case-sensitive
    OrdinalIgnoreCase            // Binary comparison, case-insensitive
}
```

**When to Use Each**:

✅ **Ordinal - File paths, keys, technical strings**:
```csharp
// File extension check
if (fileName.EndsWith(".txt", StringComparison.OrdinalIgnoreCase))
{
    ProcessTextFile(fileName);
}

// Dictionary keys
var settings = new Dictionary<string, string>(
    StringComparer.OrdinalIgnoreCase);
settings["ApiKey"] = "abc123";

// Configuration values
if (config.GetValue("Mode").Equals("Debug",
    StringComparison.OrdinalIgnoreCase))
{
    EnableDebugMode();
}
```

✅ **Culture-based - User-visible text**:
```csharp
// Sorting names for display
var sortedNames = customers
    .OrderBy(c => c.Name, StringComparer.CurrentCulture)
    .ToList();

// User input comparison
if (userInput.Equals("yes",
    StringComparison.CurrentCultureIgnoreCase))
{
    Proceed();
}
```

**Common Pitfalls**:

❌ **Dangerous - Culture-dependent behavior**:
```csharp
// Turkish "i" problem: 'i'.ToUpper() = 'İ' in Turkey
if (input.ToLower() == "file") // May fail in Turkey!
{
    // ...
}
```

✅ **Safe - Explicit comparison**:
```csharp
if (input.Equals("file", StringComparison.OrdinalIgnoreCase))
{
    // Works everywhere
}
```

### Equals vs ==

**Difference**:
- `==`: Default comparison (ordinal for string literals, overridable)
- `.Equals()`: Allows specifying comparison type

```csharp
string s1 = "Hello";
string s2 = "hello";

// ❌ Only works for exact case match
if (s1 == s2) // False
{
    // Won't execute
}

// ✅ Case-insensitive comparison
if (s1.Equals(s2, StringComparison.OrdinalIgnoreCase)) // True
{
    // Executes
}

// Null safety
string nullString = null;
bool result1 = nullString == "test";           // False (safe)
bool result2 = nullString.Equals("test");      // NullReferenceException!
bool result3 = "test".Equals(nullString);      // False (safe)
```

**Best Practices**:

```csharp
// ✅ Null-safe comparison
public bool IsMatch(string input, string expected)
{
    // Put known non-null string first
    return expected.Equals(input, StringComparison.OrdinalIgnoreCase);
}

// ✅ Or use null-conditional
public bool IsMatchSafe(string input, string expected)
{
    return input?.Equals(expected,
        StringComparison.OrdinalIgnoreCase) ?? false;
}

// ✅ For exact match with nulls
public bool ExactMatch(string s1, string s2)
{
    return string.Equals(s1, s2, StringComparison.Ordinal);
}
```

---

## String Manipulation

### Substring Operations

**Basic Methods**:

```csharp
string text = "Hello, World!";

// Extract substring
string sub1 = text.Substring(0, 5);              // "Hello"
string sub2 = text.Substring(7);                 // "World!"

// Find position
int comma = text.IndexOf(',');                   // 5
int world = text.IndexOf("World");               // 7
int notFound = text.IndexOf("xyz");              // -1

// Last occurrence
string path = @"C:\Users\John\Documents\file.txt";
int lastSlash = path.LastIndexOf('\\');          // 23
string fileName = path.Substring(lastSlash + 1); // "file.txt"

// Case-insensitive search
int pos = text.IndexOf("WORLD",
    StringComparison.OrdinalIgnoreCase);         // 7
```

**Safe Substring**:

```csharp
// ❌ Can throw ArgumentOutOfRangeException
string unsafe = text.Substring(100);

// ✅ Safe extraction
public static string SafeSubstring(string str, int start, int length)
{
    if (string.IsNullOrEmpty(str) || start >= str.Length)
        return string.Empty;

    if (start + length > str.Length)
        length = str.Length - start;

    return str.Substring(start, length);
}
```

### Modern: Span<char> for Slicing (C# 8+)

**Zero-allocation slicing**:

```csharp
string text = "Hello, World!";

// ❌ Old way - Creates new string
string hello = text.Substring(0, 5);

// ✅ New way - No allocation
ReadOnlySpan<char> helloSpan = text.AsSpan(0, 5);

// Process without allocating
public static bool StartsWithNumber(string text)
{
    if (text.Length == 0) return false;

    ReadOnlySpan<char> firstChar = text.AsSpan(0, 1);
    return char.IsDigit(firstChar[0]);
}

// Efficient parsing
public static (string firstName, string lastName) ParseName(string fullName)
{
    ReadOnlySpan<char> span = fullName.AsSpan();
    int spaceIndex = span.IndexOf(' ');

    if (spaceIndex == -1)
        return (fullName, string.Empty);

    return (
        span.Slice(0, spaceIndex).ToString(),
        span.Slice(spaceIndex + 1).ToString()
    );
}
```

### Trim, TrimStart, TrimEnd

**Whitespace Removal**:

```csharp
string input = "  Hello World  \t\n";

string trimmed = input.Trim();           // "Hello World"
string trimStart = input.TrimStart();    // "Hello World  \t\n"
string trimEnd = input.TrimEnd();        // "  Hello World"
```

**Custom Characters**:

```csharp
string path = "///folder/file.txt///";
string cleaned = path.Trim('/');         // "folder/file.txt"

string csv = ",,,value1,value2,,,";
string trimmed = csv.Trim(',');          // "value1,value2"

// Multiple characters
string text = "***---Hello---***";
char[] chars = { '*', '-' };
string result = text.Trim(chars);        // "Hello"
```

### Replace and Remove

**Replace**:

```csharp
string text = "Hello World";

// Simple replace
string replaced = text.Replace("World", "Everyone");  // "Hello Everyone"
string noSpaces = text.Replace(" ", "");              // "HelloWorld"

// Multiple replacements
string dirty = "Hello   World  !  ";
string clean = dirty.Replace("  ", " ")               // Reduce double spaces
                    .Replace(" !", "!")               // Remove space before !
                    .Trim();                          // "Hello World!"

// Case-insensitive replace (custom method)
public static string ReplaceIgnoreCase(string input,
    string find, string replace)
{
    return System.Text.RegularExpressions.Regex.Replace(
        input,
        System.Text.RegularExpressions.Regex.Escape(find),
        replace,
        System.Text.RegularExpressions.RegexOptions.IgnoreCase
    );
}
```

**Remove**:

```csharp
string text = "Hello, World!";

// Remove from index to end
string removed1 = text.Remove(5);                  // "Hello"

// Remove specific length
string removed2 = text.Remove(5, 7);               // "Hello!"

// Remove specific text
string clean = text.Replace(",", "");              // Use Replace, not Remove
```

### Split and Join

**Basic Split**:

```csharp
string csv = "John,Doe,30,Engineer";
string[] parts = csv.Split(',');        // ["John", "Doe", "30", "Engineer"]

// Multiple delimiters
string text = "apple;banana,orange;grape";
char[] delimiters = { ',', ';' };
string[] fruits = text.Split(delimiters);

// Split with options
string data = "a,,b,  ,c,,,d";
string[] cleaned = data.Split(',',
    StringSplitOptions.RemoveEmptyEntries |
    StringSplitOptions.TrimEntries);    // ["a", "b", "c", "d"]

// Limit count
string path = "one/two/three/four";
string[] parts = path.Split('/', 2);    // ["one", "two/three/four"]
```

**Split Lines**:

```csharp
string multiline = "Line 1\nLine 2\r\nLine 3\rLine 4";

// ❌ Doesn't handle all line endings
string[] lines1 = multiline.Split('\n');

// ✅ Handles all line endings
string[] lines2 = multiline.Split(
    new[] { "\r\n", "\r", "\n" },
    StringSplitOptions.None);

// ✅ C# 6+ - Environment.NewLine
string[] lines3 = multiline.Split(
    new[] { Environment.NewLine },
    StringSplitOptions.RemoveEmptyEntries);
```

---

## StringBuilder

### When to Use StringBuilder

**Decision Guide**:

```csharp
// ✅ Use StringBuilder when:
// - Concatenating in loops (3+ iterations)
// - Unknown number of concatenations
// - Building strings incrementally

// ❌ Don't use StringBuilder when:
// - Concatenating 2-3 known strings
// - Using string.Join or interpolation works
// - String is built once and never modified
```

### Capacity Management

**Why Capacity Matters**:

```csharp
// ❌ Poor - Default capacity 16, reallocates many times
var sb1 = new StringBuilder();
for (int i = 0; i < 1000; i++)
{
    sb1.Append("Item " + i + "\n");  // Reallocates at 16, 32, 64...
}

// ✅ Good - Pre-allocated capacity
var sb2 = new StringBuilder(capacity: 10000);
for (int i = 0; i < 1000; i++)
{
    sb2.Append("Item ").Append(i).Append('\n');
}

// ✅ Best - Capacity from calculation
int itemCount = items.Count;
int estimatedChars = itemCount * 50; // Estimate 50 chars per item
var sb3 = new StringBuilder(estimatedChars);
```

**Monitoring Capacity**:

```csharp
var sb = new StringBuilder(100);
Console.WriteLine($"Capacity: {sb.Capacity}");  // 100
Console.WriteLine($"Length: {sb.Length}");      // 0

sb.Append(new string('x', 150));
Console.WriteLine($"Capacity: {sb.Capacity}");  // 200 (doubled)
Console.WriteLine($"Length: {sb.Length}");      // 150
```

### StringBuilder Methods

**Core Methods**:

```csharp
var sb = new StringBuilder();

// Append
sb.Append("Hello");
sb.Append(' ');
sb.Append("World");

// AppendLine
sb.AppendLine("Line 1");
sb.AppendLine("Line 2");

// AppendFormat
sb.AppendFormat("Customer {0}: {1}", id, name);
sb.AppendFormat("Price: {0:C}", price);

// Append with interpolation (C# 10+)
sb.Append($"Customer {id}: {name}");

// AppendJoin
string[] items = { "apple", "banana", "orange" };
sb.AppendJoin(", ", items);

// Insert
sb.Insert(0, "Prefix: ");

// Replace
sb.Replace("World", "Everyone");
sb.Replace("old", "new", startIndex: 10, count: 20);

// Remove
sb.Remove(5, 7);  // Remove 7 chars starting at index 5

// Clear
sb.Clear();       // Resets length to 0, keeps capacity
```

**Chaining**:

```csharp
var result = new StringBuilder()
    .AppendLine("Report")
    .AppendLine("------")
    .Append("Name: ").AppendLine(name)
    .Append("Date: ").AppendLine(DateTime.Now.ToString("d"))
    .ToString();
```

---

## Modern String Features (C# 8+)

### Span<char> and ReadOnlySpan<char>

**Stack-allocated Slicing**:

```csharp
// Traditional - Allocates new string
public static string GetFileExtension(string fileName)
{
    int dot = fileName.LastIndexOf('.');
    return fileName.Substring(dot + 1);  // Allocates
}

// Modern - No allocation
public static ReadOnlySpan<char> GetFileExtensionSpan(string fileName)
{
    int dot = fileName.LastIndexOf('.');
    return fileName.AsSpan(dot + 1);     // No allocation
}

// Usage
string file = "document.pdf";
ReadOnlySpan<char> ext = GetFileExtensionSpan(file);
if (ext.SequenceEqual("pdf"))
{
    ProcessPdf(file);
}
```

**Zero-allocation String Processing**:

```csharp
public static bool IsValidEmail(string email)
{
    ReadOnlySpan<char> span = email.AsSpan();
    int atIndex = span.IndexOf('@');

    if (atIndex == -1 || atIndex == 0 || atIndex == span.Length - 1)
        return false;

    ReadOnlySpan<char> domain = span.Slice(atIndex + 1);
    return domain.IndexOf('.') > 0;
}

// Efficient parsing
public static int SumNumbers(string input)
{
    ReadOnlySpan<char> span = input.AsSpan();
    int sum = 0;

    foreach (Range range in span.Split(','))
    {
        ReadOnlySpan<char> numberSpan = span[range];
        if (int.TryParse(numberSpan, out int number))
            sum += number;
    }

    return sum;
}
```

### Range Operators

**Index and Range Syntax**:

```csharp
string text = "Hello, World!";

// Index from end
char last = text[^1];              // '!'
char secondLast = text[^2];        // 'd'

// Range
string hello = text[0..5];         // "Hello"
string world = text[7..12];        // "World"
string all = text[..];             // "Hello, World!"
string fromSeventh = text[7..];    // "World!"
string lastFive = text[^5..];      // "orld!"
string skipLast = text[..^1];      // "Hello, World"
```

**Combining with Span**:

```csharp
public static ReadOnlySpan<char> GetMiddlePart(string input)
{
    if (input.Length < 10) return input.AsSpan();

    // Get middle, skip first 2 and last 2
    return input.AsSpan()[2..^2];
}
```

### String.Create

**Custom String Creation**:

```csharp
// Traditional - Multiple allocations
public static string FormatId(int id)
{
    return "ID-" + id.ToString("D6");
}

// Modern - Single allocation
public static string FormatIdOptimized(int id)
{
    return string.Create(9, id, (span, value) =>
    {
        "ID-".AsSpan().CopyTo(span);
        value.TryFormat(span.Slice(3), out _, "D6");
    });
}

// Complex formatting
public static string BuildCustomerKey(int customerId, string region)
{
    int length = region.Length + 10; // "CUST-" + 5 digits + "-" + region

    return string.Create(length, (customerId, region), (span, state) =>
    {
        int pos = 0;
        "CUST-".AsSpan().CopyTo(span);
        pos += 5;

        state.customerId.TryFormat(span.Slice(pos), out int written, "D5");
        pos += written;

        span[pos++] = '-';

        state.region.AsSpan().CopyTo(span.Slice(pos));
    });
}
```

---

## Regular Expressions

### Regex for Validation

**Common Patterns**:

```csharp
using System.Text.RegularExpressions;

public class Validator
{
    // Compiled for performance
    private static readonly Regex EmailRegex = new Regex(
        @"^[^@\s]+@[^@\s]+\.[^@\s]+$",
        RegexOptions.Compiled | RegexOptions.IgnoreCase);

    private static readonly Regex PhoneRegex = new Regex(
        @"^\(?(\d{3})\)?[-.\s]?(\d{3})[-.\s]?(\d{4})$",
        RegexOptions.Compiled);

    private static readonly Regex ZipCodeRegex = new Regex(
        @"^\d{5}(-\d{4})?$",
        RegexOptions.Compiled);

    public static bool IsValidEmail(string email)
    {
        return !string.IsNullOrWhiteSpace(email) &&
               EmailRegex.IsMatch(email);
    }

    public static bool IsValidPhone(string phone)
    {
        return PhoneRegex.IsMatch(phone);
    }

    public static bool IsValidZipCode(string zip)
    {
        return ZipCodeRegex.IsMatch(zip);
    }
}

// Usage
if (Validator.IsValidEmail(txtEmail.Text))
{
    SaveEmail(txtEmail.Text);
}
```

**Extraction**:

```csharp
public static (string area, string exchange, string number)
    ParsePhone(string phone)
{
    var regex = new Regex(@"^\(?(\d{3})\)?[-.\s]?(\d{3})[-.\s]?(\d{4})$");
    var match = regex.Match(phone);

    if (!match.Success)
        return (null, null, null);

    return (match.Groups[1].Value,
            match.Groups[2].Value,
            match.Groups[3].Value);
}
```

### Regex Performance

**Static vs Instance**:

```csharp
// ❌ Slow - Creates new regex each time
public bool ValidateEmailSlow(string email)
{
    return Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$");
}

// ✅ Fast - Reuses compiled regex
private static readonly Regex EmailPattern = new Regex(
    @"^[^@\s]+@[^@\s]+\.[^@\s]+$",
    RegexOptions.Compiled);

public bool ValidateEmailFast(string email)
{
    return EmailPattern.IsMatch(email);
}
```

**Timeout Configuration**:

```csharp
// Prevent ReDoS attacks
private static readonly Regex SafeRegex = new Regex(
    pattern,
    RegexOptions.Compiled,
    matchTimeout: TimeSpan.FromMilliseconds(100));

public bool SafeMatch(string input)
{
    try
    {
        return SafeRegex.IsMatch(input);
    }
    catch (RegexMatchTimeoutException)
    {
        _logger.LogWarning("Regex timeout on input: {Input}", input);
        return false;
    }
}
```

---

## String Encoding

### UTF-8, UTF-16, ASCII

**Understanding Encodings**:

```csharp
using System.Text;

// UTF-16 (default in .NET strings)
string text = "Hello 世界";
Console.WriteLine($"String length: {text.Length}"); // 8 chars

// UTF-8 encoding
byte[] utf8Bytes = Encoding.UTF8.GetBytes(text);
Console.WriteLine($"UTF-8 bytes: {utf8Bytes.Length}"); // 12 bytes

// UTF-16 encoding
byte[] utf16Bytes = Encoding.Unicode.GetBytes(text);
Console.WriteLine($"UTF-16 bytes: {utf16Bytes.Length}"); // 16 bytes

// ASCII (loses non-ASCII chars)
byte[] asciiBytes = Encoding.ASCII.GetBytes(text);
string asciiText = Encoding.ASCII.GetString(asciiBytes);
Console.WriteLine(asciiText); // "Hello ??"
```

**When to Use Each**:

```csharp
public class EncodingExamples
{
    // ASCII - English-only, legacy systems
    public byte[] EncodeAscii(string text)
    {
        return Encoding.ASCII.GetBytes(text);
    }

    // UTF-8 - Web, JSON, most file formats
    public byte[] EncodeUtf8(string text)
    {
        return Encoding.UTF8.GetBytes(text);
    }

    // UTF-16 - Windows APIs, .NET internal
    public byte[] EncodeUtf16(string text)
    {
        return Encoding.Unicode.GetBytes(text);
    }

    // Decoding
    public string DecodeUtf8(byte[] bytes)
    {
        return Encoding.UTF8.GetString(bytes);
    }
}
```

### Base64

**Encoding/Decoding**:

```csharp
// Encode to Base64
string original = "Hello, World!";
byte[] bytes = Encoding.UTF8.GetBytes(original);
string base64 = Convert.ToBase64String(bytes);
Console.WriteLine(base64); // "SGVsbG8sIFdvcmxkIQ=="

// Decode from Base64
byte[] decoded = Convert.FromBase64String(base64);
string result = Encoding.UTF8.GetString(decoded);
Console.WriteLine(result); // "Hello, World!"
```

**Use Cases**:

```csharp
public class Base64Helper
{
    // Encode binary data for transmission
    public static string EncodeImage(byte[] imageBytes)
    {
        return Convert.ToBase64String(imageBytes);
    }

    // URL-safe Base64
    public static string EncodeUrlSafe(string input)
    {
        byte[] bytes = Encoding.UTF8.GetBytes(input);
        return Convert.ToBase64String(bytes)
            .Replace('+', '-')
            .Replace('/', '_')
            .TrimEnd('=');
    }

    public static string DecodeUrlSafe(string base64)
    {
        string base64Padded = base64
            .Replace('-', '+')
            .Replace('_', '/');

        // Add padding
        switch (base64Padded.Length % 4)
        {
            case 2: base64Padded += "=="; break;
            case 3: base64Padded += "="; break;
        }

        byte[] bytes = Convert.FromBase64String(base64Padded);
        return Encoding.UTF8.GetString(bytes);
    }
}
```

---

## Best Practices

### ✅ DO

1. **Use StringBuilder for loop concatenation**
   ```csharp
   var sb = new StringBuilder(estimatedCapacity);
   foreach (var item in items)
       sb.AppendLine(item);
   ```

2. **Pre-allocate StringBuilder capacity when known**
   ```csharp
   var sb = new StringBuilder(items.Count * 50);
   ```

3. **Use string.Join for collections**
   ```csharp
   string csv = string.Join(",", values);
   ```

4. **Specify StringComparison explicitly**
   ```csharp
   if (path.EndsWith(".txt", StringComparison.OrdinalIgnoreCase))
   ```

5. **Use string interpolation for readability**
   ```csharp
   string msg = $"User {name} logged in at {time:HH:mm}";
   ```

6. **Intern frequently-used constant strings**
   ```csharp
   private static readonly string StatusActive = string.Intern("Active");
   ```

7. **Use Span<char> for zero-allocation slicing**
   ```csharp
   ReadOnlySpan<char> span = text.AsSpan(start, length);
   ```

8. **Compile regex for repeated use**
   ```csharp
   private static readonly Regex Pattern = new Regex(
       @"pattern", RegexOptions.Compiled);
   ```

9. **Use range operators for substrings (C# 8+)**
   ```csharp
   string sub = text[5..10];
   ```

10. **Put known non-null string first in Equals**
    ```csharp
    if ("expected".Equals(userInput, StringComparison.Ordinal))
    ```

### ❌ DON'T

1. **Don't concatenate strings in loops with +**
   ```csharp
   ❌ foreach (var item in items) result += item;
   ✅ var sb = new StringBuilder(); foreach (var item in items) sb.Append(item);
   ```

2. **Don't use ToLower/ToUpper for comparison**
   ```csharp
   ❌ if (input.ToLower() == "test")
   ✅ if (input.Equals("test", StringComparison.OrdinalIgnoreCase))
   ```

3. **Don't intern user input or dynamic strings**
   ```csharp
   ❌ string interned = string.Intern(userInput);
   ✅ Use regular strings for dynamic data
   ```

4. **Don't create regex in loops**
   ```csharp
   ❌ foreach (var item in items) new Regex(pattern).IsMatch(item);
   ✅ Use static compiled regex outside loop
   ```

5. **Don't use Substring when Span works**
   ```csharp
   ❌ string part = text.Substring(0, 5);
   ✅ ReadOnlySpan<char> part = text.AsSpan(0, 5);
   ```

6. **Don't forget null checks with Equals**
   ```csharp
   ❌ userInput.Equals("test") // NullReferenceException if null
   ✅ "test".Equals(userInput) // or userInput?.Equals("test")
   ```

7. **Don't use string for large text processing**
   ```csharp
   ❌ string log = ""; foreach (var msg in messages) log += msg;
   ✅ Use StringBuilder or write directly to stream
   ```

8. **Don't forget to specify encoding**
   ```csharp
   ❌ File.ReadAllText(path) // Uses default encoding
   ✅ File.ReadAllText(path, Encoding.UTF8)
   ```

---

## Performance Tips

### Checklist for Efficient String Handling

- [ ] Use StringBuilder for 3+ concatenations
- [ ] Pre-allocate StringBuilder capacity
- [ ] Use string.Join instead of loop concatenation
- [ ] Specify StringComparison explicitly
- [ ] Use Span<char> for temporary slicing
- [ ] Compile and reuse regex patterns
- [ ] Avoid ToLower/ToUpper for comparisons
- [ ] Use ordinal comparison for technical strings
- [ ] Only intern constant/repeated strings
- [ ] Consider string.Create for complex formatting
- [ ] Use range operators instead of Substring
- [ ] Specify encoding explicitly for I/O

---

## Common Patterns

### Building CSV

```csharp
public string BuildCsv(List<Customer> customers)
{
    var sb = new StringBuilder(customers.Count * 80);
    sb.AppendLine("ID,Name,Email,Phone");

    foreach (var customer in customers)
    {
        sb.Append(customer.Id).Append(',')
          .Append(EscapeCsv(customer.Name)).Append(',')
          .Append(EscapeCsv(customer.Email)).Append(',')
          .AppendLine(EscapeCsv(customer.Phone));
    }

    return sb.ToString();
}

private string EscapeCsv(string value)
{
    if (string.IsNullOrEmpty(value))
        return string.Empty;

    if (value.Contains(',') || value.Contains('"') || value.Contains('\n'))
        return $"\"{value.Replace("\"", "\"\"")}\"";

    return value;
}
```

### Parsing Delimited Data

```csharp
public Dictionary<string, string> ParseKeyValuePairs(string input)
{
    var result = new Dictionary<string, string>(
        StringComparer.OrdinalIgnoreCase);

    string[] pairs = input.Split(';',
        StringSplitOptions.RemoveEmptyEntries |
        StringSplitOptions.TrimEntries);

    foreach (string pair in pairs)
    {
        string[] parts = pair.Split('=', 2);
        if (parts.Length == 2)
        {
            result[parts[0].Trim()] = parts[1].Trim();
        }
    }

    return result;
}

// Usage: "key1=value1; key2=value2; key3=value3"
```

### Log Message Building

```csharp
public string BuildLogEntry(LogLevel level, string message,
    Exception ex = null)
{
    var sb = new StringBuilder(256);
    sb.Append('[').Append(DateTime.Now:yyyy-MM-dd HH:mm:ss.fff).Append("] ");
    sb.Append('[').Append(level.ToString().ToUpperInvariant()).Append("] ");
    sb.AppendLine(message);

    if (ex != null)
    {
        sb.AppendLine("Exception:");
        sb.AppendLine($"  Type: {ex.GetType().FullName}");
        sb.AppendLine($"  Message: {ex.Message}");
        sb.AppendLine($"  StackTrace: {ex.StackTrace}");
    }

    return sb.ToString();
}
```

---

## Complete Working Examples

### Example 1: StringBuilder for Report Generation

```csharp
public class ReportGenerator
{
    public string GenerateCustomerReport(List<Customer> customers)
    {
        // Estimate: 5 lines * 60 chars per customer + 200 char header
        int capacity = (customers.Count * 300) + 200;
        var sb = new StringBuilder(capacity);

        // Header
        sb.AppendLine("=" + new string('=', 78));
        sb.AppendLine($"{"CUSTOMER REPORT",-78}");
        sb.AppendLine($"Generated: {DateTime.Now:F}");
        sb.AppendLine("=" + new string('=', 78));
        sb.AppendLine();

        // Column headers
        sb.AppendFormat("{0,-10} {1,-25} {2,-30} {3,12}\n",
            "ID", "Name", "Email", "Balance");
        sb.AppendLine(new string('-', 78));

        // Data rows
        decimal totalBalance = 0;
        foreach (var customer in customers)
        {
            sb.AppendFormat("{0,-10} {1,-25} {2,-30} {3,12:C}\n",
                customer.Id,
                Truncate(customer.Name, 25),
                Truncate(customer.Email, 30),
                customer.Balance);

            totalBalance += customer.Balance;
        }

        // Footer
        sb.AppendLine(new string('-', 78));
        sb.AppendFormat("{0,-67} {1,12:C}\n",
            $"Total ({customers.Count} customers):",
            totalBalance);
        sb.AppendLine("=" + new string('=', 78));

        return sb.ToString();
    }

    private string Truncate(string value, int maxLength)
    {
        if (string.IsNullOrEmpty(value))
            return string.Empty;

        return value.Length <= maxLength
            ? value
            : value.Substring(0, maxLength - 3) + "...";
    }
}
```

### Example 2: Span<char> for Efficient Parsing

```csharp
public class CsvParser
{
    public List<Dictionary<string, string>> ParseCsv(string csvContent)
    {
        var results = new List<Dictionary<string, string>>();
        ReadOnlySpan<char> content = csvContent.AsSpan();

        // Find line breaks
        int lineStart = 0;
        string[] headers = null;

        while (lineStart < content.Length)
        {
            int lineEnd = content.Slice(lineStart).IndexOfAny('\r', '\n');
            if (lineEnd == -1)
                lineEnd = content.Length - lineStart;

            ReadOnlySpan<char> line = content.Slice(lineStart, lineEnd);

            if (headers == null)
            {
                // Parse header
                headers = SplitCsvLine(line.ToString());
            }
            else if (line.Length > 0)
            {
                // Parse data row
                string[] values = SplitCsvLine(line.ToString());
                var row = new Dictionary<string, string>(
                    StringComparer.OrdinalIgnoreCase);

                for (int i = 0; i < Math.Min(headers.Length, values.Length); i++)
                {
                    row[headers[i]] = values[i];
                }

                results.Add(row);
            }

            // Move to next line
            lineStart += lineEnd + 1;
            if (lineStart < content.Length && content[lineStart - 1] == '\r'
                && content[lineStart] == '\n')
            {
                lineStart++; // Skip \n in \r\n
            }
        }

        return results;
    }

    private string[] SplitCsvLine(string line)
    {
        return line.Split(',', StringSplitOptions.TrimEntries);
    }
}
```

### Example 3: Efficient String Validation

```csharp
public class StringValidator
{
    private static readonly Regex EmailRegex = new Regex(
        @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$",
        RegexOptions.Compiled | RegexOptions.IgnoreCase,
        TimeSpan.FromMilliseconds(100));

    private static readonly Regex PhoneRegex = new Regex(
        @"^(\+\d{1,3}[-.]?)?\(?\d{3}\)?[-.\s]?\d{3}[-.\s]?\d{4}$",
        RegexOptions.Compiled,
        TimeSpan.FromMilliseconds(100));

    public ValidationResult ValidateCustomerInput(
        string name, string email, string phone)
    {
        var errors = new List<string>();

        // Name validation
        if (string.IsNullOrWhiteSpace(name))
        {
            errors.Add("Name is required");
        }
        else if (name.Length < 2 || name.Length > 100)
        {
            errors.Add("Name must be between 2 and 100 characters");
        }

        // Email validation
        if (string.IsNullOrWhiteSpace(email))
        {
            errors.Add("Email is required");
        }
        else if (!IsValidEmail(email))
        {
            errors.Add("Email format is invalid");
        }

        // Phone validation (optional)
        if (!string.IsNullOrWhiteSpace(phone) && !IsValidPhone(phone))
        {
            errors.Add("Phone format is invalid");
        }

        return new ValidationResult
        {
            IsValid = errors.Count == 0,
            Errors = errors
        };
    }

    private bool IsValidEmail(string email)
    {
        try
        {
            return EmailRegex.IsMatch(email);
        }
        catch (RegexMatchTimeoutException)
        {
            return false;
        }
    }

    private bool IsValidPhone(string phone)
    {
        try
        {
            return PhoneRegex.IsMatch(phone);
        }
        catch (RegexMatchTimeoutException)
        {
            return false;
        }
    }
}

public class ValidationResult
{
    public bool IsValid { get; set; }
    public List<string> Errors { get; set; }

    public string GetErrorMessage()
    {
        return string.Join(Environment.NewLine, Errors);
    }
}
```

---

## Related Topics

- **[Performance Optimization](../best-practices/performance.md)** - Overall performance strategies
- **[LINQ Best Practices](linq-practices.md)** - Efficient query operations
- **[Resource Management](../best-practices/resource-management.md)** - Memory and disposal

---

**Last Updated**: 2025-11-07
**Related**: Performance, LINQ, Memory Management
