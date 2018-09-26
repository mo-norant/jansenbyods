import { ConfirmationService } from 'primeng/api';
import { Router } from '@angular/router';
import { FAQService } from './../../../../../_services/FAQ.service';
import { Question, QuestionCategory } from './../../../../../auth/_models/models';
import { Component, OnInit } from '@angular/core';

@Component({
    selector: 'app-question',
    templateUrl: './question.component.html',
    styles: []
})
export class QuestionComponent implements OnInit {

    loading: boolean;

    question: Question;
    questioncategory: QuestionCategory;
    questioncategoryID: number;
    questioncategories: QuestionCategory[];

    constructor(private faq: FAQService, private router: Router, private dialogservice: ConfirmationService) {
        this.questioncategory = new QuestionCategory();
        this.question = new Question();


    }

    ngOnInit() {
        this.loading = true;
        this.faq.GetCategories().subscribe(res => {
            this.questioncategories = res;
            this.loading = false;

        }, err => {
            this.router.navigate(['admin/faq'])
        })
    }


    postCategory() {
        this.loading = true;
        this.faq.PostCategory(this.questioncategory).subscribe(res => {
            this.questioncategories = res;
            this.loading = false;
            this.router.navigate(['admin/faq'])

        }, err => {
            this.router.navigate(['admin/faq'])
        });

    }

    postQuestion() {
        this.loading = true;
        this.faq.PostQuestion(this.questioncategoryID, this.question).subscribe(res => {
            this.question = new Question();
            this.loading = false;
            this.router.navigate(['admin/faq'])

        }, err => {
            this.router.navigate(['admin/faq'])

        })
    }

}