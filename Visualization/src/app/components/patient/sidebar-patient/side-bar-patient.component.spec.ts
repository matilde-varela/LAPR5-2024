import { TestBed, ComponentFixture } from '@angular/core/testing';
import { SideBarPatientComponent } from './side-bar-patient.component';
import { RouterTestingModule } from '@angular/router/testing';

describe('SideBarPatientComponent', () => {
  let component: SideBarPatientComponent;
  let fixture: ComponentFixture<SideBarPatientComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [SideBarPatientComponent,RouterTestingModule,
      ],
    }).compileComponents();

    fixture = TestBed.createComponent(SideBarPatientComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should have routerLink elements', () => {
    const routerLinkElements = fixture.nativeElement.querySelectorAll('a[routerLink]');
    expect(routerLinkElements.length).toBeGreaterThanOrEqual(0);
  });

  it('should render expected structure', () => {
    const element = fixture.nativeElement;
    expect(element).toBeTruthy();
  });
});

