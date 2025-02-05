import { ComponentFixture, TestBed } from '@angular/core/testing';
import { of, throwError } from 'rxjs';  // Import RxJS utilities to mock observables
import { CreateOperationRequestComponent } from './create-operation-request.component';
import { OperationRequestService } from '../../../../services/operation-request.service';
import { FormsModule, NgForm } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { ActivatedRoute } from '@angular/router';

// Mock Service
const operationRequestServiceMock = {
  createOperationRequest: jasmine.createSpy('createOperationRequest'),
  getAllOperationTypes: jasmine.createSpy('getAllOperationTypes').and.returnValue(of(['OperationType1', 'OperationType2'])),
  addOperationRequestToPatient: jasmine.createSpy('addOperationRequestToPatient')
};

describe('CreateOperationRequestComponent', () => {
  let component: CreateOperationRequestComponent;
  let fixture: ComponentFixture<CreateOperationRequestComponent>;
  let service: OperationRequestService;

  beforeEach(async () => {

    const activatedRouteMock = {
      snapshot: {
        paramMap: {
          get: jasmine.createSpy('get').and.returnValue(null),
        },
      },
    }

    await TestBed.configureTestingModule({
      imports: [CreateOperationRequestComponent, FormsModule, CommonModule],
      providers: [
        { provide: OperationRequestService, useValue: operationRequestServiceMock },
        { provide: ActivatedRoute, useValue: activatedRouteMock }
      ]
    }).compileComponents();

    fixture = TestBed.createComponent(CreateOperationRequestComponent);
    component = fixture.componentInstance;
    service = TestBed.inject(OperationRequestService);
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should load operation types on init', () => {
    // Act
    component.ngOnInit();

    // Assert
    expect(service.getAllOperationTypes).toHaveBeenCalled();
    expect(component.operationTypes).toEqual(['OperationType1', 'OperationType2']);
  });

  it('should call createOperationRequest on valid form submission', () => {
    // Arrange
    component.operationRequest = {
      deadLineDate: '2024-12-31',
      priority: 'High',
      dateOfRequest: '2024-11-23',
      status: 'Pending',
      staffId: 'staff123',
      description: 'Test operation request',
      patientId: 'patient123',
      operationTypeId: 'operationType1'
    };

    const validForm = { valid: true } as any;  // Mocking valid form

    operationRequestServiceMock.createOperationRequest.and.returnValue(of('12345'));  // Mock response

    // Act
    component.onSubmit(validForm);

    // Assert
    expect(service.createOperationRequest).toHaveBeenCalledWith(
      '2024-12-31', 'High', '2024-11-23', 'Pending', 'staff123', 'Test operation request', 'patient123', 'operationType1'
    );
    expect(component.operationRequestCreated).toBeTrue();
    expect(component.opReqId).toBe('12345');
  });

  it('should handle createOperationRequest error on form submission', () => {
    // Arrange
    component.operationRequest = {
      deadLineDate: '2024-12-31',
      priority: 'High',
      dateOfRequest: '2024-11-23',
      status: 'Pending',
      staffId: 'staff123',
      description: 'Test operation request',
      patientId: 'patient123',
      operationTypeId: 'operationType1'
    };

    const validForm = { valid: true } as any;  // Mocking valid form

    operationRequestServiceMock.createOperationRequest.and.returnValue(throwError('Error'));  // Mock error

    // Act
    component.onSubmit(validForm);

    // Assert
    expect(service.createOperationRequest).toHaveBeenCalled();
    expect(component.isSubmitted).toBeFalse();  // Submission should reset
  });
  

  it('should toggle the Add to Patient form visibility', () => {
    // Arrange
    component.showAddToPatientForm = false;

    // Act
    component.toggleAddToPatientForm();

    // Assert
    expect(component.showAddToPatientForm).toBeTrue();

    // Act again
    component.toggleAddToPatientForm();

    // Assert again
    expect(component.showAddToPatientForm).toBeFalse();
  });

  it('should clear the Add to Patient form', () => {
    // Arrange
    component.isSubmittedToPatient = true;
    component.addRemoveFromPatient = {
      patientId: 'patient123',
      operationRequestId: '12345'
    };

    // Act
    component.clearAddToPatientForm();

    // Assert
    expect(component.isSubmittedToPatient).toBeFalse();
    expect(component.addRemoveFromPatient).toEqual({ patientId: '', operationRequestId: '' });
    expect(component.showAddToPatientForm).toBeFalse();
  });

  it('should clear the main form when clearForm is called', () => {
    // Arrange
    component.operationRequest.deadLineDate = '2024-12-31';
    component.operationRequest.priority ='High';
    component.operationRequest.dateOfRequest = '2024-11-23';
    component.operationRequest.status = 'Pending';
    component.operationRequest.staffId = 'staff123';
    component.operationRequest.description = 'Test operation request';
    component.operationRequest.patientId = 'patient123';
    component.operationRequest.operationTypeId = 'operationType1';

    // Act
    component.clearForm();

    // Assert
    expect(component.operationRequest.deadLineDate).toBeFalsy();
    expect(component.operationRequest.priority).toBeFalsy();
    expect(component.operationRequest.dateOfRequest).toBeFalsy();
    expect(component.operationRequest.status).toBeFalsy();
    expect(component.operationRequest.staffId).toBeFalsy();
    expect(component.operationRequest.description).toBeFalsy();
    expect(component.operationRequest.patientId).toBeFalsy();
    expect(component.operationRequest.operationTypeId).toBeFalsy();
    expect(component.isSubmitted).toBeFalse();
    expect(component.operationRequestCreated).toBeFalse();
  });
});
