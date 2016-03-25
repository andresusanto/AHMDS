:- dynamic explanation/1.
explanation([]).

% score individual
	mi_score(120, "ANDRE").
	mi_score(20, "SUSANTO").
	mi_score(20, "METAL").
	mi_score(0, _).
	
	i_score(X, Y) :- X = [H|T], i_score(T, Z), mi_score(V, H),!, Y is V+Z.
	i_score([], Y) :- Y is 0.
	
% score keterhubungan (suatu kombinasi API akan menghasilkan skor lebih tinggi dibanding API tersebut jalan secara terpisah2)
	ml_score(10, ["ANDRE", "METAL"]).
	ml_score(10, ["ANDRE", "SUSANTO"]).
	
	al_score(X, Y) :- ml_score(Y, L), subset(L, X).
	l_score(X, Y) :- aggregate(sum(C), al_score(X, C), Y). 
	

% Fungsi untuk menghitung score total
	score(X, Y) :- i_score(X, Z), Y is Z.