import { ComponentFixture, TestBed } from '@angular/core/testing';
import { of } from 'rxjs';
import { CreateSurgeryAppointmentComponent } from './create-surgery-appointment.component';
import { FormsModule, NgForm } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router } from '@angular/router';
import { TableModule } from 'primeng/table';
import { SurgeryAppointmentService } from 'src/app/services/surgery-appointment.service';
import { RoomService } from 'src/app/services/room.service';
import { OperationRequestService } from 'src/app/services/operation-request.service';
import { StaffService } from 'src/app/services/staff.service';
import { Room } from 'src/app/domain/room';
import { OperationRequest } from 'src/app/domain/OperationRequest';
import { StaffWithFunction } from 'src/app/domain/staff-with-function';
import { MessageComponent } from 'src/app/components/message/message.component';
import { SideBarDoctorComponent } from '../sidebar-doctor/side-bar-doctor.component';

describe('CreateSurgeryAppointmentComponent', () => {
  let component: CreateSurgeryAppointmentComponent;
  let fixture: ComponentFixture<CreateSurgeryAppointmentComponent>;
  let surgeryAppointmentService: SurgeryAppointmentService;
  let roomService: RoomService;
  let operationRequestService: OperationRequestService;
  let staffService: StaffService;

  const mockRooms: Room[] = [
    {
      roomNumber: '101',
      type: 'Surgical',
      capacity: 2,
      availableEquipment: ['Scalpel', 'Monitor'],
      currentStatus: 'Available',
      maintenanceSlots: [],
    },
  ];

  const mockStaffProfiles: StaffWithFunction[] = [
    {
      id: '1',
      name: 'Dr. Smith',
      phone: '123456789',
      email: 'dr.smith@example.com',
      address: '123 Main St',
      function: 'Surgeon',
      specializationId: '456',
      slots: [],
      status: 'Active',
    },
  ];

  const mockOperationRequests: OperationRequest[] = [
    {
      id: '1',
      deadLineDate: '2023-12-01',
      priority: 'High',
      dateOfRequest: '2023-11-01',
      status: 'Pending',
      staffId: '123',
      description: 'Appendectomy',
      patientId: '456',
      operationTypeId: '789',
    },
  ];

  beforeEach(async () => {  
    const surgeryAppointmentServiceMock = jasmine.createSpyObj('SurgeryAppointmentService', ['createSurgeryAppointment']);


    const roomServiceMock = {
      getAllRooms: jasmine.createSpy('getAllRooms').and.returnValue(of(mockRooms))
    }
    
    const staffServiceMock = jasmine.createSpyObj('StaffService', ['getAllActiveStaffProfiles']);
    staffServiceMock.getAllActiveStaffProfiles.and.returnValue(of(mockStaffProfiles));
    
    const operationRequestServiceMock = {
      getAllOperationRequests: jasmine.createSpy('getAllOperationRequests').and.returnValue(of(mockOperationRequests))
    }


    const activatedRouteMock = {
      snapshot: {
        paramMap: {
          get: jasmine.createSpy('get').and.returnValue(null),
        },
      },
    };

    await TestBed.configureTestingModule({
      imports: [CreateSurgeryAppointmentComponent, SideBarDoctorComponent, MessageComponent, FormsModule, CommonModule, TableModule],
      providers: [
        { provide: SurgeryAppointmentService, useValue: surgeryAppointmentServiceMock },
        { provide: RoomService, useValue: roomServiceMock },
        { provide: OperationRequestService, useValue: operationRequestServiceMock },
        { provide: StaffService, useValue: staffServiceMock },
        { provide: ActivatedRoute, useValue: activatedRouteMock }
      ],
    }).compileComponents();

    fixture = TestBed.createComponent(CreateSurgeryAppointmentComponent);
    component = fixture.componentInstance;
    roomService = TestBed.inject(RoomService);
    staffService = TestBed.inject(StaffService);
    operationRequestService = TestBed.inject(OperationRequestService);
    surgeryAppointmentService = TestBed.inject(SurgeryAppointmentService);
    fixture.detectChanges();
  });

  it('should create the component', () => {
    expect(component).toBeTruthy();
  });

  it('should fetch rooms on init', () => {
    const mockToken = JSON.stringify({
      roles: ['Doctor'],
    });
  
    component.storedToken = mockToken;
  
    component.ngOnInit();
    expect(roomService.getAllRooms).toHaveBeenCalled();
    expect(component.rooms).toEqual(mockRooms);
  });
  

  it('should fetch operation requests on init', () => {
    const mockToken = JSON.stringify({
      roles: ['Doctor'],
    });
  
    component.storedToken = mockToken;
    
    component.ngOnInit();
    expect(operationRequestService.getAllOperationRequests).toHaveBeenCalled();
    expect(component.operationRequestList).toEqual(mockOperationRequests);
  });

  it('should fetch staff profiles on init', () => {
    const mockToken = JSON.stringify({
      roles: ['Doctor'],
    });
  
    component.storedToken = mockToken;
    
    component.ngOnInit();
    expect(staffService.getAllActiveStaffProfiles).toHaveBeenCalled();
    expect(component.staffs).toEqual(mockStaffProfiles);
  });


  it('should submit the form when valid', () => {
    component.surgeryAppointment.operationRequestId = '1';
    component.surgeryAppointment.roomNumber = '101';
    component.surgeryAppointment.startTime = '09:00';
    component.surgeryAppointment.endTime = '10:00';
    component.surgeryAppointment.startDate = '2023-12-01';
    component.surgeryAppointment.endDate = '2023-12-01';
    component.surgeryAppointment.staffList = ['1'];

    const mockForm = {
      valid: true,
    } as NgForm;

    component.onSubmit(mockForm);

    expect(component.isSubmitted).toBe(true);
    expect(surgeryAppointmentService.createSurgeryAppointment).toHaveBeenCalledWith(component.surgeryAppointment.operationRequestId,
      component.surgeryAppointment.roomNumber, component.surgeryAppointment.startTime, component.surgeryAppointment.endTime,
      component.surgeryAppointment.startDate, component.surgeryAppointment.endDate, component.surgeryAppointment.staffList);
  });

  
  it('should not submit the form when invalid', () => {
    const formMock = {
      valid: false,
    } as NgForm;

    component.onSubmit(formMock);

    expect(component.isSubmitted).toBe(false);
    expect(surgeryAppointmentService.createSurgeryAppointment).not.toHaveBeenCalled();
  });


  it('should clear the form on clearForm', () => {
    component.surgeryAppointment.operationRequestId = '1';
    component.surgeryAppointment.roomNumber = '101';
    component.surgeryAppointment.startTime = '09:00';
    component.surgeryAppointment.endTime = '10:00';
    component.surgeryAppointment.startDate = '2023-12-01';
    component.surgeryAppointment.endDate = '2023-12-01';
    component.surgeryAppointment.staffList = ['1'];
    component.isSubmitted = true;
  
    component.clearForm();
  
    expect(component.surgeryAppointment.operationRequestId).toBe('');
    expect(component.surgeryAppointment.roomNumber).toBe('');
    expect(component.surgeryAppointment.startTime).toBeNull();
    expect(component.surgeryAppointment.endTime).toBeNull();
    expect(component.surgeryAppointment.startDate).toBeNull();
    expect(component.surgeryAppointment.endDate).toBeNull();
    expect(component.surgeryAppointment.staffList.length).toBe(0);
  });

});
