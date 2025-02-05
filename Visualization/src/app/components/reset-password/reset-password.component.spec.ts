import { ComponentFixture, TestBed } from '@angular/core/testing';
import { ResetPasswordComponent } from './reset-password.component';
import { UserService } from 'src/app/services/user.service';
import { FormsModule, NgForm } from '@angular/forms';
import { MessageComponent } from '../message/message.component';
import { CommonModule } from '@angular/common';
import { HttpClientTestingModule } from '@angular/common/http/testing';

describe('ResetPasswordComponent', () => {
  let component: ResetPasswordComponent;
  let fixture: ComponentFixture<ResetPasswordComponent>;
  let userService: UserService;

  beforeEach(async () => {
    const userServiceMock = {
      resetPassword: jasmine.createSpy('resetPassword')
    };

    await TestBed.configureTestingModule({
      imports: [ResetPasswordComponent, FormsModule, CommonModule, MessageComponent, HttpClientTestingModule],
      providers: [
        { provide: UserService, useValue: userServiceMock }
      ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(ResetPasswordComponent);
    component = fixture.componentInstance;
    userService = TestBed.inject(UserService);

    // Trigger change detection to ensure @ViewChild is populated
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should call resetPassword service method on form submit with correct data', () => {
    component.resetPass.email = 'new-email@test.com';

    const formMock = {
      valid: true
    } as NgForm;

    // Call onSubmit with a valid form
    component.onSubmit(formMock);

    // Ensure resetPassword was called with the correct data
    expect(userService.resetPassword).toHaveBeenCalledWith('new-email@test.com');
  });

  it('should not call resetPassword service method when form is invalid', () => {
    const formMock = {
      valid: false
    } as NgForm;

    // Call onSubmit with an invalid form
    component.onSubmit(formMock);

    // Ensure resetPassword was not called
    expect(userService.resetPassword).not.toHaveBeenCalled();
  });

  it('should reset form and clear patient data when clearForm is called', () => {
    component.resetPass.email = 'Test Email';

    // Call clearForm
    component.clearForm();

    // Verify that email was cleared
    expect(component.resetPass.email).toBeFalsy;
  });
});
