import { Component, ViewChild } from '@angular/core';
import { NgForm } from '@angular/forms';
import { SideBarAdminComponent } from '../sidebar-admin/side-bar-admin.component';
import { MessageComponent } from '../../message/message.component';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { AllergyService } from 'src/app/services/allergy.service';
import { Allergy } from 'src/app/domain/Allergy';
import { TableModule } from 'primeng/table';

@Component({
  selector: 'edit-allergy',
  standalone: true,
  imports: [SideBarAdminComponent, MessageComponent, FormsModule, TableModule, CommonModule],
  templateUrl: './edit-allergy.component.html',
  styleUrl: './edit-allergy.component.css'
})
export class EditAllergyComponent {
  @ViewChild('allergyForm') allergyForm!: NgForm;

  isSubmitted = false;
  allergy = {
    code : '',
    designation : '',
    description: ''
  };

  allergies : Allergy[] = [];
  fullAllergy!: Allergy;
  selectedAllergy!: Allergy;
  detailsVisible: boolean = false;
  isEditing = false;
  editDetails: boolean = false;

  constructor(private service: AllergyService) { }

  ngOnInit(): void {  
    this.service.getAllAllergies().subscribe({
      next: data => {
        this.allergies = data;
      }
    });
  }

  onSubmit(form: NgForm): void {
    this.allergy = { code : this.fullAllergy.code, designation : this.fullAllergy.designation, description : this.fullAllergy.description || '' };

    this.isSubmitted = true;
    if (form.valid) {
      this.service.EditAllergy(
        this.allergy.code,
        this.allergy.designation,
        this.allergy.description || ''
      );
    } else {
      this.isSubmitted = false;
    }
  }

  clearForm(): void {
    this.isSubmitted = false;

    this.allergy = {
      code : '',
      designation : '',
      description: ''
    };
    if (this.allergyForm) {
      this.allergyForm.resetForm();
    }
  }


  toggleEdition(allergy: Allergy): void {
    this.fullAllergy = { ...allergy };
    this.clearForm()
    this.editDetails = true;
  }
  

  closeEdition(): void {
    this.editDetails = false;
  }
  
}
