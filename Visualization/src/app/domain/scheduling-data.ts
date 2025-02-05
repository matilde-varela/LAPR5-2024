import { StaffWithId } from "./staff-with-id";

export interface SchedulingData {
    staff : StaffWithId[];
    operationRequestID : string;
}