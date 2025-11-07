# Code Style Guidelines

> **Quick Reference**: Formatting and style rules for consistent, readable C# code.

---

## 🎨 Code Formatting

### Braces
```csharp
// ✅ GOOD - Allman style (braces on new line)
if (condition)
{
    DoSomething();
}

// ❌ BAD - K&R style (not C# standard)
if (condition) {
    DoSomething();
}
```

### Indentation
- Use **4 spaces** (not tabs)
- Consistent indentation throughout

### Line Length
- Max **120 characters** per line
- Break long lines at logical points

### var keyword
```csharp
// ✅ GOOD - Use when type is obvious
var customer = new Customer();
var customers = await _service.GetAllAsync();

// ✅ GOOD - Explicit type when unclear
ICustomerService service = GetService();
int count = GetCount();

// ❌ BAD - Unnecessary explicit type
Customer customer = new Customer();
```

---

## 📏 Method & Class Size

### Methods
- ✅ Keep methods **< 30 lines**
- ✅ One responsibility per method
- ✅ Extract complex logic to separate methods

### Classes
- ✅ Keep classes **< 500 lines**
- ✅ Single Responsibility Principle
- ✅ Extract to multiple classes if too large

---

## ✅ Best Practices

### DO:
✅ Use LINQ for collections
✅ Use expression-bodied members when simple
✅ Order members: fields → properties → constructors → methods
✅ Group related code together
✅ Add blank lines between logical sections

### DON'T:
❌ Don't nest > 3 levels deep
❌ Don't write giant methods
❌ Don't mix concerns in one class

---

**Last Updated**: 2025-11-07
