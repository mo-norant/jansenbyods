import { QuestionCategory, Question } from './../../../../../auth/_models/models';
import { FAQService } from './../../../../../_services/FAQ.service';
import { Component, OnInit } from '@angular/core';

@Component({
    selector: 'app-questionslist',
    templateUrl: './questionslist.component.html',
    styles: []
})
export class QuestionslistComponent implements OnInit {

    selectedrowqcid: number;
    selectedrowqid: number;
    loading: boolean;
    questioncategories: QuestionCategory[];
    displayDialog: boolean;
    displayquestiondialog: boolean;
    questions: Question[] = [];
    colsqc = [
        { field: "title", header: "Vraagcategorie" },
    ];

    colsq = [
        { field: "_Question", header: "Vraag" },
        { field: "answer", header: "Antwoord" },
    ];

    constructor(private faq: FAQService) { }

    ngOnInit() {
        this.faq.GetCategories().subscribe(res => {
            this.questioncategories = [];
            this.questioncategories = res;
        });
        this.faq.GetQuestions().subscribe(res => {
            this.questions = [];
            res.forEach(e => {
                e.questions.forEach(q => {
                    this.questions.push(q);
                })
            });
        });
    }


    onRowSelect(event) {
        this.displayDialog = true;
        this.selectedrowqcid = event.data.questionCategoryID;
    }

    delete() {
        this.loading = true;
        this.faq.DeleteQuestionCategory(this.selectedrowqcid).subscribe(res => {
            this.ngOnInit();
            this.loading = false;
            this.selectedrowqcid = undefined;
            this.displayDialog = false;
        }, err => {

            this.loading = false;
            alert("Categorie is niet verwijderd.");
        });
    }

    onQuestionRowSelect(event) {
        this.displayquestiondialog = true;
        this.selectedrowqid = event.data.questionID;
    }

    deletequestion() {
        this.loading = true;
        this.faq.DeleteQuestion(this.selectedrowqid).subscribe(res => {
            this.ngOnInit();
            this.selectedrowqid = undefined;
            this.loading = false;
            this.displayquestiondialog = false;
        }, err => {
            this.loading = false;
            alert("Vraag is niet verwijderd.");
        });
    }

}
