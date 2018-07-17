import { ConfirmationService } from 'primeng/api';
import { Router, ActivatedRoute } from '@angular/router';
import { OogstKaartItem, Specificatie, Message } from './../../../../../auth/_models/models';
import { Component, OnInit } from '@angular/core';
import { OogstkaartService } from '../../../../../_services/oogstkaart.service';

@Component({
    selector: 'app-admin-oogstkaart-item',
    templateUrl: './admin-oogstkaart-item.component.html',
    styles: []
})
export class AdminOogstkaartItemComponent implements OnInit {

    item: OogstKaartItem = new OogstKaartItem();
    updated: boolean;
    msgs: Message[] = [];

    constructor(
        private route: ActivatedRoute,
        private router: Router,
        private oogstkaartservice: OogstkaartService,
        private dialogservice: ConfirmationService
    ) { }

    ngOnInit() {
        this.route.params.subscribe(data => {
            this.oogstkaartservice.getOogstkaartItem(data["id"]).subscribe(
                data => {
                    this.item = data;
                    this.item.createDate = data.createDate;
                },
                err => {
                    //    this.router.navigate(["oogstkaart"]);
                }
            );
        });
    }

    update() {



        this.dialogservice.confirm({
            message: "Wilt u dit product updaten?",
            accept: () => {
                this.oogstkaartservice.UpdateOogstkaartitem(this.item).subscribe(data => {
                    this.router.navigate(["oogstkaart"])
                });
            }
        });
    }

    delete() {
        this.dialogservice.confirm({
            message: "Wilt u dit product verwijderen?",
            accept: () => {
                this.oogstkaartservice
                    .DeleteItem(this.item.oogstkaartItemID)
                    .subscribe(d => {
                       
                        this.router.navigate(["oogstkaart"]);
                    });
            }
        });
    }

    cancel() {
        this.router.navigate(["oogstkaart"]);
    }

    itemupdatet() {
        this.updated = true;
    }

    sold() {
        this.dialogservice.confirm({
            message: "Verkoopstatus wijzigen?",
            accept: () => {
                this.oogstkaartservice
                    .ProductSold(this.item.oogstkaartItemID)
                    .subscribe(res => {
                        this.item.sold = res
                        if (this.item.sold) {
                           
                        } else {
                           
                        }

                    });
            }
        });


    }

    goToProduct() {
        window.location.href = "http://jansenbyods.com/oogstkaart/" + this.item.oogstkaartItemID;
    }


    addSpecificatie() {

        let spec = new Specificatie();
        this.item.specificaties.push(spec);

        this.itemupdatet();
    }

    removeItem(index) {
        if (index > -1) {
            this.item.specificaties.splice(index, 1);
            this.itemupdatet();

        }
    }

}
