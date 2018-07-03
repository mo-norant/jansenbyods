import { AuthenticationService } from './../auth/_services/authentication.service';
import { Injectable } from '@angular/core';
import { HttpInterceptor, HttpRequest, HttpHandler, HttpEvent } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable()
export class TokenInterceptorService implements HttpInterceptor {

  constructor(public auth: AuthenticationService) {}

  intercept(request: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
    

    if(this.auth.getToken() != null){
      if(request.url.indexOf("/connect/token") < 0 || request.url.indexOf("api/Identity/Create") < 0){
        request = request.clone({
          setHeaders: {
            Authorization: `Bearer ${this.auth.getToken().access_token}`
          }
        });
      }
  
      return next.handle(request);
    }

    else{
      return next.handle(request);
    }
    

  }

}
