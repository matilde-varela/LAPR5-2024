:- dynamic availability/3.
:- dynamic agenda_staff/3.
:- dynamic agenda_staff1/3.
:- dynamic agenda_operation_room/3.
:- dynamic agenda_operation_room1/3.
:- dynamic better_sol/5. %dynamic and auxiliary fact to storethe better solution in a certain moment
:- dynamic final_time_heuristics/1.
:- dynamic earliest_surgery/3.

% DATA
agenda_staff(d001,20241028,[(720,790,m01),(1080,1140,c01)]).
agenda_staff(d002,20241028,[(850,900,m02),(901,960,m02),(1380,1440,c02)]).
agenda_staff(d003,20241028,[(720,790,m01),(910,980,m02)]).
%agenda_staff(d004,20241028,[(850,900,m02),(940,980,c04)]).

timetable(d001,20241028,(480,1200)).
timetable(d002,20241028,(500,1440)).
timetable(d003,20241028,(520,1320)).
%timetable(d004,20241028,(620,1020)).

surgery(so2,45,60,45).
surgery(so3,45,90,45).
surgery(so4,45,75,45).

agenda_operation_room(or1,20241028,[(750, 780, mnt0001), (1080, 1110, mnt0002)]).

surgery_id(so100001,so2).
surgery_id(so100002,so3).
surgery_id(so100003,so4).
%surgery_id(so100004,so2).
%surgery_id(so100005,so4).
%surgery_id(so100006,so2).
%surgery_id(so100007,so3).
%surgery_id(so100008,so2).
%surgery_id(so100009,so2).
%surgery_id(so100010,so2).
%surgery_id(so100011,so4).
%surgery_id(so100012,so2).
%surgery_id(so100013,so2).

assignment_surgery(so100001,d001).
assignment_surgery(so100002,d002).
assignment_surgery(so100003,d003).
%assignment_surgery(so100004,d001).
%assignment_surgery(so100004,d002).
%assignment_surgery(so100005,d002).
%assignment_surgery(so100005,d003).
%assignment_surgery(so100006,d001).
%assignment_surgery(so100007,d003).
%assignment_surgery(so100008,d004).
%assignment_surgery(so100008,d003).
%assignment_surgery(so100009,d002).
%assignment_surgery(so100009,d004).
%assignment_surgery(so100010,d003).
%assignment_surgery(so100011,d001).
%assignment_surgery(so100012,d001).
%assignment_surgery(so100013,d004).
% -------------------------------------------------------------------


%-GETS THE FREE TIME FROM OCCUPIED TIME INTERVALS--------------------
% free_agenda0(list-occupied-time-intervals,list-free-time-intervals).
free_agenda0([],[(0,1440)]).
free_agenda0([(0,Tfin,_)|LT],LT1):-!,free_agenda1([(0,Tfin,_)|LT],LT1).
free_agenda0([(Tin,Tfin,_)|LT],[(0,T1)|LT1]):- T1 is Tin-1,
    free_agenda1([(Tin,Tfin,_)|LT],LT1).

free_agenda1([(_,Tfin,_)],[(T1,1440)]):-Tfin\==1440,!,T1 is Tfin+1.
free_agenda1([(_,_,_)],[]).
free_agenda1([(_,T,_),(T1,Tfin2,_)|LT],LT1):-Tx is T+1,T1==Tx,!,
    free_agenda1([(T1,Tfin2,_)|LT],LT1).
free_agenda1([(_,Tfin1,_),(Tin2,Tfin2,_)|LT],[(T1,T2)|LT1]):-T1 is Tfin1+1,T2 is Tin2-1,
    free_agenda1([(Tin2,Tfin2,_)|LT],LT1).
% -------------------------------------------------------------------

% ADAPTING A TIMETABLE SO THAT THE FREE TIMES CORRESPOND TO THE TIMES THE PROFESSIONALS WORK FOR.
% adapt_timetable(doctorId, date, list-free-time, list-free-time-during-working-hours).
adapt_timetable(D,Date,LFA,LFA2):-timetable(D,Date,(InTime,FinTime)),treatin(InTime,LFA,LFA1),treatfin(FinTime,LFA1,LFA2).
treatin(InTime,[(In,Fin)|LFA],[(In,Fin)|LFA]):-InTime=<In,!.
treatin(InTime,[(_,Fin)|LFA],LFA1):-InTime>Fin,!,treatin(InTime,LFA,LFA1).
treatin(InTime,[(_,Fin)|LFA],[(InTime,Fin)|LFA]).
treatin(_,[],[]).
treatfin(FinTime,[(In,Fin)|LFA],[(In,Fin)|LFA1]):-FinTime>=Fin,!,treatfin(FinTime,LFA,LFA1).
treatfin(FinTime,[(In,_)|_],[]):-FinTime=<In,!.
treatfin(FinTime,[(In,_)|_],[(In,FinTime)]).
treatfin(_,[],[]).
% -------------------------------------------------------------------

% INTERSECTION OF AGENDAS OF FREE TIME INTERVALS OF ALL DOCTORS
% intersect_all_agendas(list-staff-id, date, list-time-intervals-available-for-surgery)
intersect_all_agendas([Name],Date,LA):-!,availability(Name,Date,LA).
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
% -------------------------------------------------------------------



% AVAILABILITY OF OPERATIONS WILL CREATE A LIST OF POSSIBILITIES IN THE OPERATION ROOM
% availability_all_surgeries(list-operation-id, room-id, date)

availability_all_surgeries([],_,_).
availability_all_surgeries([OpCode|LOpCode],Room,Day):-
    surgery_id(OpCode,OpType),
    surgery(OpType,_,TSurgery,_),
    availability_operation(OpCode,Room,Day,LPossibilities,LDoctors),
    schedule_first_interval(TSurgery,LPossibilities,(TinS,TfinS)), % will need to change this because right now it is only scheduling the first available slot
    retract(agenda_operation_room1(Room,Day,Agenda)),
    insert_agenda((TinS,TfinS,OpCode),Agenda,Agenda1),
    assertz(agenda_operation_room1(Room,Day,Agenda1)),
    insert_agenda_doctors((TinS,TfinS,OpCode),Day,LDoctors),
    availability_all_surgeries(LOpCode,Room,Day).
% -------------------------------------------------------------------

% GENERATE A LIST OF POSSIBLE TIMES FOR AN OPERATION TO OCCUR IN A SPECIFIED ROOM ON A GIVEN DAY
availability_operation(OpCode,Room,Day,LPossibilities,LDoctors):-
    surgery_id(OpCode,OpType),
    surgery(OpType,_,TSurgery,_),
    findall(Doctor,assignment_surgery(OpCode,Doctor),LDoctors),
    intersect_all_agendas(LDoctors,Day,LA),
    agenda_operation_room1(Room,Day,LAgenda),
    free_agenda0(LAgenda,LFAgRoom),
    intersect_2_agendas(LA,LFAgRoom,LIntAgDoctorsRoom),
    remove_unf_intervals(TSurgery,LIntAgDoctorsRoom,LPossibilities).
% -------------------------------------------------------------------

% REMOVES UNFEASIBLE TIME INTERVALS FROM THE RESULTING AGENDA
% remove_unf_intervals(surgery-time, list-intersected-agendas-doctor, list-availability).
remove_unf_intervals(_,[],[]).
remove_unf_intervals(TSurgery,[(Tin,Tfin)|LA],[(Tin,Tfin)|LA1]):-DT is Tfin-Tin+1,TSurgery=<DT,!,
    remove_unf_intervals(TSurgery,LA,LA1).
remove_unf_intervals(TSurgery,[_|LA],LA1):- remove_unf_intervals(TSurgery,LA,LA1).
% -------------------------------------------------------------------

% SCHEDULES THE FIRST AVAILABLE TIME INTERVAL
schedule_first_interval(TSurgery,[(Tin,_)|_],(Tin,TfinS)):-
    TfinS is Tin + TSurgery - 1.
% -------------------------------------------------------------------

% INSERTS OPERATION INTO THE DOCTORS AND ROOM AGENDA
insert_agenda((TinS,TfinS,OpCode),[],[(TinS,TfinS,OpCode)]).
insert_agenda((TinS,TfinS,OpCode),[(Tin,Tfin,OpCode1)|LA],[(TinS,TfinS,OpCode),(Tin,Tfin,OpCode1)|LA]):-TfinS<Tin,!.
insert_agenda((TinS,TfinS,OpCode),[(Tin,Tfin,OpCode1)|LA],[(Tin,Tfin,OpCode1)|LA1]):-insert_agenda((TinS,TfinS,OpCode),LA,LA1).

insert_agenda_doctors(_,_,[]).
insert_agenda_doctors((TinS,TfinS,OpCode),Day,[Doctor|LDoctors]):-
    retract(agenda_staff1(Doctor,Day,Agenda)),
    insert_agenda((TinS,TfinS,OpCode),Agenda,Agenda1),
    assert(agenda_staff1(Doctor,Day,Agenda1)),
    insert_agenda_doctors((TinS,TfinS,OpCode),Day,LDoctors).
% -------------------------------------------------------------------


%-OBTAINS THE BETTER SOLUTION FOR THE SCHEDULING OF OPERATIONS ------
obtain_better_sol(Room,Day,AgOpRoomBetter,LAgDoctorsBetter,TFinOp):-
    get_time(Ti),
    (obtain_better_sol1(Room,Day);true),
    retract(better_sol(Day,Room,AgOpRoomBetter,LAgDoctorsBetter,TFinOp)),
    write('Final Result: AgOpRoomBetter='),write(AgOpRoomBetter),nl,
    write('LAgDoctorsBetter='),write(LAgDoctorsBetter),nl,
    write('TFinOp='),write(TFinOp),nl,
    get_time(Tf),
    T is Tf-Ti,
    write('Tempo de geracao da solucao:'),write(T),nl.

%-Generations all solutions, one by one, to compare with the better and update better_sol/5
obtain_better_sol1(Room,Day):-
    asserta(better_sol(Day,Room,_,_,1441)),
    findall(OpCode,surgery_id(OpCode,_),LOC),!,
    permutation(LOC,LOpCode),
    retractall(agenda_staff1(_,_,_)),
    retractall(agenda_operation_room1(_,_,_)),
    retractall(availability(_,_,_)),
    findall(_,(agenda_staff(D,Day,Agenda),assertz(agenda_staff1(D,Day,Agenda))),_),
    agenda_operation_room(Room,Day,Agenda),assert(agenda_operation_room1(Room,Day,Agenda)),
    findall(_,(agenda_staff1(D,Day,L),free_agenda0(L,LFA),adapt_timetable(D,Day,LFA,LFA2),assertz(availability(D,Day,LFA2))),_),
    availability_all_surgeries(LOpCode,Room,Day),
    agenda_operation_room1(Room,Day,AgendaR),
    update_better_sol(Day,Room,AgendaR,LOpCode),
    fail. %This is a a fail that is purposefully placed to force to backtrack till permutation/2 to generate the next sequence of surgeries.
% -------------------------------------------------------------------

%- UPDATES BETTER SOLUTION FACT -------------------------------------
update_better_sol(Day,Room,Agenda,LOpCode):-
    better_sol(Day,Room,_,_,FinTime), % Current better solution
    reverse(Agenda,AgendaR),
    evaluate_final_time(AgendaR,LOpCode,FinTime1),
    FinTime1<FinTime,
    retract(better_sol(_,_,_,_,_)),
    findall(Doctor,assignment_surgery(_,Doctor),LDoctors1),
    remove_equals(LDoctors1,LDoctors),
    list_doctors_agenda(Day,LDoctors,LDAgendas),
    asserta(better_sol(Day,Room,Agenda,LDAgendas,FinTime1)).
% -------------------------------------------------------------------

%-EVALUATES LIST OF OP CODES-----------------------------------------
% Finds the Tfin (end time) of the first operation that matches one of the operation codes in LOpCode. If no matching operation is found, Tfin defaults to 1441.

evaluate_final_time([],_,1441).
evaluate_final_time([(_,Tfin,OpCode)|_],LOpCode,Tfin):-member(OpCode,LOpCode),!.
evaluate_final_time([_|AgR],LOpCode,Tfin):-evaluate_final_time(AgR,LOpCode,Tfin).
% -------------------------------------------------------------------


%-OBTAINS DOCTOR AGENDAS---------------------------------------------
list_doctors_agenda(_,[],[]).
list_doctors_agenda(Day,[D|LD],[(D,AgD)|LAgD]):-agenda_staff1(D,Day,AgD),list_doctors_agenda(Day,LD,LAgD).
% -------------------------------------------------------------------

%-REMOVES EQUALS (SAME DOC CAN APPEAR TWICE)-------------------------
remove_equals([],[]).
remove_equals([X|L],L1):-member(X,L),!,remove_equals(L,L1).
remove_equals([X|L],[X|L1]):-remove_equals(L,L1).
% -------------------------------------------------------------------
%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%

obtain_heuristic_solution(Room,Day,AgOpRoomBetter,LAgDoctorsBetter,TFinOp):-
    get_time(Ti),
    (obtain_heuristic_solution1(Room,Day),!),
    retract(better_sol(Day,Room,AgOpRoomBetter,LAgDoctorsBetter,TFinOp)),
    write('Final Result: AgOpRoomBetter='),write(AgOpRoomBetter),nl,
    write('LAgDoctorsBetter='),write(LAgDoctorsBetter),nl,
    write('TFinOp='),write(TFinOp),nl,
    get_time(Tf),
    T is Tf-Ti,
    write('Tempo de geracao da solucao:'),write(T),nl.

obtain_heuristic_solution1(Room,Day):-
    findall(OpCode,surgery_id(OpCode,_),LOC),!, 
    retractall(agenda_staff1(_,_,_)),
    retractall(agenda_operation_room1(_,_,_)),
    retractall(availability(_,_,_)),
    retractall(final_time_heuristics(_)),
    findall(_,(agenda_staff(D,Day,Agenda),assertz(agenda_staff1(D,Day,Agenda))),_),
    agenda_operation_room(Room,Day,Agenda),assert(agenda_operation_room1(Room,Day,Agenda)),
    findall(_,(agenda_staff1(D,Day,L),free_agenda0(L,LFA),adapt_timetable(D,Day,LFA,LFA2),assertz(availability(D,Day,LFA2))),_),
    availability_early_surgeries(LOC, Room, Day),
    retract(final_time_heuristics(TFinOp)),
    agenda_operation_room1(Room,Day,AgendaR),
    findall(Doctor,assignment_surgery(_,Doctor),LDoctors1),
    remove_equals(LDoctors1,LDoctors),
    list_doctors_agenda(Day,LDoctors,LAgDoctorsHeuristic),
    asserta(better_sol(Day,Room,AgendaR,LAgDoctorsHeuristic,TFinOp)).


availability_early_surgeries([], _, _).
availability_early_surgeries(LOpCode, Room, Day):-
    retractall(earliest_surgery(_, _, _)),  
    asserta(earliest_surgery(_, 1441, _)),
    find_earliest_surgery(LOpCode, Room, Day), !,
    earliest_surgery(OpCode, TinS, LDoctors),
    (
        TinS == 1441,
        select(OpCode, LOpCode, LRestOpCode),
            nl, write('Info: could not schedule operation='), write(OpCode), nl,
        availability_early_surgeries(LRestOpCode, Room, Day)
    ;
        surgery_id(OpCode, OpType),
        surgery(OpType, _, TSurgery, _),
        TfinS is TinS + TSurgery - 1,
        retractall(final_time_heuristics(_)), 
        asserta(final_time_heuristics(TfinS)),
        retract(agenda_operation_room1(Room, Day, Agenda)),
        insert_agenda((TinS, TfinS, OpCode), Agenda, Agenda1),
        assertz(agenda_operation_room1(Room, Day, Agenda1)), 
        insert_agenda_doctors((TinS, TfinS, OpCode), Day, LDoctors),
        select(OpCode, LOpCode, LRestOpCode),
        availability_early_surgeries(LRestOpCode, Room, Day)
    ). 

% -------------------------------------------------------------------

% GENERATE A LIST OF POSSIBLE TIMES FOR AN OPERATION TO OCCUR IN A SPECIFIED ROOM ON A GIVEN DAY
find_earliest_surgery([], _, _).
find_earliest_surgery([OpCode|LOpCode], Room, Day) :- 
    availability_operation(OpCode, Room, Day, LPossibilities, LDoctors),
    (LPossibilities == [],
        find_earliest_surgery(LOpCode, Room, Day)
    ;
        surgery_id(OpCode, OpType),
        surgery(OpType, _, TSurgery, _),
        schedule_first_interval(TSurgery, LPossibilities, (TinS, _)),
        earliest_surgery(_, EarliestSurgeryTime, _),
        ((TinS < EarliestSurgeryTime,
            retractall(earliest_surgery(_, _, _)),
            asserta(earliest_surgery(OpCode, TinS, LDoctors)),!
        ); true),
        find_earliest_surgery(LOpCode, Room, Day)
    ).

%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
%%% Heuristic 2 %%%
obtain_heuristic_highest_occupancy_solution(Room,Day,AgOpRoomBetter,LAgDoctorsBetter,TFinOp):-
    get_time(Ti),
    (obtain_heuristic_highest_occupancy_solution1(Room,Day),!),
    retract(better_sol(Day,Room,AgOpRoomBetter,LAgDoctorsBetter,TFinOp)),
    write('Final Result: AgOpRoomBetter='),write(AgOpRoomBetter),nl,
    write('LAgDoctorsBetter='),write(LAgDoctorsBetter),nl,
    write('TFinOp='),write(TFinOp),nl,
    get_time(Tf),
    T is Tf-Ti,
    write('Tempo de geracao da solucao:'),write(T),nl.

obtain_heuristic_highest_occupancy_solution1(Room,Day):-
    asserta(better_sol(Day,Room,_,_,0)),
    findall(OpCode,surgery_id(OpCode,_),LOC),!,
    retractall(agenda_staff1(_,_,_)),
    retractall(agenda_operation_room1(_,_,_)),
    retractall(availability(_,_,_)),
    findall(_,(agenda_staff(D,Day,Agenda),assertz(agenda_staff1(D,Day,Agenda))),_),
    agenda_operation_room(Room,Day,Agenda),assert(agenda_operation_room1(Room,Day,Agenda)),
    findall(_,(agenda_staff1(D,Day,L),free_agenda0(L,LFA),adapt_timetable(D,Day,LFA,LFA2),assertz(availability(D,Day,LFA2))),_),
    find_surgery_by_highest_occupancy(LOC, Room, Day),
    retract(better_sol(_,_,_,_,FinTime)),
    agenda_operation_room1(Room,Day,AgendaR),
    findall(Doctor,assignment_surgery(_,Doctor),LDoctors1),
    remove_equals(LDoctors1,LDoctors),
    list_doctors_agenda(Day,LDoctors,LAgDoctorsHeuristic),
    asserta(better_sol(Day,Room,AgendaR,LAgDoctorsHeuristic,FinTime)).

find_surgery_by_highest_occupancy([], _, _).
find_surgery_by_highest_occupancy(LOpCodes, Room, Day) :- 
    obtain_assignment_surgeries(LOpCodes,LDoctorsInvolved),
    calculate_doctor_highest_occupancy_percentage(Day,LDoctorsInvolved,DocHighestPerc),
    obtain_assignment_surgeries(LOpCodesHighOccp,[DocHighestPerc]),
    find_first_opcode_in_list(LOpCodesHighOccp, LOpCodes, OpCode),
    availability_operation(OpCode, Room, Day, LPossibilities, LDoctors),
    (LPossibilities == [],
        select(OpCode, LOpCodes, LRestOpCode),
        write('>> INFO: could not schedule surgery='), write(OpCode), nl,
        find_surgery_by_highest_occupancy(LRestOpCode, Room, Day)
    ;
        surgery_id(OpCode, OpType),     
        surgery(OpType, _, TSurgery, _),
        schedule_first_interval(TSurgery,LPossibilities,(TinS,TfinS)),
        better_sol(Day,Room,_,_,FinTime),
        ((TfinS > FinTime,
            retractall(better_sol(_,_,_,_,_)),
            asserta(better_sol(Day,Room,_,_,TfinS))
        ); true),  
        retract(agenda_operation_room1(Room,Day,Agenda)),
        insert_agenda((TinS,TfinS,OpCode),Agenda,Agenda1),
        assertz(agenda_operation_room1(Room,Day,Agenda1)),
        insert_agenda_doctors((TinS,TfinS,OpCode),Day,LDoctors),      
        select(OpCode, LOpCodes, LRestOpCode),
        find_surgery_by_highest_occupancy(LRestOpCode, Room, Day)
    ).


find_first_opcode_in_list([OpCode|Rest], LOpCodes, Result) :-
    (
        member(OpCode, LOpCodes), 
        Result = OpCode
    ;  
        find_first_opcode_in_list(Rest, LOpCodes, Result)
    ).

% Get all doctors for a list of specific OpCodes or all OpCodes for a list of doctors
obtain_assignment_surgeries(OpCodes, DoctorsInvolved) :-
    (   is_list(OpCodes), 
        findall(Doctor, (member(OpCode, OpCodes), assignment_surgery(OpCode, Doctor)), Doctors),!,
        list_to_set(Doctors, DoctorsInvolved)
    ;   is_list(DoctorsInvolved),
        findall(OpCode, (member(Doctor, DoctorsInvolved), assignment_surgery(OpCode, Doctor)), OpCodesList),
        list_to_set(OpCodesList, OpCodes)
    ).

calculate_doctor_highest_occupancy_percentage(Day, Doctors, DocHighest):-
        findall((Doctor, Percentage), (member(Doctor, Doctors), calculate_occupancy_percentage(Day, Doctor, Percentage)), DoctorPercentages),
        max_member((DocHighest, _), DoctorPercentages).

calculate_occupancy_percentage(Day, Doctor, Percentage):-
    calculate_remaining_surgery_time(Doctor, TotalTime),
    calculate_free_time(Day, Doctor, FreeTime),
    ((FreeTime > 0, Percentage is (TotalTime / FreeTime) * 100 ); Percentage is 100).

calculate_free_time(Day, Doctor, FreeTime) :-
    availability(Doctor, Day, AvailableSlots),
    sum_free_time(AvailableSlots, FreeTime). 

calculate_remaining_surgery_time(Doctor, TotalTime) :-
    findall(Duration, (assignment_surgery(OpCode, Doctor), surgery_id(OpCode, OpType), surgery(OpType, _, Duration, _)), SurgeryDurations), % Obtains list with the surgeries duration for the specified doctor 
    sumlist(SurgeryDurations, TotalTime). % sums all the elements of the list

sum_free_time([], 0).
sum_free_time([(TimeInicial, TimeFinal)|Rest], TotalFreeTime) :-
    FreeTime is TimeFinal - TimeInicial, % Calculates the free time of the current slot
    sum_free_time(Rest, RemainingFreeTime), % Recursively call to calculate the free time of the remaining slots
    TotalFreeTime is FreeTime + RemainingFreeTime.  % Adds the current free time to the total free time

