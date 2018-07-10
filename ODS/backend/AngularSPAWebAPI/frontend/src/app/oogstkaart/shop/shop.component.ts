import { OogstKaartItem } from "./../../Utils/Models/models";
import { OogstkaartService } from "./../oogstkaart.service";
import { Component, OnInit, ViewChild } from "@angular/core";
import { Utils } from "../../Utils/Util";
import {} from "@types/googlemaps";
import { Router } from "@angular/router";
import * as _ from "lodash";

declare var google: any;
declare var MarkerClusterer: any;
@Component({
  selector: "app-shop",
  templateUrl: "./shop.component.html",
  styleUrls: ["./shop.component.css"]
})
export class ShopComponent implements OnInit {
  
  root: string;

  items: OogstKaartItem[];
  filtereditems: OogstKaartItem[];

  categories = [];
  catsortmodel = "alles";

  series = [];
  seriesortmodel = "alles";

  isListview: boolean;
  sorting = 'popularity';

  @ViewChild("gmap") gmapElement: any;
  map: google.maps.Map;


  constructor(private service: OogstkaartService, private router : Router) {}

  ngOnInit() {
    this.service.getOogstkaartItems().subscribe(data => {
      this.items = _.uniqBy(data, "oogstkaartItemID");
      this.filtereditems = this.items;
      this.getcategories(this.items);
      this.setMarkers(this.items);
     });

    this.root = Utils.getRoot().replace("/api", "");
    window.scroll(0, 0);
    }

  mapview(toggle: boolean) {
    this.isListview = toggle;
  }

  getcategories(items: OogstKaartItem[]) {
    items.forEach(item => {
      if (!this.categories.includes(item.category)) {
        this.categories.push(item.category);
      }
    });
  }


  sort($event){
    if(this.items.length > 0){
      if(this.sorting === "alphabetic"){
        this.items.sort((a: OogstKaartItem, b : OogstKaartItem) => {
          if(a.artikelnaam < b.artikelnaam) return -1;
          if(a.artikelnaam > b.artikelnaam) return 1;
          return 0
        });
      }
      else if(this.sorting === "popularity"){
        this.items.sort((a: OogstKaartItem, b : OogstKaartItem) => {
          return a.Views - b.Views;
        });
      }
      else if(this.sorting === "price"){
        this.items.sort((a: OogstKaartItem, b : OogstKaartItem) => {
          return a.vraagPrijsTotaal - b.vraagPrijsTotaal;
        });

        this.items.reverse();
      }
      else{
        this.items.sort((a: OogstKaartItem, b : OogstKaartItem) => {
          return this.getTime(a.createDate) - this.getTime(b.createDate);
        });
      }
    }

  }

  sortCategory($event){
      if(this.catsortmodel === "alles"){
        this.filtereditems = this.items;
        this.setMarkers(this.items);

      }
      else{
        this.filtereditems = this.items.filter(i => i.category === this.catsortmodel);
        this.setMarkers(this.filtereditems);
      }

  }




  setMarkers(oogstkaartmarkers : OogstKaartItem[]){
    var mapProp = {
      center: new google.maps.LatLng(51.2627187555929, 4.5024117984374925),
      zoom: 7,
      mapTypeId: google.maps.MapTypeId.ROADMAP
    };
    this.map = new google.maps.Map(this.gmapElement.nativeElement, mapProp);

    var markers = [];

    oogstkaartmarkers.forEach(element => {
      var marker = new google.maps.Marker({
        position: new google.maps.LatLng(
          element.location.latitude,
          element.location.longtitude
        ),
        animation:google.maps.Animation.DROP,
        icon: ""

      },
    );
    marker.addListener('click', function() {
      console.log(element.oogstkaartItemID);
      window.location.href='oogstkaart/'+element.oogstkaartItemID;
    });
      markers.push(marker);
    });

    var options = {
      imagePath: "assets/img/markers/"
    };

    //is ok
    var markerCluster = new MarkerClusterer(this.map, markers, options);

  }

  fillCategories(){

  }

  private getTime(date?: Date) {
    return date != null ? date.getTime() : 0;
}

}
