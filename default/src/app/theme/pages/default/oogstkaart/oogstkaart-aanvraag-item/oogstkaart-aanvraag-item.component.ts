import { Request, OogstKaartItem } from './../../../../../auth/_models/models';
import { OogstkaartService } from './../../../../../_services/oogstkaart.service';
import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { Utils } from '../../../../../auth/_helpers/Utils';

@Component({
    selector: 'app-oogstkaart-aanvraag-item',
    templateUrl: './oogstkaart-aanvraag-item.component.html',
    styles: []
})
export class OogstkaartAanvraagItemComponent implements OnInit {

    root
    request: Request;
    item: OogstKaartItem;

    constructor(private oogstkaartservice: OogstkaartService, private route: ActivatedRoute, private toastr: ToastrService
    ) { }
    ngOnInit() {

        this.root = Utils.getRoot().replace("/api", "");

        this.route.params.subscribe(data => {
            this.oogstkaartservice.GetAcceptedRequest(data['id']).subscribe(data => {
                this.request = data;
                this.oogstkaartservice.getOogstkaartItem(data.oogstkaartID).subscribe(res => {
                    this.item = res;
                    this.oogstkaartservice.OpenRequest(this.item.oogstkaartItemID).subscribe(res =>  {

                    })
                })
            })
        })


    }

    showSuccess(message: string) {
        this.toastr.success('Succes', message);
    }

}
;