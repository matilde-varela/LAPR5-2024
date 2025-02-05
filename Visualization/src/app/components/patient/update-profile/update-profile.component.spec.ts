import { ComponentFixture, TestBed } from '@angular/core/testing';
import { FormsModule, NgForm } from '@angular/forms';
import { UpdateProfileComponent } from './update-profile.component';
import { PatientService } from '../../../services/patient.service';
import { MessageComponent } from '../../message/message.component';
import { SideBarPatientComponent } from '../sidebar-patient/side-bar-patient.component';
import { ActivatedRoute } from '@angular/router';
import { of } from 'rxjs';

describe('UpdateProfileComponent', () => {
  let component: UpdateProfileComponent;
  let fixture: ComponentFixture<UpdateProfileComponent>;
  let patientServiceMock: jasmine.SpyObj<PatientService>;

  beforeEach(async () => {
    // Create a mock for PatientService
    patientServiceMock = jasmine.createSpyObj('PatientService', ['updateProfile']);

    // Mock ActivatedRoute if needed
    const activatedRouteMock = jasmine.createSpyObj('ActivatedRoute', ['snapshot']);
    activatedRouteMock.snapshot = { paramMap: of({}) };

    await TestBed.configureTestingModule({
      imports: [
        FormsModule,
        MessageComponent,
        SideBarPatientComponent,
      ],
      providers: [
        { provide: PatientService, useValue: patientServiceMock },
        { provide: ActivatedRoute, useValue: activatedRouteMock },
      ],
    }).compileComponents();

    fixture = TestBed.createComponent(UpdateProfileComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create the component', () => {
    expect(component).toBeTruthy();
  });

  it('should submit form and call updateProfile when form is valid', () => {
    // Arrange
    component.patient.name = 'John Doe';
    component.patient.phone = '123456789';
    component.patient.email = 'john.doe@example.com';
    component.patient.address = '123 Main St';
    component.patient.emergencyContact = '987654321';
    
    const form = component.patientForm;
    component.isSubmitted = false;

    // Act
    component.onSubmit(form);

    // Assert
    expect(patientServiceMock.updateProfile).toHaveBeenCalledWith(
      'John Doe',
      '123456789',
      'john.doe@example.com',
      '123 Main St',
      '987654321'
    );
  });

  it('should clear the form correctly', () => {
    // Arrange
    component.patient.name = 'John Doe';
    component.patient.phone = '123456789';
  
    // Act
    component.clearForm();
  
    // Assert
    expect(component.patient.name).toBeNull();
    expect(component.patient.phone).toBeNull();
    expect(component.patientForm.form.pristine).toBeTrue();
    expect(component.isSubmitted).toBeFalse();
  });

  it('should disable submit button if form is submitted', () => {
    // Act
    component.isSubmitted = true;
    fixture.detectChanges();
  
    // Assert
    const submitButton = fixture.debugElement.nativeElement.querySelector('.add-button-submit');
    expect(submitButton.disabled).toBeTrue();
  });
  

  it('should enable submit button if form is not submitted', () => {
    // Act
    component.isSubmitted = false;

    // Assert
    const submitButton = fixture.debugElement.nativeElement.querySelector('.add-button-submit');
    expect(submitButton.disabled).toBeFalse();
  });
});
