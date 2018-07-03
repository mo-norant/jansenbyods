
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Utils } from '../auth/_helpers/Utils';
import { OogstKaartItem, Request } from '../auth/_models/models';

@Injectable()
export class OogstkaartService {

    link = 'Oogstkaart'


    constructor(private http: HttpClient) { }
    public GetOogstkaartitems() {
        return this.http.get<OogstKaartItem[]>(Utils.getRoot() + this.link);
    }

    public getOogstkaartItem(id: number) {
        return this.http.get<OogstKaartItem>(Utils.getRoot() + this.link + '/' + id);
    }


    public postOogstkaartItem(item: OogstKaartItem) {
        return this.http.post<number>(Utils.getRoot() + this.link, item);
    }


    public PostProductPhoto(file: File, id: number) {

        console.log(file)
        let formData: FormData = new FormData();
        formData.append('uploadFile', file);
        return this.http.post(Utils.getRoot() + "Oogstkaart/oogstkaartavatar/" + id, formData);
    }

    public PostPhotoGallery(files: File[], id: number) {
        console.log(files);
        let formData: FormData = new FormData();
        files.forEach(file => {
            formData.append('file', file);
        });
        return this.http.post(Utils.getRoot() + "Oogstkaart/gallery/" + id, formData);
    }

    public PostFiles(files: File[], id: number) {
        console.log(files);
        let formData: FormData = new FormData();
        files.forEach(file => {
            formData.append('fileUpload', file);
        });
        return this.http.post(Utils.getRoot() + "Oogstkaart/files/" + id, formData);
    }

    /**
    * DeleteItem
    */
    public DeleteItem(id: number) {
        return this.http.post<OogstKaartItem>(Utils.getRoot() + 'Oogstkaart/delete/' + id, {});
    }

    public UpdateOogstkaartitem(item: OogstKaartItem) {
        return this.http.post(Utils.getRoot() + "Oogstkaart/update", item)
    }

    public GetAcceptedRequests() {
        return this.http.get<Request[]>(Utils.getRoot() + "Oogstkaart/acceptedrequests");
    }
    public GetAcceptedRequest(id: number) {
        return this.http.get<Request>(Utils.getRoot() + "Oogstkaart/acceptedrequests/" + id);
    }

    public OpenRequest(id: number) {
        return this.http.post<Request>(Utils.getRoot() + "Oogstkaart/openrequest/" + id, null);
    }

    public GetNewRequests() {
        return this.http.get<number>(Utils.getRoot() + "Oogstkaart/openrequests")
    }
}
