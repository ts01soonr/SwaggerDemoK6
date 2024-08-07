if not exist soonr.jar play fget soonr.jar https://us.workplace.datto.com/filelink/6813-79cc5be9-b5c9d2ad2e-2
if exist log\awp.log move log\awp.log log\awp_%RANDOM%.log
::start start_api.bat
java -jar soonr.jar 2>&1 | mtee log\awp.log