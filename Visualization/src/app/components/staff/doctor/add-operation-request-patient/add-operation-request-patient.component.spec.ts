import { ComponentFixture, TestBed } from '@angular/core/testing';
import { AddOperationRequestPatientComponent } from './add-operation-request-patient.component';
import { OperationRequestService } from '../../../../services/operation-request.service';
import { of } from 'rxjs';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { SideBarDoctorComponent } from '../sidebar-doctor/side-bar-doctor.component';
import { MessageComponent } from '../../../message/message.component';
import { ActivatedRoute } from '@angular/router';

describe('AddOperationRequestPatientComponent', () => {
  let component: AddOperationRequestPatientComponent;
  let fixture: ComponentFixture<AddOperationRequestPatientComponent>;
  let service: jasmine.SpyObj<OperationRequestService>;

  beforeEach(async () => {
    // Create a spy for OperationRequestService
    service = jasmine.createSpyObj('OperationRequestService', ['addOperationRequestToPatient']);

    const activatedRouteMock = {
      snapshot: {
        paramMap: {
          get: jasmine.createSpy('get').and.returnValue(null),
        },
      },
    }

    await TestBed.configureTestingModule({
      imports: [AddOperationRequestPatientComponent, FormsModule, CommonModule, SideBarDoctorComponent, MessageComponent],
      providers: [{ provide: OperationRequestService, useValue: service },
        { provide: ActivatedRoute, useValue: activatedRouteMock }]
    }).compileComponents();

    fixture = TestBed.createComponent(AddOperationRequestPatientComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should not call addOperationRequestToPatient on invalid form submission', () => {
    // Arrange
    const invalidForm = { valid: false } as any;  // Mock an invalid form

    // Act
    component.onSubmit(invalidForm);

    // Assert
    expect(service.addOperationRequestToPatient).not.toHaveBeenCalled();
    expect(component.isSubmitted).toBeFalse();  // Ensure isSubmitted is false when form is invalid
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
