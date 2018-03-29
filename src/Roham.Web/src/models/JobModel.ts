import {required, length} from 'aurelia-validatejs';

export interface IJob {
    uid: string,
    id: number,
    name: string,
    type: string,
    typeDescription: string,
    isSystemJob: boolean,
    description: string,
    created: Date,
    ownerUserId: number,
    ownerUser: string,
    siteId: number,
    siteTitle: string,
}

export class JobModel implements IJob {
    uid: string;
    id: number;

    @required
    @length({ maximum: 50 })
    name: string;

    type: string;
    typeDescription: string;
    isSystemJob: boolean;

    @length({ maximum: 150 })
    description: string;
    created: Date;
    ownerUserId: number;
    ownerUser: string;
    siteId: number;
    siteTitle: string;

    public fill = (source: IJob) : void => {
        this.uid = source.uid;
        this.id = source.id;
        this.name = source.name;
        this.type = source.type;
        this.typeDescription = source.typeDescription;
        this.isSystemJob = source.isSystemJob;
        this.description = source.description;
        this.created = source.created;
        this.ownerUserId = source.ownerUserId;
        this.ownerUser = source.ownerUser;
        this.siteId = source.siteId;
        this.siteTitle = source.siteTitle;
    }
}

export interface ITaskDetail {
    id: number,
    taskId: number,
    tryNo: number,
    arguments: string,
    status: string,
    started: Date,
    updated: Date,
    finished: Date,
    outputLog: string
}

export class TaskDetail implements ITaskDetail {
    id: number;
    taskId: number;
    tryNo: number;    
    arguments: string;
    status: string;
    started: Date;
    updated: Date;
    finished: Date;
    outputLog: string;

    public fill = (source: ITaskDetail): void => {
        this.id = source.id;
        this.taskId = source.taskId;
        this.tryNo = source.tryNo;
        this.arguments = source.arguments;
        this.status = source.status;
        this.started = source.started;
        this.updated = source.updated;
        this.finished = source.finished;
        this.outputLog = source.outputLog;
    }
}

export interface ITask {
    uid: string,
    id: number,
    jobId: number,
    jobName: string,
    name: string,
    status: string,
    progressEstimate: number,
    created: Date,
    completed: Date,
    failedMessage: string,
    ownerUserName: string,
    details: ITaskDetail[]
}

export class Task implements ITask {
    uid: string;
    id: number;
    jobId: number;
    jobName: string;
    name: string;
    status: string;
    progressEstimate: number;
    created: Date;
    completed: Date;
    failedMessage: string;
    ownerUserName: string;
    details: ITaskDetail[];

    public fill = (source: ITask): void => {
        this.uid = source.uid;
        this.id = source.id;
        this.jobId = source.jobId;
        this.jobName = source.jobName;
        this.name = source.name;
        this.status = source.status;
        this.progressEstimate = source.progressEstimate;
        this.created = source.created;
        this.completed = source.completed;
        this.failedMessage = source.failedMessage;
        this.ownerUserName = source.ownerUserName;
        
        if (source.details != undefined) {
            this.details = [];
            for (var d of source.details) {
                var newDetail = new TaskDetail();
                newDetail.fill(d);
                this.details.push(newDetail);
            }
        }
    }
}