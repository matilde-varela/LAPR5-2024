import { TestBed, ComponentFixture } from '@angular/core/testing';
import { CreateStaffProfileComponent } from './create-staff-profile.component';
import { StaffService } from '../../../services/staff.service';
import { of } from 'rxjs';
import { FormsModule } from '@angular/forms';
import { RouterTestingModule } from '@angular/router/testing';
import { MessageComponent } from '../../message/message.component';
import { SideBarAdminComponent } from '../sidebar-admin/side-bar-admin.component';

describe('CreateStaffProfileComponent', () => {
  let component: CreateStaffProfileComponent;
  let fixture: ComponentFixture<CreateStaffProfileComponent>;
  let mockStaffService: jasmine.SpyObj<StaffService>;

  beforeEach(async () => {
    mockStaffService = jasmine.createSpyObj('StaffService', ['getAllSpecializationsAvailable', 'createStaffProfile']);
    mockStaffService.getAllSpecializationsAvailable.and.returnValue(of(['Cardiology', 'Pediatrics', 'Orthopedics']));
  
    mockStaffService.createStaffProfile.and.callFake(() => undefined);
  
    await TestBed.configureTestingModule({
      imports: [
        FormsModule,
        RouterTestingModule.withRoutes([]),
        CreateStaffProfileComponent,
        MessageComponent,
        SideBarAdminComponent,
      ],
      providers: [
        { provide: StaffService, useValue: mockStaffService },
      ],
    }).compileComponents();
  
    fixture = TestBed.createComponent(CreateStaffProfileComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('ngOnInit should populate specializations on initialization', () => {
    component.ngOnInit();
    expect(mockStaffService.getAllSpecializationsAvailable).toHaveBeenCalled();
    expect(component.specializations).toEqual(['Cardiology', 'Pediatrics', 'Orthopedics']);
  });

  it('onSubmit should call createStaffProfile when form is valid', () => {
    component.staff = {
      firstName: 'John',
      lastName: 'Doe',
      phone: '1234567890',
      licenseNumber: '12345',
      email: 'john.doe@example.com',
      address: '123 Main St',
      specialization: 'Cardiology',
      function: 'Doctor',
    };

    component.staffForm = {
      valid: true,
      resetForm: jasmine.createSpy('resetForm'),
    } as unknown as any;

    component.onSubmit(component.staffForm);
    expect(mockStaffService.createStaffProfile).toHaveBeenCalledWith(
      'John',
      'Doe',
      '1234567890',
      'john.doe@example.com',
      '123 Main St',
      '12345',
      'Cardiology',
      'Doctor'
    );
  });

  it('onSubmit should not call createStaffProfile when form is invalid', () => {
    component.staffForm = { valid: false } as unknown as any;
    component.onSubmit(component.staffForm);
    expect(mockStaffService.createStaffProfile).not.toHaveBeenCalled();
  });

  it('clearForm should reset staff object and form', () => {
    component.staff = {
      firstName: 'John',
      lastName: 'Doe',
      phone: '1234567890',
      licenseNumber: '12345',
      email: 'john.doe@example.com',
      address: '123 Main St',
      specialization: 'Cardiology',
      function: 'Doctor',
    };

    component.staffForm = { resetForm: jasmine.createSpy('resetForm') } as unknown as any;

    component.clearForm();

    expect(component.staff).toEqual({
      firstName: '',
      lastName: '',
      phone: '',
      licenseNumber: '',
      email: '',
      address: '',
      specialization: '',
      function: '',
    });
    expect(component.staffForm.resetForm).toHaveBeenCalled();
  });

  it('clearForm should remove invalid-placeholder class from input fields', () => {
    const inputMock = document.createElement('input');
    inputMock.classList.add('invalid-placeholder');

    spyOn(document, 'querySelectorAll').and.returnValue([inputMock] as any);

    component.clearForm();

    expect(inputMock.classList.contains('invalid-placeholder')).toBeFalse();
  });
});
