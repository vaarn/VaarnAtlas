#!/bin/bash
# Run this ONCE inside the container to generate a Unity license activation
# request file (.alf). You only need to do this if you don't have a .ulf yet.

set -e

UNITY=/opt/unity/editor/Unity
WORKDIR="$(pwd)"

echo "Generating Unity license activation request file..."
cd /tmp

"$UNITY" -quit -batchmode -nographics \
    -createManualActivationFile \
    -logFile /dev/stdout || true

ALF=$(ls /tmp/Unity_v*.alf 2>/dev/null | head -1)

if [ -z "$ALF" ]; then
    echo ""
    echo "ERROR: No .alf file was generated. Check the log output above."
    exit 1
fi

cp "$ALF" "$WORKDIR/"
ALFNAME="$(basename "$ALF")"

echo ""
echo "========================================================"
echo " License activation request file generated:"
echo "   $ALFNAME"
echo ""
echo " Next steps:"
echo "   1. Open https://license.unity3d.com/manual in your browser"
echo "   2. Upload $ALFNAME from your workspace root"
echo "   3. Download the Unity_v2020.x.ulf file it gives you"
echo "   4. Copy the ENTIRE text contents of that .ulf file"
echo "   5. Add it as a secret named UNITY_LICENSE:"
echo "        Codespaces: repo Settings → Secrets → Codespaces"
echo "        Local Docker: add to a .env file (never commit this)"
echo "   6. Rebuild the container — setup.sh will activate automatically"
echo "========================================================"
