import { Request, Company, Address, Message } from './../../Utils/Models/models';
import { OogstkaartService } from './../oogstkaart.service';
import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';

@Component({
  selector: 'app-contact',
  templateUrl: './contact.component.html',
  styleUrls: ['./contact.component.css']
})
export class ContactComponent implements OnInit {

  request : Request;
  id: number;

  constructor(private oogstkaarservice: OogstkaartService, private route: ActivatedRoute, private router: Router) {

    this.request = new Request();
    this.request.company = new Company();
    this.request.company.address = new Address();
    this.request.messages = [];
    this.request.messages.push(new Message());
   }

  ngOnInit() {
    this.route.params.subscribe(data => {
      if(data['id'] == undefined){
        this.router.navigate(['oogstkaart'])
      }
      this.id = data['id'];

      window.scrollTo(0,0);
    })
  }


  postRequest(){
    this.oogstkaarservice.postRequest(this.id, this.request).subscribe(data => {
        this.router.navigate(['oogstkaart']);
    });
  }
}
