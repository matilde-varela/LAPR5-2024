:-dynamic generations/1.
:-dynamic population/1.
:-dynamic mix_percentage/1.
:-dynamic prob_crossover/1.
:-dynamic prob_mutation/1.
:-dynamic time_limit/1.
:-dynamic better/2.

% task(Id,ProcessTime,DueTime,PenaltyWeight).
task(t1,2,5,1).
task(t2,4,7,6).
task(t3,4,7,6).

% tasks(NTasks).
tasks(3).

% parameters initialization
reference_value(10).
time_limit(20).
population(3).
generations(3).
prob_crossover(0.5).
prob_mutation(0.25).
mix_percentage(0.2).

initialize:-write('Number of generations to consider stabilization: '),read(NG), 			
    (retract(generations(_));true), asserta(generations(NG)),
	write('Population size: '),read(PS),
	(retract(population(_));true), asserta(population(PS)),
    write('Time Limit: '),read(T),
	(retract(time_limit(_));true), asserta(time_limit(T)),
	write('Probability of crossover (%):'), read(P1),
	PC is P1/100, 
	(retract(prob_crossover(_));true), 	asserta(prob_crossover(PC)),
    write('Percentage of individuals retention (%):'), read(P2),
	MP is P2/100, 
    (retract(mix_percentage(_));true), 	asserta(mix_percentage(MP)),
	write('Probability of mutation (%):'), read(P3),
	PM is P3/100, 
	(retract(prob_mutation(_));true), asserta(prob_mutation(PM)).

generate:-
    %initialize,
    retractall(better(_,_)),
    asserta(better(_,1441)),
    generate_population(Pop),
    write('Pop='),write(Pop),nl,
    evaluate_population(Pop,PopValue),
    write('PopValue='),write(PopValue),nl,
    order_population(PopValue,PopOrd),
    generations(NG),
    get_time(Ti),
    (generate_generation(Ti,0,NG,PopOrd);true),!,
    better(X,VX),
    write("Best="),write(X),nl,
    write("Value="),write(VX).

generate_generation(_, G, G, _) :-
    write('Estabilization achieved.'),nl.
    
generate_generation(T,N,G,Pop):-  
    (
        (
            (
                evaluate_time(T), write('Surpassed time limit.'),nl
            ;
                evaluate_reference_value(), write('Surpassed reference value.'),nl
            )
        ,
            true
        )
    ;
        random_permutation(Pop, RandomPermPop),
        crossover(RandomPermPop,NPop1),
        mutation(NPop1,NPop),
        evaluate_population(NPop,NPopValue),
        population(PopSize),
        select_population(PopSize,Pop,NPopValue,FPop),!,
        order_population(FPop,NPopOrd),
        (update_best_individual(NPopOrd);true),
        (equal_pop(Pop, NPopOrd),N1 is N+1; N1 is 0),
        generate_generation(T,N1,G,NPopOrd)
    ).
    
generate_population(Pop):-
    population(PopSize),
    tasks(NumT),
    findall(Task,task(Task,_,_,_),TasksList),

    generate_population(PopSize,TasksList,NumT,Pop).

generate_population(0,_,_,[]):-!.
generate_population(PopSize,TasksList,NumT,[Ind|Rest]):-
    PopSize1 is PopSize-1,
    generate_population(PopSize1,TasksList,NumT,Rest),
    generate_individual(TasksList,NumT,Ind),
    not(member(Ind,Rest)).
generate_population(PopSize,TasksList,NumT,L):-
    generate_population(PopSize,TasksList,NumT,L).
    
generate_individual([G],1,[G]):-!.
generate_individual(TasksList,NumT,[G|Rest]):-
    NumTemp is NumT + 1, % to use with random
    random(1,NumTemp,N),
    remove(N,TasksList,G,NewList),
    NumT1 is NumT-1,
    generate_individual(NewList,NumT1,Rest).

remove(1,[G|Rest],G,Rest).
remove(N,[G1|Rest],G,[G1|Rest1]):- N1 is N-1,
            remove(N1,Rest,G,Rest1).


evaluate_population([],[]).
evaluate_population([Ind|Rest],[Ind*V|Rest1]):-
    evaluate(Ind,V),
    evaluate_population(Rest,Rest1).

evaluate(Seq,V):- evaluate(Seq,0,V).

evaluate([ ],_,0).   %% NEEDS TO BE CHANGED -- TO CALCULATE FINAL TIME SURGERIES
evaluate([T|Rest],Inst,V):-
    task(T,Dur,Due,Pen),
    FinInst is Inst+Dur,
    evaluate(Rest,FinInst,VRest),
    ((FinInst =< Due,!, VT is 0) ; (VT is (FinInst-Due)*Pen)),
    V is VT+VRest.


order_population(PopValue,PopValueOrd):-
    bsort(PopValue,PopValueOrd).

bsort([X],[X]):-!.
bsort([X|Xs],Ys):-
    bsort(Xs,Zs),
    bchange([X|Zs],Ys).


bchange([X],[X]):-!.

bchange([X*VX,Y*VY|L1],[Y*VY|L2]):-
    VX>VY,!,
    bchange([X*VX|L1],L2).

bchange([X|L1],[X|L2]):-bchange(L1,L2).

%% NEEDS TO BE CHANGED

% 1st - Change way new population is formed [to take into account first individuals of the last pop and the rest of the new pop (random)]
% 2nd - Change finish conditions | add: time, stabilization of generations, value of fitness
% 3rd - Evaluate population 
% 4th - Use permutation so that crossover isn't always 1-2,3-4,etc...



update_best_individual([X*VX|_]):-
    better(_,Value),
    Value > VX,
    retractall(better(_,_)),
    asserta(better(X,VX)).

evaluate_time(T):-
    time_limit(Limit),
    get_time(Ti),
    Tf is Ti - T,
    Tf > Limit.

evaluate_reference_value():-
    reference_value(Value), 
    better(_,V),
    V =< Value.

generate_crossover_points(P1,P2):- generate_crossover_points1(P1,P2).

generate_crossover_points1(P1,P2):-
	tasks(N),
	NTemp is N+1,
	random(1,NTemp,P11),
	random(1,NTemp,P21),
	P11\==P21,!,
	((P11<P21,!,P1=P11,P2=P21);P1=P21,P2=P11).
generate_crossover_points1(P1,P2):-
	generate_crossover_points1(P1,P2).


crossover([ ],[ ]).
crossover([Ind*_],[Ind]).
crossover([Ind1*_,Ind2*_|Rest],[NInd1,NInd2|Rest1]):-
	generate_crossover_points(P1,P2),
	prob_crossover(Pcruz),random(0.0,1.0,Pc),
	(
        (Pc =< Pcruz,!,cross(Ind1,Ind2,P1,P2,NInd1),cross(Ind2,Ind1,P1,P2,NInd2))
	;
	    (NInd1=Ind1,NInd2=Ind2)
    ),
	crossover(Rest,Rest1).

fillh([ ],[ ]).

fillh([_|R1],[h|R2]):-
	fillh(R1,R2).

sublist(L1,I1,I2,L):-I1 < I2,!,
    sublist1(L1,I1,I2,L).

sublist(L1,I1,I2,L):-sublist1(L1,I2,I1,L).

sublist1([X|R1],1,1,[X|H]):-!, fillh(R1,H).

sublist1([X|R1],1,N2,[X|R2]):-!,N3 is N2 - 1,
	sublist1(R1,1,N3,R2).

sublist1([_|R1],N1,N2,[h|R2]):-N3 is N1 - 1,
		N4 is N2 - 1,
		sublist1(R1,N3,N4,R2).

rotate_right(L,K,L1):- tasks(N),
	T is N - K,
	rr(T,L,L1).

rr(0,L,L):-!.

rr(N,[X|R],R2):- N1 is N - 1,
	append(R,[X],R1),
	rr(N1,R1,R2).

remove([],_,[]):-!.

remove([X|R1],L,[X|R2]):- 
        not(member(X,L)),!,
        remove(R1,L,R2).

remove([_|R1],L,R2):-
    remove(R1,L,R2).

insert([],L,_,L):-!.
insert([X|R],L,N,L2):-
    tasks(T),
    ((N>T,!,N1 is N mod T);N1 = N),
    insert1(X,N1,L,L1),
    N2 is N + 1,
    insert(R,L1,N2,L2).


insert1(X,1,L,[X|L]):-!.
insert1(X,N,[Y|L],[Y|L1]):-
    N1 is N-1,
    insert1(X,N1,L,L1).

cross(Ind1,Ind2,P1,P2,NInd11):-
    sublist(Ind1,P1,P2,Sub1),
    tasks(NumT),
    R is NumT-P2,
    rotate_right(Ind2,R,Ind21),
    remove(Ind21,Sub1,Sub2),
    P3 is P2 + 1,
    insert(Sub2,Sub1,P3,NInd1),
    removeh(NInd1,NInd11).

removeh([],[]).

removeh([h|R1],R2):-!,
    removeh(R1,R2).

removeh([X|R1],[X|R2]):-
    removeh(R1,R2).

mutation([],[]).
mutation([Ind|Rest],[NInd|Rest1]):-
	prob_mutation(Pmut),
	random(0.0,1.0,Pm),
	((Pm < Pmut,!,mutacao1(Ind,NInd));NInd = Ind),
	mutation(Rest,Rest1).

mutacao1(Ind,NInd):-
	generate_crossover_points(P1,P2),
	mutacao22(Ind,P1,P2,NInd).

mutacao22([G1|Ind],1,P2,[G2|NInd]):-
	!, P21 is P2-1,
	mutacao23(G1,P21,Ind,G2,NInd).
mutacao22([G|Ind],P1,P2,[G|NInd]):-
	P11 is P1-1, P21 is P2-1,
	mutacao22(Ind,P11,P21,NInd).

mutacao23(G1,1,[G2|Ind],G2,[G1|Ind]):-!.
mutacao23(G1,P,[G|Ind],G2,[G|NInd]):-
	P1 is P-1,
	mutacao23(G1,P1,Ind,G2,NInd).


%% NEW PREDICATES
select_population(0,[_],[_],[_]).
select_population(PopSize,OldPop,NewPop,Pop):-
    append(OldPop,NewPop,AllPop),
    remove_duplicates(AllPop,UniqPopOrd),
    order_population(UniqPopOrd,PopOrd),
    select_individuals_for_pop(PopOrd,FinalPop),
    order_population(FinalPop,FinalPopOrd),
    take_n_elements(PopSize,FinalPopOrd,Pop).

select_individuals_for_pop(Pop,ResultPop):-
    choose_top_individuals(Pop,TopPop),
    remove(Pop,TopPop,RestPop),
    mix_percentage(Percentage),   
    population(PopSize),
    Rest is PopSize - round(PopSize*Percentage),
    random_select_rest_pop(Rest,RestPop,RandomPop),
    append(TopPop,RandomPop,ResultPop).

random_select_rest_pop(Rest, RestPop, RandomPop) :-
    random_select_elements(Rest, RestPop, RandomPop).

random_select_elements(0, _, []) :- !.   % Base case: when Rest is 0, no more elements to select
random_select_elements(N, Pop, [E|FinalRandomPop]) :-
    N > 0,
    random_select(E, Pop, RestPop),   % Select a random element from List
    N1 is N - 1,
    random_select_elements(N1, RestPop, FinalRandomPop).

% Base case: Empty population results in an empty final population
choose_top_individuals([], []).
choose_top_individuals(Pop, FinalPop) :-
    mix_percentage(Percentage),      % Get the percentage value
    population(PopSize),         % Calculate the size of the population
    N is max(1, round(PopSize * Percentage)), % Calculate the number of elements to take
    take_n_elements(N, Pop, FinalPop). % Get the first N elements from the population.

% Helper predicate to take the first N elements of a list
take_n_elements(0, _, []):-!. % Base case: If N is 0, result is an empty list.
take_n_elements(_, [], []):-!. % Base case: If the list is empty, result is empty.
take_n_elements(N, [X*VX | Rest], [X*VX | FinalRest]) :-
    N > 0, % Ensure N is positive
    N1 is N - 1, % Decrement N
    take_n_elements(N1, Rest, FinalRest). % Recurse with the rest of the list.

remove_duplicates(Population, UniquePopulation) :-
    remove_duplicates_helper(Population, [], UniquePopulation),!.

% Helper predicate to remove duplicates
remove_duplicates_helper([], _, []). % Base case: Empty population results in an empty list.

remove_duplicates_helper([X*_ | Rest], Seen, UniquePopulation) :-
    member(X, Seen), % Check if X has already been seen
    remove_duplicates_helper(Rest, Seen, UniquePopulation). % Skip duplicate.

remove_duplicates_helper([X*VX | Rest], Seen, [X*VX | UniqueRest]) :-
    \+ member(X, Seen), % X is not in Seen
    remove_duplicates_helper(Rest, [X | Seen], UniqueRest). % Add X to Seen and keep it in the result.


equal_pop([],[]):-!.
equal_pop([P1|Pop1],[P2|Pop2]):-
    P1=P2, 
    equal_pop(Pop1, Pop2).