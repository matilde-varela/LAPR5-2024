export interface UpdateAppointment {
    appointmentId: string;
    newRoomNumber?: string;
    newStartTime?: string;
    newEndTime?: string;
    newStartDate?: string;
    newEndDate?: string;
    newStaffList?: string[];
}