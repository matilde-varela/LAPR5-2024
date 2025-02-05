import { Component, ViewChild } from '@angular/core';
import { SideBarPatientComponent } from "../sidebar-patient/side-bar-patient.component";
import { MessageComponent } from "../../message/message.component";
import { FormsModule, NgForm } from '@angular/forms';
import { DownloadService } from 'src/app/services/download.service';
import { CommonModule } from '@angular/common';

@Component({
  standalone: true,
  selector: 'app-export-medical-record',
  templateUrl: './export-medical-record.component.html',
  styleUrl: './export-medical-record.component.css',
  imports: [SideBarPatientComponent, MessageComponent, FormsModule, CommonModule]
})

export class ExportMedicalRecordComponent {
  @ViewChild('downloadForm') downloadForm!: NgForm;

  isSubmitted = false;
  download = {
    filePath: '',
    password: ''
  };

  typeDesignations : string[] = [];
  
  constructor(private service: DownloadService) { }

  onSubmit(form: NgForm): void {
    this.isSubmitted = true;
    if (form.valid) {
      this.service.downloadMedicalRecord(
        this.download.filePath,
        this.download.password
      );
    } else {
      this.isSubmitted = false;
    }
  }

  clearForm(): void {
    this.isSubmitted = false;

    this.download = {
      filePath: '',
      password: ''
    };
    if (this.downloadForm) {
      this.downloadForm.resetForm();
    }
    const inputs = document.querySelectorAll('.input-field input');
    inputs.forEach(input => {
      input.classList.remove('invalid-placeholder');
    });
  }
}

