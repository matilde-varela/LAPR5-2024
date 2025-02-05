import { ScheduleSlot } from "./shedule-slot";

export interface RoomSchedule {
    roomNumber : string;
    schedule : ScheduleSlot[];
}