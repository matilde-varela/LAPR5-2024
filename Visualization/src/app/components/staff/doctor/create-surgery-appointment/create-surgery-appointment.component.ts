import { Component, ViewChild } from '@angular/core';
import { SideBarDoctorComponent } from '../sidebar-doctor/side-bar-doctor.component';
import { CommonModule } from '@angular/common';
import { FormsModule, NgForm } from '@angular/forms';
import { MessageComponent } from 'src/app/components/message/message.component';
import { SurgeryAppointmentService } from 'src/app/services/surgery-appointment.service';
import { Room } from 'src/app/domain/room';
import { RoomService } from 'src/app/services/room.service';
import { OperationRequestService } from 'src/app/services/operation-request.service';
import { OperationRequest } from 'src/app/domain/OperationRequest';
import { UserInfo } from 'src/app/domain/user-info';
import { Router } from '@angular/router';
import { StaffWithFunction } from 'src/app/domain/staff-with-function';
import { StaffService } from 'src/app/services/staff.service';
import { TableModule } from 'primeng/table';

@Component({
  selector: 'app-create-surgery-appointment',
  standalone: true,
  imports: [SideBarDoctorComponent, MessageComponent, FormsModule, CommonModule, TableModule],
  templateUrl: './create-surgery-appointment.component.html',
  styleUrl: './create-surgery-appointment.component.css'
})
export class CreateSurgeryAppointmentComponent {
  @ViewChild('surgeryAppointmentForm') surgeryAppointmentForm!: NgForm;

  isSubmitted = false;
  surgeryAppointmentCreated = false;
  surgeryAppointment = {
    operationRequestId: '',
    roomNumber: '',
    startTime: '',
    endTime: '',
    startDate: '',
    endDate: '',
    staffList: [] as string[]
  }

  rooms: Room[] = [];
  selectedRoom !: Room;
  operationRequestList: OperationRequest[] = [];
  selectedOperationRequest!: OperationRequest;
  staffs: StaffWithFunction[] = [];
  selectedStaff: StaffWithFunction[] = [];
  messageService: any;
  storedToken = localStorage.getItem('user');

  constructor(private service: SurgeryAppointmentService, private roomService: RoomService, private opRequestsService: OperationRequestService, private staffService: StaffService, private router: Router) { }

  ngOnInit(): void {
    if (this.storedToken) {

      var userInfo: UserInfo;

      userInfo = JSON.parse(this.storedToken);

      if (userInfo.roles.includes('Doctor')) {
        this.fetchRooms();
        this.fetchOperationRequests();
        this.fetchStaff();
      }
      else {
        this.router.navigate(['']);
      }
    }
    else {
      this.router.navigate(['login']);
    }
  }

  fetchOperationRequests() {
    if (this.storedToken) {
      this.opRequestsService.getAllOperationRequests().subscribe({
        next: (data) => {
          this.operationRequestList = data;
        },
        error: (error) => {
          console.error('Error fetching operation requests:', error);
        }
      });
    }
  }

  fetchRooms() {
    if (this.storedToken) {
      this.roomService.getAllRooms().subscribe({
        next: (data) => {
          this.rooms = data;
        },
        error: (error) => {
          console.error('Error fetching hospital rooms:', error);
        }
      });
    }
  }

  fetchStaff() {
    if (this.storedToken) {
      this.staffService.getAllActiveStaffProfiles().subscribe({
        next: (data) => {
          this.staffs = data;
        },
        error: (error) => {
          console.error('Error fetching staff profiles:', error);
        }
      });
    }
  }

  onSubmit(form: NgForm): void {
    if (form.valid) {
      this.isSubmitted = true;
      this.service.createSurgeryAppointment(
        this.surgeryAppointment.operationRequestId,
        this.surgeryAppointment.roomNumber,
        this.surgeryAppointment.startTime,
        this.surgeryAppointment.endTime,
        this.surgeryAppointment.startDate,
        this.surgeryAppointment.endDate,
        this.surgeryAppointment.staffList
      )
    }
  }

  clearForm(): void {
    this.isSubmitted = false;
    this.surgeryAppointmentCreated = false;

    this.surgeryAppointment = {
      operationRequestId: '',
      roomNumber: '',
      startTime: '',
      endTime: '',
      startDate: '',
      endDate: '',
      staffList: []
    };

    if (this.surgeryAppointmentForm) {
      this.surgeryAppointmentForm.resetForm();
    }

    const inputs = document.querySelectorAll('.input-field input');
    inputs.forEach(input => {
      input.classList.remove('invalid-placeholder');
    });
  }

}
