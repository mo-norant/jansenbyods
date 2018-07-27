import { Router } from '@angular/router';
import { OogstKaartItem } from './../../../../../auth/_models/models';
import { AdminService } from './../../../../../_services/admin.service';
import { Component, OnInit } from '@angular/core';
import { Request } from '../../../../../auth/_models/models';

@Component({
    selector: 'app-admin-oogstkaart-list',
    templateUrl: './admin-oogstkaart-list.component.html',
    styles: []
})
export class AdminOogstkaartListComponent implements OnInit {

    items: OogstKaartItem[];
    cols: any[];
    selecteditem: OogstKaartItem;
    loading : boolean;
    constructor(private adminservice: AdminService, private router: Router) { }

    ngOnInit() {

        this.cols = [
            { field: "oogstkaartItemID", header: "ID" },
            { field: "artikelnaam", header: "Naam" },
            { field: "jansenserie", header: "Serie" },
            { field: "views", header: "Aantal keer bekeken" },
            { field: "localdatestring", header: "Aangemaakt op" }

        ];


        this.loading = true;
        this.adminservice.getOogstkaartItems().subscribe(data => {
            data.forEach(i => {
                i.localdatestring = new Date(i.createDate).toLocaleString();
            });
            this.items = data;
            this.loading = false;
        })
    }



    onRowSelect(event) {
        this.router.navigate(['admin/oogstkaart', event.data.oogstkaartItemID]);
    }
}
