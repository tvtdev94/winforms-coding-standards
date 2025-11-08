#!/bin/bash
# Installation script for git hooks

echo "[Installing git hooks...]"

# Get the directory where this script is located
SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"

# Configure git to use .githooks directory
git config core.hooksPath "$SCRIPT_DIR"

# Make hooks executable
chmod +x "$SCRIPT_DIR"/pre-commit 2>/dev/null || true

echo "[OK] Git hooks installed successfully!"
echo ""
echo "Hooks configured in: $SCRIPT_DIR"
echo ""
echo "The following hooks are now active:"
echo "  - pre-commit: Quality checks before commits"
echo ""
echo "To disable hooks temporarily, use:"
echo "  git commit --no-verify"
echo ""
