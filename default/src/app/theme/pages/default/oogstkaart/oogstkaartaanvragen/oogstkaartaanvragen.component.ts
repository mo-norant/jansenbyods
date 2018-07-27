import { Router } from '@angular/router';
import { Request } from './../../../../../auth/_models/models';
import { Component, OnInit } from '@angular/core';
import { OogstkaartService } from '../../../../../_services/oogstkaart.service';

@Component({
    selector: 'app-oogstkaartaanvragen',
    templateUrl: './oogstkaartaanvragen.component.html',
    styles: []
})
export class OogstkaartaanvragenComponent implements OnInit {

    requests: Request[];
    cols;
    selecteditem: Request;
    loading : boolean;


    constructor(private oogstkaartservice: OogstkaartService, private router: Router) {

        this.cols = [
            { field: "requestID", header: "ID" },
            { field: "name", header: "Titel" },
            { field: "status", header: "Status" },
            { field: "create", header: "Aangevraag op" }
        ];


    }

    ngOnInit() {
        this.oogstkaartservice.GetAcceptedRequests().subscribe(res => {

            res.forEach(i => {
                i.create = new Date(i.create).toLocaleString()
            });

            this.requests = res;
            this.requests.sort(i => i.create);
            this.requests.reverse();
        })
    }

    onRowSelect(event) {
        this.router.navigate(['oogstkaart/aanvragen', event.data.requestID]);
    }


    reload(){
        this.loading = true;
         this.oogstkaartservice.GetAcceptedRequests().subscribe(data => {
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

}
