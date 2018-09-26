import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'app-form',
  templateUrl: './form.component.html',
  styles: []
})
export class FormComponent implements OnInit {

  loading : boolean;
  type : string;
  title : string;


  constructor() { }

  ngOnInit() {
  }


  simpleProduct(){
    this.type = "simple"
    this.title = "Enkel subproduct"
  }

  unitProduct(){
    this.type = "unit"
    this.title = "Speciaal subproduct"

  }

}

export class SingleProduct {
  ProductPrice : number;
  Name :string;
  Description : string;
}

export class UnitProduct implements SingleProduct{
  ProductPrice : number;
  Name :string;
  Description : string;
  UnitCall : string;
  UnitMetric : string;
  UnitValue : number;
}