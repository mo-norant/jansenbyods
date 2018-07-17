import { AuthenticationService } from './../auth/_services/authentication.service';
import { Request, OogstKaartItem } from './../auth/_models/models';
import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Utils } from '../auth/_helpers/Utils';

@Injectable()
export class AdminService {

    constructor(private http: HttpClient, private _authservice : AuthenticationService) { }

    /**
     * getRequests
     */
    public getOogstkaartItems() {
        return this.http.get<OogstKaartItem[]>(Utils.getRoot() + 'admin/oogstkaart');
    }

    public getRequests(status: string) {

        return this.http.get<Request[]>(Utils.getRoot() + 'admin/requests?status=' + status);
    }

    public getRequest(id: number) {
        return this.http.get<Request>(Utils.getRoot() + 'admin/requests/' + id);
    }

    public getOogstkaartItem(id: number) {
        return this.http.get<OogstKaartItem>(Utils.getRoot() + 'admin/oogstkaart/' + id);

    }

    public changeStatus(id: number, status: string) {
        return this.http.post(Utils.getRoot() + 'admin/requests/update/' + id + '?status=' + status, null);

    }

    
    public GetAllUsers() {
        return this.http.get(Utils.getRoot() + 'admin/user/');

    }

    public GetUser(id: string){

        return this.http.get(Utils.getRoot() + 'admin/user/' + id  );

    }

    public DeleteUser(id : string){
        return this.http.post(Utils.getRoot() + 'Admin/delete/user/' + id , null );

    }

    
    public PostMessage(id : string, message : string, subject: string){

        let params : HttpParams = new HttpParams();
        params = params.append("subject", subject)
        params = params.append("message", message)
        return this.http.post(Utils.getRoot() + 'Admin/message/user/' + id , null, {params : params} );
    }

    public resetPassword(email :string){
            return this._authservice.passwordrequest(email)
    }

    public lockEnable(userid : string){
        return this.http.post<boolean>(Utils.getRoot() + 'Admin/lockenabled/' + userid , null );
    }

}
