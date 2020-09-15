#!/bin/sh

# NOTE: Requires JDK 1.6.0

if [ -z "$ANDROID_SDK_ROOT" ]; then
    echo "Missing ANDROID_SDK_ROOT environment variable"
    exit 1
fi

if [ -z "$ANDROID_NDK_ROOT" ]; then
    echo "Missing ANDROID_NDK_ROOT environment variable"
    exit 1
fi

if [ -z "$UNITY_HOME" ]; then
    echo "Missing UNITY_HOME environment variable. Using default"
    UNITY_HOME=/Applications/Unity/Unity.app
fi

echo $CLASSPATH F
CLASSPATH="$CLASSPATH:$UNITY_HOME/../PlaybackEngines/AndroidPlayer/Variations/mono/Release/Classes/classes.jar:../:$ANDROID_SDK_ROOT/extras/android/support/v13/android-support-v13.jar"
# Also reference zipffile.jar
CLASSPATH="$CLASSPATH:./../../../Plugins/Android/zip_file.jar"
echo $CLASSPATH

echo ""
echo "Building JavaClass PlatformHelperBridge..."
javac -verbose ./PlatformHelperBridge.java -bootclasspath $ANDROID_SDK_ROOT/platforms/android-23/android.jar -classpath $CLASSPATH -d  .

echo ""
echo "Signature dump of JavaClass..."

javap -s com.tfbgames.platformhelper.PlatformHelperBridge

echo "Creating PlatformHelperBridge.jar..."
jar cvf ./PlatformHelperBridge.jar ./com/tfbgames/platformhelper/*.class



echo ""
echo "Cleaning up / removing build folders..."
rm -rf com
rm -rf libs
rm -rf obj
rm -rf org
echo ""
echo "Done!"
