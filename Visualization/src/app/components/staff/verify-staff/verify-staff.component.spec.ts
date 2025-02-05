import { ComponentFixture, TestBed } from '@angular/core/testing';
import { VerifyStaffComponent } from './verify-staff.component';
import { ActivatedRoute } from '@angular/router';
import { HttpClientModule } from '@angular/common/http';
import { of } from 'rxjs';

describe('VerifyStaffComponent', () => {
  let component: VerifyStaffComponent;
  let fixture: ComponentFixture<VerifyStaffComponent>;

  beforeEach(async () => {
    // Mock the ActivatedRoute
    const activatedRouteMock = {
      snapshot: {
        queryParamMap: {
          get: jasmine.createSpy('get').and.returnValue('123'), // Mock userId
        },
      },
    };

    await TestBed.configureTestingModule({
      imports: [VerifyStaffComponent, HttpClientModule],
      providers: [
        { provide: ActivatedRoute, useValue: activatedRouteMock },
      ],
    }).compileComponents();

    fixture = TestBed.createComponent(VerifyStaffComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
