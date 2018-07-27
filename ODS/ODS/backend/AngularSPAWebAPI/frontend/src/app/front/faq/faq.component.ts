import { QuestionCategory } from './../../Utils/Models/models';
import { HttpClient } from '@angular/common/http';
import { Component, OnInit } from '@angular/core';
import { Utils } from '../../Utils/Util';

@Component({
  selector: 'app-faq',
  templateUrl: './faq.component.html',
  styleUrls: ['./faq.component.css']
})
export class FaqComponent implements OnInit {

  qcs: QuestionCategory[];
  link = 'Faq/Question';
  loading: boolean;

  constructor(private http: HttpClient) { }

  ngOnInit() {
    this.loading = true;
    this.http.get<QuestionCategory[]>(Utils.getRoot() + this.link).subscribe(res => {
      this.qcs = res;
      this.loading = false;
    });
  }

}
