import { Component, ViewChild } from '@angular/core';
import { NgForm } from '@angular/forms';
import { SideBarAdminComponent } from "../sidebar-admin/side-bar-admin.component";
import { MessageComponent } from '../../message/message.component';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { RoomService } from 'src/app/services/room.service';

@Component({
  selector: 'app-create-staff-profile',
  standalone: true,
  imports: [SideBarAdminComponent, MessageComponent, FormsModule, CommonModule],
  templateUrl: './create-room.component.html',
  styleUrls: ['./create-room.component.css']
})
export class CreateRoomComponent {
  @ViewChild('roomForm') roomForm!: NgForm;

  isSubmitted = false;
  room = {
    roomNumber: '',
    typeDesignation: '',
    capacity: '',
    availableEquipment: '',
    maintenanceSlots: ''
  };

  typeDesignations : string[] = [];
  
  constructor(private service: RoomService) { }

  ngOnInit(): void {  
    this.service.getAllRoomTypesAvailable().subscribe({
      next: data => {
        this.typeDesignations = data;
        console.log(data);
      }
    });
    
  }


  onSubmit(form: NgForm): void {
    this.isSubmitted = true;
    if (form.valid) {
      this.service.createRoom(
        this.room.roomNumber,
        this.room.typeDesignation,
        this.room.capacity,
        this.room.availableEquipment,
        this.room.maintenanceSlots
      );
    } else {
      this.isSubmitted = false;
    }
  }

  clearForm(): void {
    this.isSubmitted = false;

    this.room = {
      roomNumber: '',
      typeDesignation: '',
      capacity: '',
      availableEquipment: '',
      maintenanceSlots: ''
    };
    if (this.roomForm) {
      this.roomForm.resetForm();
    }
    const inputs = document.querySelectorAll('.input-field input');
    inputs.forEach(input => {
      input.classList.remove('invalid-placeholder');
    });
  }
}

