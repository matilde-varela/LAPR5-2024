% DATA
:-dynamic agenda_operation_room1/3.
:-dynamic room_in_scheduling/1.
:-dynamic occupied_time/2. % occupied_time(roomId, time).
:-dynamic operation_assigment/2. % operation_assigment(opId, roomId).

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

agenda_operation_room(r101,20241028,[(770, 950, so1001), (1080, 1110, cnt1002)]). %180 + 30
agenda_operation_room(r102,20241028,[(720, 780, mnt0001), (1080, 1110, cnt1001)]). %60 + 30
agenda_operation_room(r301,20241028,[(720, 780, mnt0002), (1080, 1110, cnt1003)]). %60 + 30
agenda_operation_room(r404,20241028,[]).


% ------------------------------------------------------------------------
distribute_rooms(Day) :-
    retractall(occupied_time(_, _)),
    retractall(operation_assigment(_, _)),
    findall(OpCode, surgery_id(OpCode, _), OperationsList),
    findall(Room, agenda_operation_room(Room, Day, _), RoomsList),    % Find all rooms with agendas
    assert_occupied_times(RoomsList),     % Calculate and assert occupied time for each room
    distribute_operations_round_robin1(OperationsList, RoomsList),!,
    print_operation_assignments.

print_operation_assignments :-
    findall(operation_assigment(OpId, RoomId), operation_assigment(OpId, RoomId), Assignments),
    print_assignments_to_console(Assignments).
print_assignments_to_console([]).
print_assignments_to_console([operation_assigment(OpId, RoomId) | Rest]) :-
    format('operation_assigment(~w, ~w).~n', [OpId, RoomId]),
    print_assignments_to_console(Rest).

distribute_operations_round_robin1([], _).
distribute_operations_round_robin1([Operation | RemainingOps], RoomsList) :-
    (    attempt_assign_to_rooms(Operation, RoomsList),  distribute_operations_round_robin1(RemainingOps, RoomsList)  % Move to the next operation
    ;   write('Failed to assign operation: '), write(Operation), nl  % Handle unassignable operation
    ).

attempt_assign_to_rooms(_, [], false):- fail.
attempt_assign_to_rooms(Operation, [Room | RemainingRooms]) :-
    check_current_room_occupancy_ratio(Room, Ratio),
    (   Ratio < 0.8,
        schedule_operation_in_room(Operation, Room)
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

schedule_operation_in_room(Operation, Room) :-
    surgery_id(Operation, OpType),
    surgery(OpType, PrepTime, Duration, CleanupTime),
    OpTotalTime is PrepTime + Duration + CleanupTime,
    retract(occupied_time(Room, OldOccupTime)),
    NewOccupTime is OldOccupTime + OpTotalTime,
    assert(occupied_time(Room, NewOccupTime)),
    retractall(operation_assigment(Operation, _)),
    assert(operation_assigment(Operation, Room)).

