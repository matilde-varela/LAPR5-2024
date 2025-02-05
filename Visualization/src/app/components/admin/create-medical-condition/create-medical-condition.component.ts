import { Component, ViewChild } from '@angular/core';
import { NgForm } from '@angular/forms';
import { SideBarAdminComponent } from "../sidebar-admin/side-bar-admin.component";
import { MessageComponent } from '../../message/message.component';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { MedicalCondition } from 'src/app/domain/MedicalCondition';
import { MedicalConditionService } from 'src/app/services/medical-condition.service';

@Component({
  selector: 'app-create-medical-condition',
  standalone: true,
  imports: [SideBarAdminComponent, MessageComponent, FormsModule, CommonModule],
  templateUrl: './create-medical-condition.component.html',
  styleUrls: ['./create-medical-condition.component.css']
})


export class CreateMedicalConditionComponent {
  @ViewChild('medicalConditionForm') medicalConditionForm!: NgForm;

  isSubmitted = false;
  medicalCondition: MedicalCondition = {
    id: '',
    designation: '',
    description: '',
    symptoms: ''
  };

  constructor(private service: MedicalConditionService) { }

  onSubmit(form: NgForm): void {
    if (form.valid) {
      this.isSubmitted = true;
      this.service.createMedicalCondition(this.medicalCondition.id, this.medicalCondition.designation, this.medicalCondition.description!, this.medicalCondition.symptoms);
    } else {
      this.isSubmitted = false;
    }
  }

  clearForm(): void {
    this.medicalCondition = {
      id: '',
      designation: '',
      description: '',
      symptoms: ''
    };
  
    if (this.medicalConditionForm) {
      this.medicalConditionForm.resetForm();
    }
  
    const inputs = document.querySelectorAll('.input-field input');
    inputs.forEach(input => {
      input.classList.remove('invalid-placeholder');
    });
  }
  

}

