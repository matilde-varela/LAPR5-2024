import { TestBed, ComponentFixture } from '@angular/core/testing';
import { PatientComponent } from './patient.component';
import { RouterTestingModule } from '@angular/router/testing';

describe('PatientComponent', () => {
  let component: PatientComponent;
  let fixture: ComponentFixture<PatientComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [PatientComponent, RouterTestingModule,
      ],
    }).compileComponents();

    fixture = TestBed.createComponent(PatientComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should render the SideBarPatientComponent', () => {
    const sidebarElement = fixture.nativeElement.querySelector('app-side-bar-patient');
    expect(sidebarElement).toBeTruthy();
  });

  it('should have a RouterLink directive', () => {
    const routerLinkElements = fixture.nativeElement.querySelectorAll('a[routerLink]');
    expect(routerLinkElements.length).toBeGreaterThanOrEqual(0);
  });
});
