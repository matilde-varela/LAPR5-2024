% Http libraries
:- use_module(library(http/thread_httpd)).
:- use_module(library(http/http_client)).
:- use_module(library(http/http_server)).
:- use_module(library(http/http_dispatch)).
:- use_module(library(http/http_parameters)).
:- use_module(library(http/http_open)).
:- use_module(library(http/http_cors)).
:- use_module(library(date)).
:- use_module(library(random)).

% JSON libraries
:- use_module(library(http/json_convert)).
:- use_module(library(http/http_json)).
:- use_module(library(http/json)).

% Used Predicates
:- dynamic availability/3.
:- dynamic agenda_staff/3.
:- dynamic agenda_staff1/3.
:- dynamic agenda_operation_room1/3.
:- dynamic better_sol/5.
:- dynamic final_time_heuristics/1.
:- dynamic earliest_surgery/3.
:- dynamic agenda_operation_room/3.
:- dynamic staff/4.
:- dynamic surgery/4.
:- dynamic agenda_staff/3.
:- dynamic timetable/3.
:- dynamic surgery_id/3.
:- dynamic assignment_surgery/3.

%%%%%%%%%%%%%%
%%% SERVER %%%
%%%%%%%%%%%%%%

% Example: 
% ?- initiate_server(8080)
% the server will start on localhost:8080, and it will be ready to receive HTTP requests routed through the http_dispatch handler.
initiate_server(Port) :-						
    http_server(http_dispatch, [port(Port)]).

stop_server(Port):-
    http_stop_server(Port,_).

%%% REQUEST - HEURISTIC HIGHEST OCCUPANCY
:- http_handler('/api/p/heuristic-highest-occupancy', heuristic_occupancy_handler, []).



heuristic_occupancy_handler(Request) :-  
    http_read_json_dict(Request,Dict,[]),
    process_json(Dict),
    (
        obtain_heuristic_highest_occupancy_solution(Room, Day, RoomSchedule, StaffSchedule, _),
        convert_list_of_slots(RoomSchedule, RoomJsonFormatted),
        with_output_to(atom(JsonRoomSchedule), json_write_dict(current_output, RoomJsonFormatted)),
        convert_to_json_format(StaffSchedule, StaffJsonFormatted),
        with_output_to(atom(JsonStaffSchedule), json_write_dict(current_output, StaffJsonFormatted)),
        Reply = json{
            'Room' : Room,
            'Day' : Day,
            'RoomSchedule:':JsonRoomSchedule,
            'StaffSchedule':JsonStaffSchedule},
        reply_json(Reply)
    ; 
        reply_json(json{'error': 'Failed to obtain a solution'}, [status(500)])
    ).

%%% REQUEST - HEURISTIC FIRST AVAILABLE
:- http_handler('/api/p/heuristic-first-available', heuristic_available_handler, []).

:- debug(heuristic_available_handler).
heuristic_available_handler(Request) :-  
    http_read_json_dict(Request,Dict,[]),
    process_json(Dict),
    (
        obtain_heuristic_solution(Room, Day, RoomSchedule, StaffSchedule, _),
        convert_list_of_slots(RoomSchedule, RoomJsonFormatted),
        with_output_to(atom(JsonRoomSchedule), json_write_dict(current_output, RoomJsonFormatted)),
        convert_to_json_format(StaffSchedule, StaffJsonFormatted),
        with_output_to(atom(JsonStaffSchedule), json_write_dict(current_output, StaffJsonFormatted)),
        Reply = json{
            'Room' : Room,
            'Day' : Day,
            'RoomSchedule:':JsonRoomSchedule,
            'StaffSchedule':JsonStaffSchedule},
        reply_json(Reply)
    ; 
        reply_json(json{'error': 'Failed to obtain a solution'}, [status(500)])
    ).

%%% REQUEST - BETTER SOLUTION
:- http_handler('/api/p/better-solution', better_sol_handler, [method(post)]).
better_sol_handler(Request) :-  
        http_read_json_dict(Request,Dict,[]),
        process_json(Dict),
        (
            obtain_better_sol(Room, Day, RoomSchedule, StaffSchedule, _),
            convert_list_of_slots(RoomSchedule, RoomJsonFormatted),
            with_output_to(atom(JsonRoomSchedule), json_write_dict(current_output, RoomJsonFormatted)),
            convert_to_json_format(StaffSchedule, StaffJsonFormatted),
            with_output_to(atom(JsonStaffSchedule), json_write_dict(current_output, StaffJsonFormatted)),
            Reply = json{
                'Room' : Room,
                'Day' : Day,
                'RoomSchedule:':JsonRoomSchedule,
                'StaffSchedule':JsonStaffSchedule},
            reply_json(Reply)
        ; 
            reply_json(json{'error': 'Failed to obtain a solution'}, [status(500)])
        ).

%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
%%%%%%%%%%%%%%%% Server Utils %%%%%%%%%%%%%%%%
%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
convert_list_of_slots([], []).
convert_list_of_slots([(Start, End, OperationId) | Rest], [SlotDict | RestFormatted]) :-
    SlotDict = _{start: Start, end: End, operationId: OperationId},
    convert_list_of_slots(Rest, RestFormatted).

convert_to_json_format([], []).
convert_to_json_format([(Id, Slots) | Rest], [JsonDict | RestFormatted]) :-
    convert_list_of_slots(Slots, SlotsFormatted),
    JsonDict = _{id: Id, slots: SlotsFormatted},
    convert_to_json_format(Rest, RestFormatted).
              
process_json(Dict):-
    retractall(availability(_,_,_)),
    retractall(agenda_staff(_,_,_)),
    retractall(agenda_staff1(_,_,_)),
    retractall(agenda_operation_room(_,_,_)),
    retractall(agenda_operation_room1(_,_,_)),
    retractall(better_sol(_,_,_,_,_)),
    retractall(final_time_heuristics(_)),
    retractall(earliest_surgery(_,_,_)),
    retractall(staff(_,_,_)),
    retractall(surgery(_,_,_,_)),
    retractall(agenda_staff(_,_,_)),
    retractall(timetable(_,_,_)),
    retractall(surgery_id(_,_)),
    retractall(assignment_surgery(_,_)),
    ListAgRoom = Dict.agRoom, process_rooms(ListAgRoom),
    ListStaff = Dict.staff, process_staff_list(ListStaff),
    ListSurgery = Dict.surgery, process_surgery_list(ListSurgery),
    ListAgStaff = Dict.agStaff, process_agstaff_list(ListAgStaff),
    ListTimetable = Dict.timetable, process_timetable(ListTimetable),
    ListSurgeryId = Dict.surgeryId, process_surgery_ids(ListSurgeryId),
    ListAssignSurgery = Dict.assignSurgery, process_assign_surgery(ListAssignSurgery).

process_assign_surgery([]).
process_assign_surgery([Entry | Rest]) :-
    OperationId = Entry.operationId,
    StaffId = Entry.staffId,
    assertz(assignment_surgery(OperationId, StaffId)),
    process_assign_surgery(Rest).

process_surgery_ids([]).
process_surgery_ids([Entry | Rest]) :-
    OperationId = Entry.operationId,
    SurgeryId = Entry.surgeryId,
    assertz(surgery_id(OperationId, SurgeryId)),
    process_surgery_ids(Rest).

process_timetable([]).
process_timetable([Entry | Rest]) :-
    StaffId = Entry.staffId,
    Date = Entry.date,
    Availability = Entry.availability,
    Start = Availability.start,
    End = Availability.end,
    assertz(timetable(StaffId, Date, (Start, End))),
    process_timetable(Rest).

process_agstaff_list([]).
process_agstaff_list([Staff | Rest]) :-
    StaffId = Staff.staffId,
    Date = Staff.date,
    ScheduleRaw = Staff.schedule,
    convert_slots(ScheduleRaw, Schedule),
    assertz(agenda_staff(StaffId, Date, Schedule)),
    process_agstaff_list(Rest).

process_surgery_list([]).
process_surgery_list([Surgery | Rest]) :-
    SurgeryId = Surgery.surgeryId,
    PrepTime = Surgery.prepTime,
    SurgeryDuration = Surgery.surgeryDuration,
    RecoveryTime = Surgery.recoveryTime,
    assertz(surgery(SurgeryId, PrepTime, SurgeryDuration, RecoveryTime)),
    process_surgery_list(Rest).

process_rooms([]).
process_rooms([Room | Rest]) :-
    RoomCode = Room.roomId,
    atom_string(DayString, Room.date),
    atom_number(DayString,Date),
    OccupiedSlotsRaw = Room.occupied,
    convert_slots(OccupiedSlotsRaw, OccupiedSlots),
    assertz(agenda_operation_room(RoomCode, Date, OccupiedSlots)),
    process_rooms(Rest).

convert_slots([], []). 
convert_slots([Slot | Rest], [(Start, End, OperationId) | RestTuples]) :-
    atom_string(StartString, Slot.start),
    atom_number(StartString,Start),
    atom_string(EndString, Slot.end),
    atom_number(EndString,End),
    atom_string(OperationId, Slot.operationId),
    convert_slots(Rest, RestTuples).

process_staff_list([]).
process_staff_list([Staff | Rest]) :-
    StaffId = Staff.staffId,
    Function = Staff.function,
    Spec = Staff.spec,
    AssocOp = Staff.assocOp,
    assertz(staff(StaffId, Function, Spec, AssocOp)),
    process_staff_list(Rest).    


%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
%%%%%%%%%%%%%%%% Better Solution %%%%%%%%%%%%%%%%
%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
obtain_better_sol(Room,Day,AgOpRoomBetter,LAgDoctorsBetter,TFinOp):-
    get_time(Ti),
    (obtain_better_sol1(Room,Day);true),
    better_sol(Day,Room,AgOpRoomBetter,LAgDoctorsBetter,TFinOp),
    get_time(Tf),
    _ is Tf-Ti.

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
    fail. 

update_better_sol(Day,Room,Agenda,LOpCode):-
    better_sol(Day,Room,_,_,FinTime),
    reverse(Agenda,AgendaR),
    evaluate_final_time(AgendaR,LOpCode,FinTime1),
    FinTime1<FinTime,
    retract(better_sol(_,_,_,_,_)),
    findall(Doctor,assignment_surgery(_,Doctor),LDoctors1),
    remove_equals(LDoctors1,LDoctors),
    list_doctors_agenda(Day,LDoctors,LDAgendas),
    asserta(better_sol(Day,Room,Agenda,LDAgendas,FinTime1)).

evaluate_final_time([],_,1441).
evaluate_final_time([(_,Tfin,OpCode)|_],LOpCode,Tfin):-
    member(OpCode,LOpCode),!.
evaluate_final_time([_|AgR],LOpCode,Tfin):-
    evaluate_final_time(AgR,LOpCode,Tfin).

list_doctors_agenda(_,[],[]).
list_doctors_agenda(Day,[D|LD],[(D,AgD)|LAgD]):-agenda_staff1(D,Day,AgD),list_doctors_agenda(Day,LD,LAgD).

remove_equals([],[]).
remove_equals([X|L],L1):-member(X,L),!,remove_equals(L,L1).
remove_equals([X|L],[X|L1]):-remove_equals(L,L1).


%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
%%%%%%%%%%%%%%%% Heuristic 1 %%%%%%%%%%%%%%%%
%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
obtain_heuristic_solution(Room,Day,AgOpRoomBetter,LAgDoctorsBetter,TFinOp):-
    get_time(Ti),
    (obtain_heuristic_solution1(Room,Day),!),
    retract(better_sol(Day,Room,AgOpRoomBetter,LAgDoctorsBetter,TFinOp)),
    get_time(Tf),
    _ is Tf-Ti.

obtain_heuristic_solution1(Room,Day):-
    findall(OpCode,surgery_id(OpCode,_),LOC),!, 
    retractall(agenda_staff1(_,_,_)),
    retractall(agenda_operation_room1(_,_,_)),
    retractall(availability(_,_,_)),
    retractall(final_time_heuristics(_)),
    findall(_,(agenda_staff(D,Day,Agenda),assertz(agenda_staff1(D,Day,Agenda))),_),
    agenda_operation_room(Room,Day,Agenda),assert(agenda_operation_room1(Room,Day,Agenda)),
    findall(_,(agenda_staff1(D,Day,L),free_agenda0(L,LFA),adapt_timetable(D,Day,LFA,LFA2),assertz(availability(D,Day,LFA2))),_),
    debug(heuristic_available_handler, 'GOT HERE',[]),
    availability_early_surgeries(LOC, Room, Day),
    debug(heuristic_available_handler, 'PASSED',[]),
    retract(final_time_heuristics(TFinOp)),
    agenda_operation_room1(Room,Day,AgendaR),
    findall(Doctor,assignment_surgery(_,Doctor),LDoctors1),
    remove_equals(LDoctors1,LDoctors),
    list_doctors_agenda(Day,LDoctors,LAgDoctorsHeuristic),
    asserta(better_sol(Day,Room,AgendaR,LAgDoctorsHeuristic,TFinOp)).

availability_early_surgeries([], _, _) :-!.  
availability_early_surgeries(LOpCode, Room, Day):-
    retractall(earliest_surgery(_, _, _)),  
    asserta(earliest_surgery(_, 1441, _)),
    find_earliest_surgery(LOpCode, Room, Day), !,
    earliest_surgery(OpCode, TinS, LStaff),
    (
        TinS == 1441,
        select(OpCode, LOpCode, LRestOpCode),
        availability_early_surgeries(LRestOpCode, Room, Day)
    ;
        surgery_id(OpCode, OpType),
        surgery(OpType, TAnesthesia, TSurgery, TCleaning),
        TfinS is TinS + TSurgery + TAnesthesia + TCleaning - 1,
        retractall(final_time_heuristics(_)), 
        asserta(final_time_heuristics(TfinS)),

        retract(agenda_operation_room1(Room, Day, Agenda)),
        insert_agenda((TinS, TfinS, OpCode), Agenda, Agenda1),
        assertz(agenda_operation_room1(Room, Day, Agenda1)), 

        include(is_doctor, LStaff, LSurgeons),
        include(is_anaesthetist, LStaff, LAnesth),
        include(is_assistant, LStaff, LCleaners),
        
        insert_agenda_anesthesia_team((TinS,TAnesthesia,TSurgery,OpCode),Day,LAnesth),
        insert_agenda_surgery_team((TinS,TAnesthesia,TSurgery,OpCode),Day,LSurgeons),
        insert_agenda_cleaning_team((TinS,TAnesthesia,TSurgery,TCleaning,OpCode),Day,LCleaners),

        select(OpCode, LOpCode, LRestOpCode),
        availability_early_surgeries(LRestOpCode, Room, Day)
    ). 

find_earliest_surgery([], _, _).
find_earliest_surgery([OpCode|LOpCode], Room, Day) :- 
    availability_operation_changed2(OpCode,Room,Day,LPossibilities,LStaff),!,
    (LPossibilities == [],
        find_earliest_surgery(LOpCode, Room, Day)
    ;
        surgery_id(OpCode, OpType),
        surgery(OpType, TAnesthesia, TSurgery, TCleaning),
        schedule_first_interval(TAnesthesia,TSurgery,TCleaning,LPossibilities,(TinS,_)),
        earliest_surgery(_, EarliestSurgeryTime, _),
        ((TinS < EarliestSurgeryTime,
            retractall(earliest_surgery(_, _, _)),
            asserta(earliest_surgery(OpCode, TinS, LStaff)),!
        ); true),
        find_earliest_surgery(LOpCode, Room, Day)
    ).

%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
%%%%%%%%%%%%%%%% Heuristic 2 %%%%%%%%%%%%%%%%
%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
obtain_heuristic_highest_occupancy_solution(Room,Day,AgOpRoomBetter,LAgDoctorsBetter,TFinOp):-
    get_time(Ti),
    (obtain_heuristic_highest_occupancy_solution1(Room,Day),!),
    retract(better_sol(Day,Room,AgOpRoomBetter,LAgDoctorsBetter,TFinOp)),
    get_time(Tf),
    _ is Tf-Ti.

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

find_surgery_by_highest_occupancy([], _, _):-!.
find_surgery_by_highest_occupancy(LOpCodes, Room, Day) :- 
    obtain_assignment_surgeries(LOpCodes,LDoctorsInvolved),
    calculate_doctor_highest_occupancy_percentage(Day,LDoctorsInvolved,DocHighestPerc),
    obtain_assignment_surgeries(LOpCodesHighOccp,[DocHighestPerc]),
    find_first_opcode_in_list(LOpCodesHighOccp, LOpCodes, OpCode),

    availability_operation_changed2(OpCode, Room, Day, LPossibilities, LStaff),!,

    (LPossibilities == [],
        select(OpCode, LOpCodes, LRestOpCode),
        find_surgery_by_highest_occupancy(LRestOpCode, Room, Day)
    ;
        surgery_id(OpCode, OpType),     
        surgery(OpType, TAnesthesia, TSurgery, TCleaning),
        schedule_first_interval(TAnesthesia,TSurgery,TCleaning,LPossibilities,(TinS,TfinS)),
        better_sol(Day,Room,_,_,FinTime),
        ((TfinS > FinTime,
            retractall(better_sol(_,_,_,_,_)),
            asserta(better_sol(Day,Room,_,_,TfinS))
        ); true),  
        retract(agenda_operation_room1(Room,Day,Agenda)),
        insert_agenda((TinS,TfinS,OpCode),Agenda,Agenda1),
        assertz(agenda_operation_room1(Room,Day,Agenda1)),
        include(is_doctor, LStaff, LSurgeons),
        include(is_anaesthetist, LStaff, LAnesth),
        include(is_assistant, LStaff, LCleaners),
        insert_agenda_anesthesia_team((TinS,TAnesthesia,TSurgery,OpCode),Day,LAnesth),
        insert_agenda_surgery_team((TinS,TAnesthesia,TSurgery,OpCode),Day,LSurgeons),
        insert_agenda_cleaning_team((TinS,TAnesthesia,TSurgery,TCleaning,OpCode),Day,LCleaners),
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

condition_to_check_opcode(OpCode, DocHighestPerc) :-
    member(OpCode, DocHighestPerc).

obtain_assignment_surgeries(OpCodes, DoctorsInvolved) :-
    (   is_list(OpCodes), 
        findall(Doctor, 
            (   member(OpCode, OpCodes),
                assignment_surgery(OpCode, Doctor),
                include(is_doctor, [Doctor], [FilteredDoctor]),
                FilteredDoctor \= []
            ), 
            DoctorsFiltered),
        list_to_set(DoctorsFiltered, DoctorsInvolved)
    ;   is_list(DoctorsInvolved),
        findall(OpCode, 
            (   member(Doctor, DoctorsInvolved),
                include(is_doctor, [Doctor], [FilteredDoctor]),
                FilteredDoctor \= [],
                assignment_surgery(OpCode, Doctor)
            ), 
            OpCodesList),
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
    sumlist(SurgeryDurations, TotalTime).

sum_free_time([], 0).
sum_free_time([(TimeInicial, TimeFinal)|Rest], TotalFreeTime) :-
    FreeTime is TimeFinal - TimeInicial,
    sum_free_time(Rest, RemainingFreeTime),
    TotalFreeTime is FreeTime + RemainingFreeTime.  


%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
%%%%%%%%%%%%%%%% Utils %%%%%%%%%%%%%%%%
%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
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

adapt_timetable(D,Date,LFA,LFA2):-timetable(D,Date,(InTime,FinTime)),treatin(InTime,LFA,LFA1),treatfin(FinTime,LFA1,LFA2).
treatin(InTime,[(In,Fin)|LFA],[(In,Fin)|LFA]):-InTime=<In,!.
treatin(InTime,[(_,Fin)|LFA],LFA1):-InTime>Fin,!,treatin(InTime,LFA,LFA1).
treatin(InTime,[(_,Fin)|LFA],[(InTime,Fin)|LFA]).
treatin(_,[],[]).
treatfin(FinTime,[(In,Fin)|LFA],[(In,Fin)|LFA1]):-FinTime>=Fin,!,treatfin(FinTime,LFA,LFA1).
treatfin(FinTime,[(In,_)|_],[]):-FinTime=<In,!.
treatfin(FinTime,[(In,_)|_],[(In,FinTime)]).
treatfin(_,[],[]).

find_free_agendas(Date):-
    retractall(availability(_,_,_)),
    findall(_,(agenda_staff(D,Date,L),
    free_agenda0(L,LFA),
    adapt_timetable(D,Date,LFA,LFA2),
    assertz(availability(D,Date,LFA2))),_).

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

availability_all_surgeries([],_,_).
availability_all_surgeries([OpCode|LOpCode],Room,Day):-
    surgery_id(OpCode,OpType),
    surgery(OpType,TAnesthesia,TSurgery,TCleaning),
    availability_operation_changed2(OpCode,Room,Day,LPossibilities,LStaff),!,
    schedule_first_interval(TAnesthesia,TSurgery,TCleaning,LPossibilities,(TinS,TfinS)),
    retract(agenda_operation_room1(Room,Day,Agenda)),
    insert_agenda((TinS,TfinS,OpCode),Agenda,Agenda1),
    assertz(agenda_operation_room1(Room,Day,Agenda1)),
    surgery_id(OpCode, OpType),
    surgery(OpType, TAnesthesia, TSurgery, TCleaning),
    include(is_doctor, LStaff, LSurgeons),
    include(is_anaesthetist, LStaff, LAnesth),
    include(is_assistant, LStaff, LCleaners),
    insert_agenda_anesthesia_team((TinS,TAnesthesia,TSurgery,OpCode),Day,LAnesth),
    insert_agenda_surgery_team((TinS,TAnesthesia,TSurgery,OpCode),Day,LSurgeons),
    insert_agenda_cleaning_team((TinS,TAnesthesia,TSurgery,TCleaning,OpCode),Day,LCleaners),
    availability_all_surgeries(LOpCode,Room,Day).

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

is_doctor(Staff) :-
    (staff(Staff, "doctor", _, _);
     staff(Staff, "nurse", _, _)),
     \+ staff(Staff, _, "anesthesia", _),
     \+ staff(Staff, "assistant", _, _). 
 is_anaesthetist(Staff) :-
     staff(Staff, _, "anesthesia", _).
 
 is_assistant(Staff) :-
     staff(Staff, "assistant", _, _).

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
    TfinS is Tin + TSurgery + TAnesthesia +TCleaning - 1.

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
