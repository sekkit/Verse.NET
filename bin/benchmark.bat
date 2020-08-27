for /l %%i in (1,1,50) do (
	call start_client.bat
	timeout /T 1 /NOBREAK
)