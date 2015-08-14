rem pack files to distribute into a zip file
del RigRepeater.zip
pushd bin\release
7z a ..\..\RigRepeater.zip RigRepeater.exe* *.dll
popd
7z a RigRepeater.zip RigRepeaterNotes.htm