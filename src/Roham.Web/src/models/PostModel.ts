import {required, length} from 'aurelia-validatejs';

export interface IComment {
    uid: string,
    id: number,
    postId: number,
    postTitle: string,
    authorName: string,
    authorUrl: string,
    authorEmail: string,
    authorIp: string,
    body: string,
    posted: Date,
    status: string
}

export class CommentModel implements IComment {
    uid: string;
    id: number;
    postId: number;
    postTitle: string;

    @length({ maximum: 50 })
    authorName: string;

    @length({ maximum: 1024 })
    authorUrl: string;

    @length({ maximum: 150 })
    authorEmail: string;

    @length({ maximum: 50 })
    authorIp: string;

    @required
    @length({ minimum: 1 })
    body: string;

    posted: Date;

    status: string;

    public fill = (source: IComment): void => {
        this.uid = source.uid;
        this.id = source.id;
        this.authorName = source.authorName;
        this.authorUrl = source.authorUrl;
        this.authorEmail = source.authorEmail;
        this.authorIp = source.authorIp;
        this.body = source.body;
        this.posted = source.posted;
        this.status = source.status;
    }
}

export interface ILink {    
    type: string,
    ref: string,
}

export class LinkModel implements ILink {
    @required
    @length({ minimum: 1, maximum: 50 })
    type: string;

    @required
    @length({ minimum: 1, maximum: 1024 })
    ref: string;

    public fill = (source: ILink): void => {
        this.type = source.type;
        this.ref = source.ref;        
    }
}

export interface IPost {
    uid: string,
    id: number,
    revisionNumber: number,
    siteId: number,
    siteName: string,
    siteTitle: string,    
    zoneId: number,
    zoneName: string,
    zoneTitle: string,
    seriesId: number,
    seriesTitle: string,
    name: string,
    title: string,    
    uri: string,
    metaTitle: string,
    metaDescription: string,
    pageTemplate: string,
    author: string,
    isPrivate: boolean,
    commentsCount: number,
    isDiscussionEnabled: boolean,
    disableDiscussionDays: number,
    isRatingEnabled: boolean,
    isAnonymousCommentAllowed: boolean,
    rating: number,
    status: string,
    created: Date,
    published: Date,
    displayDate: Date,
    tagsCommaSeparated: string,
    categoriesCommaSeperated: string,
    conentSummary: string,
    content: string,
    comments: IComment[],
    links: ILink[]
}

export class PostModel implements IPost {
    uid: string;
    id: number;
    revisionNumber: number;
    siteId: number;
    siteName: string;
    siteTitle: string;
    zoneId: number;
    zoneName: string;
    zoneTitle: string;
    seriesId: number;
    seriesTitle: string;

    @required
    @length({ minimum: 2, maximum: 50 })
    name: string;

    @required
    @length({ minimum: 2, maximum: 50 })
    title: string;

    uri: string;

    @length({ maximum: 50 })
    metaTitle: string;

    metaDescription: string;

    pageTemplate: string;

    author: string;

    isPrivate: boolean;

    commentsCount: number;

    isDiscussionEnabled: boolean;

    disableDiscussionDays: number;

    isRatingEnabled: boolean;
    
    isAnonymousCommentAllowed: boolean;

    rating: number;

    status: string;

    created: Date;

    published: Date;

    displayDate: Date;

    tagsCommaSeparated: string;

    categoriesCommaSeperated: string;

    conentSummary: string;

    content: string;

    isPersisted: boolean;

    comments: IComment[];

    links: ILink[];

    public fill = (source: IPost): void => {
        this.uid = source.uid;
        this.id = source.id;
        this.revisionNumber = source.revisionNumber;
        this.siteId = source.siteId;
        this.siteName = source.siteName;
        this.siteTitle = source.siteTitle;
        this.zoneId = source.zoneId;
        this.zoneName = source.zoneName;
        this.zoneTitle = source.zoneTitle;
        this.seriesId = source.seriesId;
        this.seriesTitle = source.seriesTitle;
        this.name = source.name;
        this.title = source.title;
        this.metaTitle = source.metaTitle;
        this.metaDescription = source.metaDescription;
        this.pageTemplate = source.pageTemplate;
        this.author = source.author;
        this.isPrivate = source.isPrivate;
        this.commentsCount = source.commentsCount;
        this.isDiscussionEnabled = source.isDiscussionEnabled;
        this.disableDiscussionDays = source.disableDiscussionDays;
        this.isRatingEnabled = source.isRatingEnabled;
        this.isAnonymousCommentAllowed = source.isAnonymousCommentAllowed;
        this.rating = source.rating;
        this.status = source.status;
        this.created = source.created;
        this.published = source.published;
        this.displayDate = source.displayDate;
        this.tagsCommaSeparated = source.tagsCommaSeparated;
        this.categoriesCommaSeperated = source.categoriesCommaSeperated;
        this.conentSummary = source.conentSummary;
        this.content = source.content;
        this.isPersisted = source.id > 0;

        this.comments = [];
        if (source.comments != undefined) {
            for (var comment of source.comments) {
                var newComment = new CommentModel();
                newComment.fill(comment);
                this.comments.push(newComment);
            }
        }

        this.links = [];
        if (source.links != undefined) {
            for (var link of source.links) {
                var newLink = new LinkModel();
                newLink.fill(newLink);
                this.links.push(newLink);
            }
        }
    }
}

export interface IPostSerie {
    uid: string,
    id: number,
    siteId: number,
    siteTitle: string,
    name:string,
    title: string,
    description: string,
    isPrivate: boolean
}

export class PostSerieModel implements IPostSerie {
    uid: string;
    id: number;
    siteId: number;
    siteTitle: string;

    @required
    @length({ minimum: 2, maximum: 50 })
    name: string;

    @required
    @length({ minimum: 2, maximum: 50 })
    title: string;

    @length({ maximum: 255 })
    description: string;

    isPrivate: boolean;

    public fill = (source: IPostSerie): void => {
        this.uid = source.uid;
        this.id = source.id;
        this.title = source.title;
        this.description = source.description;
        this.isPrivate = source.isPrivate;
    }
}
