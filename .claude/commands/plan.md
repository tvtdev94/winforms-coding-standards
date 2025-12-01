---
description: Research, analyze, and create an implementation plan for a feature
argument-hint: [feature description]
---

Create an implementation plan for this task:
<task>$ARGUMENTS</task>

## Instructions

### Step 0: Load Rules (MANDATORY - ALWAYS FIRST!)

**Use `rules-loader` subagent to load ALL coding rules:**

```
Task(subagent_type="rules-loader", prompt="Load rules for planning: $ARGUMENTS")
```

**Wait for rules summary before proceeding.**

### Step 1: Research Phase (PARALLEL):

Use parallel subagents:
- `researcher` subagent → explore best practices
- `Explore` subagent → find related files in codebase

### Step 2: Create Plan:
   - Save plan to `./plans/YYMMDD-feature-name-plan.md`
   - Use template from `./plans/templates/feature-plan-template.md`
   - Include all files to create/modify
   - Define TODO checklist

**Output:**
- Path to created plan file
- Summary of key implementation steps
- List of files to create/modify

**IMPORTANT:** Sacrifice grammar for the sake of concision.
**IMPORTANT:** List unresolved questions at the end.
**IMPORTANT:** Do NOT start implementing - only create the plan.
