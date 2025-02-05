import { ComponentFixture, TestBed } from '@angular/core/testing';
import { of, throwError } from 'rxjs';
import { FormsModule } from '@angular/forms';
import { TableModule } from 'primeng/table';
import { ListAllergies } from './list-allergies.component';
import { AllergyService } from 'src/app/services/allergy.service';
import { Allergy } from 'src/app/domain/Allergy';
import { CommonModule } from '@angular/common';
import { ActivatedRoute } from '@angular/router';

describe('ListAllergies', () => {
  let component: ListAllergies;
  let fixture: ComponentFixture<ListAllergies>;
  let allergyServiceMock: jasmine.SpyObj<AllergyService>;

  const mockAllergies: Allergy[] = [
    { code: 'A1', designation: 'Pollen', description: 'Pollen allergy' },
    { code: 'A2', designation: 'Dust', description: 'Dust allergy' }
  ];

  beforeEach(async () => {
    allergyServiceMock = jasmine.createSpyObj('AllergyService', [
      'getAllergiesByFilters',
      'getAllergyByCode'
    ]);

    allergyServiceMock = jasmine.createSpyObj('AllergyService', [
      'getAllergiesByFilters',
      'getAllergyByCode'
    ]);

    allergyServiceMock.getAllergiesByFilters.and.returnValue(of([]));
    allergyServiceMock.getAllergyByCode.and.returnValue(of());

    const activatedRouteMock = {
      snapshot: {
        paramMap: {
          get: jasmine.createSpy('get').and.returnValue(null),
        },
      },
    };
    await TestBed.configureTestingModule({
      imports: [CommonModule, TableModule, FormsModule],
      providers: [
        { provide: AllergyService, useValue: allergyServiceMock },
         { provide: ActivatedRoute, useValue: activatedRouteMock }
      ]
    }).compileComponents();

    fixture = TestBed.createComponent(ListAllergies);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should fetch allergies on initialization', () => {
    allergyServiceMock.getAllergiesByFilters.and.returnValue(of(mockAllergies));

    component.ngOnInit();

    expect(allergyServiceMock.getAllergiesByFilters).toHaveBeenCalled();
    expect(component.allergyList).toEqual(mockAllergies);
  });

  it('should handle error when fetching allergies fails', () => {
    const consoleSpy = spyOn(console, 'error');
    allergyServiceMock.getAllergiesByFilters.and.returnValue(throwError(() => new Error('Error fetching allergies')));

    component.fetchAllergies();

    expect(consoleSpy).toHaveBeenCalledWith('Error fetching allergies:', jasmine.any(Error));
    expect(component.allergyList).toEqual([]);
  });

  it('should toggle details visibility and fetch full allergy details', () => {
    const mockAllergy: Allergy = { code: 'A1', designation: 'Pollen', description: 'Pollen allergy' };
    const fullAllergyInfo: Allergy = { code: 'A1', designation: 'Pollen', description: 'Severe pollen allergy' };

    allergyServiceMock.getAllergyByCode.and.returnValue(of(fullAllergyInfo));

    component.toggleDetails(mockAllergy);

    expect(allergyServiceMock.getAllergyByCode).toHaveBeenCalledWith('A1');
    expect(component.fullAllergy).toEqual(fullAllergyInfo);
    expect(component.detailsVisible).toBeTrue();
  });

  it('should handle error when fetching full allergy details fails', () => {
    const consoleSpy = spyOn(console, 'error');
    const mockAllergy: Allergy = { code: 'A1', designation: 'Pollen', description: 'Pollen allergy' };

    allergyServiceMock.getAllergyByCode.and.returnValue(throwError(() => new Error('Error fetching allergy details')));

    component.toggleDetails(mockAllergy);

    expect(consoleSpy).toHaveBeenCalledWith('Error fetching allergy details:', jasmine.any(Error));
    expect(component.detailsVisible).toBeFalse();
  });

  it('should add a filter', () => {
    const initialLength = component.queryFiltersList.length;

    component.addFilter();

    expect(component.queryFiltersList.length).toBe(initialLength + 1);
  });

  it('should remove a filter', () => {
    component.addFilter();
    const initialLength = component.queryFiltersList.length;

    component.removeFilter(0);

    expect(component.queryFiltersList.length).toBe(initialLength - 1);
  });

  it('should clear the form', () => {
    component.allergy = { code: 'Test', designation: 'Test', description: 'Test' };
    component.isSubmitted = true;

    component.clearForm();

    expect(component.allergy).toEqual({ code: '', designation: '', description: '' });
    expect(component.isSubmitted).toBeFalse();
  });
});
