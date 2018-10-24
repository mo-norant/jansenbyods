import { OogstkaartService } from './../oogstkaart.service';
import { OogstKaartItem, Afbeelding } from './../../Utils/Models/models';
import { Component, OnInit, ViewChild } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { Utils } from '../../Utils/Util';


@Component({
  selector: 'app-item',
  templateUrl: './item.component.html',
  styleUrls: ['./item.component.css']
})
export class ItemComponent implements OnInit {

  item: OogstKaartItem;
  relateditems: OogstKaartItem[];
  root: string;

  loading: boolean;
  images: Afbeelding[] = [];

  @ViewChild("gmap") gmapElement: any;
  map: google.maps.Map;

  constructor(private route: ActivatedRoute, private service: OogstkaartService, private router: Router) { }

  ngOnInit() {

    this.root = Utils.getRoot().replace('/api', '');

    this.loading = true;
    this.route.params.subscribe(data => {
      this.service.getItem(data['id']).subscribe( res => {

        if (res.specificaties === undefined) {
          res.specificaties = [];
        }
        window.scrollTo(0, 0);

        this.item = res;
        this.setMarker(res);
        this.loading = false;
        this.item.gallery.forEach(element => {
          this.images.push(element);
        });

        this.createView();
        this.getRelatedItems(res.oogstkaartItemID);

      }, err => {
        this.router.navigate(['oogstkaart']);
      });
    });

  }

  setMarker(item : OogstKaartItem){
  
    var temploc = new google.maps.LatLng(item.location.latitude, item.location.longtitude);
    var mapProp = {
      center:temploc,
      zoom: 7,
      mapTypeId: google.maps.MapTypeId.ROADMAP
    };
    this.map = new google.maps.Map(this.gmapElement.nativeElement, mapProp);



        var marker = new google.maps.Marker({
          position: temploc,
          map: this.map,
          title: item.artikelnaam
        });
  }

  goToItem() {
    this.router.navigate(['oogstkaart/contact/' + this.item.oogstkaartItemID]);
  }

  createView() {
    this.service.postView(this.item.oogstkaartItemID).subscribe();
  }


  getRelatedItems(id: number) {
      this.service.getRelatedItems(id).subscribe(data => {
        this.relateditems = data;

      });
  }

}
