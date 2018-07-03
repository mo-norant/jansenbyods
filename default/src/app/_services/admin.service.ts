import { Request, OogstKaartItem } from './../auth/_models/models';
import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Utils } from '../auth/_helpers/Utils';

@Injectable()
export class AdminService {

  constructor(private http: HttpClient) { }

  /**
   * getRequests
   */
  public getOogstkaartItems() {
    return this.http.get<OogstKaartItem[]>(Utils.getRoot() + 'admin/oogstkaart');
  }

  public getRequests(status: string){
   
    return this.http.get<Request[]>(Utils.getRoot() + 'admin/requests?status=' + status);
  }

  public getRequest(id: number){
    return this.http.get<Request>(Utils.getRoot() + 'admin/requests/' + id);
  }

  public getOogstkaartItem(id: number){
    return this.http.get<OogstKaartItem>(Utils.getRoot() + 'admin/oogstkaart/' + id);

  }

  public changeStatus(id: number, status: string){
    return this.http.post(Utils.getRoot() + 'admin/requests/update/' + id + '?status=' +  status, null);

  }

}
