import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { UserService } from '../../services/user.service';

@Component({
  selector: 'app-login-callback',
  standalone: true,
  imports: [],
  templateUrl: './login-callback.component.html',
  styleUrl: './login-callback.component.css'
})
export class LoginCallbackComponent implements OnInit {
  constructor(private router: Router, private service: UserService) { }

  ngOnInit(): void {
    const params = new URLSearchParams(window.location.search);
    const token = params.get('token');
    const error = params.get('error');

    if (token) {
      this.service.decodeTokenandRedirect(token);   // calls service to redirect page to the current logged in user
    } else if (error) {
      this.router.navigate(['/login']); // Redirect to login page
    }
  }
}
