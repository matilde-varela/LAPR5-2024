import { Component } from '@angular/core';
import { RouterLink } from '@angular/router';
import { SideBarAdminComponent } from "./sidebar-admin/side-bar-admin.component";

@Component({
  selector: 'app-admin',
  standalone: true,
  // imports: [MenuComponent, AppComponent],
  imports: [RouterLink, SideBarAdminComponent],
  templateUrl: './admin.component.html',
  styleUrl: './admin.component.css'
})
export class AdminComponent {

}
