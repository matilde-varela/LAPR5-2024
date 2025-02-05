import { Component, OnInit } from '@angular/core';
import { SideBarDoctorComponent } from '../sidebar-doctor/side-bar-doctor.component';
import { CommonModule } from '@angular/common';
import { TableModule } from 'primeng/table';
import { ListOperationRequest } from '../../../../domain/list-operation-request';
import { OperationRequestService } from '../../../../services/operation-request.service';
import { FormsModule } from '@angular/forms';
import { OperationRequest } from '../../../../domain/OperationRequest';
import { MessageComponent } from '../../../message/message.component';

interface UpdateOperationRequest {
  id : string,
  priority : string,
  description : string,
  deadlineDate : string
}


@Component({
  selector: 'app-list-operation-request',
  standalone: true,
  imports: [SideBarDoctorComponent, CommonModule, TableModule, FormsModule, MessageComponent],
  templateUrl: './list-operation-request.component.html',
  styleUrl: './list-operation-request.component.css'
})

export class ListOperationRequestComponent implements OnInit {
  operationRequests: ListOperationRequest[] = [];
  selectedOperationRequest!: ListOperationRequest;
  fullOperationRequest!: OperationRequest;
  detailsVisible: boolean = false;
  updateVisible: boolean = false;

  filters = {
    name: '',
    priority: '',
    operationType: '',
    status: '',
    dateOfRequest: '',
    deadlineDate: ''
  };

  update : UpdateOperationRequest = {
    id : '',
    priority: '',
    description: '',
    deadlineDate: ''
  };

  constructor(private service: OperationRequestService) {}

  ngOnInit(): void {
    this.fetchOperations();
  }

  fetchOperations(): void {
    this.service.getOperationRequestsByFilters(
      this.filters.name,
      this.filters.priority,
      this.filters.operationType,
      this.filters.status,
      this.filters.dateOfRequest,
      this.filters.deadlineDate
    ).subscribe({
      next: data => {
        console.log('Fetched Operations:', data);  // Log the fetched data to check its structure
        this.operationRequests = data;
      }
    });
  }

  applyFilters(): void {
    this.fetchOperations();
  }

  toggleDetails(operationRequest: ListOperationRequest): void {
    this.service.getOperationRequestById(operationRequest.id).subscribe({
      next: (fullRequest: OperationRequest) => {
        this.fullOperationRequest = fullRequest;
        this.detailsVisible = true; // Show the details section after fetching the data
      },
      error: (error) => {
        console.error('Error fetching operation request details:', error);
      }
    });
  }

  closeDetails(): void {
    // Hide the details section
    this.detailsVisible = false;
  }

  closeUpdate(): void {
    this.updateVisible = false;
  }

  editOperationRequest(operationRequest: ListOperationRequest): void {
    this.service.getOperationRequestById(operationRequest.id).subscribe({
      next: (fullRequest: OperationRequest) => {
        this.fullOperationRequest = fullRequest;
        this.updateVisible = true;
      },
      error: (error) => {
        console.error('Error fetching operation request details:', error);
      }
    });
  }

  saveUpdateDetails(){
    this.update.id = this.fullOperationRequest.id || '';
    console.log("update ID:" + this.update.id);
    this.service.updateOperationRequest(this.update);
  }

  deleteOperationRequest(operationRequest: ListOperationRequest): void {
    console.log('Delete button clicked:', operationRequest);
    this.service.deleteOperationRequestById(operationRequest.id);
    this.fetchOperations();
  }

}
