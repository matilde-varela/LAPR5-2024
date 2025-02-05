import { ComponentFixture, TestBed } from '@angular/core/testing';

import { LoginCallbackComponent } from './login-callback.component';
import { HttpClientTestingModule } from '@angular/common/http/testing';

describe('LoginCallbackComponent', () => {
  let component: LoginCallbackComponent;
  let fixture: ComponentFixture<LoginCallbackComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [LoginCallbackComponent, HttpClientTestingModule]
    })
    .compileComponents();

    fixture = TestBed.createComponent(LoginCallbackComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
