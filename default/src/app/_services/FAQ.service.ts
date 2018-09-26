import { QuestionCategory, Question } from './../auth/_models/models';
import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Utils } from '../auth/_helpers/Utils';

@Injectable()
export class FAQService {

    link = 'FAQ'


    constructor(private http: HttpClient) { }
    public PostCategory(qc: QuestionCategory) {
        return this.http.post<QuestionCategory[]>(Utils.getRoot() + this.link + '/category', qc);
    }
    public GetCategories() {
        return this.http.get<QuestionCategory[]>(Utils.getRoot() + this.link + '/QuestionsCategories');
    }

    public PostQuestion(questioncategoryid: number, q: Question) {
        return this.http.post(Utils.getRoot() + this.link + '/Question/' + questioncategoryid, q);
    }

    public DeleteQuestionCategory(qcid: number) {
        return this.http.post(Utils.getRoot() + this.link + '/delete/questioncategory/' + qcid, null);
    }

    public DeleteQuestion(qid: number) {
        return this.http.post(Utils.getRoot() + this.link + '/delete/question/' + qid, null);
    }

    public GetQuestions() {
        return this.http.get<QuestionCategory[]>(Utils.getRoot() + this.link + '/Question');
    }
}
