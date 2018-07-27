import { OogstkaartService } from './../oogstkaart.service';
import { OogstKaartItem, Afbeelding } from './../../Utils/Models/models';
import { Component, OnInit } from '@angular/core';
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
