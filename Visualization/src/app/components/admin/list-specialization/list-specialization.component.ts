import { Component } from '@angular/core';
import { SideBarAdminComponent } from '../sidebar-admin/side-bar-admin.component';
import { CommonModule } from '@angular/common';
import { TableModule } from 'primeng/table';
import { FormsModule } from '@angular/forms';
import { MessageComponent } from '../../message/message.component';
import { Specialization } from 'src/app/domain/specialization';
import { SpecializationService } from 'src/app/services/specialization.service';

@Component({
  selector: 'app-list-specialization',
  standalone: true,
  imports: [SideBarAdminComponent,CommonModule, TableModule, FormsModule, MessageComponent],
  templateUrl: './list-specialization.component.html',
  styleUrl: './list-specialization.component.css'
})
export class ListSpecializationComponent {
    specializations: Specialization[] = [];
    selectedSpecialization!: Specialization;
    fullSpecialization!: Specialization;
    detailsVisible: boolean = false;
    updateVisible: boolean = false;
  
    filters = {
      code: '',
      denomination : '',
      description : ''
    };
  
    update = {
      code: '',
      denomination : '',
      description : ''
    };
  
    constructor(private service: SpecializationService) {}
  
    ngOnInit(): void {
      this.fetchOperations();
    }
  
    fetchOperations(): void {
      this.service.getSpecializationsByFilters(
        this.filters.code,
        this.filters.denomination,
        this.filters.description,
      ).subscribe({
        next: data => {
          console.log('Fetched Operations:', data);  // Log the fetched data to check its structure
          this.specializations = data;
        }
      });
    }
  
    applyFilters(): void {
      this.fetchOperations();
    }
    
    closeUpdate(): void {
      this.updateVisible = false;
    }
  
    editSpecialization(specialization: Specialization): void {
      this.service.getSpecializationById(specialization.code).subscribe({
        next: (fullRequest: Specialization) => {
          this.fullSpecialization = fullRequest;
          this.updateVisible = true;
        },
        error: (error) => {
          console.error('Error fetching operation request details:', error);
        }
      });
    }
  
    saveUpdateDetails(){
      this.update.code = this.fullSpecialization.code || '';
      this.service.updateSpecialization(this.update.code,this.update.denomination,this.update.description);
    }
  
    deleteSpecialization(specialization: Specialization): void {
      this.service.deleteSpecializationById(specialization.code);
      this.fetchOperations();
    }
  
}
