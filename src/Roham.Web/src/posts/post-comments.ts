import {autoinject} from 'aurelia-framework';
import {HttpClient} from 'aurelia-http-client';
import {ValidationController} from 'aurelia-validation';
import {NotificationService} from './../services/NotificationService';
import {NavigationService} from './../services/NavigationService';
import {IComment, CommentModel} from './../models/PostModel';
import {ISite} from './../models/SiteModel';
import {IZone} from './../models/ZoneModel';
import {IResult} from './../models/ResultModel';
import {CaptureModel} from './../capture-model';

@autoinject
export class PostComments {
    private selComment: IComment;
    commentsGrid: any;

    constructor(private controller: ValidationController, private httpClient: HttpClient, notification: NotificationService, navService: NavigationService) {
    }

    get selectedComment() {
        return this.selComment;
    }

    set selectedComment(value: IComment) {
        this.selComment = value;
    }

    getComments(gridArgs) {
        var self = this;
        return this.httpClient.get('api/post/comment')
            .then(response => {
                var comments: IComment[] = response.content;
                var commentsCount = comments.length;
                if (commentsCount > 0) {
                    self.selectedComment = comments[0];
                }
                return {
                    data: comments,
                    count: commentsCount
                };
            });
    }
}