:: This script contains two option how you can "repair" lost GPU
:: Default option (option 1) will restart your entire rig, you can also uncomment "NirCMD" to make a screenshot befor reboot (you need to place NirCMD in NHML directory under "NirCMD\nircmd.exe")
:: Second option will use nvidiaInspector to restart display drivers, you need to place "NvidiaInspector" in NHML directory under "NV_Inspector\nvidiaInspector.exe", this will set GPU profile (OC etc.) to default so it will also select MSI Afterburner profile
:: Option 1 is used by default, if you whant to use Option 2 you need to change line "set OPTION=1" to "set OPTION=2"
:: Remember that Option 2 requires "nvidiaInspector" and MSIAfterburner, you can tune Option 2 to your needs (look for comments for more clues)
@echo off
::Select desire option here
set OPTION=%1
set GPU=%2
IF %OPTION%==2 GOTO OPT2

:OPT1
:: Option 1: restart RIG
echo %DATE% %TIME% Lost GPU#%GPU%. Restart Windows >> logs\GPU_Lost.txt
taskkill /F /IM MinerLegacyForkFixMonitor.exe
timeout 2
taskkill /F /IM NiceHashMinerLegacy.exe
timeout 2
shutdown -r -f -t 0
exit

:OPT2
:: Option 2: close NHML, restart display drivers, start NHML, nvidiaInspector is required in NHML directory
echo %DATE% %TIME% Lost GPU#%GPU%. Restart driver >> logs\GPU_Lost.txt
taskkill /F /IM MinerLegacyForkFixMonitor.exe
timeout 2
taskkill /F /IM NiceHashMinerLegacy.exe
timeout 2
start utils\nvidiaInspector.exe -restartDisplayDriver
timeout 15
start NiceHashMinerLegacy.exe
