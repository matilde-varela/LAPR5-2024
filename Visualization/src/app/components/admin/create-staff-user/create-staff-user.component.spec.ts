import { ComponentFixture, TestBed } from '@angular/core/testing';
import { CreateStaffUserComponent } from './create-staff-user.component';
import { ActivatedRoute, Router } from '@angular/router';
import { UserService } from '../../../services/user.service';
import { of } from 'rxjs';
import { FormsModule, NgForm } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { TableModule } from 'primeng/table';
import { MessageComponent } from '../../message/message.component';

describe('CreateStaffUserComponent', () => {
  let component: CreateStaffUserComponent;
  let fixture: ComponentFixture<CreateStaffUserComponent>;
  let mockUserService: jasmine.SpyObj<UserService>;
  let mockRouter: jasmine.SpyObj<Router>;

  beforeEach(async () => {
    // Create mock services

    const userServiceMock = {
      createStaffUser: jasmine.createSpy('createStaffUser')
    };

    const activatedRouteMock = {
      snapshot: {
        paramMap: {
          get: jasmine.createSpy('get').and.returnValue(null),
        },
      },
    };

    await TestBed.configureTestingModule({
      imports: [CreateStaffUserComponent, FormsModule, CommonModule, TableModule, MessageComponent],
      providers: [
        { provide: UserService, useValue: userServiceMock },
        { provide: ActivatedRoute, useValue: activatedRouteMock }
      ]
    }).compileComponents();

    fixture = TestBed.createComponent(CreateStaffUserComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create the component', () => {
    expect(component).toBeTruthy();
  });

  it('should load no data when component initializes', () => {
    // Check that the patient data is loaded
    expect(component.userStaff.email).toBe('');
    expect(component.userStaff.password).toBe('');
    expect(component.userStaff.phone).toBe('');
    expect(component.userStaff.role).toBe('');
  });

  it('should not call createStaffUser when form is invalid', () => {
    const mockForm = {
      valid: false
    } as NgForm;

    component.onSubmit(mockForm);

    expect(component.isSubmitted).toBeFalse();
  });


  it('should reset the form when clearForm is called', () => {
    component.userStaff = {
      email: 'test@example.com',
      password: 'password123',
      phone: '123456789',
      role: 'Doctor'
    };

    component.clearForm();

    expect(component.userStaff.email).toBeNull();
    expect(component.userStaff.password).toBeNull();
    expect(component.userStaff.phone).toBeNull();
    expect(component.userStaff.role).toBeNull();
    expect(component.isSubmitted).toBe(false);
  });

});
