import { Component, OnInit } from '@angular/core';
import { User } from '../../_models/User';
import { AlertifyService } from '../../_services/alertify.service';
import { ActivatedRoute } from '@angular/router';
import { UserService } from '../../_services/user.service';

@Component({
  selector: 'app-member-detail',
  templateUrl: './member-detail.component.html',
  styleUrls: ['./member-detail.component.css']
})
export class MemberDetailComponent implements OnInit {
  user: User;

  constructor(
    private userService: UserService,
    private alertify: AlertifyService,
    private route: ActivatedRoute
  ) {}

  ngOnInit() {
    this.loadUser(+this.route.snapshot.params['id']);
  }

  loadUser(id: number) {
    this.userService.getUser(id).subscribe(
      user => {
        this.user = user;
      },
      error => {
        this.alertify.error(error);
      }
    );
  }
}
