import { ComponentFixture, TestBed } from '@angular/core/testing';
import { ExportMedicalRecordComponent } from './export-medical-record.component';
import { DownloadService } from 'src/app/services/download.service';
import { FormsModule, NgForm } from '@angular/forms';
import { of } from 'rxjs';
import { SideBarPatientComponent } from "../sidebar-patient/side-bar-patient.component";
import { MessageComponent } from "../../message/message.component";
import { ActivatedRoute } from '@angular/router';

describe('ExportMedicalRecordComponent', () => {
  let component: ExportMedicalRecordComponent;
  let fixture: ComponentFixture<ExportMedicalRecordComponent>;
  let downloadServiceMock: DownloadService;

  beforeEach(async () => {
    downloadServiceMock = jasmine.createSpyObj('DownloadService', ['downloadMedicalRecord']);
    const activatedRouteMock = {
        snapshot: {
          paramMap: {
            get: jasmine.createSpy('get').and.returnValue(null),
          },
        },
      };
      
    await TestBed.configureTestingModule({
      imports: [SideBarPatientComponent, MessageComponent, FormsModule],
      providers: [
        { provide: DownloadService, useValue: downloadServiceMock },
        { provide: ActivatedRoute, useValue: activatedRouteMock }
        
      ]
    }).compileComponents();

    fixture = TestBed.createComponent(ExportMedicalRecordComponent);
    component = fixture.componentInstance;
  });

  it('should create the component', () => {
    expect(component).toBeTruthy();
  });

  it('should call downloadMedicalRecord when the form is valid', () => {
    component.download = { filePath: 'path/to/record', password: 'password123' };
    
    const form = {
      valid: true
    } as NgForm;

    component.onSubmit(form);

    expect(downloadServiceMock.downloadMedicalRecord).toHaveBeenCalledWith('path/to/record', 'password123');
  });

  it('should not call downloadMedicalRecord when the form is invalid', () => {
    const form = {
      valid: false
    } as NgForm;

    component.onSubmit(form);

    expect(downloadServiceMock.downloadMedicalRecord).not.toHaveBeenCalled();
  });

  it('should clear the form when clearForm is called', () => {
    component.download = { filePath: 'path/to/record', password: 'password123' };
    component.isSubmitted = true;
  
    const resetFormSpy = jasmine.createSpy('resetForm');
    component.downloadForm = { resetForm: resetFormSpy } as any; 

    component.clearForm();
  
    expect(component.download.filePath).toBe('');
    expect(component.download.password).toBe('');
    expect(component.isSubmitted).toBeFalse();

    expect(resetFormSpy).toHaveBeenCalled();
  });
});
