echo Cleaning up Build directory
rm -rf ../../MultiplayerECS_Test/Builds

echo Starting Build Process
'/f/Unity/2019.2.6f1/Editor/Unity.exe' -quit -batchmode -projectPath ../../MultiplayerECS_Test -executeMethod BuildScript.BuildLocal
echo Ended Build Process