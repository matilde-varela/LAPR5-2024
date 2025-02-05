import { ComponentFixture, TestBed } from '@angular/core/testing';
import { SideBarAdminComponent } from '../sidebar-admin/side-bar-admin.component';
import { CommonModule } from '@angular/common';
import { TableModule } from 'primeng/table';
import { FormsModule, NgForm } from '@angular/forms';
import { MessageComponent } from '../../message/message.component';
import { ActivatedRoute } from '@angular/router';
import { HttpClientTestingModule } from '@angular/common/http/testing';
import { EditStaffProfileComponent } from './edit-staff-profile.component';
import { StaffService } from 'src/app/services/staff.service';
import { of } from 'rxjs';

describe('EditStaffProfileComponent', () => {
  let component: EditStaffProfileComponent;
  let fixture: ComponentFixture<EditStaffProfileComponent>;
  let staffService: StaffService;

  beforeEach(async () => {

    const staffServiceMock = {
      EditStaffProfile: jasmine.createSpy('EditPatientProfile'),
      getAllSpecializationsAvailable: jasmine.createSpy('getAllSpecializationsAvailable').and.returnValue(of(['Specialization 1', 'Specialization 2']))
    };

    const activatedRouteMock = {
      snapshot: {
        paramMap: {
          get: jasmine.createSpy('get').and.returnValue(null),
        },
      },
    };

    await TestBed.configureTestingModule({
      imports: [EditStaffProfileComponent, SideBarAdminComponent, CommonModule, TableModule, FormsModule, MessageComponent, HttpClientTestingModule],
      providers: [
        { provide: StaffService, useValue: staffServiceMock },
        { provide: ActivatedRoute, useValue: activatedRouteMock } // Provide the mock ActivatedRoute
      ]
    }).compileComponents();

    fixture = TestBed.createComponent(EditStaffProfileComponent);
    component = fixture.componentInstance;
    staffService = TestBed.inject(StaffService);
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should load no data when component initializes', () => {
    // Check that the patient data is loaded
    expect(component.staff.id).toBeFalsy;
    expect(component.staff.email).toBeFalsy;
    expect(component.staff.address).toBeFalsy;
    expect(component.staff.phone).toBeFalsy;
    expect(component.staff.specializationId).toBeFalsy;
  });

  it('should call EditStaffProfile service method on form submit with correct data', () => {
    component.staff.id = 'test-id';
    component.staff.phone = 'New Phone';
    component.staff.email = 'new-email@test.com';
    component.staff.address = 'New Address';
    component.staff.specializationId = 'id-specialization';

    // Call onSubmit with a valid form
    component.onSubmit(component.staffForm);

    // Ensure the EditPatientProfile method was called with the correct parameters
    expect(staffService.EditStaffProfile).toHaveBeenCalledWith(
      'test-id',
      'New Phone',
      'new-email@test.com',
      'New Address',
      'id-specialization'
    );
  });

  it('should not call EditStaffProfile service method when form is invalid', () => {
    // Simulate an invalid form submission
    component.staffForm.control.setErrors({ invalid: true });
    component.onSubmit(component.staffForm);

    // Ensure the service method was not called
    expect(staffService.EditStaffProfile).not.toHaveBeenCalled();
  });

  it('should reset form and clear patient data when clearForm is called', () => {
    component.staff.id = 'test-id';
    component.staff.phone = 'New Phone';
    component.staff.email = 'new-email@test.com';
    component.staff.address = 'New Address';
    component.staff.specializationId = 'id-specialization';
  
    component.clearForm();
  
    // Use null or empty string for flexible validation
    expect(component.staff.id).toBeFalsy;
    expect(component.staff.email).toBeFalsy;
    expect(component.staff.address).toBeFalsy;
    expect(component.staff.phone).toBeFalsy;
    expect(component.staff.specializationId).toBeFalsy;
  
    expect(component.staffForm.form.pristine).toBeTrue();
  });
  

});
