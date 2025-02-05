import { ComponentFixture, TestBed } from '@angular/core/testing';
import { ListOperationRequestComponent } from './list-operation-request.component';
import { SideBarDoctorComponent } from '../sidebar-doctor/side-bar-doctor.component';
import { CommonModule } from '@angular/common';
import { TableModule } from 'primeng/table';
import { FormsModule } from '@angular/forms';
import { MessageComponent } from 'src/app/components/message/message.component';
import { OperationRequestService } from 'src/app/services/operation-request.service';
import { ActivatedRoute } from '@angular/router';
import { of } from 'rxjs';
import { ListOperationRequest } from 'src/app/domain/list-operation-request';
import { OperationRequest } from 'src/app/domain/OperationRequest';

describe('ListOperationRequestComponent', () => {
  let component: ListOperationRequestComponent;
  let fixture: ComponentFixture<ListOperationRequestComponent>;
  let operationRequestService: OperationRequestService;;

  const listOpRequest: ListOperationRequest[] = [
    {
      id: "test-id",
      patientName: "test-patient",
      operationType: "test-op-type",
      status: "test-status"
    },
    {
      id: "test-id1",
      patientName: "test-patient1",
      operationType: "test-op-type1",
      status: "test-status1"
    }
  ]

  const fullOpRequest: OperationRequest = {
    id: "test-id",
    deadLineDate: "2020-10-24",
    priority: "test-priority",
    dateOfRequest: "2020-10-24",
    status: "test-status",
    staffId: "test-staff-id",
    description: "test-descrip",
    patientId: "test-patient",
    operationTypeId: "test-op-type",
  }



  beforeEach(async () => {
    const operationRequestServiceMock = {
      getOperationRequestsByFilters: jasmine.createSpy('getOperationRequestsByFilters').and.returnValue(of(listOpRequest)),
      getOperationRequestById: jasmine.createSpy('getOperationRequestById').and.returnValue(of(fullOpRequest)),
      updateOperationRequest: jasmine.createSpy('updateOperationRequest'),
      deleteOperationRequestById: jasmine.createSpy('deleteOperationRequestById')
    }

    const activatedRouteMock = {
      snapshot: {
        paramMap: {
          get: jasmine.createSpy('get').and.returnValue(null),
        },
      },
    };
    await TestBed.configureTestingModule({
      imports: [ListOperationRequestComponent, SideBarDoctorComponent, CommonModule, TableModule, FormsModule, MessageComponent],
      providers: [
        { provide: OperationRequestService, useValue: operationRequestServiceMock },
        { provide: ActivatedRoute, useValue: activatedRouteMock }
      ]
    }).compileComponents();

    fixture = TestBed.createComponent(ListOperationRequestComponent);
    component = fixture.componentInstance;
    operationRequestService = TestBed.inject(OperationRequestService);
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should fetch operation requests on initialization', () => {
    expect(operationRequestService.getOperationRequestsByFilters).toHaveBeenCalled();
    expect(component.operationRequests.length).toBe(2);
    expect(component.operationRequests[0].patientName).toBe('test-patient');
  });

  it('should apply filters and fetch filtered results', () => {
    component.filters.name = 'test-id';
    component.applyFilters();
    expect(operationRequestService.getOperationRequestsByFilters).toHaveBeenCalledWith(
      'test-id', '', '', '', '', ''
    );
  });

  it('should toggle details visibility when "toggleDetails" is called', () => {
    component.toggleDetails(listOpRequest[0]);
    expect(operationRequestService.getOperationRequestById).toHaveBeenCalledWith('test-id');
    expect(component.detailsVisible).toBeTrue();
    expect(component.fullOperationRequest.id).toBe('test-id');
  });

  it('should hide details when "closeDetails" is called', () => {
    component.closeDetails();
    expect(component.detailsVisible).toBeFalse();
  });

  it('should show update form when "editOperationRequest" is called', () => {
    component.editOperationRequest(listOpRequest[0]);
    expect(operationRequestService.getOperationRequestById).toHaveBeenCalledWith('test-id');
    expect(component.updateVisible).toBeTrue();
    expect(component.fullOperationRequest.id).toBe('test-id');
  });

  it('should call service to save update details', () => {
    component.fullOperationRequest = fullOpRequest;
    component.update = { id: 'test-id', priority: 'test-priority', description: 'test-descrip', deadlineDate: '2020-10-24' };
    component.saveUpdateDetails();
    expect(operationRequestService.updateOperationRequest).toHaveBeenCalledWith({ id: 'test-id', priority: 'test-priority', description: 'test-descrip', deadlineDate: '2020-10-24' });
  });

  it('should call service to delete an operation request and refresh the list', () => {
    component.deleteOperationRequest(listOpRequest[1]);
    expect(operationRequestService.deleteOperationRequestById).toHaveBeenCalledWith('test-id1');
    expect(operationRequestService.getOperationRequestsByFilters).toHaveBeenCalled();
  });

  it('should call service to delete an operation request', () => {
    const opReqTest: ListOperationRequest = {
      id: 'id',
      patientName: 'name',
      operationType: 'operationType',
      status: 'status'
    };

    component.deleteOperationRequest(opReqTest);
    expect(operationRequestService.deleteOperationRequestById).toHaveBeenCalledWith('id');
  });

});
