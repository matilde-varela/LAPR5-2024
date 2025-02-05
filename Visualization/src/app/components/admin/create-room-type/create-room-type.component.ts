import { Component, ViewChild } from '@angular/core';
import { FormsModule, NgForm } from '@angular/forms';
import { SideBarAdminComponent } from '../sidebar-admin/side-bar-admin.component';
import { MessageComponent } from '../../message/message.component';
import { CommonModule } from '@angular/common';
import { RoomType } from 'src/app/domain/room-type';
import { RoomTypeService } from 'src/app/services/room-type.service';

@Component({
  selector: 'app-create-room-type',
  standalone: true,
  imports: [SideBarAdminComponent, MessageComponent, FormsModule, CommonModule],
  templateUrl: './create-room-type.component.html',
  styleUrl: './create-room-type.component.css'
})
export class CreateRoomTypeComponent {
 @ViewChild('roomtypeForm') roomtypeForm!: NgForm;

  isSubmitted = false;
  roomtype: RoomType = {
    code: '',
    designation: '',
    description: '',
  };

  constructor(private service: RoomTypeService) { }

  onSubmit(form: NgForm): void {
    if (form.valid) {
      this.isSubmitted = true;
      this.service.createRoomType(this.roomtype.code, this.roomtype.designation, this.roomtype.description!);
    } else {
      this.isSubmitted = false;
    }
  }

  clearForm(): void {
    this.roomtype = {
      code: '',
      designation: '',
      description: '',
    };
  
    if (this.roomtypeForm) {
      this.roomtypeForm.resetForm();
    }
  
    const inputs = document.querySelectorAll('.input-field input');
    inputs.forEach(input => {
      input.classList.remove('invalid-placeholder');
    });
  }
}
