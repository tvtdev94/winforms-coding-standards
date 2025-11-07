# Comments & Documentation

> **Quick Reference**: When and how to write effective code comments and XML documentation.

---

## 📝 XML Documentation Comments

### Public APIs
Always document public classes, methods, properties:

```csharp
/// <summary>
/// Manages customer data and business operations.
/// </summary>
public class CustomerService : ICustomerService
{
    /// <summary>
    /// Retrieves all active customers from the database.
    /// </summary>
    /// <returns>A list of active customers.</returns>
    /// <exception cref="DatabaseException">Thrown when database connection fails.</exception>
    public async Task<List<Customer>> GetAllActiveCustomersAsync()
    {
        // Implementation
    }

    /// <summary>
    /// Saves a customer to the database.
    /// </summary>
    /// <param name="customer">The customer entity to save.</param>
    /// <returns>True if saved successfully, false otherwise.</returns>
    public async Task<bool> SaveAsync(Customer customer)
    {
        // Implementation
    }
}
```

---

## 💬 Inline Comments

### When to Comment
✅ **Complex algorithms** - Explain the "why"
✅ **Non-obvious code** - Clarify intent
✅ **Workarounds** - Document temporary solutions
✅ **TODOs** - Mark future improvements

### When NOT to Comment
❌ **Obvious code** - Code should be self-explanatory
❌ **Bad code** - Refactor instead of commenting
❌ **Redundant** - Don't repeat what code already says

```csharp
// ✅ GOOD - Explains WHY
// Use exponential backoff to avoid overwhelming the API
await Task.Delay(retryCount * 1000);

// ❌ BAD - States the obvious
// Increment i by 1
i++;
```

---

## 🏷️ TODO Comments

```csharp
// TODO: Add validation for email format
// FIXME: Memory leak in this method under high load
// HACK: Temporary workaround until API v2 is released
// NOTE: This method is called by reflection - do not rename
```

---

## ✅ Best Practices

### DO:
✅ Write XML comments for all public APIs
✅ Explain complex logic
✅ Document assumptions and constraints
✅ Keep comments up-to-date with code

### DON'T:
❌ Don't comment obvious code
❌ Don't leave commented-out code (use version control)
❌ Don't write novels - be concise

---

**Last Updated**: 2025-11-07
