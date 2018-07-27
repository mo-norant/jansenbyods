import { AdminService } from './../../../../../_services/admin.service';
import { ConfirmationService } from 'primeng/api';
import { Router, ActivatedRoute } from '@angular/router';
import { OogstKaartItem, Specificatie, Message } from './../../../../../auth/_models/models';
import { Component, OnInit } from '@angular/core';

@Component({
    selector: 'app-admin-oogstkaart-item',
    templateUrl: './admin-oogstkaart-item.component.html',
    styles: []
})
export class AdminOogstkaartItemComponent implements OnInit {

    item: OogstKaartItem = new OogstKaartItem();
    updated: boolean;
    msgs: Message[] = [];
    loading: boolean;
    prijsovereentekomen: boolean;

    constructor(
        private route: ActivatedRoute,
        private router: Router,
        private adminservice: AdminService,
        private dialogservice: ConfirmationService
    ) { }

    ngOnInit() {
        this.loading = true;
        this.route.params.subscribe(data => {
            this.adminservice.getOogstkaartItem(data["id"]).subscribe(
                data => {
                    this.item = data;
                    this.item.createDate = data.createDate;

                    if(data.vraagPrijsPerEenheid === 0 && data.vraagPrijsTotaal === 0  ){
                        this.prijsovereentekomen = true;
                    }

                    this.loading = false;


                },
                err => {
                       this.router.navigate(["admin/oogstkaart"]);
                }
            );
        });
    }

    update() {

        this.loading = true;

        this.dialogservice.confirm({
            message: "Wilt u dit product updaten?",
            accept: () => {

                if(this.prijsovereentekomen){
                    this.item.vraagPrijsPerEenheid = 0;
                    this.item.vraagPrijsTotaal = 0;
                }

                this.adminservice.UpdateOogstkaartitem(this.item).subscribe(data => {
                    this.item = data;
                    this.loading = false;

                }, err => {
                    this.loading = false;
                });
            }, reject : () => {
                this.loading = false;
            }
        });
    }

    delete() {
        this.loading = true
        this.dialogservice.confirm({
            message: "Wilt u dit product verwijderen?",
            accept: () => {
                this.adminservice
                    .DeleteItem(this.item.oogstkaartItemID)
                    .subscribe(d => {
                        this.loading = false;
                        this.router.navigate(["admin/oogstkaart"]);
                    }, err => {
                        this.loading = false;
                        alert("product werd niet verwijderd.");
                        this.router.navigate(["admin/oogstkaart"]);

                    });
            }, reject : () => {
                this.loading = false;
            }
        });
    }

    cancel() {
        this.router.navigate(["admin/oogstkaart"]);
    }

    itemupdatet() {
        this.updated = true;
    }

    sold() {
        this.loading = true;
        this.dialogservice.confirm({
            message: "Verkoopstatus wijzigen?",
            accept: () => {
                this.adminservice
                    .ProductSold(this.item.oogstkaartItemID)
                    .subscribe(res => {
                        this.item.sold = res
                        this.loading = false;
                    }, err => {
                        this.loading = false;

                    });
            }
        });


    }

    goToProduct() {
        var url = "http://jansenbyods.com/oogstkaart/" + this.item.oogstkaartItemID;
        var win = window.open(url, '_blank');
        win.focus();      }


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
