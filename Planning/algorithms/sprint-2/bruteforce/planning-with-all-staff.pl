:- dynamic availability/3.%:- retractall(availability(_,_,_)).
:- dynamic availability/3.
:- dynamic agenda_staff/3.
:- dynamic agenda_staff1/3.
:- dynamic agenda_operation_room/3.
:- dynamic agenda_operation_room1/3.
:- dynamic better_sol/5. %dynamic and auxiliary fact to storethe better solution in a certain moment
:- dynamic final_time_heuristics/1.
:- dynamic earliest_surgery/3.

% DATA
staff(d001,doctor,orthopaedist,[so2,so3,so4]).
staff(d002,doctor,orthopaedist,[so2,so3,so4]).
staff(d003,doctor,orthopaedist,[so2,so3,so4]).
staff(d004,doctor,anaesthetist,[so2,so3,so4]).
staff(n001,nurse,instrumenting,[so2,so3,so4]).
staff(n002,nurse,circulating,[so2,so3,so4]). 
staff(n003,nurse,anaesthetist,[so2,so3,so4]).
staff(a001,assistant,medical,[so2,so3,so4]).

surgery(so2,45,60,45).
surgery(so3,45,90,45).
surgery(so4,45,75,45).

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
timetable(n002,20241028,(500,1140)).
timetable(n003,20241028,(400,1320)).
timetable(a001,20241028,(500,1440)).

surgery_id(so100001,so2).
surgery_id(so100002,so3).
%surgery_id(so100003,so3).
%surgery_id(so100004,so3).

assignment_surgery(so100001,d001).
assignment_surgery(so100001,d004).
assignment_surgery(so100001,n001).
assignment_surgery(so100001,a001).


assignment_surgery(so100002,d002).
assignment_surgery(so100002,d004).
assignment_surgery(so100002,n002).
assignment_surgery(so100002,a001).

%assignment_surgery(so100003,d003).
%assignment_surgery(so100003,d004).
%assignment_surgery(so100003,n003).
%assignment_surgery(so100003,a001).


%assignment_surgery(so100004,d001).
%assignment_surgery(so100004,d002).
%assignment_surgery(so100005,d002).
%assignment_surgery(so100005,d003).
%assignment_surgery(so100006,d001).
%assignment_surgery(so100007,d003).
%assignment_surgery(so100008,d004). %%ALERT
%assignment_surgery(so100008,d003).
%assignment_surgery(so100009,d002).
%assignment_surgery(so100009,d004).
%assignment_surgery(so100010,d003).
%assignment_surgery(so100011,d001).
%assignment_surgery(so100012,d001).
%assignment_surgery(so100013,d004).
% -------------------------------------------------------------------

agenda_operation_room(or1,20241028,[(750, 780, mnt0001), (1080, 1110, mnt0002)]).

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

% CHECKS IF THERE ARE ANY FREE AGENDAS FOR ALL STAFF
% find_free_agendas(date).

find_free_agendas(Date):-
    retractall(availability(_,_,_)),
    findall(_,(agenda_staff(D,Date,L),
    free_agenda0(L,LFA),
    adapt_timetable(D,Date,LFA,LFA2),
    assertz(availability(D,Date,LFA2))),_).
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
    surgery(OpType,TAnesthesia,TSurgery,TCleaning),
    availability_operation_changed2(OpCode,Room,Day,LPossibilities,LStaff),!,
    schedule_first_interval(TAnesthesia,TSurgery,TCleaning,LPossibilities,(TinS,TfinS)), % will need to change this because right now it is only scheduling the first available slot
    retract(agenda_operation_room1(Room,Day,Agenda)),
    insert_agenda((TinS,TfinS,OpCode),Agenda,Agenda1),
    assertz(agenda_operation_room1(Room,Day,Agenda1)),
    surgery_id(OpCode, OpType),
    surgery(OpType, TAnesthesia, TSurgery, TCleaning),
    findall(Anesth, (staff(Anesth, _, anaesthetist, OpTypes), member(Anesth, LStaff)), LAnesth),
    findall(Cleaner, (staff(Cleaner,assistant,medical,OpTypes), member(Cleaner, LStaff)), LCleaners),
    subtract(LStaff, LAnesth, LStaffWOAnesth),
    subtract(LStaffWOAnesth, LCleaners, LSurgeons),
    insert_agenda_anesthesia_team((TinS,TAnesthesia,TSurgery,OpCode),Day,LAnesth),
    insert_agenda_surgery_team((TinS,TAnesthesia,TSurgery,OpCode),Day,LSurgeons),
    insert_agenda_cleaning_team((TinS,TAnesthesia,TSurgery,TCleaning,OpCode),Day,LCleaners),
    availability_all_surgeries(LOpCode,Room,Day).
% -------------------------------------------------------------------

% GENERATE A LIST OF POSSIBLE TIMES FOR AN OPERATION TO OCCUR IN A SPECIFIED ROOM ON A GIVEN DAY
availability_operation_changed2(OpCode, Room, Day, LPossibilities, LParticipants) :-
    surgery_id(OpCode, OpType),
    surgery(OpType, TAnesthesia, TSurgery, TCleaning),
    findall(Staff,assignment_surgery(OpCode, Staff),LAllStaff),
    include(is_doctor, LAllStaff, LSurgeons),
    include(is_anaesthetist, LAllStaff, LAnesth),
    include(is_assistant, LAllStaff, LCleaners),
    % Compute availability for each staff group
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

% --- Helper predicates --- %
% Define predicates to categorize the roles
is_doctor(Staff) :-
    ( staff(Staff, doctor, _, _);
     staff(Staff, nurse, _, _)),
     \+ staff(Staff, _, anaesthetist, _),  % Exclude anaesthetists
     \+ staff(Staff, assistant, medical, _).  % Exclude assistants
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
% -------------------------------------------------------------------

% SCHEDULES THE FIRST AVAILABLE TIME INTERVAL
schedule_first_interval(TAnesthesia,TSurgery,TCleaning,[(Tin,_)|_],(Tin,TfinS)):-
    TfinS is Tin + TSurgery + TAnesthesia +TCleaning - 1.
% -------------------------------------------------------------------

% INSERTS OPERATION INTO THE DOCTORS AND ROOM AGENDA
insert_agenda((TinS,TfinS,OpCode),[],[(TinS,TfinS,OpCode)]).
insert_agenda((TinS,TfinS,OpCode),[(Tin,Tfin,OpCode1)|LA],[(TinS,TfinS,OpCode),(Tin,Tfin,OpCode1)|LA]):-TfinS<Tin,!.
insert_agenda((TinS,TfinS,OpCode),[(Tin,Tfin,OpCode1)|LA],[(Tin,Tfin,OpCode1)|LA1]):-insert_agenda((TinS,TfinS,OpCode),LA,LA1).

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
    findall(_,(agenda_staff(D,Day,Agenda),
    assertz(agenda_staff1(D,Day,Agenda))),_),
    agenda_operation_room(Room,Day,Agenda),
    assert(agenda_operation_room1(Room,Day,Agenda)),
    findall(_,(agenda_staff1(D,Day,L),free_agenda0(L,LFA),adapt_timetable(D,Day,LFA,LFA2),assertz(availability(D,Day,LFA2))),_),
    availability_all_surgeries(LOpCode,Room,Day),
    agenda_operation_room1(Room,Day,AgendaR),
    write('AgendaR'),write(AgendaR),nl,
    update_better_sol(Day,Room,AgendaR,LOpCode),
    fail. %This is a a fail that is purposefully placed to force to backtrack till permutation/2 to generate the next sequence of surgeries.
% -------------------------------------------------------------------

%- UPDATES BETTER SOLUTION FACT -------------------------------------
update_better_sol(Day,Room,Agenda,LOpCode):-
    better_sol(Day,Room,_,_,FinTime), % Current better solution
    reverse(Agenda,AgendaR),
    evaluate_final_time(AgendaR,LOpCode,FinTime1),
    write('Analysing for LOpCode='),write(LOpCode),nl,
    write('now: FinTime1='),write(FinTime1),write(' Agenda='),write(Agenda),nl,
    FinTime1<FinTime,
    write('best solution updated'),nl,
    % Update the better solution:
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