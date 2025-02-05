import { TestBed, ComponentFixture } from '@angular/core/testing';
import { StaffComponent } from './staff.component';
import { RouterTestingModule } from '@angular/router/testing';
import { SideBarStaffComponent } from './sidebar-staff/side-bar-staff.component';

describe('StaffComponent', () => {
  let component: StaffComponent;
  let fixture: ComponentFixture<StaffComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [
        StaffComponent,SideBarStaffComponent,RouterTestingModule
      ],
    }).compileComponents();

    fixture = TestBed.createComponent(StaffComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should include the SideBarStaffComponent', () => {
    const sideBarElement = fixture.nativeElement.querySelector('app-side-bar-staff');
    expect(sideBarElement).toBeTruthy();
  });

  it('should have a RouterLink directive', () => {
    const routerLinkElements = fixture.nativeElement.querySelectorAll('a[routerLink]');
    expect(routerLinkElements.length).toBeGreaterThanOrEqual(0);
  });
});
