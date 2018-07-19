import { Router } from '@angular/router';
import { AuthenticationService } from './../auth/_services/authentication.service';
import { Injectable } from '@angular/core';
import { HttpInterceptor, HttpRequest, HttpHandler, HttpEvent, HttpErrorResponse, HttpResponse } from '@angular/common/http';
import { Observable } from 'rxjs';
import 'rxjs/add/operator/do';

@Injectable()
export class TokenInterceptorService implements HttpInterceptor {

    constructor(public auth: AuthenticationService, private router : Router) { }

    intercept(request: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {


        if(this.auth.tokenExpired()){
            this.auth.removeToken();
            this.router.navigate(["login"]);
        }

        if (this.auth.getToken() != null) {
            if (request.url.indexOf("/connect/token") < 0 || request.url.indexOf("api/Identity/Create") < 0) {
                request = request.clone({
                    setHeaders: {
                        Authorization: `Bearer ${this.auth.getToken().access_token}`
                    }
                });
            }
        }

        return next.handle(request).do((event: HttpEvent<any>) => {
            if (event instanceof HttpResponse) {
              // do stuff with response if you want
            }
          }, (err: any) => {
            if (err instanceof HttpErrorResponse) {
              if (err.status === 401) {
                  this.auth.removeToken();
                this.router.navigate(["login"])
              }
            }
          });


    }

}
