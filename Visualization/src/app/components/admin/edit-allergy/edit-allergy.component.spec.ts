import { ComponentFixture, TestBed } from '@angular/core/testing';
import { EditAllergyComponent } from './edit-allergy.component';
import { AllergyService } from 'src/app/services/allergy.service';
import { of } from 'rxjs';
import { NgForm } from '@angular/forms';
import { By } from '@angular/platform-browser';
import { ActivatedRoute } from '@angular/router';

describe('EditAllergyComponent', () => {
  let component: EditAllergyComponent;
  let fixture: ComponentFixture<EditAllergyComponent>;
  let allergyServiceSpy: jasmine.SpyObj<AllergyService>;

  beforeEach(async () => {
    const allergyServiceMock = jasmine.createSpyObj('AllergyService', ['getAllAllergies', 'EditAllergy']);
    const activatedRouteMock = {
      snapshot: {
        paramMap: {
          get: jasmine.createSpy('get').and.returnValue(null),
        },
      },
    };

    await TestBed.configureTestingModule({
      imports: [EditAllergyComponent],
      providers: [
        { provide: AllergyService, useValue: allergyServiceMock },
        { provide: ActivatedRoute, useValue: activatedRouteMock }

      ]
    }).compileComponents();

    allergyServiceSpy = TestBed.inject(AllergyService) as jasmine.SpyObj<AllergyService>;
    allergyServiceSpy.getAllAllergies.and.returnValue(of([
      {
        code: 'BZ04', designation: 'Peanuts', description: 'Irritated skin with rashes and blood.'
      },
      {
        code: 'BZ05.3', designation: 'Pollen Allergy', description: 'Mucus.'
      }
    ]));
    fixture = TestBed.createComponent(EditAllergyComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();

  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should initialize allergies on ngOnInit', () => {
    component.ngOnInit();
    expect(allergyServiceSpy.getAllAllergies).toHaveBeenCalled();
    expect(component.allergies.length).toBe(2);
    expect(component.allergies[0].code).toBe('BZ04');
    expect(component.allergies[1].code).toBe('BZ05.3');
  });

  it('should call service.EditAllergy on valid form submission', () => {
    const form = {
      valid: true,
      resetForm: jasmine.createSpy('resetForm')
    } as unknown as NgForm;

    component.fullAllergy = { code: 'A002', designation: 'Milk', description: 'Milk allergy' };
    component.onSubmit(form);

    expect(component.isSubmitted).toBeTrue();
    expect(allergyServiceSpy.EditAllergy).toHaveBeenCalledWith('A002', 'Milk', 'Milk allergy');
  });

  it('should not call service.EditAllergy on invalid form submission', () => {
    const form = {
      valid: false,
      resetForm: jasmine.createSpy('resetForm')
    } as unknown as NgForm;

    component.fullAllergy = { code: '', designation: '', description: '' };

    component.onSubmit(form);

    expect(component.isSubmitted).toBeFalse();
    expect(allergyServiceSpy.EditAllergy).not.toHaveBeenCalled();
  });


  it('should clear form when clearForm is called', () => {
    component.allergy = { code: 'A002', designation: 'Milk', description: 'Milk allergy' };
    const form = jasmine.createSpyObj('NgForm', ['resetForm']);
    component.allergyForm = form as unknown as NgForm;

    component.clearForm();

    expect(component.allergy.code).toBe('');
    expect(component.allergy.designation).toBe('');
    expect(component.allergy.description).toBe('');
    expect(form.resetForm).toHaveBeenCalled();
  });

  it('should set editDetails to true and populate fullAllergy on toggleEdition', () => {
    const mockAllergy = { code: 'A003', designation: 'Eggs', description: 'Egg allergy' };
    component.toggleEdition(mockAllergy);

    expect(component.fullAllergy).toEqual(mockAllergy);
    expect(component.editDetails).toBeTrue();
  });

  it('should set editDetails to false on closeEdition', () => {
    component.editDetails = true;
    component.closeEdition();
    expect(component.editDetails).toBeFalse();
  });
});
