import { Injectable } from '@angular/core';
import { map, catchError } from 'rxjs/operators';
import { throwError, BehaviorSubject } from 'rxjs';
import {
  HttpClient,
  HttpHeaders,
  HttpResponse,
  HttpErrorResponse
} from '@angular/common/http';
import { JwtHelperService } from '@auth0/angular-jwt';
import { environment } from '../../environments/environment';
import { User } from '../_models/User';

const LOCALSTORAGE_TOKEN_KEY = 'token';
const LOCALSTORAGE_USER_KEY = 'user';

@Injectable()
export class AuthService {
  baseUrl = environment.apiUrl + 'auth/';
  private userToken: any;
  private decodedToken: any;
  private readonly jwt = new JwtHelperService();
  private user: User;
  private mainPhotoUrl = new BehaviorSubject<string>('../../assets/user.png');
  currentPhotoUrl = this.mainPhotoUrl.asObservable();

  constructor(private http: HttpClient) {
    this.loadFromLocalStorage();
  }

  register(model: any) {
    return this.http
      .post(this.baseUrl + 'register', model, this.httpOptions())
      .pipe(catchError(this.handleError));
  }

  login(model: any) {
    return this.http
      .post(this.baseUrl + 'login', model, this.httpOptions())
      .pipe(
        map((response: any) => {
          if (response) {
            const token = response.tokenString;
            localStorage.setItem(LOCALSTORAGE_TOKEN_KEY, token);
            this.userToken = token;
            this.decodedToken = this.jwt.decodeToken(token);

            this.user = response.user;
            localStorage.setItem(LOCALSTORAGE_USER_KEY, JSON.stringify(this.user));
            this.mainPhotoUrl.next(this.user.profilePhotoUrl);
          }
        }),
        catchError(this.handleError)
      );
  }

  logout() {
    this.userToken = null;
    this.decodedToken = null;
    localStorage.removeItem(LOCALSTORAGE_TOKEN_KEY);
    localStorage.removeItem(LOCALSTORAGE_USER_KEY);
  }

  loggedIn() {
    const token: string = localStorage.getItem(LOCALSTORAGE_TOKEN_KEY);
    return token ? !this.jwt.isTokenExpired(token) : false;
  }

  getUsername() {
    // the token can still be null because we are not logged in
    return this.decodedToken ? this.decodedToken.unique_name : '';
  }

  getUserId() {
    // the token can still be null because we are not logged in
    return this.decodedToken ? this.decodedToken.nameid : '';
  }

  getToken() {
    return this.userToken;
  }

  changeMemberPhoto(photoUrl: string) {
    this.mainPhotoUrl.next(photoUrl);
    this.user.profilePhotoUrl = photoUrl;
    localStorage.setItem(LOCALSTORAGE_USER_KEY, JSON.stringify(this.user));
  }

  loadFromLocalStorage() {
    const token = localStorage.getItem(LOCALSTORAGE_TOKEN_KEY);
    if (token) {
      this.userToken = token;
      this.decodedToken = this.jwt.decodeToken(token);
    }
    const user = localStorage.getItem(LOCALSTORAGE_USER_KEY);
    if (user) {
      this.user = JSON.parse(user);
      this.mainPhotoUrl.next(this.user.profilePhotoUrl);
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
