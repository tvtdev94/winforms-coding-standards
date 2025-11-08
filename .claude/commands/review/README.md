# Review Commands

Code review slash commands for self-review and team collaboration.

## Available Commands

### `/review-pr <branch>`
Comprehensive Pull Request review for team collaboration.

**Use when**:
- Reviewing a team member's Pull Request
- Need full PR analysis with recommendations
- Want structured feedback with severity levels

**Features**:
- Reviews all changed files in PR
- Categorizes issues (Critical/Major/Minor)
- Uses review comment templates
- Provides positive feedback
- Makes clear recommendation (Approve/Request Changes/Comment)

**Time**: 15-30 minutes

**Example**:
```
/review-pr feature/customer-management
```

---

### `/review-code <files>`
Detailed review of specific file(s).

**Use when**:
- Reviewing specific files (not entire PR)
- Need focused review on changed code
- Self-review before committing

**Features**:
- File-specific detailed analysis
- Pattern validation (MVP/MVVM)
- Best practices check
- Security and performance review

**Time**: 5-10 minutes per file

**Example**:
```
/review-code CustomerForm.cs CustomerService.cs
```

---

## Resources

- **[PR Review Workflow](../../workflows/pr-review-workflow.md)** - Complete PR review process
- **[Review Comment Templates](../../../templates/review-comment-templates.md)** - Reusable templates
- **[Code Review Checklist](../../workflows/code-review-checklist.md)** - Comprehensive checklist

---

## Quick Start

### Reviewing Your Own Code (Self-Review)
```
/review-code MyForm.cs MyService.cs
```

### Reviewing Team Member's PR
```
/review-pr feature/new-feature
```

---

**Last Updated**: 2025-11-08
