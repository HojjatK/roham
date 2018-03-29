import {autoinject} from 'aurelia-framework';
import {HttpClient} from 'aurelia-http-client';
import {Router} from 'aurelia-router'
import {NotificationService} from './../services/NotificationService';
import {IPost, PostModel} from './../models/PostModel';

@autoinject
export class PostsList {
    private selPost: IPost;
    postsGrid: any;

    constructor(private httpClient: HttpClient, private notification: NotificationService, private router: Router) {
    }

    get selectedPost() {
        return this.selPost;
    }

    set selectedPost(value: IPost) {
        this.selPost = value;
    }

    getPosts(gridArgs) {
        var self = this;
        return this.httpClient.get('api/post')
            .then(response => {
                var posts: IPost[] = response.content;
                var postsCount = posts.length;
                if (postsCount > 0) {
                    self.selectedPost = posts[0];
                }
                return {
                    data: posts,
                    count: postsCount
                };
            });
    }

    public editPost = () => {
        var self = this;
        if (self.selectedPost != undefined) {
            var fragment = `edit/${self.selectedPost.id}`;
            this.router.navigate(fragment);
        }
    }
}