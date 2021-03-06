import { Injectable } from '@angular/core';
import { environment } from '../../environments/environment';
import { User } from '../_models/User';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';
import { PaginatedResult } from '../_models/PaginatedResult';
import { Message } from '../_models/Message';

@Injectable({
  providedIn: 'root'
})
export class UserService {
  baseUrl = environment.apiUrl + 'users/';

  constructor(private http: HttpClient) {}

  getUsers(page?: any, itemsPerPage?: any, userParams?: any, likesParam?: string): Observable<PaginatedResult<User[]>> {
    let params = new HttpParams();
    if (page != null && itemsPerPage != null) {
      params = params.set('pageNumber', page).set('pageSize', itemsPerPage);
    }
    if (userParams != null) {
      params = params
        .set('minAge', userParams.minAge)
        .set('maxAge', userParams.maxAge)
        .set('gender', userParams.gender)
        .set('orderBy', userParams.orderBy);
    }
    if (likesParam === 'Likers') {
      params = params.set('likers', 'true');
    }
    if (likesParam === 'Likees') {
      params = params.set('likees', 'true');
    }

    return this.http
      .get<User[]>(this.baseUrl, {
        params: params,
        observe: 'response'
      })
      .pipe(
        map(response => {
          const paginatedResults = new PaginatedResult<User[]>();
          paginatedResults.result = response.body;
          if (response.headers.get('Pagination') != null) {
            paginatedResults.pagination = JSON.parse(response.headers.get('Pagination'));
          }
          return paginatedResults;
        })
      );
  }

  getUser(id: number): Observable<User> {
    return this.http.get<User>(this.baseUrl + id);
  }

  updateUser(id: number, user: User): Observable<any> {
    return this.http.put(this.baseUrl + id, user);
  }

  setMainPhoto(userId: number, photoId: number) {
    return this.http.post(this.baseUrl + userId + '/photos/' + photoId + '/setMain', {});
  }

  deletePhoto(userId: number, photoId: number) {
    return this.http.delete(this.baseUrl + userId + '/photos/' + photoId);
  }

  sendLike(fromUserId: number, toUserId: number) {
    return this.http.post(this.baseUrl + fromUserId + '/like/' + toUserId, {});
  }

  getMessages(
    userId: number,
    pageNumber?: number,
    itemsPerPage?: number,
    messageContainer?: string
  ): Observable<PaginatedResult<Message[]>> {
    let params = new HttpParams();
    if (pageNumber != null && itemsPerPage != null) {
      params = params.set('pageNumber', pageNumber.toString()).set('pageSize', itemsPerPage.toString());
    }
    if (messageContainer != null) {
      params = params.set('messageContainer', messageContainer);
    }

    return this.http
      .get<Message[]>(this.baseUrl + userId + '/messages', {
        params: params,
        observe: 'response'
      })
      .pipe(
        map(response => {
          const paginatedResults = new PaginatedResult<Message[]>();
          paginatedResults.result = response.body;
          if (response.headers.get('Pagination') != null) {
            paginatedResults.pagination = JSON.parse(response.headers.get('Pagination'));
          }
          return paginatedResults;
        })
      );
  }

  getMessageThread(fromUserId: number, toUserId: number) {
    return this.http.get<Message[]>(this.baseUrl + fromUserId + '/messages/thread/' + toUserId);
  }

  sendMessage(userId: number, message: Message) {
    return this.http.post(this.baseUrl + userId + '/messages', message);
  }

  deleteMessage(userId: number, messageId: number) {
    return this.http.post(this.baseUrl + userId + '/messages/' + messageId, {});
  }

  markMessageAsRead(userId: number, messageId: number) {
    return this.http.post(this.baseUrl + userId + '/messages/' + messageId + '/read', {}).subscribe();
  }
}
