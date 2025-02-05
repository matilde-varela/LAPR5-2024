:-dynamic generations/1.
:-dynamic population/1.
:-dynamic mix_percentage/1.
:-dynamic prob_crossover/1.
:-dynamic prob_mutation/1.
:-dynamic time_limit/1.
:-dynamic better/4.
%%
:- dynamic availability/3.
:- dynamic agenda_staff1/3.
:- dynamic agenda_staff_aux/3.
:- dynamic agenda_operation_room/3.
:- dynamic agenda_operation_room1/3.
:- dynamic agenda_operation_room_aux/3.
%%
:-dynamic occupied_time/2. % occupied_time(roomId, time).
:-dynamic operation_assigment/2. % operation_assigment(opId, roomId).
:-dynamic room_in_scheduling/1.
:-dynamic surgeries/1.

reference_value(1100).
time_limit(10).
population(2).
generations(10).
prob_crossover(0.5).
prob_mutation(0.25).
mix_percentage(0.2).

staff(d001,doctor,orthopaedist,[so2,so3,so4]).
staff(d002,doctor,orthopaedist,[so2,so3,so4]).
staff(d003,doctor,orthopaedist,[so2,so3,so4]).
staff(d004,doctor,anaesthetist,[so2,so3,so4]).
staff(n001,nurse,instrumenting,[so2,so3,so4]).
staff(n002,nurse,circulating,[so2,so3,so4]). 
staff(n003,nurse,anaesthetist,[so2,so3,so4]).
staff(a001,assistant,medical,[so2,so3,so4]).

agenda_staff(d001,20241028,[(720,790,m01),(1080,1140,c01)]).
agenda_staff(d002,20241028,[(850,900,m02),(901,960,m03),(1380,1440,c02)]).
agenda_staff(d003,20241028,[(720,790,m01),(910,980,m02)]).
agenda_staff(d004,20241028,[(940,980,c04)]).
agenda_staff(n001,20241028,[(720,790,m01),(1380,1440,c02)]).
agenda_staff(n002,20241028,[(850,900,m02),(940,980,c04)]).
agenda_staff(n003,20241028,[(1010,1070,m01),(1080,1140,c01)]).
agenda_staff(a001,20241028,[(850,900,m02),(1380,1440,c02)]).

timetable(d001,20241028,(480,1200)).
timetable(d002,20241028,(500,1440)).
timetable(d003,20241028,(530,1320)).
timetable(d004,20241028,(300,1400)).

timetable(n001,20241028,(480,1200)).
timetable(n002,20241028,(500,1400)).
timetable(n003,20241028,(400,1320)).
timetable(a001,20241028,(500,1440)).

agenda_operation_room(r101,20241028,[(720, 850, mnt0002)]). %180 + 30
agenda_operation_room(r102,20241028,[(720, 850, mnt0002),(1080, 1110, cnt1003)]). %60 + 30
agenda_operation_room(r301,20241028,[(720, 780, mnt0002), (1080, 1110, cnt1003)]). %60 + 30

surgery(so2,45,60,45).
surgery(so3,45,90,45).
surgery(so4,45,75,45).

surgery_id(so100001,so2).
surgery_id(so100002,so3).
surgery_id(so100003,so3).
surgery_id(so100004,so3).
surgery_id(so100005,so2).
surgery_id(so100006,so3).
surgery_id(so100007,so3).
surgery_id(so100008,so3).

assignment_surgery(so100001,d001).%
assignment_surgery(so100001,d004).%
assignment_surgery(so100001,n001).%
assignment_surgery(so100001,a001).%

assignment_surgery(so100002,d002).
assignment_surgery(so100002,d004).
assignment_surgery(so100002,n002).
assignment_surgery(so100002,a001).

assignment_surgery(so100003,d003).
assignment_surgery(so100003,d004).
assignment_surgery(so100003,n003).
assignment_surgery(so100003,a001).

assignment_surgery(so100004,d003).
assignment_surgery(so100004,d004).
assignment_surgery(so100004,n003).
assignment_surgery(so100004,a001).

assignment_surgery(so100005,d003).
assignment_surgery(so100005,d004).
assignment_surgery(so100005,n003).
assignment_surgery(so100005,a001).

assignment_surgery(so100006,d003).
assignment_surgery(so100006,d004).
assignment_surgery(so100006,n003).
assignment_surgery(so100006,a001).

assignment_surgery(so100007,d003).
assignment_surgery(so100007,d004).
assignment_surgery(so100007,n003).
assignment_surgery(so100007,a001).

assignment_surgery(so100008,d003).
assignment_surgery(so100008,d004).
assignment_surgery(so100008,n003).
assignment_surgery(so100008,a001).



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

generate :-
    retractall(better(_,_,_,_)),
    asserta(better(_,1441,_,_)),
    generate_population(Pop),
    evaluate_population(Pop, PopValue),
    order_population(PopValue, PopOrd),
    generations(NG),
    get_time(Ti),
    (   generate_generation(Ti, 0, NG, PopOrd),
        better(_,VX,_,_),
        (VX \== 1441,update_staff_aux,update_operation_room_aux,!; true),!
    ; true), 
    !. 

update_staff_aux :-
    better(_, _, AgendaDoctors, _),
    forall(member((D, Agenda), AgendaDoctors),
           update_or_add_aux_staff(D, Agenda)).

update_or_add_aux_staff(D, NewAgenda) :-
    (   agenda_staff_aux(D, Day, ExistingAgenda)
    ->  merge_agendas(ExistingAgenda, NewAgenda, MergedAgenda),
        retract(agenda_staff_aux(D, Day, ExistingAgenda)),
        assertz(agenda_staff_aux(D, Day, MergedAgenda))
    ;   true
    ).

update_operation_room_aux :-
    room_in_scheduling(R),
    better(_, _, _, AgendaR),
    forall(member((Agenda), AgendaR),
            update_or_add_aux_room(R, Agenda)).

update_or_add_aux_room(R, NewAgenda) :-
    (   agenda_operation_room_aux(R, Day, ExistingAgenda)
    ->  merge_agendas(ExistingAgenda, [NewAgenda], MergedAgenda),
        retract(agenda_operation_room_aux(R, Day, ExistingAgenda)),
        assertz(agenda_operation_room_aux(R, Day, MergedAgenda))
    ;   true
    ).

merge_agendas(Existing, New, Merged) :-
    append(Existing, New, Combined),
    sort(Combined, Merged).
    
generate_generation(_, G, G, _).
generate_generation(T,N,G,Pop):-  
    (((evaluate_time(T); evaluate_reference_value),true);
    random_permutation(Pop, RandomPermPop),
    crossover(RandomPermPop,NPop1),
    mutation(NPop1,NPop),
    evaluate_population(NPop,NPopValue),
    population(PopSize),
    select_population(PopSize,Pop,NPopValue,FPop),!,
    order_population(FPop,NPopOrd),
    (update_best_individual(NPopOrd);true),
    (equal_pop(Pop, NPopOrd),N1 is N+1; N1 is 0),
    generate_generation(T,N1,G,NPopOrd)).

generate_population(Pop):-
    population(PopSize),
    surgeries(NumT),
    room_in_scheduling(Room),
    findall(Id,operation_assigment(Id,Room),SurgeryList),
    generate_population(PopSize,SurgeryList,NumT,Pop).

generate_population(0,_,_,[]):-!.  %%% PROBLEMS WHEN INDIVDUALS ARE <= 3
generate_population(PopSize,SurgeryList,NumT,[Ind|Rest]):-
    PopSize1 is PopSize-1,
    generate_population(PopSize1,SurgeryList,NumT,Rest),
    generate_individual(SurgeryList,NumT,Ind),
    not(member(Ind,Rest)).
generate_population(PopSize,SurgeryList,NumT,L):-
    generate_population(PopSize,SurgeryList,NumT,L).
    
generate_individual([G],1,[G]):-!.
generate_individual(TasksList,NumT,[G|Rest]):-
    NumTemp is NumT + 1,
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

evaluate(Seq, V):- 
    retractall(agenda_staff1(_, _, _)),
    retractall(agenda_operation_room1(_, _, _)),
    retractall(availability(_, _, _)),
    room_in_scheduling(Room),
    findall(_, (agenda_staff_aux(D, Day, Agenda), assertz(agenda_staff1(D, Day, Agenda))), _),
    agenda_operation_room_aux(Room, Day, Agenda), assert(agenda_operation_room1(Room, Day, Agenda)),
    findall(_, (agenda_staff_aux(D, Day, L), free_agenda0(L, LFA), adapt_timetable(D, Day, LFA, LFA2), assertz(availability(D, Day, LFA2))), _),
    (can_schedule_surgeries(Seq,Room,Day,V),!;V is 1441).

can_schedule_surgeries([], _, _, V):- V = 0, !.
can_schedule_surgeries([OpCode|LOpCode], Room, Day, Tfin):-
    surgery_id(OpCode, OpType),
    surgery(OpType, TAnesthesia, TSurgery, TCleaning),
    availability_operation_changed2(OpCode, Room, Day, LPossibilities, LStaff), !,
    schedule_first_interval(TAnesthesia, TSurgery, TCleaning, LPossibilities, (TinS, TfinS)),
    retract(agenda_operation_room1(Room, Day, Agenda)),
    insert_agenda((TinS, TfinS, OpCode), Agenda, Agenda1),
    assertz(agenda_operation_room1(Room, Day, Agenda1)),
    include(is_doctor, LStaff, LSurgeons),
    include(is_anaesthetist, LStaff, LAnesth),
    include(is_assistant, LStaff, LCleaners),   
    insert_agenda_anesthesia_team((TinS, TAnesthesia, TSurgery, OpCode), Day, LAnesth),
    insert_agenda_surgery_team((TinS, TAnesthesia, TSurgery, OpCode), Day, LSurgeons),
    insert_agenda_cleaning_team((TinS, TAnesthesia, TSurgery, TCleaning, OpCode), Day, LCleaners),
    can_schedule_surgeries(LOpCode, Room, Day, TfinRecursive),
    Tfin is max(TfinS, TfinRecursive).

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

update_best_individual([X*VX|_]):-
    update_best_individual1(X*VX).

update_best_individual1(X*VX):-
    better(_,Value,_,_),
    Value > VX,
    retractall(better(_,_,_,_)),
    obtain_schedule(X),
    findall(Doctor,assignment_surgery(_,Doctor),LDoctors1),
    remove_equals(LDoctors1,LDoctors),
    get_staff_agenda(LDoctors,LDAgendas),
    room_in_scheduling(Room),
    agenda_operation_room1(Room,_,AgendaR),
    asserta(better(X,VX,LDAgendas,AgendaR)).

obtain_schedule(VX):-
    evaluate(VX,_).

%-OBTAINS DOCTOR AGENDAS---------------------------------------------
get_staff_agenda([],[]).
get_staff_agenda([D|LD],[(D,AgD)|LAgD]):-agenda_staff1(D,_,AgD),get_staff_agenda(LD,LAgD).


evaluate_time(T):-
    time_limit(Limit),
    get_time(Ti),
    Tf is Ti - T,
    Tf > Limit.

evaluate_reference_value:-
    reference_value(Value), 
    better(_,V,_,_),
    V =< Value.

generate_crossover_points(P1,P2):- generate_crossover_points1(P1,P2).

generate_crossover_points1(P1,P2):-
	surgeries(N),
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

rotate_right(L,K,L1):- surgeries(N),
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
    surgeries(T),
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
    surgeries(NumT),
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

%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
%%%% PREDICATES FROM SPRINT C %%%%
%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
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
    Rest is PopSize - max(1, round(PopSize * Percentage)),
    random_select_rest_pop(Rest,RestPop,RandomPop),
    append(TopPop,RandomPop,ResultPop).

random_select_rest_pop(Rest, RestPop, RandomPop) :-
    random_select_elements(Rest, RestPop, RandomPop).
random_select_elements(0, _, []) :- !. % Base case: when Rest is 0, no more elements to select
random_select_elements(N, Pop, [E|FinalRandomPop]) :-
    N > 0,
    random_select(E, Pop, RestPop), % Select a random element from List
    N1 is N - 1,
    random_select_elements(N1, RestPop, FinalRandomPop).

% Base case: Empty population results in an empty final population
choose_top_individuals([], []).
choose_top_individuals(Pop, FinalPop) :-
    mix_percentage(Percentage), % Get the percentage value
    population(PopSize), % Calculate the size of the population
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
remove_duplicates_helper([], _, []). 
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


%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
%%%% PREDICATES TO DISTRIBUTE OPS %%%%
%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
schedule_operations_in_rooms([]).
schedule_operations_in_rooms([Room | RestRooms]):-
    retractall(room_in_scheduling(_)),
    assert(room_in_scheduling(Room)),
    findall(Id,operation_assigment(Id,Room),SurgeryList),
    (
        length(SurgeryList, Len), Len >= 2,
        (
            retractall(surgeries(_)),
            asserta(surgeries(Len)),
            generate,
            better(X,VX,AgS,AgR),
            (
                VX \== 1441, 
                write("Alert: was able to schedule all surgeries of room "), write(Room),write(">> Schedule"),write(X),nl,
                write('Staff='),write(AgS),nl,write('Room='),write(AgR),nl,!
            ; 
                write(">> Warning: was not able to schedule surgeries of room "),write(Room), write(" Therefore, the following surgeries were not scheduled: "),write(SurgeryList),nl
            )
        ),!
    ;
       (    
            not(SurgeryList = []),
            write(">> Warning: this algorithm needs at least 2 surgeries in a room to work. "),
            write("The following surgeries were not scheduled: "),write(SurgeryList),nl
        ; 
            write('No Surgeries assigned to room '),write(Room),nl
       )
    ),
    schedule_operations_in_rooms(RestRooms).

distribute_rooms(Day) :-
    retractall(agenda_operation_room_aux(_,_,_)),
    retractall(agenda_staff_aux(_,_,_)),
    retractall(occupied_time(_, _)),
    retractall(operation_assigment(_, _)),
    findall(_,(agenda_staff(D,Day,Agenda),assertz(agenda_staff_aux(D,Day,Agenda))),_),
    findall(_,(agenda_operation_room(R,Day,Agenda),assertz(agenda_operation_room_aux(R,Day,Agenda))),_),
    findall(OpCode, surgery_id(OpCode, _), OperationsList),
    findall(Room, agenda_operation_room(Room, Day, _), RoomsList),    % Find all rooms with agendas
    assert_occupied_times(RoomsList),     % Calculate and assert occupied time for each room
    distribute_operations_round_robin1(OperationsList, RoomsList),!,
    schedule_operations_in_rooms(RoomsList).


remove_equals([],[]).
remove_equals([X|L],L1):-member(X,L),!,remove_equals(L,L1).
remove_equals([X|L],[X|L1]):-remove_equals(L,L1).


distribute_operations_round_robin1([], _).
distribute_operations_round_robin1([Operation | RemainingOps], RoomsList) :-
    (    attempt_assign_to_rooms(Operation, RoomsList),  distribute_operations_round_robin1(RemainingOps, RoomsList)  % Move to the next operation
    ;   write('Failed to assign operation: '), write(Operation), nl  % Handle unassignable operation
    ).

attempt_assign_to_rooms(_, [], false):- fail.
attempt_assign_to_rooms(Operation, [Room | RemainingRooms]) :-
    check_current_room_occupancy_ratio(Room, Ratio),
    (   Ratio < 0.8,
        associate_operation_in_room(Operation, Room)
    ;   attempt_assign_to_rooms(Operation, RemainingRooms)
    ).
    
assert_occupied_times([]).
assert_occupied_times([Room | RestRooms]) :-
    agenda_operation_room(Room, _, Agenda),
    get_total_time_sum_in_intervals(Agenda, 0, TotalTime),  % Calculate the total occupied time for the room
    assert(occupied_time(Room, TotalTime)),  % Assert the occupied time as a dynamic fact
    assert_occupied_times(RestRooms).

get_total_time_sum_in_intervals([], Acc, Acc).
get_total_time_sum_in_intervals([(Start, End, _) | Agenda], Acc, TotalTimeOccupied) :-
    integer(Start), integer(End),
    SlotTime is End - Start,
    UpdatedAcc is Acc + SlotTime,
    get_total_time_sum_in_intervals(Agenda, UpdatedAcc, TotalTimeOccupied).
get_total_time_sum_in_intervals([(Start, End) | FreeIntervals], Acc, TotalTime) :-
    integer(Start), integer(End),
    SlotTime is End - Start,
    UpdatedAcc is Acc + SlotTime,
    get_total_time_sum_in_intervals(FreeIntervals, UpdatedAcc, TotalTime).

check_current_room_occupancy_ratio(Room, Ratio) :-
    (   occupied_time(Room, OccupTime),
        TotalCapacity is 1440,
        TotalCapacity > OccupTime,
        FreeTime is 1440 - OccupTime,
        Ratio is (OccupTime / FreeTime)
    ;   Ratio = 1  % Assume full occupancy if conditions fail
    ).

associate_operation_in_room(Operation, Room) :-
    surgery_id(Operation, OpType),
    surgery(OpType, PrepTime, Duration, CleanupTime),
    OpTotalTime is PrepTime + Duration + CleanupTime,
    retract(occupied_time(Room, OldOccupTime)),
    NewOccupTime is OldOccupTime + OpTotalTime,
    assert(occupied_time(Room, NewOccupTime)),
    retractall(operation_assigment(Operation, _)),
    assert(operation_assigment(Operation, Room)).


%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
%%%% PREDICATES FROM SPRINT B %%%%
%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%

availability_operation_changed2(OpCode, Room, Day, LPossibilities, LParticipants) :-
    surgery_id(OpCode, OpType),
    surgery(OpType, TAnesthesia, TSurgery, TCleaning),
    findall(Staff,assignment_surgery(OpCode, Staff),LAllStaff),
    include(is_doctor, LAllStaff, LSurgeons),
    include(is_anaesthetist, LAllStaff, LAnesth),
    include(is_assistant, LAllStaff, LCleaners),
    intersect_all_agendas(LSurgeons, Day, LFreeSurgeons),
    intersect_all_agendas(LAnesth, Day, LFreeAnesth),
    intersect_all_agendas(LCleaners, Day, LFreeCleaners),
    agenda_operation_room1(Room, Day, LRoomAgenda),
    free_agenda0(LRoomAgenda, LFreeRoom),
    TotalTime is TAnesthesia + TSurgery + TCleaning,
    filter_full_intervals1(LFreeRoom, TotalTime, FullIntervals),
    TimeAnesthSurgery is TAnesthesia + TSurgery,
    filter_by_availability(FullIntervals, LFreeAnesth, TimeAnesthSurgery, IntervalsAnesth),
    filter_by_surgery_availability(IntervalsAnesth, LFreeSurgeons, TAnesthesia, TSurgery, IntervalsSurgeons),
    filter_by_cleaning_availability(IntervalsSurgeons, LFreeCleaners, TAnesthesia, TSurgery, TCleaning, LPossibilities),
    append(LAnesth, LSurgeons, TempParticipants),
    append(TempParticipants, LCleaners, LParticipants).


free_agenda0([],[(0,1440)]).
free_agenda0([(0,Tfin,_)|LT],LT1):-
    !,
    free_agenda1([(0,Tfin,_)|LT],LT1).
free_agenda0([(Tin,Tfin,_)|LT],[(0,T1)|LT1]):- T1 is Tin-1,
    free_agenda1([(Tin,Tfin,_)|LT],LT1).
free_agenda1([(_,Tfin,_)],[(T1,1440)]):-Tfin\==1440,!,T1 is Tfin+1.
free_agenda1([(_,_,_)],[]).
free_agenda1([(_,T,_),(T1,Tfin2,_)|LT],LT1):-Tx is T+1,T1==Tx,!,
    free_agenda1([(T1,Tfin2,_)|LT],LT1).
free_agenda1([(_,Tfin1,_),(Tin2,Tfin2,_)|LT],[(T1,T2)|LT1]):-T1 is Tfin1+1,T2 is Tin2-1,
    free_agenda1([(Tin2,Tfin2,_)|LT],LT1).

adapt_timetable(D,Date,LFA,LFA2):-
    timetable(D,Date,(InTime,FinTime)),
    treatin(InTime,LFA,LFA1),
    treatfin(FinTime,LFA1,LFA2).
treatin(InTime,[(In,Fin)|LFA],[(In,Fin)|LFA]):-InTime=<In,!.
treatin(InTime,[(_,Fin)|LFA],LFA1):-InTime>Fin,!,treatin(InTime,LFA,LFA1).
treatin(InTime,[(_,Fin)|LFA],[(InTime,Fin)|LFA]).
treatin(_,[],[]).
treatfin(FinTime,[(In,Fin)|LFA],[(In,Fin)|LFA1]):-FinTime>=Fin,!,treatfin(FinTime,LFA,LFA1).
treatfin(FinTime,[(In,_)|_],[]):-FinTime=<In,!.
treatfin(FinTime,[(In,_)|_],[(In,FinTime)]).
treatfin(_,[],[]).

intersect_all_agendas([Name],Date,LA):-!,
    availability(Name,Date,LA).
intersect_all_agendas([Name|LNames],Date,LI):-
    availability(Name,Date,LA),
    intersect_all_agendas(LNames,Date,LI1),
    intersect_2_agendas(LA,LI1,LI).

intersect_2_agendas([],_,[]).
intersect_2_agendas([D|LD],LA,LIT):-	
    intersect_availability(D,LA,LI,LA1),
	intersect_2_agendas(LD,LA1,LID),
	append(LI,LID,LIT).

intersect_availability((_,_),[],[],[]).
intersect_availability((_,Fim),[(Ini1,Fim1)|LD],[],[(Ini1,Fim1)|LD]):-
		Fim<Ini1,!.
intersect_availability((Ini,Fim),[(_,Fim1)|LD],LI,LA):-
		Ini>Fim1,!,
		intersect_availability((Ini,Fim),LD,LI,LA).
intersect_availability((Ini,Fim),[(Ini1,Fim1)|LD],[(Imax,Fmin)],[(Fim,Fim1)|LD]):-
		Fim1>Fim,!,
		min_max(Ini,Ini1,_,Imax),
		min_max(Fim,Fim1,Fmin,_).
intersect_availability((Ini,Fim),[(Ini1,Fim1)|LD],[(Imax,Fmin)|LI],LA):-
		Fim>=Fim1,!,
		min_max(Ini,Ini1,_,Imax),
		min_max(Fim,Fim1,Fmin,_),
		intersect_availability((Fim1,Fim),LD,LI,LA).
min_max(I,I1,I,I1):- I<I1,!.
min_max(I,I1,I1,I).

is_doctor(Staff) :-
    ( staff(Staff, doctor, _, _);
     staff(Staff, nurse, _, _)),
     \+ staff(Staff, _, anaesthetist, _), 
     \+ staff(Staff, assistant, medical, _).
     
is_anaesthetist(Staff) :-
    staff(Staff, _, anaesthetist, _).
 
is_assistant(Staff) :-
    staff(Staff, assistant, medical, _).

filter_full_intervals1([], _, []).
filter_full_intervals1([(Start,End)|Rest], TotalTime, AllSubIntervals) :-
    findall((SubStart,SubEnd), (
        between(Start, End, SubStart),
        SubEnd is SubStart + TotalTime - 1,
        SubEnd =< End
    ), SubIntervals),
    filter_full_intervals1(Rest, TotalTime, FilteredRest),
    append(SubIntervals, FilteredRest, AllSubIntervals).

filter_by_availability([], _, _, []).
filter_by_availability([(Start, End) | Rest], LFreeAnesth, TimeNeeded, [(NewStart, End) | FilteredRest]) :-
    EndAnesth is Start + TimeNeeded - 1,
    EndAnesth =< End,
    check_availability(Start, EndAnesth, LFreeAnesth),
    NewStart is Start,
    filter_by_availability(Rest, LFreeAnesth, TimeNeeded, FilteredRest).
filter_by_availability([_|Rest], LFreeAnesth, TimeNeeded, FilteredRest) :-
    filter_by_availability(Rest, LFreeAnesth, TimeNeeded, FilteredRest).

filter_by_surgery_availability([], _, _, _, []).
filter_by_surgery_availability([(Start,End)|Rest], LFreeSurgeons, TAnesthesia, TSurgery, [(NewStart,End)|FilteredRest]) :-
    StartSurgery is Start + TAnesthesia - 1,
    EndSurgery is StartSurgery + TSurgery - 1,
    EndSurgery =< End,
    check_availability(StartSurgery, EndSurgery, LFreeSurgeons),
    NewStart is Start,
    filter_by_surgery_availability(Rest, LFreeSurgeons, TAnesthesia, TSurgery, FilteredRest).
filter_by_surgery_availability([_|Rest], LFreeSurgeons, TAnesthesia, TSurgery, FilteredRest) :-
    filter_by_surgery_availability(Rest, LFreeSurgeons, TAnesthesia, TSurgery, FilteredRest).

filter_by_cleaning_availability([], _, _, _, _, []).
filter_by_cleaning_availability([(Start,End)|Rest], LFreeCleaners, TAnesthesia, TSurgery, TCleaning, [(NewStart,End)|FilteredRest]) :-
    StartCleaning is Start + TAnesthesia + TSurgery - 1,
    EndCleaning is StartCleaning + TCleaning - 1,
    EndCleaning =< End,
    check_availability(StartCleaning, EndCleaning, LFreeCleaners),
    NewStart is Start,
    filter_by_cleaning_availability(Rest, LFreeCleaners, TAnesthesia, TSurgery, TCleaning, FilteredRest).
filter_by_cleaning_availability([_|Rest], LFreeCleaners, TAnesthesia, TSurgery, TCleaning, FilteredRest) :-
    filter_by_cleaning_availability(Rest, LFreeCleaners, TAnesthesia, TSurgery, TCleaning, FilteredRest).

check_availability(Start, End, FreeIntervals) :-
    member((StartFree,EndFree), FreeIntervals),
    Start >= StartFree,
    End =< EndFree.

schedule_first_interval(TAnesthesia,TSurgery,TCleaning,[(Tin,_)|_],(Tin,TfinS)):-
    TfinS is Tin + TSurgery + TAnesthesia + TCleaning - 1.

insert_agenda((TinS,TfinS,OpCode),[],[(TinS,TfinS,OpCode)]).
insert_agenda((TinS,TfinS,OpCode),[(Tin,Tfin,OpCode1)|LA],[(TinS,TfinS,OpCode),(Tin,Tfin,OpCode1)|LA]):-TfinS<Tin,!.
insert_agenda((TinS,TfinS,OpCode),[(Tin,Tfin,OpCode1)|LA],[(Tin,Tfin,OpCode1)|LA1]):-insert_agenda((TinS,TfinS,OpCode),LA,LA1).

insert_agenda_doctors(_,_,[]).
insert_agenda_doctors((TinS,TfinS,OpCode),Day,[Doctor|LDoctors]):-
    retract(agenda_staff1(Doctor,Day,Agenda)),
    insert_agenda((TinS,TfinS,OpCode),Agenda,Agenda1),
    assert(agenda_staff1(Doctor,Day,Agenda1)),
    insert_agenda_doctors((TinS,TfinS,OpCode),Day,LDoctors).

insert_agenda_anesthesia_team(_,_,[]).
insert_agenda_anesthesia_team((TinS,TAnesthesia,TSurgery,OpCode),Day,[Doctor|LDoctors]):-
    retract(agenda_staff1(Doctor,Day,Agenda)),
    TfinS is TinS + TAnesthesia + TSurgery - 1,
    insert_agenda((TinS,TfinS,OpCode),Agenda,Agenda1),
    assert(agenda_staff1(Doctor,Day,Agenda1)),
    insert_agenda_anesthesia_team((TinS,TAnesthesia,TSurgery,OpCode),Day,LDoctors).

insert_agenda_surgery_team(_,_,[]).
insert_agenda_surgery_team((TinS,TAnesthesia,TSurgery,OpCode),Day,[Doctor|LDoctors]):-
    retract(agenda_staff1(Doctor,Day,Agenda)),
    TfinS is TinS + TSurgery + TAnesthesia - 1,
    TinSNew is TinS + TAnesthesia - 1,
    insert_agenda((TinSNew,TfinS,OpCode),Agenda,Agenda1),
    assert(agenda_staff1(Doctor,Day,Agenda1)),
    insert_agenda_surgery_team((TinS,TAnesthesia,TSurgery,OpCode),Day,LDoctors).

insert_agenda_cleaning_team(_,_,[]).
insert_agenda_cleaning_team((TinS,TAnesthesia,TSurgery,TCleaning,OpCode),Day,[Doctor|LDoctors]):-
    retract(agenda_staff1(Doctor,Day,Agenda)),
    TinSNew is TinS + TAnesthesia + TSurgery - 1,
    TfinS is TinS + TSurgery + TAnesthesia + TCleaning - 1,
    insert_agenda((TinSNew,TfinS,OpCode),Agenda,Agenda1),
    assert(agenda_staff1(Doctor,Day,Agenda1)),
    insert_agenda_cleaning_team((TinS,TfinS,OpCode),Day,LDoctors).


