import { element } from 'protractor';
import { OogstkaartService } from './../oogstkaart.service';
import { OogstKaartItem } from './../../Utils/Models/models';
import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { Utils } from '../../Utils/Util';
import { NgxGalleryAnimation } from 'ngx-gallery';
import { GALLERY_IMAGE } from 'ngx-image-gallery';
import { GALLERY_CONF } from 'ngx-image-gallery';

@Component({
  selector: 'app-item',
  templateUrl: './item.component.html',
  styleUrls: ['./item.component.css']
})
export class ItemComponent implements OnInit {

  item: OogstKaartItem;
  relateditems: OogstKaartItem[];
  root: string;


  images: Array<string>;



  constructor(private route: ActivatedRoute, private service : OogstkaartService, private router: Router) { }

  ngOnInit() {

    this.root = Utils.getRoot().replace("/api", "");


    this.route.params.subscribe(data => {
      this.service.getItem(data['id']).subscribe(data => {

        if(data.specificaties == undefined){
          data.specificaties = [];
        }
        window.scrollTo(0,0);

        this.item = data;

        this.item.gallery.forEach(element => {
        });

        this.createView();
        this.getRelatedItems(data.oogstkaartItemID);

      }, err => {
        this.router.navigate(['oogstkaart']);
      })
    });

  }

  goToItem(){
    this.router.navigate(['oogstkaart/contact/' + this.item.oogstkaartItemID]);
  }

  createView(){
    this.service.postView(this.item.oogstkaartItemID).subscribe();
  }


  getRelatedItems(item: number){
      this.service.getRelatedItems(item).subscribe(data => {
        this.relateditems = data;

      });
  }

}
