import { Observable } from 'rxjs';
import { Router } from '@angular/router';
import { Component, OnInit } from '@angular/core';
import { AdminService } from '../../../../../_services/admin.service';
import { Request } from '../../../../../auth/_models/models';

@Component({
    selector: 'app-admin-aanvragen',
    templateUrl: './admin-aanvragen.component.html',
    styles: [`
    .declined {
        background-color: red !important;
        color: #ffffff !important;
    }

    .tobereviewed {
        background-color: orange !important;
        color: #ffffff !important;
    }

    .accepted {
        background-color: green !important;
        color: #ffffff !important;
    }
`
    ]
})
export class AdminAanvragenComponent implements OnInit {

    requests: Request[];
    cols;
    selecteditem: Request;
    loading : boolean;
    statusses;

    selectedstatus;


    constructor(private adminservice: AdminService, private router: Router) {

        this.cols = [
            { field: "requestID", header: "ID" },
            { field: "name", header: "Titel" },
            { field: "status", header: "Status" },
            { field: "create", header: "Aangevraag op" }

        ];

        this.statusses = [
            { label: 'Goedgekeurd', value: 'accepted' },
            { label: 'In review', value: 'tobereviewed' },
            { label: 'Afgekeurd', value: 'declined' },

        ];


    }

    ngOnInit() {
        
        this.loading = true;
        this.adminservice.getRequests('').subscribe(data => {
            data.forEach(i => {
                i.create = new Date(i.create).toLocaleString()
            });
            this.requests = data;
            this.requests.sort(i => i.create);
            this.requests.reverse();
            this.loading = false;


        }, err => {
            this.loading = false;

        });
    }

    reload(){
        this.loading = true;
        this.adminservice.getRequests('').subscribe(data => {
            if(data.length !== this.requests.length){
                data.forEach(i => {
                    i.create = new Date(i.create).toLocaleString()
                });
                this.requests = data;
                this.requests.sort(i => i.create);
                this.requests.reverse();
            }

            this.loading = false;
          
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
