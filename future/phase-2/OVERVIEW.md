# Phase 2: WinForms-Specific Agents

**Status**: 📋 High-Level Plan (Detail when Phase 1 complete)
**Duration**: 1-2 days
**Priority**: 🔴 Critical
**Prerequisites**: Phase 1 complete

---

## 🎯 Goals

Create specialized AI agents for automating common WinForms development tasks:

1. **winforms-reviewer** - Automated code quality review
2. **test-generator** - Auto-generate unit and integration tests
3. **docs-manager** - Keep documentation in sync with code
4. **mvp-validator** - Validate MVP/MVVM pattern adherence

---

## 📦 Deliverables

### Files to CREATE

```
.claude/agents/
├── winforms-reviewer.md         # Code review specialist
├── test-generator.md            # Test generation automation
├── docs-manager.md              # Documentation synchronization
└── mvp-validator.md             # Architecture pattern validation
```

### Features

**winforms-reviewer Agent**:
- ✅ Check MVP/MVVM pattern adherence
- ✅ Validate control naming conventions
- ✅ Verify Dispose() implementation
- ✅ Check thread safety (Invoke/BeginInvoke)
- ✅ Validate async/await usage
- ✅ Generate detailed review reports in `plans/reports/`

**test-generator Agent**:
- ✅ Auto-generate unit tests for Services
- ✅ Auto-generate integration tests for Repositories
- ✅ Auto-generate presenter tests
- ✅ Use AAA pattern (Arrange-Act-Assert)
- ✅ Include success, error, and edge case tests
- ✅ Target 80%+ coverage

**docs-manager Agent**:
- ✅ Sync documentation with code changes
- ✅ Auto-update code examples in docs
- ✅ Detect outdated documentation
- ✅ Generate codebase-summary.md
- ✅ Maintain CHANGELOG.md

**mvp-validator Agent**:
- ✅ Validate separation of concerns
- ✅ Check for business logic in Forms
- ✅ Verify presenter pattern usage
- ✅ Validate service layer structure
- ✅ Generate architecture compliance reports

---

## 📊 Impact

### Before Phase 2
- Manual code review (30-60 min per feature)
- Manual test writing (2-4 hours per feature)
- Manual docs updates (often forgotten)
- Pattern violations discovered late

### After Phase 2
- Automated code review (<5 min)
- Auto-generated tests (1-click)
- Auto-updated documentation
- Pattern violations caught immediately

**Time Savings**: ~4-8 hours per feature

---

## 🔧 Implementation Approach

### Step 1: Create Agent Definitions (4 hours)

Each agent needs:
1. Frontmatter (name, description, model)
2. Core responsibilities section
3. Workflow/process definition
4. Report format template
5. Usage examples

**Template Structure**:
```markdown
---
name: agent-name
description: Brief description
model: sonnet
---

# Agent Name

You are a specialist in [domain].

## Core Responsibilities
1. Responsibility 1
2. Responsibility 2

## Process
1. Step 1
2. Step 2

## Report Format
[Template]

## Usage
[Examples]
```

### Step 2: Create Report Templates (2 hours)

For each agent, define report structure:
- `plans/reports/code-review-report-template.md`
- `plans/reports/test-generation-report-template.md`
- `plans/reports/docs-sync-report-template.md`
- `plans/reports/architecture-validation-report-template.md`

### Step 3: Test Agents (2-4 hours)

Test each agent on example-project:
- Run winforms-reviewer on CustomerService.cs
- Run test-generator on CustomerService.cs
- Run mvp-validator on CustomerForm.cs
- Run docs-manager on recent changes

### Step 4: Update Commands (2 hours)

Add new slash commands to invoke agents:
- `/review-code [files]` → spawns winforms-reviewer
- `/generate-tests [class]` → spawns test-generator
- `/validate-architecture [files]` → spawns mvp-validator
- `/sync-docs` → spawns docs-manager

---

## ✅ Success Criteria

Phase 2 is complete when:

- [ ] All 4 agents created and working
- [ ] Report templates defined
- [ ] Agents tested on example-project
- [ ] Slash commands added for agent invocation
- [ ] Documentation updated (README.md, CLAUDE.md)
- [ ] Agents can be spawned from main conversation
- [ ] Reports saved to `plans/reports/` correctly

---

## 📋 Checklist (Detailed in PLAN.md - To be created)

When ready to implement Phase 2, we'll create detailed `PLAN.md` with:

- [ ] Step-by-step agent creation instructions
- [ ] Full agent file contents
- [ ] Report template specifications
- [ ] Command integration steps
- [ ] Testing procedures
- [ ] Rollback plan

---

## 🔗 Dependencies

**Requires**:
- Phase 1 workflows (agents reference these)
- Phase 1 plans directory (for report storage)

**Enables**:
- Phase 4 (init script can setup agents)
- Phase 5 (skills can use agents)

---

## 📝 Notes for Future Implementation

### Agent Orchestration Patterns

From claudekit-engineer, we learned:
1. **Sequential Chaining**: Planning → Implementation → Testing → Review
2. **Parallel Execution**: Code + Tests + Docs simultaneously
3. **Query Fan-Out**: Spawn 3-5 agents for parallel research

### Context Management

- Keep agent files <500 lines each
- Reference workflow files instead of duplicating
- Use concise reporting format
- Save detailed reports to plans/reports/

### Example Usage

```
User: "Review my CustomerService code"