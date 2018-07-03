import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Utils } from '../Utils/Util';
import { OogstKaartItem, Request } from '../Utils/Models/models';

@Injectable()
export class OogstkaartService {

  link = 'Oogstkaart/mapview';


  constructor(private http: HttpClient,) { }



  public getOogstkaartItems(){
    return this.http.get<OogstKaartItem[]>(Utils.getRoot() + this.link)
  }

  public getItem(id : number){
    return this.http.get<OogstKaartItem>(Utils.getRoot()+ "Oogstkaart/mapview/" +id)
  }

  public postRequest(id: number, request: Request){
    return this.http.post(Utils.getRoot() + "Oogstkaart/request/" +id, request);
  }

  public postView(id: number){
    return this.http.post(Utils.getRoot()+ "Oogstkaart/view/" +id, {});
  }

  public getRelatedItems(id: number){
    return this.http.get<OogstKaartItem[]>(Utils.getRoot()+ "Oogstkaart/gerelateerdeproducten?oogstkaartid=" +id, {});
  }
}
