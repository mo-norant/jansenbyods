import { OogstkaartService } from './../../../_services/oogstkaart.service';
import { GeneralService } from './../../../_services/general.service';
import { Menu, MenuItem } from './../../../auth/_models/models';
import { Component, OnInit, ViewEncapsulation, AfterViewInit } from '@angular/core';
import { Helpers } from '../../../helpers';
import { AuthenticationService } from '../../../auth/_services';

declare let mLayout: any;
@Component({
    selector: "app-aside-nav",
    templateUrl: "./aside-nav.component.html",
    encapsulation: ViewEncapsulation.None,
})
export class AsideNavComponent implements OnInit, AfterViewInit {


    menu: Menu;
    role: string;
    openrequests: number = 0;

    constructor(private auth: AuthenticationService, private oogstkaartservice: OogstkaartService) {
        this.role = this.auth.role;
        console.log(this.role)
    }
    ngOnInit() {

    }
    ngAfterViewInit() {

        mLayout.initAside();

    }

    administratorMenu(): Menu {
        let menu: Menu = new Menu();
        menu.menuitems = [];

        let dashboarditem: MenuItem = new MenuItem();
        dashboarditem.class = 'm-menu__item';
        dashboarditem.route = '/index';
        dashboarditem.name = 'Dashboard';
        dashboarditem.logo = 'm-menu__link-icon flaticon-line-graph'

        menu.menuitems.push(dashboarditem);

        let productenbeheren: MenuItem = new MenuItem();
        productenbeheren.class = 'm-menu__item';
        productenbeheren.route = '/oogstkaart';
        productenbeheren.name = 'Producten beheren';
        productenbeheren.logo = 'm-menu__link-icon flaticon-settings'

        menu.menuitems.push(productenbeheren);


        let productenaanbieden: MenuItem = new MenuItem();
        productenaanbieden.class = 'm-menu__item';
        productenaanbieden.route = '/oogstkaart/nieuwproduct';
        productenaanbieden.name = 'Producten aanbieden';
        productenaanbieden.logo = 'm-menu__link-icon flaticon-settings'

        menu.menuitems.push(productenaanbieden);

        return menu;
    }



}