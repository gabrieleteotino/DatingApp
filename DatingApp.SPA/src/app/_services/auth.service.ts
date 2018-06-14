import { map } from 'rxjs/operators';
import { Injectable } from '@angular/core';
import { Http, Headers, RequestOptions } from '@angular/http';

@Injectable()
export class AuthService {
  baseUrl = 'https://localhost:5001/api/auth/';
  userToken: any;

  constructor(private http: Http) {}

  login(model: any) {
    return this.http.post(this.baseUrl + 'login', model, this.httpOptions()).pipe(
      map(response => {
        const user = response.json();
        if (user) {
          localStorage.setItem('token', user.tokenString);
          this.userToken = user.tokenString;
        }
      })
    );
  }

  register(model: any) {
    return this.http.post(this.baseUrl + 'register', model, this.httpOptions());
  }

  private httpOptions() {
    const header = new Headers({ 'Content-type': 'application/json' });
    return new RequestOptions({ headers: header });
  }
}
