import { Component, ViewChild } from '@angular/core';
import { NgForm } from '@angular/forms';
import { SideBarAdminComponent } from "../sidebar-admin/side-bar-admin.component";
import { MessageComponent } from '../../message/message.component';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { Allergy } from 'src/app/domain/Allergy';
import { AllergyService } from 'src/app/services/allergy.service';

@Component({
  selector: 'app-create-allergy',
  standalone: true,
  imports: [SideBarAdminComponent, MessageComponent, FormsModule, CommonModule],
  templateUrl: './create-allergy.component.html',
  styleUrls: ['./create-allergy.component.css']
})


export class CreateAllergyComponent {
  @ViewChild('allergyForm') allergyForm!: NgForm;

  isSubmitted = false;
  allergy: Allergy = {
    code: '',
    designation: '',
    description: '',
  };

  constructor(private service: AllergyService) { }

  onSubmit(form: NgForm): void {
    if (form.valid) {
      this.isSubmitted = true;
      this.service.createAllergy(this.allergy.code, this.allergy.designation, this.allergy.description!);
    } else {
      this.isSubmitted = false;
    }
  }

  clearForm(): void {
    this.allergy = {
      code: '',
      designation: '',
      description: '',
    };
  
    if (this.allergyForm) {
      this.allergyForm.resetForm();
    }
  
    const inputs = document.querySelectorAll('.input-field input');
    inputs.forEach(input => {
      input.classList.remove('invalid-placeholder');
    });
  }
  

}

