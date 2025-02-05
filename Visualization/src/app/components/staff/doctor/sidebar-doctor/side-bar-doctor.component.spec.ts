import { ComponentFixture, TestBed } from '@angular/core/testing';
import { SideBarDoctorComponent } from './side-bar-doctor.component';
import { RouterTestingModule } from '@angular/router/testing';

describe('SideBarDoctorComponent', () => {
  let component: SideBarDoctorComponent;
  let fixture: ComponentFixture<SideBarDoctorComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [SideBarDoctorComponent, RouterTestingModule],
    }).compileComponents();

    fixture = TestBed.createComponent(SideBarDoctorComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should render sidebar links', () => {
    const sidebarLinks = fixture.nativeElement.querySelectorAll('a[routerLink]');
    expect(sidebarLinks.length).toBeGreaterThan(0);
  });

  it('should render expected structure', () => {
    const element = fixture.nativeElement;
    expect(element).toBeTruthy();
  });
});

