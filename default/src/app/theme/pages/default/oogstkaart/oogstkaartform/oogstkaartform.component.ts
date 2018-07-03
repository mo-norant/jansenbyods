import { OogstkaartService } from "./../../../../../_services/oogstkaart.service";
import {
    OogstKaartItem,
    LocationOogstKaartItem,
    Specificatie
} from "./../../../../../auth/_models/models";
import { Component, OnInit, Inject } from "@angular/core";
import { ScriptLoaderService } from "../../../../../_services/script-loader.service";
import { DOCUMENT } from "@angular/platform-browser";
import { Router } from "@angular/router";
import { ToastrService } from "ngx-toastr";
import { FormGroup, FormBuilder, Validators, FormControl } from "@angular/forms";



declare var google: any;

@Component({
    selector: "app-oogstkaartform",
    templateUrl: "./oogstkaartform.component.html",
    styles: []
})
export class OogstkaartformComponent implements OnInit {
    item: OogstKaartItem = new OogstKaartItem();
    firstform: FormGroup;
    today: Date;
    wizardcounter: number = 0;
    options: any;
    specificaties: Specificatie[] = [];
    uploadloading: boolean;
    selectedlocation: LocationOogstKaartItem;
    returnedid: number;

    files = {
        mainpicture: [],
        supportfiles: [],
        photogallery: []
    };




    constructor(
        private _script: ScriptLoaderService,
        @Inject(DOCUMENT) private document: Document,
        private oogstkaartservice: OogstkaartService,
        private router: Router,
        private toastr: ToastrService,
        private fb: FormBuilder
    ) {
        this.today = new Date();


    }

    ngOnInit() {
        this.options = {
            center: { lat: 51.2627187555929, lng: 4.5024117984374925 },
            zoom: 7
        };


        this.firstform = this.fb.group({
            artikelnaam: new FormControl("", Validators.required),
            category: new FormControl("", Validators.required),
            omschrijving: new FormControl(""),
            jansenserie: new FormControl("", Validators.required),
            vraagPrijsPerEenheid: new FormControl(""),
            vraagPrijsTotaal: new FormControl("", [Validators.required]),
            hoeveelheid: new FormControl("", [Validators.required]),
            transportInbegrepen: new FormControl("", Validators.required),
            concept: new FormControl("", Validators.required),
            datumBeschikbaar: new FormControl("", Validators.required),
        });
        window.scrollTo(0, 0);

    }

    next() {

        if (this.wizardcounter + 1 == 2) {
            this.postItem();
        }
        else {
            this.wizardcounter++;
        }

        if (this.wizardcounter == 3) {
            this.router.navigate(['oogstkaart']);
        }

        window.scrollTo(0, 0);

    }

    setLocation($event) {
        if (this.selectedlocation == null) {
            this.selectedlocation = new LocationOogstKaartItem();
        }

        this.selectedlocation.latitude = $event.coords.lat;
        this.selectedlocation.longtitude = $event.coords.lng;
    }

    addSpecificatie() {

        let spec = new Specificatie();
        this.specificaties.push(spec);
    }

    removeItem(index) {
        if (index > -1) {
            this.specificaties.splice(index, 1);
        }
    }

    onMainPhotoUpload(event) {
        this.uploadloading = true
        this.oogstkaartservice.PostProductPhoto(event.files[0], this.returnedid).subscribe(i => {
            this.showSuccess("Hoofdfoto werd succesvol geupload")
            this.uploadloading = false;
        }, err => {
            this.uploadloading = false
        });

    }

    onSupportFiles(event) {
        this.uploadloading = true
        this.files.supportfiles = [];
        for (let file in event.files) {
            this.files.supportfiles.push(file);
        }
        this.oogstkaartservice.PostFiles(event.files, this.returnedid).subscribe(i => {
            this.showSuccess("Uw bestanden werd succesvol geupload")
            this.uploadloading = false;
        }, err => {
            this.uploadloading = false
        });
    }
    onPhotoGalleryUpload(event) {
        this.uploadloading = true
        this.files.photogallery = [];
        for (let file in event.files) {
            this.files.photogallery.push(file);
        }
        this.oogstkaartservice.PostPhotoGallery(event.files, this.returnedid).subscribe(i => {
            this.showSuccess("Fotogallerij werd succesvol geupload");
            this.uploadloading = false;
        }, err => {
            this.uploadloading = false
        });
    }

    postItem() {
        if (!this.uploadloading) {
            this.uploadloading = true;
            console.log(this.firstform.value);
            //cast to number
            this.firstform.value.vraagPrijsPerEenheid = + this.firstform.value.vraagPrijsPerEenheid;
            this.firstform.value.hoeveelheid = + this.firstform.value.hoeveelheid;
            this.firstform.value.vraagPrijsTotaal = + this.firstform.value.vraagPrijsTotaal;
            this.item = this.firstform.value;
            this.item.location = this.selectedlocation;
            this.item.specificaties = this.specificaties;
            this.oogstkaartservice.postOogstkaartItem(this.item).subscribe(data => {
                this.returnedid = data;
                this.wizardcounter++;
                this.uploadloading = false;
            }, err => {
                this.uploadloading = false;
                this.wizardcounter = 1;
            });
        }

    }

    back() {
        this.wizardcounter = 0
        window.scrollTo(0, 0);

    }
    showSuccess(message: string) {
        this.toastr.success('Succes', message);
    }
}
