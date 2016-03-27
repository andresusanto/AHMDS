abolish.
:- dynamic explanation/1.
explanation([]).

add_explanation(New) :- explanation(L1), append(L1, New, L2), retractall(explanation(L1)), asserta(explanation(L2)).
del_explanation :- retractall(explanation(_)), asserta(explanation([])).


% score individual
	
	% Anti-reversing / Anti-debugging API
	mi_score(70, "IsDebuggerPresent") :- add_explanation(["Program checks whether itself is being debugged by user-mode debugger."]).
	mi_score(70, "CheckRemoteDebuggerPresent") :- add_explanation(["Program checks whether itself is being debugged"]).
	mi_score(70, "NTQueryInformationProcess") :- add_explanation(["Program retreives information about a process."]).
	mi_score(70, "OutputDebugString") :- add_explanation(["Program sends strings to a debugger."]).
	mi_score(70, "QueryPerformanceCounter") :- add_explanation(["Program analyze whether itself is being debugged by using performance measurement (QueryPerformanceCounter)."]).
	mi_score(20, "GetTickCount") :- add_explanation(["Program analyze whether itself is being debugged by using performance measurement (GetTickCount)."]).
	mi_score(20, "timeGetTime") :- add_explanation(["Program analyze whether itself is being debugged by using performance measurement (timeGetTime)."]).
	mi_score(30, "ZwClose") :- add_explanation(["Program generate an exception when it is being debugged."]).
	mi_score(70, "DebugActiveProcess") :- add_explanation(["Program debug active processs."]).
	
	% Password dumping API
	mi_score(30, "SamIConnect") :- add_explanation(["Program connects to SAM (Security Account Manager)."]).
	mi_score(30, "SamrQueryInformationUser") :- add_explanation(["Program query user's information to SAM (Security Account Manager)."]).
	mi_score(30, "SamIGetPrivateData") :- add_explanation(["Program gets user's provate data to SAM (Security Account Manager)."]).
	mi_score(70, "SystemFunction025") :- add_explanation(["Program decrypts user's password hashes."]).
	mi_score(70, "SystemFunction027") :- add_explanation(["Program decrypts user's password hashes."]).
	mi_score(30, "LsaEnumerateLogonSessions") :- add_explanation(["Program obtains a list of locally unique identifiers (contains usernames/domains for each logon)."]).
	
	% obfuscation api
	mi_score(20, "CryptAcquireContext") :- add_explanation(["Program initializes Windows encryption (Obfuscation)."]).
	
	% keylogging activity
	mi_score(120, "GetAsyncKeyState") :- add_explanation(["Program logs user's keyboard inputs."]).
	mi_score(120, "GetKeyState") :- add_explanation(["Program logs user's keyboard inputs."]).
	mi_score(60, "GetForegroundWindow") :- add_explanation(["Program monitors active windows (so they can be logged)."]).
	
	% shellcode API
	mi_score(20, "WinExec") :- add_explanation(["Program executes other program."]).
	mi_score(20, "CreateProcessW") :- add_explanation(["Program creates new process."]).
	mi_score(20, "CreateProcessA") :- add_explanation(["Program creates new process."]).
	
	% registry API
	mi_score(20, "RegOpenKeyEx") :- add_explanation(["Program opens registry keys."]).
	mi_score(20, "RegGetValue") :- add_explanation(["Program gets registry values."]).
	mi_score(60, "RegSetValueEx") :- add_explanation(["Program sets registry values."]).
	
	
	% service related API
	mi_score(50, "OpenSCManagerW") :- add_explanation(["Program establishes a connection to the service control manager on the specified computer and opens the specified service control manager database"]).
	mi_score(50, "OpenSCManagerA") :- add_explanation(["Program establishes a connection to the service control manager on the specified computer and opens the specified service control manager database"]).
	mi_score(100, "CreateService") :- add_explanation(["Program creates a service object and adds it to the specified service control manager database."]).
	mi_score(100, "StartService") :- add_explanation(["Program starts a service."]).
	
	% suspicious APIs
	mi_score(20, "OpenProcessToken") :- add_explanation(["Program adjusts its access token."]).
	mi_score(20, "LookupPrivilegeValueA") :- add_explanation(["Program adjusts its access token."]).
	mi_score(20, "AdjustTokenPrivileges") :- add_explanation(["Program adjusts its access token."]).
	mi_score(20, "CreateToolhelp32Snapshot") :- add_explanation(["Program searches through running processes."]).
	mi_score(30, "Process32First") :- add_explanation(["Program searches through running processes."]).
	mi_score(30, "Process32Next") :- add_explanation(["Program searches through running processes."]).
	mi_score(50, "SetWindowsHookEx") :- add_explanation(["Program sets a hook."]).
	mi_score(50, "UnhookWindowsHookEx") :- add_explanation(["Program removes a hook."]).
	mi_score(20, "ZwUnmapViewOfSection") :- add_explanation(["Program releases memory in system."]).
	mi_score(35, "NtQuerySystemInformation") :- add_explanation(["Program retrieves some system information."]).
	mi_score(35, "NtQueryInformationProcess") :- add_explanation(["Program retrieves some system process information."]).
	mi_score(35, "NtQueryInformationThread") :- add_explanation(["Program retrieves some system thread information."]).
	mi_score(35, "NtQueryInformationFile") :- add_explanation(["Program retrieves some system file information."]).
	mi_score(35, "NtQueryInformationKey") :- add_explanation(["Program retrieves some system key information."]).
	
	
	mi_score(0, _).
	
	i_score(X, Y) :- X = [H|T], i_score(T, Z), mi_score(V, H),!, Y is V+Z.
	i_score([], 0).
	
% score keterhubungan (suatu kombinasi API akan menghasilkan skor lebih tinggi dibanding API tersebut jalan secara terpisah2)
	ml_score(70, ["NtQueryObject", "ObjectAllTypesInformation"]) :- add_explanation(["Program looks for DebugObjects which can point to a debugger being present."]).
	ml_score(70, ["NtSetInformationThread", "HideThreadFromDebugger"]) :- add_explanation(["Program hides from debugger."]).
	ml_score(70, ["ZwSetInformationThread", "HideThreadFromDebugger"]) :- add_explanation(["Program hides from debugger."]).
	ml_score(70, ["SamIConnect", "SamrQueryInformationUser", "SamIGetPrivateData"]) :- add_explanation(["Program grabs user's password."]).
	ml_score(100, ["GetKeyState", "GetForegroundWindow"]) :- add_explanation(["Program does keylogging activity."]).
	ml_score(100, ["CreateService", "StartService"]) :- add_explanation(["Program creates and starts a suspicious service."]).
	ml_score(70, ["OpenProcessToken", "LookupPrivilegeValueA", "AdjustTokenPrivileges"]) :- add_explanation(["Program tries to escalate privileges."]).
	ml_score(70, ["CreateToolhelp32Snapshot", "Process32First", "Process32Next"]) :- add_explanation(["Program looks for a specific process (to do process injection on, detect a process running i.e. a debugger or anti-virus)."]).
	ml_score(100, ["SetWindowsHookEx", "UnhookWindowsHookEx"]) :- add_explanation(["Program sets and unsets a hook, usually used to hook root kits."]).
	
	al_score(X, Y) :- ml_score(Y, L), subset(L, X).
	l_score(X, Y) :- aggregate(sum(C), al_score(X, C), Y), !. 
	l_score(_, 0).

% Fungsi untuk menghitung score total
	score(X, Y) :- del_explanation, i_score(X, Z), l_score(X, V), Y is Z+V.