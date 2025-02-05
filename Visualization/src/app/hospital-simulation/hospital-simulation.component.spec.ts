import { ComponentFixture, TestBed } from '@angular/core/testing';

import { HospitalSimulationComponent } from './hospital-simulation.component';
import { RoomService } from '../services/room.service';
import { of } from 'rxjs';
import { ActivatedRoute } from '@angular/router';
import { SurgeryAppointmentService } from '../services/surgery-appointment.service';
import { OperationRequestService } from '../services/operation-request.service';
import { PatientService } from '../services/patient.service';

describe('HospitalSimulationComponent', () => {
  let component: HospitalSimulationComponent;
  let fixture: ComponentFixture<HospitalSimulationComponent>;
  const MockRoomService = jasmine.createSpyObj('RoomService', ['getRoomsSchedule']);
  MockRoomService.getRoomsSchedule.and.returnValue(of([]));
  const MockAppointmentService = jasmine.createSpyObj('AppointmentService', ['getAppointmentFromRoom']);
  MockAppointmentService.getAppointmentFromRoom.and.returnValue(of([]));
  const MockOperationRequestService = jasmine.createSpyObj('OperationRequestService', ['getOperationRequestById']);
  MockOperationRequestService.getOperationRequestById.and.returnValue(of([]));
  const MockPatientService = jasmine.createSpyObj('PatientService', ['getPatientById']);
  MockPatientService.getPatientById.and.returnValue(of([]));

  beforeEach(async () => {
    const activatedRouteMock = {
      snapshot: {
        paramMap: {
          get: jasmine.createSpy('get').and.returnValue(null),
        },
      },
    };

    await TestBed.configureTestingModule({
      imports: [HospitalSimulationComponent],
      providers: [
        { provide: RoomService, useValue: MockRoomService },
        { provide: SurgeryAppointmentService, useValue: MockAppointmentService },
        { provide: OperationRequestService, useValue: MockOperationRequestService },
        { provide: PatientService, useValue: MockPatientService },
        { provide: ActivatedRoute, useValue: activatedRouteMock }
      ]
    })
      .compileComponents();

    fixture = TestBed.createComponent(HospitalSimulationComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
