import { ComponentFixture, TestBed } from '@angular/core/testing';
import { UpdatePasswordComponent } from './update-password.component';
import { UserService } from 'src/app/services/user.service';
import { HttpClientTestingModule } from '@angular/common/http/testing';
import { ActivatedRoute } from '@angular/router';
import { of, throwError } from 'rxjs';
import { NgForm } from '@angular/forms';

describe('UpdatePasswordComponent', () => {
  let component: UpdatePasswordComponent;
  let fixture: ComponentFixture<UpdatePasswordComponent>;
  let userService: jasmine.SpyObj<UserService>;
  let activatedRoute: jasmine.SpyObj<ActivatedRoute>;

  beforeEach(async () => {
    const successResponse = { message: 'Password updated successfully' };
    const userServiceMock = {
      updatePassword: jasmine.createSpy('updatePassword').and.returnValue(of(successResponse))
    };
    
    // Mock queryParamMap as an object that contains the 'get' method
    const activatedRouteMock = jasmine.createSpyObj('ActivatedRoute', ['snapshot']);
    activatedRouteMock.snapshot = {
      queryParamMap: {
        get: jasmine.createSpy('get').and.callFake((key: string) => {
          if (key === 'email') {
            return 'test-email'; // Mocked email
          } else if (key === 'token') {
            return 'test-token'; // Mocked token
          }
          return null; // Default return for other keys
        }),
      }
    };

    await TestBed.configureTestingModule({
      imports: [UpdatePasswordComponent, HttpClientTestingModule],
      providers: [
        { provide: UserService, useValue: userServiceMock },
        { provide: ActivatedRoute, useValue: activatedRouteMock }
      ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(UpdatePasswordComponent);
    component = fixture.componentInstance;
    userService = TestBed.inject(UserService) as jasmine.SpyObj<UserService>;
    activatedRoute = TestBed.inject(ActivatedRoute) as jasmine.SpyObj<ActivatedRoute>;

    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should call updatePassword service method on valid form submission', () => {
    // Arrange: Mock the form validity and provide form data
    component.form.newPassword = 'newPassword123';
    const validForm = { valid: true } as NgForm;

    // Act: Call the onSubmit method
    component.onSubmit(validForm);

    // Assert: Ensure the updatePassword method is called with correct parameters
    expect(userService.updatePassword).toHaveBeenCalledWith(
      'test-email', 
      jasmine.any(String),  // Mock token (not used in this test but included for consistency)
      'newPassword123'
    );
  });

  it('should not call updatePassword service method on invalid form submission', () => {
    // Arrange: Mock the form as invalid
    const invalidForm = { valid: false } as NgForm;
    
    // Act: Call the onSubmit method
    component.onSubmit(invalidForm);

    // Assert: Ensure the updatePassword method was not called
    expect(userService.updatePassword).not.toHaveBeenCalled();
  });

  it('should display a success message on successful password update', () => {
    // Act: Call the onSubmit method with valid form
    component.form.newPassword = 'newPassword123';
    component.onSubmit({ valid: true } as NgForm);

    // Assert: Check if the success message is updated correctly
    expect(component.message).toBe('Password updated successfully');
  });

  it('should display an error message on password update failure', () => {
    // Arrange: Mock the error response
    userService.updatePassword.and.returnValue(throwError(() => new Error('Error')));

    // Act: Call the onSubmit method with valid form
    component.form.newPassword = 'newPassword123';
    component.onSubmit({ valid: true } as NgForm);

    // Assert: Check if the error message is displayed
    expect(component.message).toBe('Error changing the password. Please try again.');
  });

  it('should reset the form when clearForm is called', () => {
    // Arrange: Set form data
    component.form.newPassword = 'newPassword123';

    // Act: Call clearForm
    component.clearForm();

    // Assert: Check if form is cleared
    expect(component.form.newPassword).toBeFalsy;
    expect(component.newPasswordForm.pristine).toBeTrue();
  });

});
