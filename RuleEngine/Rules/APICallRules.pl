:- dynamic explanation/1.
explanation([]).

tambah_explanation(New) :- explanation(L1), append(L1, New, L2), retractall(explanation(L1)), asserta(explanation(L2)).
hapus_explanation :- retractall(explanation(_)), asserta(explanation([])).


% score individual
	mi_score(120, "ANDRE") :- tambah_explanation(["Program mengakses reg"]).
	mi_score(20, "SUSANTO") :- tambah_explanation(["Program mengakses file system"]).
	mi_score(20, "METAL") :- tambah_explanation(["Program mengakses metal"]).
	mi_score(0, _).
	
	i_score(X, Y) :- X = [H|T], i_score(T, Z), mi_score(V, H),!, Y is V+Z.
	i_score([], 0).
	
% score keterhubungan (suatu kombinasi API akan menghasilkan skor lebih tinggi dibanding API tersebut jalan secara terpisah2)
	ml_score(10, ["ANDRE", "METAL"]) :- tambah_explanation(["Program diduga mengirim data rekaman ke internet"]).
	ml_score(10, ["ANDRE", "SUSANTO"]) :- tambah_explanation(["Program diduga memanfaatkan fitur ABC"]).
	
	al_score(X, Y) :- ml_score(Y, L), subset(L, X).
	l_score(X, Y) :- aggregate(sum(C), al_score(X, C), Y), !. 
	l_score(_, 0).

% Fungsi untuk menghitung score total
	score(X, Y) :- hapus_explanation, i_score(X, Z), l_score(X, V), Y is Z+V.