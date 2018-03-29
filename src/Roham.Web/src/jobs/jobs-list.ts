import {autoinject} from 'aurelia-framework';
import {HttpClient} from 'aurelia-http-client';
import {Router} from 'aurelia-router'
import {NotificationService} from './../services/NotificationService';
import {IJob, JobModel} from './../models/JobModel';

@autoinject
export class JobsList {
    private selJob: IJob;
    jobsGrid: any;

    constructor(private httpClient: HttpClient, private notification: NotificationService, private router: Router) {
    }

    get selectedJob() {
        return this.selJob;
    }

    set selectedJob(value: IJob) {
        this.selJob = value;
    }

    getJobs(gridArgs) {
        var self = this;
        return this.httpClient.get('api/job')
            .then(response => {
                var jobs: IJob[] = response.content;
                var jobsCount = jobs.length;
                if (jobsCount > 0) {
                    self.selectedJob = jobs[0];
                }
                return {
                    data: jobs,
                    count: jobsCount
                };
            });

    }

    public editJob = () => {
        var self = this;
        if (self.selectedJob != undefined) {
            var fragment = `edit/${self.selectedJob.id}`;
            this.router.navigate(fragment);
        }
    }
}