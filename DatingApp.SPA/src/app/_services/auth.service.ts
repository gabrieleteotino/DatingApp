import { map } from 'rxjs/operators';
// import 'rxjs/add/operator/map';
import { Injectable } from '@angular/core';
import { Http, Headers, RequestOptions } from '@angular/http';

@Injectable()
export class AuthService {
  baseUrl = 'https://localhost:5001/api/auth/';
  userToken: any;

  constructor(private http: Http) {}

  login(model: any) {
    const header = new Headers({ 'Content-type': 'application/json' });
    const options = new RequestOptions({ headers: header });

    return this.http.post(this.baseUrl + 'login', model, options).pipe(
      map(response => {
        const user = response.json();
        if (user) {
          localStorage.setItem('token', user.tokenString);
          this.userToken = user.tokenString;
        }
      })
    );
  }
}
