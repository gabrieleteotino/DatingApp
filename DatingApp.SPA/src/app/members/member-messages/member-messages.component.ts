import { Component, OnInit, Input } from '@angular/core';
import { Message } from '../../_models/Message';
import { UserService } from '../../_services/user.service';
import { AuthService } from '../../_services/auth.service';
import { AlertifyService } from '../../_services/alertify.service';
import { tap } from 'rxjs/operators';

@Component({
  selector: 'app-member-messages',
  templateUrl: './member-messages.component.html',
  styleUrls: ['./member-messages.component.css']
})
export class MemberMessagesComponent implements OnInit {
  @Input() recipientId: number;
  messages: Message[];
  newMessage: any = {};

  constructor(private userService: UserService, private authService: AuthService, private alertify: AlertifyService) {}

  ngOnInit() {
    this.loadMessages();
  }

  loadMessages() {
    const userID = +this.authService.getUserId();
    this.userService
      .getMessageThread(userID, this.recipientId)
      .pipe(
        tap(messages => {
          for (let i = 0; i < messages.length; i++) {
            const message = messages[i];
            if (message.recipientId === userID && !message.isRead) {
              this.userService.markMessageAsRead(userID, message.id);
            }
          }
        })
      )
      .subscribe(messages => (this.messages = messages), error => this.alertify.error(error));
  }

  sendMessage() {
    this.newMessage.recipientId = this.recipientId;
    this.userService.sendMessage(this.authService.getUserId(), this.newMessage).subscribe(
      (message: Message) => {
        this.messages.unshift(message);
        // reset the form
        this.newMessage.content = '';
      },
      error => this.alertify.error(error)
    );
  }
}
