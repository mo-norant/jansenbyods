import { Injectable } from '@angular/core';
import { AuthenticationService } from '../_services';
import { Router, ActivatedRouteSnapshot, RouterStateSnapshot } from '@angular/router';
import { Observable } from 'rxjs/Observable';

@Injectable()
export class UserguardService {

    constructor(private auth: AuthenticationService, private _router: Router) { }


    canActivate(route: ActivatedRouteSnapshot, state: RouterStateSnapshot): boolean | Observable<boolean> | Promise<boolean> {
        if (this.auth.role !== "user") {
            this._router.navigate(['/login'], { queryParams: { returnUrl: state.url } });
            return false;
        }
        return true;


    }

}
