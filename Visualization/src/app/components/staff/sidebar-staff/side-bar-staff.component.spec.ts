import { ComponentFixture, TestBed } from '@angular/core/testing';
import { SideBarStaffComponent } from './side-bar-staff.component';
import { RouterTestingModule } from '@angular/router/testing';

describe('SideBarStaffComponent', () => {
  let component: SideBarStaffComponent;
  let fixture: ComponentFixture<SideBarStaffComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [SideBarStaffComponent, RouterTestingModule],
    }).compileComponents();

    fixture = TestBed.createComponent(SideBarStaffComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should render navigation links', () => {
    const navLinks = fixture.nativeElement.querySelectorAll('a[routerLink]');
    expect(navLinks.length).toBeGreaterThan(0);
  });

  it('should render expected structure', () => {
    const element = fixture.nativeElement;
    expect(element).toBeTruthy();
  });
});
