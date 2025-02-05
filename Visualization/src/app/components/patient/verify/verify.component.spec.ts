import { ComponentFixture, TestBed } from '@angular/core/testing';
import { ActivationComponent } from './verify.component';
import { ActivatedRoute } from '@angular/router';
import { HttpClientTestingModule, HttpTestingController } from '@angular/common/http/testing';
import { MessageComponent } from '../../message/message.component';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';

describe('ActivationComponent', () => {
  let component: ActivationComponent;
  let fixture: ComponentFixture<ActivationComponent>;
  let httpMock: HttpTestingController;
  let activatedRouteMock: any;

  beforeEach(async () => {
    activatedRouteMock = {
      snapshot: {
        queryParamMap: {
          get: jasmine.createSpy('get').and.callFake((param: string) => {
            if (param === 'userId') return 'test-user-id';
            if (param === 'token') return 'test-token';
            return null;
          }),
        },
      },
    };

    await TestBed.configureTestingModule({
      imports: [ActivationComponent, MessageComponent, FormsModule, CommonModule, HttpClientTestingModule],
      providers: [
        { provide: ActivatedRoute, useValue: activatedRouteMock },
      ],
    }).compileComponents();

    fixture = TestBed.createComponent(ActivationComponent);
    component = fixture.componentInstance;
    httpMock = TestBed.inject(HttpTestingController);
    fixture.detectChanges();
  });

  it('should create the component', () => {
    activatedRouteMock.snapshot.queryParamMap.get = jasmine.createSpy('get').and.callFake((param: string) => {
      if (param === 'userId') return 'test-user-id';
      if (param === 'token') return 'test-token';
      return null;
    });

    const req = httpMock.expectOne('https://localhost:5001/api/activate-patient?userId=test-user-id&token=test-token');
    expect(req.request.method).toBe('PUT');
    req.flush({ message: 'Account successfully activated' });

    fixture.detectChanges();

    expect(component.message).toBe('Account successfully activated');

    httpMock.verify();
  });

  it('should send a PUT request with the correct URL parameters', () => {
    const expectedUrl = `https://localhost:5001/api/activate-patient?userId=test-user-id&token=test-token`;

    const req = httpMock.expectOne(expectedUrl);
    expect(req.request.method).toBe('PUT');
    httpMock.verify();
  });

  it('should update message when activation is successful', () => {
    const mockResponse = { message: 'Account successfully activated' };

    const req = httpMock.expectOne(
      'https://localhost:5001/api/activate-patient?userId=test-user-id&token=test-token'
    );
    req.flush(mockResponse);

    fixture.detectChanges();

    expect(component.message).toBe('Account successfully activated');
  });

  it('should update message when activation fails', () => {
    const mockError = { status: 500, statusText: 'Internal Server Error' };

    const req = httpMock.expectOne(
      'https://localhost:5001/api/activate-patient?userId=test-user-id&token=test-token'
    );
    req.flush(null, mockError);

    fixture.detectChanges();

    expect(component.message).toBe('Account activation failed. Please try again.');
  });

});
