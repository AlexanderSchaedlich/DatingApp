import { Injectable } from '@angular/core';
import {
  HttpRequest,
  HttpHandler,
  HttpEvent,
  HttpInterceptor
} from '@angular/common/http';
import { Observable, throwError } from 'rxjs';
import { NavigationExtras, Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { catchError } from 'rxjs/operators';

@Injectable()
export class ErrorInterceptor implements HttpInterceptor {

  constructor(private router: Router, private toastr: ToastrService) {}

  intercept(request: HttpRequest<unknown>, next: HttpHandler): Observable<HttpEvent<unknown>> {
    return next.handle(request).pipe(
      catchError(error => {
        if (error) {
          switch(error.status) {
            case 400:
              if (typeof error.error == 'object') {
                const modalStateErrors = [];

                Object.entries(error.error.errors).forEach(([key, value]) => {
                  modalStateErrors.push(value);
                });

                throw modalStateErrors.flat();
              } else {
                this.toastr.error(error.statusText === 'OK' ? 'Bad request' : error.statusText, error.status);
              }

              break;

            case 401:
              this.toastr.error(error.statusText === 'OK' ? 'Unauthorized' : error.statusText, error.status);
              break;

            case 404:
              this.router.navigateByUrl('/not-found');
              break;

            case 500:
              const navigationExtras: NavigationExtras = {state: {error: error.error}};
              this.router.navigateByUrl('/server-error', navigationExtras);
              break;

            default:
              this.toastr.error('Something unexpected went wrong');
              console.log(error);
          }
        }

        return throwError(error);
      })
    );
  }
}
