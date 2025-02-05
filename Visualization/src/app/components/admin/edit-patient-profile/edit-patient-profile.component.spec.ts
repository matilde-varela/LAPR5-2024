import { ComponentFixture, TestBed } from '@angular/core/testing';
import { EditPatientProfileComponent } from './edit-patient-profile.component';
import { PatientService } from 'src/app/services/patient.service';
import { PatientWithId } from 'src/app/domain/patient-with-id';
import { SideBarAdminComponent } from '../sidebar-admin/side-bar-admin.component';
import { CommonModule } from '@angular/common';
import { TableModule } from 'primeng/table';
import { FormsModule, NgForm } from '@angular/forms';
import { MessageComponent } from '../../message/message.component';
import { ActivatedRoute } from '@angular/router';
import { HttpClientTestingModule } from '@angular/common/http/testing';

describe('EditPatientProfileComponent', () => {
  let component: EditPatientProfileComponent;
  let fixture: ComponentFixture<EditPatientProfileComponent>;
  let patientService: PatientService;

  beforeEach(async () => {

    const patientServiceMock = {
      editPatientProfile: jasmine.createSpy('editPatientProfile')
    };

    const activatedRouteMock = {
      snapshot: {
        paramMap: {
          get: jasmine.createSpy('get').and.returnValue(null),
        },
      },
    };

    await TestBed.configureTestingModule({
      imports: [EditPatientProfileComponent, SideBarAdminComponent, CommonModule, TableModule, FormsModule, MessageComponent, HttpClientTestingModule],
      providers: [
        { provide: PatientService, useValue: patientServiceMock },
        { provide: ActivatedRoute, useValue: activatedRouteMock } // Provide the mock ActivatedRoute
      ]
    }).compileComponents();

    fixture = TestBed.createComponent(EditPatientProfileComponent);
    component = fixture.componentInstance;
    patientService = TestBed.inject(PatientService);
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should load no data when component initializes', () => {
    // Check that the patient data is loaded
    expect(component.patient.name).toBe('');
    expect(component.patient.phone).toBe('');
    expect(component.patient.email).toBe('');
    expect(component.patient.address).toBe('');
    expect(component.patient.dateBirth).toBe('');
  });

  it('should call EditPatientProfile service method on form submit with correct data', () => {
    component.patient.id = 'test-id';
    component.patient.name = 'New Name';
    component.patient.phone = 'New Phone';
    component.patient.email = 'new-email@test.com';
    component.patient.address = 'New Address';
    component.patient.dateBirth = '01/01/1990';

    // Call onSubmit with a valid form
    component.onSubmit(component.patientForm);

    // Ensure the EditPatientProfile method was called with the correct parameters
    expect(patientService.editPatientProfile).toHaveBeenCalledWith(
      'test-id',
      'New Name',
      'New Phone',
      'new-email@test.com',
      'New Address',
      '01/01/1990'
    );
  });

  it('should not call EditPatientProfile service method when form is invalid', () => {
    // Simulate an invalid form submission
    component.patientForm.control.setErrors({ invalid: true });
    component.onSubmit(component.patientForm);

    // Ensure the service method was not called
    expect(patientService.editPatientProfile).not.toHaveBeenCalled();
  });

  it('should reset form and clear patient data when clearForm is called', () => {
    component.patient.name = 'Test Name';
    component.patient.phone = 'Test Phone';
    component.patient.email = 'Test Email';
    component.patient.address = 'Test Address';
    component.patient.dateBirth = 'Test Date of Birth';
  
    component.clearForm();
  
    // Use null or empty string for flexible validation
    expect(component.patient.name).toBeFalsy(); // Allows '' or null
    expect(component.patient.phone).toBeFalsy();
    expect(component.patient.email).toBeFalsy();
    expect(component.patient.address).toBeFalsy();
    expect(component.patient.dateBirth).toBeFalsy();
  
    expect(component.patientForm.form.pristine).toBeTrue();
  });
  

});
