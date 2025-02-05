import { Component, ViewChild } from '@angular/core';
import { NgForm } from '@angular/forms';
import { SideBarAdminComponent } from "../sidebar-admin/side-bar-admin.component";
import { OperationTypeService } from '../../../services/operation-type.service';
import { MessageComponent } from '../../message/message.component';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { OperationType } from '../../../domain/OperationType';

@Component({
  selector: 'app-create-operation-type',
  standalone: true,
  imports: [SideBarAdminComponent, MessageComponent, FormsModule, CommonModule],
  templateUrl: './create-operation-type.component.html',
  styleUrls: ['./create-operation-type.component.css']
})


export class CreateOperationTypeComponent {
  @ViewChild('operationTypeForm') operationTypeForm!: NgForm;

  functions: string[] = [`Doctor`, `Intern`,`Nurse`, `Assistant`];
  specializations: string[] = [];

  isSubmitted = false;
  operationType: OperationType = {
    name: '',
    status: true,
    estimatedDuration: 0,
    requiredStaff: [],
    phases: [
      { description: 'Anesthesia/Patient Preparation', duration: 0 },
      { description: 'Surgery', duration: 0 },
      { description: 'Cleaning', duration: 0 }
    ]
  };

  constructor(private service: OperationTypeService) { }

  ngOnInit(): void {  
    this.service.getAllSpecializationsAvailable().subscribe({
      next: data => {
        this.specializations = data;
      }
    });
  }


  onSubmit(form: NgForm): void {
    if (form.valid) {
      this.isSubmitted = true;
      this.service.createOperationType(this.operationType);
    } else {
      this.isSubmitted = false;
    }
  }

  clearForm(): void {

    this.operationType = {
      name: '',
      status: true,
      estimatedDuration: 0,
      requiredStaff: [],
      phases: [
        { description: 'Anesthesia/Patient Preparation', duration: 0 },
        { description: 'Surgery', duration: 0 },
        { description: 'Cleaning', duration: 0 }
      ]
    };
    this.isSubmitted = false;

    if (this.operationTypeForm) {
      this.operationTypeForm.resetForm();
    }

    const inputs = document.querySelectorAll('.input-field input');
    inputs.forEach(input => {
      input.classList.remove('invalid-placeholder');
    });
  }

  addStaff() {
    this.operationType.requiredStaff.push({
      function: '',
      specialization: '',
      staffQuantity: 1
    });
  }

  removeStaff(index: number) {
    this.operationType.requiredStaff.splice(index, 1);
  }

  calculateTotalDuration(): number {
    var value = this.operationType.phases.reduce((total, phase) => total + phase.duration, 0);
    this.operationType.estimatedDuration = value;
    return value;
  }


}

