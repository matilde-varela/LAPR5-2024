import { ComponentFixture, TestBed } from '@angular/core/testing';
import { ListPatientProfilesWithMedicalRecord } from './list-patient-profiles-with-medical-records.component';
import { PatientService } from 'src/app/services/patient.service';
import { AllergyService } from 'src/app/services/allergy.service';
import { MedicalConditionService } from 'src/app/services/medical-condition.service';
import { of } from 'rxjs';
import { FormsModule, NgForm } from '@angular/forms';
import { PatientWithId } from 'src/app/domain/patient-with-id';
import { ActivatedRoute } from '@angular/router';
import { SideBarDoctorComponent } from '../sidebar-doctor/side-bar-doctor.component';

describe('ListPatientProfilesWithMedicalRecord', () => {
  let component: ListPatientProfilesWithMedicalRecord;
  let fixture: ComponentFixture<ListPatientProfilesWithMedicalRecord>;
  let mockPatientService: jasmine.SpyObj<PatientService>;
  let mockAllergyService: jasmine.SpyObj<AllergyService>;
  let mockMedicalConditionService: jasmine.SpyObj<MedicalConditionService>;

  beforeEach(async () => {
    mockPatientService = jasmine.createSpyObj('PatientService', [
      'getPatientsByFilters',
      'getAllMedicalRecords',
      'getFilteredMedicalRecords',
      'updateMedicalRecord',
    ]);
    mockAllergyService = jasmine.createSpyObj('AllergyService', ['getAllAllergies']);
    mockMedicalConditionService = jasmine.createSpyObj('MedicalConditionService', ['getAllMedicalConditions']);

    const activatedRouteMock = {
      snapshot: {
        paramMap: {
          get: jasmine.createSpy('get').and.returnValue(null),
        },
      },
    };

    await TestBed.configureTestingModule({
      imports: [SideBarDoctorComponent, FormsModule],
      providers: [
        { provide: PatientService, useValue: mockPatientService },
        { provide: AllergyService, useValue: mockAllergyService },
        { provide: MedicalConditionService, useValue: mockMedicalConditionService },
        { provide: ActivatedRoute, useValue: activatedRouteMock }
      ],
    }).compileComponents();

    fixture = TestBed.createComponent(ListPatientProfilesWithMedicalRecord);
    component = fixture.componentInstance;
  });

  it('should create the component', () => {
    expect(component).toBeTruthy();
  });

  it('should initialize with patients', () => {
    const mockPatients: PatientWithId[] = [{
      name: 'John Doe',
      phone: '123456789',
      email: 'john.doe@example.com',
      address: '123 Main St',
      dateBirth: '1990-01-01',
      patientId: '1',
    }];

    mockPatientService.getPatientsByFilters.and.returnValue(of(mockPatients));
    component.fetchPatients();

    expect(mockPatientService.getPatientsByFilters).toHaveBeenCalled();
    expect(component.patientList).toEqual(mockPatients);
  });

  it('should initialize with medical records', () => {
    const mockRecords = [{
      id: '1',
      medicalRecordNumber: 'MR001',
      medicalConditions: [],
      allergies: [],
      description: 'No issues.',
    }];

    mockPatientService.getAllMedicalRecords.and.returnValue(of(mockRecords));
    component.fetchPatientsMedicalRecords();

    expect(mockPatientService.getAllMedicalRecords).toHaveBeenCalled();
    expect(component.medicalRecords).toEqual(mockRecords);
  });

  it('should fetch allergies and populate designations', () => {
    const mockAllergies = [{ code: 'A01', designation: 'Peanut Allergy', description: 'Severe allergy to peanuts' }];

    mockAllergyService.getAllAllergies.and.returnValue(of(mockAllergies));
    component.fetchAllergies();

    expect(mockAllergyService.getAllAllergies).toHaveBeenCalled();
    expect(component.allAllergiesDesignations).toEqual(['Peanut Allergy']);
  });

  it('should fetch medical conditions and populate designations', () => {
    const mockConditions = [{
      id: 'FB70.0',
      designation: 'Low back pain',
      description: 'Pain in the lower back.',
      symptoms: 'Pain, stiffness',
    }];

    mockMedicalConditionService.getAllMedicalConditions.and.returnValue(of(mockConditions));
    component.fetchMedicalConditions();

    expect(mockMedicalConditionService.getAllMedicalConditions).toHaveBeenCalled();
    expect(component.allMedicalConditionsDesignations).toEqual(['Low back pain']);
  });

  it('should toggle medical record visibility', () => {
    const mockPatient: PatientWithId = {
      name: 'John Doe',
      phone: '123456789',
      email: 'john.doe@example.com',
      address: '123 Main St',
      dateBirth: '1990-01-01',
      patientId: '1',
    };

    const mockRecord = {
      id: '1',
      medicalRecordNumber: 'MR001',
      medicalConditions: [],
      allergies: [],
      description: 'No issues.',
    };

    component.medicalRecords = [mockRecord];
    component.toggleMedicalRecord(mockPatient);

    expect(component.fullPatient).toEqual(mockPatient);
    expect(component.fullPatientMedicalRecord).toEqual(mockRecord);
    expect(component.showMedicalRecord).toBeTrue();
  });

  it('should handle form submission and update medical record', () => {
    component.medicalRecord = {
      id: '1',
      medicalRecordNumber: 'MR001',
      medicalConditions: [],
      allergies: [],
      description: 'Updated description.',
    };

    component.onSubmit({ valid: true } as NgForm);

    expect(mockPatientService.updateMedicalRecord).toHaveBeenCalledWith(
      '1',
      'MR001',
      [],
      [],
      'Updated description.'
    );
  });

  it('should add a medical condition if valid', () => {
    component.allFullMedicalConditions = [{
      id: 'FB70.0',
      designation: 'Low back pain',
      description: 'Pain in the lower back.',
      symptoms: 'Pain, stiffness',
    }];

    component.addMedicalCondition('Low back pain');

    expect(component.medicalRecord.medicalConditions).toEqual([{
      id: 'FB70.0',
      designation: 'Low back pain',
      description: 'Pain in the lower back.',
      symptoms: 'Pain, stiffness',
    }]);
  });

  it('should not add duplicate medical conditions', () => {
    component.medicalRecord.medicalConditions = [{
      id: 'FB70.0',
      designation: 'Low back pain',
      description: 'Pain in the lower back.',
      symptoms: 'Pain, stiffness',
    }];

    component.allFullMedicalConditions = [{
      id: 'FB70.0',
      designation: 'Low back pain',
      description: 'Pain in the lower back.',
      symptoms: 'Pain, stiffness',
    }];

    component.addMedicalCondition('Low back pain');

    expect(component.medicalRecord.medicalConditions.length).toBe(1);
  });

});
