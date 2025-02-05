import { ComponentFixture, TestBed } from '@angular/core/testing';
import { CreateOperationTypeComponent } from './create-operation-type.component';
import { SideBarAdminComponent } from '../sidebar-admin/side-bar-admin.component';
import { MessageComponent } from '../../message/message.component';
import { FormsModule, NgForm } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { OperationTypeService } from 'src/app/services/operation-type.service';
import { of } from 'rxjs';
import { ActivatedRoute } from '@angular/router';

describe('CreateOperationTypeComponent', () => {
  let component: CreateOperationTypeComponent;
  let fixture: ComponentFixture<CreateOperationTypeComponent>;
  let operationTypeService: OperationTypeService;

  const listSpec: string[] = ["test1", "test2"];
  beforeEach(async () => {
    const operationTypeServiceMock = {
      getAllSpecializationsAvailable: jasmine.createSpy('getAllSpecializationsAvailable').and.returnValue(of(listSpec)),
      createOperationType: jasmine.createSpy('createOperationType')
    }

    const activatedRouteMock = {
      snapshot: {
        paramMap: {
          get: jasmine.createSpy('get').and.returnValue(null),
        },
      },
    };

    await TestBed.configureTestingModule({
      imports: [CreateOperationTypeComponent, SideBarAdminComponent, MessageComponent, FormsModule, CommonModule],
      providers: [
        { provide: OperationTypeService, useValue: operationTypeServiceMock },
        { provide: ActivatedRoute, useValue: activatedRouteMock }
      ]
    }).compileComponents();

    fixture = TestBed.createComponent(CreateOperationTypeComponent);
    component = fixture.componentInstance;
    operationTypeService = TestBed.inject(OperationTypeService);
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should initialize with fetched specializations', () => {
    expect(operationTypeService.getAllSpecializationsAvailable).toHaveBeenCalled();
    expect(component.specializations).toEqual(listSpec);
  });

  it('should add a new staff entry', () => {
    component.addStaff();
    expect(component.operationType.requiredStaff.length).toBe(1);
    expect(component.operationType.requiredStaff[0]).toEqual({
      function: '',
      specialization: '',
      staffQuantity: 1
    });
  });

  it('should remove a staff entry', () => {
    component.addStaff();
    component.addStaff();
    expect(component.operationType.requiredStaff.length).toBe(2);
    component.removeStaff(0);
    expect(component.operationType.requiredStaff.length).toBe(1);
  });

  it('should calculate the total duration correctly', () => {
    component.operationType.phases = [
      { description: 'Phase 1', duration: 10 },
      { description: 'Phase 2', duration: 20 },
      { description: 'Phase 3', duration: 30 }
    ];
    const totalDuration = component.calculateTotalDuration();
    expect(totalDuration).toBe(60);
    expect(component.operationType.estimatedDuration).toBe(60);
  });

  it('should clear the form correctly', () => {
    component.operationType.name = 'test-op';
    component.operationType.requiredStaff = [{ function: 'test-function', specialization: 'test1', staffQuantity: 2 }];
    component.operationType.phases[0].duration = 10;
    component.isSubmitted = true;

    component.clearForm();

    expect(component.operationType.name).toBeNull();
    expect(component.operationType.requiredStaff.length).toBe(0);
    expect(component.operationType.phases[0].duration).toBe(0);
    expect(component.isSubmitted).toBe(false);
  });

  it('should submit the form when valid', () => {
    component.operationType.name = 'test-op';
    component.operationType.status = true;
    component.operationType.requiredStaff = [{ function: 'test-function', specialization: 'test1', staffQuantity: 1 }];
    component.operationType.phases[0].duration = 20;

    const formMock = {
      valid: true,
    }  as NgForm;

    component.onSubmit(formMock);
    expect(component.isSubmitted).toBe(true);
    expect(operationTypeService.createOperationType).toHaveBeenCalledWith(component.operationType);
  });

  it('should not submit the form when invalid', () => {
    const formMock = {
      valid: false,
    } as NgForm;

    component.onSubmit(formMock);
    expect(component.isSubmitted).toBe(false);
    expect(operationTypeService.createOperationType).not.toHaveBeenCalled();
  });
});
