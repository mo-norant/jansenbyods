import { Router } from '@angular/router';
import { OogstKaartItem } from './../../../../../auth/_models/models';
import { Component, OnInit } from '@angular/core';

@Component({
    selector: 'app-admin-oogstkaart-item',
    templateUrl: './admin-oogstkaart-item.component.html',
    styles: []
})
export class AdminOogstkaartItemComponent implements OnInit {

    item: OogstKaartItem;
    constructor(private router: Router) { }

    ngOnInit() {
    }

    onRowSelect($event) {
        console.log($event);
    }


    itemupdatet() { }

    delete() {

    }
    update() {

    }

    cancel() {
        this.router.navigate(['admin'])
    }

}
