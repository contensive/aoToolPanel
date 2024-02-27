
rem build and deliver to deployment folder

set appName=c5test

call build.cmd

rem upload to contensive application
c:
cd %collectionPath%
cc -a %appName% --installFile "%collectionName%.zip"
cd ..\..\scripts

pauseapp://resources/notifications.html#