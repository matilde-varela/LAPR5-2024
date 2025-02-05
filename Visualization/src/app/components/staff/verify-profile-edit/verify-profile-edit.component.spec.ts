import { ComponentFixture, TestBed } from '@angular/core/testing';
import { VerifyProfileEditComponent } from './verify-profile-edit.component';
import { ActivatedRoute } from '@angular/router';
import { HttpClientModule } from '@angular/common/http';
import { of, throwError } from 'rxjs';
import { StaffService } from '../../../services/staff.service';

describe('VerifyProfileEditComponent', () => {
  let component: VerifyProfileEditComponent;
  let fixture: ComponentFixture<VerifyProfileEditComponent>;
  let staffServiceMock: jasmine.SpyObj<StaffService>;

  beforeEach(async () => {
    // Mock the ActivatedRoute
    const activatedRouteMock = {
      snapshot: {
        queryParamMap: {
          get: jasmine.createSpy('get').and.callFake((param: string) => {
            if (param === 'userId') return 'user123';
            if (param === 'staffId') return 'staff123';
            if (param === 'token') return 'validToken';
            return null;
          }),
        },
      },
    };

    // Mock StaffService
    staffServiceMock = jasmine.createSpyObj('StaffService', ['confirmEmailStaff']);
    
    // Ensure the method returns an observable using of()
    staffServiceMock.confirmEmailStaff.and.returnValue(of({ message: 'Email confirmed successfully' }));

    await TestBed.configureTestingModule({
      imports: [VerifyProfileEditComponent, HttpClientModule],
      providers: [
        { provide: ActivatedRoute, useValue: activatedRouteMock },
        { provide: StaffService, useValue: staffServiceMock },
      ],
    }).compileComponents();

    fixture = TestBed.createComponent(VerifyProfileEditComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should call confirmEmailStaff with correct parameters when the link is valid', () => {
    const confirmEmailStaffSpy = staffServiceMock.confirmEmailStaff;

    component.ngOnInit();

    expect(confirmEmailStaffSpy).toHaveBeenCalledWith('user123', 'staff123', 'validToken');
    expect(component.message).toBe('Email confirmed successfully');
  });

  it('should handle error if the API call fails', () => {
    // Simulate an API failure by returning a throwError
    staffServiceMock.confirmEmailStaff.and.returnValue(throwError(() => new Error('API error')));

    component.ngOnInit();

    expect(component.message).toBe('The contact information change failed. Please try again.');
  });
});
