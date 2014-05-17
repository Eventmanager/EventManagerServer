EventManagerServer
==================

How to run
----------
- Compile in Visual Studio
- To run in Visual Studio (if you need to debug or edit code), you need to do the following:
  - Go to Start, enter cmd, right click -> Run as Administrator
  - Enter the following line: `http add urlacl url=http://*:8778/ user=baggykiin listen=yes`, put the appropriate username in the user field.
  - It should say: "URL reservation successfully added".
- Otherwise, simply start the exe found in EventManagerServer\EventManagerServer\bin\Debug. It will ask for Administrator privileges. Accept.
- If it starts successfully, the line "HTTP Server STarted. Press Enter to exit." should be displayed.

