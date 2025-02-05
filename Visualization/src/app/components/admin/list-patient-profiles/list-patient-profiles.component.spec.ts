import { ComponentFixture, TestBed } from '@angular/core/testing';
import { PatientService } from 'src/app/services/patient.service';
import { PatientWithId } from 'src/app/domain/patient-with-id';
import { SideBarAdminComponent } from '../sidebar-admin/side-bar-admin.component';
import { CommonModule } from '@angular/common';
import { TableModule } from 'primeng/table';
import { FormsModule, NgForm } from '@angular/forms';
import { MessageComponent } from '../../message/message.component';
import { ListPatientProfiles } from './list-patient-profiles.component';
import { ActivatedRoute } from '@angular/router';
import { of } from 'rxjs';

describe('ListPatientProfiles', () => {
  let component: ListPatientProfiles;
  let fixture: ComponentFixture<ListPatientProfiles>;
  let patientService: PatientService;


  beforeEach(async () => {
    var p1 = {
      name: "José Maria",
      phone: "+123 098098098",
      email: "test@email.t",
      address: "Country, 1234-345, Street of tests",
      dateBirth: "20/10/2000",
      patientId: "test-id"
    } as PatientWithId;
    var p2 = {
      name: "José Mariana",
      phone: "+123 098098097",
      email: "test2@email.t",
      address: "Country, 5432-321, Street of tests",
      dateBirth: "22/10/2000",
      patientId: "test-id2"
    } as PatientWithId;
    const listPatient = [p1, p2];



    const patientServiceMock = {
      getPatientsByFilters: jasmine.createSpy('getPatientsByFilters').and.returnValue(of(listPatient)),
      getPatientById: jasmine.createSpy('getPatientById').and.returnValue(of(p1)),
      deactivatePatientProfile: jasmine.createSpy('deactivatePatientProfile').and.returnValue(of(null)),
      editPatientProfile: jasmine.createSpy('editPatientProfile'),
    };

    const activatedRouteMock = {
      snapshot: {
        paramMap: {
          get: jasmine.createSpy('get').and.returnValue(null),
        },
      },
    };

    await TestBed.configureTestingModule({
      imports: [ListPatientProfiles, SideBarAdminComponent, CommonModule, TableModule, FormsModule, MessageComponent],
      providers: [
        { provide: PatientService, useValue: patientServiceMock },
        { provide: ActivatedRoute, useValue: activatedRouteMock } // Provide the mock ActivatedRoute
      ]
    }).compileComponents();

    fixture = TestBed.createComponent(ListPatientProfiles);
    component = fixture.componentInstance;
    patientService = TestBed.inject(PatientService);
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
  it('should add a new filter', () => {
    const initialFilters = component.queryFiltersList.length;
    component.addFilter();
    expect(component.queryFiltersList.length).toBe(initialFilters + 1);
  });

  it('should remove a filter', () => {
    component.addFilter();
    const initialFilters = component.queryFiltersList.length;
    component.removeFilter(0);
    expect(component.queryFiltersList.length).toBe(initialFilters - 1);
  });

  it('should fetch patients on applyFilters', () => {
    component.applyFilters();
    expect(patientService.getPatientsByFilters).toHaveBeenCalled();
    expect(component.patientList.length).toBe(2);
  });

  it('should toggle details visibility', () => {
    const patient = {
      name: 'José Maria',
      phone: '+123 098098098',
      email: 'test@email.t',
      address: 'Country, 1234-345, Street of tests',
      dateBirth: '20/10/2000',
      patientId: 'test-id'
    };

    component.toggleDetails(patient);
    expect(patientService.getPatientById).toHaveBeenCalledWith(patient.patientId);
    expect(component.fullPatient).toEqual(patient);
    expect(component.detailsVisible).toBeTrue();
  });

  it('should close patient details', () => {
    component.detailsVisible = true;
    component.closeDetails();
    expect(component.detailsVisible).toBeFalse();
  });

  it('should set isEditing to true when editPatientProfile is called', () => {
    const patient = {
      name: 'José Maria',
      phone: '+123 098098098',
      email: 'test@email.t',
      address: 'Country, 1234-345, Street of tests',
      dateBirth: '20/10/2000',
      patientId: 'test-id'
    } as PatientWithId;

    component.editPatientProfile(patient);

    expect(component.isEditing).toBeTrue();
  });


  it('should submit the form when onSubmit is called with valid data', () => {
    component.patient = {
      id: 'test-id',
      name: 'José Maria',
      phone: '+123 098098098',
      email: 'test@email.t',
      address: 'Country, 1234-345, Street of tests',
      dateBirth: '20/10/2000'
    };

    const form = {
      valid: true
    } as NgForm;

    component.onSubmit(form);

    expect(component.isSubmitted).toBeTrue();
    expect(patientService.editPatientProfile).toHaveBeenCalledWith(
      component.patient.id,
      component.patient.name,
      component.patient.phone,
      component.patient.email,
      component.patient.address,
      component.patient.dateBirth
    );
  });

  it('should not submit the form when onSubmit is called with invalid data', () => {
    component.patient = {
      id: '',
      name: '',
      phone: '',
      email: '',
      address: '',
      dateBirth: ''
    };

    const form = {
      valid: false
    } as NgForm;

    component.onSubmit(form);

    expect(component.isSubmitted).toBeFalse();
  });

  it('should reset the form and clear patient data on clearForm', () => {
    component.patient = {
      id: 'test-id',
      name: 'José Maria',
      phone: '+123 098098098',
      email: 'test@email.t',
      address: 'Country, 1234-345, Street of tests',
      dateBirth: '20/10/2000'
    };

    component.clearForm();

    expect(component.isSubmitted).toBeFalse();
    expect(component.patient).toEqual({
      id: '',
      name: '',
      phone: '',
      email: '',
      address: '',
      dateBirth: ''
    });
  });

  it('should delete patient profile', () => {
    const patientToDelete = {
      name: 'José Maria',
      phone: '+123 098098098',
      email: 'test@email.t',
      address: 'Country, 1234-345, Street of tests',
      dateBirth: '20/10/2000',
      patientId: 'test-id'
    } as PatientWithId;

    component.deletePatientProfile(patientToDelete);

    expect(patientService.deactivatePatientProfile).toHaveBeenCalledWith(patientToDelete.patientId);
  });
});
