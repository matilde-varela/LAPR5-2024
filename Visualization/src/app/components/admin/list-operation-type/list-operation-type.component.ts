import { Component, OnInit, ViewChild } from '@angular/core';
import { SideBarAdminComponent } from "../sidebar-admin/side-bar-admin.component";
import { CommonModule } from '@angular/common';
import { TableModule } from 'primeng/table';
import { FormsModule, NgForm } from '@angular/forms';
import { MessageComponent } from '../../message/message.component';
import { ListOperationType } from '../../../domain/list-operation-type';
import { OperationType } from '../../../domain/OperationType';
import { OperationTypeService } from '../../../services/operation-type.service';
import { catchError, of, switchMap } from 'rxjs';

@Component({
  selector: 'app-list-operation-type',
  standalone: true,
  imports: [SideBarAdminComponent, CommonModule, TableModule, FormsModule, MessageComponent],
  templateUrl: './list-operation-type.component.html',
  styleUrls: ['./list-operation-type.component.css']
})
export class ListOperationTypeComponent implements OnInit {
  @ViewChild('newOperationTypeForm') newOperationTypeForm!: NgForm;
  operationTypes: ListOperationType[] = [];
  selectedOperationType!: ListOperationType;
  fullOperationType!: OperationType;
  detailsVisible: boolean = false;
  editDetails: boolean = false;
  isSubmitted: boolean = false;

  getId: string = '';

  newOperationType: OperationType = {
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

  filters = {
    name: '',
    specialization: '',
    status: ''
  };

  operationType: OperationType = {
    name: '',
    estimatedDuration: 0,
    status: false,
    requiredStaff: [],
    phases: []
  };

  functions: string[] = ['Doctor', 'Nurse', 'Technician']; // Example functions
  specializations: string[] = ['Cardiology', 'Orthopaedics', 'General Surgery']; // Example specializations

  constructor(private service: OperationTypeService) { }

  ngOnInit(): void {
    this.fetchOperations();
  }

  fetchOperations(): void {
    const queryFilters = [
      {
        name: this.filters.name || '',
        specialization: this.filters.specialization || '',
        status: this.filters.status || ''
      }
    ];

    // Fazer a requisição para o endpoint do backend
    this.service.getOperationTypesByFilters({ queryFilters }).subscribe({
      next: data => {
        this.operationTypes = data;
      },
      error: err => {
        console.error('Error fetching operation types:', err);
        this.operationTypes = [];
      }
    });
  }

  applyFilters(): void {
    this.fetchOperations();
  }

  toggleDetails(operationType: ListOperationType): void {
    this.service.getOperationTypeByName(operationType.name).subscribe({
      next: (fullRequest: OperationType) => {
        this.fullOperationType = fullRequest;
        this.detailsVisible = true;
      },
      error: error => {
        console.error('Error fetching operation type details:', error);
      }
    });
  }

  closeDetails(): void {
    this.detailsVisible = false;
  }

  editOperationType(form: NgForm): void {
    if (form.valid && this.selectedOperationType) {
      console.log("It reached here : Start editMethod at component.");
      this.isSubmitted = true; // Mark form as submitted initially
      console.log(this.newOperationType);
      this.service
        .getOperationTypeByName(this.selectedOperationType.name)
        .pipe(
          switchMap((data) => {
            if (data?.id) {
              console.log("It reached here : Middle editMethod at component.");
              return this.service.editOperationType(data.id, this.newOperationType);
            } else {
              throw new Error('Operation type not found'); // Handle case where no data is returned
            }
          }),
          catchError((err) => {
            console.error('Error during operation type edit process:', err);
            this.isSubmitted = false;
            return of(null); // Allow graceful fallback on error
          })
        )
        .subscribe((result) => {
          if (result) {
            this.isSubmitted = false;
            this.editDetails = false; // Close edit details
            this.fetchOperations(); // Refresh the list
          }
        });
    }
  }  

  toggleEdition(operationType: ListOperationType): void {
    this.editDetails = true;
  }

  closeEdition(): void {
    this.editDetails = false;
  }

  deleteOperationType(operationType: ListOperationType): void {
    this.service.removeOperationType(operationType.name);
    this.fetchOperations(); // to update the list of operation types
  }

  addStaff(): void {
    this.newOperationType.requiredStaff.push({ function: '', specialization: '', staffQuantity: 1 });
  }

  removeStaff(index: number): void {
    this.newOperationType.requiredStaff.splice(index, 1);
  }

  calculateTotalDuration(): number {
    var value = this.newOperationType.phases.reduce((total, phase) => total + phase.duration, 0);
    this.newOperationType.estimatedDuration = value;
    return value;
  }

  clearForm(): void {

    this.isSubmitted = false;

    if (this.newOperationTypeForm) {
      this.newOperationTypeForm.resetForm();
    }

    this.newOperationType = {
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

    const inputs = document.querySelectorAll('.input-field input');
    inputs.forEach(input => {
      input.classList.remove('invalid-placeholder');
    });
  }

}
