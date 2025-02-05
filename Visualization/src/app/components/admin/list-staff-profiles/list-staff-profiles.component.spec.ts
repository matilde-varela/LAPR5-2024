import { ComponentFixture, fakeAsync, TestBed, tick } from '@angular/core/testing';
import { ListStaffProfiles } from './list-staff-profiles.component';
import { StaffService } from 'src/app/services/staff.service';
import { of } from 'rxjs';
import { StaffWithId } from 'src/app/domain/staff-with-id';
import { SideBarAdminComponent } from '../sidebar-admin/side-bar-admin.component';
import { CommonModule } from '@angular/common';
import { TableModule } from 'primeng/table';
import { FormsModule, NgForm } from '@angular/forms';
import { MessageComponent } from '../../message/message.component';
import { ActivatedRoute } from '@angular/router';


describe('ListStaffProfiles', () => {
  let component: ListStaffProfiles;
  let fixture: ComponentFixture<ListStaffProfiles>;
  let staffService: StaffService;

  const listStaff: StaffWithId[] = [
    {
      id: "test",
      name: "José Maria",
      phone: "+123 098098098",
      email: "test@email.t",
      address: "Country, 1234-345, Street of tests",
      specializationId: "test1",
      slots: [],
      status: "teststat",
    },
    {
      id: "test2",
      name: "José jose",
      phone: "+123 098098097",
      email: "test2@email.t",
      address: "Country, 1234-345, Street of tests 2",
      specializationId: "test2",
      slots: [],
      status: "teststat2",
    },
  ];

  const listSpec: string[] = ["test1", "test2"];

  beforeEach(async () => {
    localStorage.clear();
     const staffServiceMock = {
      getAllSpecializationsAvailable: jasmine.createSpy('getAllSpecializationsAvailable').and.returnValue(of(listSpec)),
      getStaffByFilters: jasmine.createSpy('getStaffByFilters').and.returnValue(of(listStaff)),
      getStaffById: jasmine.createSpy('getStaffById').and.returnValue(of(listStaff[0])),
      deactivateStaffProfile: jasmine.createSpy('deactivateStaffProfile').and.returnValue(of({})),
      EditStaffProfile: jasmine.createSpy('EditPatientProfile'),
    };

    const activatedRouteMock = {
      snapshot: {
        paramMap: {
          get: jasmine.createSpy('get').and.returnValue(null),
        },
      },
    };

    await TestBed.configureTestingModule({
      imports: [ListStaffProfiles, SideBarAdminComponent, CommonModule, TableModule, FormsModule, MessageComponent],
      providers: [
        { provide: StaffService, useValue: staffServiceMock },
        { provide: ActivatedRoute, useValue: activatedRouteMock } // Provide the mock ActivatedRoute
      ]
    }).compileComponents();

    fixture = TestBed.createComponent(ListStaffProfiles);
    component = fixture.componentInstance;
    staffService = TestBed.inject(StaffService);
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should fetch specializations on initialization', () => {
    expect(staffService.getAllSpecializationsAvailable).toHaveBeenCalled();
    expect(component.specializations).toEqual(listSpec);
  });

  it('should add a new filter', () => {
    const initialLength = component.queryFiltersList.length;
    component.addFilter();
    expect(component.queryFiltersList.length).toBe(initialLength + 1);
  });

  it('should remove a filter', () => {
    component.addFilter(); // Ensure there is at least one removable filter
    const initialLength = component.queryFiltersList.length;
    component.removeFilter(0);
    expect(component.queryFiltersList.length).toBe(initialLength - 1);
  });

  it('should fetch staff when filters are applied', () => {
    component.applyFilters();
    expect(staffService.getStaffByFilters).toHaveBeenCalled();
    expect(component.staffList.length).toBe(2);
  });

  it('should toggle details and fetch staff by id', () => {
    component.toggleDetails(listStaff[0]);
    expect(staffService.getStaffById).toHaveBeenCalledWith(listStaff[0].id);
    expect(component.fullStaff).toEqual(listStaff[0]);
    expect(component.detailsVisible).toBeTrue();
  });

  it('should close details', () => {
    component.detailsVisible = true;
    component.closeDetails();
    expect(component.detailsVisible).toBeFalse();
  });

  it('should display fetched staff in the table', () => {
    component.staffList = listStaff;
    fixture.detectChanges();
    const tableRows = fixture.nativeElement.querySelectorAll('tbody tr');
    expect(tableRows.length).toBe(listStaff.length);
    expect(tableRows[0].textContent).toContain(listStaff[0].name);
    expect(tableRows[1].textContent).toContain(listStaff[1].name);
  });

  it('should call deactivateStaffProfile when deactivating a staff', () => {
    const staffToDeactivate: StaffWithId = {
      id: 'test',
      name: 'José Maria',
      phone: '+123 098098098',
      email: 'test@email.t',
      address: 'Country, 1234-345, Street of tests',
      specializationId: 'test1',
      slots: [],
      status: 'teststat',
    };

    const mockToken = { email: 'admin@admin.com', roles: ['admin'], token: 'valid-token' };

    localStorage.setItem('user', JSON.stringify(mockToken));

    component.storedToken = JSON.stringify(mockToken);

    component.deactivateStaffProfile(staffToDeactivate);

    expect(staffService.deactivateStaffProfile).toHaveBeenCalledWith(staffToDeactivate.id, mockToken.token);
  });


  it('should enable editing when editStaffProfile is called', () => {
    const staffToEdit: StaffWithId = {
      id: 'test',
      name: 'José Maria',
      phone: '+123 098098098',
      email: 'test@email.t',
      address: 'Country, 1234-345, Street of tests',
      specializationId: 'test1',
      slots: [],
      status: 'teststat',
    };

    component.editStaffProfile(staffToEdit);

    expect(component.isEditing).toBeTrue();
  });

  it('should set isSubmitted to true and call service on valid form submission', () => {

    component.staff = {
      id: 'test',
      phone: '+123 098098098',
      email: 'test@email.t',
      address: 'Country, 1234-345, Street of tests',
      specializationId: 'test1',
    };

    const validForm: Partial<NgForm> = { valid: true } as NgForm;

    component.onSubmit(validForm as NgForm);

    expect(component.isSubmitted).toBeTrue();
    expect(staffService.EditStaffProfile).toHaveBeenCalledWith(
      'test',
      '+123 098098098',
      'test@email.t',
      'Country, 1234-345, Street of tests',
      'test1'
    );
  });

  it('should not call service.EditStaffProfile on invalid form submission', () => {
    const invalidForm: Partial<NgForm> = { valid: false } as NgForm;

    component.onSubmit(invalidForm as NgForm);

    expect(component.isSubmitted).toBeFalse();
    expect(staffService.EditStaffProfile).not.toHaveBeenCalled();
  });


  it('should reset isSubmitted and staff form on clearForm', () => {
    component.staff = {
      id: 'test',
      phone: '+123 098098098',
      email: 'test@email.t',
      address: 'Country, 1234-345, Street of tests',
      specializationId: 'test1',
    };

    component.clearForm();

    expect(component.isSubmitted).toBeFalse();
    expect(component.staff).toEqual({
      id: '',
      phone: '',
      email: '',
      address: '',
      specializationId: '',
    });
  });


  it('should not call deactivateStaffProfile if no token is in localStorage',() => {
    const staffToDeactivate: StaffWithId = {
      id: 'test',
      name: 'José Maria',
      phone: '+123 098098098',
      email: 'test@email.t',
      address: 'Country, 1234-345, Street of tests',
      specializationId: 'test1',
      slots: [],
      status: 'teststat',
    };

    localStorage.removeItem('user');

    component.deactivateStaffProfile(staffToDeactivate);

    expect(staffService.deactivateStaffProfile).not.toHaveBeenCalled();
  });
});
