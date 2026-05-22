#!/bin/bash
# Runs once after the container is created (postCreateCommand).
# Activates the Unity license and generates IntelliSense project files.

set -e

UNITY=/opt/unity/editor/Unity
PROJECT="$(pwd)"

echo "========================================"
echo " VaarnAtlas – Dev Container Setup"
echo "========================================"

# ── Step 1: License activation ────────────────────────────────────────────────
if [ -z "$UNITY_LICENSE" ]; then
    echo ""
    echo "  UNITY_LICENSE secret is not set — skipping activation."
    echo "  To set up your license, run inside this terminal:"
    echo ""
    echo "      bash .devcontainer/get-license.sh"
    echo ""
    echo "  Then follow the instructions it prints."
    echo ""
    echo "  IntelliSense project files will be generated after you rebuild"
    echo "  the container with UNITY_LICENSE set."
    exit 0
fi

echo ""
echo "Activating Unity license..."
echo "$UNITY_LICENSE" > /tmp/unity.ulf
"$UNITY" -quit -batchmode -nographics \
    -manualLicenseFile /tmp/unity.ulf \
    -logFile /dev/stdout || true
rm -f /tmp/unity.ulf
echo "License activation done."

# ── Step 2: Generate .csproj / .sln for IntelliSense ─────────────────────────
echo ""
echo "Generating project files (this takes ~2 minutes)..."
"$UNITY" -quit -batchmode -nographics \
    -projectPath "$PROJECT" \
    -executeMethod DevContainerSetup.GenerateProjectFiles \
    -logFile /dev/stdout

echo ""
echo "========================================"
echo " Setup complete. IntelliSense is ready."
echo "========================================"
