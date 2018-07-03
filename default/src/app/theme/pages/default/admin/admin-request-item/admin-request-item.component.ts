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
  

  constructor(private admin : AdminService, private route : ActivatedRoute,     private toastr: ToastrService
  ) { }

  ngOnInit() {
    this.root = Utils.getRoot().replace("/api", "");

    this.route.params.subscribe( data => {
      this.admin.getRequest(data['id']).subscribe(data =>{
        this.request = data;
        this.admin.getOogstkaartItem(data.oogstkaartID).subscribe(res => {
          this.item = res;
          this.showSuccess("Aanvraag en product geladen");
        })
      })
    })

    
  }

  showSuccess(message: string) {
    this.toastr.success('Succes', message);
  }

  declineRequest(){
    this.admin.changeStatus(this.request.requestID, "declined").subscribe(res => {
        this.showSuccess("Aanvraag geweigerd");
    });
  }

  acceptRequest(){
    this.admin.changeStatus(this.request.requestID, "accepted").subscribe(res => {
      this.showSuccess("Aanvraag geaccepteerd");
  });
  }

  reviewRequest(){
    this.admin.changeStatus(this.request.requestID, "tobereviewed").subscribe(res => {
      this.showSuccess("Aanvraag in onderzoek");
  });
  }

}
