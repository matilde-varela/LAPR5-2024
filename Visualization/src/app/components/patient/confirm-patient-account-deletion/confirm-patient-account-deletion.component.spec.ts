import { ComponentFixture, TestBed } from '@angular/core/testing';
import { PatientAccountDeletionConfirm } from './confirm-patient-account-deletion.component'; // Use correct component import
import { ActivatedRoute } from '@angular/router';
import { HttpClientModule } from '@angular/common/http';
import { of } from 'rxjs';

describe('PatientAccountDeletionConfirm', () => {
  let component: PatientAccountDeletionConfirm;
  let fixture: ComponentFixture<PatientAccountDeletionConfirm>;

  beforeEach(async () => {
    // Mock the ActivatedRoute
    const activatedRouteMock = {
      snapshot: {
        queryParamMap: {
          get: jasmine.createSpy('get').and.callFake((param: string) => {
            if (param === 'userId') return '123';
            if (param === 'token') return 'abcd-token';
            return null;
          }),
        },
      },
    };

    await TestBed.configureTestingModule({
      imports: [PatientAccountDeletionConfirm, HttpClientModule], // Import correct component
      providers: [
        { provide: ActivatedRoute, useValue: activatedRouteMock },
      ],
    }).compileComponents();

    fixture = TestBed.createComponent(PatientAccountDeletionConfirm);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should send deletion request with correct parameters', () => {
    const httpSpy = spyOn(component['http'], 'put').and.returnValue(of({ message: 'Deletion successful' }));
    component.ngOnInit();
    
    expect(httpSpy).toHaveBeenCalledWith(
      'https://localhost:5001/api/Update-PatientAccountDeletionConfirmation?userId=123&token=abcd-token',
      {}
    );
    expect(component.message).toBe('Deletion successful');
  });
});
