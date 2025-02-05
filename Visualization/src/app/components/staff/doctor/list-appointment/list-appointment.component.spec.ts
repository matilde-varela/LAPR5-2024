import { TestBed, ComponentFixture } from '@angular/core/testing';
import { of } from 'rxjs';
import { ListAppointmentComponent } from './list-appointment.component';
import { SurgeryAppointmentService } from 'src/app/services/surgery-appointment.service';
import { FullAppointment } from 'src/app/domain/full-appointment';
import { StaffWithFunction } from 'src/app/domain/staff-with-function';
import { UpdateAppointment } from 'src/app/domain/update-appointment';
import { HttpClientTestingModule } from '@angular/common/http/testing';
import { SideBarDoctorComponent } from '../sidebar-doctor/side-bar-doctor.component';
import { CommonModule } from '@angular/common';
import { TableModule } from 'primeng/table';
import { FormsModule } from '@angular/forms';
import { MessageComponent } from 'src/app/components/message/message.component';
import { ActivatedRoute } from '@angular/router';

describe('ListAppointmentComponent', () => {
  let component: ListAppointmentComponent;
  let fixture: ComponentFixture<ListAppointmentComponent>;
  let mockService: jasmine.SpyObj<SurgeryAppointmentService>;

  const mockAppointments: FullAppointment[] = [
    {
      id: '1',
      status: 'Scheduled',
      operationRequestId: '101',
      roomNumber: 'A1',
      startTime: '10:00',
      endTime: '11:00',
      startDate: '2024-12-30',
      endDate: '2024-12-30',
      staffs: ['staff1', 'staff2']
    }
  ];

  const mockStaffs: StaffWithFunction[] = [
    {
      id: 'staff1',
      name: 'Dr. Smith',
      phone: '1234567890',
      email: 'drsmith@example.com',
      address: '123 Medical Lane',
      function: 'Surgeon',
      specializationId: 'spec1',
      slots: [],
      status: 'Active'
    },
    {
      id: 'staff2',
      name: 'Dr. Brown',
      phone: '9876543210',
      email: 'drbrown@example.com',
      address: '456 Surgical Street',
      function: 'Anesthetist',
      specializationId: 'spec2',
      slots: [],
      status: 'Active'
    }
  ];

  beforeEach(async () => {
    const serviceSpy = jasmine.createSpyObj('SurgeryAppointmentService', [
      'getAllAppointments',
      'getAllStaffs',
      'getRoomNumbers',
      'editAppointment'
    ]);

    const activatedRouteMock = {
      snapshot: {
        paramMap: {
          get: jasmine.createSpy('get').and.returnValue(null),
        },
      },
    };

    await TestBed.configureTestingModule({
      imports: [SideBarDoctorComponent, CommonModule, TableModule, FormsModule, MessageComponent],
      providers: [
        { provide: SurgeryAppointmentService, useValue: serviceSpy },
        { provide: ActivatedRoute, useValue: activatedRouteMock }
      ]
    }).compileComponents();

    mockService = TestBed.inject(SurgeryAppointmentService) as jasmine.SpyObj<SurgeryAppointmentService>;
    fixture = TestBed.createComponent(ListAppointmentComponent);
    component = fixture.componentInstance;
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should fetch appointments and staff on init', () => {
    mockService.getAllAppointments.and.returnValue(of(mockAppointments));
    mockService.getAllStaffs.and.returnValue(of(mockStaffs));

    component.ngOnInit();

    expect(mockService.getAllAppointments).toHaveBeenCalled();
    expect(mockService.getAllStaffs).toHaveBeenCalled();
    expect(component.appointments).toEqual(mockAppointments);
    expect(component.staffListToShow).toEqual(mockStaffs);
  });

  it('should apply filters by refetching operations', () => {
    spyOn(component, 'fetchOperations');

    component.applyFilters();

    expect(component.fetchOperations).toHaveBeenCalled();
  });

  it('should close the update form', () => {
    component.updateVisible = true;

    component.closeUpdate();

    expect(component.updateVisible).toBeFalse();
  });

  it('should fetch room numbers', () => {
    const mockRoomNumbers = ['A1', 'B2'];
    mockService.getRoomNumbers.and.returnValue(of(mockRoomNumbers));

    component.editAppointment(mockAppointments[0]);

    expect(mockService.getRoomNumbers).toHaveBeenCalled();
    expect(component.updateVisible).toBeTrue();
    expect(component.roomNumbers).toEqual(mockRoomNumbers);
  });

  it('should save update details and call service to edit appointment', () => {
    component.selectedStaff = mockStaffs;
    component.selectedAppointment = mockAppointments[0];
    component.updateAppointment = {
      appointmentId: '',
      newRoomNumber: '',
      newStartTime: '',
      newEndTime: '',
      newStartDate: '',
      newEndDate: '',
      newStaffList: []
    };

    component.saveUpdateDetails();

    expect(component.updateAppointment.newStaffList).toEqual(['staff1', 'staff2']);
    expect(component.updateAppointment.appointmentId).toEqual('1');
    expect(mockService.editAppointment).toHaveBeenCalledWith(component.updateAppointment);
  });
});
