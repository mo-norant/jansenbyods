import { FormBuilder } from '@angular/forms';
import { FormGroup, Validators } from '@angular/forms';
import { ProductCategory } from './../models/productcategory';
import { Component, OnInit } from '@angular/core';
import { ProductconfiguratorService } from '../productconfigurator.service';

@Component({
  selector: 'app-productcategory',
  templateUrl: './productcategory.component.html',
  styles: []
})
export class ProductcategoryComponent implements OnInit {

  loading : boolean;
  pc: ProductCategory;

  pclist : ProductCategory[];

  pcForm : FormGroup;

  selecteditem : ProductCategory;

  cols = [
    { field: "productCategoryID", header: "ID" },
    { field: "name", header: "Naam" },
    { field: "description", header: "Omschrijving" }

];

  constructor(private fb : FormBuilder, private pService : ProductconfiguratorService) { }

  ngOnInit() {
    this.pcForm = this.fb.group({
      name: ['', [Validators.required, Validators.minLength(3)]],
      description: ['', [Validators.required, Validators.minLength(3)]],
  });

  this.pService.GetProductCategory().subscribe(res => {
    this.pclist = res;
    this.pclist.reverse();
  });

  }


  newCategory(){
      this.pc = this.pcForm.value;
      this.pService.PostProductCategory(this.pc).subscribe(res => {
        this.pclist.push(res);
        this.pclist.reverse();
        this.pc = null;
        this.pcForm.reset();
      })
  }

  onRowSelect(event) {
    console.log(event);
  }

}
