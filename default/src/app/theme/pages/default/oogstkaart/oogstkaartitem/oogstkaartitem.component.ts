import { MessageService } from "primeng/components/common/messageservice";
import { OogstKaartItem } from "./../../../../../auth/_models/models";
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
  ) {}

  ngOnInit() {
    this.route.params.subscribe(data => {
      this.oogstkaartservice.getOogstkaartItem(data["id"]).subscribe(
        item => {
          this.item = item;
          this.item.createDate = item.createDate;
        },
        err => {
          this.router.navigate(["oogstkaart"]);
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
}
