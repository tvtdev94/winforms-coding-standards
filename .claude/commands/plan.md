---
description: Research, analyze, and create an implementation plan for a feature
argument-hint: [feature description]
---

Use the `planner` subagent to create an implementation plan for this task:
<task>$ARGUMENTS</task>

## Instructions

1. **Load Context First**:
   - Read `.claude/project-context.md` for project settings
   - Read `.claude/INDEX.md` to find relevant templates

2. **Research Phase**:
   - Use `researcher` subagent to explore best practices
   - Use `Explore` subagent to find related files in codebase

3. **Create Plan**:
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
