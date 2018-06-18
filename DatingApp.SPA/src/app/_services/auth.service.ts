import { Injectable } from '@angular/core';
import { map, catchError } from 'rxjs/operators';
import { throwError } from 'rxjs';
import {
  HttpClient,
  HttpHeaders,
  HttpResponse,
  HttpErrorResponse
} from '@angular/common/http';
import { JwtHelperService } from '@auth0/angular-jwt';

const LOCALSTORAGE_TOKEN_KEY = 'token';

@Injectable()
export class AuthService {
  baseUrl = 'https://localhost:5001/api/auth/';
  private userToken: any;
  private decodedToken: any;
  private readonly jwt = new JwtHelperService();

  constructor(private http: HttpClient) {}

  login(model: any) {
    return this.http
      .post(this.baseUrl + 'login', model, this.httpOptions())
      .pipe(
        map((user: any) => {
          if (user) {
            const token = user.tokenString;
            localStorage.setItem(LOCALSTORAGE_TOKEN_KEY, token);
            this.userToken = token;
            this.decodedToken = this.jwt.decodeToken(token);
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

  logout() {
    this.userToken = null;
    this.decodedToken = null;
    localStorage.removeItem(LOCALSTORAGE_TOKEN_KEY);
  }

  loggedIn() {
    const token: string = localStorage.getItem(LOCALSTORAGE_TOKEN_KEY);
    return token ? !this.jwt.isTokenExpired(token) : false;
  }

  getUsername() {
    if (!this.decodedToken) {
      this.loadTokenFromLocalStorage();
    }

    // the token can still be null because we are not logged in
    return this.decodedToken ? this.decodedToken.unique_name : '';
  }

  private loadTokenFromLocalStorage() {
    const token = localStorage.getItem(LOCALSTORAGE_TOKEN_KEY);
    if (token) {
      this.userToken = token;
      this.decodedToken = this.jwt.decodeToken(token);
    }
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
