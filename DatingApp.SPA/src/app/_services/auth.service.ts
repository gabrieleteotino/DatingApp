import { Injectable } from '@angular/core';
import { map, catchError } from 'rxjs/operators';
import { _throw } from 'rxjs/observable/throw';
import { HttpClient, HttpHeaders, HttpResponse, HttpErrorResponse } from '@angular/common/http';

@Injectable()
export class AuthService {
  baseUrl = 'https://localhost:5001/api/auth/';
  userToken: any;

  constructor(private http: HttpClient) {}

  login(model: any) {
    return this.http
      .post(this.baseUrl + 'login', model, this.httpOptions())
      .pipe(
        map((user: HttpResponse<any>) => {
          if (user) {
            localStorage.setItem('token', user.tokenString);
            this.userToken = user.tokenString;
          }
        }),
        catchError(this.handleError)
      );
  }

  register(model: any) {
    return this.http
      .post(this.baseUrl + 'register', model, this.httpOptions())
      .pipe(catchError(this.handleError));
  }

  private httpOptions() {
    return {
      headers: new HttpHeaders({ 'Content-type': 'application/json' }),
      oberve: 'response'
    };
  }

  private handleError(errorResponse: HttpErrorResponse) {
    const applicationError = errorResponse.headers.get('Application-Error');
    if (applicationError) {
      return _throw(applicationError);
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
    return _throw(
      // if there is no model state error we still send a message
      modelStateErrors || 'Server error'
    );
  }
}
