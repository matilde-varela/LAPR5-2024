import { Component, ViewChild } from '@angular/core';
import { OperationRequestService } from '../../../../services/operation-request.service';
import { FormsModule, NgForm } from '@angular/forms';
import { MessageComponent } from '../../../message/message.component';
import { CommonModule } from '@angular/common';
import { SideBarDoctorComponent } from '../sidebar-doctor/side-bar-doctor.component';

@Component({
  selector: 'app-add-operation-request-patient',
  standalone: true,
  imports: [SideBarDoctorComponent, MessageComponent, FormsModule, CommonModule],
  templateUrl: './add-operation-request-patient.component.html',
  styleUrl: './add-operation-request-patient.component.css'
})
export class AddOperationRequestPatientComponent {
  @ViewChild('operationRequestToPatientForm') operationRequestToPatientForm!: NgForm;

  isSubmitted = false;
  addRemoveFromPatient = {
    patientId: '',
    operationRequestId: ''
  }

  constructor(private service: OperationRequestService) { }

  onSubmit(form: NgForm): void {
    this.isSubmitted = true;
    if (form.valid) {
      this.service.addOperationRequestToPatient(
        this.addRemoveFromPatient.patientId,
        this.addRemoveFromPatient.operationRequestId
      );
    } else {
      this.isSubmitted = false;
    }
  }

  clearForm(): void {
    this.isSubmitted = false;

    this.addRemoveFromPatient = {
      patientId: '',
      operationRequestId: ''
    };
    if (this.operationRequestToPatientForm) {
      this.operationRequestToPatientForm.resetForm();
    }
    const inputs = document.querySelectorAll('.input-field input');
    inputs.forEach(input => {
      input.classList.remove('invalid-placeholder');
    });
  }

}
