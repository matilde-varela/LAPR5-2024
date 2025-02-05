import { ComponentFixture, TestBed } from '@angular/core/testing';
import { PatientPreferences } from '../patient-preferences/patient-preferences.component';
import { UserService } from '../../../services/user.service';
import { MessageComponent } from '../../message/message.component';
import { CommonModule } from '@angular/common';
import { SideBarPatientComponent } from '../sidebar-patient/side-bar-patient.component';
import { ActivatedRoute } from '@angular/router';
import { of } from 'rxjs';
import { PrivacyPolicyComponent } from '../../privacy-policy/privacy-policy.component';
import { PatientService } from 'src/app/services/patient.service';

describe('PatientPreferences', () => {
  let component: PatientPreferences;
  let fixture: ComponentFixture<PatientPreferences>;
  let userService: UserService;

  beforeEach(async () => {
    const userServiceMock = {
      sendAccountDeleteRequest: jasmine.createSpy('sendAccountDeleteRequest').and.returnValue(of(null)),
    };
    const patientServiceMock = jasmine.createSpyObj('PatientService', ['getPrivacyPolicyText']);
    const activatedRouteMock = {
      snapshot: {
        paramMap: {
          get: jasmine.createSpy('get').and.returnValue(null),
        },
      },
    };

    await TestBed.configureTestingModule({
      imports: [CommonModule, MessageComponent, SideBarPatientComponent,PrivacyPolicyComponent],
      providers: [
        { provide: UserService, useValue: userServiceMock },
        { provide: PatientService, useValue: patientServiceMock },
        { provide: ActivatedRoute, useValue: activatedRouteMock }, // Provide the mock ActivatedRoute
      ],
    }).compileComponents();

    fixture = TestBed.createComponent(PatientPreferences);
    component = fixture.componentInstance;
    userService = TestBed.inject(UserService); // Inject the mocked service
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should call sendAccountDeleteRequest when confirmDelete is called', () => {
    component.confirmDelete();
    expect(userService.sendAccountDeleteRequest).toHaveBeenCalled();
  });
});
