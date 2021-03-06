import { Component, OnInit } from '@angular/core';
import { Message } from '../_models/Message';
import { Pagination } from '../_models/Pagination';
import { UserService } from '../_services/user.service';
import { AuthService } from '../_services/auth.service';
import { ActivatedRoute } from '@angular/router';
import { AlertifyService } from '../_services/alertify.service';
import { MessagesResolver } from '../_resolvers/messages.resolver';
import { PaginatedResult } from '../_models/PaginatedResult';

@Component({
  selector: 'app-messages',
  templateUrl: './messages.component.html',
  styleUrls: ['./messages.component.css']
})
export class MessagesComponent implements OnInit {
  messages: Message[];
  pagination: Pagination;
  messageContainer = 'Unread';

  constructor(
    private userService: UserService,
    private authService: AuthService,
    private route: ActivatedRoute,
    private alertify: AlertifyService
  ) {}

  ngOnInit() {
    this.route.data.subscribe(data => {
      const paginatedResult = <PaginatedResult<Message[]>>data['messages'];
      this.messages = paginatedResult.result;
      this.pagination = paginatedResult.pagination;
    });
  }

  loadMessages() {
    this.userService
      .getMessages(
        this.authService.getUserId(),
        this.pagination.currentPage,
        this.pagination.itemsPerPage,
        this.messageContainer
      )
      .subscribe(
        paginatedResult => {
          this.messages = paginatedResult.result;
          this.pagination = paginatedResult.pagination;
        },
        error => this.alertify.error(error)
      );
  }

  pageChanged(event: any): void {
    this.pagination.currentPage = event.page;
    this.loadMessages();
  }

  deleteMessage(messageId: number) {
    this.alertify.confirm('Are you sure?', () => {
      this.userService.deleteMessage(this.authService.getUserId(), messageId).subscribe(
        () => {
          this.messages.splice(this.messages.findIndex(m => m.id === messageId), 1);
          this.alertify.success('Message deleted');
        },
        error => this.alertify.error(error)
      );
    });
  }
}
