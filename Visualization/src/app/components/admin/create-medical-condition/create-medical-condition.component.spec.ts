import { ComponentFixture, TestBed } from '@angular/core/testing';
import { CreateMedicalConditionComponent } from './create-medical-condition.component';
import { FormsModule, NgForm } from '@angular/forms';
import { MedicalConditionService } from 'src/app/services/medical-condition.service';
import { of } from 'rxjs';
import { SideBarAdminComponent } from '../sidebar-admin/side-bar-admin.component';
import { MessageComponent } from '../../message/message.component';
import { CommonModule } from '@angular/common';
import { ActivatedRoute } from '@angular/router';

describe('CreateMedicalConditionComponent', () => {
  let component: CreateMedicalConditionComponent;
  let fixture: ComponentFixture<CreateMedicalConditionComponent>;
  let medicalConditionServiceMock: MedicalConditionService;

  beforeEach(() => {
    const serviceMock = jasmine.createSpyObj('MedicalConditionService', ['createMedicalCondition']);
    const activatedRouteMock = {
      snapshot: {
        paramMap: {
          get: jasmine.createSpy('get').and.returnValue(null),
        },
      },
    };

    TestBed.configureTestingModule({
      imports: [SideBarAdminComponent, MessageComponent, FormsModule, CommonModule],
      providers: [
        { provide: MedicalConditionService, useValue: serviceMock },
        { provide: ActivatedRoute, useValue: activatedRouteMock }
      ]
    });

    fixture = TestBed.createComponent(CreateMedicalConditionComponent);
    component = fixture.componentInstance;
    medicalConditionServiceMock = TestBed.inject(MedicalConditionService);
    fixture.detectChanges();
  });

  it('should create the component', () => {
    expect(component).toBeTruthy();
  });

  it('should initialize medical condition with default values', () => {
    expect(component.medicalCondition).toEqual({
      id: '',
      designation: '',
      description: '',
      symptoms: ''
    });
  });

  it('should call createMedicalCondition on service when form is valid', () => {
    const validForm: NgForm = { valid: true } as NgForm; // Mocking NgForm
    component.medicalCondition.id = '1';
    component.medicalCondition.designation = 'Condition';
    component.medicalCondition.description = 'Description';
    component.medicalCondition.symptoms = 'Symptoms';

    component.onSubmit(validForm);

    expect(medicalConditionServiceMock.createMedicalCondition).toHaveBeenCalledOnceWith(
      component.medicalCondition.id,
      component.medicalCondition.designation,
      component.medicalCondition.description,
      component.medicalCondition.symptoms
    );
    expect(component.isSubmitted).toBeTrue();
  });

  it('should set isSubmitted to false when form is invalid', () => {
    const invalidForm: NgForm = { valid: false } as NgForm;

    component.onSubmit(invalidForm);

    expect(component.isSubmitted).toBeFalse();
  });

  it('should clear the form and reset medical condition on clearForm', () => {
    // Set some values in the medicalCondition object
    component.medicalCondition = {
      id: '1',
      designation: 'Test Condition',
      description: 'Test Description',
      symptoms: 'Test Symptoms'
    };

    // Spy on the resetForm method of the NgForm
    const resetFormSpy = spyOn(component.medicalConditionForm, 'resetForm');

    // Call clearForm
    component.clearForm();

    // Check if medicalCondition is reset
    expect(component.medicalCondition).toEqual({
      id: '',
      designation: '',
      description: '',
      symptoms: ''
    });

    // Ensure resetForm was called
    expect(resetFormSpy).toHaveBeenCalled();

    // Check if input fields have invalid placeholder removed
    const inputs = document.querySelectorAll('.input-field input');
    inputs.forEach(input => {
      expect(input.classList).not.toContain('invalid-placeholder');
    });
  });
});
