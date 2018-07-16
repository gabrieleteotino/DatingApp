import { Injectable } from '@angular/core';
import { environment } from '../../environments/environment';
import { User } from '../_models/User';
import {
  HttpClient,
  HttpHeaders,
  HttpErrorResponse
} from '@angular/common/http';
import { Observable, throwError } from 'rxjs';
import { catchError } from 'rxjs/operators';

@Injectable({
  providedIn: 'root'
})
export class UserService {
  baseUrl = environment.apiUrl + 'users/';

  constructor(private http: HttpClient) {}

  getUsers(): Observable<User[]> {
    return this.http
      .get<User[]>(this.baseUrl)
      .pipe(catchError(this.handleError));
  }

  getUser(id: number): Observable<User> {
    return this.http
      .get<User>(this.baseUrl + id)
      .pipe(catchError(this.handleError));
  }

  updateUser(id: number, user: User): Observable<any> {
    return this.http
      .put(this.baseUrl + id, user)
      .pipe(catchError(this.handleError));
  }

  setMainPhoto(userId: number, photoId: number) {
    return this.http
      .post(this.baseUrl + userId + '/photos/' + photoId + '/setMain', {})
      .pipe(catchError(this.handleError));
  }

  deletePhoto(userId: number, photoId:number) {
    return this.http
      .delete(this.baseUrl + userId + '/photos/' + photoId)
      .pipe(catchError(this.handleError));
  }

  private handleError(errorResponse: HttpErrorResponse) {
    const applicationError = errorResponse.headers.get('Application-Error');
    if (applicationError) {
      return throwError(applicationError);
    }
    const serverError = errorResponse.error;
    let modelStateErrors = '';
    if (serverError) {
      for (const key in serverError) {
        if (serverError[key]) {
          modelStateErrors += serverError[key] + '\n';
        }
      }
    }
    return throwError(
      // if there is no model state error we still send a message
      modelStateErrors || 'Server error'
    );
  }
}
