import { Injectable } from '@angular/core';
import { environment } from '../../environments/environment';
import { User } from '../_models/User';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';
import { PaginatedResult } from '../_models/PaginatedResult';

@Injectable({
  providedIn: 'root'
})
export class UserService {
  baseUrl = environment.apiUrl + 'users/';

  constructor(private http: HttpClient) {}

  getUsers(page?: any, itemsPerPage?: any, userParams?: any): Observable<PaginatedResult<User[]>> {
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
}
