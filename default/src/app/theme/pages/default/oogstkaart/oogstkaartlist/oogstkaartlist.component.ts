import { Router } from '@angular/router';
import { OogstkaartService } from "./../../../../../_services/oogstkaart.service";
import { OogstKaartItem } from "./../../../../../auth/_models/models";
import { Component, OnInit } from "@angular/core";


@Component({
    selector: "app-oogstkaartlist",
    templateUrl: "./oogstkaartlist.component.html",
    styles: []
})
export class OogstkaartlistComponent implements OnInit {
    items: OogstKaartItem[];
    cols: any[];
    selecteditem: OogstKaartItem;

    constructor(private oogstkaartservice: OogstkaartService, private router: Router) { }

    ngOnInit() {
        this.cols = [
            { field: "oogstkaartItemID", header: "ID" },
            { field: "artikelnaam", header: "Naam" },
            { field: "jansenserie", header: "Serie" },
            { field: "views", header: "Aantal keer bekeken" },
            { field: "localdatestring", header: "Aangemaakt op" }

        ];


        this.oogstkaartservice.GetOogstkaartitems().subscribe(data => {

            data.forEach(i => {
                i.localdatestring = new Date(i.createDate).toLocaleString();
            });
            this.items = data;
        }


        );
    }

    onRowSelect(event) {
        this.router.navigate(['oogstkaart', event.data.oogstkaartItemID]);
    }


}
