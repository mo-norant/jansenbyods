import { Observable } from 'rxjs';
import { Router } from '@angular/router';
import { Component, OnInit } from '@angular/core';
import { AdminService } from '../../../../../_services/admin.service';
import { Request } from '../../../../../auth/_models/models';

@Component({
    selector: 'app-admin-aanvragen',
    templateUrl: './admin-aanvragen.component.html',
    styles: []
})
export class AdminAanvragenComponent implements OnInit {

    requests: Request[];
    cols;
    selecteditem: Request;
    loading : boolean;
    statusses;


    constructor(private adminservice: AdminService, private router: Router) {

        this.cols = [
            { field: "requestID", header: "ID" },
            { field: "name", header: "Titel" },
            { field: "status", header: "Status" },
            { field: "create", header: "Aangevraag op" }

        ];

        this.statusses = [
            { label: 'Alles', value: 'all' },
            { label: 'In review', value: 'tobereviewed' },
            { label: 'Afgekeurd', value: 'declined' },
            { label: 'Goedgekeurd', value: 'accepted' },

        ];


    }

    ngOnInit() {
        
        this.loading = true;
        this.adminservice.getRequests('').subscribe(data => {
            data.forEach(i => {
                i.create = new Date(i.create).toLocaleString()
            });
            this.requests = data;
            this.loading = false;

            //refresh voor nieuwe aanvragen
        Observable.interval(10000).takeWhile(() => true).subscribe(() => {
            this.adminservice.getRequests('').subscribe(data => {
                if(data.length !== this.requests.length){
                    data.forEach(i => {
                        i.create = new Date(i.create).toLocaleString()
                    });
                    this.requests = data;
                }
              
            }, err => {
                this.loading = false;
    
            });
        });

        }, err => {
            this.loading = false;

        });
    }

    onRowSelect(event) {
        this.router.navigate(['admin/aanvragen', event.data.requestID]);
    }


    filter($event) {
        this.loading = true;
        this.adminservice.getRequests($event.value).subscribe(data => {
            data.forEach(i => {
                i.create = new Date(i.create).toLocaleString()
            });
            this.requests = data;
            this.loading = false;

        },err => {
            alert("Data niet geladen");
            this.loading = false;
        })
    }

    


}
