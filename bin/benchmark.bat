for /l %%i in (1,1,50) do (
	call start_client.bat
        call sleep.vbs
	rem timeout /T 1 /NOBREAK
)