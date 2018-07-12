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
            this.oogstkaartservice.GetAcceptedRequest(data['id']).subscribe(res => {
                this.request = res;
                this.oogstkaartservice.getOogstkaartItem(this.request.oogstkaartID).subscribe(data => {
                    this.item = data;
                })
            });
        })


    }

    showSuccess(message: string) {
        this.toastr.success('Succes', message);
    }

}
;