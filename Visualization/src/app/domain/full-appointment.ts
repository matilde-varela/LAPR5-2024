export interface FullAppointment {
    id: string,
    status: string,
    operationRequestId: string,
    roomNumber: string,
    startTime: string,
    endTime: string,
    startDate: string,
    endDate: string,
    staffs: string[]
}