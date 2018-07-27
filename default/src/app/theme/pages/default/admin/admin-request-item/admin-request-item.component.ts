import { Router } from '@angular/router';
import { ActivatedRoute } from '@angular/router';
import { OogstKaartItem, Request } from './../../../../../auth/_models/models';
import { AdminService } from './../../../../../_services/admin.service';
import { Component, OnInit } from '@angular/core';
import { Utils } from '../../../../../auth/_helpers/Utils';
import { ToastrService } from 'ngx-toastr';

@Component({
    selector: 'app-admin-request-item',
    templateUrl: './admin-request-item.component.html',
    styles: []
})
export class AdminRequestItemComponent implements OnInit {


    item: OogstKaartItem;
    request: Request;
    root: string;
    loading: boolean;

    constructor(private admin: AdminService, private route: ActivatedRoute, private toastr: ToastrService
    , private router: Router) { }

    ngOnInit() {
        this.loading = true;
        this.root = Utils.getRoot().replace("/api", "");

        this.route.params.subscribe(data => {
            this.admin.getRequest(data['id']).subscribe(data => {
                this.request = data;
                this.admin.getOogstkaartItem(data.oogstkaartID).subscribe(res => {
                    this.loading = false;
                    this.item = res;
                }, err=> {
                    this.showError("data niet geladen");
                    this.loading = false;
                    this.router.navigate(["admin"])
                })
            })
        })


    }

    showSuccess(message: string) {
        this.toastr.success('Succes', message);
    }

    showError(message: string){
        this.toastr.error('Error', message);
    }

    declineRequest() {
        this.loading = true;
        this.admin.changeStatus(this.request.requestID, "declined").subscribe(res => {
            this.request.status = res;
            this.loading = false;
            this.showSuccess("Aanvraag geweigerd");
        }, err => {
            this.showError("Aanvraag niet gewijzigd")
        });
    }

    acceptRequest() {
        this.loading = true;
        this.admin.changeStatus(this.request.requestID, "accepted").subscribe(res => {
            this.loading = false;
            this.request.status = res;
            this.showSuccess("Aanvraag geaccepteerd");
        }, err => {
            this.showError("Aanvraag niet gewijzigd")
        });
    }

    reviewRequest() {
        this.loading = true;
        this.admin.changeStatus(this.request.requestID, "tobereviewed").subscribe(res => {
            this.loading = false;
            this.request.status = res;
            this.showSuccess("Aanvraag in onderzoek");
        }, err => {
            this.showError("Aanvraag niet gewijzigd")
        });
    }

    email(){
        window.location.href = "mailto:" + this.request.company.email;
    }

    viewProduct(){
        var url = "http://jansenbyods.com/oogstkaart/" + this.item.oogstkaartItemID;
        var win = window.open(url, '_blank');
        win.focus();

    }

}
