import { Component, OnInit } from '@angular/core';
import { AuthService } from '../_services/auth.service';
import { AlertifyService } from '../_services/alertify.service';
import { Router } from '@angular/router';
import { User } from '../_models/User';

@Component({
  selector: 'app-nav',
  templateUrl: './nav.component.html',
  styleUrls: ['./nav.component.css']
})
export class NavComponent implements OnInit {
  model: any = {};
  photoUrl: string;

  constructor(
    private authService: AuthService,
    private alertify: AlertifyService,
    private router: Router
  ) {}

  ngOnInit() {
    this.authService.currentPhotoUrl.subscribe(next => this.photoUrl = next);
  }

  login() {
    console.log(this.model);
    this.authService.login(this.model).subscribe(
      data => {
        this.alertify.success('Sucessfully logged in');
      },
      error => {
        console.log(error);
        this.alertify.error('Failed to login');
      },
      () => {
        this.router.navigate(['/members']);
      }
    );
  }

  logout() {
    this.authService.logout();
    this.alertify.message('Logged out');
    this.router.navigate(['/home']);
  }

  loggedIn() {
    const token = localStorage.getItem('token');
    return !!token;
  }

  getUsername() {
    return this.authService.getUsername();
  }
}
