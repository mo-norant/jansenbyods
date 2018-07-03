import { CanActivate, ActivatedRouteSnapshot, RouterStateSnapshot, Router } from '@angular/router';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs/Observable';
import { AuthenticationService } from '../_services';

@Injectable()
export class RoleService implements CanActivate {


  constructor(private auth : AuthenticationService, private _router: Router) { }

  
  canActivate(route: ActivatedRouteSnapshot, state: RouterStateSnapshot): boolean | Observable<boolean> | Promise<boolean> {
    if (this.auth.role !== "administrator") {      
        this._router.navigate(['/login'], { queryParams: { returnUrl: state.url } });
        return false;
    }
      return true;
    

  }
}
