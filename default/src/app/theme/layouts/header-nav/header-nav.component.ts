import { Router } from '@angular/router';
import { AuthenticationService } from './../../../auth/_services/authentication.service';
import { Component, OnInit, ViewEncapsulation, AfterViewInit } from '@angular/core';
import { Helpers } from '../../../helpers';

declare let mLayout: any;
@Component({
    selector: "app-header-nav",
    templateUrl: "./header-nav.component.html",
    encapsulation: ViewEncapsulation.None,
})
export class HeaderNavComponent implements OnInit, AfterViewInit {


    constructor(private router: Router) {

    }
    ngOnInit() {

    }
    ngAfterViewInit() {

        mLayout.initHeader();

    }

    signOut(){
            this.router.navigate(['logout']);
    }

}