import { AuthenticationService } from './../_services/authentication.service';
import { Component, OnInit } from '@angular/core';

@Component({
    selector: 'app-lockout',
    templateUrl: './lockout.component.html',
    styles: []
})
export class LockoutComponent implements OnInit {

    constructor(private _auth: AuthenticationService) { }

    ngOnInit() {
        this._auth.removeToken();
    }

}
