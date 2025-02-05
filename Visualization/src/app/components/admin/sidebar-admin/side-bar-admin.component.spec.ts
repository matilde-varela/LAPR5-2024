import { TestBed, ComponentFixture } from '@angular/core/testing';
import { SideBarAdminComponent } from './side-bar-admin.component';
import { RouterTestingModule } from '@angular/router/testing';

describe('SideBarAdminComponent', () => {
  let component: SideBarAdminComponent;
  let fixture: ComponentFixture<SideBarAdminComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [SideBarAdminComponent, RouterTestingModule
      ],
    }).compileComponents();

    fixture = TestBed.createComponent(SideBarAdminComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should include routerLink elements', () => {
    const routerLinkElements = fixture.nativeElement.querySelectorAll('a[routerLink]');
    expect(routerLinkElements.length).toBeGreaterThanOrEqual(0);
  });

  it('should render expected structure', () => {
    const element = fixture.nativeElement;
    expect(element).toBeTruthy();
  });
});
