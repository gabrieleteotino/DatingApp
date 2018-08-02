import { Injectable } from '@angular/core';
import {
  HttpInterceptor,
  HttpRequest,
  HttpHandler,
  HttpEvent,
  HttpErrorResponse,
  HTTP_INTERCEPTORS
} from '@angular/common/http';
import { Observable, throwError } from 'rxjs';
import { catchError } from 'rxjs/operators';

@Injectable()
export class ErrorInterceptor implements HttpInterceptor {
  intercept(req: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
    return next.handle(req).pipe(catchError(this.handleError));
  }

  private handleError(error: any) {
    if (error instanceof HttpErrorResponse) {
      if (error.status === 401) {
        return throwError(error.statusText);
      }
      const applicationError = error.headers.get('Application-Error');
      if (applicationError) {
        console.error(applicationError);
        return throwError(applicationError);
      }
      const serverError = error.error;
      let modelStateErrors = '';
      if (serverError && typeof serverError === 'object') {
        for (const key in serverError) {
          if (serverError[key]) {
            modelStateErrors += serverError[key] + '\n';
          }
        }
      }
      return throwError(
        // if there is no model state error we still send a message
        modelStateErrors || serverError || 'Server error'
      );
    }
  }
}

export const ErrorInterceptorProvider = {
  provide: HTTP_INTERCEPTORS,
  useClass: ErrorInterceptor,
  multi: true
};