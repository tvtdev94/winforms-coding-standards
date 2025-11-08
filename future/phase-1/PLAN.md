# Phase 1: Restructure Project Organization

**Duration**: 4-6 hours
**Priority**: 🔴 Critical (Do this first!)
**Effort**: Low
**Risk**: Low (mostly file moves and extractions)

---

## 🎯 Goals

1. **Reduce CLAUDE.md size** from 800+ lines to ~400 lines
2. **Extract workflows** to separate files for modularity
3. **Reorganize commands** into logical categories
4. **Add metadata tracking** for version management
5. **Create plans directory** structure for future planning

---

## 📊 Success Criteria

- [ ] CLAUDE.md is <450 lines
- [ ] 4 workflow files created in `.claude/workflows/`
- [ ] Commands organized into 7 categories
- [ ] `metadata.json` created with current stats
- [ ] `plans/` directory structure ready
- [ ] All existing functionality still works
- [ ] Claude Code can still find and execute commands

---

## 🗂️ File Changes Overview

### Files to CREATE (9 new files)

1. `.claude/workflows/winforms-development-workflow.md`
2. `.claude/workflows/testing-workflow.md`
3. `.claude/workflows/code-review-checklist.md`
4. `.claude/workflows/expert-behavior-guide.md`
5. `.claude/metadata.json`
6. `plans/templates/.gitkeep`
7. `plans/reports/.gitkeep`
8. `future/phase-1/files-to-create.md` (detailed content)
9. `future/phase-1/files-to-edit.md` (detailed edits)

### Files to EDIT (2 files)

1. `CLAUDE.md` - Extract workflows, reduce size
2. `.gitignore` - Add `plans/reports/*` (except .gitkeep)

### Files to MOVE (18 command files)

Move from `commands/*.md` → `commands/{category}/*.md`

**Categories**:
- `create/` - 5 commands
- `add/` - 5 commands
- `fix/` - 2 commands
- `refactor/` - 1 command
- `setup/` - 1 command
- `docs/` - 0 commands (will add in Phase 3)
- `test/` - 0 commands (will add in Phase 3)
- Root: `auto-implement.md` (orchestrator, stays at root)

---

## 📋 Implementation Checklist

### Step 1: Create Workflows Directory (10 min)

- [ ] Create `.claude/workflows/` directory
- [ ] Extract "WinForms Development Workflow" from CLAUDE.md → `winforms-development-workflow.md`
- [ ] Extract "Testing Workflow" section → `testing-workflow.md`
- [ ] Extract "Pre-Commit Checklist" → `code-review-checklist.md`
- [ ] Extract "Expert Behavior" section → `expert-behavior-guide.md`
- [ ] Verify all 4 workflow files are complete and well-formatted

**📁 See**: `files-to-create.md` for full workflow file contents

---

### Step 2: Create Metadata File (5 min)

- [ ] Create `.claude/metadata.json` with current project stats
- [ ] Include version, name, stats, repository info
- [ ] Validate JSON syntax

**📁 See**: `files-to-create.md` for full metadata.json content

---

### Step 3: Create Plans Directory (5 min)

- [ ] Create `plans/templates/` directory
- [ ] Create `plans/reports/` directory
- [ ] Add `.gitkeep` files to both
- [ ] Update `.gitignore` to exclude `plans/reports/*` but keep `.gitkeep`

---

### Step 4: Reorganize Commands (90 min)

**4a. Create category directories** (5 min)
- [ ] Create `commands/create/`
- [ ] Create `commands/add/`
- [ ] Create `commands/fix/`
- [ ] Create `commands/refactor/`
- [ ] Create `commands/setup/`

**4b. Move commands to categories** (30 min)

| Current File | New Location | Category |
|-------------|--------------|----------|
| `create-dialog.md` | `create/dialog.md` | create |
| `create-custom-control.md` | `create/custom-control.md` | create |
| `create-repository.md` | `create/repository.md` | create |
| `create-service.md` | `create/service.md` | create |
| `add-data-binding.md` | `add/data-binding.md` | add |
| `add-error-handling.md` | `add/error-handling.md` | add |
| `add-logging.md` | `add/logging.md` | add |
| `add-settings.md` | `add/settings.md` | add |
| `add-validation.md` | `add/validation.md` | add |
| `fix-threading.md` | `fix/threading.md` | fix |
| `optimize-performance.md` | `fix/performance.md` | fix |
| `refactor-to-mvp.md` | `refactor/to-mvp.md` | refactor |
| `setup-di.md` | `setup/di.md` | setup |
| `auto-implement.md` | `auto-implement.md` | root (orchestrator) |

**4c. Update command file references** (15 min)
- [ ] In each moved file, update any internal cross-references
- [ ] Update file paths in documentation references

**4d. Create category index files** (40 min)
- [ ] Create `commands/create/README.md` listing all create commands
- [ ] Create `commands/add/README.md` listing all add commands
- [ ] Create `commands/fix/README.md` listing all fix commands
- [ ] Create `commands/refactor/README.md` listing all refactor commands
- [ ] Create `commands/setup/README.md` listing all setup commands

**📁 See**: `command-reorganization-map.md` for detailed move instructions

---

### Step 5: Update CLAUDE.md (60 min)

**5a. Remove extracted sections** (20 min)
- [ ] Remove "WinForms Development Workflow" section (replaced by workflow file reference)
- [ ] Remove "Testing Workflow" section
- [ ] Remove "Pre-Commit Checklist" section
- [ ] Remove "Expert Behavior" section

**5b. Add workflow references** (10 min)
- [ ] Add "Workflows" section with links to `.claude/workflows/` files
- [ ] Add brief 2-3 line description of each workflow
- [ ] Keep workflow section concise (<100 lines total)

**5c. Update "Code Generation Patterns"** (15 min)
- [ ] Simplify to reference plan templates (Phase 3)
- [ ] Keep only high-level patterns
- [ ] Remove verbose examples (keep in workflows)

**5d. Update "Claude Code Context Loading"** (10 min)
- [ ] Update table to reference workflow files
- [ ] Mention plan templates (Phase 3)
- [ ] Update file size guidelines

**5e. Validate CLAUDE.md size** (5 min)
- [ ] Count lines: should be <450 lines
- [ ] Verify all sections still present
- [ ] Verify markdown formatting correct

**📁 See**: `files-to-edit.md` for exact CLAUDE.md diffs

---

### Step 6: Update Documentation (30 min)

**6a. Update COMPLETION_STATUS.md** (10 min)
- [ ] Add Phase 1 completion entry
- [ ] Update "Recent Changes" section
- [ ] Increment version to 4.1.0

**6b. Update README.md** (10 min)
- [ ] Mention new workflow structure
- [ ] Update "Project Structure" section
- [ ] Add link to `future/` folder

**6c. Create CHANGELOG.md** (10 min)
- [ ] Create new CHANGELOG.md if doesn't exist
- [ ] Add entry for Phase 1 changes
- [ ] Follow Keep a Changelog format

---

### Step 7: Testing & Validation (45 min)

**7a. Test command discovery** (15 min)
- [ ] Open Claude Code
- [ ] Type `/` and verify commands appear
- [ ] Test that categorized commands still work
- [ ] Verify `/create/dialog` works (or however Claude Code handles nested commands)

**7b. Test CLAUDE.md loading** (10 min)
- [ ] Start new Claude Code session
- [ ] Verify CLAUDE.md loads successfully
- [ ] Ask Claude to read a workflow file
- [ ] Verify workflow references work

**7c. Test metadata** (5 min)
- [ ] Verify `metadata.json` is valid JSON
- [ ] Try reading it with a script/tool

**7d. Run example project** (15 min)
- [ ] Navigate to `example-project/`
- [ ] Run `dotnet build`
- [ ] Run `dotnet test`
- [ ] Verify nothing broke

---

## 🎯 Expected Results

### Before Phase 1
```
.claude/
├── commands/
│   ├── add-data-binding.md
│   ├── add-error-handling.md
│   ├── add-logging.md
│   ├── add-settings.md
│   ├── add-validation.md
│   ├── auto-implement.md
│   ├── create-custom-control.md
│   ├── create-dialog.md
│   ├── create-repository.md
│   ├── create-service.md
│   ├── fix-threading.md
│   ├── optimize-performance.md
│   ├── refactor-to-mvp.md
│   └── setup-di.md
└── CLAUDE.md (800+ lines)
```

### After Phase 1
```
.claude/
├── commands/
│   ├── create/
│   │   ├── README.md
│   │   ├── dialog.md
│   │   ├── custom-control.md
│   │   ├── repository.md
│   │   └── service.md
│   ├── add/
│   │   ├── README.md
│   │   ├── data-binding.md
│   │   ├── error-handling.md
│   │   ├── logging.md
│   │   ├── settings.md
│   │   └── validation.md
│   ├── fix/
│   │   ├── README.md
│   │   ├── threading.md
│   │   └── performance.md
│   ├── refactor/
│   │   ├── README.md
│   │   └── to-mvp.md
│   ├── setup/
│   │   ├── README.md
│   │   └── di.md
│   └── auto-implement.md
├── workflows/
│   ├── winforms-development-workflow.md
│   ├── testing-workflow.md
│   ├── code-review-checklist.md
│   └── expert-behavior-guide.md
├── metadata.json
└── CLAUDE.md (~400 lines)

plans/
├── templates/.gitkeep
└── reports/.gitkeep
```

---

## ⚠️ Potential Issues & Solutions

### Issue 1: Claude Code doesn't recognize nested commands

**Problem**: `/create/dialog` might not work if Claude Code expects flat structure

**Solution**:
- Keep command files as `create-dialog.md` (no nesting in filenames)
- Organize in folders for developer experience
- Test thoroughly in Step 7

**Alternative**: If nested doesn't work, use prefixes: `create-dialog.md`, `add-validation.md` (keep current naming)

---

### Issue 2: CLAUDE.md references break

**Problem**: Removing sections might break Claude's understanding

**Solution**:
- Keep section headers, add "See workflow file" references
- Don't remove concepts, just move to workflow files
- Test with Claude Code before committing

---

### Issue 3: Plans directory feels empty

**Problem**: Only .gitkeep files, no actual plans yet

**Solution**:
- This is expected in Phase 1
- Phase 3 will populate `templates/`
- `reports/` will be used when agents are implemented (Phase 2)

---

## 🔄 Rollback Plan

If Phase 1 causes issues:

1. **Restore CLAUDE.md**: `git checkout CLAUDE.md`
2. **Move commands back**: Reverse the file moves
3. **Remove new files**: Delete `.claude/workflows/`, `plans/`, `metadata.json`
4. **Verify**: Test that everything works as before

**Safety**: Make a backup before starting:
```bash
git checkout -b backup-before-phase-1
git add -A
git commit -m "Backup before Phase 1 restructure"
git checkout main
```

---

## 📝 Notes for Implementation

### For Claude Code Web

When implementing with Claude Code web:

1. **Start with Step 1** (workflows) - Most impactful
2. **Read `files-to-create.md`** - Has full file contents ready to copy
3. **Read `files-to-edit.md`** - Has exact diffs for CLAUDE.md
4. **Follow checklist order** - Dependencies between steps
5. **Test after each step** - Don't wait until end

### Context Management

- Load CLAUDE.md first to understand current structure
- Load one workflow section at a time during extraction
- Reference `files-to-create.md` for exact content to avoid rewriting

---

## ✅ Definition of Done

Phase 1 is complete when:

- [x] All 9 new files created
- [x] CLAUDE.md reduced to <450 lines
- [x] All commands reorganized and working
- [x] Tests pass (`dotnet test` in example-project)
- [x] Claude Code can load and use new structure
- [x] Documentation updated (README, COMPLETION_STATUS, CHANGELOG)
- [x] Changes committed to git
- [x] No broken links or references

---

**Next Phase**: [Phase 2: WinForms-Specific Agents](../phase-2/PLAN.md)

---

**Last Updated**: 2025-11-08
**Estimated Time**: 4-6 hours
**Difficulty**: ⭐⭐☆☆☆ (Easy-Medium)
