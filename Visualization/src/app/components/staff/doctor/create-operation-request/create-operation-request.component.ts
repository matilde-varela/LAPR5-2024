import { Component, ViewChild } from '@angular/core';
import { SideBarDoctorComponent } from '../sidebar-doctor/side-bar-doctor.component';
import { MessageComponent } from '../../../message/message.component';
import { FormsModule, NgForm } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { OperationRequestService } from '../../../../services/operation-request.service';

@Component({
  selector: 'app-create-operation-request',
  standalone: true,
  imports: [SideBarDoctorComponent, MessageComponent, FormsModule, CommonModule],
  templateUrl: './create-operation-request.component.html',
  styleUrl: './create-operation-request.component.css'
})
export class CreateOperationRequestComponent {
  @ViewChild('operationRequestForm') operationRequestForm!: NgForm;
  @ViewChild('operationRequestToPatientForm') operationRequestToPatientForm!: NgForm;

  isSubmitted = false;
  operationRequestCreated = false;
  operationRequest = {
    deadLineDate: '',
    priority: '',
    dateOfRequest: '',
    status: '',
    staffId: '',
    description: '',
    patientId: '',
    operationTypeId: ''
  };

  isSubmittedToPatient = false;
  addRemoveFromPatient = {
    patientId: '',
    operationRequestId: ''
  }

  patients: string[] = [];
  staffs: string[] = [];
  operationTypes: string[] = [];
  showAddToPatientForm = false;
  messageService: any;
  opReqId!: string;
  mainTitleCheck = false;

  constructor(private service: OperationRequestService) { }

  ngOnInit(): void {
    this.service.getAllOperationTypes().subscribe({
      next: data => {
        this.operationTypes = data;
      }
    });
  }


  onSubmit(form: NgForm): void {
    this.isSubmitted = true;
    if (form.valid) {
      this.service.createOperationRequest(
        this.operationRequest.deadLineDate,
        this.operationRequest.priority,
        this.operationRequest.dateOfRequest,
        this.operationRequest.status,
        this.operationRequest.staffId,
        this.operationRequest.description,
        this.operationRequest.patientId,
        this.operationRequest.operationTypeId
      ).subscribe({
        next: (id: string) => {
          if (id != null) {
            this.opReqId = id,
              this.operationRequestCreated = true;
          }
        },
        error: (err) => {
          this.isSubmitted = false;
          console.error('Failed to create operation request:', err);
        }
      });
    }
  }

  clearForm(): void {
    this.isSubmitted = false;
    this.operationRequestCreated = false;

    this.operationRequest = {
      deadLineDate: '',
      priority: '',
      dateOfRequest: '',
      status: '',
      staffId: '',
      description: '',
      patientId: '',
      operationTypeId: ''
    };
    if (this.operationRequestForm) {
      this.operationRequestForm.resetForm();
    }
    const inputs = document.querySelectorAll('.input-field input');
    inputs.forEach(input => {
      input.classList.remove('invalid-placeholder');
    });
  }

  addToPatient(form: NgForm): void {
    if (form.valid) {
      this.service.addOperationRequestToPatient(
        this.operationRequest.patientId,
        this.opReqId
      );
      this.isSubmittedToPatient = true;
      this.showAddToPatientForm = false;
      this.mainTitleCheck = true;
      form.resetForm();
      this.clearAddToPatientForm();
    }
  }

  toggleAddToPatientForm(): void {
    this.showAddToPatientForm = !this.showAddToPatientForm;
  }

  clearAddToPatientForm(): void {
    this.addRemoveFromPatient = {
      patientId: '',
      operationRequestId: ''
    };

    this.showAddToPatientForm = false;
    this.isSubmittedToPatient = false;
  }

}