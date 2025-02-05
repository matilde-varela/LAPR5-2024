import { ComponentFixture, TestBed } from '@angular/core/testing';
import { CreateAllergyComponent } from './create-allergy.component';
import { FormsModule, NgForm } from '@angular/forms';
import { AllergyService } from 'src/app/services/allergy.service';
import { SideBarAdminComponent } from '../sidebar-admin/side-bar-admin.component';
import { MessageComponent } from '../../message/message.component';
import { CommonModule } from '@angular/common';
import { ActivatedRoute } from '@angular/router';

describe('CreateAllergyComponent', () => {
  let component: CreateAllergyComponent;
  let fixture: ComponentFixture<CreateAllergyComponent>;
  let allergyServiceMock: AllergyService;

  beforeEach(() => {
    const serviceMock = jasmine.createSpyObj('AllergyService', ['createAllergy']);
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
        { provide: AllergyService, useValue: serviceMock },
        { provide: ActivatedRoute, useValue: activatedRouteMock }
      ],
    });

    fixture = TestBed.createComponent(CreateAllergyComponent);
    component = fixture.componentInstance;
    allergyServiceMock = TestBed.inject(AllergyService);
    fixture.detectChanges();
  });

  it('should create the component', () => {
    expect(component).toBeTruthy();
  });

  it('should initialize allergy with default values', () => {
    expect(component.allergy).toEqual({
      code: '',
      designation: '',
      description: '',
    });
  });

  it('should call createAllergy on service when form is valid', () => {
    const validForm: NgForm = { valid: true } as NgForm; // Mocking NgForm
    component.allergy.code = 'A01';
    component.allergy.designation = 'Peanut Allergy';
    component.allergy.description = 'Severe allergy to peanuts';

    component.onSubmit(validForm);

    expect(allergyServiceMock.createAllergy).toHaveBeenCalledOnceWith(
      component.allergy.code,
      component.allergy.designation,
      component.allergy.description
    );
    expect(component.isSubmitted).toBeTrue();
  });

  it('should set isSubmitted to false when form is invalid', () => {
    const invalidForm: NgForm = { valid: false } as NgForm;

    component.onSubmit(invalidForm);

    expect(component.isSubmitted).toBeFalse();
  });

  it('should clear the form and reset allergy on clearForm', () => {
    component.allergy = {
      code: 'A01',
      designation: 'Test Allergy',
      description: 'Test Description',
    };

    const resetFormSpy = spyOn(component.allergyForm, 'resetForm');

    component.clearForm();

    expect(component.allergy).toEqual({
      code: '',
      designation: '',
      description: '',
    });

    expect(component.allergyForm.resetForm).toHaveBeenCalled();

    const inputs = document.querySelectorAll('.input-field input');
    inputs.forEach(input => {
      expect(input.classList).not.toContain('invalid-placeholder');
    });
  });
});
