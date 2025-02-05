import { Component } from '@angular/core';
import { FullAppointment } from 'src/app/domain/full-appointment';
import { StaffWithFunction } from 'src/app/domain/staff-with-function';
import { SurgeryAppointment } from 'src/app/domain/SurgeryAppointment';
import { UpdateAppointment } from 'src/app/domain/update-appointment';
import { SurgeryAppointmentService } from 'src/app/services/surgery-appointment.service';
import { MessageComponent } from "../../../message/message.component";
import { SideBarDoctorComponent } from "../sidebar-doctor/side-bar-doctor.component";
import { CommonModule } from '@angular/common';
import { TableModule } from 'primeng/table';
import { FormsModule } from '@angular/forms';
import { data } from 'jquery';

@Component({
  standalone: true,
  selector: 'app-list-appointment',
  templateUrl: './list-appointment.component.html',
  styleUrl: './list-appointment.component.css',
  imports: [SideBarDoctorComponent, CommonModule, TableModule, FormsModule, MessageComponent],
})
export class ListAppointmentComponent {

  staffListToShow: StaffWithFunction[] = [];
  selectedStaff: StaffWithFunction[] = [];
  fullAppointment: FullAppointment= {
    id: '',
    status: '',
    operationRequestId: '',
    roomNumber: '',
    startTime: '',
    endTime: '',
    startDate: '',
    endDate: '',
    staffs: [] as string[]
  }

  selectedAppointment: FullAppointment= {
    id: '',
    status: '',
    operationRequestId: '',
    roomNumber: '',
    startTime: '',
    endTime: '',
    startDate: '',
    endDate: '',
    staffs: [] as string[]
  }

  isSubmitted = false;

  appointments: FullAppointment[] = [];

  roomNumbers: string[] = [];

  updateAppointment : UpdateAppointment = {
    appointmentId: '',
    newRoomNumber: '',
    newStartTime: '',
    newEndTime: '',
    newStartDate: '',
    newEndDate: '',
    newStaffList: []
  };

  updateVisible: boolean = false;

  
    constructor(private service: SurgeryAppointmentService) {}
  
    ngOnInit(): void {
      this.fetchOperations();
    }
  
    fetchOperations(): void {
      this.service.getAllAppointments().subscribe({
        next: (data: FullAppointment[]) => {
          console.log('Fetched Operations:', data);  // Log the fetched data to check its structure
          this.appointments = data;
        }
      });

      this.service.getAllStaffs().subscribe({
        next: (data: StaffWithFunction[]) => {
          console.log('Fetched Operations:', data);  // Log the fetched data to check its structure
          this.staffListToShow = data;
        }
      });
    }
  
    applyFilters(): void {
      this.fetchOperations();
    }
  
    closeUpdate(): void {
      this.updateVisible = false;
    }
  
    editAppointment(appointment: FullAppointment): void {
      this.updateVisible = true;
      this.service.getRoomNumbers().subscribe({
        next: (roomNumbersGot: string[]) => {
        this.roomNumbers = roomNumbersGot;
        },
        error: (error: any) => {
          console.error('Error fetching operation request details:', error);
        }
      });
    }
  
    saveUpdateDetails(){
      this.updateAppointment.newStaffList = this.selectedStaff.map(staff => staff.id);
      this.updateAppointment.appointmentId = this.selectedAppointment.id;
      this.updateAppointment.newEndDate = this.updateAppointment.newEndDate || '';
      this.updateAppointment.newStartDate = this.updateAppointment.newStartDate || '';
      this.updateAppointment.newEndTime = this.updateAppointment.newEndTime || '';
      this.updateAppointment.newStartTime = this.updateAppointment.newStartTime || '';
      this.updateAppointment.newRoomNumber = this.updateAppointment.newRoomNumber || '';
      this.updateAppointment.newStaffList = this.updateAppointment.newStaffList || [];
      console.log(this.updateAppointment);
      this.service.editAppointment(this.updateAppointment);
    }

}
