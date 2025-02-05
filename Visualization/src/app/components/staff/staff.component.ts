import { Component } from '@angular/core';
import { RouterLink } from '@angular/router';
import { SideBarStaffComponent } from "./sidebar-staff/side-bar-staff.component";

@Component({
  selector: 'app-staff',
  standalone: true,
  imports: [RouterLink, SideBarStaffComponent],
  templateUrl: './staff.component.html',
  styleUrl: './staff.component.css'
})
export class StaffComponent {

}
