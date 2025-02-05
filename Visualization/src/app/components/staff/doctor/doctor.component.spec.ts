import { ComponentFixture, TestBed } from '@angular/core/testing';
import { DoctorComponent } from './doctor.component';
import { SideBarDoctorComponent } from './sidebar-doctor/side-bar-doctor.component';
import { RouterTestingModule } from '@angular/router/testing';

describe('DoctorComponent', () => {
  let component: DoctorComponent;
  let fixture: ComponentFixture<DoctorComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [DoctorComponent, SideBarDoctorComponent, RouterTestingModule],
    }).compileComponents();

    fixture = TestBed.createComponent(DoctorComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should render the sidebar component', () => {
    const sidebarElement = fixture.nativeElement.querySelector('app-side-bar-doctor');
    expect(sidebarElement).toBeTruthy();
  });

  it('should render navigation links', () => {
    const navLinks = fixture.nativeElement.querySelectorAll('a[routerLink]');
    expect(navLinks.length).toBeGreaterThan(0);
  });
});
