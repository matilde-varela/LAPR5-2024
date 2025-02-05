import { ComponentFixture, TestBed } from '@angular/core/testing';
import { CreatePatientProfileComponent } from './create-patient-profile.component';
import { PatientService } from '../../../services/patient.service';
import { ActivatedRoute } from '@angular/router';
import { FormsModule, NgForm } from '@angular/forms';
import { of } from 'rxjs';
import { SideBarAdminComponent } from '../sidebar-admin/side-bar-admin.component';
import { MessageComponent } from '../../message/message.component';
import { CommonModule } from '@angular/common';
import { DropdownModule } from 'primeng/dropdown';
import { AllergyService } from 'src/app/services/allergy.service';
import { MedicalConditionService } from 'src/app/services/medical-condition.service';

describe('CreatePatientProfileComponent', () => {
  let component: CreatePatientProfileComponent;
  let fixture: ComponentFixture<CreatePatientProfileComponent>;
  let mockPatientService: jasmine.SpyObj<PatientService>;
  let mockAllergyService: jasmine.SpyObj<AllergyService>;
  let mockMedicalConditionService: jasmine.SpyObj<MedicalConditionService>;
  let mockMessageComponent: jasmine.SpyObj<MessageComponent>;


  beforeEach(async () => {
    mockPatientService = jasmine.createSpyObj('PatientService', ['createPatientProfile']);
    mockAllergyService = jasmine.createSpyObj('AllergyService', ['getAllAllergies']);
    mockMedicalConditionService = jasmine.createSpyObj('MedicalConditionService', ['getAllMedicalConditions']);

    mockAllergyService.getAllAllergies.and.returnValue(of([
      { code: 'A1', designation: 'Peanut' },
      { code: 'A2', designation: 'Shellfish' },
      { code: 'A3', designation: 'Pollen' },
      { code: 'A4', designation: 'Milk' },
      { code: 'A5', designation: 'Eggs' }
    ]));

    mockMedicalConditionService.getAllMedicalConditions.and.returnValue(of([
      { id: 'M1', designation: 'Hypertension', symptoms: 'Headaches, shortness of breath, nosebleeds' },
      { id: 'M2', designation: 'Asthma', symptoms: 'Coughing, wheezing, shortness of breath' },
      { id: 'M3', designation: 'Diabetes', symptoms: 'Increased thirst, frequent urination, extreme fatigue' },
      { id: 'M4', designation: 'Osteoarthritis', symptoms: 'Joint pain, stiffness, swelling' },
      { id: 'M5', designation: 'Anemia', symptoms: 'Fatigue, weakness, pale skin' }
    ]));

    const activatedRouteMock = {
      snapshot: {
        paramMap: {
          get: jasmine.createSpy('get').and.returnValue(null),
        },
      },
    };

    await TestBed.configureTestingModule({
      imports: [SideBarAdminComponent, MessageComponent, FormsModule, CommonModule, DropdownModule],
      providers: [
        { provide: PatientService, useValue: mockPatientService },
        { provide: AllergyService, useValue: mockAllergyService },
        { provide: MedicalConditionService, useValue: mockMedicalConditionService },
        { provide: ActivatedRoute, useValue: activatedRouteMock },
      ],
    }).compileComponents();

    fixture = TestBed.createComponent(CreatePatientProfileComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create the component', () => {
    expect(component).toBeTruthy();
  });

  it('should call PatientService.createPatientProfile with correct parameters on valid form submit', () => {
    // Arrange
    component.patient = {
      firstName: 'John',
      lastName: 'Doe',
      phone: '1234567890',
      emergencyContact: '0987654321',
      email: 'john.doe@example.com',
      address: '123 Main St',
      gender: 'Male',
      dateBirth: '2000-01-01',
      medicalConditions : [],
      allergies: [],
      description: ''
    };

    component.patientForm = {
      valid: true,
      resetForm: jasmine.createSpy('resetForm'),
    } as unknown as NgForm;

    // Act
    component.onSubmit(component.patientForm);

    // Assert
    expect(mockPatientService.createPatientProfile).toHaveBeenCalledWith(
      'John',
      'Doe',
      '1234567890',
      'john.doe@example.com',
      '123 Main St',
      '0987654321',
      'Male',
      '2000-01-01',
      [],
      [],
      ''
    );
  });

  it('should not call PatientService.createPatientProfile on invalid form submit', () => {
    // Arrange
    const form = { valid: false } as unknown as NgForm;

    // Act
    component.onSubmit(form);

    // Assert
    expect(mockPatientService.createPatientProfile).not.toHaveBeenCalled();
  });

  it('should reset the form and fields when clearForm is called', () => {
    // Arrange
    component.patient = {
      firstName: 'John',
      lastName: 'Doe',
      phone: '1234567890',
      emergencyContact: '0987654321',
      email: 'john.doe@example.com',
      address: '123 Main St',
      gender: 'Male',
      dateBirth: '2000-01-01',
      medicalConditions : [],
      allergies: [],
      description: ''
    };
    spyOn(component.patientForm, 'resetForm');

    // Act
    component.clearForm();

    // Assert
    expect(component.isSubmitted).toBeFalse();
    expect(component.patient).toEqual({
      firstName: '',
      lastName: '',
      phone: '',
      emergencyContact: '',
      email: '',
      address: '',
      gender: '',
      dateBirth: '',
      medicalConditions : [],
      allergies: [],
      description: ''
    });
    expect(component.patientForm.resetForm).toHaveBeenCalled();
  });

  it('should remove invalid-placeholder class from input fields in clearForm', () => {
    // Arrange
    const mockInput = document.createElement('input');
    mockInput.classList.add('invalid-placeholder');

    const mockNodeList = {
      forEach: (callback: (el: Element) => void) => callback(mockInput),
      length: 1,
      item: (index: number) => (index === 0 ? mockInput : null),
      [0]: mockInput,
    } as unknown as NodeListOf<HTMLInputElement>;

    spyOn(document, 'querySelectorAll').and.returnValue(mockNodeList);

    // Act
    component.clearForm();

    // Assert
    expect(mockInput.classList.contains('invalid-placeholder')).toBeFalse();
  });
});
