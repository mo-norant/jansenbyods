import { Request, Company, Address, Message } from './../../Utils/Models/models';
import { OogstkaartService } from './../oogstkaart.service';
import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { FormGroup, FormBuilder, FormControl, Validators } from '@angular/forms';


@Component({
  selector: 'app-contact',
  templateUrl: './contact.component.html',
  styleUrls: ['./contact.component.css']
})
export class ContactComponent implements OnInit {

  request : Request;
  id: number;
  contactform: FormGroup;
  loading: boolean;

  constructor(private oogstkaarservice: OogstkaartService, private route: ActivatedRoute, private router: Router, private fb : FormBuilder) {

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
    });

    this.contactform = this.fb.group({
      name : new FormControl("", Validators.required),
      companyName : new FormControl(""),
      street : new FormControl("", Validators.required),
      number : new FormControl("", Validators.required),
      city : new FormControl("", Validators.required),
      zipcode : new FormControl("", Validators.required),
      email : new FormControl("", Validators.required),
      phone : new FormControl("", Validators.required),
      message : new FormControl("", Validators.required),

    })
  }


  postRequest(){
    this.loading = true;
    this.request.name = this.contactform.value.name;
    this.request.company.companyName = this.contactform.value.companyName;
    this.request.company.address.street = this.contactform.value.street;
    this.request.company.address.number = this.contactform.value.number;
    this.request.company.address.city = this.contactform.value.city;
    this.request.company.address.zipcode = this.contactform.value.zipcode;
    this.request.company.email = this.contactform.value.email;
    this.request.company.phone = this.contactform.value.phone;
    this.request.messages[0].messageString = this.contactform.value.message;

    this.oogstkaarservice.postRequest(this.id, this.request).subscribe(data => {
      this.router.navigate(['oogstkaart']);
      this.loading = false;
  }, err => {
    this.loading = false;
    alert("bericht werd niet correct verzonden.")
  });


  
  }
}
