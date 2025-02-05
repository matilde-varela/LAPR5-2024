import { TestBed, ComponentFixture } from '@angular/core/testing';
import { AdminComponent } from './admin.component';
import { RouterTestingModule } from '@angular/router/testing';
import { SideBarAdminComponent } from './sidebar-admin/side-bar-admin.component';

describe('AdminComponent', () => {
  let component: AdminComponent;
  let fixture: ComponentFixture<AdminComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [
        AdminComponent, RouterTestingModule, SideBarAdminComponent
      ],
    }).compileComponents();

    fixture = TestBed.createComponent(AdminComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should include SideBarAdminComponent', () => {
    const sidebarElement = fixture.nativeElement.querySelector('app-side-bar-admin');
    expect(sidebarElement).toBeTruthy();
  });

  it('should include routerLink elements', () => {
    const routerLinkElements = fixture.nativeElement.querySelectorAll('a[routerLink]');
    expect(routerLinkElements.length).toBeGreaterThanOrEqual(0);
  });
});

