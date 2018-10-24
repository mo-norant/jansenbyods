import { OogstKaartItem } from "./../../Utils/Models/models";
import { OogstkaartService } from "./../oogstkaart.service";
import { Component, OnInit, ViewChild } from "@angular/core";
import { Utils } from "../../Utils/Util";
import { Router } from "@angular/router";
import { ScrollToService, ScrollToConfigOptions } from "@nicky-lenaers/ngx-scroll-to";


import * as _ from "lodash";

declare var MarkerClusterer:any;


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



  constructor(private service: OogstkaartService, private router : Router, private _scrollToService: ScrollToService) {}

  ngOnInit() {
    var mapProp = {
      center: new google.maps.LatLng(51.2627187555929, 4.5024117984374925),
      zoom: 7,
      mapTypeId: google.maps.MapTypeId.ROADMAP
    };
    this.map = new google.maps.Map(this.gmapElement.nativeElement, mapProp);

    this.service.getOogstkaartItems().subscribe(data => {
      this.items = _.uniqBy(data, "oogstkaartItemID").reverse();
      this.filtereditems = this.items;
      this.getcategories(this.items);
      this.setMarkers(this.items);
     });

    this.root = Utils.getRoot().replace("/api", "");
    window.scroll(0, 0);
    }

  mapview(toggle: boolean) {
    this.goToAppTop();
    this.isListview = toggle;
  }

  getcategories(items: OogstKaartItem[]) {
    items.forEach(item => {
      if (!this.categories.includes(item.category)) {
        this.categories.push(item.category);
      }
    });
  }
  sortCategory($event) {
      if (this.catsortmodel === 'alles') {
        this.filtereditems = this.items;
        this.setMarkers(this.items);

      } else {
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
        icon: "assets/img/markers/categories/" + element.category + ".png"

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

goToLegend(){
  const config: ScrollToConfigOptions = {
    target: 'legend'
  };

  this._scrollToService.scrollTo(config);
}

goToAppTop(){
  const config: ScrollToConfigOptions = {
    target: 'apptop'
  };
  this._scrollToService.scrollTo(config);

}

}
