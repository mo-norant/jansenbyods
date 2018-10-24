import { MessageService } from "primeng/components/common/messageservice";
import { OogstKaartItem, Specificatie } from "./../../../../../auth/_models/models";
import { OogstkaartService } from "./../../../../../_services/oogstkaart.service";
import { Component, OnInit } from "@angular/core";
import { ActivatedRoute, Router } from "@angular/router";
import { ConfirmationService } from "primeng/api";
import { Message } from "primeng/components/common/api";

@Component({
    selector: "app-oogstkaartitem",
    templateUrl: "./oogstkaartitem.component.html",
    styles: []
})
export class OogstkaartitemComponent implements OnInit {
    item: OogstKaartItem = new OogstKaartItem();
    updated: boolean;
    msgs: Message[] = [];
    loading: boolean;
    prijsovereentekomen: boolean;
    eigenschappen = [
        { label: 'Kleur', value: 'kleur' },
        { label: 'Gewicht', value: 'gewicht' },
        { label: 'Brandwerend', value: 'brandwerend' },
        { label: 'Lengte', value: 'lengte' },
        { label: 'Breedte', value: 'breedte' },
        { label: 'Hoogte', value: 'hoogte' },
        { label: 'RAL', value: 'hoogte' },
        { label: 'U-waarde', value: 'hoogte' },
        { label: 'Glas', value: 'hoogte' },

    ];


    constructor(
        private route: ActivatedRoute,
        private router: Router,
        private oogstkaartservice: OogstkaartService,
        private dialogservice: ConfirmationService
    ) { }

    ngOnInit() {
        this.loading = true;
        this.route.params.subscribe(data => {
            this.oogstkaartservice.getOogstkaartItem(data["id"]).subscribe(
                data => {
                    this.item = data;
                    this.item.createDate = data.createDate;

                    if (data.vraagPrijsPerEenheid === 0 && data.vraagPrijsTotaal === 0) {
                        this.prijsovereentekomen = true;
                    }

                    this.loading = false;


                },
                err => {
                    alert("Data niet beschikbaar");
                    this.router.navigate(["oogstkaart"]);
                }
            );
        });
    }

    update() {

        this.loading = true;

        this.dialogservice.confirm({
            message: "Wilt u dit product updaten?",
            accept: () => {

                if (this.prijsovereentekomen) {
                    this.item.vraagPrijsPerEenheid = 0;
                    this.item.vraagPrijsTotaal = 0;
                }

                this.oogstkaartservice.UpdateOogstkaartitem(this.item).subscribe(data => {
                    this.router.navigate(["oogstkaart"]);
                }, err => {
                    this.loading = false;
                    alert("data niet aangepast.")
                });
            }, reject: () => {
                this.loading = false;
            }
        });
    }

    delete() {
        this.loading = true
        this.dialogservice.confirm({
            message: "Wilt u dit product verwijderen?",
            accept: () => {
                this.oogstkaartservice
                    .DeleteItem(this.item.oogstkaartItemID)
                    .subscribe(d => {
                        this.loading = false;
                        this.msgs.push({
                            severity: "delete",
                            summary: "Verwijderd",
                            detail: "Product succesvol verwijderd"
                        });
                        this.router.navigate(["oogstkaart"]);
                    }, err => {
                        this.loading = false;
                        alert("product niet verwijderd.")
                    });
            }, reject: () => {
                this.loading = false;
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
                            this.msgs.push({
                                severity: "success",
                                summary: "Product",
                                detail: "Product werd verkocht."
                            });
                        } else {
                            this.msgs.push({
                                severity: "warning",
                                summary: "Product",
                                detail: "Product wordt terug verkocht."
                            });
                        }

                    });
            }
        });


    }

    goToProduct() {
        var url = "http://jansenbyods.com/oogstkaart/" + this.item.oogstkaartItemID;
        var win = window.open(url, '_blank');
        win.focus();
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

    removeFile(uri) {
        this.dialogservice.confirm({
            message: "Wilt u dit bestand verwijderen?",
            accept: () => {
                this.oogstkaartservice.RemoveFile(uri).subscribe(res => {
                    this.router.navigate(["oogstkaart"])
                })
            }
        });
    }

}
