import { TestBed, ComponentFixture } from '@angular/core/testing';
import { ListSpecializationComponent } from './list-specialization.component';
import { RouterTestingModule } from '@angular/router/testing';
import { FormsModule } from '@angular/forms';
import { MessageComponent } from '../../message/message.component';
import { SideBarAdminComponent } from '../sidebar-admin/side-bar-admin.component';
import { SpecializationService } from 'src/app/services/specialization.service';
import { CommonModule } from '@angular/common';
import { TableModule } from 'primeng/table';
import { ActivatedRoute } from '@angular/router';
import { of } from 'rxjs';

describe('ListSpecializationComponent', () => {
  let component: ListSpecializationComponent;
  let fixture: ComponentFixture<ListSpecializationComponent>;
  let specializationService: jasmine.SpyObj<SpecializationService>;

  beforeEach(async () => {
    specializationService = jasmine.createSpyObj('SpecializationService', [
      'getSpecializationsByFilters',
      'getSpecializationById',
      'updateSpecialization',
      'deleteSpecializationById'
    ]);

    specializationService.getSpecializationsByFilters.and.returnValue(of([]));

    const activatedRouteMock = {
      snapshot: {
        paramMap: {
          get: jasmine.createSpy('get').and.returnValue(null),
        },
      },
    };

    await TestBed.configureTestingModule({
      declarations: [],
      imports: [
        CommonModule,
        TableModule,
        FormsModule,
        MessageComponent,
        SideBarAdminComponent,
        RouterTestingModule,
      ],
      providers: [
        { provide: SpecializationService, useValue: specializationService },
        { provide: ActivatedRoute, useValue: activatedRouteMock },
      ],
    }).compileComponents();

    fixture = TestBed.createComponent(ListSpecializationComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should fetch specializations on initialization', () => {
    expect(specializationService.getSpecializationsByFilters).toHaveBeenCalled();
  });

  it('should apply filters and fetch data', () => {
    component.applyFilters();
    expect(specializationService.getSpecializationsByFilters).toHaveBeenCalled();
  });

  it('should close update modal', () => {
    component.updateVisible = true;
    component.closeUpdate();
    expect(component.updateVisible).toBeFalse();
  });

  it('should edit a specialization', () => {
    const mockSpecialization = { code: '123', denomination: '', description: '' } as any;
    specializationService.getSpecializationById.and.returnValue(of(mockSpecialization));

    component.editSpecialization(mockSpecialization);

    expect(specializationService.getSpecializationById).toHaveBeenCalledWith('123');
    expect(component.fullSpecialization).toEqual(mockSpecialization);
    expect(component.updateVisible).toBeTrue();
  });

  it('should save updated specialization details', () => {
    component.fullSpecialization = { code: '123', denomination: '', description: '' } as any;
    component.update = { code: '', denomination: '', description: '' };

    component.saveUpdateDetails();

    expect(specializationService.updateSpecialization).toHaveBeenCalledWith(
      '123',
      '',
      ''
    );
  });
});
