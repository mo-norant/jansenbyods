import { Component, OnInit } from '@angular/core';
import { AuthenticationService } from '../../_services';

@Component({
    selector: 'app-confirmmail',
    templateUrl: './confirmmail.component.html',
    styles: []
})
export class ConfirmmailComponent implements OnInit {

    constructor(private _auth : AuthenticationService) { }

    ngOnInit() {
        this._auth.removeToken();
    }

}
