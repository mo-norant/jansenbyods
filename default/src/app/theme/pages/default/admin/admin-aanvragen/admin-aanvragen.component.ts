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

  requests : Request[];
  cols;
  selecteditem : Request;

  statusses;


  constructor(private adminservice: AdminService, private router: Router) {

    this.cols = [
      { field: "requestID", header: "ID" },
      { field: "name", header: "Titel" },
      { field: "status", header: "Status" },
      { field: "create", header: "Aangevraag op" }

    ];

    this.statusses   = [
      { label: 'Alles', value: 'all' },
      { label: 'In review', value: 'tobereviewed' },
      { label: 'Afgekeurd', value: 'declined' },
      { label: 'Goedgekeurd', value: 'accepted' },
  
  ];
   }

  ngOnInit(){
    this.adminservice.getRequests('').subscribe(data => {
      data.forEach(i => {
        i.create = new Date(i.create).toLocaleString()
      });
      this.requests = data;
    });
  }  

  onRowSelect(event) {
    console.log(event)
    this.router.navigate(['admin/aanvragen', event.data.requestID]);
  }


  filter($event){
    console.log($event.value);
   this.adminservice.getRequests($event.value).subscribe(data => {
    data.forEach(i => {
      i.create = new Date(i.create).toLocaleString()
    });
    this.requests = data;
   })
  }
  

}
