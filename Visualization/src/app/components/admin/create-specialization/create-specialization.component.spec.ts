import { TestBed, ComponentFixture } from '@angular/core/testing';
import { CreateSpecializationComponent } from './create-specialization.component';
import { RouterTestingModule } from '@angular/router/testing';
import { FormsModule, NgForm } from '@angular/forms';
import { MessageComponent } from '../../message/message.component';
import { SideBarAdminComponent } from '../sidebar-admin/side-bar-admin.component';
import { SpecializationService } from 'src/app/services/specialization.service';
import { By } from '@angular/platform-browser';

describe('CreateSpecializationComponent', () => {
  let component: CreateSpecializationComponent;
  let fixture: ComponentFixture<CreateSpecializationComponent>;
  let mockSpecializationService: jasmine.SpyObj<SpecializationService>;

  beforeEach(async () => {
    mockSpecializationService = jasmine.createSpyObj('SpecializationService', ['createSpecialization']);

    await TestBed.configureTestingModule({
      imports: [
        CreateSpecializationComponent,
        RouterTestingModule,
        FormsModule,
        MessageComponent,
        SideBarAdminComponent
      ],
      providers: [
        { provide: SpecializationService, useValue: mockSpecializationService }
      ]
    }).compileComponents();

    fixture = TestBed.createComponent(CreateSpecializationComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should call createSpecialization on valid form submission', () => {
    component.specialization = {
      code: 'TEST01',
      denomination: 'Test Specialization',
      description: 'This is a test description'
    };
    const form = new NgForm([], []);

    component.onSubmit(form);

    expect(component.isSubmitted).toBeTrue();
    expect(mockSpecializationService.createSpecialization).toHaveBeenCalledWith(
      'TEST01',
      'Test Specialization',
      'This is a test description'
    );
  });

  it('should reset the form and fields on clearForm', () => {
    component.specialization = {
      code: 'TEST01',
      denomination: 'Test Specialization',
      description: 'This is a test description'
    };

    const mockResetForm = jasmine.createSpy();
    component.specializationForm = { resetForm: mockResetForm } as any;

    fixture.detectChanges();
    component.clearForm();

    expect(component.specialization).toEqual({
      code: '',
      denomination: '',
      description: '',
    });
    expect(mockResetForm).toHaveBeenCalled();
  });

  it('should remove invalid-placeholder class on clearForm', () => {
    const inputElement = document.createElement('input');
    inputElement.classList.add('invalid-placeholder');
    spyOn(document, 'querySelectorAll').and.returnValue([inputElement] as any);

    component.clearForm();

    expect(inputElement.classList.contains('invalid-placeholder')).toBeFalse();
  });

  it('should correctly handle ViewChild NgForm', () => {
    const formElement = fixture.debugElement.query(By.css('form'));
    expect(formElement).toBeTruthy();
  });
});
