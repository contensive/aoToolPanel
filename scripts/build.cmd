
rem @echo off

rem Must be run from the projects git\project\scripts folder - everything is relative
rem run >build [versionNumber]
rem versionNumber is YY.MM.DD.build-number, like 20.5.8.1
rem

c:
cd \Git\aoToolPanel\scripts

rem -- Release or Debug
set DebugRelease=Debug

set collectionName=aoToolPanel
set solutionName=aoToolPanel.sln
set collectionPath=..\collections\aoToolPanel\
set binPath=..\source\aoToolPanel\bin\%DebugRelease%\\
set msbuildLocation=C:\Program Files (x86)\Microsoft Visual Studio\2017\Community\MSBuild\15.0\Bin\
set deploymentFolderRoot=C:\Deployments\aoToolPanel\Dev\
set msbuildLocation=C:\Program Files (x86)\Microsoft Visual Studio\2019\Community\MSBuild\Current\Bin\
set NuGetLocalPackagesFolder=C:\NuGetLocalPackages\

rem @echo off
rem Setup deployment folder

set year=%date:~12,4%
set month=%date:~4,2%
if %month% GEQ 10 goto monthOk
set month=%date:~5,1%
:monthOk
set day=%date:~7,2%
if %day% GEQ 10 goto dayOk
set day=%date:~8,1%
:dayOk
set versionMajor=%year%
set versionMinor=%month%
set versionBuild=%day%
set versionRevision=1
rem
rem if deployment folder exists, delete it and make directory
rem
:tryagain
set versionNumber=%versionMajor%.%versionMinor%.%versionBuild%.%versionRevision%
if not exist "%deploymentFolderRoot%%versionNumber%" goto :makefolder
set /a versionRevision=%versionRevision%+1
goto tryagain
:makefolder
md "%deploymentFolderRoot%%versionNumber%"

rem ==============================================================
rem
rem clean build folders
rem
rd /S /Q "..\source\aoToolPanel\bin"
rd /S /Q "..\source\aoToolPanel\obj"

rem pause

rem ==============================================================
rem
rem create helpfiles.zip file for install in private/helpfiles/
rem 
rem make a \help folder in the addon Git folder and store the collections markup files there. 
rem a comma in the filename represents a topic on the navigation, so to make an article "Shopping" in the "Ecommerce" topic, create a document "Ecommerce,Shopping.md"
rem help files are installed in the "privateFiles\helpfiles\(collectionname)" folder. The collectionname must match the addoon collections name exactly.
rem add a resource node to the collection xml file to install the helpfile zip to the site. For example
rem    <Resource name="HelpFiles.zip" type="privatefiles" path="helpfiles/(collectionname)" />
rem then if the first install, 
rem

cd ..\help
del %collectionPath%HelpFiles.zip

rem copy default article and articles for the  Help Pages collection
"c:\program files\7-zip\7z.exe" a "%collectionPath%HelpFiles.zip" 
cd ..\scripts

rem pause

rem ==============================================================
rem
echo build solution 
rem
cd ..\source
"%msbuildLocation%msbuild.exe" %solutionName% /property:Configuration=%DebugRelease%
if errorlevel 1 (
   echo failure building solution
   pause
   exit /b %errorlevel%
)
cd ..\scripts

rem pause

rem ==============================================================
rem
echo build api Nuget
rem
rem cd ..\source\accountBillingApi
rem IF EXIST "*.nupkg" (
rem 	del "*.nupkg" /Q
rem )
rem "nuget.exe" pack "ecommerceApi.nuspec" -version %versionNumber%
rem if errorlevel 1 (
rem    echo failure in nuget
rem    pause
rem    exit /b %errorlevel%
rem )
rem xcopy "Contensive.ecommerceApi.%versionNumber%.nupkg" "C:\nugetLocalPackages" /Y
rem xcopy "Contensive.ecommerceApi.%versionNumber%.nupkg" "%deploymentFolderRoot%%versionNumber%" /Y
rem cd ..\..\scripts

rem ==============================================================
rem
echo Build addon collection
rem

rem remove old ui files
del %collectionPath%*.html
del %collectionPath%*.jpg
del %collectionPath%*.png
del %collectionPath%*.css
del %collectionPath%*.js
del %collectionPath%*.gif

rem remove old DLL files from the collection folder
del %collectionPath%*.DLL
del %collectionPath%*.dll.config

rem copy new  ui files
copy ..\ui\*.html  "%collectionPath%"
copy ..\ui\*.jpg  "%collectionPath%"
copy ..\ui\*.png  "%collectionPath%"
copy ..\ui\*.css  "%collectionPath%"
copy ..\ui\*.js  "%collectionPath%"
copy ..\ui\*.gif  "%collectionPath%"

rem copy bin folder assemblies to collection folder
copy "%binPath%*.dll" "%collectionPath%"
copy "%binPath%*.dll.config" "%collectionPath%"

rem create new collection zip file
c:
cd %collectionPath%
del "%collectionName%.zip" /Q
"c:\program files\7-zip\7z.exe" a "%collectionName%.zip"
xcopy "%collectionName%.zip" "%deploymentFolderRoot%%versionNumber%" /Y
cd ..\..\scripts

rem ==============================================================
rem
echo clean collection folder
rem

del %collectionPath%*.html
del %collectionPath%*.jpg
del %collectionPath%*.png
del %collectionPath%*.css
del %collectionPath%*.js
del %collectionPath%*.gif
del %collectionPath%*.dll
del %collectionPath%*.dll.config
del %collectionPath%HelpFiles.zip

rem pause





