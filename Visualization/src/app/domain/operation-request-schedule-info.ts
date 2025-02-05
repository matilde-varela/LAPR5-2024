import { StaffWithFunction } from "./staff-with-function";

export interface OperationRequestScheduleInfo {
    selectedStaff: StaffWithFunction[], 
    selectedRoomId: string, 
    operationRequestId: string, 
    algorithm: string,
    day: string
}
