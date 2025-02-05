import { ComponentFixture, TestBed } from '@angular/core/testing';
import { RemoveOperationRequestPatientComponent } from './remove-operation-request-patient.component';
import { OperationRequestService } from '../../../../services/operation-request.service';
import { of } from 'rxjs';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { SideBarDoctorComponent } from '../sidebar-doctor/side-bar-doctor.component';
import { MessageComponent } from '../../../message/message.component';
import { ActivatedRoute } from '@angular/router';

describe('RemoveOperationRequestPatientComponent', () => {
  let component: RemoveOperationRequestPatientComponent;
  let fixture: ComponentFixture<RemoveOperationRequestPatientComponent>;
  let service: jasmine.SpyObj<OperationRequestService>;

  beforeEach(async () => {
    // Create a spy for OperationRequestService
    service = jasmine.createSpyObj('OperationRequestService', ['removeOperationRequestToPatient']);

    const activatedRouteMock = {
      snapshot: {
        paramMap: {
          get: jasmine.createSpy('get').and.returnValue(null),
        },
      },
    }

    await TestBed.configureTestingModule({
      imports: [RemoveOperationRequestPatientComponent, FormsModule, CommonModule, SideBarDoctorComponent, MessageComponent],
      providers: [{ provide: OperationRequestService, useValue: service },
        { provide: ActivatedRoute, useValue: activatedRouteMock }]
    }).compileComponents();

    fixture = TestBed.createComponent(RemoveOperationRequestPatientComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should not call removeOperationRequestToPatient on invalid form submission', () => {
    // Arrange
    const invalidForm = { valid: false } as any;  // Mock an invalid form

    // Act
    component.onSubmit(invalidForm);

    // Assert
    expect(service.removeOperationRequestToPatient).not.toHaveBeenCalled();
    expect(component.isSubmitted).toBeFalse();  // Ensure isSubmitted is false when form is invalid
  });

  it('should call removeOperationRequestToPatient on valid form submission', () => {
    // Arrange
    const validForm = { valid: true } as any;  // Mock a valid form
    component.addRemoveFromPatient.patientId = 'patient123';
    component.addRemoveFromPatient.operationRequestId = 'req456';

    // Act
    component.onSubmit(validForm);

    // Assert
    expect(service.removeOperationRequestToPatient).toHaveBeenCalledWith('patient123', 'req456');
    expect(component.isSubmitted).toBeTrue();  // Ensure isSubmitted is true when form is valid
  });

  it('should clear the form when clearForm is called', () => {
    // Arrange
    component.addRemoveFromPatient = {
      patientId: 'patient123',
      operationRequestId: 'req456'
    };
    component.isSubmitted = true;

    const formSpy = jasmine.createSpyObj('NgForm', ['resetForm']);
    component.operationRequestToPatientForm = formSpy;

    // Act
    component.clearForm();

    // Assert
    expect(component.isSubmitted).toBeFalse();
    expect(component.addRemoveFromPatient.patientId).toBe('');
    expect(component.addRemoveFromPatient.operationRequestId).toBe('');
    expect(formSpy.resetForm).toHaveBeenCalled();
  });
});
