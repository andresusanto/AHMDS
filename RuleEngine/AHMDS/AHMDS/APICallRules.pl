abolish.
:- dynamic explanation/1.
explanation([]).

add_explanation(New) :- explanation(L1), append(L1, New, L2), retractall(explanation(L1)), asserta(explanation(L2)).
del_explanation :- retractall(explanation(_)), asserta(explanation([])).


% score individual
	
	% Anti-reversing / Anti-debugging API
	mi_score(70, "IsDebuggerPresent") :- add_explanation([""]).
	mi_score(70, "CheckRemoteDebuggerPresent") :- add_explanation([""]).
	mi_score(70, "NTQueryInformationProcess") :- add_explanation([""]).
	mi_score(70, "OutputDebugString") :- add_explanation([""]).
	mi_score(70, "QueryPerformanceCounter") :- add_explanation([""]).
	mi_score(70, "GetTickCount") :- add_explanation([""]).
	mi_score(70, "timeGetTime") :- add_explanation([""]).
	mi_score(70, "NtQueryObject") :- add_explanation([""]).
	mi_score(70, "NtSetInformationThread") :- add_explanation([""]).
	mi_score(70, "ZwSetInformationThread") :- add_explanation([""]).
	mi_score(70, "ZwClose") :- add_explanation([""]).
	mi_score(70, "DebugActiveProcess") :- add_explanation([""]).
	
	% Password dumping API
	mi_score(20, "SamIConnect") :- add_explanation([""]).
	mi_score(20, "SamrQueryInformationUser") :- add_explanation([""]).
	mi_score(20, "SamIGetPrivateData") :- add_explanation([""]).
	mi_score(20, "SystemFunction025") :- add_explanation([""]).
	mi_score(20, "SystemFunction027") :- add_explanation([""]).
	mi_score(20, "LsaEnumerateLogonSessions") :- add_explanation([""]).
	
	% obfuscation api
	mi_score(20, "CryptAcquireContext") :- add_explanation([""]).
	
	% keylogging activity
	mi_score(120, "GetAsyncKeyState") :- add_explanation([""]).
	mi_score(120, "GetKeyState") :- add_explanation([""]).
	mi_score(120, "GetForegroundWindow") :- add_explanation([""]).
	
	
	% service related API
	mi_score(20, "OpenSCManagerW") :- add_explanation([""]).
	mi_score(20, "OpenSCManagerA") :- add_explanation([""]).
	mi_score(20, "CreateService") :- add_explanation([""]).
	mi_score(20, "StartService") :- add_explanation([""]).
	
	% suspicious APIs
	mi_score(50, "OpenProcessToken") :- add_explanation([""]).
	mi_score(50, "LookupPrivilegeValueA") :- add_explanation([""]).
	mi_score(50, "AdjustTokenPrivileges") :- add_explanation([""]).
	mi_score(50, "CreateToolhelp32Snapshot") :- add_explanation([""]).
	mi_score(50, "Process32First") :- add_explanation([""]).
	mi_score(50, "Process32Next") :- add_explanation([""]).
	mi_score(50, "SetWindowsHookEx") :- add_explanation([""]).
	mi_score(50, "UnhookWindowsHookEx") :- add_explanation([""]).
	mi_score(50, "ZwUnmapViewOfSection") :- add_explanation([""]).
	mi_score(50, "NtQuerySystemInformation") :- add_explanation([""]).
	mi_score(50, "NtQueryInformationProcess") :- add_explanation([""]).
	mi_score(50, "NtQueryInformationThread") :- add_explanation([""]).
	mi_score(50, "NtQueryInformationFile") :- add_explanation([""]).
	mi_score(50, "NtQueryInformationKey") :- add_explanation([""]).
	
	
	mi_score(0, _).
	
	i_score(X, Y) :- X = [H|T], i_score(T, Z), mi_score(V, H),!, Y is V+Z.
	i_score([], 0).
	
% score keterhubungan (suatu kombinasi API akan menghasilkan skor lebih tinggi dibanding API tersebut jalan secara terpisah2)
	ml_score(10, ["ANDRE", "METAL"]) :- add_explanation(["Program diduga mengirim data rekaman ke internet"]).
	ml_score(10, ["ANDRE", "SUSANTO"]) :- add_explanation(["Program diduga memanfaatkan fitur ABC"]).
	
	al_score(X, Y) :- ml_score(Y, L), subset(L, X).
	l_score(X, Y) :- aggregate(sum(C), al_score(X, C), Y), !. 
	l_score(_, 0).

% Fungsi untuk menghitung score total
	score(X, Y) :- del_explanation, i_score(X, Z), l_score(X, V), Y is Z+V.