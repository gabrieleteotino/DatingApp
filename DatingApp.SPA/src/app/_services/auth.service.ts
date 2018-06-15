import { map, catchError } from 'rxjs/operators';
import { Injectable } from '@angular/core';
import { Http, Headers, RequestOptions } from '@angular/http';
import { Observable } from 'rxjs/Observable';
import { ErrorObservable } from 'rxjs/observable/ErrorObservable';
import { _throw } from 'rxjs/observable/throw';
// import { Observable, throwError } from 'rxjs';

// import 'rxjs/add/observable/throw';

@Injectable()
export class AuthService {
  baseUrl = 'https://localhost:5001/api/auth/';
  userToken: any;

  constructor(private http: Http) {}

  login(model: any) {
    return this.http
      .post(this.baseUrl + 'login', model, this.httpOptions())
      .pipe(
        map(response => {
          const user = response.json();
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
    const header = new Headers({ 'Content-type': 'application/json' });
    return new RequestOptions({ headers: header });
  }

  private handleError(error: any) {
    const applicationError = error.headers.get('Application-Error');
    if (applicationError) {
      // return Observable.throw(applicationError);
      _throw(applicationError);
    }
    const serverError = error.json();
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
