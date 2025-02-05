import { ComponentFixture, TestBed } from '@angular/core/testing';
import { OperationRequestScheduler } from './operation-request-scheduler.component';
import { SideBarAdminComponent } from '../sidebar-admin/side-bar-admin.component';
import { CommonModule } from '@angular/common';
import { TableModule } from 'primeng/table';
import { FormsModule } from '@angular/forms';
import { MessageComponent } from '../../message/message.component';
import { StaffService } from 'src/app/services/staff.service';
import { RoomService } from 'src/app/services/room.service';
import { OperationRequestService } from 'src/app/services/operation-request.service';
import { Router } from '@angular/router';
import { of } from 'rxjs';
import { RouterTestingModule } from '@angular/router/testing';


describe('OperationRequestSchedulerComponent', () => {
  let component: OperationRequestScheduler;
  let fixture: ComponentFixture<OperationRequestScheduler>;

  beforeEach(async () => {
    const MockStaffService = jasmine.createSpyObj('StaffService', ['getAllActiveStaffProfiles']);
    MockStaffService.getAllActiveStaffProfiles.and.returnValue(of([]));
    const MockRoomService = jasmine.createSpyObj('RoomService', ['getAllRooms']);
    MockRoomService.getAllRooms.and.returnValue(of([]));
    const MockOperationRequestService = jasmine.createSpyObj('OperationRequestService', ['getAllOperationRequests', 'scheduleOperationRequest']);
    MockOperationRequestService.getAllOperationRequests.and.returnValue(of([]));


    await TestBed.configureTestingModule({
      imports: [
        CommonModule,
        TableModule,
        FormsModule,
        RouterTestingModule, // Add this for routing
        OperationRequestScheduler, // Declare component here
        SideBarAdminComponent,
        MessageComponent
      ],
      providers: [
        { provide: StaffService, useValue: MockStaffService },
        { provide: RoomService, useValue: MockRoomService },
        { provide: OperationRequestService, useValue: MockOperationRequestService },
      ]
    })
      .compileComponents();

    fixture = TestBed.createComponent(OperationRequestScheduler);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
