import { ComponentFixture, TestBed } from '@angular/core/testing';
import { ListOperationTypeComponent } from './list-operation-type.component';
import { OperationTypeService } from '../../../services/operation-type.service';
import { of } from 'rxjs';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { TableModule } from 'primeng/table';
import { SideBarAdminComponent } from '../sidebar-admin/side-bar-admin.component';
import { MessageComponent } from '../../message/message.component';
import { ActivatedRoute } from '@angular/router';

describe('ListOperationTypeComponent', () => {
  let component: ListOperationTypeComponent;
  let fixture: ComponentFixture<ListOperationTypeComponent>;
  let service: jasmine.SpyObj<OperationTypeService>;

  beforeEach(async () => {
    // Create a spy for the OperationTypeService
    service = jasmine.createSpyObj('OperationTypeService', [
      'getOperationTypesByFilters',
      'getOperationTypeByName',
      'editOperationType',
      'removeOperationType',
      'fetchOperations'
    ]);

    service.getOperationTypesByFilters.and.returnValue(of([])); // Mocking a successful response with an empty array


    const activatedRouteMock = {
      snapshot: {
        paramMap: {
          get: jasmine.createSpy('get').and.returnValue(null),
        },
      },
    };

    await TestBed.configureTestingModule({
      imports: [ListOperationTypeComponent, FormsModule, CommonModule, TableModule, SideBarAdminComponent, MessageComponent],
      providers: [{ provide: OperationTypeService, useValue: service },
        { provide: ActivatedRoute, useValue: activatedRouteMock }]
    }).compileComponents();

    fixture = TestBed.createComponent(ListOperationTypeComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should call fetchOperations on ngOnInit', () => {
    // Arrange
    const fetchOperationsSpy = spyOn(component, 'fetchOperations').and.callThrough();

    // Act
    component.ngOnInit();

    // Assert
    expect(fetchOperationsSpy).toHaveBeenCalled();
  });

  it('should toggle details visibility and fetch full operation type details', () => {
    // Arrange
    const operationType = { name: 'Surgery' };
    const fullOperationType = { name: 'Surgery', estimatedDuration: 120, status: true, requiredStaff: [{ function: 'Doctor', specialization: 'Surgery', staffQuantity: 1 }], phases: [{ description: 'Surgery', duration: 120 }] };
    service.getOperationTypeByName.and.returnValue(of(fullOperationType));

    // Act
    component.toggleDetails(fullOperationType);

    // Assert
    expect(service.getOperationTypeByName).toHaveBeenCalledWith('Surgery');
    expect(component.fullOperationType).toEqual(fullOperationType);
    expect(component.detailsVisible).toBeTrue();
  });

  it('should edit operation type on valid form submission', (done) => {
    // Arrange
    component.selectedOperationType = { name: 'Surgery', estimatedDuration: 120, status: true, requiredStaff: [], phases: [] };
    component.newOperationType = {
      name: 'Surgery Updated',
      status: true,
      estimatedDuration: 150,
      requiredStaff: [],
      phases: []
    };

    const validForm = { valid: true } as any;
    const editResponse = { id: '123' };

    service.getOperationTypeByName.and.returnValue(of(editResponse));
    service.editOperationType.and.returnValue(of(editResponse));

    // Act
    component.editOperationType(validForm);

    // Wait for the asynchronous code to complete
    fixture.detectChanges();
    setTimeout(() => {
      // Assert
      expect(service.getOperationTypeByName).toHaveBeenCalledWith('Surgery');
      expect(service.editOperationType).toHaveBeenCalledWith('123', component.newOperationType);
      expect(component.isSubmitted).toBeFalse();
      done();
    }, 0);  // Use done() to handle async completion
  });



  it('should not edit operation type if form is invalid', () => {
    // Arrange
    const invalidForm = { valid: false } as any;

    // Act
    component.editOperationType(invalidForm);

    // Assert
    expect(service.editOperationType).not.toHaveBeenCalled();
    expect(component.isSubmitted).toBeFalse();
  });

  it('should get operation type but not edit operation type if form is invalid', () => {
    // Arrange
    const invalidForm = { valid: false } as any;
    const editResponse = { id: '123' };
    service.getOperationTypeByName.and.returnValue(of(editResponse));

    // Act
    component.editOperationType(invalidForm);

    // Assert
    expect(service.editOperationType).not.toHaveBeenCalled();
    expect(component.isSubmitted).toBeFalse();
  });

  it('should delete operation type and update the list', () => {
    // Arrange
    const operationType = { name: 'Surgery', estimatedDuration: 120, status: true, requiredStaff: [{ function: 'Doctor', specialization: 'Surgery', staffQuantity: 1 }], phases: [{ description: 'Surgery', duration: 120 }] };
    const fetchOperationsSpy = spyOn(component, 'fetchOperations').and.callThrough();

    // Act
    component.deleteOperationType(operationType);

    // Assert
    expect(service.removeOperationType).toHaveBeenCalledWith('Surgery');
    expect(fetchOperationsSpy).toHaveBeenCalled();
  });

  it('should clear the form and reset the newOperationType object', () => {
    // Arrange
    component.newOperationType = {
      name: 'Surgery',
      status: true,
      estimatedDuration: 120,
      requiredStaff: [{ function: 'Doctor', specialization: 'Surgery', staffQuantity: 1 }],
      phases: [{ description: 'Surgery', duration: 120 }]
    };
    component.isSubmitted = true;

    const formSpy = jasmine.createSpyObj('NgForm', ['resetForm']);
    component.newOperationTypeForm = formSpy;

    // Act
    component.clearForm();

    // Assert
    expect(component.isSubmitted).toBeFalse();
    expect(component.newOperationType.name).toBe('');
    expect(formSpy.resetForm).toHaveBeenCalled();
  });

  it('should apply filters and fetch operations', () => {
    // Arrange
    component.filters = { name: 'Surgery', specialization: 'Orthopaedics', status: 'active' };
    const fetchOperationsSpy = spyOn(component, 'fetchOperations').and.callThrough();

    // Act
    component.applyFilters();

    // Assert
    expect(fetchOperationsSpy).toHaveBeenCalled();
  });
});
