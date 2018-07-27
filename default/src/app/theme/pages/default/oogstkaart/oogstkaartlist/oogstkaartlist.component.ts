import { Router } from '@angular/router';
import { OogstkaartService } from "./../../../../../_services/oogstkaart.service";
import { OogstKaartItem } from "./../../../../../auth/_models/models";
import { Component, OnInit } from "@angular/core";
import { Message, ConfirmationService } from 'primeng/api';
import { IfStmt } from '@angular/compiler';


@Component({
    selector: "app-oogstkaartlist",
    templateUrl: "./oogstkaartlist.component.html",
    styles: []
})
export class OogstkaartlistComponent implements OnInit {
    items: OogstKaartItem[];
    cols: any[];
    selecteditem: OogstKaartItem;
    msgs: Message[] = [];
    loading: boolean;
    
    selectionitems: number[] = [];


    constructor(private oogstkaartservice: OogstkaartService, private router: Router, private dialogservice: ConfirmationService) { }

    ngOnInit() {
        this.cols = [
            { field: "oogstkaartItemID", header: "ID" },
            { field: "artikelnaam", header: "Naam" },
            { field: "jansenserie", header: "Serie" },
            { field: "viewcount", header: "Aantal keer bekeken" },
            { field: "localdatestring", header: "Aangemaakt op" }

        ];

        this.loading = true;

        this.oogstkaartservice.GetOogstkaartitems().subscribe(data => {
            

            data.forEach(i => {
                i.localdatestring = new Date(i.createDate).toLocaleString();
            });

            data.forEach(element => {
                element.viewcount = element.views.length
            })

            this.items = data;
            this.loading = false;
        }, err => {
            this.loading = false;
            alert("Data niet beschikbaar");
        }


        );
    }

    selectbox($event) {
        if (!this.selectionitems.includes($event)) {
            this.selectionitems.push($event);
        } else {
            var index = this.selectionitems.indexOf($event, 0);
            if (index > -1) {
                this.selectionitems.splice(index, 1);
            }
        }
    }

    onRowSelect(event) {
        console.log(event);
        this.dialogservice.confirm({
            message: "Product bewerken?",
            key: "second",
            accept: () => {
                this.router.navigate(['oogstkaart', event.data.oogstkaartItemID]);
            },
        });

    }

    productDelete() {
        this.loading = true;
        this.dialogservice.confirm({
            message: "Product verwijderen?",
            key: "delete",
            accept: () => {
                console.log(this.selectionitems)
                this.oogstkaartservice.DeleteRange(this.selectionitems).subscribe(res => {
                    this.oogstkaartservice.GetOogstkaartitems().subscribe(data => {
                        data.forEach(i => {
                            i.localdatestring = new Date(i.createDate).toLocaleString();
                        });
                        this.items = data;
                        this.loading = false;
                    });
                });
            },
            reject: () => {
                this.loading = false
            }
        });
    }




}
