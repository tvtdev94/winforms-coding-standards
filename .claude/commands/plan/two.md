---
description: Create implementation plan with 2 alternative approaches
argument-hint: [feature description]
---

Use the `planner` subagent to create an implementation plan with **multiple approaches** for this task:
<task>$ARGUMENTS</task>

## Instructions

1. **Load Context First**:
   - Read `.claude/project-context.md` for project settings
   - Read `.claude/INDEX.md` to find relevant templates

2. **Research Phase**:
   - Use multiple `researcher` subagents in parallel to explore different approaches
   - Use `Explore` subagent to find related patterns in codebase

3. **Create Plan with 2+ Approaches**:
   - Save plan to `./plans/YYMMDD-feature-name-plan.md`

**Output Format:**

```markdown
# Implementation Plan: [Feature]

## Approach 1: [Name]
### Pros
- Pro 1
- Pro 2

### Cons
- Con 1
- Con 2

### Implementation Steps
1. Step 1
2. Step 2

## Approach 2: [Name]
### Pros
- Pro 1
- Pro 2

### Cons
- Con 1
- Con 2

### Implementation Steps
1. Step 1
2. Step 2

## Recommendation
[Which approach and why]

## Trade-offs Summary
| Aspect | Approach 1 | Approach 2 |
|--------|------------|------------|
| Complexity | Low | Medium |
| Performance | Good | Better |
| Maintainability | High | Medium |
```

**IMPORTANT:** Provide at least 2 implementation approaches with clear trade-offs.
**IMPORTANT:** Explain pros and cons of each approach.
**IMPORTANT:** Provide a recommended approach with justification.
**IMPORTANT:** Do NOT start implementing - ask user for confirmation first.
