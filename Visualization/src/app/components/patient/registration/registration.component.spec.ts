import { ComponentFixture, TestBed } from '@angular/core/testing';
import { FormsModule, NgForm } from '@angular/forms';
import { RegistrationComponent } from './registration.component';
import { AuthService } from '../../../shared/services/auth.service';
import { MessageComponent } from '../../message/message.component';
import { CommonModule } from '@angular/common';
import { of } from 'rxjs';

describe('RegistrationComponent', () => {
  let component: RegistrationComponent;
  let fixture: ComponentFixture<RegistrationComponent>;
  let authService: AuthService;

  beforeEach(async () => {
    const authServiceMock = {
      CreateUserPatient: jasmine.createSpy('CreateUserPatient').and.returnValue(of({})),
    };

    await TestBed.configureTestingModule({
      imports: [RegistrationComponent, FormsModule, MessageComponent, CommonModule],
      providers: [{ provide: AuthService, useValue: authServiceMock }],
    }).compileComponents();

    fixture = TestBed.createComponent(RegistrationComponent);
    component = fixture.componentInstance;
    authService = TestBed.inject(AuthService);
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should initialize with default userPatient values', () => {
    expect(component.userPatient.email).toBeFalsy();
    expect(component.userPatient.password).toBeFalsy();
    expect(component.userPatient.phone).toBeFalsy();
    expect(component.isSubmitted).toBeFalsy();
  });

  it('should call CreateUserPatient on valid form submission', () => {
    const mockForm = {
      valid: true,
    } as NgForm;

    component.userPatient = { email: 'test@test.com', password: 'password123', phone: '123456789' };

    component.onSubmit(mockForm);

    expect(authService.CreateUserPatient).toHaveBeenCalledWith(
      'test@test.com',
      'password123',
      '123456789'
    );


    expect(component.isSubmitted).toBeTrue();
  });

  it('should not call CreateUserPatient if form is invalid', () => {
    const mockForm = {
      valid: false,
    } as NgForm;

    component.onSubmit(mockForm);

    expect(authService.CreateUserPatient).not.toHaveBeenCalled();

    expect(component.isSubmitted).toBeFalse();
  });

  it('should reset form and userPatient on clearForm', () => {
    component.userPatient = { email: 'test@test.com', password: 'password123', phone: '123456789' };

    component.clearForm();

    expect(component.userPatient.email).toBeFalsy();
    expect(component.userPatient.password).toBeFalsy();
    expect(component.userPatient.phone).toBeFalsy();
    expect(component.isSubmitted).toBeFalsy();
    expect(component.userPatientForm.form.pristine).toBeTrue();
  });
});

