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
                        this.msgs.push({
                            severity: "delete",
                            summary: "Verwijderd",
                            detail: "Product succesvol verwijderd"
                        });
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
