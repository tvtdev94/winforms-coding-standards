# Fix Commands

Commands for fixing common WinForms issues and problems.

## Available Commands

- **/fix:bug** - Smart bug fixer that analyzes error logs and suggests fixes
- **/fix:threading** - Fix cross-thread UI access issues
- **/fix:performance** - Optimize WinForms application performance

## Usage

Use these commands to identify and fix common issues in WinForms applications.

### Quick Guide

**Have an error log or stack trace?** → Use `/fix:bug`
- Paste your error message or stack trace
- Get root cause analysis
- Receive automatic fix suggestions
- Apply fixes with explanations

**Cross-thread UI issues?** → Use `/fix:threading`
- Comprehensive thread safety review
- Detect all cross-thread UI accesses
- Add proper Invoke/BeginInvoke patterns

**Performance problems?** → Use `/fix:performance`
- Identify performance bottlenecks
- Optimize slow operations
- Reduce memory usage

## What These Commands Fix

### /fix:bug
- NullReferenceException
- Cross-thread UI access errors
- ObjectDisposedException
- InvalidOperationException
- Missing Dispose patterns
- Async void methods
- Exception swallowing
- And more...

### /fix:threading
- Thread safety issues
- Cross-thread UI access
- Race conditions
- UI responsiveness problems
- Background worker issues

### /fix:performance
- Performance bottlenecks
- Memory leaks
- Resource management issues
- Slow UI rendering
- Inefficient database queries
