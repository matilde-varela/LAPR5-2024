import { Component } from '@angular/core';
import { SideBarDoctorComponent } from './sidebar-doctor/side-bar-doctor.component';
import { RouterLink } from '@angular/router';

@Component({
  selector: 'app-doctor',
  standalone: true,
  imports: [RouterLink,SideBarDoctorComponent],
  templateUrl: './doctor.component.html',
  styleUrl: './doctor.component.css'
})
export class DoctorComponent {

}
