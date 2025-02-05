import { ComponentFixture, TestBed } from '@angular/core/testing';
import { NgForm } from '@angular/forms';
import { LoginComponent } from './login.component';
import { UserService } from 'src/app/services/user.service';
import { of } from 'rxjs';
import { MessageComponent } from '../message/message.component';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { By } from '@angular/platform-browser';
import { ActivatedRoute } from '@angular/router';
import { HttpClientTestingModule } from '@angular/common/http/testing';

describe('LoginComponent', () => {
  let component: LoginComponent;
  let fixture: ComponentFixture<LoginComponent>;
  let userService: UserService;

  beforeEach(async () => {
    const userServiceMock = jasmine.createSpyObj('UserService', ['login', 'loginExternal']);

    const activatedRouteMock = {
      snapshot: {
        paramMap: {
          get: jasmine.createSpy('get').and.returnValue(null),
        },
      },
    };

    await TestBed.configureTestingModule({
      imports: [LoginComponent, MessageComponent, FormsModule, CommonModule],
      providers: [
        { provide: UserService, useValue: userServiceMock },
        { provide: ActivatedRoute, useValue: activatedRouteMock }
      ]
    }).compileComponents();

    fixture = TestBed.createComponent(LoginComponent);
    component = fixture.componentInstance;
    userService = TestBed.inject(UserService);
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should call login on userservice when onSubmit is called', () => {
    const email = "test@email.t";
    const password = "test-password";

    const mockForm = {
      valid: true,
      value: {
        email,
        password,
      }
    } as NgForm;

    component.login = { email, password };
    component.onSubmit(mockForm);

    expect(component.isSubmitted).toBeTrue();
    expect(userService.login).toHaveBeenCalledWith(email, password);
  });

  it('should not allow form submission if form is invalid', () => {
    const mockForm = {
      valid: false,
    } as NgForm;

    component.onSubmit(mockForm);
    expect(component.isSubmitted).toBeFalse();
  });

  it('should disable the submit button when form is invalid', () => {
    const submitButton = fixture.debugElement.query(By.css('button[type="submit"]'));
    const form = fixture.debugElement.query(By.css('form')).injector.get(NgForm);

    form.control.setErrors({ invalid: true });
    fixture.detectChanges();

    expect(submitButton.nativeElement.disabled).toBeTrue();
  });


  it('should clear the form when clearForm is called', () => {
    component.login = { email: 'test@test.com', password: 'password' };
    component.clearForm();

    expect(component.login.email).toBeNull();
    expect(component.login.password).toBeNull();
  });

  it('should reset the input fields back to its original color when clearForm is called', () => {
    const emailInput = fixture.debugElement.query(By.css('input[name="email"]')).nativeElement;
    const passwordInput = fixture.debugElement.query(By.css('input[name="password"]')).nativeElement;
    emailInput.classList.add('invalid-placeholder');
    passwordInput.classList.add('invalid-placeholder');

    component.clearForm();
    fixture.detectChanges();

    expect(emailInput.classList.contains('invalid-placeholder')).toBeFalse();
    expect(passwordInput.classList.contains('invalid-placeholder')).toBeFalse();
  });

  it('should changes input fields that are invalid and/or touched', () => {
    const emailInput = fixture.debugElement.query(By.css('input[name="email"]')).nativeElement;
    const passwordInput = fixture.debugElement.query(By.css('input[name="password"]')).nativeElement;

    const form = fixture.debugElement.query(By.css('form')).injector.get(NgForm);
    form.controls['email']?.setErrors({ required: true });
    form.controls['password']?.setErrors({ required: true });

    emailInput.dispatchEvent(new Event('blur')); // Simulate touched
    passwordInput.dispatchEvent(new Event('blur')); // Simulate touched

    fixture.detectChanges();

    expect(emailInput.classList.contains('invalid-placeholder')).toBeTrue();
    expect(passwordInput.classList.contains('invalid-placeholder')).toBeTrue();
  });

  it('should call loginExternal when loginWithGoogle is called', () => {
    userService.loginExternal();

    component.loginWithGoogle();

    expect(userService.loginExternal).toHaveBeenCalled();
  });
});
