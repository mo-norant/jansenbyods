import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { ProductCategory } from './models/productcategory';
import { Utils } from '../../../../../auth/_helpers/Utils';

@Injectable()
export class ProductconfiguratorService {

  private readonly link : string="ProductManager";

  constructor(private http : HttpClient) { }


  /**
   * PostProductCategory
   */
  public PostProductCategory(pc: ProductCategory) {
    return this.http.post<ProductCategory>(Utils.getRoot() + this.link + "/Productcategory", pc);
  }

  public GetProductCategory(){
    return this.http.get<ProductCategory[]>(Utils.getRoot() + this.link + "/Productcategory");
  }

}
